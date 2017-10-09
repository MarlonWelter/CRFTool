using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class ComputeDiscretization<T>
    {
        public static double[] Do(IEnumerable<AgO<T, double, int>> elements, int intervalCount, int labels, int tries, double min = 0, double max = 1)
        {
            var random = new Random();
            var bestDivisors = new double[intervalCount - 1];
            var bestScore = 0.0;

            var totalRatios = new double[labels];// 
            for (int label = 0; label < labels; label++)
            {
                totalRatios[label] = (double)elements.Sum(e => e.Data3 == label ? 1 : 0) / elements.Count();
            }

            for (int i = 0; i < tries; i++)
            {
                var infoScore = 0.0;
                var divisors = new double[intervalCount - 1];
                var intervals = new LinkedList<AgO<T, double, int>>[intervalCount];
                for (int k = 0; k < intervalCount - 1; k++)
                {
                    divisors[k] = min + max * random.NextDouble();
                }
                divisors = divisors.OrderBy(d => d).ToArray();

                for (int k = 0; k < intervalCount; k++)
                {
                    intervals[k] = new LinkedList<AgO<T, double, int>>();
                }

                foreach (var item in elements)
                {
                    int index = 0;
                    while (index < intervalCount - 1 && divisors[index] <= item.Data2)
                        index++;

                    intervals[index].Add(item);
                }

                for (int k = 0; k < intervalCount; k++)
                {
                    for (int label = 0; label < labels; label++)
                    {
                        var count = intervals[k].Count;
                        if (count > 0)
                        {
                            var labelCounts = intervals[k].Sum(v => v.Data3 == label ? 1 : 0) / (totalRatios[label] * labels);
                            var odds = (double)labelCounts / count;
                            var score = count * Math.Abs(odds - 0.5);
                            infoScore += score;
                        }
                    }
                }

                if (infoScore > bestScore)
                {
                    bestDivisors = divisors;
                    bestScore = infoScore;
                }
            }
            return bestDivisors;
        }
    }
}
