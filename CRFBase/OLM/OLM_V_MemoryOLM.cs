using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.IO;

namespace CRFBase
{
    public class OLM_V_MemoryOLM<NodeData, EdgeData, GraphData> : OLMBase<NodeData, EdgeData, GraphData>
        where NodeData : ICRFNodeData
        where EdgeData : ICRFEdgeData
        where GraphData : ProteinCRFGraphData
    {
        public OLM_V_MemoryOLM(int labels, int bufferSizeInference, BasisMerkmal<NodeData, EdgeData, GraphData>[] basisMerkmale, Func<int[], int[], double> lossfunctionIteration, Func<int[], int[], double> lossfunctionValidation, string name, int memoryPoints)
        {
            MemoryPointsCount = memoryPoints;
            Name = name;
            Labels = labels;
            BufferSizeInference = bufferSizeInference;
            LossFunctionIteration = lossfunctionIteration;
            LossFunctionValidation = lossfunctionValidation;
            BasisMerkmale = basisMerkmale;
        }
        protected override bool CheckCancelCriteria()
        {
            return Iteration >= MaxIterations;
        }
        class MemoryPoint
        {
            public MemoryPoint(double[] weights, int[] counts, double mcc)
            {
                Weights = weights;
                Counts = counts;
                Score = mcc;
            }
            public double[] Weights;
            public int[] Counts;
            public double Score
            {
                get; set;
            }
        }
        public bool AddRdmNode { get; set; }

        List<MemoryPoint> MemoryPoints { get; set; } = new List<MemoryPoint>();
        MemoryPoint ReferencePoint;
        public int MemoryPointsCount { get; set; }
        protected override double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration)
        {
            var weights = weightCurrent.ToArray();
            var weightsSum = new double[weightCurrent.Length];
            int iteration = 0;

            if (AddRdmNode && globalIteration % 20 == 0)
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] += 0.0 + 0.2 * random.NextDouble() - 0.1;
                }
            }

            tp = 0; tn = 0; fp = 0; fn = 0;

            var newPoint = new MemoryPoint(weights, new int[weights.Length], 0.0);
            MemoryPoints.Add(newPoint);
            while (MemoryPoints.Count > MemoryPointsCount)
                MemoryPoints.RemoveAt(0);

            ReferencePoint = new MemoryPoint(weights, new int[weights.Length], 1.0);
            for (int i = 0; i < TrainingGraphs.Count; i++)
            {
                var graph = TrainingGraphs[i];
                SetWeightsCRF(weights, graph);

                //compute labeling with viterbi algorithm
                var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, Labels, BufferSizeInference);
                request.RequestInDefaultContext();
                int[] labeling = request.Result.Labeling;
                //check nonequality

                iteration++;

                int[] countsPred = CountPred(graph, labeling);
                int[] countsRef = CountPred(graph, graph.Data.ReferenceLabeling);
                for (int k = 0; k < countsPred.Length; k++)
                {
                    newPoint.Counts[k] += countsPred[k];
                    ReferencePoint.Counts[k] += countsRef[k];
                }
                TrackResults(labeling, graph.Data.ReferenceLabeling);
            }
            var mcc = (tp * tn - fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));
            newPoint.Score = mcc;

            if (globalIteration == 1)
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


            return weights;
        }

        internal override void SetStartingWeights()
        {
            throw new NotImplementedException();
        }
    }
}
