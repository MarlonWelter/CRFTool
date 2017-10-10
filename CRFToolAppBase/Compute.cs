using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using CRFBase;

namespace CRFToolAppBase {
    class Compute {
        public static double[] computeKeys(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph,
    IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>[] graphs, int i) {

            double sensitivity = 0.0, specificity = 0.0, mcc = 0.0;

            // für jeden graphen: true positives / false positives / true negatives / fn   || 0: negative 1: positive
            int[] tps = computeClassification(graph, graphs[i]);

            int tp = tps[0], tn = tps[1], fp = tps[2], fn = tps[3];
            // Sensitivität / Spezifität / MCC Matthew Correlation Coefficient
            sensitivity = computeTPR(tp, fn);
            specificity = computeSPC(tn, fp);
            mcc = computeMCC(tp, tn, fp, fn);

            double[] keys = { tp, tn, fp, fn, sensitivity, specificity, mcc };

            return keys;
        }

        public static double computeMCC(int tp, int tn, int fp, int fn) {
            double mcc = 0.0;
            mcc = (tp * tn - fp * fn) / System.Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));
            return mcc;
        }

        public static double computeTPR(int tp, int fn) {
            double tpr = 0.0;
            tpr = (double) tp / (tp + fn);
            return tpr;
        }

        public static double computeSPC(int tn, int fp) {
            double spc = 0.0;
            spc = (double) tn / (tn + fp);
            return spc;
        }

        public static int[] computeClassification(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> trueLabeling,
            IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph) {

            int tp = 0, fn = 0, fp = 0, tn = 0;
            int[] labeling = trueLabeling.Data.ReferenceLabeling;

            for (int j = 0; j < labeling.Length; j++) {
                if (graph.Data.ReferenceLabeling[j] > 0) {
                    if (labeling[j] > 0)
                        tp += 1;
                    else
                        fn += 1;
                }
                else {
                    if (labeling[j] > 0)
                        fp += 1;
                    else
                        tn += 1;
                }
            }

            int[] rates = { tp, tn, fp, fn};

            return rates;
        }
    }
}
