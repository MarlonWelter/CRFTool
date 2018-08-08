using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.GradientDescent
{
    class GradientDescentManager : GWManager<ExecuteGradientDescent>
    {
        protected override void OnRequest(ExecuteGradientDescent request)
        {
            // first read the input parameters:
            var inputParam1 = request.InputParameter1;
            var inputParam2 = request.InputParameter2;

            // here goes the code that executes the gradient descent:


            // store result
            request.Result = "my result";
        }
    }
}
