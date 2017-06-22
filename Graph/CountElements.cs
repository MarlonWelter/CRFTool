
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class CountElements<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>, IMarkable
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
    {
        public static int Do(NodeType startnode)
        {
            var logic = new WidthWalk<NodeType, EdgeType>();
            logic.Do(n => { }, startnode);
            return logic.Counter;
        }
    }

    public class Counter<T>
    {
        public int Count { get; set; }
        public void Do(T element)
        {
            Count++;
        }
    }
    public class Sum<T>
    {
        public Sum(Func<T, double> countLogic)
        {
            CountLogic = countLogic;
        }
        public double Count { get; set; }
        private Func<T, double> CountLogic;
        public void Do(T element)
        {
            Count += CountLogic(element);
        }
    }
}
