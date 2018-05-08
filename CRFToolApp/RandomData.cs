using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolApp
{
    class RandomData
    {
        static Random random = new Random();
        public static void AddDefault(IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph)
        {
            AddCharacteristic(graph);
            AddScore(graph);
            AddObservation(graph);
            AddReference(graph);
            AddViterbi(graph);
            AddSample(graph);
        }
        public static void AddCharacteristic(IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph)
        {
            foreach (var node in graph.Nodes)
            {
                node.Data.Characteristics = new double[] { random.NextDouble() };
            }
        }
        public static void AddScore(IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph)
        {
            foreach (var node in graph.Nodes)
            {
                node.Data.Scores = new double[] { random.NextDouble(), random.NextDouble() };
            }
            foreach (var edge in graph.Edges)
            {
                edge.Data.Scores = new double[,] { { random.NextDouble(), random.NextDouble() } , { random.NextDouble(), random.NextDouble() } };
            }
        }
        public static void AddObservation(IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph)
        {
            foreach (var node in graph.Nodes)
            {
                node.Data.Observation = random.Next(2);
            }
        }
        public static void AddReference(IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph)
        {
            foreach (var node in graph.Nodes)
            {
                node.Data.ReferenceLabel = random.Next(2);
            }
        }

        public static void AddViterbi(IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph)
        {
            graph.Data.Viterbi = new int[graph.Nodes.Count()];
            foreach (var node in graph.Nodes)
            {
                graph.Data.Viterbi[node.GraphId] = random.Next(2);
            }
        }

        public static void AddSample(IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph)
        {
            graph.Data.Sample = new List<int[]>();
            for (int i = 0; i < 100; i++)
            {
                graph.Data.Sample.Add(new int[graph.Nodes.Count()]); 
                foreach (var node in graph.Nodes)
                {
                    graph.Data.Sample[i][node.GraphId] = random.Next(2);
                }
            }
            
        }
    }
}
