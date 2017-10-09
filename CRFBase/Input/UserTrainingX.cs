using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.Input
{
    public static class UserTrainingX
    {
        public static IGWGraph<ITrainingInputNodeData, ITrainingInputEdgeData, ITrainingInputGraphData> ParseTrainingData(string file)
        {
            var graph = JSONX.LoadFromJSON<GWGraph<TrainingInputNodeData, TrainingInputEdgeData, TrainingInputGraphData>>(file);


            return graph;
        }

        public static GWGraph<TrainingInputNodeData, TrainingInputEdgeData, TrainingInputGraphData> ExampleData()
        {
            var rdm = new Random();
            var graph = GWGraphpackageOne.GWString<TrainingInputNodeData, TrainingInputEdgeData, TrainingInputGraphData>(6);
            graph.Data = new TrainingInputGraphData();
            graph.Data.ObservationToCRFScoreProbability = new double[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int k = 0; k < 5; k++)
                {
                    graph.Data.ObservationToCRFScoreProbability[i, k] = rdm.NextDouble();
                }
            }

            foreach (var node in graph.Nodes)
            {
                node.Data = new TrainingInputNodeData();
                node.Data.Observation = rdm.Next(5);
            }
            foreach (var edge in graph.Edges)
            {
                edge.Data = new TrainingInputEdgeData();
            }

            return graph;
        }
    }
}
