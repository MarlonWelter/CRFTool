using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class GWInputData<PositionData>
        where PositionData : Cloneable<PositionData>
    {
        public PartialTarget<PositionData>[] Targets { get; set; }
        public abstract PositionData CreateStartPosition();

    }
    public class GWPosition<PositionData, ActionData> : GWPosition<PositionData>
        where PositionData : Cloneable<PositionData>
    {
        internal GWPosition(PositionData data, int depth, LinkedList<int> categoryPath)
        {
            Depth = depth;
            Data = data;
            Id = Program.Random.Next();
            if (categoryPath.NotNullOrEmpty())
                CategoryPath.AddRange(categoryPath);
        }

        public int Id { get; set; }
        public LinkedList<int> CategoryPath { get; } = new LinkedList<int>();

        public LinkedList<GWAction<PositionData, ActionData>> Path { get; } = new LinkedList<GWAction<PositionData, ActionData>>();
        
        public void Try(int category)
        {
            OptionCategoriesTried.Add(category);
        }

        public LinkedList<int> OptionCategoriesTried { get; } = new LinkedList<int>();

    }
    public class GWPosition<PositionData> : GWPosition
        where PositionData : Cloneable<PositionData>
    {
        public PositionData Data { get; internal set; }
    }
    public class GWPosition
    {

        public bool HasInitActions { get; internal set; }

        public bool IsEndPosition { get; internal set; }

        public int Depth { get; internal set; }

        public double Score { get; internal set; }

    }

    public sealed class GWAction<PositionData, ActionData> : IComparable, IScoreHolder
        where PositionData : Cloneable<PositionData>
    {
        public GWAction(GWPosition<PositionData, ActionData> position, ActionData actiondata, bool hasImpactOnTarget)
        {
            HasImpactOnTarget = hasImpactOnTarget;
            Position = position;
            Data = actiondata;
        }
        public ActionData Data { get; set; }
        public GWPosition<PositionData, ActionData> Position { get; internal set; }

        public double Score { get; set; }
        public int TempNumber { get; internal set; }
        public bool HasImpactOnTarget { get; set; }
        internal EvaluatorInfo EvaluatorInfo { get; set; }

        public int CompareTo(object obj)
        {
            var other = obj as GWAction<PositionData, ActionData>;
            if (other == null)
                return 0;
            return Score.CompareTo(other.Score);
        }
    }

    public abstract class GWActionFinder<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public abstract LinkedList<GWAction<PositionData, ActionData>> FindActions(GWPosition<PositionData, ActionData> position);
    }

    public abstract class GWActionEvaluator<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public double Weight { get; set; }
        protected abstract double DoEvaluate(GWAction<PositionData, ActionData> action);
        public double Evaluate(GWAction<PositionData, ActionData> action)
        {
            try
            {
                return DoEvaluate(action);
            }
            catch
            {
                return 0.0;
            }
        }
    }
    public abstract class GWPositionEvaluator<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public GWPositionEvaluator(double startWeight, int priority)
        {
            Weight = startWeight;
            Priority = priority;
        }
        public double Weight { get; internal set; }
        public int Priority { get; internal set; }
        public abstract double Evaluate(GWPosition<PositionData, ActionData> position);
    }

    public class GenericPositionEvaluator<PositionData, ActionData> : GWPositionEvaluator<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public GenericPositionEvaluator(Func<PositionData, double> evaluation, double startWeight = 1.0, int priority = 0) : base(startWeight, priority)
        {
            Evaluation = evaluation;
        }
        Func<PositionData, double> Evaluation;

        public override double Evaluate(GWPosition<PositionData, ActionData> position)
        {
            return Evaluation(position.Data);
        }
    }

    public abstract class GWInputConverter<InputData, PositionData>
        where PositionData : Cloneable<PositionData>
    {
        public abstract PositionData Convert(InputData input);
    }
    //public abstract class GWResultConverter<ResultData, PositionData, ActionData>
    //{
    //    public abstract ResultData Convert(GWPosition<PositionData, ActionData> input);
    //}

    public abstract class GWActionExecuterClone<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        protected abstract PositionData ExecuteAction(GWAction<PositionData, ActionData> action);
        public GWPosition<PositionData, ActionData> CreateNewPosition(GWAction<PositionData, ActionData> action)
        {
            var newPosition = new GWPosition<PositionData, ActionData>(ExecuteAction(action), action.Position.Depth + 1, action.Position.CategoryPath);

            return newPosition;
        }
    }

    public abstract class GWActionExecuter<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        protected abstract PositionData ExecuteAction(GWAction<PositionData, ActionData> action);
        public GWPosition<PositionData, ActionData> CreateNewPosition(GWAction<PositionData, ActionData> action)
        {
            var newPosition = new GWPosition<PositionData, ActionData>(
            ExecuteAction(action), action.Position.Depth + 1, action.Position.CategoryPath);
            return newPosition;
        }
    }

    public abstract class GWProgressEstimator<PositionData>
        where PositionData : Cloneable<PositionData>
    {
        public abstract void Init(PositionData startPosition);
        public abstract double ExecuteAction(PositionData position);
    }

    public abstract class GWPositionCategorizer<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public abstract int Categorize(GWPosition<PositionData, ActionData> position);
    }

    public abstract class GWActionCategorizer<PositionData, ActionData>
      where PositionData : Cloneable<PositionData>
    {
        public abstract int Categorize(GWAction<PositionData, ActionData> action);
    }

    public abstract class PartialTarget<PositionData>
        where PositionData : Cloneable<PositionData>
    {
        public abstract bool ReachTarget(GWPosition<PositionData> position);
        public bool IsOptional { get; set; }
    }
}
