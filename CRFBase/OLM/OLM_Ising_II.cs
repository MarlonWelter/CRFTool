﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.IO;
using CRFBase.GibbsSampling;

namespace CRFBase
{
    // olm training with cancel criterium based on difference between mcmc and ref, vector correction based on viterbi
    public class OLM_Ising_II<NodeData, EdgeData, GraphData> : OLMBase<NodeData, EdgeData, GraphData>
        where NodeData : ICRFNodeData
        where EdgeData : ICRFEdgeData
        where GraphData : ICRFGraphData
    {
        public OLM_Ising_II(int labels, int bufferSizeInference, IList<BasisMerkmal<NodeData, EdgeData, GraphData>> basisMerkmale,
            Func<int[], int[], double> lossfunctionIteration, Func<int[], int[], double> lossfunctionValidation, double sensitivityFactor, string name)
        {
            Name = name;
            Labels = labels;
            BufferSizeInference = bufferSizeInference;
            LossFunctionIteration = lossfunctionIteration;
            LossFunctionValidation = lossfunctionValidation;
            BasisMerkmale = basisMerkmale.ToArray();
        }

        private const double eps = 0.01;
        private const double delta = 0.2;
        // mittlerer Fehler
        private double middev = delta * 2;
        // realer Fehler
        private double realdev = 2 * eps;
        private bool debugOutputEnabled = false;

        protected override double[] DoIteration(List<IGWGraph<NodeData, EdgeData, GraphData>> TrainingGraphs, double[] weightCurrent, int globalIteration)
        {

            var weights = weightCurrent.ToArray();

            int u = TrainingGraphs.Count;
            var vit = new int[u][];
            var mcmc = new int[u][];
            var dev = 0;
            double devges = 0.0;
            // Anzahl Knoten
            double mx = 0;
            var refLabel = new int[u][];
            double devgesT = 0;
            double devT = 0;
            // Summe aller Knoten aller Graphen
            double mu = 0;
            
            int[] countsRefMinusPred = new int[weightCurrent.Length];

            Log.Post("#Iteration: " + globalIteration);

            for (int g = 0; g < TrainingGraphs.Count; g++)
            {
                var graph = TrainingGraphs[g];
                mx = graph.Nodes.Count();
                mu += mx;
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
                {
                    labelingMCMC[item.Key.GraphId] = (int)item.Value;
                    //TODO: check not all 0's
                }
                mcmc[g] = labelingMCMC;


                // reales labeling
                int[] labeling = graph.Data.ReferenceLabeling;
                refLabel[g] = labeling;

                // Berechnung des typischen/mittleren Fehlers
                dev = 0;
                for (int n = 0; n < TrainingGraphs[g].Nodes.Count(); n++)
                {
                    dev += refLabel[g][n] == mcmc[g][n] ? 0 : 1;
                }
                devges += ((double)dev) / mx;

                // set scores according to weights
                SetWeightsCRF(weights, graph);

                if (debugOutputEnabled)
                {
                    printLabelings(vit[g], mcmc[g], refLabel[g], g);
                }

                devT = 0;
                for (int n = 0; n < graph.Nodes.Count(); n++)
                {
                    devT += vit[g][n] == refLabel[g][n] ? 0 : 1;
                }
                devgesT += devT / mx;
                

                int[] countsRef = CountPred(graph, refLabel[g]);
                int[] countsPred = CountPred(graph, vit[g]);

                for (int k = 0; k < countsRef.Length; k++)
                {
                    countsRefMinusPred[k] += countsRef[k] - countsPred[k];
                }
            }

            // mittlerer (typischer) Fehler (Summen-Gibbs-Score)
            middev = devges / u;

            // realer Fehler fuer diese Runde (Summen-Trainings-Score)
            realdev = devgesT / u;

            var loss = realdev * mu;

            // Scores berechnen?? Im Skript so, aber nicht notwendig

            double l2norm = (countsRefMinusPred.Sum(entry => entry * entry));
            
            var deltaomega = new double[weights.Length];
            var weightedScore = 0.0;

            for (int k = 0; k < weights.Length; k++)
            {
                weightedScore += weights[k] * countsRefMinusPred[k];
            }
            var deltaomegaFactor = (loss - weightedScore) / l2norm;

            for (int k = 0; k < weights.Length; k++)
            {
                if (l2norm > 0)
                    deltaomega[k] = deltaomegaFactor * countsRefMinusPred[k];
                else
                {
                    Log.Post("wiu wiu");
                    deltaomega[k] = 0;
                }                    
                weights[k] += deltaomega[k];
            }

            // debug output
            Log.Post("Loss: " + (int)loss + " Realdev: " + realdev + " Middev: " + middev);

            return weights;
        }

        protected override bool CheckCancelCriteria()
        {
            return (middev <= delta);
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
