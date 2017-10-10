using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class GreedyStrategy<PositionType> : IStrategy<PositionType>
    {
        public GreedyStrategy(IEvaluationMethod<PositionType> evalMethod, Func<PositionType, IEnumerable<PositionType>> nextPossiblePositions)
        {
            EvaluationMethod = evalMethod;
            NextPossiblePositions = nextPossiblePositions;
        }

        public Func<PositionType, IEnumerable<PositionType>> NextPossiblePositions { get; set; }
        public IEvaluationMethod<PositionType> EvaluationMethod { get; set; }

        public bool Apply(PositionType position)
        {
            var nextPositions = NextPossiblePositions(position);
            if (nextPositions.Any())
            {
                nextPositions.OrderByDescending(pos => EvaluationMethod.Evaluate(pos));
                return true;
            }
            else return false;
        }
    }
}
