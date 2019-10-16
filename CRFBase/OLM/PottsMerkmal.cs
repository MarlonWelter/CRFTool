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
        public PottsMerkmalNode(double lowerBoundary, double upperBoundary, int label, double amplifierControlParameter)
        {
            Label = label;
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
            AmplifierControlParameter = amplifierControlParameter;
        }
        public int Label { get; set; }
        public double AmplifierControlParameter { get; private set; }

        public override int Count(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph, int[] labeling)
        {
            var amplifier = CalculateAmplifierNodes(graph);
            int count = 0;
            foreach (var node in graph.Nodes)
            {
                var label = labeling[node.GraphId];
                if (label == Label)
                {
                    // Characteristics[0] contains Zellner Score
                    if (node.Data.Characteristics[0] > LowerBoundary && node.Data.Characteristics[0] <= UpperBoundary)
                    {
                        count += label == 0 ? 1 : (int)(1+AmplifierControlParameter*amplifier);
                    }
                }
            }
            return count;
        }

        private double CalculateAmplifierNodes(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            var I = graph.Data.ReferenceLabeling.Sum();
            var N = graph.Nodes.Count() - I;
            return N/I - 1;
        }

        public override double Score(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node, int label)
        {
            if (label == Label)
            {
                // Characteristics[0] contains Zellner Score
                if (node.Data.Characteristics[0] > LowerBoundary && node.Data.Characteristics[0] <= UpperBoundary)
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
