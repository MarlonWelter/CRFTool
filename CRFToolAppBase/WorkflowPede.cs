using CodeBase;
using CRFBase;
using PPIBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CRFToolAppBase
{
    public class WorkflowPede
    {
        private const string RandomlySelectedPDBFile = @"../../Data/hermannData/"+ RandomlySelectedPDBFileName+"_A.txt";
        private const string RandomlySelectedPDBFileName = "1a00";
        private const int NumberOfGraphInstances = 100;
        private const int NumberOfSeedsForPatchCreation = 2;
        private const int IsingConformityParameter = 1;
        private const int IsingCorrelationParameter = 1;
        private const int NumberOfLabels = 2;
        private const int BufferSizeViterbi = 1000;
        private static Random rdm = new Random();

        private static int clones = 12;
        private static readonly string fileFolder = @"../../Data/hermannData";
        public static string InterfaceDefLocation = @"../../Data/nuss.iface.2012.txt";

        // just need to be done once, is stored in a file named terminationCondition.txt 
        public const bool ComputeTerminationCondition = true;

        // false = true labeling, true = computed labeling
        public const bool GraphVisualization = false;

        // allgemein: 0: nicht-interface ; 1: interface
        public static void Start(string[] args)
        {
            // manager erzeugen
            new ConsoleLogger();
            new FileLogger("Log/");
            new RasaManager("../../Data/RASA/", @"../../Data/hermannData/");
            new PDBFileManager(@"../../Data/hermannData/");

            startTrainingCycle();

            BaseProgram.Exit.Enter();
        }

        private static void startTrainingCycle()
        {
            // erzeugt die RequestListener
            Build.Do();
            CRFBase.Build.Do();


            // calculation of the termination condition for the seeding method
            double[] averageRatioItoS = calculateTerminationConditionForSeeding();

            // starting of TrainingCycle:
            var trainingCycle = new TrainingEvaluationCycle();
            var parameters = new TrainingEvaluationCycleInputParameters();

            // take OLM variants we want to test, ISING and OLM_III (Default)
            List<OLMVariant> variants = new List<OLMVariant>();
            variants.Add(OLMVariant.Ising);
            variants.Add(OLMVariant.Default);

            // setting of transition probabilities
            //var a = 0.5 + 0.15 * rdm.NextDouble();
            var a = 0.85;
            var b = 0.85;
            double[,] transition = new double[2, 2] { { a, 1 - a }, { 1 - b, b } };

            #region modify graph
            var pdbFile = PDBExt.Parse(RandomlySelectedPDBFile);
                pdbFile.Name = RandomlySelectedPDBFileName;

                // setzen der IsCore
                var rasaRequest = new RequestRasa(pdbFile);
                rasaRequest.RequestInDefaultContext();

                foreach (var rasa in rasaRequest.Rasavalues)
                {
                    rasa.Key.IsCore = rasa.Value <= 0.15;
                }
                var proteinGraph = CreateProteinGraph.Do(pdbFile, 7.0, ResidueNodeCenterDefinition.CAlpha).Values.First();

                // graph trimmen
                var trimmedGraph = trimGraph(pdbFile, proteinGraph);

                // set real reference label of the graph
                var nameWithChain = RandomlySelectedPDBFile.Substring(fileFolder.Length + 1, 6);
                var interfacesList = new List<string>();
                using (var reader = new StreamReader(InterfaceDefLocation))
                {
                    var line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        var nuss = nameWithChain;
                        if (line.StartsWith(nameWithChain))
                        {
                            interfacesList.Add(line.Substring(8));
                        }
                    }
                }
                foreach (var interfaceEntry in interfacesList)
                {
                    var node = trimmedGraph.Nodes.FirstOrDefault(n => n.Data.Residue.Id.Equals(interfaceEntry));
                    if (node != null)
                        node.Data.ReferenceLabel = 1;
                }
            #endregion

            //test
            if (GraphVisualization)
            {
                var graph3D = trimmedGraph.Wrap3D(nd => new Node3DWrap<ResidueNodeData>(nd.Data) { ReferenceLabel = nd.Data.ReferenceLabel, X = nd.Data.X, Y = nd.Data.Y, Z = nd.Data.Z });
                new ShowGraph3D(graph3D).Request(RequestRunType.Background);
                Thread.Sleep(120000);
            }

            var crfGraph = trimmedGraph.Convert<ResidueNodeData, CRFNodeData, SimpleEdgeData, CRFEdgeData,
                ProteinGraphData, CRFGraphData>(nd => new CRFNodeData(nd.Data.Residue.Id) { X = nd.Data.X, Y = nd.Data.Y, Z = nd.Data.Z }, ed => new CRFEdgeData(),
                gd => new CRFGraphData());
            crfGraph.SaveAsJSON("testGraph.txt");

            // setting the MaximumTotalPatchSize based on the ratio calculated with the hermann data
            int min = (int)(crfGraph.NodeCounter * averageRatioItoS[1]);
            int max = (int)(crfGraph.NodeCounter * averageRatioItoS[2]);
            int av = (int)(crfGraph.NodeCounter * averageRatioItoS[0]);
            int MaximumTotalPatchSize = rdm.Next(min, max);

            // set parameters for the training cycle
            parameters = new TrainingEvaluationCycleInputParameters(crfGraph, NumberOfGraphInstances,
                NumberOfSeedsForPatchCreation, MaximumTotalPatchSize, variants,
                IsingConformityParameter, IsingCorrelationParameter, transition, NumberOfLabels, BufferSizeViterbi);
            // running the cycle
            trainingCycle.RunCycle(parameters);
        }

        private static ProteinGraph trimGraph(PDBFile pdbFile, ProteinGraph proteinGraph)
        {
            var trimmedGraph = new ProteinGraph();
            Dictionary<GWNode<ResidueNodeData, SimpleEdgeData, ProteinGraphData>, GWNode<ResidueNodeData, SimpleEdgeData, ProteinGraphData>> dict = new Dictionary<GWNode<ResidueNodeData, SimpleEdgeData, ProteinGraphData>, GWNode<ResidueNodeData, SimpleEdgeData, ProteinGraphData>>();
            foreach (var node in proteinGraph.Nodes)
            {
                if (!node.Data.IsCore)
                {
                    var newNode = trimmedGraph.CreateNode();
                    newNode.Data = node.Data;
                    dict.Add(node, newNode);
                }
                else
                {
                }
            }

            foreach (var edge in proteinGraph.Edges)
            {
                if (!edge.Head.Data.IsCore && !edge.Foot.Data.IsCore)
                {
                    var newEdge = trimmedGraph.CreateEdge(dict[edge.Head], dict[edge.Foot]);
                    newEdge.Data = edge.Data;
                }
            }

            return trimmedGraph;
        }

        private static double[] calculateTerminationConditionForSeeding()
        {
            double[] averageRatioItoS = new double[3];
            if (ComputeTerminationCondition)
            {

                List<double> ratios = new List<double>();

                // Getting the ratio of all surface nodes to the interface nodes
                // to get a reality near parameter when to stop the seeding
                foreach (var file in Directory.GetFiles(fileFolder))
                {
                    var pdbFile = PDBExt.Parse(file);
                    pdbFile.Name = file.Substring(fileFolder.Length + 1, 4);
                    var nameWithChain = file.Substring(fileFolder.Length + 1, 6);

                    // counting all the nodes
                    var rasaRequest = new RequestRasa(pdbFile);
                    rasaRequest.RequestInDefaultContext();

                    // counting all the surface nodes
                    int SurfaceCounter = 0;
                    foreach (var rasa in rasaRequest.Rasavalues)
                    {
                        SurfaceCounter += rasa.Value > 0.15 ? 1 : 0;
                        rasa.Key.IsCore = rasa.Value <= 0.15;
                    }

                    // counting the interface nodes for each file
                    int InterfaceCounter = 0;
                    using (var reader = new StreamReader(InterfaceDefLocation))
                    {
                        var line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            var nuss = nameWithChain;
                            InterfaceCounter += line.StartsWith(nameWithChain) ? 1 : 0;
                        }
                    }

                    double ratio = 1.0 * InterfaceCounter / SurfaceCounter;
                    ratios.Add(ratio);
                }

                // calculation of the average ratio between surface and interface nodes
                double ratioSum = 0.0;
                for (int i = 0; i < ratios.Count; i++)
                {
                    ratioSum += ratios[i];
                }
                double max = ratios.Max();
                double min = ratios.Min();
                averageRatioItoS[0] = ratioSum / ratios.Count;
                averageRatioItoS[1] = min;
                averageRatioItoS[2] = max;

                averageRatioItoS.SaveAsJSON("terminationCondition.txt");
            }
            else
            {
                averageRatioItoS = JSONX.LoadFromJSON<double[]>("terminationCondition.txt");
            }

            return averageRatioItoS;
        }

        private static void startTestCycle()
        {
            // erzeugt die RequestListener
            Build.Do();

            // einfache Graphen erzeugen (12 clone)
            //var graph = CreateGraphs.createGraph_Grid2D();
            var pdbFile = PDBExt.Parse(RandomlySelectedPDBFile);
            var proteinGraph = CreateProteinGraph.Do(pdbFile, 6.0, ResidueNodeCenterDefinition.CAlpha).Values.First();
            IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> crfGraph = proteinGraph.Convert(nd => new CRFNodeData(nd.Data.Residue.Id), ed => new CRFEdgeData(), gd => new CRFGraphData());

            // seeding

            // 12 clone erzeugen
            var clone = new GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>[clones];

            for (int i = 0; i < clones; i++)
            {
                var cloneGraph = crfGraph.Clone(ConvertData.convertNodeData, ConvertData.convertEdgeData, ConvertData.convertGraphData);

                // für jeden graphen (clone): Erzeugung eines Labelings (rdm) => als int[]
                Labeling.randomLabeling(cloneGraph);
                Labeling.assignedLabeling(cloneGraph);

                clone[i] = cloneGraph;

            }

            // calculation of tp, fp, tn, fn, sensitivity, specificity, mcc
            double[][] keysForAllClones = new double[clones][];
            for (int i = 0; i < clones; i++)
            {
                double[] keys = Compute.computeKeys(crfGraph, clone, i);
                keysForAllClones[i] = keys;
            }

            // test
            //outputKeys(keysForAllClones);
            outputLabelingsScores(clone);
        }


        public static void outputKeys(double[][] keysForAllClones)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter
               ("ergebnisse.txt");

            string[] s = new string[clones];
            string head = "tp\ttn\tfp\tfn\tsen\tspe\tmcc";
            Console.WriteLine(head);
            file.WriteLine(head);

            for (int i = 0; i < clones; i++)
            {
                for (int j = 0; j < keysForAllClones[i].Length; j++)
                {
                    Console.Write(Math.Round(keysForAllClones[i][j], 2) + "\t");
                    s[i] += Math.Round(keysForAllClones[i][j], 2);
                    s[i] += "\t";
                }
                s[i] += "\n";
                file.WriteLine(s[i]);
                Console.WriteLine();
            }
            file.Close();
        }

        private static void outputLabelingsScores(GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>[] clone)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter
               ("labelingAndScores.txt");

            string[] s = new string[clones];
            string head = "ReferenceLabeling\tAssignedLabeling\tNodeScores\tEdgeScores";
            Console.WriteLine(head);
            file.WriteLine(head);

            // for each graph print out the RL and AL
            for (int i = 0; i < clones; i++)
            {
                // Reference Labeling
                for (int j = 0; j < clone[i].Data.ReferenceLabeling.Count(); j++)
                {
                    Console.Write(clone[i].Data.ReferenceLabeling[j] + " ");
                }
                Console.Write("\t");
                // AssignedLabeling
                for (int j = 0; j < clone[i].Data.ReferenceLabeling.Count(); j++)
                {
                    Console.Write(clone[i].Data.AssginedLabeling[j] + " ");
                }
                Console.Write("\t");

                // TODO: print out the scores of edges and nodes of each graph
                for (int j = 0; j < clone[i].Nodes.Count(); j++)
                {
                    // Node scores
                    for (int k = 0; k < clone[i].Nodes.ElementAt(j).Data.Scores.Count(); k++)
                    {
                        Console.Write(clone[i].Nodes.ElementAt(j).Data.Scores[k] + "\t");
                        s[i] += clone[i].Nodes.ElementAt(j).Data.Scores[k] + "\t";
                    }
                    // Edge scores
                    Console.Write(clone[i].Nodes.ElementAt(j).Data.Scores + "\t");
                    s[i] += clone[i].Nodes.ElementAt(j).Data.Scores + "\t";
                    s[i] += "\n";
                    file.WriteLine(s[i]);
                    Console.WriteLine();
                }

                // TODO: print out the scores of edges and nodes of each graph
                //Console.Write(clone[i].Nodes.ToString() + "\t");
                //s[i] += clone[i].Nodes.ToString() + "\t";
                //Console.Write(clone[i].Edges.ToString() + "\t");
                //s[i] += clone[i].Edges.ToString() + "\t";
                //s[i] += "\n";
                //file.WriteLine(s[i]);
            }
            file.Close();
        }
    }
}
