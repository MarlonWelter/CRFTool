using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class MultiStepComparer<T> : IComparer<T>
    {
        public MultiStepComparer(IComparer<T> primaryComparer, IEnumerable<IComparer<T>> secondaryComparers)
        {
            Comparers.Add(primaryComparer);
            Comparers.AddRange(secondaryComparers);
        }
        public MultiStepComparer(IComparer<T> primaryComparer, params IComparer<T>[] secondaryComparers)
        {
            Comparers.Add(primaryComparer);
            Comparers.AddRange(secondaryComparers);
        }

        private List<IComparer<T>> Comparers = new List<IComparer<T>>();

        public int Compare(T x, T y)
        {
            for (int i = 0; i < Comparers.Count; i++)
            {
                var result = Comparers[i].Compare(x, y);
                if (result != 0)
                    return result;
            }

            return 0;
        }
    }
    public class SimpleComparer<T> : IComparer<T>
    {
        public SimpleComparer(Func<T, double> func)
        {
            Evaluation = func;
        }
        Func<T, double> Evaluation;

        public int Compare(T x, T y)
        {
            if (Evaluation(x) > Evaluation(y))
                return 1;
            else if (Evaluation(x) < Evaluation(y))
                return -1;

            return 0;
        }
    }

}
