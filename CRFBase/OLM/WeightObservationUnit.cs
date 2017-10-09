using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class WeightObservationUnit
    {
        public WeightObservationUnit(double positiveMultiplier, double negativeMultiplier)
        {
            PositiveMultiplier = positiveMultiplier;
            NegativeMultiplier = negativeMultiplier;
        }

        public void Init(double[] weightsInit)
        {
            weights = weightsInit.Length;
            SensitivityVector = new double[weights];
            ChangesMemory = new double[weights];
            for (int i = 0; i < weights; i++)
            {
                SensitivityVector[i] = 1.0;
                ChangesMemory[i] = 0.0;
            }
        }

        private double PositiveMultiplier;
        private double NegativeMultiplier;
        private int weights;

        private double[] ChangesMemory;

        private double[] SensitivityVector;

        public double[] ApplyChangeVector(double[] changeVector, double[] weightVector)
        {
            for (int i = 0; i < weights; i++)
            {
                if (Math.Sign(ChangesMemory[i]) == Math.Sign(changeVector[i]))
                {
                    SensitivityVector[i] *= PositiveMultiplier;
                }
                else
                    SensitivityVector[i] *= NegativeMultiplier;

                weightVector[i] += changeVector[i] * SensitivityVector[i];
                ChangesMemory[i] = changeVector[i];
            }
            return weightVector;
        }
    }
}
