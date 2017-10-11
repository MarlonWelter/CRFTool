using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase
{
    public class CreateTestData
    {
        public static void Execute()
        {
            string folder = "\\Graphs\\";
            int numberNodes = 100;
            int numberGraphs = 100;
            var random = new Random();
            for (int i = 0; i < numberGraphs; i++)
            {
                var graph = GWGraphPackageTwo.CategoryGraph<CRFNodeData, CRFEdgeData, CRFGraphData>(numberNodes, 5);

                graph.Data = new CRFGraphData();
                graph.Data.NumberOfLabels = 2;
                graph.Data.Characteristics = new string[] { "A", "B" };
                graph.Data.ReferenceLabeling = new int[numberNodes];

                foreach (var node in graph.Nodes)
                {
                    node.Data = new CRFNodeData();
                    node.Data.Characteristics = new double[] { random.NextDouble(), random.NextDouble() };
                    node.Data.ReferenceLabel = random.Next(2);
                    graph.Data.ReferenceLabeling[node.GraphId] = node.Data.ReferenceLabel;
                }

                foreach (var edge in graph.Edges)
                {
                    edge.Data = new CRFEdgeData();
                }
                Directory.CreateDirectory(folder);
                graph.SaveAsJSON( "testGraph_" + i);
            }
        }
    }
}
