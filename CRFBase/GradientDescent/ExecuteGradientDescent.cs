using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.GradientDescent
{
    public class ExecuteGradientDescent : GWRequest<ExecuteGradientDescent>
    {
        public string InputParameter1 { get; set; }
        public string InputParameter2 { get; set; }

        public string Result { get; set; }
    }
}
