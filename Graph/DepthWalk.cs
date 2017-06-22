
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class DepthWalk<NodeType> : DepthWalk<NodeType, SimpleEdge<NodeType>>
        where NodeType : IHas<INodeLogic<NodeType, SimpleEdge<NodeType>>>, IMarkable
    {
    }
    public class DepthWalk<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, IMarkable
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
    {
        private LinkedList<NodeType> memory = new LinkedList<NodeType>();
        public void Do(Action<NodeType> action, NodeType node, bool unmarkOnFinish = true)
        {
            node.Mark();
            memory.Add(node);
            action(node);

            var edges = node.Edges().Where(e => e.Foot().Equals(node)).ToList();

            if (edges.NotNullOrEmpty())
            {
                foreach (var edge in edges)
                {
                    var nb = node.Neighbour(edge);
                    if (!nb.IsMarked)
                    {
                        Do(action, nb, false);
                    }
                }
            }
            if (unmarkOnFinish)
                memory.Each(n => n.Unmark());
        }

        public void Do(Action<NodeType> action, NodeType node, int maxDepth, bool unmarkOnFinish = true)
        {
            node.Mark();
            memory.Add(node);
            action(node);

            if (maxDepth > 0)
            {
                var edges = node.Edges().Where(e => e.Foot().Equals(node)).ToList();

                if (edges.NotNullOrEmpty())
                {
                    foreach (var edge in edges)
                    {
                        var nb = node.Neighbour(edge);
                        if (!nb.IsMarked)
                        {
                            Do(action, nb, maxDepth - 1, false);
                        }
                    }
                }
            }
            if (unmarkOnFinish)
                memory.Each(n => n.Unmark());
        }
    }
    public class DepthWalk
    {
        private LinkedList<IHas<INodeLogic>> memory = new LinkedList<IHas<INodeLogic>>();
        private Dictionary<IHas<INodeLogic>, bool> marks = new Dictionary<IHas<INodeLogic>, bool>();
        public void Do(Action<IHas<INodeLogic>, int> action, IHas<INodeLogic> node, int depth = 0)
        {

            memory.Add(node);
            action(node, depth);


            foreach (var nb in node.Logic.Neighbours)
            {
                if (!memory.Contains(nb))
                {
                    Do(action, nb, depth + 1);
                }
            }
        }
    }
    public class DepthWalk2<NodeType> where NodeType : IHas<INodeLogic<NodeType>>
    {
        private LinkedList<NodeType> memory = new LinkedList<NodeType>();
        private Dictionary<NodeType, bool> marks = new Dictionary<NodeType, bool>();
        public void Do(Action<NodeType, int> action, NodeType node, int depth = 0)
        {

            memory.Add(node);
            action(node, depth);


            foreach (var nb in node.Neighbours())
            {
                if (!memory.Contains(nb))
                {
                    Do(action, nb, depth + 1);
                }
            }
        }
    }
}
