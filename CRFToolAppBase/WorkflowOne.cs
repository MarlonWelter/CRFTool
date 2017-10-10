using CodeBase;
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

            //    -n characteristics for each node
            var request = new UserInput();
            request.TextForUser = "Please select the folder with your graph training data.";
            request.Request();
            GraphDataFolder = request.UserText;

            foreach (var file in Directory.EnumerateFiles(GraphDataFolder))
            {
                var graph = JSONX.LoadFromJSON<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>(file);
            }


            //-Step 1:

            //   -discretize characteristics
            //   - Use Training Pede

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
