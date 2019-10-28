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
                        //count++;
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

    public class MaxEdgeBasisMerkmal : BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>
    {
        public MaxEdgeBasisMerkmal(double lowerBoundary, double upperBoundary, int labelhead, int labelFoot)
        {
            LabelHead = labelhead;
            LabelFoot = labelFoot;
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
        }
        public int LabelHead { get; set; }
        public int LabelFoot { get; set; }

        public override int Count(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph, int[] labeling)
        {
            int count = 0;
            foreach (var edge in graph.Edges)
            {
                var labelHead = labeling[edge.Head.GraphId];
                var labelFoot = labeling[edge.Foot.GraphId];

                //    if ((labelHead == LabelHead && labelFoot == LabelFoot) ||
                //        (labelHead == LabelFoot && labelFoot == LabelHead))
                //    {
                //        if (edge.Data.Max > LowerBoundary && edge.Data.Max <= UpperBoundary)
                //        {
                //            count++;
                //        }
                //    }
            }
            return count;
        }
        public override double Score(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node, int label)
        {
            return 0.0;
        }

        public override double Score(IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge, int labelHead, int labelFoot)
        {
            //if ((labelHead == LabelHead && labelFoot == LabelFoot) ||
            //       (labelHead == LabelFoot && labelFoot == LabelHead))
            //{
            //    if (edge.Data.Max > LowerBoundary && edge.Data.Max <= UpperBoundary)
            //    {
            //        return 1.0;
            //    }
            //}
            return 0.0;
        }
    }
    public class RASADiffEdgeBasisMerkmal : BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>
    {
        public RASADiffEdgeBasisMerkmal(double lowerBoundary, double upperBoundary, int labelhead, int labelFoot)
        {
            LabelHead = labelhead;
            LabelFoot = labelFoot;
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
        }
        public int LabelHead { get; set; }
        public int LabelFoot { get; set; }

        public override int Count(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph, int[] labeling)
        {
            int count = 0;
            //foreach (var edge in graph.Edges)
            //{
            //    var labelHead = labeling[edge.Head.Data.Ordinate];
            //    var labelFoot = labeling[edge.Foot.Data.Ordinate];

            //    if ((labelHead == LabelHead && labelFoot == LabelFoot) ||
            //        (labelHead == LabelFoot && labelFoot == LabelHead))
            //    {
            //        if (edge.Data.Diff > LowerBoundary && edge.Data.Diff <= UpperBoundary)
            //        {
            //            count++;
            //        }
            //    }
            //}
            return count;
        }
        public override double Score(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node, int label)
        {
            return 0.0;
        }

        public override double Score(IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge, int labelHead, int labelFoot)
        {
            //if ((labelHead == LabelHead && labelFoot == LabelFoot) ||
            //       (labelHead == LabelFoot && labelFoot == LabelHead))
            //{
            //    if (edge.Data.Diff > LowerBoundary && edge.Data.Diff <= UpperBoundary)
            //    {
            //        return 1.0;
            //    }
            //}
            return 0.0;
        }
    }
}
