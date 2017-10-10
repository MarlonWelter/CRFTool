using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace GrundWelt
{
    public abstract class GWAction
    {
        public double Score { get; set; }
    }
    public abstract class Strategy<ActionType, Position> : GWAction
        where ActionType : GWAction
    {
        public abstract LinkedList<ActionType> FindActions();
        public abstract LinkedList<ActionType> FindCounterActions();
    }
    public abstract class Position<ActionType>
        where ActionType : GWAction
    {
        public abstract void Execute(ActionType action);
    }
    public abstract class Model<PositionType, ActionType>
        where PositionType : Position<ActionType>
        where ActionType : GWAction
    {
        public PositionType Position { get; set; }
        
    }
    public abstract class Idea<PositionType, ActionType>
        where ActionType : GWAction
    {
        public double Weight { get; set; }
        public abstract LinkedList<ActionType> FindOptions(PositionType position);
    }
    public abstract class PlayerMind<PositionType, ModelType, Strategy, ActionType>
        where ActionType : GWAction
        where Strategy : Strategy<ActionType, ModelType>
        where ModelType : Model<PositionType, ActionType>
        where PositionType : Position<ActionType>
    {
        public List<Idea<ModelType, Strategy>> Ideas { get; set; }

        public IEvaluationMethod<ModelType> EvaluationMethod { get; set; }

        protected LinkedList<ActionType> CurrentActions = new LinkedList<ActionType>();

        public ActionType FindNextMove(PositionType position)
        {
            var model = EvaluatePosition(position);
            var options = FindOptions(model);
            return null;
        }

        protected abstract ModelType EvaluatePosition(PositionType position);

        public int MaximumOptions { get; set; }
        protected LinkedList<Strategy> FindOptions(ModelType situation)
        {
            var options = new LinkedList<Strategy>();
            foreach (var idea in Ideas)
            {
                var optionsLocal = idea.FindOptions(situation);
                foreach (var option in optionsLocal)
                {
                    options.SortedInsert(option, MaximumOptions, (action) => action.Score);
                }
            }
            return options;
        }


        public ModelType CurrentSituation { get; set; }
        protected void AnalyzeOptions(LinkedList<Strategy> strategies)
        {
            //var bestOption = default(ActionType);

            foreach (var strategy in strategies)
            {
                var options = strategy.FindActions();
                foreach (var option in options)
                {
                    AnalyzeOption(option, strategy);
                }
            }
        }

        private void AnalyzeOption(ActionType option, Strategy strategy)
        {
            CurrentSituation.Position.Execute(option);
        }
    }
}
