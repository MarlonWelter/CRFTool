using CodeBase;
using CRFBase;
using CRFBase.OLM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase
{
    public class WorkflowOne
    {
        public WorkflowOneDataset Dataset { get; set; } = new WorkflowOneDataset();
        public string GraphDataFolder { get; set; }
        public int NumberIntervals { get; set; } = 4;

        public List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> TrainingData { get; set; }
        public List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> EvaluationData { get; set; }

        public void Execute()
        {
            //    - Prerequisites:
            //	  - Graphstructure
            //    - Training Data
            //    - 2 Node Classifications
            TrainingData = new List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>();
            EvaluationData = new List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>();

            // decision wether user wants to train or load pre-trained data
            var requestTraining = new UserDecision("Use Training.", "Load Training Result.");
            requestTraining.Request();

            if (requestTraining.Decision == 0) // use training
            {
                //    -n characteristics for each node
                {
                    var request = new UserInput(UserInputLookFor.Folder);
                    request.DefaultPath = "..\\..\\CRFToolApp\\bin\\Graphs";
                    request.TextForUser = "Please select the folder with your graph training data.";
                    request.Request();
                    GraphDataFolder = request.UserText;

                    foreach (var file in Directory.EnumerateFiles(GraphDataFolder))
                    {
                        var graph = JSONX.LoadFromJSON<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>(file);
                        TrainingData.Add(graph);
                    }
                }

                //   - Step 1:

                //   - discretize characteristics
                #region Use Training Pede
                {
                    // create features
                    CreateFeatures(Dataset, TrainingData);
                    InitCRFScores(TrainingData);

                    var request = new OLMRequest(OLMVariant.Ising, TrainingData);
                    request.BasisMerkmale.AddRange(Dataset.NodeFeatures);
                    request.BasisMerkmale.AddRange(Dataset.EdgeFeatures);
                    
                    request.LossFunctionValidation = OLM.LossRatio;

                    request.Request();

                    // zugehörige Scores erzeugen für jeden Graphen (auch Evaluation)
                    CreateCRFScores(TrainingData, Dataset.NodeFeatures, request.Result.ResultingWeights);

                    // store trained Weights
                    Dataset.NumberIntervals = NumberIntervals;
                    Dataset.Characteristics = TrainingData.First().Data.Characteristics.ToArray();
                    Dataset.EdgeCharacteristic = "IsingEdgeCharacteristic";
                    Dataset.Weights = request.Result.ResultingWeights;
                    Dataset.SaveAsJSON("results.json");
                }

                #endregion
            }
            else
            { // load pre-trained data
                var request = new UserInput();
                request.TextForUser = "Please select the training result file.";
                request.Request();
                var file = request.UserText;
                var trainingResult = JSONX.LoadFromJSON<WorkflowOneDataset>(file);
                Dataset = trainingResult;
            }
            //- Step2:

            // User Choice here
            {
                var request = new UserInput(UserInputLookFor.Folder);
                request.TextForUser = "Please select the folder with your graph evaluation data.";
                request.Request();
                GraphDataFolder = request.UserText;

                foreach (var file in Directory.EnumerateFiles(GraphDataFolder))
                {
                    var graph = JSONX.LoadFromJSON<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>(file);
                    EvaluationData.Add(graph);
                }
            }


            //scores erzeugen
            CreateCRFScores(EvaluationData, Dataset.NodeFeatures, Dataset.Weights);

            //   - Create ROC Curve
            {
            }
            //   - Give Maximum with Viterbi
            {
                foreach (var graph in EvaluationData)
                {
                    var request = new SolveInference(graph, null, 2);
                    request.Request();
                    graph.Data.AssginedLabeling = request.Solution.Labeling;
                }

                //show results in 3D Viewer
                {
                    var request = new ShowGraphs();
                    request.Graphs = EvaluationData;
                    request.Request();
                }
            }
            //   - Give Sample with MCMC
            {

                foreach (var graph in EvaluationData)
                {
                    SoftwareGraphLearningParameters parameters = new SoftwareGraphLearningParameters();
                    parameters.NumberOfGraphs = 60;
                    parameters.NumberNodes = 50;
                    parameters.NumberLabels = 2;
                    parameters.NumberCategories = 4;
                    parameters.IntraConnectivityDegree = 0.15;
                    parameters.InterConnectivityDegree = 0.01;


                    //sample parameters
                    var samplerParameters = new MHSamplerParameters();
                    var sglGraph = graph.Convert((nodeData) => new SGLNodeData() { }, (edgeData) => new SGLEdgeData(), (graphData) => new SGLGraphData());

                    samplerParameters.Graph = sglGraph;
                    samplerParameters.NumberChains = 1;

                    //sampler starten
                    var gibbsSampler = new MHSampler();
                    gibbsSampler.Do(samplerParameters);

                }
            }
        }

        private void CreateCRFScores(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> data, List<CharacteristicFeature> nodefeatures, double[] weights)
        {
            foreach (var graph in data)
            {
                foreach (var node in graph.Nodes)
                {
                    node.Data.Scores = new double[2];
                    for (int c = 0; c < graph.Data.Characteristics.Length; c++)
                    {
                        for (int i = 0; i < NumberIntervals; i++)
                        {
                            node.Data.Scores[0] += nodefeatures[c * NumberIntervals + i].Score(node, 0) * weights[c * NumberIntervals + i];
                            node.Data.Scores[1] += nodefeatures[c * NumberIntervals + i].Score(node, 1) * weights[c * NumberIntervals + i];
                        }
                    }
                }
                var correlationParameter = weights.Last();
                foreach (var edge in graph.Edges)
                {
                    edge.Data.Scores = new double[2, 2] { { correlationParameter, -correlationParameter }, { -correlationParameter, correlationParameter } };
                }
            }
        }
        private void InitCRFScores(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> data)
        {
            var random = new Random();
            foreach (var graph in data)
            {
                foreach (var node in graph.Nodes)
                {
                    node.Data.Scores = new double[2];
                    node.Data.Scores[0] = random.NextDouble() - 0.5;
                    node.Data.Scores[1] = random.NextDouble() - 0.5;

                }
                var correlationParameter = 0.1;
                foreach (var edge in graph.Edges)
                {
                    edge.Data.Scores = new double[2, 2] { { correlationParameter, -correlationParameter }, { -correlationParameter, correlationParameter } };
                }
            }
        }

        private void CreateFeatures(WorkflowOneDataset dataset, List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> testGraphs)
        {

            // node-features
            var allNodes = testGraphs.SelectMany(graph => graph.Nodes).ToList();
            for (int characteristic = 0; characteristic < testGraphs.First().Data.Characteristics.Length; characteristic++)
            {
                var orderedNodes = allNodes.OrderBy(n => n.Data.Characteristics[characteristic]).ToList();
                var intervals = orderedNodes.SplitToIntervals(NumberIntervals);
                for (int i = 0; i < NumberIntervals; i++)
                {
                    dataset.NodeFeatures.Add(new CharacteristicFeature(characteristic, 0, i > 0 ? intervals[i - 1].Last().Data.Characteristics[characteristic] : double.MinValue, i < NumberIntervals - 1 ? intervals[i].Last().Data.Characteristics[characteristic] : double.MaxValue));

                    dataset.NodeFeatures.Add(new CharacteristicFeature(characteristic, 1, i > 0 ? intervals[i - 1].Last().Data.Characteristics[characteristic] : double.MinValue, i < NumberIntervals - 1 ? intervals[i].Last().Data.Characteristics[characteristic] : double.MaxValue));
                }
            }

            // edge-features
            dataset.EdgeFeatures.Add(new IsingMerkmalEdge());

        }
    }
    public class WorkflowOneDataset
    {
        public List<CharacteristicFeature> NodeFeatures { get; set; } = new List<CharacteristicFeature>();
        public List<IsingMerkmalEdge> EdgeFeatures { get; set; } = new List<IsingMerkmalEdge>();

        public int NumberIntervals { get; set; }
        public string[] Characteristics { get; set; }
        public string EdgeCharacteristic { get; set; }
        public double[] Weights { get; set; }
    }
}
