
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class CreateGraphs
    {
        public static void Do<GraphType, NodeType, EdgeType>(Action<NodeType> initNode, Action<EdgeType> initEdge)
            where GraphType : IGraph<NodeType, EdgeType>, new()
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            var graph = new GraphType();
            graph.Nodes.Each(n => initNode(n));            
        }
    }
}
