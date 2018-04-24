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
        }
    }
}
