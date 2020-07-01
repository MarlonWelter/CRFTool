using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.IO;
using CRFBase.GibbsSampling;

namespace CRFBase
{
    // olm training with cancel criterium based on difference between viterbi and reference, vector correction based on viterbi
    public class OLM_Vit<NodeData, EdgeData, GraphData> : OLMBase<NodeData, EdgeData, GraphData>
        where NodeData : ICRFNodeData
        where EdgeData : ICRFEdgeData
        where GraphData : ICRFGraphData
    {
        public OLM_Vit(int labels, int bufferSizeInference, IList<BasisMerkmal<NodeData, EdgeData, GraphData>> basisMerkmale,
            Func<int[], int[], double> lossfunctionIteration, Func<int[], int[], double> lossfunctionValidation, double sensitivityFactor, string name)
        {
            Name = name;
            Labels = labels;
            BufferSizeInference = bufferSizeInference;
            LossFunctionIteration = lossfunctionIteration;
            LossFunctionValidation = lossfunctionValidation;
            BasisMerkmale = basisMerkmale.ToArray();
        }

        private const double eps = 0.02;
        private const double delta = 0.25;
        private double lossCumulated = delta * 2;
        private double NumberOfNodes = 0;

        protected override double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration)
        {
            var weights = weightCurrent.ToArray();
            TrainingGraphs = TrainingGraphs.RandomizeOrder().ToList();
            int NumberOfGraphs = TrainingGraphs.Count;
            var vit = new int[NumberOfGraphs][];
            var mcmc = new int[NumberOfGraphs][];
            var refLabel = new int[NumberOfGraphs][];
            lossCumulated = 0;

            Log.Post("#Iteration: " + globalIteration);

            for (int g = 0; g < TrainingGraphs.Count; g++)
            {       
                var graph = TrainingGraphs[g];
                NumberOfNodes = graph.Nodes.Count();

                // Labeling mit Viterbi (MAP)
                var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, Labels, BufferSizeInference);
                request.RequestInDefaultContext();
                int[] labelingVit = request.Solution.Labeling;
                vit[g] = labelingVit;

                // reales labeling
                int[] labeling = graph.Data.ReferenceLabeling;
                refLabel[g] = labeling;

                // Berechnung des realen Fehlers
                var loss = LossFunctionIteration(refLabel[g], vit[g]);
                lossCumulated += loss/NumberOfNodes;

                // set scores according to weights
                SetWeightsCRF(weights, graph);

                int[] countsRef = CountPred(graph, refLabel[g]);
                int[] countsPred = CountPred(graph, vit[g]);
                int[] countsRefMinusPred = new int[weightCurrent.Length];

                for (int k = 0; k < countsRef.Length; k++)
                    countsRefMinusPred[k] += countsRef[k] - countsPred[k];

                double l2norm = (countsRefMinusPred.Sum(entry => entry * entry));

                var deltaomega = new double[weights.Length];
                var weightedScore = 0.0;

                for (int k = 0; k < weights.Length; k++)
                    weightedScore += weights[k] * countsRefMinusPred[k];

                var deltaomegaFactor = (loss - weightedScore) / l2norm;

                for (int k = 0; k < weights.Length; k++)
                {
                    if (l2norm > 0)
                        deltaomega[k] = deltaomegaFactor * countsRefMinusPred[k];
                    deltaomega[k] /= NumberOfGraphs;
                    weights[k] += deltaomega[k];
                }
            }

            // normalize weights
            for (int k = 0; k < weights.Length; k++)
            {
                //weights[k] /= NumberOfGraphs;
                Log.Post("Weight: "+weights[k]);
            }
            lossCumulated /= NumberOfGraphs;
            Log.Post("Loss normalized: " + lossCumulated);
            return weights;
        }

        protected override bool CheckCancelCriteria()
        {
            //return (lossCumulated <= delta) && Iteration>1;
            return Iteration >= MaxIterations;
        }

        internal override void SetStartingWeights()
        {
            weightOpt = new double[Weights];
            weightCurrent = new double[Weights];
            for (int i = 0; i < Weights; i++)
            {
                weightCurrent[i] = 1;
                weightOpt[i] = weightCurrent[i];
            }
        }


        // debug output to visualize labelings with given weigths
        private void printLabelings(int[] vit, int[] mcmc, int[] train, int g)
        {
            String outLabel = "Graph " + g + Environment.NewLine;
            outLabel += "vit:  ";
            foreach (int label in vit)
            {
                outLabel += label.ToString() + " ";
            }
            outLabel += Environment.NewLine + "mcmc: ";
            foreach (int label in mcmc)
            {
                outLabel += label.ToString() + " ";
            }
            outLabel += Environment.NewLine + "real: ";
            foreach (int label in train)
            {
                outLabel += label.ToString() + " ";
            }
            outLabel += Environment.NewLine;
            System.IO.File.AppendAllText("labelsForVisualization.txt", outLabel); //label_list.Select(label=>label.ToString()));
        }
    }
}
