using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace GrundWelt
{
    public abstract class GWUnit<PositionType, ActionType> : SimpleWorkStation<PositionType, PositionType>
    {
        public GWUnit(IGWMind<PositionType> mind)
            : base(1, 1)
        {
            Mind = mind;
        }
        public IGWMind<PositionType> Mind { get; set; }

        protected override void ExecuteWork(PositionType startPosition)
        {
            Mind.SetStartPosition(startPosition);

            while (true)
            {
                if (!Mind.DoAction())
                    break;
            }
            AddOutput(Mind.BestPosition());
        }
    }

    public interface IGWMind<PositionType>
    {
        bool DoAction();
        void SetStartPosition(PositionType position);
        PositionType BestPosition();
    }

    public abstract class GWBody<PositionType, ActionType>
    {
        public PositionType CurrentPosition { get; set; }
        public abstract void ExecuteAction(ActionType action);
    }

    public abstract class GWBigBody<PositionType, ActionType>
    {
        public GWBigBody(int size)
        {
            CurrentPositions = new PositionType[size];
        }
        public PositionType[] CurrentPositions { get; set; }
        public abstract void ExecuteActions(IEnumerable<ActionType> actions);
    }

    public abstract class GWBigMind<PositionType, ActionType> : IGWMind<PositionType>
    {
        public GWBigMind(IEvaluationMethod<ActionType> actionEval, int size, GWBigBody<PositionType, ActionType> body)
        {
            ActionEvaluation = actionEval;
            Size = size;
            Body = body;
        }

        public int Size { get; set; }

        public bool DoAction()
        {
            LinkedList<ActionType> actions = new LinkedList<ActionType>();
            foreach (var position in Body.CurrentPositions)
            {
                if (position == null)
                    continue;
                actions.AddRange(FindPossibleActions(position));
            }

            var actionsToExecute = actions.Where(a => ActionEvaluation.Evaluate(a) >= 0);
            actionsToExecute = actionsToExecute.OrderByDescending(a => ActionEvaluation.Evaluate(a)).Take(Math.Min(actionsToExecute.Count(), Size));

            if (!actions.Any())
                return false;

            Body.ExecuteActions(actionsToExecute);
            return true;
        }

        public GWBigBody<PositionType, ActionType> Body { get; set; }

        protected abstract IEnumerable<ActionType> FindPossibleActions(PositionType position);

        public readonly IEvaluationMethod<ActionType> ActionEvaluation;

        public void SetStartPosition(PositionType position)
        {
            Body.CurrentPositions[0] = position;
        }

        public PositionType BestPosition()
        {
            return Body.CurrentPositions[0];
        }
    }

    public abstract class GWMind<PositionType, ActionType> : IGWMind<PositionType>
    {
        public GWMind(IEvaluationMethod<ActionType> actionEval)
        {
            ActionEvaluation = actionEval;
        }
        public bool DoAction()
        {
            var actions = FindPossibleActions(Body.CurrentPosition);
            if (!actions.Any())
                return false;
            var actionToExecute = Utilities.MaxEntry(actions, a => ActionEvaluation.Evaluate(a));
            if (ActionEvaluation.Evaluate(actionToExecute) < 0)
                return false;
            Body.ExecuteAction(actionToExecute);
            return true;
        }

        public GWBody<PositionType, ActionType> Body { get; set; }

        protected abstract IEnumerable<ActionType> FindPossibleActions(PositionType position);

        public readonly IEvaluationMethod<ActionType> ActionEvaluation;


        public void SetStartPosition(PositionType position)
        {
            Body.CurrentPosition = position;
        }

        public PositionType BestPosition()
        {
            return Body.CurrentPosition;
        }
    }
}
