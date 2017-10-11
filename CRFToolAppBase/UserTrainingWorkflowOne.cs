using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase
{
    class UserTrainingWorkflowOne
    {
        public int NumberIntervals { get; set; }
        public string[] Characteristics { get; set; }
        public string EdgeCharacteristic { get; set; }
        public double[] Weights { get; set; }
    }
}
