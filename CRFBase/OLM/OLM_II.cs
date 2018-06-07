using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.IO;

namespace CRFBase
{
    public class OLM_II<NodeData, EdgeData, GraphData> : OLMBase<NodeData, EdgeData, GraphData>
        where NodeData : ICRFNodeData
        where EdgeData : ICRFEdgeData
        where GraphData : ProteinCRFGraphData
    {
        public OLM_II(double alpha, int labels, int bufferSizeInference, BasisMerkmal<NodeData, EdgeData, GraphData>[] basisMerkmale, Func<int[], int[], double> lossfunctionIteration, Func<int[], int[], double> lossfunctionValidation, double sensitivityFactor, string name)
        {
            Name = name;
            Alpha = alpha;
            Labels = labels;
            BufferSizeInference = bufferSizeInference;
            LossFunctionIteration = lossfunctionIteration;
            LossFunctionValidation = lossfunctionValidation;
            BasisMerkmale = basisMerkmale;
            SensitivityFactor = sensitivityFactor;
            WeightObservationUnit_II = new WeightObservationUnit_II(basisMerkmale.Length, 0.1);
        }
        public double SensitivityFactor { get; set; }
        public double Alpha { get; set; }

        public double[] Impulses { get; set; }
        public double[] LastChanges { get; set; }
        public List<double> Changes { get; set; }



        private double mccMax = 0.0;
        protected override double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration)
        {
            var weights = weightCurrent.ToArray();
            var weightsSum = new double[weightCurrent.Length];
            int iteration = 0;

            double tp = 0.001, tn = 0.001 + 0, fp = 0.001, fn = 0.001;

            int[] countsRefMinusPred = new int[weightCurrent.Length];

            WeightObservationUnit_II.Update(weights);

            for (int i = 0; i < TrainingGraphs.Count; i++)
            {
                var graph = TrainingGraphs[i];
                //set scores according to weights
                SetWeightsCRF(weights, graph);

                //compute labeling with viterbi algorithm
                var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, Labels, BufferSizeInference);
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

            var mcc = (tp * tn + fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));
            mccMax = Math.Max(mccMax, mcc);
            
            return weights;
        }

        protected override bool CheckCancelCriteria()
        {
            return Iteration >= MaxIterations;
        }

        internal override void SetStartingWeights()
        {
            throw new NotImplementedException();
        }
    }
}

