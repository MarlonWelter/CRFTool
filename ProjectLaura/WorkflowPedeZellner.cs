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
        private const int NumberOfIntervals = 5;
        private static double[] PottsConformityParameters = new double[NumberOfIntervals*2];
       
        private const int IsingConformityParameter = 1;
        private const int IsingCorrelationParameter = 1;
        private const int NumberOfLabels = 2;
        private const int BufferSizeViterbi = 1000;
        private static Random rdm = new Random();
        
        private static readonly string fileFolder = @"../../Data/ArtificialValue";
        private static readonly string fileNames = @"../../Data/myFiles.txt";
        public static string InterfaceDefLocation = @"../../Data/nuss.iface.2012.txt";
        private static string RandomlySelectedPDBFile;
        private static string RandomlySelectedPDBFileName;

        public static void Main(string[] args)
        {
            // manager erzeugen
            new ConsoleLogger();
            new FileLogger("Log/");
            new RasaManager("../../Data/RASA/", @"../../Data/ArtificialValue/");
            new PDBFileManager(@"../../Data/ArtificialValue/");

            //for(int i=0; i<20; i++)
            //{
            Log.Post("Begin");
            StartTrainingCycle();
            Log.Post("End");
            //}
            Console.ReadKey();

            BaseProgram.Exit.Enter();
        }

        private static void ListFiles() {
            foreach (String name in File.ReadLines(fileNames))
            {
                Console.WriteLine(fileFolder +"/"+ name +" : "+ name.Substring(0, 4));
            }
        }

        private static void StartTrainingCycle()
        {
            // erzeugt die RequestListener
            CRFToolApp.Build.Do();
            CRFBase.Build.Do();

            // initialize conformity parameters
            var ini = 1;
            for (int i = 0; i < PottsConformityParameters.Length; i++)
            {
                PottsConformityParameters[i] = ini;
                ini *= -1;
            }

            // starting of TrainingCycle:
            var trainingCycle = new TrainingEvaluationCycleZellner();
            var parameters = new TrainingEvaluationCycleInputParameters();           

            // take OLM variants we want to test, ISING and OLM_III (Default)
            List<OLMVariant> variants = new List<OLMVariant>
            {
                OLMVariant.Ising
                //OLMVariant.Default
            };            

            // setting of transition probabilities to create observation from reference labeling
            double[,] transition = SetTransitionProbabilities();

            #region modify graph
            // do this for all graphs (currently saved in form of pdbfiles)
            List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> crfGraphList = new List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>();
            var id = 0;

            foreach (String file in File.ReadLines(fileNames))
            {
                RandomlySelectedPDBFile = fileFolder + "/" + file;
                RandomlySelectedPDBFileName = file.Substring(0, 4);

                var pdbFile = PDBExt.Parse(RandomlySelectedPDBFile);
                pdbFile.Name = RandomlySelectedPDBFileName;

                // set IsCore nodes, cuz we only need the interface nodes (the core nodes cannot interact)
                var proteinGraph = SetIsCore(pdbFile);  

                // trim graph -> remove isCore nodes
                var trimmedGraph = TrimGraph(pdbFile, proteinGraph);

                // set real reference label of the graph
                var crfGraph = SetReferenceLabel(trimmedGraph);
                crfGraph.Id = id++;
                crfGraphList.Add(crfGraph);
            }
            #endregion      

            // set parameters for the training cycle
            parameters = new TrainingEvaluationCycleInputParameters(crfGraphList, crfGraphList.Count, variants,
                IsingConformityParameter, PottsConformityParameters, IsingCorrelationParameter, NumberOfIntervals, transition, NumberOfLabels, BufferSizeViterbi);

            // running the cycle
            trainingCycle.RunCycle(parameters);
        }

        private static GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> SetReferenceLabel(ProteinGraph trimmedGraph)
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
                        interfacesList.Add(line.Substring(7));
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
            //if (GraphVisualization)
            //{
            //    var graph3D = trimmedGraph.Wrap3D(nd => new Node3DWrap<ResidueNodeData>(nd.Data) { ReferenceLabel = nd.Data.ReferenceLabel, X = nd.Data.X, Y = nd.Data.Y, Z = nd.Data.Z });
            //    new ShowGraph3D(graph3D).Request(RequestRunType.Background);
            //    Thread.Sleep(120000);
            //}

            var crfGraph = trimmedGraph.Convert<ResidueNodeData, CRFNodeData, SimpleEdgeData, CRFEdgeData,
                ProteinGraphData, CRFGraphData>(nd => new CRFNodeData(nd.Data.Residue.Id) { X = nd.Data.X, Y = nd.Data.Y, Z = nd.Data.Z, ReferenceLabel = nd.Data.ReferenceLabel, Characteristics = new double[] { nd.Data.ZScore } }, ed => new CRFEdgeData(),
                gd => new CRFGraphData());
            //crfGraph.SaveAsJSON("testGraph.txt");

            return crfGraph;
        }

        private static ProteinGraph SetIsCore(PDBFile pdbFile)
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

        private static double[,] SetTransitionProbabilities()
        {
            var a = 0.85;
            var b = 0.85;
            double[,] transition = new double[2, 2] { { a, 1 - a }, { 1 - b, b } };
            return transition;
        }

        private static ProteinGraph TrimGraph(PDBFile pdbFile, ProteinGraph proteinGraph)
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
