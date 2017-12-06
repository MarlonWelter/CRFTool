using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CodeBase
{
    public class CategoryGraphCreator
    {
        private Random random = new Random();
        public void CreateCategoryGraph(GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> erdösGraph)
        {
            GWGraph<CGNodeData, CGEdgeData, CGGraphData> categoryGraph = new GWGraph<CGNodeData, CGEdgeData, CGGraphData>();
            categoryGraph.Data = new CGGraphData();
            categoryGraph.Data.ErdösGraph = erdösGraph;

            //create nodes
            for (int i = 0; i < erdösGraph.Data.NumberCategories; i++)
            {
                var node = categoryGraph.CreateNode();
                node.Data = new CGNodeData();
                node.Data.Category = i;
                node.Data.ObservationProbabilty = random.NextDouble();
                node.Data.Nodes = new List<GWNode<SGLNodeData, SGLEdgeData, SGLGraphData>>();
                node.Data.InterEdges = new List<IGWEdge<SGLNodeData, SGLEdgeData, SGLGraphData>>();
            }

            //count nodes in each category
            var categoryNodes = categoryGraph.Nodes.ToList();

            foreach (var node in erdösGraph.Nodes)
            {
                int category = node.Data.Category;
                categoryNodes[category].Data.NumberNodes++;
                categoryNodes[category].Data.Nodes.Add(node);
            }

            //create edges
            foreach (var edge in erdösGraph.Edges)
            {
                int footCategory = edge.Foot.Data.Category;
                int headCategory = edge.Head.Data.Category;
                //edge between categories -> find out if an edge already exists between the 2 categories, if not create it. increment weight in both cases
                if (edge.Data.Type == EdgeType.Inter)
                {

                    //add edges to incoming edges of nodes
                    var head = categoryNodes.Find(n => (n.Data.Category == headCategory));
                    head.Data.InterEdges.Add(edge);

                    var foot = categoryNodes.Find(n => (n.Data.Category == footCategory));
                    foot.Data.InterEdges.Add(edge); 


                    var categoryEdges = categoryGraph.Edges.ToList();
                    var categoryEdge = categoryEdges.Find(e => (e.Foot.Data.Category == footCategory) && (e.Head.Data.Category == headCategory));
                    if (categoryEdge == null)
                    {
                        categoryEdge = categoryEdges.Find(e => (e.Foot.Data.Category == headCategory) && (e.Head.Data.Category == footCategory));
                    }

                    //edge is new
                    if (categoryEdge == null)
                    {
                        var newEdge = categoryGraph.CreateEdge(
                            categoryNodes.Find(n => n.Data.Category == footCategory), categoryNodes.Find(n => n.Data.Category == headCategory));
                        newEdge.Data = new CGEdgeData();
                        newEdge.Data.Weight++;
                    }
                    //edge already exists
                    else
                    {
                        categoryEdge.Data.Weight++;
                    }
                }
                //edge inside a category -> increment NumberEdges
                else if (edge.Data.Type == EdgeType.Intra)
                {
                    if (footCategory != headCategory)
                    {
                        throw new ArgumentException("Conflicting Graph Data: Node " + edge.Foot.GraphId + " and Node " + edge.Head.GraphId + " have to be in the same category");
                    }
                    categoryNodes.Find(n => n.Data.Category == footCategory).Data.NumberEdges++;
                }

            }
            erdösGraph.Data.CategoryGraph = categoryGraph;
        }


    }
}
