using CodeBase;
using System;
using System.Collections.Generic;

namespace CRFBase
{
    public class OLMEvaluationResult
    {
        public List<OLMEvaluationGraphResult> GraphResults { get; set; } = new List<OLMEvaluationGraphResult>();
        public double CorrelationParameter { get; set; }
        public double ConformityParameter { get; set; }
        public double[] ConformityParameters { get; set; }

        public double AverageSensitivity { get; set; }
        public double AverageSpecificity { get; set; }
        public double AverageMCC { get; set; }
        public double TotalTP { get; set; }
        public double AverageAccuracy { get; private set; }
        public double TotalTN { get; private set; }
        public double TotalFP { get; private set; }
        public double TotalFN { get; private set; }

        // ...

        public void ComputeValues(OLMEvaluationResult result)
        {
            //hier die Liste von GraphResults durchgehen und die Average-Werte etc berechnen
            double avSensitivity = 0, avSpecificity = 0, avMCC = 0, totalTP = 0, totalTN = 0, totalFP = 0, totalFN = 0, avAccuracy = 0;
            int i = 0;
            foreach (OLMEvaluationGraphResult graph in result.GraphResults)
            {
                avSensitivity += graph.Sensitivity;
                avSpecificity += graph.Specificity;
                avMCC += graph.MCC;
                avAccuracy += graph.Accuracy;
                totalTP += graph.TP;
                totalTN += graph.TN;
                totalFP += graph.FP;
                totalFN += graph.FN;
                i++;
            }
            result.AverageSensitivity = Math.Round(avSensitivity / result.GraphResults.Count, 3);
            result.AverageSpecificity = Math.Round(avSpecificity / result.GraphResults.Count, 3);
            result.AverageMCC = Math.Round(avMCC / result.GraphResults.Count, 3);
            result.AverageAccuracy = Math.Round(avAccuracy / result.GraphResults.Count, 3);
            result.TotalTP = totalTP;
            result.TotalTN = totalTN;
            result.TotalFP = totalFP;
            result.TotalFN = totalFN;
        }
    }
    public class OLMEvaluationGraphResult
    {
        public OLMEvaluationGraphResult() { }
        public OLMEvaluationGraphResult(GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph, int[] prediction, int tp, int tn, int fp, int fn, double sensitivity, double specificity, double mcc, double accuracy)
        {
            Graph = graph;
            Prediction = prediction;
            TP = tp;
            TN = tn;
            FP = fp;
            FN = fn;
            Sensitivity = sensitivity;
            Specificity = specificity;
            MCC = mcc;
            Accuracy = accuracy;
        }
        // amount of the true positives
        public int TP { get; set; }
        // amount of the true negatives
        public int TN { get; set; }
        // amount of false positives
        public int FP { get; set; }
        // amount of false negatives
        public int FN { get; set; }
        //  true positive rate (TPR)
        public double Sensitivity { get; set; }
        // true negative rate
        public double Specificity { get; set; }
        // Matthew Correlation Coefficient
        public double MCC { get; set; }

        public GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> Graph { get; set; }
        public int[] Prediction { get; set; }

        public double Accuracy { get; private set; }
    }
}