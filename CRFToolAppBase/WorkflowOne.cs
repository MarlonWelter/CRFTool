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

            //    -n characteristics for each node
            {
                var request = new UserInput();
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
                Dataset.Features = CreateFeatures(TrainingData);

                var request = new OLMRequest(OLMVariant.Ising, TrainingData);
                request.BasisMerkmale = Dataset.Features.ToArray();

                //TODO: loss function auslagern
                request.LossFunctionValidation = (a, b) =>
                {
                    var loss = 0.0;
                    for (int i = 0; i < a.Length; i++)
                    {
                        loss += a[i] != b[i] ? 1 : 0;
                    }
                    return loss / a.Length;
                };

                request.Request();

                Dataset.TrainingResult = request.Result;

                // zugehörige Scores erzeugen für jeden Graphen (auch Evaluation)
                CreateCRFScores(TrainingData, Dataset.Features, Dataset.TrainingResult);

                // store trained Weights
                var results = new UserTrainingWorkflowOne();
                results.NumberIntervals = NumberIntervals;
                results.Characteristics = TrainingData.First().Data.Characteristics.ToArray();
                results.EdgeCharacteristic = "IsingEdgeCharacteristic";
                results.Weights = Dataset.TrainingResult.ResultingWeights;
                results.SaveAsJSON("results.json");
            }

            #endregion

            //- Step2:

            // User Choice here
            {
                var request = new UserInput();
                request.TextForUser = "Please select the folder with your graph evaluation data.";
                request.Request();
                GraphDataFolder = request.UserText;

                foreach (var file in Directory.EnumerateFiles(GraphDataFolder))
                {
                    var graph = JSONX.LoadFromJSON<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>(file);
                    EvaluationData.Add(graph);
                }
            }

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
                    Dataset.GraphDatas.Add(graph.Data);
                }
            }
            //   - Give Sample with MCMC
            {
                //scores erzeugen
                CreateCRFScores(EvaluationData, Dataset.Features, Dataset.TrainingResult);

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

        private void CreateCRFScores(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> trainingData, List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> features, OLMRequestResult olmResult)
        {
            foreach (var graph in TrainingData)
            {
                foreach (var node in graph.Nodes)
                {
                    node.Data.Scores = new double[2];
                    for (int c = 0; c < graph.Data.Characteristics.Length; c++)
                    {
                        for (int i = 0; i < NumberIntervals; i++)
                        {
                            node.Data.Scores[0] += features[c * NumberIntervals + i].Score(node, 0) * olmResult.ResultingWeights[c * NumberIntervals + i];
                            node.Data.Scores[1] += features[c * NumberIntervals + i].Score(node, 1) * olmResult.ResultingWeights[c * NumberIntervals + i];
                        }
                    }
                }
                var correlationParameter = olmResult.ResultingWeights.Last();
                foreach (var edge in graph.Edges)
                {
                    edge.Data.Scores = new double[2, 2] { { correlationParameter, -correlationParameter }, { -correlationParameter, correlationParameter } };
                }
            }
        }

        private List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> CreateFeatures(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> testGraphs)
        {
            var features = new List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();

            // node-features
            var allNodes = testGraphs.SelectMany(graph => graph.Nodes).ToList();
            for (int characteristic = 0; characteristic < testGraphs.First().Data.Characteristics.Length; characteristic++)
            {
                var orderedNodes = allNodes.OrderBy(n => n.Data.Characteristics[characteristic]).ToList();
                var intervals = orderedNodes.SplitToIntervals(NumberIntervals);
                for (int i = 0; i < NumberIntervals; i++)
                {
                    features.Add(new CharacteristicFeature(characteristic, 0, i > double.MinValue ? intervals[i - 1].Last().Data.Characteristics[characteristic] : 0, i < NumberIntervals - 1 ? intervals[i].Last().Data.Characteristics[characteristic] : double.MaxValue));

                    features.Add(new CharacteristicFeature(characteristic, 1, i > double.MinValue ? intervals[i - 1].Last().Data.Characteristics[characteristic] : 0, i < NumberIntervals - 1 ? intervals[i].Last().Data.Characteristics[characteristic] : double.MaxValue));
                }
            }

            // edge-features
            features.Add(new IsingMerkmalEdge());

            return features;
        }
    }
    public class WorkflowOneDataset
    {
        public string Folder { get; set; }

        public List<CRFGraphData> GraphDatas { get; set; }

        public List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> Features { get; set; }
        public OLMRequestResult TrainingResult { get; set; }
    }
}
