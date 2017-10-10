using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRFBase;
using CodeBase;

namespace SoftwareGraphLearning
{   
    //information about graph
    class GraphView
    {


        public void GetGraphInfo(IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> graph)
        {
            GetErdösGraphInfo(graph);
            GetCategoryGraphInfo(graph.Data.CategoryGraph);
        }



        public void GetErdösGraphInfo(IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> graph)
        {   

            if (graph == null)
            {
                Console.WriteLine("No Graph exists");
                Console.WriteLine("\n");
                return;
            }
            Console.WriteLine("ErdösGraph:");
            Console.WriteLine("Number Nodes: " + graph.Nodes.Count());
            Console.WriteLine("Number Categories: " + graph.Data.NumberCategories);

            string[] nodes_per_category = new string[graph.Data.NumberCategories];
            foreach (var node in graph.Nodes)
            {
                nodes_per_category[node.Data.Category] += node.GraphId + "-" + node.Data.Observation +" | ";
            }
            int i = 0;
            foreach (string s in nodes_per_category)
            {
                Console.WriteLine("Category {0} (Node-Observation): {1}", i++, s);
            }

            Console.WriteLine("Number Edges: " + graph.Edges.Count());
            int intraEdges = 0;
            int interEdges = 0;
            foreach (var edge in graph.Edges)
            {
                if (edge.Data.Type.Equals(EdgeType.Intra))
                {
                    intraEdges++;
                }
                else
                {
                    interEdges++;
                }
            }
            Console.WriteLine("Number intraEdges: " + intraEdges);
            Console.WriteLine("Number interEdges: " + interEdges);
            Console.WriteLine();
        }



        public void GetCategoryGraphInfo(IGWGraph<CGNodeData, CGEdgeData, CGGraphData> categoryGraph)
        {

            if (categoryGraph == null)
            {
                Console.WriteLine("No CategoryGraph exists");
                Console.WriteLine("\n");
                return;
            }
            Console.WriteLine("CategoryGraph:");
            Console.WriteLine("Number of Nodes: " + categoryGraph.Nodes.Count());

            foreach (var node in categoryGraph.Nodes)
            {
                Console.WriteLine("Number of Nodes in Category {0}: {1}\tEdges in Category: {2}\tProbabilty Parameter: {3}",
                    node.Data.Category, node.Data.NumberNodes, node.Data.NumberEdges, node.Data.ObservationProbabilty);
            }

            Console.WriteLine("Number of Edges: " + categoryGraph.Edges.Count());
            int i = 0;
            foreach (var edge in categoryGraph.Edges)
            {
                Console.WriteLine("Edge {0}: Category {1} <--> Category {2}\tWeight: {3}", i++, edge.Foot.Data.Category, edge.Head.Data.Category, edge.Data.Weight);
            }


            Console.WriteLine("\n\n");

        }

    }
}
