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
    // online version of large margin training (bachelor thesis) with cancel criterion based on mcmc deviations, vector correction based on mcmc deviations
    public class OLM_Ising_I<NodeData, EdgeData, GraphData> : OLMBase<NodeData, EdgeData, GraphData>
        where NodeData : ICRFNodeData
        where EdgeData : ICRFEdgeData
        where GraphData : ICRFGraphData
    {
        public OLM_Ising_I(int labels, int bufferSizeInference, IList<BasisMerkmal<NodeData, EdgeData, GraphData>> basisMerkmale,
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
        // mittlerer Fehler
        private double middev = 0;
        double middevCumulated = 0.0;
        // realer Fehler
        private double realdev = 2 * eps;
        double realdevCumulated = 2 * eps;
        private bool debugOutputEnabled = false;

        protected override double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration)
        {
            var weights = weightCurrent.ToArray();
            int NumberOfGraphs = TrainingGraphs.Count;
            var vit = new int[NumberOfGraphs][];
            var mcmc = new int[NumberOfGraphs][];
            var refLabel = new int[NumberOfGraphs][];
            // Anzahl Knoten
            double NumberOfNodes = 0;
            realdevCumulated = 0.0;
            middevCumulated = 0.0;
            // Summe aller Knoten aller Graphen
            //double SumOfNodes = 0;

            Log.Post("#Iteration: " + globalIteration);

            for (int g = 0; g < TrainingGraphs.Count; g++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var elapsedMs = 0.0;

                var graph = TrainingGraphs[g];
                NumberOfNodes = graph.Nodes.Count();
                //SumOfNodes += NumberOfNodes;
                // Labeling mit Viterbi (MAP)
                var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, Labels, BufferSizeInference);
                request.RequestInDefaultContext();
                int[] labelingVit = request.Solution.Labeling;
                vit[g] = labelingVit;

                // Labeling mit MCMC basierend auf MAP
                var requestMCMC = new GiveProbableLabelings(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>) { StartingPoints = 1, PreRunLength = 100000, RunLength = 1 };
                requestMCMC.RequestInDefaultContext();
                var result = requestMCMC.Result;
                int[] labelingMCMC = new int[labelingVit.Length];
                foreach (var item in result)
                    labelingMCMC[item.Key.GraphId] = (int)item.Value;
                mcmc[g] = labelingMCMC;

                // reales labeling
                int[] labeling = graph.Data.ReferenceLabeling;
                refLabel[g] = labeling;

                // Berechnung des typischen/mittleren Fehlers
                middev = LossFunctionIteration(refLabel[g], mcmc[g]);
                middev /= NumberOfNodes;
                middevCumulated += middev;
                realdev = LossFunctionIteration(refLabel[g], vit[g]);
                realdev /= NumberOfNodes;
                realdevCumulated += realdev;

                // set scores according to weights
                SetWeightsCRF(weights, graph);

                if (debugOutputEnabled)
                    printLabelings(vit[g], mcmc[g], refLabel[g], g);

                // calculate equation 6.13 and 6.14
                int[] countsRef = CountPred(graph, refLabel[g]);
                int[] countsMCMC = CountPred(graph, mcmc[g]);

                int[] countsMCMCMinusRef = new int[weightCurrent.Length];
                int[] countsRefMinusMCMC = new int[weightCurrent.Length];

                for (int k = 0; k < countsRef.Length; k++)
                {
                    countsMCMCMinusRef[k] += countsMCMC[k] - countsRef[k];
                    countsRefMinusMCMC[k] += countsRef[k] - countsMCMC[k];
                }

                double loss = (realdev - middev);

                long l2norm = (countsRefMinusMCMC.Sum(entry => entry * entry));

                var deltaomegaFactor = 0.0;
                var deltaomega = new double[weights.Length];
                var weightedScore = 0.0;

                for (int k = 0; k < weights.Length; k++)
                {
                    if (l2norm > 0)
                    {
                        weightedScore += weights[k] * countsMCMCMinusRef[k];
                        deltaomegaFactor = (loss + weightedScore) / l2norm;
                        deltaomega[k] = deltaomegaFactor * countsRefMinusMCMC[k];
                    }
                    else
                    {
                        weightedScore += weights[k] * countsRefMinusMCMC[k];
                        deltaomegaFactor = (loss + weightedScore) / l2norm;
                        deltaomega[k] = deltaomegaFactor * countsMCMCMinusRef[k];
                    }
                    deltaomega[k] /= NumberOfGraphs;
                    weights[k] += deltaomega[k];
                }

                watch.Stop();
                elapsedMs = watch.ElapsedMilliseconds;

                // debug output
                Log.Post("Elapsed time: " + elapsedMs);
                System.IO.File.AppendAllText("times.txt", elapsedMs.ToString());
                System.IO.File.AppendAllText("times.txt", "\n");
                //Log.Post("Loss: " + loss + " Realdev: " + realdev + " Middev: " + middev);
            }

            // normalize weights
            foreach (var weight in weights)
            {
                //weights[i] /= NumberOfGraphs;
                Log.Post("Weight: " + weight);
            }
            middevCumulated /= NumberOfGraphs;
            realdevCumulated /= NumberOfGraphs;
            Log.Post("Middev normalized: " + middevCumulated + " Realdev normalized: " + realdevCumulated);

            return weights;
        }

        protected override bool CheckCancelCriteria()
        {
            //return ((realdevCumulated <= middevCumulated + eps) && (realdevCumulated >= middevCumulated - eps)) && Iteration>1;
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
