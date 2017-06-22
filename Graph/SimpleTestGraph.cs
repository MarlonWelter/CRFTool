using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class SimpleGraphRectangle<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, new()
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>, new()
    {
        public NodeType NodeA { get; set; }
        public NodeType NodeB { get; set; }
        public NodeType NodeC { get; set; }
        public NodeType NodeD { get; set; }

        public EdgeType EdgeAB { get; set; }
        public EdgeType EdgeBC { get; set; }
        public EdgeType EdgeCD { get; set; }
        public EdgeType EdgeDA { get; set; }

        public SimpleGraphRectangle()
        {
            NodeA = new NodeType();
            NodeB = new NodeType();
            NodeC = new NodeType();
            NodeD = new NodeType();

            EdgeAB = new EdgeType();
            EdgeAB.Connect(NodeA, NodeB);
            EdgeBC = new EdgeType();
            EdgeBC.Connect(NodeB, NodeC);
            EdgeCD = new EdgeType();
            EdgeCD.Connect(NodeC, NodeD);
            EdgeDA = new EdgeType();
            EdgeDA.Connect(NodeD, NodeA);
        }
    }

    public class CreatePreNode<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, new()
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>, new()
    {
        public static NodeType Do(NodeType node)
        {
            var head = new NodeType();
            var edge = new EdgeType();
            edge.Connect(head, node);
            return head;
        }
    }
    public class CreatePostNode<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, new()
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>, new()
    {
        public static NodeType Do(NodeType node)
        {
            var foot = new NodeType();
            var edge = new EdgeType();
            edge.Connect(node, foot);
            return foot;
        }
    }
}
