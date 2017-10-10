using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class MultiEvaluationRdmWeight<ActionType> : MultiEvaluationWeighted<ActionType>
    {
        public MultiEvaluationRdmWeight(IList<IEvaluationMethod<ActionType>> evalMethods)
            : base(evalMethods)
        {
            Name = "MultiRDMEvaluation";
            for (int i = 0; i < Weights.Count; i++)
            {
                Weights[i] = Program.Random.NextDouble();
            }
        }
    }
    public class MultiEvaluationWeighted<ActionType> : IEvaluationMethod<ActionType>
    {
        public MultiEvaluationWeighted(IList<IEvaluationMethod<ActionType>> evalMethods)
        {
            EvalMethods = evalMethods.ToArray();
            Weights = new double[evalMethods.Count];
        }
        public MultiEvaluationWeighted(IList<IEvaluationMethod<ActionType>> evalMethods, IList<double> weights)
        {
            EvalMethods = evalMethods.ToArray();
            Weights = weights.ToArray();
        }
        public IList<double> Weights { get; set; }
        public string Name = "MultiEvaluationWeighted";
        public IEvaluationMethod<ActionType>[] EvalMethods { get; set; }

        public double Evaluate(ActionType action)
        {
            var value = 0.0;
            for (int i = 0; i < EvalMethods.Length; i++)
            {
                if (Weights[i] > 0)
                    value += EvalMethods[i].Evaluate(action) * Weights[i];
            }
            return value;
        }
    }
}
