using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using RASACRFGraph = CodeBase.IGWGraph<CRFBase.RASACRFNodeData, CRFBase.RASACRFEdgeData, CodeBase.ProteinCRFGraphData>;

namespace CRFBase
{
    public class RASACRFNodeData : CRFNodeData
    {
        public RASACRFNodeData(string id, double rasa)
            : base()
        {
            Id = id;
            RASA = rasa;
        }
        public double RASA { get; set; }
    }
    public class RASACRFEdgeData : CRFEdgeData
    {
        public RASACRFEdgeData(double max, double diff)
            : base()
        {
            Max = max;
            Diff = diff;
        }
        public double Max { get; set; }
        public double Diff { get; set; }
    }

    public class RASANodeData
    {
        public RASANodeData(string id, double rasa)
        {
            Id = id;
            RASA = rasa;
        }
        public string Id { get; set; }
        public double RASA { get; set; }
    }
    public class RASAEdgeData
    {
        public RASAEdgeData(double max, double diff)
        {
            Max = max;
            Diff = diff;
        }
        public double Max { get; set; }
        public double Diff { get; set; }
    }
    public class GraphData
    {

    }
    public class KeyuFeaturesOLM
    {
        public List<BasisMerkmal<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData>> Do(List<IGWGraph<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData>> graphs, int intervalsCount, int labels)
        {
            var basisMerkmale = new List<BasisMerkmal<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData>>();
            //for (int i = 0; i < graphs.Count; i++)
            //{
            var rasa = graphs.SelectMany(g => g.Nodes.Select(n => n.Data.RASA));
            var intervals = rasa.OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);

            var lowerBoundary = -0.1;
            for (int k = 0; k < intervals.Length; k++)
            {
                var upperBoundary = intervals[k].Max();

                for (int label = 1; label < labels; label++)
                {
                    var merkmal = new RASANodeBasisMerkmal(lowerBoundary, upperBoundary, label);
                    basisMerkmale.Add(merkmal);
                }
                lowerBoundary = upperBoundary;
            }

            var edges = graphs.SelectMany(g => g.Edges);

