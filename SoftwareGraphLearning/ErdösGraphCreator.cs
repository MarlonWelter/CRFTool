
using CodeBase;
using CRFBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareGraphLearning
{

    class ErdösGraphCreator
    {
        private Random random = new Random();

        public GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> CreateGraph(ErdösGraphCreationParameter parameter)
        {
            GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> graph = new GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>();
            graph.Data = new SGLGraphData();
            graph.Data.NumberCategories = parameter.NumberCategories;

            for (int i = 0; i < parameter.NumberNodes; i++)
                graph.CreateNode();


            //put each node in category s.t. no empty categories
            bool hasEmptyCategory = true;
            while (hasEmptyCategory)
            {
                List<int> categoriesUsed = new List<int>();
                for (int i = 0; i < parameter.NumberCategories; i++)
                {
                    categoriesUsed.Add(i);
                }
                foreach (var node in graph.Nodes)
                {
                    node.Data = new SGLNodeData();
                    //put node in random category
                    var category = random.Next(parameter.NumberCategories);
                    node.Data.Category = category;
                    categoriesUsed.Remove(category);
                }

                if (categoriesUsed.Count == 0)
                {
                    hasEmptyCategory = false;
                }

            }


            var nodes = graph.Nodes.ToList();
            //connect nodes
            for (int i = 1; i < nodes.Count; i++)
            {

                GWNode<SGLNodeData, SGLEdgeData, SGLGraphData> currentNode = nodes[i];
                //create edge to previous nodes with defined probabilty 
                for (int j = 0; j < i; j++)
                {
                    if (currentNode.Data.Category == nodes[j].Data.Category)
                    {
                        double probabilty = random.NextDouble();
                        if (probabilty <= parameter.IntraConnectivityDegree)
                        {
                            GWEdge<SGLNodeData, SGLEdgeData, SGLGraphData> edge = graph.CreateEdge(currentNode, nodes[j]);
                            edge.Data = new SGLEdgeData();
                            edge.Data.Type = EdgeType.Intra;
                        }
                    }
                    //nodes are in different category
                    else if (currentNode.Data.Category != nodes[j].Data.Category)
                    {
                        double probabilty = random.NextDouble();
                        if (probabilty <= parameter.InterConnectivityDegree)
                        {
                            GWEdge<SGLNodeData, SGLEdgeData, SGLGraphData> edge = graph.CreateEdge(currentNode, nodes[j]);
                            edge.Data = new SGLEdgeData();
                            edge.Data.Type = EdgeType.Inter;
                        }
                    }
                }


            }


            return graph;
        }

    }

    class ErdösGraphCreationParameter
    {
        public ErdösGraphCreationParameter() { }

        public ErdösGraphCreationParameter(int NumberCategories, int NumberNodes, double IntraConnectivityDegree,
            double InterConnectivityDegree)
        {
            this.NumberCategories = NumberCategories;
            this.NumberNodes = NumberNodes;
            this.IntraConnectivityDegree = IntraConnectivityDegree;
            this.InterConnectivityDegree = InterConnectivityDegree;
        }

        public ErdösGraphCreationParameter(SoftwareGraphLearningParameters parameters) : this(parameters.NumberCategories,
            parameters.NumberNodes, parameters.IntraConnectivityDegree, parameters.InterConnectivityDegree)
        { }

        public int NumberCategories { get; set; }
        public int NumberNodes { get; set; }

        public double IntraConnectivityDegree { get; set; }

        public double InterConnectivityDegree { get; set; }


    }
}
