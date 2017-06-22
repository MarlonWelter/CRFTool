
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class CategoryGraph : GWGraph<CGND, CGED, CGGD>
    {
    }
    public class CGND : ICoordinated
    {
        public string Category { get; set; }
        public int Modules { get; set; }
        public double Quality { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
    public class CGED
    {
        public int Connections { get; set; }
    }
    public class CGGD
    {

    }

    public static class CreateCategoryGraph
    {
        public static void Do(IGWGraph<SSPND, SSPED, SSPGD> graph)
        {
            var catGraph = new CategoryGraph();

            var tempdict = new Dictionary<string, CGND>();

            foreach (var category in graph.Data.Categories)
            {
                var cnode = catGraph.CreateNode();
                cnode.Data = new CGND();
                tempdict.Add(category, cnode.Data);
                cnode.Data.Category = category;

                var goodModules = 0.0;
                foreach (var gnode in graph.Nodes)
                {
                    if (!gnode.Data.Category.Equals(category))
                        continue;
                    cnode.Data.Modules++;
                    if (gnode.Data.Classifiation == 0)
                        goodModules++;
                }

                cnode.Data.Quality = (goodModules / cnode.Data.Modules) * (goodModules / cnode.Data.Modules);
            }

            var nodesList = catGraph.Nodes.ToList();
            for (int i = 0; i < nodesList.Count - 1; i++)
            {
                for (int k = i + 1; k < nodesList.Count; k++)
                {
                    var edge = catGraph.CreateEdge(nodesList[i], nodesList[k]);
                    edge.Data = new CGED();

                    foreach (var gedge in graph.Edges)
                    {
                        if (edge.Head.Data.Category.Equals(gedge.Head.Data.Category) && edge.Foot.Data.Category.Equals(gedge.Foot.Data.Category))
                            edge.Data.Connections++;
                        else if (edge.Head.Data.Category.Equals(gedge.Foot.Data.Category) && edge.Foot.Data.Category.Equals(gedge.Head.Data.Category))
                            edge.Data.Connections++;
                    }
                }
            }


            graph.Data.CategoryGraph = catGraph;
        }
    }
}
