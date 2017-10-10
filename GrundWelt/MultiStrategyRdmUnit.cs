using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace GrundWelt
{
    public class MultiStrategyRdmUnit<T> : Runner<T>
    {
        public MultiStrategyRdmUnit(IEnumerable<IStrategy<T>> strategies)
        {
            Strategies = strategies.ToList();
        }

        public MultiStrategyRdmUnit(params IStrategy<T>[] strategies)
        {
            Strategies = strategies.ToList();
        }
        public IList<IStrategy<T>> Strategies { get; set; }

        private Random Random;

        bool changedPosition = true;

        protected override void Run()
        {
            Random = new Random(); 
            while (changedPosition)
            {
                changedPosition = Strategies.RandomElement(Random).Apply(CurrentPosition);
            }
        }
    }
}
