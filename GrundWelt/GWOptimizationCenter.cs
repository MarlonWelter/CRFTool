using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace GrundWelt
{

    public class GWOptimizationCenter<InputData, PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public GWOptimizationCenter(GWInputConverter<InputData, PositionData> inputConverter, GWActionExecuterClone<PositionData, ActionData> actionExecuter, IEnumerable<GWActionFinder<PositionData, ActionData>> actionFinders, IEnumerable<GWActionEvaluator<PositionData, ActionData>> actionEvaluators, IEnumerable<GWPositionEvaluator<PositionData, ActionData>> positionEvaluators, GWPositionEvaluator<PositionData, ActionData> endPositionEvaluator, GWBranchingPointsControlUnit<PositionData, ActionData> branchingPointsControlUnit)
        {
            InputConverter = inputConverter;
            ActionFinders = actionFinders;
            ActionEvaluators = actionEvaluators;
            PositionEvaluators = positionEvaluators;
            EndPositionEvaluator = endPositionEvaluator;
            ActionExecuter = actionExecuter;
            BranchingPointsControlUnit = branchingPointsControlUnit;
        }
        public GWBranchingPointsControlUnit<PositionData, ActionData> BranchingPointsControlUnit { get; set; }

        public GWActionExecuterClone<PositionData, ActionData> ActionExecuter { get; set; }

        public GWInputConverter<InputData, PositionData> InputConverter { get; set; }

        public IEnumerable<GWActionFinder<PositionData, ActionData>> ActionFinders { get; set; }

        public IEnumerable<GWActionEvaluator<PositionData, ActionData>> ActionEvaluators { get; set; }

        public IEnumerable<GWPositionEvaluator<PositionData, ActionData>> PositionEvaluators { get; set; }

        public GWPositionEvaluator<PositionData, ActionData> EndPositionEvaluator { get; set; }

        public InputData Input { get; set; }

        public GWPosition<PositionData, ActionData> BestResult { get; set; }

        public GWPosition<PositionData, ActionData> CurrentPosition { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan ComputationTime { get; set; }

        public void Start(InputData input, TimeSpan computationTime)
        {
            ComputationTime = computationTime;
            Input = input;
            BestResult = null;
            CurrentPosition = null;
            StartTime = DateTime.Now;
            Work();
            EndTime = DateTime.Now;
        }

        public MEvent<GWPosition<PositionData, ActionData>> NewBestResult = new MEvent<GWPosition<PositionData, ActionData>>();
        internal void EndPositionDiscovered(GWPosition<PositionData, ActionData> position)
        {
            CurrentPosition.IsEndPosition = true;
            CurrentPosition.Score = EndPositionEvaluator.Evaluate(CurrentPosition);

            if (BestResult == null || CurrentPosition.Score > BestResult.Score)
            {
                BestResult = CurrentPosition;
                NewBestResult.Enter(BestResult);
            }
        }

        public static int NodesVisited = 0;
        internal void Work()
        {
            try
            {
                //prepare
                CurrentPosition = new GWPosition<PositionData, ActionData>(InputConverter.Convert(Input), 0, null);
                //CurrentPositions.AddLast(CurrentPosition);

                var isnewPosition = true;

                while (CurrentPosition != null && DateTime.Now - StartTime < ComputationTime)
                {
                    NodesVisited++;
                    //foreach (var position in CurrentPositions)
                    {
                        //gather options:
                        var options = new LinkedList<GWAction<PositionData, ActionData>>();

                        if (CurrentPosition.HasInitActions)
                        {
                            //options.AddRange(CurrentPosition.UndiscoveredActions);
                        }
                        else
                        {
                            foreach (var actionFinder in ActionFinders)
                            {
                                var foundOptions = actionFinder.FindActions(CurrentPosition);
                                options.AddRange(foundOptions);
                                //TODO: filter double actions (from several actionFinders)
                            }
                            if (options.NullOrEmpty())
                            {
                                //discovery of ending position
                                EndPositionDiscovered(CurrentPosition);
                            }
                            else
                            {
                                foreach (var actionEvaluator in ActionEvaluators)
                                {
                                    actionEvaluator.Weight = Program.Random.NextDouble();
                                }
                                int tempNr = 0;
                                foreach (var option in options)
                                {
                                    option.TempNumber = tempNr;
                                    tempNr++;
                                }
                                //evaluateOptions
                                int EachEvaluatorOptions = 5;
                                //int Branches = 50;
                                var optionsScore = new double[options.Count];
                                foreach (var actionEvaluator in ActionEvaluators)
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
                                        optionsScore[bestOptionsList[i].TempNumber] += (EachEvaluatorOptions - i) * actionEvaluator.Weight;
                                    }
                                }
                                foreach (var option in options)
                                {
                                    option.Score = optionsScore[option.TempNumber];
                                }
                                //CurrentPosition.AddActions(options.MaxEntries(o => o.Score, Branches));
                            }
                        }

                        if (options.NullOrEmpty())
                        {
                            //this position is fully discovered

                            BranchingPointsControlUnit.RemoveBranchingPoint(CurrentPosition);

                            CurrentPosition = BranchingPointsControlUnit.GetNextBranchingPoint();
                            isnewPosition = false;
                        }
                        else
                        {
                            if (isnewPosition)
                                BranchingPointsControlUnit.AddBranchingPoint(CurrentPosition);

                            var bestOption = options.MaxEntry(op => op.Score);

                            //execute best Option
                            //var bestOption = ordered_options.First();


                            var newPosition = ActionExecuter.CreateNewPosition(bestOption);

                            foreach (var evaluator in PositionEvaluators)
                            {
                                newPosition.Score += evaluator.Evaluate(newPosition);
                            }

                            //newPosition.Parents.AddLast(CurrentPosition);
                            //CurrentPosition.Children.AddLast(newPosition);
                            //CurrentPosition.UndiscoveredActions.Remove(bestOption);
                            //CurrentPosition.DiscoveredActions.AddLast(new AgO<GWAction<PositionData, ActionData>, GWPosition<PositionData, ActionData>>(bestOption, null));
                            //CurrentPosition.DiscoveredActions.AddLast(new AgO<GWAction<PositionData, ActionData>, GWPosition<PositionData, ActionData>>(bestOption, newPosition));



                            CurrentPosition = newPosition;
                            isnewPosition = true;
                        }


                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }



    }
}
