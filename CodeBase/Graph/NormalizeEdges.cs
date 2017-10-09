using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class NormalizeEdges<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, IWeighted
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>, IWeighted
    {
        public void Do(NodeType startNode)
        {
            var currentNode = startNode;
            var edges = currentNode.ChildrenEdges().ToList();

            var sum = edges.Sum(e => e.Weight * currentNode.Neighbour(e).Weight);

            edges.Each(e => e.Weight = (e.Weight * currentNode.Neighbour(e).Weight) / sum);
        }
    }
}
