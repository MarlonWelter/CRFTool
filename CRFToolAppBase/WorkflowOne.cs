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
        public string GraphDataFolder { get; set; }
        public int NumberIntervals { get; set; } = 4;

        public void Execute()
        {
            //    - Prerequisites:
            //	  - Graphstructure
            //    - Training Data
            //    - 2 Node Classifications
            var testGraphs = new List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>();

            //    -n characteristics for each node
            {
                var request = new UserInput();
                request.TextForUser = "Please select the folder with your graph training data.";
                request.Request();
                GraphDataFolder = request.UserText;

                foreach (var file in Directory.EnumerateFiles(GraphDataFolder))
                {
                    var graph = JSONX.LoadFromJSON<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>(file);
                    testGraphs.Add(graph);
                }
            }

            //   - Step 1:

            //   - discretize characteristics
            #region Use Training Pede
            {
                // create features
                var features = CreateFeatures(testGraphs);

                var request = new OLMRequest(OLMVariant.Ising, testGraphs);
                request.BasisMerkmale = features.ToArray();

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

                var olmResult = request.Result;

                // zugehörige Scores erzeugen für jeden Graphen (auch Evaluation)
                foreach (var graph in testGraphs)
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
            #endregion

            //- Step2:

            //   -Create ROC Curve
            //   - Give Maximum with Viterbi
            //   - Give Sample with MCMC
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
    }
}
