using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.TrainingEvaluationOLM
{
    class PottsModel
    {
        public PottsModel(double[] conformityParameter, double correlationParameter)
        {
            ConformityParameter = conformityParameter;
            CorrelationParameter = correlationParameter;
        }
        public double[] ConformityParameter { get; set; }
        public double CorrelationParameter { get; set; }

        public const int NumberObservations = 2;

        //public void SetInterval(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        //{
        //    foreach(var node in graph.Nodes)
        //    {
        //        node.Data.Interval = 1;
        //    }
        //}

        public void CreateCRFScore(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            foreach (var node in graph.Nodes)
            {
                node.Data.Scores = new double[NumberObservations];
                int interval = 0;
                for (int observationCount = 0; observationCount < NumberObservations; observationCount++)
                {
                    node.Data.Scores[observationCount] = node.Data.Observation == observationCount ? ConformityParameter[interval] : -ConformityParameter[interval];
                }
            }
            foreach (var edge in graph.Edges)
            {
                edge.Data.Scores = new double[NumberObservations, NumberObservations] { { CorrelationParameter, -CorrelationParameter }, { -CorrelationParameter, CorrelationParameter } };
            }
        }
    }
}
