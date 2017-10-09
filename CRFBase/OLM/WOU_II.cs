using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class WeightObservationUnit_II
    {
        public WeightObservationUnit_II(int length, double startSens)
        {
            Sensitivity = new double[length];
            CurrentChanges = new double[length];
            ProbUp = new double[length];
            for (int i = 0; i < length; i++)
            {
                Sensitivity[i] = startSens;
                CurrentChanges[i] = 0.0;
                ProbUp[i] = 1.0;
            }
        }

        private double[] Sensitivity;
        private double[] CurrentChanges;
        private double[] ProbUp;
        private Random rdm = new Random();
        public void Update(double[] CurrentVector)
        {
            for (int i = 0; i < CurrentVector.Length; i++)
            {
                if (rdm.NextDouble() <= ProbUp[i] / (ProbUp[i] + 1.0))
                {
                    CurrentVector[i] += Sensitivity[i];

                    CurrentChanges[i] = 1;
                }
                else
                {
                    CurrentVector[i] -= Sensitivity[i];
                    CurrentChanges[i] = -1;
                }
            }
        }
        public void Feedback(double change)
        {
            for (int i = 0; i < Sensitivity.Length; i++)
            {
                Sensitivity[i] *= 0.99;
                if (change > 0)
                {
                    if (CurrentChanges[i] > 0)
                        ProbUp[i] *= 1.1;
                    else if (CurrentChanges[i] < 0)
                        ProbUp[i] /= 1.1;
                }
            }
        }
    }
}
