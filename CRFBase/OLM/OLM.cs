using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.OLM
{
    public static class OLM
    {
        public static double LossRatio(int[] a, int[] b)
        {
            var loss = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                loss += a[i] != b[i] ? 1 : 0;
            }
            return loss;// / a.Length;
        }
    }
}
