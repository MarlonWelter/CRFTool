
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class TransformGraph<NodeType1, EdgeType1, NodeType2, EdgeType2>
        where NodeType1 : IHas<INodeLogic<NodeType1, EdgeType1>>, IMarkable
        where EdgeType1 : IHas<IEdgeLogic<NodeType1, EdgeType1>>
        where NodeType2 : IHas<INodeLogic<NodeType2, EdgeType2>>
        where EdgeType2 : IHas<IEdgeLogic<NodeType2, EdgeType2>>, new()
    {
        public Dictionary<NodeType1, NodeType2> Memory = new Dictionary<NodeType1, NodeType2>();
        public NodeType2 Do(NodeType1 startNode, Func<NodeType1, NodeType2> transform)
        {

            new WidthWalk<NodeType1, EdgeType1>().Do(
               ColX.Fix2<NodeType1, Func<NodeType1, NodeType2>>(transformNode, transform),
               startNode);

            return Memory[startNode];
        }
        private void transformNode(NodeType1 obj, Func<NodeType1, NodeType2> transform)
        {
            var newNode = transform(obj);
            Memory.Add(obj, newNode);
            foreach (var edge in obj.Edges())
            {
                var neighbour = obj.Neighbour(edge);
                if (Memory.ContainsKey(neighbour))
                {
                    var newEdge = new EdgeType2();
                    newEdge.Connect(Memory[edge.Head()], Memory[edge.Foot()]);
                }
            }
        }
    }

    public class TransformGraph<NodeType1, NodeType2>
        where NodeType1 : IHas<INodeLogic<NodeType1>>
        where NodeType2 : IHas<INodeLogic<NodeType2>>
    {
        public Dictionary<NodeType1, NodeType2> Memory = new Dictionary<NodeType1, NodeType2>();
        public Graph<NodeType2> Do(IEnumerable<NodeType1> originalGraph, Func<NodeType1, NodeType2> transform)
        {
            var graph2 = new Graph<NodeType2>();

            foreach (var node in originalGraph)
            {
                transformNode(node, transform);
            }

            return graph2;
        }
        private void transformNode(NodeType1 obj, Func<NodeType1, NodeType2> transform)
        {
            var newNode = transform(obj);
            Memory.Add(obj, newNode);
            foreach (var neighbour in obj.Neighbours())
            {
                if (Memory.ContainsKey(neighbour))
                {
                    newNode.Connect(Memory[neighbour]);
                }
            }
        }
    }


    public class TransformGraph<NodeType2>
        where NodeType2 : IHas<INodeLogic<NodeType2>>
    {
        public Dictionary<IHas<INodeLogic>, NodeType2> Memory = new Dictionary<IHas<INodeLogic>, NodeType2>();
        public Graph<NodeType2> Do(IEnumerable<IHas<INodeLogic>> originalGraph, Func<IHas<INodeLogic>, NodeType2> transform)
        {
            var graph2 = new Graph<NodeType2>();

            foreach (var node in originalGraph)
            {
                transformNode(node, transform);
            }

            graph2.Nodes.AddRange(Memory.Values);

            return graph2;
        }
        private void transformNode(IHas<INodeLogic> obj, Func<IHas<INodeLogic>, NodeType2> transform)
        {
            var newNode = transform(obj);
            Memory.Add(obj, newNode);
            foreach (var neighbour in obj.Logic.Neighbours)
            {
                if (Memory.ContainsKey(neighbour))
                {
                    newNode.Connect(Memory[neighbour]);
                }
            }
        }
    }
}
