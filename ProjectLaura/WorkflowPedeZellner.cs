using System;
using CRFBase;
using CRFBase.GradientDescent;
using CRFToolAppBase;
using CodeBase;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPIBase;
using System.Threading;
using System.IO;

namespace ProjectLaura

{
    class WorkflowPedeZellner
    {
        //private const string RandomlySelectedPDBFile = @"../../Data/hermannData/" + RandomlySelectedPDBFileName + "_A.txt";
        //private const string RandomlySelectedPDBFileName = "1a00";
        //private const int NumberOfGraphInstances = 100;
        //private const int NumberOfSeedsForPatchCreation = 2;
        private const int IsingConformityParameter = 1;
        private const int IsingCorrelationParameter = 1;
        private const int NumberOfLabels = 2;
        private const int BufferSizeViterbi = 1000;
        private static Random rdm = new Random();
        
        private static readonly string fileFolder = @"../../Data/hermannData";
        private static readonly string fileNames = @"../../Data/myFiles.txt";
        public static string InterfaceDefLocation = @"../../Data/nuss.iface.2012.txt";
        private static string RandomlySelectedPDBFile;
        private static string RandomlySelectedPDBFileName;


        public static void Main(string[] args)
        {
            // manager erzeugen
            new ConsoleLogger();
            new FileLogger("Log/");
            new RasaManager("../../Data/RASA/", @"../../Data/hermannData/");
            new PDBFileManager(@"../../Data/hermannData/");

            Console.WriteLine("Hello");
            startTrainingCycle();
            Console.WriteLine("Worked");

            BaseProgram.Exit.Enter();
        }

        private static void listFiles() {
            foreach (String name in File.ReadLines(fileNames))
            {
                Console.WriteLine(fileFolder +"/"+ name +" : "+ name.Substring(0, 4));
            }
        }

        private static void startTrainingCycle()
        {
            // erzeugt die RequestListener
            CRFToolApp.Build.Do();
            CRFBase.Build.Do();

            // starting of TrainingCycle:
            var trainingCycle = new TrainingEvaluationCycle();
            var parameters = new TrainingEvaluationCycleInputParameters();

            // take OLM variants we want to test, ISING and OLM_III (Default)
            List<OLMVariant> variants = new List<OLMVariant>();
            variants.Add(OLMVariant.Ising);
            variants.Add(OLMVariant.Default);

            // TODO setting of transition probabilities
            double[,] transition = setTransitionProbabilities();

            #region modify graph
            // do this for all graphs (currently saved in form of pdbfiles)
            foreach (String file in File.ReadLines(fileNames))
            {
                RandomlySelectedPDBFile = fileFolder + "/" + file;
                RandomlySelectedPDBFileName = file.Substring(0, 4);

                var pdbFile = PDBExt.Parse(RandomlySelectedPDBFile);
                pdbFile.Name = RandomlySelectedPDBFileName;

                // set IsCore nodes, cuz we only need the interface nodes (the core nodes cannot interact)
                var proteinGraph = setIsCore(pdbFile);  

                // trim graph -> remove isCore nodes
                var trimmedGraph = trimGraph(pdbFile, proteinGraph);

                // set real reference label of the graph
                var crfGraph = setReferenceLabel(trimmedGraph);
            }
            #endregion

            #region old
            // setting the MaximumTotalPatchSize based on the ratio calculated with the hermann data
            //int min = (int)(crfGraph.NodeCounter * averageRatioItoS[1]);
            //int max = (int)(crfGraph.NodeCounter * averageRatioItoS[2]);
            //int av = (int)(crfGraph.NodeCounter * averageRatioItoS[0]);
            //int MaximumTotalPatchSize = rdm.Next(min, max);
            #endregion

            // set parameters for the training cycle
            // TODO parameters for seeding not needed anymore
            /*parameters = new TrainingEvaluationCycleInputParameters(crfGraphList, NumberOfGraphInstances, variants,
                IsingConformityParameter, IsingCorrelationParameter, transition, NumberOfLabels, BufferSizeViterbi);
                */
            // running the cycle
            trainingCycle.RunCycle(parameters);
        }

        private static GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> setReferenceLabel(ProteinGraph trimmedGraph)
        {
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

            //test
            /*if (GraphVisualization)
            {
                var graph3D = trimmedGraph.Wrap3D(nd => new Node3DWrap<ResidueNodeData>(nd.Data) { ReferenceLabel = nd.Data.ReferenceLabel, X = nd.Data.X, Y = nd.Data.Y, Z = nd.Data.Z });
                new ShowGraph3D(graph3D).Request(RequestRunType.Background);
                Thread.Sleep(120000);
            }*/

            var crfGraph = trimmedGraph.Convert<ResidueNodeData, CRFNodeData, SimpleEdgeData, CRFEdgeData,
                ProteinGraphData, CRFGraphData>(nd => new CRFNodeData(nd.Data.Residue.Id) { X = nd.Data.X, Y = nd.Data.Y, Z = nd.Data.Z }, ed => new CRFEdgeData(),
                gd => new CRFGraphData());
            crfGraph.SaveAsJSON("testGraph.txt");

            return crfGraph;
        }

        private static ProteinGraph setIsCore(PDBFile pdbFile)
        {
            var rasaRequest = new RequestRasa(pdbFile);
            rasaRequest.RequestInDefaultContext();

            foreach (var rasa in rasaRequest.Rasavalues)
            {
                rasa.Key.IsCore = rasa.Value <= 0.15;
            }
            var proteinGraph = CreateProteinGraph.Do(pdbFile, 7.0, ResidueNodeCenterDefinition.CAlpha).Values.First();
            return proteinGraph;
        }

        private static double[,] setTransitionProbabilities()
        {
            var a = 0.85;
            var b = 0.85;
            double[,] transition = new double[2, 2] { { a, 1 - a }, { 1 - b, b } };
            return transition;
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
    }
}
