using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class CRFResult
    {
        public CRFResult()
        {
        }
        public int[] Labeling { get; set; }
        public double Score { get; set; }
        public TimeSpan RunTime { get; set; }
    }
}
