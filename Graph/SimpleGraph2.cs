using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class SimpleGraph2<NodeType, EdgeType> : IGraph<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, new()
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>, new()
    {
        public NodeType NodeA { get; set; }
        public NodeType NodeB { get; set; }
        public NodeType NodeC { get; set; }
        public NodeType NodeD { get; set; }

        private LinkedList<NodeType> nodes = new LinkedList<NodeType>();

        public EdgeType EdgeAB { get; set; }
        public EdgeType EdgeBC { get; set; }
        public EdgeType EdgeCA { get; set; }
        public EdgeType EdgeBD { get; set; }
        private LinkedList<EdgeType> edges = new LinkedList<EdgeType>();
        public SimpleGraph2()
        {
            NodeA = new NodeType();
            NodeB = new NodeType();
            NodeC = new NodeType();
            NodeD = new NodeType();
            nodes.AddRange(NodeA, NodeB, NodeC, NodeD);

            EdgeAB = new EdgeType();
            EdgeAB.Connect(NodeA, NodeB);
            EdgeBC = new EdgeType();
            EdgeBC.Connect(NodeB, NodeC);
            EdgeCA = new EdgeType();
            EdgeCA.Connect(NodeC, NodeA);
            EdgeBD = new EdgeType();
            EdgeBD.Connect(NodeB, NodeD);
            edges.AddRange(EdgeAB, EdgeBC, EdgeBD, EdgeCA);
        }

        public ICollection<NodeType> Nodes
        {
            get { return nodes; }
        }
        public ICollection<EdgeType> Edges
        {
            get { return edges; }
        }
    }
}