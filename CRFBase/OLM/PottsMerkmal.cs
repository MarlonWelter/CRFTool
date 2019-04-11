using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.OLM
{
    public class PottsMerkmalNode : BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>
    {
        public PottsMerkmalNode(double lowerBoundary, double upperBoundary, int label)
        {
            Label = label;
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
        }
        public int Label { get; set; }

        // TODO need node.Data.Ordinate and node.Data.ZScore
        public override int Count(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph, int[] labeling)
        {
            int count = 0;
            foreach (var node in graph.Nodes)
            {
                var label = labeling[node.Data.Ordinate];
                if (label == Label)
                {
                    if (node.Data.ZScore > LowerBoundary && node.Data.ZScore <= UpperBoundary)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public override double Score(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node, int label)
        {
            if (label == Label)
            {
                if (node.Data.ZScore > LowerBoundary && node.Data.ZScore <= UpperBoundary)
                {
                    return 1.0;
                }
            }
            return 0.0;
        }

        public override double Score(IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge, int labelhead, int labelfoot)
        {
            return 0.0;
        }
    }
}
