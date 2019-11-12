using CodeBase;
using CRFBase.OLM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class PottsModelComplex
    {
        public PottsModelComplex(double[] conformityParameter, double[] correlationParameter, double amplifierControlParameter, int numberOfLabels)
        {
            ConformityParameter = conformityParameter;
            CorrelationParameter = correlationParameter;
            AmplifierControlParameter = amplifierControlParameter;
            NumberOfLabels = numberOfLabels;
        }
        public double[] ConformityParameter { get; set; }
        public double[] CorrelationParameter { get; set; }
        public double AmplifierControlParameter { get; set; }
        public int NumberOfLabels { get; set; }

        public List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> AddNodeFeatures(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> graphs, int intervalsCount)
        {
            var basisMerkmale = new List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
            var intervals = getIntervals(graphs, intervalsCount);

            var lowerBoundary = -0.1;
            for (int k = 0; k < intervalsCount; k++)
            {
                var upperBoundary = intervals[k].Max();

                for (int label = 0; label < NumberOfLabels; label++)
                {
                    var merkmal = new PottsMerkmalNode(lowerBoundary, upperBoundary, label, AmplifierControlParameter);
                    basisMerkmale.Add(merkmal);
                }
                lowerBoundary = upperBoundary;
            }

            return basisMerkmale;
        }

        private IEnumerable<double>[] getIntervals(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> graphs, int intervalsCount)
        {
            // Observation = Zellner Scores -> use for different features -> Zellner Score in different intervals
            var zscores = graphs.SelectMany(g => g.Nodes.Select(n => n.Data.Characteristics[0]));
            var zscore_list = zscores.ToList();
            var label_list = graphs.SelectMany((g => g.Nodes.Select(n => n.Data.ReferenceLabel))).ToList();
            int multiplier = 3;

            //System.IO.File.AppendAllLines("listForIntervalVisualisation_artificial.txt", zscore_list.Select(score => score.ToString())); //label_list.Select(label=>label.ToString()));

            for (int i = 0; i < label_list.Count; i++)
            {
                if (label_list[i] == 1)
                {
                    for (int j = 0; j < multiplier; j++)
                    {
                        zscore_list.Add(zscore_list[i]);
                    }
                }
            }

            var intervals = zscore_list.OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);
            //var intervals = zscores.OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);

            return intervals;
        }

        public List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> AddEdgeFeatures(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> graphs, int intervalsCount)
        {
            var basisMerkmale = new List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();

            var edges = graphs.SelectMany(g => g.Edges);
            var maxFeatures = edges.Select(e => e.Data.MaxZellnerScore).ToList().OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);
            var diffFeatures = edges.Select(e => e.Data.DiffZellnerScore).ToList().OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);
                                             
            for (int label1 = 0; label1 < NumberOfLabels; label1++)
            {
                for (int label2 = 0; label2 < NumberOfLabels; label2++)
                {
                    var lowerBoundaryMax = -0.1;
                    for (int k = 0; k < intervalsCount; k++)
                    {
                        var upperBoundaryMax = maxFeatures[k].Max();
                        var merkmal1 = new MaxEdgeBasisMerkmal(lowerBoundaryMax, upperBoundaryMax, label1, label2);
                        basisMerkmale.AddRange(merkmal1);
                        lowerBoundaryMax = upperBoundaryMax;
                    }
                }
            }

            
            for (int label1 = 0; label1 < NumberOfLabels; label1++)
            {
                for (int label2 = 0; label2 < NumberOfLabels; label2++)
                {
                    var lowerBoundaryDiff = -0.1;
                    for (int k = 0; k < intervalsCount; k++)
                    {
                        var upperBoundaryDiff = diffFeatures[k].Max();
                        var merkmal2 = new DiffEdgeBasisMerkmal(lowerBoundaryDiff, upperBoundaryDiff, label1, label2);
                        basisMerkmale.AddRange(merkmal2);
                        lowerBoundaryDiff = upperBoundaryDiff;
                    }
                }
            }

            return basisMerkmale;
        }

            public void InitCRFScore(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            var random = new Random();
            foreach (var node in graph.Nodes)
            {
                node.Data.Scores = new double[NumberOfLabels];
                for (int labelCount = 0; labelCount < NumberOfLabels; labelCount++)
                {
                    node.Data.Scores[labelCount] = node.Data.Observation == labelCount ? ConformityParameter[0] : ConformityParameter[1];
                }

            }
            foreach (var edge in graph.Edges)
            {
                edge.Data.Scores = new double[,] {{ CorrelationParameter[0], -CorrelationParameter[0] }, { -CorrelationParameter[0], CorrelationParameter[0] } };
            }
        }

        public void CreateCRFScore(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph, List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> BasisMerkmale)
        {
            foreach (var node in graph.Nodes)
            {
                var scores = new double[NumberOfLabels];
                node.Data.Scores = scores;
                for (int k = 0; k < ConformityParameter.Length; k++)
                {
                    for (int label = 0; label < NumberOfLabels; label++)
                    {
                        scores[label] += ConformityParameter[k] * BasisMerkmale[k].Score(node, label);
                    }
                }
            }
            foreach (var edge in graph.Edges)
            {
                var scores = new double[NumberOfLabels, NumberOfLabels];
                edge.Data.Scores = scores;
                for (int k = 0; k < CorrelationParameter.Length; k++)
                {
                    for (int label1 = 0; label1 < NumberOfLabels; label1++)
                    {
                        for (int label2 = 0; label2 < NumberOfLabels; label2++)
                        {
                            scores[label1, label2] += CorrelationParameter[k] * BasisMerkmale[k].Score(edge, label1, label2);
                        }
                    }
                }
            }
        }
    }
}
