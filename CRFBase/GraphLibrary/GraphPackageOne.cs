//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CodeBase;
//namespace CRFBase.GraphLibrary
//{
//    class GraphPackageOne
//    {
//        private static Random rdm = new Random();
//        public CRFGraph String(int numberNodes)
//        {
//            var graph = new CRFGraph();
//            for (int i = 0; i < numberNodes; i++)
//            {
//                graph.Nodes.Add(new CRFNode());
//            }

//            var nodes = graph.Nodes.ToList();

//            for (int i = 0; i < numberNodes; i++)
//            {
//                graph.Edges.AddRange(new CRFEdge(nodes[i], nodes[(i + 1) % nodes.Count]));
//            }
//            return graph;
//        }
//        public CRFGraph DoubleString(int numberNodes)
//        {
//            var graph = new CRFGraph();
//            for (int i = 0; i < numberNodes; i++)
//            {
//                graph.Nodes.Add(new CRFNode());
//            }

//            var nodes = graph.Nodes.ToList();

//            for (int i = 0; i < numberNodes; i++)
//            {
//                //for (int k = i; k < numberNodes; k++)
//                {
//                    //if (rdm.NextDouble() > 0.9)
//                    graph.Edges.Add(new CRFEdge(nodes[i], nodes[(i + 1) % nodes.Count]));
//                    graph.Edges.Add(new CRFEdge(nodes[i], nodes[(i + 2) % nodes.Count]));
//                }
//            }
//            return graph;
//        }
//        public CRFGraph DoubleStringEight(int numberNodes)
//        {
//            var graph = new CRFGraph();
//            for (int i = 0; i < numberNodes; i++)
//            {
//                graph.Nodes.Add(new CRFNode());
//            }

//            var nodes = graph.Nodes.ToList();

//            for (int i = 0; i < numberNodes; i++)
//            {
//                //for (int k = i; k < numberNodes; k++)
//                {
//                    //if (rdm.NextDouble() > 0.9)
//                    graph.Edges.Add(new CRFEdge(nodes[i], nodes[(i + 1) % nodes.Count]));
//                    graph.Edges.Add(new CRFEdge(nodes[i], nodes[(i + 2) % nodes.Count]));
//                    if (i == 0)
//                    {
//                        graph.Edges.Add(new CRFEdge(nodes[i], nodes[numberNodes / 2]));
//                        graph.Edges.Add(new CRFEdge(nodes[i], nodes[numberNodes / 2 + 1]));
//                        graph.Edges.Add(new CRFEdge(nodes[i + 1], nodes[numberNodes / 2 + 1]));
//                        graph.Edges.Add(new CRFEdge(nodes[i + 1], nodes[numberNodes / 2]));
//                    }
//                }
//            }
//            return graph;
//        }
//        public CRFGraph ThreeCoreElementsGraph(int numberNodes)
//        {
//            var graph = new CRFGraph();
//            for (int i = 0; i < numberNodes; i++)
//            {
//                graph.Nodes.Add(new CRFNode());
//            }

//            var nodes = graph.Nodes.ToList();

//            for (int i = 0; i < numberNodes; i++)
//            {
//                //for (int k = i; k < numberNodes; k++)
//                {
//                    //if (rdm.NextDouble() > 0.9)
//                    graph.Edges.Add(new CRFEdge(nodes[i], nodes[rdm.Next(nodes.Count)]));
//                    if (i < 3)
//                    {
//                        for (int k = 0; k < 6; k++)
//                        {
//                            graph.Edges.Add(new CRFEdge(nodes[i], nodes[rdm.Next(nodes.Count)]));
//                        }
//                    }
//                }
//            }
//            return graph;
//        }
//        public CRFGraph Grid(int dimensionX, int dimensionY)
//        {
//            var graph = new CRFGraph();

//            var grid = new CRFNode[dimensionX, dimensionY];
//            for (int x = 0; x < dimensionX; x++)
//            {
//                for (int y = 0; y < dimensionY; y++)
//                {
//                    //for (int z = 0; z < numberNodes; z++)
//                    {
//                        var node = new CRFNode();
//                        grid[x, y] = node;
//                        graph.Nodes.Add(node);
//                    }
//                }
//            }
//            for (int x = 0; x < dimensionX; x++)
//            {
//                for (int y = 0; y < dimensionY; y++)
//                {
//                    //for (int z = 0; z < numberNodes; z++)
//                    {
//                        if (x < dimensionX - 1)
//                            graph.Edges.Add(new CRFEdge(grid[x, y], grid[x + 1, y]));
//                        if (y < dimensionY - 1)
//                            graph.Edges.Add(new CRFEdge(grid[x, y], grid[x, y + 1]));
//                    }
//                }
//            }
//            return graph;
//        }

//        public CRFGraph Cube(int dimX, int dimY, int dimZ)
//        {
//            var graph = new CRFGraph();

//            var cube = new CRFNode[dimZ][,];
//            for (int z = 0; z < dimZ; z++)
//            {
//                cube[z] = new CRFNode[dimX, dimY];
//            }
//            for (int x = 0; x < dimX; x++)
//            {
//                for (int y = 0; y < dimY; y++)
//                {
//                    for (int z = 0; z < dimZ; z++)
//                    {

//                        var node = (new CRFNode());
//                        cube[z][x, y] = node;
//                        graph.Nodes.Add(node);
//                    }
//                }
//            }
//            for (int x = 0; x < dimX; x++)
//            {
//                for (int y = 0; y < dimY; y++)
//                {
//                    for (int z = 0; z < dimZ; z++)
//                    {
//                        if (x < dimX - 1)
//                            graph.Edges.Add(new CRFEdge(cube[z][x, y], cube[z][x + 1, y]));
//                        if (y < dimY - 1)
//                            graph.Edges.Add(new CRFEdge(cube[z][x, y], cube[z][x, y + 1]));
//                        if (z < dimZ - 1)
//                            graph.Edges.Add(new CRFEdge(cube[z][x, y], cube[z + 1][x, y]));

