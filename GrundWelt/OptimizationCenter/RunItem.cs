using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class RunItem<PositionData, ActionData, CheckPointType>
             where PositionData : BasePositionData<PositionData, ActionData, CheckPointType>, Cloneable<PositionData>, IHasFingerprint
          where ActionData : BaseActionData<PositionData, ActionData, CheckPointType>
          where CheckPointType : CheckPoint<PositionData>
    {
        public RunItem(GWPosition<PositionData, ActionData> position, Action<RunItem<PositionData, ActionData, CheckPointType>> onTargetReached, ActionHandling<PositionData, ActionData> actionHandling, double[] weights, int multiEvaluationOptions)
        {
            MultiEvaluationOptions = multiEvaluationOptions;
            Id = Program.Random.Next();
            OnTargetReached = onTargetReached;
            ActionHandling = actionHandling;
            Weights = weights;
            if (Weights == null)
            {
                Weights = new double[actionHandling.ActionEvaluators.Count()];

                for (int i = 0; i < actionHandling.ActionEvaluators.Count(); i++)
                {
                    Weights[i] = Program.Random.NextDouble();
                }
            }

            PreSolutions.Add(new PreSolution<PositionData, ActionData, CheckPointType>(new GWPosition<PositionData, ActionData>(position.Data.Clone(), position.Depth + 1, position.CategoryPath), null));
        }
        public ActionHandling<PositionData, ActionData> ActionHandling { get; set; }

        internal double[] Weights { get; set; }
        public bool HasMoved { get; set; }
        public Action<RunItem<PositionData, ActionData, CheckPointType>> OnTargetReached { get; set; }

        internal List<PreSolution<PositionData, ActionData, CheckPointType>> PreSolutions { get; set; } = new List<PreSolution<PositionData, ActionData, CheckPointType>>();
        internal int PositionCounter { get; set; }

        internal bool MovedToNewArea { get; set; }
        internal PreSolution<PositionData, ActionData, CheckPointType> PreSolution => (PositionCounter < PreSolutions.Count) ? PreSolutions[PositionCounter] : null;
        public GWPosition<PositionData, ActionData> Position => PreSolution?.Position;
        public GWAction<PositionData, ActionData> ChosenAction { get; set; }

        public double Gold { get; set; }
        public int Id { get; set; }
        public double Score { get; set; }
        public int MultiEvaluationOptions { get; set; }

        public void Save(StreamWriter writer)
        {
            writer.WriteLine("Id: " + Id);
            writer.WriteLine("Gold: " + Gold);
            writer.WriteLine("Weights: ");
            for (int i = 0; i < Weights.Length; i++)
            {
                writer.WriteLine(Weights[i]);
            }
        }

        internal static RunItem<PositionData, ActionData, CheckPointType> Load(StreamReader reader, ActionHandling<PositionData, ActionData> actionHandling, int multiEvaluationOptions)
        {
            var id = int.Parse(reader.ReadLine().Substring(4));
            var gold = double.Parse(reader.ReadLine().Substring(6));
            reader.ReadLine(); //weights
            var line = "";
            var weights = new List<double>();
            while ((line = reader.ReadLine()).NotNullOrEmpty())
            {
                weights.Add(double.Parse(line));
            }
            var item = new RunItem<PositionData, ActionData, CheckPointType>(null, null, actionHandling, weights.ToArray(), multiEvaluationOptions);
            item.Id = id;
            item.Gold = gold;

            return item;
        }

        public void Clean()
        {
            HasMoved = false;
            Score = 0.0;
            ChosenAction = null;
            PreSolutions.Clear();
            MovedToNewArea = false;
        }

        public void RunUntilMeetTargetOrEnd(PartialTarget<PositionData>[] targets, CancelCriteria criteria, int maxSteps = 0)
        {
            //Position.Data.NextCheckPoint = NextCheckPoint;
            //LastCrossroad = new GWCrossroad<PositionData, ActionData>(Position);
            int stepsDone = 0;
            while (true)
            {
                //var fallbackDone = false;
                var currentTargetsReached = new bool[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    currentTargetsReached[i] = targets[i].ReachTarget(Position);
                }

                var options = new List<GWAction<PositionData, ActionData>>();
                foreach (var actionFinder in ActionHandling.ActionFinders)
                {
                    // filter actions that have been researched 
                    var newOptions = actionFinder.FindActions(Position);
                    options.AddRange(newOptions);
                    //options.AddRange(newOptions.Where(a => !Position.Data.ActionsTried.Contains(a.Data.FingerPrint)));
                }
                if (HasMoved)
                    options = (options.Where(op => op.HasImpactOnTarget)).ToList();
                //TODO: filter double actions (from several actionFinders)
                if (options.NullOrEmpty())
                {   //no more options that help reach a target, can stop searching
                    break;
                }

                //categorize actions
                //var optionCategories = options.Split(action => ActionHandling.ActionCategorizer.Categorize(action)).ToArray();
               

                if (options.NullOrEmpty())
                {   //no more options that help reach a target, can stop searching
                    break;
                }

                //choose by multievaluation:                
                ChosenAction = MultiEvaluation.Do(options, ActionHandling.ActionEvaluators, Weights, 1, MultiEvaluationOptions).First();
                              

                PreSolution.Position = ActionHandling.ActionExecuter.CreateNewPosition(ChosenAction);
                HasMoved = true;
                stepsDone++;

                //check if new target is reached
                bool newTargetReached = false;
                for (int i = 0; i < targets.Length; i++)
                {
                    var targetReached = targets[i].ReachTarget(Position);
                    if (currentTargetsReached[i] != targetReached)
                    {
                        currentTargetsReached[i] = targetReached;
                        newTargetReached = true;
                    }
                }
                if (targets.All(t => t.ReachTarget(Position) || t.IsOptional))
                {
                    Position.IsEndPosition = true;
                    OnTargetReached(this);
                    if (criteria == CancelCriteria.TargetsReached)
                        break;
                }
                if (newTargetReached)
                {
                    OnTargetReached(this);
                    if (criteria == CancelCriteria.TargetsReached)
                        break;
                }

                if (criteria == CancelCriteria.MaxSteps && stepsDone >= maxSteps)
                {
                    break;
                }
            }
            //Score = Position.Data.NextCheckPoint.Score(Position.Data);
        }

        public void RunUntilNewArea(PositionGuide<PositionData> guide, int positionCase = 0)
        {
            HasMoved = false;
            PositionCounter = positionCase;
            int stepsDone = 0;
            while (true)
            {
                //var fallbackDone = false;
                int areaId = guide.PositionAreaId(Position.Data);

                var options = new List<GWAction<PositionData, ActionData>>();
                foreach (var actionFinder in ActionHandling.ActionFinders)
                {
                    // filter actions that have been researched 
                    var newOptions = actionFinder.FindActions(Position);
                    options.AddRange(newOptions);
                    //options.AddRange(newOptions.Where(a => !Position.Data.ActionsTried.Contains(a.Data.FingerPrint)));
                }

             
                if (HasMoved)
                    options = (options.Where(op => op.HasImpactOnTarget)).ToList();
                //TODO: filter double actions (from several actionFinders)
                if (options.NullOrEmpty())
                {   //no more options that help reach a target, can stop searching
                    break;
                }

                if (options.NullOrEmpty())
                {   //no more options that help reach a target, can stop searching
                    break;
                }

                //choose by multievaluation:
                ChosenAction = MultiEvaluation.Do(options, ActionHandling.ActionEvaluators, Weights, 1, MultiEvaluationOptions).First();
                

                PreSolution.Position = ActionHandling.ActionExecuter.CreateNewPosition(ChosenAction);
                HasMoved = true;
                stepsDone++;

                //check if new area is reached
                int newAreaId = guide.PositionAreaId(Position.Data);
                if (areaId != newAreaId)
                {
                    MovedToNewArea = true;
                    Position.IsEndPosition = guide.IsEndPosition(Position.Data);
                    OnTargetReached(this);
                    break;
                }
            }
            //PreSolution.InternalCanBeFiltered = MovedToNewArea;
            //Score = Position.Data.NextCheckPoint.Score(Position.Data);
        }

        public void RunDoOneThing(PositionGuide<PositionData> guide, int positionCase = 0)
        {
            HasMoved = false;
            PositionCounter = positionCase;
            int stepsDone = 0;
            while (stepsDone == 0)
            {
                //int areaId = guide.PositionAreaId(Position.Data);

                var options = new List<GWAction<PositionData, ActionData>>();
                foreach (var actionFinder in ActionHandling.ActionFinders)
                {
                    // filter actions that have been researched 
                    var newOptions = actionFinder.FindActions(Position);
                    options.AddRange(newOptions);
                    //options.AddRange(newOptions.Where(a => !Position.Data.ActionsTried.Contains(a.Data.FingerPrint)));
                }    

                if (options.NullOrEmpty())
                {   //no more options that help reach a target, can stop searching
                    break;
                }                

                // choose by multievaluation:    
                if (options.Count > 1)
                    ChosenAction = MultiEvaluation.Do(options, ActionHandling.ActionEvaluators, Weights, 1, MultiEvaluationOptions).First();
                else
                    ChosenAction = options.First();
                
                // execute action
                PreSolution.Position = ActionHandling.ActionExecuter.CreateNewPosition(ChosenAction);
                HasMoved = true;
                stepsDone++;

                //check if new area is reached
                //int newAreaId = guide.PositionAreaId(Position.Data);
                //if (areaId != newAreaId)
                {
                    Position.IsEndPosition = guide.IsEndPosition(Position.Data);
                    OnTargetReached(this);
                    break;
                }
            }           
        }

        public RunItem<PositionData, ActionData, CheckPointType> Breed(RunItem<PositionData, ActionData, CheckPointType> item2)
        {
            var weights = new double[Weights.Length];

            for (int i = 0; i < weights.Length; i++)
            {
                var witem1 = GrundWelt.Program.Random.NextDouble();
                weights[i] = witem1 * Weights[i] + (1 - witem1) * item2.Weights[i] * (1.0 + GrundWelt.Program.Random.NextDouble() * 0.2 - 0.1);
            }

            var child = new RunItem<PositionData, ActionData, CheckPointType>(null, OnTargetReached, ActionHandling, weights, MultiEvaluationOptions);
            return child;
        }
    }
}
