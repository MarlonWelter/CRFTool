
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.IO;

namespace CRFBase
{
    public class OLM_VI_Mem_BoundChange<NodeData, EdgeData, GraphData> : OLMBase<NodeData, EdgeData, GraphData>
        where NodeData : RASACRFNodeData
        where EdgeData : RASACRFEdgeData
        where GraphData : ProteinCRFGraphData
    {
        public OLM_VI_Mem_BoundChange(int labels, int intervals, int bufferSizeInference, BasisMerkmal<NodeData, EdgeData, GraphData>[] basisMerkmale, Func<int[], int[], double> lossfunctionIteration, Func<int[], int[], double> lossfunctionValidation, string name)
        {
            Labels = labels;
            Intervals = intervals;
            BufferSizeInference = bufferSizeInference;
            LossFunctionIteration = lossfunctionIteration;
            LossFunctionValidation = lossfunctionValidation;
            BasisMerkmale = basisMerkmale;
            Name = name;

            ValueMapRasa = new IntervalValueMap<NodeData, EdgeData, GraphData>();
            ValueMapEMax01 = new IntervalValueMap<NodeData, EdgeData, GraphData>();
            ValueMapEMax11 = new IntervalValueMap<NodeData, EdgeData, GraphData>();
            ValueMapEDiff01 = new IntervalValueMap<NodeData, EdgeData, GraphData>();
            ValueMapEDiff11 = new IntervalValueMap<NodeData, EdgeData, GraphData>();
        }
        protected override bool CheckCancelCriteria()
        {
            return Iteration >= MaxIterations;
        }

        private IntervalValueMap<NodeData, EdgeData, GraphData> ValueMapRasa;
        private IntervalValueMap<NodeData, EdgeData, GraphData> ValueMapEMax01;
        private IntervalValueMap<NodeData, EdgeData, GraphData> ValueMapEMax11;
        private IntervalValueMap<NodeData, EdgeData, GraphData> ValueMapEDiff01;
        private IntervalValueMap<NodeData, EdgeData, GraphData> ValueMapEDiff11;

        private readonly int Intervals;

        public int CoreResiduesValidation { get; set; }
        public int CoreResiduesTraining { get; set; }

        private void WriterResults()
        {
            using (var writer = new StreamWriter(Name + ".txt", true))
            {
                var sensitivity = tp / (tp + fn);
                var specificity = tn / (tn + fp);
                var mcc = (tp * tn - fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));

                writer.WriteLine("" + tp + "_" + tn + "_" + fp + "_" + fn);
                writer.WriteLine(sensitivity);
                writer.WriteLine(specificity);
                writer.WriteLine(mcc);
                writer.WriteLine();
            }
        }


        class Point
        {
            public Point(double[] weights, int[] counts, double mcc)
            {
                Weights = weights;
                Counts = counts;
                Score = mcc;
            }
            public double[] Weights;
            public int[] Counts;
            public double Score;
        }

