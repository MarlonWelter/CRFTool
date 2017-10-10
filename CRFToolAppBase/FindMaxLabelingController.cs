using CRFBase;
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase
{
    class FindMaxLabelingController
    {
        public void FindMaxLabellingCommand(CRFToolData crfToolData)
        {
            var request = new SolveInference(null, null, 0);
            determineSettings(request, crfToolData);
            request.RequestInDefaultContext();
        }

        private void determineSettings(SolveInference request, CRFToolData crfToolData)
        {
            // request settings
            //default:
            request.Labels = 2;
            request.Graph = crfToolData.SelectedGraph;
        }
    }
}
