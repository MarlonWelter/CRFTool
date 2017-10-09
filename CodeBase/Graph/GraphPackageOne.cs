using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class GraphPackageOne
    {
        private static Random rdm = new Random();
        public static Graph<NodeType> String<NodeType>(int numberNodes) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();
            for (int i = 0; i < numberNodes; i++)
            {
                graph.Nodes.Add(new NodeType());
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                nodes[i].Connect(nodes[(i + 1) % nodes.Count]);
            }
            return graph;
        }
        public static Graph<NodeType> DoubleString<NodeType>(int numberNodes) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();
            for (int i = 0; i < numberNodes; i++)
            {
                graph.Nodes.Add(new NodeType());
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                //for (int k = i; k < numberNodes; k++)
                {
                    //if (rdm.NextDouble() > 0.9)
                    nodes[i].Connect(nodes[(i + 1) % nodes.Count]);
                    nodes[i].Connect(nodes[(i + 2) % nodes.Count]);
                }
            }
            return graph;
        }
        public static Graph<NodeType> DoubleStringEight<NodeType>(int numberNodes) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();
            for (int i = 0; i < numberNodes; i++)
            {
                graph.Nodes.Add(new NodeType());
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                //for (int k = i; k < numberNodes; k++)
                {
                    //if (rdm.NextDouble() > 0.9)
                    nodes[i].Connect(nodes[(i + 1) % nodes.Count]);
                    nodes[i].Connect(nodes[(i + 2) % nodes.Count]);
                    if (i == 0)
                    {
                        nodes[i].Connect(nodes[numberNodes / 2]);
                        nodes[i].Connect(nodes[numberNodes / 2 + 1]);
                        nodes[i + 1].Connect(nodes[numberNodes / 2 + 1]);
                        nodes[i + 1].Connect(nodes[numberNodes / 2]);
                    }
                }
            }
            return graph;
        }
        public static Graph<NodeType> ThreeCoreElementsGraph<NodeType>(int numberNodes) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();
            for (int i = 0; i < numberNodes; i++)
            {
                graph.Nodes.Add(new NodeType());
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                //for (int k = i; k < numberNodes; k++)
                {
                    //if (rdm.NextDouble() > 0.9)
                    nodes[i].Connect(nodes[rdm.Next(nodes.Count)]);
                    if (i < 3)
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            nodes[i].Connect(nodes[rdm.Next(nodes.Count)]);
                        }
                    }
                }
            }
            return graph;
        }
        public static Graph<NodeType> Grid<NodeType>(int dimensionX, int dimensionY) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();

            var grid = new NodeType[dimensionX, dimensionY];
            for (int x = 0; x < dimensionX; x++)
            {
                for (int y = 0; y < dimensionY; y++)
                {
                    //for (int z = 0; z < numberNodes; z++)
                    {
                        var node = new NodeType();
                        grid[x, y] = node;
                        graph.Nodes.Add(node);
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
                            grid[x, y].Connect(grid[x + 1, y]);
                        if (y < dimensionY - 1)
                            grid[x, y].Connect(grid[x, y + 1]);
                    }
                }
            }
            return graph;
        }

        public static Graph<NodeType> Cube<NodeType>(int dimX, int dimY, int dimZ) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();

            var cube = new NodeType[dimZ][,];
            for (int z = 0; z < dimZ; z++)
            {
                cube[z] = new NodeType[dimX, dimY];
            }
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    for (int z = 0; z < dimZ; z++)
                    {

                        var node = (new NodeType());
                        cube[z][x, y] = node;
                        graph.Nodes.Add(node);
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
                            cube[z][x, y].Connect(cube[z][x + 1, y]);
                        if (y < dimY - 1)
                            cube[z][x, y].Connect(cube[z][x, y + 1]);
                        if (z < dimZ - 1)
                            cube[z][x, y].Connect(cube[z + 1][x, y]);

                    }
                }
            }
            return graph;
        }

        public static Graph<NodeType> OneCoreElement<NodeType>(int numberNodes) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();
            for (int i = 0; i < numberNodes; i++)
            {
                graph.Nodes.Add(new NodeType());
            }

            var nodes = graph.Nodes.ToList();

            for (int i = 0; i < numberNodes; i++)
            {
                //for (int k = i; k < numberNodes; k++)
                {
                    //if (rdm.NextDouble() > 0.9)
                    nodes[i].Connect(nodes[rdm.Next(nodes.Count)]);
                    if (i < 1)
                    {
                        for (int k = 0; k < 10; k++)
                        {
                            nodes[i].Connect(nodes[rdm.Next(nodes.Count)]);
                        }
                    }
                }
            }
            return graph;
        }

        public static Graph<NodeType> BinaryTree<NodeType>(int depth) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();

            var center = new NodeType();
            var currenNodes = new List<NodeType>();
            currenNodes.Add(center);
            graph.Nodes.Add(center);
            var nextNodes = new List<NodeType>();
            for (int i = 0; i < depth; i++)
            {
                foreach (var parent in currenNodes)
                {
                    var node1 = (new NodeType());
                    var node2 = (new NodeType());

                    graph.Nodes.AddRange(node1, node2);
                    nextNodes.AddRange(node1, node2);
                    //if (rdm.NextDouble() > 0.9)
                    parent.Connect(node1);
                    parent.Connect(node2);

                }
                currenNodes = nextNodes;
                nextNodes = new List<NodeType>();
            }
            return graph;
        }
        public static Graph<NodeType> RandomTree<NodeType>(int depth, int maxBranches) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();

            var center = new NodeType();
            var currenNodes = new List<NodeType>();
            currenNodes.Add(center);
            graph.Nodes.Add(center);
            var nextNodes = new List<NodeType>();
            for (int i = 0; i < depth; i++)
            {
                foreach (var parent in currenNodes)
                {
                    var children = rdm.Next(5);
                    for (int child = 0; child < children; child++)
                    {
                        var node = (new NodeType());
                        nextNodes.Add(node);
                        graph.Nodes.Add(node);
                        parent.Connect(node);
                    }
                }
                currenNodes = nextNodes;
                nextNodes = new List<NodeType>();
            }
            return graph;
        }
        public static Graph<NodeType> Helix<NodeType>(int numberNodes, int connectionSkip) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();
            for (int i = 0; i < numberNodes; i++)
            {
                graph.Nodes.Add(new NodeType());
            }

            var nodes = graph.Nodes.ToList();
            for (int i = 0; i < numberNodes; i++)
            {
                nodes[i].Connect(nodes[(i + 1) % nodes.Count]);
                nodes[i].Connect(nodes[(i + connectionSkip) % nodes.Count]);
            }
            return graph;
        }
        public static Graph<NodeType> Tunnel<NodeType>(int dimX, int dimY) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {
            var graph = new Graph<NodeType>();
            var grid = new NodeType[dimX, dimY];
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    //for (int z = 0; z < numberNodes; z++)
                    {
                        var node = (new NodeType());
                        grid[x, y] = node;
                        graph.Nodes.Add(node);
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
                            grid[x, y].Connect(grid[(x + 1) % dimX, y]);
                        if (y < dimY - 1)
                            grid[x, y].Connect(grid[x, y + 1]);
                    }
                }
            }
            return graph;
        }
        public static Graph<NodeType> Torus<NodeType>(int dimX, int dimY) where NodeType : IHas<INodeLogic<NodeType>>, new()
        {

            var graph = new Graph<NodeType>();
            var grid = new NodeType[dimX, dimY];
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    //for (int z = 0; z < numberNodes; z++)
                    {
                        var node = (new NodeType());
                        grid[x, y] = node;
                        graph.Nodes.Add(node);
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
                            grid[x, y].Connect(grid[(x + 1) % dimX, y]);
                        if (y < dimY)
                            grid[x, y].Connect(grid[x, (y + 1) % dimY]);
                    }
                }
            }
            return graph;
        }
    }
}
