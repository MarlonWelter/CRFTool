using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace GrundWelt
{
    class ResultCenter<PositionData, ActionData> : IComparer<GWPosition<PositionData, ActionData>>
        where PositionData : Cloneable<PositionData>
    {
        public ResultCenter(int resultsCount, double[] feedBack, IEnumerable<GWPositionEvaluator<PositionData, ActionData>> evaluators)
        {
            Evaluators = new LinkedList<GWPositionEvaluator<PositionData, ActionData>>[evaluators.Max(e => e.Priority + 1)];
            for (int level = 0; level < Evaluators.Length; level++)
            {
                Evaluators[level] = new LinkedList<GWPositionEvaluator<PositionData, ActionData>>();
                Evaluators[level].AddRange(evaluators.Where(e => e.Priority == level));
            }
            MaxResultsCount = resultsCount;
            Feedback = feedBack;
        }

        public MEvent<ResultCenterFeed<PositionData, ActionData>> NewFeed
        {
            get;
        } = new MEvent<ResultCenterFeed<PositionData, ActionData>>();

        private readonly double[] Feedback;

        private readonly int MaxResultsCount;

        private readonly LinkedList<GWPosition<PositionData, ActionData>> mainResultsList = new LinkedList<GWPosition<PositionData, ActionData>>();

        private readonly LinkedList<GWPositionEvaluator<PositionData, ActionData>>[] Evaluators;

        public void AddPosition(GWPosition<PositionData, ActionData> position)
        {
            var insertionIndex = mainResultsList.SortedInsertPos(position, MaxResultsCount, this);

            if (insertionIndex < Feedback.Length)
                NewFeed.Enter(new ResultCenterFeed<PositionData, ActionData>(position, insertionIndex, Feedback[insertionIndex]));
        }

        public int Compare(GWPosition<PositionData, ActionData> x, GWPosition<PositionData, ActionData> y)
        {
            for (int level = 0; level < Evaluators.Length; level++)
            {
                double scoreX = 0, scoreY = 0;
                foreach (var evaluator in Evaluators[level])
                {
                    scoreX += evaluator.Weight * evaluator.Evaluate(x);
                    scoreY += evaluator.Weight * evaluator.Evaluate(y);
                }
                if (scoreX > scoreY)
                    return 1;
                if (scoreX < scoreY)
                    return -1;
            }
            return 0;
        }
    }

    class ResultCenterFeed<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public ResultCenterFeed(GWPosition<PositionData, ActionData> item, int rank, double food)
        {
            Item = item;
            Food = food;
            Rank = rank;
        }
        public readonly GWPosition<PositionData, ActionData> Item;
        public readonly double Food;
        public readonly int Rank;
    }
}
