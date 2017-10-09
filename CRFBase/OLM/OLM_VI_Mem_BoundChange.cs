
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
        //public void Do(int weights, IEnumerable<IGWGraph<NodeData, EdgeData, GraphData>> graphs, int maxIterations)
        //{
        //    int validationQuantils = 5;
        //    double quantilratio = 1.0 / validationQuantils;
        //    var quantiledGraphs = new List<IGWGraph<NodeData, EdgeData, GraphData>>[validationQuantils];

        //    for (int i = 0; i < validationQuantils; i++)
        //    {
        //        quantiledGraphs[i] = new List<IGWGraph<NodeData, EdgeData, GraphData>>();
        //    }

        //    //divide graphs in training / validation
        //    foreach (var graph in graphs)
        //    {
        //        var quantil = random.Next(validationQuantils);
        //        quantiledGraphs[quantil].Add(graph);
        //    }

        //    for (int quantilIteration = 0; quantilIteration < validationQuantils; quantilIteration++)
        //    {
        //        TrainingGraphs = new List<IGWGraph<NodeData, EdgeData, GraphData>>();
        //        ValidationGraphs = new List<IGWGraph<NodeData, EdgeData, GraphData>>();

        //        CoreResiduesValidation = 0;
        //        CoreResiduesTraining = 0;

        //        for (int quantil = 0; quantil < validationQuantils; quantil++)
        //        {
        //            if (quantil == quantilIteration)
        //            {
        //                ValidationGraphs.AddRange(quantiledGraphs[quantil]);
        //                CoreResiduesValidation += quantiledGraphs[quantil].Sum(g => g.Data.CoreResidues);
        //            }
        //            else
        //            {
        //                TrainingGraphs.AddRange(quantiledGraphs[quantil]);
        //                CoreResiduesTraining += quantiledGraphs[quantil].Sum(g => g.Data.CoreResidues);
        //            }
        //        }

        //        //create ValueMap
        //        ValueMapRasa = new IntervalValueMap<NodeData, EdgeData, GraphData>();
        //        ValueMapEMax01 = new IntervalValueMap<NodeData, EdgeData, GraphData>();
        //        ValueMapEMax11 = new IntervalValueMap<NodeData, EdgeData, GraphData>();
        //        ValueMapEDiff01 = new IntervalValueMap<NodeData, EdgeData, GraphData>();
        //        ValueMapEDiff11 = new IntervalValueMap<NodeData, EdgeData, GraphData>();
        //        var valuesRasa = new LinkedList<double>();
        //        var valuesEMax = new LinkedList<double>();
        //        var valuesEDiff = new LinkedList<double>();
        //        valuesRasa.AddLast(0.0);
        //        valuesEMax.AddLast(0.0);
        //        valuesEDiff.AddLast(0.0);
        //        valuesRasa.AddLast(double.MaxValue);
        //        valuesEMax.AddLast(double.MaxValue);
        //        valuesEDiff.AddLast(double.MaxValue);
        //        foreach (var graph in TrainingGraphs)
        //        {
        //            foreach (var node in graph.Nodes)
        //            {
        //                valuesRasa.AddLast(node.Data.RASA);
        //            }
        //            foreach (var edge in graph.Edges)
        //            {
        //                valuesEMax.AddLast(edge.Data.Max);
        //                valuesEDiff.AddLast(edge.Data.Diff);
        //            }
        //        }
        //        ValueMapRasa.values = valuesRasa.Distinct().OrderBy(d => d).ToArray();
        //        ValueMapEMax01.values = valuesEMax.Distinct().OrderBy(d => d).ToArray();
        //        ValueMapEMax11.values = valuesEMax.Distinct().OrderBy(d => d).ToArray();
        //        ValueMapEDiff01.values = valuesEDiff.Distinct().OrderBy(d => d).ToArray();
        //        ValueMapEDiff11.values = valuesEDiff.Distinct().OrderBy(d => d).ToArray();
        //        for (int mr = 0; mr < BasisMerkmale.Length; mr++)
        //        {
        //            var merkmal = BasisMerkmale[mr];
        //            if (mr < Intervals - 1)
        //            {
        //                var divisor = new Divisor<NodeData, EdgeData, GraphData>(merkmal.UpperBoundary, merkmal, BasisMerkmale[mr + 1]);
        //                divisor.Index = ValueMapRasa.values.TakeWhile(entry => entry <= divisor.Value).Count();
        //                divisor.Values = ValueMapRasa.values;
        //                ValueMapRasa.divisors.Add(divisor);
        //            }
        //            else if (mr < Intervals)
        //            {
        //            }
        //            else if (mr < 2 * Intervals - 1)
        //            {
        //                var divisor = new Divisor<NodeData, EdgeData, GraphData>(merkmal.UpperBoundary, merkmal, BasisMerkmale[mr + 1]);
        //                divisor.Index = ValueMapEMax01.values.TakeWhile(entry => entry <= divisor.Value).Count();
        //                divisor.Values = ValueMapEMax01.values;
        //                ValueMapEMax01.divisors.Add(divisor);
        //            }
        //            else if (mr < 2 * Intervals)
        //            {

        //            }
        //            else if (mr < 3 * Intervals - 1)
        //            {
        //                var divisor = new Divisor<NodeData, EdgeData, GraphData>(merkmal.UpperBoundary, merkmal, BasisMerkmale[mr + 1]);
        //                divisor.Index = ValueMapEMax11.values.TakeWhile(entry => entry <= divisor.Value).Count();
        //                divisor.Values = ValueMapEMax11.values;
        //                ValueMapEMax11.divisors.Add(divisor);
        //            }
        //            else if (mr < 3 * Intervals)
        //            {

        //            }
        //            else if (mr < 4 * Intervals - 1)
        //            {
        //                var divisor = new Divisor<NodeData, EdgeData, GraphData>(merkmal.UpperBoundary, merkmal, BasisMerkmale[mr + 1]);
        //                divisor.Index = ValueMapEDiff01.values.TakeWhile(entry => entry <= divisor.Value).Count();
        //                divisor.Values = ValueMapEDiff01.values;
        //                ValueMapEDiff01.divisors.Add(divisor);
        //            }
        //            else if (mr < 4 * Intervals)
        //            {

        //            }
        //            else if (mr < 5 * Intervals - 1)
        //            {
        //                var divisor = new Divisor<NodeData, EdgeData, GraphData>(merkmal.UpperBoundary, merkmal, BasisMerkmale[mr + 1]);
        //                divisor.Index = ValueMapEDiff11.values.TakeWhile(entry => entry <= divisor.Value).Count();
        //                divisor.Values = ValueMapEDiff11.values;
        //                ValueMapEDiff11.divisors.Add(divisor);
        //            }
        //        }

        //        ValueMapRasa.Init();
        //        ValueMapEMax01.Init();
        //        ValueMapEMax11.Init();
        //        ValueMapEDiff01.Init();
        //        ValueMapEDiff11.Init();

        //        int iteration = 0;
        //        MaxIterations = maxIterations;

        //        var rdm = new Random();
        //        double[] weightOpt = new double[weights];
        //        double[] weightCurrent = new double[weights];
        //        for (int i = 0; i < weights; i++)
        //        {
        //            weightCurrent[i] = 0.0 + 0.02 * rdm.NextDouble() - 0.01;
        //            weightOpt[i] = weightCurrent[i];
        //        }
        //        this.WeightObservationUnit.Init(weightCurrent);
        //        var lossOpt = double.MaxValue;
        //        var lossOptOld = 0.0;
        //        var lossCurrent = 0.0;

        //        OLMTracker = new OLMTracking(weights, new int[] { 1, 3, 5, 8, 12, 20, 50 }, weightCurrent, Name + "OLMTracking.txt");

        //        var interfaceValid = 0;
        //        var noninterfaceValid = 0;

        //        foreach (var graph in ValidationGraphs)
        //        {
        //            interfaceValid += graph.Data.ReferenceLabeling.Sum();
        //            noninterfaceValid += graph.Nodes.Count() - graph.Data.ReferenceLabeling.Sum();
        //        }

        //        var sitesValid = interfaceValid + noninterfaceValid;

        //        while (!(iteration >= MaxIterations))
        //        {
        //            iteration++;

        //            var oldWVector = weightCurrent.ToArray();
        //            weightCurrent = Doiteration(TrainingGraphs, weightCurrent, iteration);


        //            tp = 0; tn = CoreResiduesValidation; fp = 0; fn = 0;
        //            lossCurrent = 0.0;
        //            foreach (var graph in ValidationGraphs)
        //            {
        //                //set scores according to weights
        //                foreach (var node in graph.Nodes)
        //                {
        //                    var scores = new double[Labels];
        //                    node.Data.Scores = scores;
        //                    for (int k = 0; k < weightCurrent.Length; k++)
        //                    {
        //                        for (int label = 0; label < Labels; label++)
        //                        {
        //                            scores[label] += weightCurrent[k] * BasisMerkmale[k].Score(node, label);
        //                        }
        //                    }
        //                }
        //                foreach (var edge in graph.Edges)
        //                {
        //                    var scores = new double[Labels, Labels];
        //                    edge.Data.Scores = scores;
        //                    for (int k = 0; k < weightCurrent.Length; k++)
        //                    {
        //                        for (int label1 = 0; label1 < Labels; label1++)
        //                        {
        //                            for (int label2 = 0; label2 < Labels; label2++)
        //                            {
        //                                scores[label1, label2] += weightCurrent[k] * BasisMerkmale[k].Score(edge, label1, label2);
        //                            }
        //                        }
        //                    }
        //                }

        //                var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, null, Labels, BufferSizeInference);
        //                request.RequestInDefaultContext();

        //                var labeling = request.Solution.Labeling;
        //                lossCurrent += LossFunctionValidation(graph.Data.ReferenceLabeling, labeling);

        //                TrackResults(graph.Data.ReferenceLabeling, labeling);
        //            }
        //            WriterResults();
        //            lossCurrent /= sitesValid;

        //            if (lossCurrent < lossOpt)
        //            {
        //                lossOptOld = lossOpt;
        //                lossOpt = lossCurrent;
        //                weightOpt = weightCurrent;
        //            }

        //            OLMTracker.Track(weightCurrent, lossCurrent);
        //        }
        //    }

        //    OLMTracker.WriteWeights();

        //    //return weightOpt;
        //}
        //double tp = 0, tn = 0, fp = 0, fn = 0;
       
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
        //Random random = new Random();
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
                    var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, null, Labels, BufferSizeInference);
                    request.RequestInDefaultContext();
                    int[] labeling = request.Solution.Labeling;
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

                //var newPoint = new Point(weights, new int[weights.Length], 0.0);
                //MemoryPoints.Add(newPoint);
                //if (MemoryPoints.Count > memoryPoints)
                //    MemoryPoints.RemoveAt(0);

                //ReferencePoint = new Point(weights, new int[weights.Length], 1.0);
                for (int i = 0; i < TrainingGraphs.Count; i++)
                {
                    var graph = TrainingGraphs[i];
                    SetWeightsCRF(weights, graph);

                    //compute labeling with viterbi algorithm
                    var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, null, Labels, BufferSizeInference);
                    request.RequestInDefaultContext();
                    int[] labeling = request.Solution.Labeling;
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
                    //for (int k = 0; k < countsPred.Length; k++)
                    //{
                    //    newPoint.Counts[k] += countsPred[k];
                    //    //ReferencePoint.Counts[k] += countsRef[k];
                    //}
                    //TrackResults(labeling, graph.Data.ReferenceLabeling);
                }
                //WriterResults();
                var sensitivity = tp / (tp + fn);
                var specificity = tn / (tn + fp);
                mcc2 = (tp * tn - fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));
                //newPoint.Score = mcc2;

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

                //if (MemoryPoints.Count >= memoryPoints)
                {
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
                //else
                //{
                //    for (int k = 0; k < weights.Length; k++)
                //    {
                //        weights[k] = 0.02 * random.NextDouble() - 0.01;
                //    }
                //}
                //if (iteration == 1)
                //MemoryPoints.Remove(ReferencePoint);
            }

            return weights;
        }

        internal override void SetStartingWeights()
        {
            throw new NotImplementedException();
        }
    }
}
