using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public static class GWGraphPackageTwo
    {
        private static Random rdm = new Random();
        public static GWGraph<NodeData, EdgeData, GraphData> CategoryGraph<NodeData, EdgeData, GraphData>(int numberNodes, int categorieCount, Func<NodeData> createNodeData = null)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("CategoryGraph (" + numberNodes + "," + categorieCount + ")");
            for (int i = 0; i < numberNodes; i++)
            {
                graph.CreateNode();
            }

            var nodes = graph.Nodes.ToList();

            if(createNodeData != null)
            {
                nodes.ForEach(n => n.Data = createNodeData());
            }

            var categories = new List<GWNode<NodeData, EdgeData, GraphData>>[categorieCount];
            var catsTemp = new int[nodes.Count];
            for (int c = 0; c < categorieCount; c++)
            {
                categories[c] = new List<GWNode<NodeData, EdgeData, GraphData>>();

            }

            for (int i = 0; i < numberNodes; i++)
            {
                var category = rdm.Next(categorieCount);
                catsTemp[i] = category;
                categories[category].Add(nodes[i]);
            }

            for (int i = 0; i < numberNodes; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    var nb = categories[catsTemp[i]][rdm.Next(categories[catsTemp[i]].Count)];
                    if (nb.GraphId != i)
                        graph.CreateEdge(nodes[i], nb);
                }
                if (rdm.NextDouble() < 0.15)
                {
                    var nb = nodes[rdm.Next(nodes.Count)];
                    if (nb.GraphId != i)
                        graph.CreateEdge(nodes[i], nb);
                }
            }
            return graph;
        }
    }
}
