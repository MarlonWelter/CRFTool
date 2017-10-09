

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class Resample<NodeType, EdgeType, ResultType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, IWeighted
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>, IWeighted
    {
        public LinkedList<ResultType> Do(NodeType startNode, Func<NodeType, ResultType> absorbDataFromNode)
        {
            var samples = new LinkedList<ResultType>();

            var currentNode = startNode;

            var rand = new Random();
            while (currentNode.ChildrenEdges().NotNullOrEmpty())
            {
                var nextNode = currentNode.Neighbour(currentNode.ChildrenEdges().
                    Select(edge => new AgO<EdgeType, double>(edge, edge.Weight)).
                    ChooseByProb(rand));
                samples.AddLast(absorbDataFromNode(nextNode));
                currentNode = nextNode;
            }

            return samples;
        }


    }
}
