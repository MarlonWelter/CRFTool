using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public static class GWGraphpackageOne
    {
        private static Random rdm = new Random();

        public static List<GWGraph<NodeData, EdgeData, GraphData>> DefaultPackage<NodeData, EdgeData, GraphData>(int graphsizes)
        {
            var list = new List<GWGraph<NodeData, EdgeData, GraphData>>();

            list.AddRange(GWString<NodeData, EdgeData, GraphData>(graphsizes),
                DoubleString<NodeData, EdgeData, GraphData>(graphsizes),
                DoubleStringEight<NodeData, EdgeData, GraphData>(graphsizes),
                ThreeCoreElements<NodeData, EdgeData, GraphData>(graphsizes),
                Grid<NodeData, EdgeData, GraphData>((int)Math.Sqrt(graphsizes), (int)Math.Sqrt(graphsizes)),
                Cube<NodeData, EdgeData, GraphData>((int)Math.Pow(graphsizes, 0.33), (int)Math.Pow(graphsizes, 0.33), (int)Math.Pow(graphsizes, 0.33)),
                OneCoreElement<NodeData, EdgeData, GraphData>(graphsizes),
                BinaryTree<NodeData, EdgeData, GraphData>((int)Math.Log(graphsizes, 2)),
                RandomTree<NodeData, EdgeData, GraphData>((int)Math.Log(graphsizes, 2), 4),
                Helix<NodeData, EdgeData, GraphData>(graphsizes, 3),
                Helix<NodeData, EdgeData, GraphData>(graphsizes, 4),
                Helix<NodeData, EdgeData, GraphData>(graphsizes, 5),
                Tunnel<NodeData, EdgeData, GraphData>((int)Math.Sqrt(graphsizes), (int)Math.Sqrt(graphsizes)),
                Torus<NodeData, EdgeData, GraphData>((int)Math.Sqrt(graphsizes), (int)Math.Sqrt(graphsizes))
                );

            return list;
        }

        public static GWGraph<NodeData, EdgeData, GraphData> GWString<NodeData, EdgeData, GraphData>(int numberNodes)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("String (" + numberNodes + ")");

            for (int i = 0; i < numberNodes; i++)
            {
                graph.CreateNode();
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                graph.CreateEdge(nodes[i], nodes[(i + 1) % nodes.Count]);
            }
            return graph;
        }
        public static GWGraph<NodeData, EdgeData, GraphData> DoubleString<NodeData, EdgeData, GraphData>(int numberNodes)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("DoubleString (" + numberNodes + ")");
            for (int i = 0; i < numberNodes; i++)
            {
                graph.CreateNode();
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                //for (int k = i; k < numberNodes; k++)
                {
                    //if (rdm.NextDouble() > 0.9)
                    graph.CreateEdge(nodes[i], nodes[(i + 1) % nodes.Count]);
                    graph.CreateEdge(nodes[i], nodes[(i + 2) % nodes.Count]);
                }
            }
            return graph;
        }
        public static GWGraph<NodeData, EdgeData, GraphData> DoubleStringEight<NodeData, EdgeData, GraphData>(int numberNodes)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("DoubleStringEight (" + numberNodes + ")");
            for (int i = 0; i < numberNodes; i++)
            {
                graph.CreateNode();
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                //for (int k = i; k < numberNodes; k++)
                {
                    //if (rdm.NextDouble() > 0.9)
                    graph.CreateEdge(nodes[i], nodes[(i + 1) % nodes.Count]);
                    graph.CreateEdge(nodes[i], nodes[(i + 2) % nodes.Count]);
                    if (i == 0)
                    {
                        graph.CreateEdge(nodes[i], nodes[numberNodes / 2]);
                        graph.CreateEdge(nodes[i], nodes[numberNodes / 2 + 1]);
                        graph.CreateEdge(nodes[i + 1], nodes[numberNodes / 2]);
                        graph.CreateEdge(nodes[i + 1], nodes[numberNodes / 2 + 1]);
                    }
                }
            }
            return graph;
        }
        public static GWGraph<NodeData, EdgeData, GraphData> ThreeCoreElements<NodeData, EdgeData, GraphData>(int numberNodes)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("ThreeCoreElement (" + numberNodes + ")");
            for (int i = 0; i < numberNodes; i++)
            {
                graph.CreateNode();
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                //for (int k = i; k < numberNodes; k++)
                {
                    //if (rdm.NextDouble() > 0.9)
                    graph.CreateEdge(nodes[i], nodes[rdm.Next(nodes.Count)]);
                    if (i < 3)
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            graph.CreateEdge(nodes[i], nodes[rdm.Next(nodes.Count)]);
                        }
                    }
                }
            }
            return graph;
        }
        public static GWGraph<NodeData, EdgeData, GraphData> Grid<NodeData, EdgeData, GraphData>(int dimensionX, int dimensionY)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("Grid (" + dimensionX + "," + dimensionY + ")");

            var grid = new GWNode<NodeData, EdgeData, GraphData>[dimensionX, dimensionY];
            for (int x = 0; x < dimensionX; x++)
            {
                for (int y = 0; y < dimensionY; y++)
                {
                    //for (int z = 0; z < numberNodes; z++)
                    {
                        var node = graph.CreateNode();
                        grid[x, y] = node;
                    }
                }
            }
            for (int x = 0; x < dimensionX; x++)
            {
                for (int y = 0; y < dimensionY; y++)
                {
                    //for (int z = 0; z < numberNodes; z++)
                    {
                        if (x < dimensionX - 1)
                            graph.CreateEdge(grid[x, y], grid[x + 1, y]);
                        if (y < dimensionY - 1)
                            graph.CreateEdge(grid[x, y], grid[x, y + 1]);
                    }
                }
            }
            return graph;
        }

        public static GWGraph<NodeData, EdgeData, GraphData> Cube<NodeData, EdgeData, GraphData>(int dimX, int dimY, int dimZ)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("Cube (" + dimX + "x" + dimY + "x" + dimZ + ")");

            var cube = new GWNode<NodeData, EdgeData, GraphData>[dimZ][,];
            for (int z = 0; z < dimZ; z++)
            {
                cube[z] = new GWNode<NodeData, EdgeData, GraphData>[dimX, dimY];
            }
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    for (int z = 0; z < dimZ; z++)
                    {
                        var node = graph.CreateNode();
                        cube[z][x, y] = node;
                    }
                }
            }
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    for (int z = 0; z < dimZ; z++)
                    {
                        if (x < dimX - 1)
                            graph.CreateEdge(cube[z][x, y], cube[z][x + 1, y]);
                        if (y < dimY - 1)
                            graph.CreateEdge(cube[z][x, y], cube[z][x, y + 1]);
                        if (z < dimZ - 1)
                            graph.CreateEdge(cube[z][x, y], cube[z + 1][x, y]);

                    }
                }
            }
            return graph;
        }

        public static GWGraph<NodeData, EdgeData, GraphData> OneCoreElement<NodeData, EdgeData, GraphData>(int numberNodes)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("OneCoreElement (" + numberNodes + ")");
            for (int i = 0; i < numberNodes; i++)
            {
                var node = graph.CreateNode();
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                //for (int k = i; k < numberNodes; k++)
                {
                    //if (rdm.NextDouble() > 0.9)
                    graph.CreateEdge(nodes[i], nodes[rdm.Next(nodes.Count)]);
                    if (i < 1)
                    {
                        for (int k = 0; k < 10; k++)
                        {
                            graph.CreateEdge(nodes[i], nodes[rdm.Next(nodes.Count)]);
                        }
                    }
                }
            }
            return graph;
        }

        public static GWGraph<NodeData, EdgeData, GraphData> BinaryTree<NodeData, EdgeData, GraphData>(int depth)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("BinaryTree (" + depth + ")");

            var center = graph.CreateNode();
            var currenNodes = new List<GWNode<NodeData, EdgeData, GraphData>>();
            currenNodes.Add(center);
            var nextNodes = new List<GWNode<NodeData, EdgeData, GraphData>>();
            for (int i = 0; i < depth; i++)
            {
                foreach (var parent in currenNodes)
                {
                    var node1 = (graph.CreateNode());
                    var node2 = (graph.CreateNode());

                    nextNodes.AddRange(node1, node2);
                    //if (rdm.NextDouble() > 0.9)
                    graph.CreateEdge(parent, node1);
                    graph.CreateEdge(parent, node2);

                }
                currenNodes = nextNodes;
                nextNodes = new List<GWNode<NodeData, EdgeData, GraphData>>();
            }
            return graph;
        }
        public static GWGraph<NodeData, EdgeData, GraphData> RandomTree<NodeData, EdgeData, GraphData>(int depth, int maxBranches)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("RandomTree (" + depth + "," + maxBranches + ")");

            var center = graph.CreateNode();
            var currenNodes = new List<GWNode<NodeData, EdgeData, GraphData>>();
            currenNodes.Add(center);
            var nextNodes = new List<GWNode<NodeData, EdgeData, GraphData>>();
            for (int i = 0; i < depth; i++)
            {
                foreach (var parent in currenNodes)
                {
                    var children = rdm.Next(5);
                    for (int child = 0; child < children; child++)
                    {
                        var node = (graph.CreateNode());
                        nextNodes.Add(node);
                        graph.CreateEdge(parent, node);
                    }
                }
                currenNodes = nextNodes;
                nextNodes = new List<GWNode<NodeData, EdgeData, GraphData>>();
            }
            return graph;
        }
        public static GWGraph<NodeData, EdgeData, GraphData> Helix<NodeData, EdgeData, GraphData>(int numberNodes, int connectionSkip)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("Helix (" + connectionSkip + "," + numberNodes + ")");
            for (int i = 0; i < numberNodes; i++)
            {
                graph.CreateNode();
            }

            var nodes = graph.Nodes.ToList();
            for (int i = 0; i < numberNodes; i++)
            {
                graph.CreateEdge(nodes[i], nodes[(i + 1) % nodes.Count]);
                graph.CreateEdge(nodes[i], nodes[(i + connectionSkip) % nodes.Count]);
            }
            return graph;
        }
        public static GWGraph<NodeData, EdgeData, GraphData> Tunnel<NodeData, EdgeData, GraphData>(int dimX, int dimY)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("Tunnel (" + dimX + "," + dimY + ")");
            var grid = new GWNode<NodeData, EdgeData, GraphData>[dimX, dimY];
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    //for (int z = 0; z < numberNodes; z++)
                    {
                        var node = graph.CreateNode();
                        grid[x, y] = node;
                    }
                }
            }
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    //for (int z = 0; z < numberNodes; z++)
                    {
                        if (x < dimX)
                            graph.CreateEdge(grid[x, y], grid[(x + 1) % dimX, y]);
                        if (y < dimY - 1)
                            graph.CreateEdge(grid[x, y], grid[x, y + 1]);
                    }
                }
            }
            return graph;
        }
        public static GWGraph<NodeData, EdgeData, GraphData> Torus<NodeData, EdgeData, GraphData>(int dimX, int dimY)
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>("Torus (" + dimX + "," + dimY + ")");
            var grid = new GWNode<NodeData, EdgeData, GraphData>[dimX, dimY];
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    //for (int z = 0; z < numberNodes; z++)
                    {
                        var node = graph.CreateNode();
                        grid[x, y] = node;
                    }
                }
            }
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    //for (int z = 0; z < numberNodes; z++)
                    {
                        if (x < dimX)
                            graph.CreateEdge(grid[x, y], grid[(x + 1) % dimX, y]);
                        if (y < dimY)
                            graph.CreateEdge(grid[x, y], grid[x, (y + 1) % dimY]);
                    }
                }
            }
            return graph;
        }
    }
}
