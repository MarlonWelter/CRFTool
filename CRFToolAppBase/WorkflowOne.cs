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

        public void Execute()
        {
            //            -Prerequisites:
            //	-Graphstructure
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

            //-Step 1:

            //   -discretize characteristics
            #region Use Training Pede
            {
                var isingConformityParameter = 0.5;
                var isingCorrelationParameter = 0.5;
                var isingModel = new IsingModel(isingConformityParameter, isingCorrelationParameter);
                var request = new OLMRequest(OLMVariant.Ising, testGraphs);
                request.BasisMerkmale = new BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>[]
                    { new IsingMerkmalNode(), new IsingMerkmalEdge() };
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


                // update Ising parameters in IsingModel
                isingModel.ConformityParameter = olmResult.ResultingWeights[0];
                isingModel.CorrelationParameter = olmResult.ResultingWeights[1];

                // zugehörige Scores erzeugen für jeden Graphen (auch Evaluation)
                //foreach (var graph in graphList)
                //{
                //    isingModel.CreateCRFScore(graph);
                //}
            }
            #endregion

            //- Step2:

            //   -Create ROC Curve
            //   - Give Maximum with Viterbi
            //   - Give Sample with MCMC
        }
    }
    public class WorkflowOneDataset
    {
        public string Folder { get; set; }
    }
}
