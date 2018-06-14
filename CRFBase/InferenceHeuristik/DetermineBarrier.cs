
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class DetermineBarrier<T>
    {
        public DetermineBarrier(int nodeStates)
        {
            NodeStates = nodeStates;
        }
        public int NodeStates { get; set; }
        public double Do(IList<T> Sample, Func<T, double> scoringFunction, double desiredSurvivorRate)
        {
            double barrier = 0.0;
            int sampleCount = Sample.Count;

            int survivors = (int)(desiredSurvivorRate * sampleCount);
            LinkedList<double> Scores = new LinkedList<double>();

            Random rand = new Random();

            foreach (var item in Sample)
            {
                LinkedListHandler.SortedInsert(Scores, scoringFunction(item), survivors);
            }
            barrier = Scores.Last.Value;

            return barrier;
        }
    }
}
