using CodeBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.OLM
{
    public static class OLMDefaultValues
    {
        public const double Alpha = 0.5;
        public const int NumberLabels = 2;
        public const int BufferSizeCRF = 200;
        public const int OLMMaxIterations = 1000;
    }
    public class OLMRequest : GWRequest<OLMRequest>
    {
        public OLMRequest(OLMVariant variant, IEnumerable<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> graphs)
        {
            Variante = variant;
            Graphs = graphs.ToList();
        }
        public OLMVariant Variante { get; set; }
        public double Alpha { get; set; } = OLMDefaultValues.Alpha;
        public int NumberLabels { get; set; } = OLMDefaultValues.NumberLabels;
        public int BufferSizeCRF { get; set; } = OLMDefaultValues.BufferSizeCRF;
        public int MaxIterations { get; set; } = OLMDefaultValues.OLMMaxIterations;
        public BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>[] BasisMerkmale { get; set; }
        public Func<int[], int[], double> LossFunctionIteration { get; set; }
        public Func<int[], int[], double> LossFunctionValidation { get; set; }
        public List<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> Graphs { get; set; }

        public OLMRequestResult Result { get; set; } = new OLMRequestResult(new double[2]);

    }
    public class OLMRequestResult
    {

        public OLMRequestResult(double[] resultingWeights)
        {
            ResultingWeights = resultingWeights;
        }

        public double[] ResultingWeights { get; set; }

        public OLMResults ResultsHistory { get; set; } = new OLMResults();

    }
    public class OLMResults
    {

        private LinkedList<OLMIterationResult> iterationResultHistory = new LinkedList<OLMIterationResult>();
        public LinkedList<OLMIterationResult> IterationResultHistory
        {
            get { return iterationResultHistory; }
            set { iterationResultHistory = value; }
        }
    }
    public class OLMIterationResult
    {
        public OLMIterationResult(double[] weights, double loss)
        {
            Weights = weights;
            Loss = loss;
        }
        public double[] Weights { get; set; }
        public double Loss { get; set; }
    }
}