        List<Point> MemoryPoints = new List<Point>();
        Point ReferencePoint;
        int memoryPoints = 5;
        protected override double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration)
        {
            var weights = weightCurrent.ToArray();
            var weightsSum = new double[weightCurrent.Length];
            int iteration = 0;
            double mcc = 0.0, mcc2 = 0.0;

            var newPoint = new Point(weights, new int[weights.Length], 0.0);


            {
                double tp = 0.001, tn = CoreResiduesTraining, fp = 0.001, fn = 0.001;

                MemoryPoints.Add(newPoint);
                if (MemoryPoints.Count > memoryPoints)
                    MemoryPoints.RemoveAt(0);

                ReferencePoint = new Point(weights, new int[weights.Length], 1.0);
                for (int i = 0; i < TrainingGraphs.Count; i++)
                {
                    var graph = TrainingGraphs[i];
                    //set scores according to weights
                    #region set weights
                    foreach (var node in graph.Nodes)
                    {
                        var scores = new double[Labels];
                        node.Data.Scores = scores;

                        for (int label = 0; label < Labels; label++)
                        {
                            for (int k = 0; k < weights.Length; k++)
                            {
                                scores[label] += weights[k] * BasisMerkmale[k].Score(node, label);
                            }
                        }
                    }
                    foreach (var edge in graph.Edges)
                    {
                        var scores = new double[Labels, Labels];
                        edge.Data.Scores = scores;

                        for (int label1 = 0; label1 < Labels; label1++)
                        {
                            for (int label2 = 0; label2 < Labels; label2++)
                            {
                                for (int k = 0; k < weights.Length; k++)
                                {
                                    scores[label1, label2] += weights[k] * BasisMerkmale[k].Score(edge, label1, label2);
                                }
                            }
                        }
                    }
                    #endregion

                    //compute labeling with viterbi algorithm
                    var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, Labels, BufferSizeInference);
                    request.RequestInDefaultContext();
                    int[] labeling = request.Result.Labeling;
                    //check nonequality

                    iteration++;
                    for (int k = 0; k < labeling.Length; k++)
                    {
                        if (graph.Data.ReferenceLabeling[k] > 0)
                        {
                            if (labeling[k] > 0)
                                tp += 1;
                            else
                                fn += 1;
                        }
                        else
                        {
                            if (labeling[k] > 0)
                                fp += 1;
                            else
                                tn += 1;
                        }
                    }

                    int[] countsPred = CountPred(graph, labeling);
                    int[] countsRef = CountPred(graph, graph.Data.ReferenceLabeling);
                    for (int k = 0; k < countsPred.Length; k++)
                    {
                        newPoint.Counts[k] += countsPred[k];
                        ReferencePoint.Counts[k] += countsRef[k];
                    }
                    //TrackResults(labeling, graph.Data.ReferenceLabeling);
                }
                //WriterResults();
                var sensitivity = tp / (tp + fn);
                var specificity = tn / (tn + fp);
                mcc = (tp * tn - fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));
                newPoint.Score = mcc;

                Log.Post("New MCC: " + Math.Round(mcc, 5) + " Sens: " + Math.Round(sensitivity, 5) + " Spec: " + Math.Round(specificity, 5));
            }
            //change Interval borders
            {
                var randomChanges = DivisorChange.RandomInstances(Intervals - 1);

                foreach (var change in randomChanges)
                {
                    var changesFor = random.Next(5);
                    if (changesFor == 0)
                        ValueMapRasa.ApplyChange(change);
                    else if (changesFor == 1)
                    {
                        ValueMapEMax01.ApplyChange(change);
                    }
                    else if (changesFor == 2)
                    {
                        ValueMapEMax11.ApplyChange(change);
                    }
                    else if (changesFor == 3)
                    {
                        ValueMapEDiff01.ApplyChange(change);
                    }
                    else if (changesFor == 4)
                    {
                        ValueMapEDiff11.ApplyChange(change);
                    }
                }


                double tp = 0.001, tn = CoreResiduesTraining, fp = 0.001, fn = 0.001;


                for (int i = 0; i < TrainingGraphs.Count; i++)
                {
                    var graph = TrainingGraphs[i];
                    SetWeightsCRF(weights, graph);

                    //compute labeling with viterbi algorithm
                    var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, Labels, BufferSizeInference);
                    request.RequestInDefaultContext();
                    int[] labeling = request.Result.Labeling;
                    //check nonequality

                    for (int k = 0; k < labeling.Length; k++)
                    {
                        if (graph.Data.ReferenceLabeling[k] > 0)
                        {
                            if (labeling[k] > 0)
                                tp += 1;
                            else
                                fn += 1;
                        }
                        else
                        {
                            if (labeling[k] > 0)
                                fp += 1;
                            else
                                tn += 1;
                        }
                    }

                    int[] countsPred = CountPred(graph, labeling);
                    int[] countsRef = CountPred(graph, graph.Data.ReferenceLabeling);
                }
                var sensitivity = tp / (tp + fn);
                var specificity = tn / (tn + fp);
                mcc2 = (tp * tn - fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));

                Log.Post("New MCC(IChanged): " + Math.Round(mcc, 5) + " Sens: " + Math.Round(sensitivity, 5) + " Spec: " + Math.Round(specificity, 5));
            }

            if (mcc2 > mcc)
            {
                ValueMapRasa.lastChanges.Clear();
                ValueMapEMax01.lastChanges.Clear();
                ValueMapEMax11.lastChanges.Clear();
                ValueMapEDiff01.lastChanges.Clear();
                ValueMapEDiff11.lastChanges.Clear();
                Log.Post("Intervals changed");

                MemoryPoints.RemoveAt(MemoryPoints.Count - 1);
            }
            else
            {
                ValueMapRasa.UndoLastChanges();
                ValueMapEMax01.UndoLastChanges();
                ValueMapEMax11.UndoLastChanges();
                ValueMapEDiff01.UndoLastChanges();
                ValueMapEDiff11.UndoLastChanges();

                if (MemoryPoints.Count == 1)
                    MemoryPoints.Add(ReferencePoint);

                var deltaomega = new double[weights.Length];
                for (int k = 0; k < MemoryPoints.Count - 1; k++)
                {
                    var pointOne = MemoryPoints[k];
                    for (int l = k + 1; l < MemoryPoints.Count; l++)
                    {
                        var pointTwo = MemoryPoints[l];
                        int[] countsRefMinusPred = new int[weights.Length];
                        for (int m = 0; m < weights.Length; m++)
                        {
                            countsRefMinusPred[m] = (pointOne.Counts[m] - pointTwo.Counts[m]);
                        }
                        var weightedScore = 0.0;
                        for (int m = 0; m < weights.Length; m++)
                        {
                            weightedScore += weights[m] * (countsRefMinusPred[m]);
                        }
                        double l2normsq = (countsRefMinusPred.Sum(entry => entry * entry));

                        var loss = 100 * (pointOne.Score - pointTwo.Score);

                        var deltaomegaFactor = (loss - weightedScore) / (l2normsq);
                        for (int m = 0; m < weights.Length; m++)
                        {
                            if (l2normsq > 0)
                                deltaomega[m] += deltaomegaFactor * countsRefMinusPred[m];
                        }
                    }
                }

                //normalize 
                var normFactor = MemoryPoints.Count * (MemoryPoints.Count - 1) / 2;
                for (int m = 0; m < weights.Length; m++)
                {
                    deltaomega[m] /= normFactor;
                }

                for (int k = 0; k < weights.Length; k++)
                {
                    weights[k] += deltaomega[k];
                }
            }

            return weights;
        }

        internal override void SetStartingWeights()
        {
            throw new NotImplementedException();
        }
    }
}
