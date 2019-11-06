using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace CRFBase
{
    class ComputeKeys
    {
        public OLMEvaluationGraphResult computeEvalutionGraphResult(GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph,
            int[] predicitionLabeling)
        {
            double sensitivity = 0.0, specificity = 0.0, mcc = 0.0, accuracy = 0.0;
            int[] referenceLabel = graph.Data.ReferenceLabeling;

            // für jeden graphen: true positives / false positives / true negatives / false negatives   || 0: negative 1: positive
            long[] tps = computeClassification(referenceLabel, predicitionLabeling);
            long tp = tps[0], tn = tps[1], fp = tps[2], fn = tps[3];

            // Sensitivität / Spezifität / MCC Matthew Correlation Coefficient
            sensitivity = computeTPR(tp, fn);
            specificity = computeSPC(tn, fp);
            mcc = computeMCC(tp, tn, fp, fn);
            accuracy = computeAcc(tp, tn, fp, fn);

            OLMEvaluationGraphResult result = new OLMEvaluationGraphResult(graph, predicitionLabeling, tp, tn, fp, fn, sensitivity, specificity, mcc, accuracy);

            return result;
        }

        private long[] computeClassification(int[] referenceLabel, int[] trainingLabel)
        {
            long tp = 0, fn = 0, fp = 0, tn = 0;

            for (int j = 0; j < trainingLabel.Length; j++)
            {
                if (referenceLabel[j] > 0)
                {
                    if (trainingLabel[j] > 0)
                        tp += 1;
                    else
                        fn += 1;
                }
                else
                {
                    if (trainingLabel[j] > 0)
                        fp += 1;
                    else
                        tn += 1;
                }
            }

            long[] rates = { tp, tn, fp, fn };

            return rates;
        }

        public double computeMCC(long tp, long tn, long fp, long fn)
        {
            double mcc = 0.0;
            mcc = (tp * tn - fp * fn) / System.Math.Sqrt((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn));
            if (mcc.ToString() == "n. def.")
                mcc = 0;
            if (mcc.ToString() == "NaN")
                mcc = 0;
            return mcc;
        }

        public double computeSPC(long tn, long fp)
        {
            double spc = 0.0;
            spc = (double)tn / (tn + fp);
            return spc;
        }

        public double computeTPR(long tp, long fn)
        {
            double tpr = 0.0;
            tpr = (double)tp / (tp + fn);
            return tpr;
        }

        public double computeAcc(long tp, long tn, long fp, long fn)
        {
            double acc = 0.0;
            acc = (double)(tp + tn) / (tp + fp + fn + tn);
            return acc;
        }
    }
}
