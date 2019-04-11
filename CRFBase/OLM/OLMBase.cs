using CodeBase;
using CRFBase.OLM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public abstract class OLMBase<NodeData, EdgeData, GraphData>
      where NodeData : ICRFNodeData
      where EdgeData : ICRFEdgeData
      where GraphData : ICRFGraphData
    {
        public string Name { get; set; }
        public int Labels { get; set; }
        public Func<int[], int[], double> LossFunctionIteration { get; set; }
        public Func<int[], int[], double> LossFunctionValidation { get; set; }
        public BasisMerkmal<NodeData, EdgeData, GraphData>[] BasisMerkmale { get; set; }

        public List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs { get; set; }
        public List<IGWGraph<NodeData, EdgeData, GraphData>> ValidationGraphs { get; set; }
        public int MaxIterations { get; set; }
        public int Iteration { get; set; }
        public int BufferSizeInference { get; set; }

        protected Random random = new Random();
        protected long tp = 0, tn = 0, fp = 0, fn = 0;
        public OLMTracking OLMTracker { get; set; }
        protected double[] weightOpt;
        protected double[] weightCurrent;
        protected int Weights { get; set; }
        protected Random rdm = new Random();

        public double[] ResultingWeights { get; set; }

        protected WeightObservationUnit WeightObservationUnit = new WeightObservationUnit(1.00, 1.00);
        protected WeightObservationUnit_II WeightObservationUnit_II { get; set; }

        protected int[] CountPred(IGWGraph<NodeData, EdgeData, GraphData> graph, int[] labeling)
        {
            var countPreds = new int[BasisMerkmale.Length];

            for (int i = 0; i < BasisMerkmale.Length; i++)
            {
                countPreds[i] = BasisMerkmale[i].Count(graph, labeling);
            }

            return countPreds;
        }

        protected void SetWeightsCRF(double[] weightCurrent, IGWGraph<NodeData, EdgeData, GraphData> graph)
        {
            foreach (var node in graph.Nodes)
            {
                var scores = new double[Labels];
                node.Data.Scores = scores;
                for (int k = 0; k < weightCurrent.Length; k++)
                {
                    for (int label = 0; label < Labels; label++)
                    {
                        scores[label] += weightCurrent[k] * BasisMerkmale[k].Score(node, label);
                    }
                }
            }
            foreach (var edge in graph.Edges)
            {
                var scores = new double[Labels, Labels];
                edge.Data.Scores = scores;
                for (int k = 0; k < weightCurrent.Length; k++)
                {
                    for (int label1 = 0; label1 < Labels; label1++)
                    {
                        for (int label2 = 0; label2 < Labels; label2++)
                        {
                            scores[label1, label2] += weightCurrent[k] * BasisMerkmale[k].Score(edge, label1, label2);
                        }
                    }
                }
            }
        }

        protected abstract bool CheckCancelCriteria();

        protected abstract double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration);

        public void Do(int weights, IEnumerable<IGWGraph<NodeData, EdgeData, GraphData>> graphs, int maxIterations, OLMRequest olmrequest)
        {
            Weights = weights;
            int validationQuantils = 2;
            double quantilratio = 1.0 / validationQuantils;
            var quantiledGraphs = new List<IGWGraph<NodeData, EdgeData, GraphData>>[validationQuantils];

            for (int i = 0; i < validationQuantils; i++)
            {
                quantiledGraphs[i] = new List<IGWGraph<NodeData, EdgeData, GraphData>>();
            }

            //divide graphs in training / validation
            foreach (var graph in graphs)
            {
                var quantil = random.Next(validationQuantils);
                quantiledGraphs[quantil].Add(graph);
            }

            for (int quantilIteration = 0; quantilIteration < validationQuantils; quantilIteration++)
            {
                TrainingGraphs = new List<IGWGraph<NodeData, EdgeData, GraphData>>();
                ValidationGraphs = new List<IGWGraph<NodeData, EdgeData, GraphData>>();


                for (int quantil = 0; quantil < validationQuantils; quantil++)
                {
                    if (quantil == quantilIteration)
                    {
                        ValidationGraphs.AddRange(quantiledGraphs[quantil]);
                    }
                    else
                    {
                        TrainingGraphs.AddRange(quantiledGraphs[quantil]);
                    }
                }

                Iteration = 0;
                MaxIterations = maxIterations;

                SetStartingWeights();

                this.WeightObservationUnit.Init(weightCurrent);
                var lossOpt = double.MaxValue;
                var lossOptOld = 0.0;
                var lossCurrent = 0.0;

                OLMTracker = new OLMTracking(weights, new int[] { 1, 3, 5, 8, 12, 20, 50 }, weightCurrent, Name + "_q" + quantilIteration + "_OLMTracking.txt");

                var interfaceValid = 0;
                var noninterfaceValid = 0;

                foreach (var graph in ValidationGraphs)
                {
                    interfaceValid += graph.Data.ReferenceLabeling.Sum();
                    noninterfaceValid += graph.Nodes.Count() - graph.Data.ReferenceLabeling.Sum();
                }

                var sitesValid = interfaceValid + noninterfaceValid;

                while (!CheckCancelCriteria())
                {
                    Iteration++;

                    var oldWVector = weightCurrent.ToArray();
                    // train
                    weightCurrent = DoIteration(TrainingGraphs, weightCurrent, Iteration);

                    ResultingWeights = weightCurrent;


                    tp = 0; tn = 0; fp = 0; fn = 0;
                    lossCurrent = 0.0;
                    foreach (var graph in ValidationGraphs)
                    {
                        SetWeightsCRF(weightCurrent, graph);

                        var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, Labels, BufferSizeInference);
                        request.RequestInDefaultContext();

                        var prediction = request.Solution.Labeling;
                        lossCurrent += LossFunctionValidation(graph.Data.ReferenceLabeling, prediction);

                        TrackResults(graph.Data.ReferenceLabeling, prediction);
                    }
                    WriterResults();
                    lossCurrent /= sitesValid;

                    if (lossCurrent < lossOpt)
                    {
                        lossOptOld = lossOpt;
                        lossOpt = lossCurrent;
                        weightOpt = weightCurrent;
                    }

                    OLMTracker.Track(weightCurrent, lossCurrent);
                    var iterationResult = new OLMIterationResult(weightCurrent.ToArray(), lossCurrent);
                    olmrequest.Result.ResultsHistory.IterationResultHistory.Add(iterationResult);


                }
            }

            OLMTracker.WriteWeights();

        }

        internal abstract void SetStartingWeights();

        protected void TrackResults(int[] truth, int[] prediction)
        {

            for (int i = 0; i < prediction.Length; i++)
            {
                if (truth[i] == 1)
                {
                    if (prediction[i] == 1)
                        tp++;
                    else
                        fn++;
                }
                else
                {
                    if (prediction[i] == 1)
                        fp++;
                    else
                        tn++;
                }
            }

        }

        private void WriterResults()
        {
            using (var writer = new StreamWriter(Name + "_Results.txt", true))
            {
                var sensitivity = tp / (tp + fn);
                var specificity = tn / (tn + fp);
                var mcc = (tp * tn + fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));


                writer.WriteLine("" + tp + "_" + tn + "_" + fp + "_" + fn);
                writer.WriteLine(sensitivity);
                writer.WriteLine(specificity);
                writer.WriteLine(mcc);
                writer.WriteLine();
            }
        }
    }
    public abstract class BasisMerkmal<NodeData, EdgeData, GraphData>
    {
        public int Counter { get; set; }
        public double LowerBoundary { get; set; }
        public double UpperBoundary { get; set; }

        public abstract int Count(IGWGraph<NodeData, EdgeData, GraphData> graph, int[] labeling);

        public abstract double Score(IGWNode<NodeData, EdgeData, GraphData> node, int label);
        public abstract double Score(IGWEdge<NodeData, EdgeData, GraphData> edge, int labelhead, int labelfoot);
    }

    public enum OLMVariant
    {
        Default,
        AverageDistCriteria,
        LastTwoMax,
        Test,
        Ising
    }

    public class CharacteristicFeature : BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>
    {
        public CharacteristicFeature(int characteristic, int classification, double lowerBound, double upperBound)
        {
            Characteristic = characteristic;
            Classification = classification;
            LowerBoundary = lowerBound;
            UpperBoundary = upperBound;
        }
        public int Characteristic { get; set; }
        public int Classification { get; set; }

        public override int Count(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph, int[] labeling)
        {
            int counter = 0;
            foreach (var node in graph.Nodes)
            {
                if (labeling[node.GraphId] == Classification && node.Data.Characteristics[Characteristic] >= LowerBoundary && node.Data.Characteristics[Characteristic] <= UpperBoundary)
                {
                    counter++;
                }
            }
            return counter;
        }

        public override double Score(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node, int label)
        {
            if (label == Classification && node.Data.Characteristics[Characteristic] >= LowerBoundary && node.Data.Characteristics[Characteristic] <= UpperBoundary)
            {
                return 1;
            }
            return 0;
        }

        public override double Score(IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge, int labelhead, int labelfoot)
        {
            return 0;
        }
    }
}
