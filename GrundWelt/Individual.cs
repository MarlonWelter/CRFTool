using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public interface IndividualLogic<IndividualType> : ILogic
    {
        double[] Code { get; set; }

        IndividualType Mate(IndividualType mate);

        IndividualType Owner { get; }
    }

    public static class IndividualX
    {
        public static Random Random = new Random();
        public static double[] Average<IndividualType>(this IHas<IndividualLogic<IndividualType>> individualOne, IHas<IndividualLogic<IndividualType>> IndividualTwo)
        {
            var resultingCode = individualOne.Logic.Code.ToArray();
            if (IndividualTwo.Logic.Code.Length != resultingCode.Length)
                throw new ArgumentException("Cannot mate individuals with different Code-Lengths.");

            for (int i = 0; i < resultingCode.Length; i++)
            {
                resultingCode[i] = 0.5 * resultingCode[i] + 0.5 * IndividualTwo.Logic.Code[i];
            }

            return resultingCode;
        }
        public static void AddRdm<IndividualType>(this IHas<IndividualLogic<IndividualType>> individualOne, double weight)
        {
            var resultingCode = individualOne.Logic.Code;

            for (int i = 0; i < resultingCode.Length; i++)
            {
                resultingCode[i] += weight * (Random.NextDouble() - 0.5);
            }
        }
    }
}
