using CRFBase.GibbsSampling;
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRFBase.OLM;

namespace CRFBase
{
    public static class Build
    {
        public static Random Random = new Random();
        public static void Do(IGWContext context = null)
        {
            new ComputeOrderManager(context);
            new ViterbiManager();
            new MCMCSamplingManager();
            new OLMManager();
        }
    }
}