            var maxFeatures = edges.Select(e => e.Data.Max).ToList().OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);
            var diffFeatures = edges.Select(e => e.Data.Diff).ToList().OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);

            var lowerBoundaryMax = -0.1;
            var lowerBoundaryDiff = -0.1;

            //var upperBoundaryDiff = diffFeatures[k].Max();

            for (int label1 = 0; label1 < labels; label1++)
            {
                for (int label2 = label1; label2 < labels; label2++)
                {
                    if (label1 == 0 && label2 == 0)
                        continue;
                    for (int k = 0; k < intervals.Length; k++)
                    {
                        var upperBoundaryMax = maxFeatures[k].Max();
                        var merkmal1 = new RASAMaxEdgeBasisMerkmal(lowerBoundaryMax, upperBoundaryMax, label1, label2);
                        //var merkmal2 = new RASADiffEdgeBasisMerkmal(lowerBoundaryDiff, upperBoundaryDiff, label1, label2);
                        basisMerkmale.AddRange(merkmal1);
                        lowerBoundaryMax = upperBoundaryMax;
                    }
                }

                //lowerBoundaryDiff = upperBoundaryDiff;
            }

            for (int label1 = 0; label1 < labels; label1++)
            {
                for (int label2 = label1; label2 < labels; label2++)
                {
                    if (label1 == 0 && label2 == 0)
                        continue;
                    for (int k = 0; k < intervals.Length; k++)
                    {
                        //var upperBoundaryMax = maxFeatures[k].Max();
                        var upperBoundaryDiff = diffFeatures[k].Max();

                        //var merkmal1 = new RASAMaxEdgeBasisMerkmal(lowerBoundaryMax, upperBoundaryMax, label1, label2);
                        var merkmal2 = new RASADiffEdgeBasisMerkmal(lowerBoundaryDiff, upperBoundaryDiff, label1, label2);
                        basisMerkmale.AddRange(merkmal2);
                        lowerBoundaryDiff = upperBoundaryDiff;
                    }
                }

                //lowerBoundaryMax = upperBoundaryMax;
            }
            return basisMerkmale;
        }
    }

    //public class KeyuFeaturesOLM
    //{
    //    public List<BasisMerkmal<RASACRFNodeData, RASACRFEdgeData, CRFGraphData>> Do(List<IGWGraph<RASACRFNodeData, RASACRFEdgeData, CRFGraphData>> graphs, int intervalsCount, int labels, int tries)
    //    {
    //        var basisMerkmale = new List<BasisMerkmal<RASACRFNodeData, RASACRFEdgeData, CRFGraphData>>();
    //        //for (int i = 0; i < graphs.Count; i++)
    //        //{
    //        var rasa = graphs.SelectMany(g => g.Nodes.Select(n => n.Data.RASA));
    //        var intervals = rasa.OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);

    //        var allnodes = graphs.SelectMany(g => g.Nodes);

    //        var min = 0.0;
    //        var max = 1.0;
    //        var divisors = ComputeDiscretization<RASACRFNodeData>.Do(allnodes.Select(n => new AgO<RASACRFNodeData, double, int>(n.Data, n.Data.RASA, n.Data.ReferenceLabel == 0 ? 0 : 1)), intervalsCount, labels, tries);

    //        var lowerBoundary = min;
    //        for (int k = 0; k < intervals.Length; k++)
    //        {
    //            var upperBoundary = k < (intervalsCount - 1) ? divisors[k] : max;
    //            for (int label = 1; label < labels; label++)
    //            {
    //                var merkmal = new RASANodeBasisMerkmal(lowerBoundary, upperBoundary, label);
    //                basisMerkmale.Add(merkmal);
    //            }
    //            lowerBoundary = upperBoundary;
    //        }

    //        var edges = graphs.SelectMany(g => g.Edges);


    //        min = edges.Min(e => e.Data.Max);
    //        max = edges.Max(e => e.Data.Max);
    //        divisors = ComputeDiscretization<RASACRFEdgeData>.Do(edges.Select(n => new AgO<RASACRFEdgeData, double, int>(n.Data, n.Data.Max, (n.Head.Data.ReferenceLabel == 0 ? 0 : 1) + (n.Foot.Data.ReferenceLabel == 0 ? 0 : 1))), intervalsCount, labels, tries, min, max);

    //        //var maxFeatures = edges.Select(e => e.Data.Max).ToList().OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);
    //        //var diffFeatures = edges.Select(e => e.Data.Diff).ToList().OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);

    //        lowerBoundary = min;
    //        for (int k = 0; k < intervals.Length; k++)
    //        {
    //            var upperBoundary = k < (intervalsCount - 1) ? divisors[k] : max;

    //            for (int label1 = 0; label1 < labels; label1++)
    //            {
    //                for (int label2 = label1; label2 < labels; label2++)
    //                {
    //                    if (label1 == 0 && label2 == 0)
    //                        continue;
    //                    var merkmal1 = new RASAMaxEdgeBasisMerkmal(lowerBoundary, upperBoundary, label1, label2);
    //                    basisMerkmale.AddRange(merkmal1);
    //                }
    //            }
    //            lowerBoundary = upperBoundary;
    //        }

    //        min = edges.Min(e => e.Data.Max);
    //        max = edges.Max(e => e.Data.Max);
    //        divisors = ComputeDiscretization<RASACRFEdgeData>.Do(edges.Select(n => new AgO<RASACRFEdgeData, double, int>(n.Data, n.Data.Diff, (n.Head.Data.ReferenceLabel == 0 ? 0 : 1) + (n.Foot.Data.ReferenceLabel == 0 ? 0 : 1))), intervalsCount, labels, tries, min, max);


    //        lowerBoundary = min;
    //        for (int k = 0; k < intervals.Length; k++)
    //        {
    //            var upperBoundary = k < (intervalsCount - 1) ? divisors[k] : max;

    //            for (int label1 = 0; label1 < labels; label1++)
    //            {
    //                for (int label2 = label1; label2 < labels; label2++)
    //                {
    //                    if (label1 == 0 && label2 == 0)
    //                        continue;
    //                    var merkmal1 = new RASADiffEdgeBasisMerkmal(lowerBoundary, upperBoundary, label1, label2);
    //                    basisMerkmale.AddRange(merkmal1);
    //                }
    //            }
    //            lowerBoundary = upperBoundary;
    //        }
    //        return basisMerkmale;
    //    }

    public class RASANodeBasisMerkmal : BasisMerkmal<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData>
    {
        public RASANodeBasisMerkmal(double lowerBoundary, double upperBoundary, int label)
        {
            Label = label;
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
        }
        public int Label { get; set; }

        public override int Count(RASACRFGraph graph, int[] labeling)
        {
            int count = 0;
            foreach (var node in graph.Nodes)
            {
                var label = labeling[node.Data.Ordinate];
                if (label == Label)
                {
                    if (node.Data.RASA > LowerBoundary && node.Data.RASA <= UpperBoundary)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public override double Score(IGWNode<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData> node, int label)
        {
            if (label == Label)
            {
                if (node.Data.RASA > LowerBoundary && node.Data.RASA <= UpperBoundary)
                {
                    return 1.0;
                }
            }
            return 0.0;
        }

        public override double Score(IGWEdge<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData> edge, int labelhead, int labelfoot)
        {
            return 0.0;
        }

    }
    public class RASAMaxEdgeBasisMerkmal : BasisMerkmal<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData>
    {
        public RASAMaxEdgeBasisMerkmal(double lowerBoundary, double upperBoundary, int labelhead, int labelFoot)
        {
            LabelHead = labelhead;
            LabelFoot = labelFoot;
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
        }
        public int LabelHead { get; set; }
        public int LabelFoot { get; set; }

        public override int Count(RASACRFGraph graph, int[] labeling)
        {
            int count = 0;
            foreach (var edge in graph.Edges)
            {
                var labelHead = labeling[edge.Head.Data.Ordinate];
                var labelFoot = labeling[edge.Foot.Data.Ordinate];

                if ((labelHead == LabelHead && labelFoot == LabelFoot) ||
                    (labelHead == LabelFoot && labelFoot == LabelHead))
                {
                    if (edge.Data.Max > LowerBoundary && edge.Data.Max <= UpperBoundary)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        public override double Score(IGWNode<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData> node, int label)
        {
            return 0.0;
        }

        public override double Score(IGWEdge<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData> edge, int labelHead, int labelFoot)
        {
            if ((labelHead == LabelHead && labelFoot == LabelFoot) ||
                   (labelHead == LabelFoot && labelFoot == LabelHead))
            {
                if (edge.Data.Max > LowerBoundary && edge.Data.Max <= UpperBoundary)
                {
                    return 1.0;
                }
            }
            return 0.0;
        }
    }
    public class RASADiffEdgeBasisMerkmal : BasisMerkmal<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData>
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

        public override int Count(RASACRFGraph graph, int[] labeling)
        {
            int count = 0;
            foreach (var edge in graph.Edges)
            {
                var labelHead = labeling[edge.Head.Data.Ordinate];
                var labelFoot = labeling[edge.Foot.Data.Ordinate];

                if ((labelHead == LabelHead && labelFoot == LabelFoot) ||
                    (labelHead == LabelFoot && labelFoot == LabelHead))
                {
                    if (edge.Data.Diff > LowerBoundary && edge.Data.Diff <= UpperBoundary)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        public override double Score(IGWNode<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData> node, int label)
        {
            return 0.0;
        }

        public override double Score(IGWEdge<RASACRFNodeData, RASACRFEdgeData, ProteinCRFGraphData> edge, int labelHead, int labelFoot)
        {
            if ((labelHead == LabelHead && labelFoot == LabelFoot) ||
                   (labelHead == LabelFoot && labelFoot == LabelHead))
            {
                if (edge.Data.Diff > LowerBoundary && edge.Data.Diff <= UpperBoundary)
                {
                    return 1.0;
                }
            }
            return 0.0;
        }
    }
}
