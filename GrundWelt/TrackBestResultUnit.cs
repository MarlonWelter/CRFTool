using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class TrackBestResultLogic<T> : IResultTrackingUnit<T>
    {
        public TrackBestResultLogic(IEvaluationMethod<T> evalMethod)
        {
            EvaluationMethod = evalMethod;
            BestScore = double.MinValue;
        }
        public void TrackUnit(Runner<T> unit)
        {
            unit.EndRun.AddPath((t) => AddResult(t.CurrentPosition));
        }

        public void AddResult(T position)
        {
            var eval = EvaluationMethod.Evaluate(position);
            //Log.Post(runner.Name + " achieved: " + Math.Round(eval, 5));
            if (eval > BestScore)
            {
                BestResult = position;
                BestScore = eval;
            }
        }

        public T BestResult { get; set; }
        public double BestScore { get; set; }
        public IEvaluationMethod<T> EvaluationMethod { get; set; }
    }
}
