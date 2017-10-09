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
    public class OLM_Ising_I<NodeData, EdgeData, GraphData> : OLMBase<NodeData, EdgeData, GraphData>
        where NodeData : ICRFNodeData
        where EdgeData : ICRFEdgeData
        where GraphData : ICRFGraphData
    {
        public OLM_Ising_I(int labels, int bufferSizeInference, BasisMerkmal<NodeData, EdgeData, GraphData>[] basisMerkmale, Func<int[], int[], double> lossfunctionIteration, Func<int[], int[], double> lossfunctionValidation, double sensitivityFactor, string name)
        {
            Name = name;
            Labels = labels;
            BufferSizeInference = bufferSizeInference;
            LossFunctionIteration = lossfunctionIteration;
            LossFunctionValidation = lossfunctionValidation;
            BasisMerkmale = basisMerkmale;
        }

        private const double eps = 0.01;
        // mittlerer Fehler
        private double middev = 0;
        // realer Fehler
        private double realdev = 2*eps;
        protected override double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration)
        {

            var weights = weightCurrent.ToArray();

            Log.Post("Conformity: " + weights[0] + " Correlation: " + weights[1]);

            int u = TrainingGraphs.Count;
            var vit = new int[u][];
            var mcmc = new int[u][];
            var dev = 0;
            double devges = 0.0;
            // Anzahl Knoten
            double mx = 0;
            var train = new int[u][];
            double devgesT = 0;
            double devT = 0;
            // Summe aller Knoten aller Graphen
            double mu = 0;

            int[] countsMCMCMinusRef = new int[weightCurrent.Length];
            int[] countsRefMinusMCMC = new int[weightCurrent.Length];

            for (int g = 0; g < TrainingGraphs.Count; g++)
            {
                var graph = TrainingGraphs[g];
                mx = graph.Nodes.Count();
                mu += mx;
                // Labeling mit Viterbi (MAP)
                var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, null, Labels, BufferSizeInference);
                request.RequestInDefaultContext();
                int[] labelingVit = request.Solution.Labeling;
                vit[g] = labelingVit;
                // Labeling mit MCMC basierend auf MAP
                var requestMCMC = new GiveProbableLabelings(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>) { StartingPoints = 1, PreRunLength = 100000, RunLength = 1 };
                requestMCMC.RequestInDefaultContext();
                var result = requestMCMC.Result;
                int[] labelingMCMC = new int[labelingVit.Length];
                foreach (var item in result)
                {
                    labelingMCMC[item.Key.GraphId] = (int)item.Value;
                    //TODO: check not all 0's
                }
                mcmc[g] = labelingMCMC;

                // Berechnung des typischen/mittleren Fehlers
                dev = 0;
                for (int n = 0; n < TrainingGraphs[g].Nodes.Count(); n++)
                {
                    dev += vit[g][n] == mcmc[g][n] ? 0 : 1;
                }
                devges += ((double)dev) / mx;

                // set scores according to weights
                SetWeightsCRF(weights, graph);

                // reales labeling
                int[] labeling = graph.Data.ReferenceLabeling;
                train[g] = labeling;

                devT = 0;
                for (int n = 0; n < graph.Nodes.Count(); n++)
                {
                    devT += vit[g][n] == train[g][n] ? 0 : 1;
                }
                devgesT += devT / mx;

                // calculate Basismerkmale 
                // TODO: check if correct
                int[] countsRef = CountPred(graph, train[g]);
                int[] countsMCMC = CountPred(graph, mcmc[g]);

                for (int k = 0; k < countsRef.Length; k++)
                {
                    countsMCMCMinusRef[k] += countsMCMC[k] - countsRef[k];
                    countsRefMinusMCMC[k] += countsRef[k] - countsMCMC[k];
                }
            }

            // mittlerer (typischer) Fehler
            middev = devges / u;

            // realer Fehler fuer diese Runde
            realdev = devgesT / u;

            var loss = (realdev - middev) * mu;

            // Scores berechnen?? Im Skript so, aber nicht notwendig

            double l2norm = (countsRefMinusMCMC.Sum(entry => entry * entry));

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
                weights[k] += deltaomega[k];
            }

            // debug output
            Log.Post("Loss: " + (int)loss + " Realdev: " + realdev + " Middev: " + middev);

            return weights;


            // old OLM
            //for (int i = 0; i < TrainingGraphs.Count; i++)
            //{
            //    var graph = TrainingGraphs[i];
            //    //set scores according to weights
            //    SetWeightsCRF(weights, graph);

            //    //compute labeling with viterbi algorithm
            //    var request = new SolveInference(graph as IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>, null, Labels, BufferSizeInference);
            //    request.RequestInDefaultContext();
            //    int[] labeling = request.Solution.Labeling;
            //    //check nonequality

            //    //iteration++;

            //    //for (int k = 0; k < labeling.Length; k++)
            //    //{
            //    //    if (graph.Data.ReferenceLabeling[k] > 0)
            //    //    {
            //    //        if (labeling[k] > 0)
            //    //            tp += 1;
            //    //        else
            //    //            fn += 1;
            //    //    }

            //    //    else
            //    //    {
            //    //        if (labeling[k] > 0)
            //    //            fp += 1;
            //    //        else
            //    //            tn += 1;
            //    //    }
            //    //}

            //    //var loss = LossFunctionIteration(labeling, graph.Data.ReferenceLabeling);
            //    int[] countsPred = CountPred(graph, labeling);
            //    int[] countsRef = CountPred(graph, graph.Data.ReferenceLabeling);
            //    for (int k = 0; k < countsPred.Length; k++)
            //    {
            //        countsRefMinusPred[k] += countsRef[k] - countsPred[k];
            //    }

            //}

            //var weightedScore = 0.0;
            //for (int k = 0; k < weights.Length; k++)
            //{
            //    weightedScore += weights[k] * countsRefMinusPred[k] - weights[k] * TrainingGraphs.Sum(gr => gr.Nodes.Count());
            //}
            //double l2normsq = (countsRefMinusPred.Sum(entry => entry * entry));

            //var mcc = (tp * tn + fp * fn) / Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));
            //mccMax = Math.Max(mccMax, mcc);
            //var losss = (mccMax - mcc) * lossFunctionFactorMCC;

            //var deltaomegaFactor = (losss - weightedScore) / l2normsq;
            //var deltaomega = new double[weights.Length];
            //for (int k = 0; k < weights.Length; k++)
            //{
            //    if (l2normsq > 0)
            //        deltaomega[k] = deltaomegaFactor * countsRefMinusPred[k];
            //    weights[k] += deltaomega[k];
            //    //weightsSum[k] = weights[k] + deltaomega[k];
            //}


            //var weightChanges = new double[weights.Length];
            //for (int k = 0; k < weights.Length; k++)
            //{
            //    weightChanges[k] = (weights[k]) - weightCurrent[k];
            //}
            //weights = this.WeightObservationUnit.ApplyChangeVector(weightChanges, weightCurrent);
            //return weights;
        }

        protected override bool CheckCancelCriteria()
        {
            return (realdev <= middev + eps) && (realdev >= middev - eps); // && Iteration >= 20;
        }

        internal override void SetStartingWeights()
        {
            weightOpt = new double[Weights];
            weightCurrent = new double[Weights];
            for (int i = 0; i < Weights; i++)
            {
                weightCurrent[i] = 1.0;
                weightOpt[i] = weightCurrent[i];
            }
        }
    }
}
