using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public interface IScoreHolder
    {
        double Score { get; set; }
    }

    public class ScoreComparer : IComparer<IScoreHolder>
    {

        public int Compare(IScoreHolder x, IScoreHolder y)
        {
            if (x.Score > y.Score)
                return 1;
            if (x.Score < y.Score)
                return -1;
            else return 0;
        }
    }
}
