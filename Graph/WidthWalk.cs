
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    /***
     *  this is only to be used for a directional graph without loops 
     * 
     * 
     */
    public class WidthWalk<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, IMarkable
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
    {
        private int counter = 0;
        public int Counter { get { return counter; } }
        private LinkedList<NodeType> markedElements = new LinkedList<NodeType>();
        public void Do(Action<NodeType> action, NodeType startnode)
        {
            var order = new LinkedList<NodeType>();
            order.AddFirst(startnode);
            startnode.Mark();
            markedElements.Add(startnode);

            while (order.NotNullOrEmpty())
            {
                Do(action, order);
            }
            markedElements.Each(el => el.Unmark());
        }
        public void Do(Action<NodeType> action, LinkedList<NodeType> order)
        {
            var node = order.First.Value;
            order.RemoveFirst();
            action(node);
            counter++;

            var children = node.ChildrenEdges().Select(e => node.Neighbour(e)).Where(n => !n.IsMarked).ToList();
            children.Each(n => n.Mark());
            order.AddRange(children);
            markedElements.AddRange(children);
        }
    }
    public class WidthWalk<NodeType>
        where NodeType : IHas<INodeLogic<NodeType>>, IMarkable
    {
        private int counter = 0;
        public int Counter { get { return counter; } }
        private LinkedList<NodeType> markedElements = new LinkedList<NodeType>();
        public void Do(Action<NodeType> action, NodeType startnode)
        {
            var order = new LinkedList<NodeType>();
            order.AddFirst(startnode);
            startnode.Mark();
            markedElements.Add(startnode);

            while (order.NotNullOrEmpty())
            {
                Do(action, order);
            }
            markedElements.Each(el => el.Unmark());
        }
        public void Do(Action<NodeType> action, LinkedList<NodeType> order)
        {
            var node = order.First.Value;
            order.RemoveFirst();
            action(node);
            counter++;

            var children = node.Neighbours().Where(n => !n.IsMarked);
            children.Each(n => n.Mark());
            order.AddRange(children);
            markedElements.AddRange(children);
        }
    }
}
