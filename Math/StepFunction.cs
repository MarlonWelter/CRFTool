
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodeBase
{
    public class StepFunction 
    {
        public StepFunction()
        {

        }

        public StepFunction(LinkedList<AgO<double, double>> steps)
        {
            XVals = new double[steps.Count];
            YVals = new double[steps.Count];
            var node = steps.First;
            for (int i = 0; i < steps.Count; i++)
            {
                XVals[i] = node.Value.Data1;
                YVals[i] = node.Value.Data2;
                node = node.Next;
            }
            MinValue = 0.0;
            MaxValue = steps.Last.Value.Data1;
        }


        public StepFunction ValueFunction { get; set; }

        public double[] XVals { get; set; }
        public double[] YVals { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }
        public double GetValue(double x)
        {
            if (x < MinValue || x >= MaxValue)
            {
                throw new ArgumentException("Function not defined for x = " + x);
            }

            if (XVals.NotNullOrEmpty())
            {
                for (int i = 0; i < XVals.Length; i++)
                {
                    if (XVals[i] >= x)
                        return YVals[i];
                }
            }

            throw new ArgumentException("Function not defined for x = " + x);
        }
    }
}
