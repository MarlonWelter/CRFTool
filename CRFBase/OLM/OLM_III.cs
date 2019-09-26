using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.IO;

namespace CRFBase
{
    public class OLM_III<NodeData, EdgeData, GraphData> : OLMBase<NodeData, EdgeData, GraphData>
        where NodeData : ICRFNodeData
        where EdgeData : ICRFEdgeData
        where GraphData : ICRFGraphData
    {
        // default OLM algorithm
        public OLM_III(int labels, int bufferSizeInference, IList<BasisMerkmal<NodeData, EdgeData, GraphData>> basisMerkmale, Func<int[], int[], double> lossfunctionIteration, Func<int[], int[], double> lossfunctionValidation, double sensitivityFactor, string name)
        {
            Name = name;
            Labels = labels;
            BufferSizeInference = bufferSizeInference;
            LossFunctionIteration = lossfunctionIteration;
            LossFunctionValidation = lossfunctionValidation;
            BasisMerkmale = basisMerkmale.ToArray();
        }

        protected override bool CheckCancelCriteria()
        {
            return Iteration >= MaxIterations;
        }

        protected override double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration)
        {
            var weights = weightCurrent.ToArray();
            var weightsSum = new double[weightCurrent.Length];
            int iteration = 0;

            double tp = 0.001, tn = 0.001, fp = 0.001, fn = 0.001;

            int[] countsRefMinusPred = new int[weightCurrent.Length];

            for (int i = 0; i < TrainingGraphs.Count; i++)
            {
                var graph = TrainingGraphs[i];
                //set scores according to weights
                SetWeightsCRF(weights, graph);

                //compute labeling with viterbi algorithm
                var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>,  Labels, BufferSizeInference);
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
                    countsRefMinusPred[k] += countsRef[k] - countsPred[k];
                }

            }

            var weightedScore = 0.0;
            for (int k = 0; k < weights.Length; k++)
            {
                weightedScore += weights[k] * countsRefMinusPred[k];
            }
            double l2normsq = (countsRefMinusPred.Sum(entry => entry * entry));
            
            var loss = (fp + fn);

            var deltaomegaFactor = (loss - weightedScore) / l2normsq;
            var deltaomega = new double[weights.Length];
            for (int k = 0; k < weights.Length; k++)
            {
                if (l2normsq > 0)
                    deltaomega[k] = deltaomegaFactor * countsRefMinusPred[k];
                weights[k] += deltaomega[k];
                weightsSum[k] = weights[k] + deltaomega[k];
            }
            
            return weights;
        }

        internal override void SetStartingWeights()
        {
            weightOpt = new double[Weights];
            weightCurrent = new double[Weights];
            for (int i = 0; i < Weights; i++)
            {
                weightCurrent[i] = 0.0 + 0.2 * rdm.NextDouble() - 0.1;
                weightOpt[i] = weightCurrent[i];
            }
        }
    }
}
