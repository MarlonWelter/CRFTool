using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class SingleStrategyGreedyUnit<T> : Runner<T>
    {
        public SingleStrategyGreedyUnit(IStrategy<T> strategy)
        {
            Strategy = strategy;
        }
        public IStrategy<T> Strategy { get; set; }


        bool changedPosition = true;
        protected override void Run()
        {
            while (changedPosition)
            {
                changedPosition = Strategy.Apply(CurrentPosition);
            }
        }
    }
}
