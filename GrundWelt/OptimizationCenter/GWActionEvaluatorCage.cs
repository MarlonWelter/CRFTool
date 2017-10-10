using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    class GWActionEvaluatorCage<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public GWActionEvaluatorCage(IList<GWActionEvaluator<PositionData, ActionData>> evaluators)
        {
            Evaluators = new ActionEvaluator<PositionData, ActionData>[evaluators.Count()];
            for (int i = 0; i < Evaluators.Length; i++)
            {
                Evaluators[i] = new ActionEvaluator<PositionData, ActionData>(evaluators[i].Evaluate);
                Evaluators[i].Weight = 1.0;
                Evaluators[i].Number = i;
            }
        }

        public void Feedback(IEnumerable<GWAction<PositionData, ActionData>> actions, double food)
        {
            foreach (var item in actions)
            {
                for (int i = 0; i < item.EvaluatorInfo.Evaluators.Count; i++)
                {
                    Evaluators[item.EvaluatorInfo.Evaluators[i]].Weight += item.EvaluatorInfo.Weights[i];
                }
            }
        }

        public void Age()
        {
            for (int i = 0; i < Evaluators.Length; i++)
            {
                Evaluators[i].Weight *= TimeDeclineWeightsFactor;
            }
        }
        public readonly double TimeDeclineWeightsFactor = 0.75;

        public readonly double[] ScoringScheme = new double[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        public List<GWAction<PositionData, ActionData>> Choose(IEnumerable<GWAction<PositionData, ActionData>> options, int number)
        {
            EvaluateActions(options);
            return options.MaxEntries(o => o.Score, number).ToList();
        }
        public void EvaluateActions(IEnumerable<GWAction<PositionData, ActionData>> options)
        {
            int tempNr = 0;
            foreach (var option in options)
            {
                option.TempNumber = tempNr;
                option.EvaluatorInfo = option.EvaluatorInfo ?? new EvaluatorInfo();
                tempNr++;
            }

            //evaluateOptions
            int EachEvaluatorOptions = ScoringScheme.Length;
            var optionsScore = new double[options.Count()];
            foreach (var actionEvaluator in Evaluators)
            {
                var bestOptions = new LinkedList<GWAction<PositionData, ActionData>>();
                foreach (var option in options)
                {
                    option.Score = actionEvaluator.Evaluate(option) * actionEvaluator.Weight;
                    bestOptions.SortedInsert(option, EachEvaluatorOptions, o => o.Score);
                }
                var bestOptionsList = bestOptions.ToList();
                for (int i = 0; i < bestOptionsList.Count; i++)
                {
                    optionsScore[bestOptionsList[i].TempNumber] += ScoringScheme[i] * actionEvaluator.Weight;
                    bestOptionsList[i].EvaluatorInfo.Evaluators.Add(actionEvaluator.Number);
                    bestOptionsList[i].EvaluatorInfo.Weights.Add(ScoringScheme[i]);
                }
            }
            foreach (var option in options)
            {
                option.Score = optionsScore[option.TempNumber];
            }
        }

        public ActionEvaluator<PositionData, ActionData>[] Evaluators;
    }

    class ActionEvaluator<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public ActionEvaluator(Func<GWAction<PositionData, ActionData>, double> evaluationMethod)
        {
            EvaluationMethod = evaluationMethod;
        }
        readonly Func<GWAction<PositionData, ActionData>, double> EvaluationMethod;
        public double Evaluate(GWAction<PositionData, ActionData> action)
        {
            return EvaluationMethod(action);
        }
        public int Number { get; set; }
        public double Weight { get; set; }

        public override string ToString()
        {
            return "" + Math.Round(Weight, 3);
        }
    }

    class EvaluatorInfo
    {
        public readonly List<int> Evaluators = new List<int>();
        public readonly List<double> Weights = new List<double>();
    }
}