//                    }
//                }
//            }
//            return graph;
//        }

//        public CRFGraph OneCoreElement(int numberNodes)
//        {
//            var graph = new CRFGraph();
//            for (int i = 0; i < numberNodes; i++)
//            {
//                graph.Nodes.Add(new CRFNode());
//            }

//            var nodes = graph.Nodes.ToList();

//            for (int i = 0; i < numberNodes; i++)
//            {
//                //for (int k = i; k < numberNodes; k++)
//                {
//                    //if (rdm.NextDouble() > 0.9)
//                    graph.Edges.Add(new CRFEdge(nodes[i], nodes[rdm.Next(nodes.Count)]));
//                    if (i < 1)
//                    {
//                        for (int k = 0; k < 10; k++)
//                        {
//                            graph.Edges.Add(new CRFEdge(nodes[i], nodes[rdm.Next(nodes.Count)]));
//                        }
//                    }
//                }
//            }
//            return graph;
//        }

//        public CRFGraph BinaryTree(int depth)
//        {
//            var graph = new CRFGraph();

//            var center = new CRFNode();
//            var currenNodes = new List<CRFNode>();
//            currenNodes.Add(center);
//            graph.Nodes.Add(center);
//            var nextNodes = new List<CRFNode>();
//            for (int i = 0; i < depth; i++)
//            {
//                foreach (var parent in currenNodes)
//                {
//                    var node1 = (new CRFNode());
//                    var node2 = (new CRFNode());

//                    graph.Nodes.AddRange(node1, node2);
//                    nextNodes.AddRange(node1, node2);
//                    //if (rdm.NextDouble() > 0.9)
//                    graph.Edges.Add(new CRFEdge(parent, node1));
//                    graph.Edges.Add(new CRFEdge(parent, node2));

//                }
//                currenNodes = nextNodes;
//                nextNodes = new List<CRFNode>();
//            }
//            return graph;
//        }
//        public CRFGraph RandomTree(int depth, int maxBranches)
//        {
//            var graph = new CRFGraph();

//            var center = new CRFNode();
//            var currenNodes = new List<CRFNode>();
//            currenNodes.Add(center);
//            graph.Nodes.Add(center);
//            var nextNodes = new List<CRFNode>();
//            for (int i = 0; i < depth; i++)
//            {
//                foreach (var parent in currenNodes)
//                {
//                    var children = rdm.Next(5);
//                    for (int child = 0; child < children; child++)
//                    {
//                        var node = (new CRFNode());
//                        nextNodes.Add(node);
//                        graph.Nodes.Add(node);
//                        graph.Edges.Add(new CRFEdge(parent, node));
//                    }
//                }
//                currenNodes = nextNodes;
//                nextNodes = new List<CRFNode>();
//            }
//            return graph;
//        }
//        public CRFGraph Helix(int numberNodes, int connectionSkip)
//        {
//            var graph = new CRFGraph();
//            for (int i = 0; i < numberNodes; i++)
//            {
//                graph.Nodes.Add(new CRFNode());
//            }

//            var nodes = graph.Nodes.ToList();
//            for (int i = 0; i < numberNodes; i++)
//            {
//                graph.Edges.Add(new CRFEdge(nodes[i], nodes[(i + 1) % nodes.Count]));
//                graph.Edges.Add(new CRFEdge(nodes[i], nodes[(i + connectionSkip) % nodes.Count]));
//            }
//            return graph;
//        }
//        public CRFGraph Tunnel(int dimX, int dimY)
//        {
//            var graph = new CRFGraph();
//            var grid = new CRFNode[dimX, dimY];
//            for (int x = 0; x < dimX; x++)
//            {
//                for (int y = 0; y < dimY; y++)
//                {
//                    //for (int z = 0; z < numberNodes; z++)
//                    {
//                        var node = (new CRFNode());
//                        grid[x, y] = node;
//                        graph.Nodes.Add(node);
//                    }
//                }
//            }
//            for (int x = 0; x < dimX; x++)
//            {
//                for (int y = 0; y < dimY; y++)
//                {
//                    //for (int z = 0; z < numberNodes; z++)
//                    {
//                        if (x < dimX)
//                            graph.Edges.Add(new CRFEdge(grid[x, y], grid[(x + 1) % dimX, y]));
//                        if (y < dimY - 1)
//                            graph.Edges.Add(new CRFEdge(grid[x, y], grid[x, y + 1]));
//                    }
//                }
//            }
//            return graph;
//        }
//        public CRFGraph Torus(int dimX, int dimY)
//        {

//            var graph = new CRFGraph();
//            var grid = new CRFNode[dimX, dimY];
//            for (int x = 0; x < dimX; x++)
//            {
//                for (int y = 0; y < dimY; y++)
//                {
//                    //for (int z = 0; z < numberNodes; z++)
//                    {
//                        var node = (new CRFNode());
//                        grid[x, y] = node;
//                        graph.Nodes.Add(node);
//                    }
//                }
//            }
//            for (int x = 0; x < dimX; x++)
//            {
//                for (int y = 0; y < dimY; y++)
//                {
//                    //for (int z = 0; z < numberNodes; z++)
//                    {
//                        if (x < dimX)
//                            graph.Edges.Add(new CRFEdge(grid[x, y], grid[(x + 1) % dimX, y]));
//                        if (y < dimY)
//                            graph.Edges.Add(new CRFEdge(grid[x, y], grid[x, (y + 1) % dimY]));
//                    }
//                }
//            }
//            return graph;
//        }

//    }
//}
