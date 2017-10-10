using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRFGraph = CodeBase.IGWGraph<CRFBase.SGLNodeData, CRFBase.SGLEdgeData, CRFBase.SGLGraphData>;

namespace SoftwareGraphLearning
{


    public static class MHSamplerDefaultValues
    {
        public const int NumberChains = 3;  
        public const int PreRunLength = 100000;
        public const int TestInterval = 10000;
        public const double ToleranceMarginalDistribution = 0.14;
        public const double ToleranceVariance = 0.005;
        public const double ToleranceAutoCorrelation = 0.025;
    }
    class MHSamplerParameters
    {

        public CRFGraph Graph { get; set; }

        public int NumberChains { get; set; } = MHSamplerDefaultValues.NumberChains;

        public int PreRunLength { get; set; } = MHSamplerDefaultValues.PreRunLength;

        public int TestInterval { get; set; } = MHSamplerDefaultValues.TestInterval;

        public double ToleranceMarginalDistribution { get; set; } = MHSamplerDefaultValues.ToleranceMarginalDistribution;

        public double ToleranceVariance { get; set; } = MHSamplerDefaultValues.ToleranceVariance;

        public double ToleranceAutoCorrelation { get; set; } = MHSamplerDefaultValues.ToleranceAutoCorrelation;


    }
}
