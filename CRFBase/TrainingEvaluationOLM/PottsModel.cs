using CodeBase;
using CRFBase.OLM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class PottsModel
    {
        public PottsModel(double[] conformityParameter, double correlationParameter)
        {
            ConformityParameter = conformityParameter;
            CorrelationParameter = correlationParameter;
        }
        public double[] ConformityParameter { get; set; }
        public double CorrelationParameter { get; set; }
        
        public const int NumberOfLabels = 2;

        public List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> AddNodeFeatures(List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> graphs, int intervalsCount, int labels)
        {
            var basisMerkmale = new List<BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
            // Observation = Zellner Scores -> use for different features -> Zellner Score in different intervals
            var zscores = graphs.SelectMany(g => g.Nodes.Select(n => n.Data.Characteristics[0]));


            var zscore_list = zscores.ToList();
            var label_list = graphs.SelectMany((g => g.Nodes.Select(n => n.Data.ReferenceLabel))).ToList();
            int multiplier = 3;
            //System.IO.File.WriteAllLines("listForIntervalVisualisation2.txt", listForIntervalVisualisation);
            for(int i=0; i<label_list.Count; i++)
            {
                if(label_list[i] == 1)
                {
                    for(int j=0; j<multiplier; j++)
                    {
                        zscore_list.Add(zscore_list[i]);
                    }
                }
            }

            var intervals = zscore_list.OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);
            //var intervals = zscores.OrderBy(r => r).ToList().SplitToIntervals(intervalsCount);

            var lowerBoundary = -0.1;
            for (int k = 0; k < intervals.Length; k++)
            {
                var upperBoundary = intervals[k].Max();

                for (int label = 0; label < labels; label++)
                {
                    var merkmal = new PottsMerkmalNode(lowerBoundary, upperBoundary, label);
                    basisMerkmale.Add(merkmal);
                }
                lowerBoundary = upperBoundary;
            }

            return basisMerkmale;
        }

        public void InitCRFScore(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            var random = new Random();
            foreach (var node in graph.Nodes)
            {
                node.Data.Scores = new double[2];
                node.Data.Scores[0] = random.NextDouble();
                node.Data.Scores[1] = random.NextDouble();

            }
            foreach (var edge in graph.Edges)
            {
                edge.Data.Scores = new double[NumberOfLabels, NumberOfLabels] { { CorrelationParameter, -CorrelationParameter }, { -CorrelationParameter, CorrelationParameter } };
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
                edge.Data.Scores = new double[NumberOfLabels, NumberOfLabels] { { CorrelationParameter, -CorrelationParameter }, { -CorrelationParameter, CorrelationParameter } };
            }
        }
    }
}
