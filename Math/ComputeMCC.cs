using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class ComputeMCC
    {
        public static double Do(decimal tp, decimal tn, decimal fp, decimal fn)
        {
            return System.Math.Sqrt(decimal.ToDouble(((tp * tn - fp * fn) * (tp * tn - fp * fn)) / ((tp + fp) * (tp + fn) * (tn + fp) * (tn + fn))));
        }
    }
}
