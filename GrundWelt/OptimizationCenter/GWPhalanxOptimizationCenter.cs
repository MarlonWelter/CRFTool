using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{

    //class GWPhalanxOptimizationCenter<PositionData, ActionData> : OptUnit<PositionData, ActionData>, IOptimizationCenter<PositionData, ActionData>
    //    where PositionData : Cloneable<PositionData>
    //{
    //    public GWPhalanxOptimizationCenter(int phalanxSize, int resultsSize, double[] feedBack, GWActionExecuter<PositionData, ActionData> actionExecuter, IEnumerable<GWActionFinder<PositionData, ActionData>> actionFinders, IEnumerable<GWActionEvaluator<PositionData, ActionData>> actionEvaluators, IEnumerable<GWPositionEvaluator<PositionData, ActionData>> endPositionEvaluators) : base()
    //    {
    //        PhalanxSize = phalanxSize;
    //        ActionFinders = actionFinders;
    //        actionEvaluatorCage = new GWActionEvaluatorCage<PositionData, ActionData>(actionEvaluators.ToList());

    //        ResultCenter = new ResultCenter<PositionData, ActionData>(resultsSize, feedBack, endPositionEvaluators);
    //        ResultCenter.NewFeed.AddPath(OnResultCenterFeed);

    //        ActionExecuter = actionExecuter;
    //        Random = new Random();
    //    }
    //    IComputationInfo IOptimizationCenter<PositionData, ActionData>.ComputationInfo => ComputationInfo;

    //    public readonly Random Random;
    //    public int PhalanxSize;

    //    private LinkedList<PhalanxItem> Phalanx = new LinkedList<PhalanxItem>();
    //    private readonly LinkedList<PhalanxItem> NextGeneration = new LinkedList<PhalanxItem>();

    //    public readonly GWActionExecuter<PositionData, ActionData> ActionExecuter;
    //    public readonly IEnumerable<GWActionFinder<PositionData, ActionData>> ActionFinders;
    //    private readonly GWActionEvaluatorCage<PositionData, ActionData> actionEvaluatorCage;
    //    private readonly ResultCenter<PositionData, ActionData> ResultCenter;

    //    public MEvent<GWPosition<PositionData, ActionData>> NewBestResult { get; } = new MEvent<GWPosition<PositionData, ActionData>>();

    //    public LinkedList<SpeedChangeMessage> Inbox { get; } = new LinkedList<SpeedChangeMessage>();

    //    public void Start(GWInputData<PositionData> input)
    //    {
    //        ComputationInfo.IsActive = true;
    //        Input = input;
    //        StartTime = DateTime.Now;
    //        Work();
    //        EndTime = DateTime.Now;
    //        ComputationInfo.IsActive = false;
    //        RunFinished.Enter();
    //    }
    //    private void OnResultCenterFeed(ResultCenterFeed<PositionData, ActionData> obj)
    //    {
    //        actionEvaluatorCage.Feedback(obj.Item.Path, obj.Food);
    //        if (obj.Rank == 0)
    //        {
    //            NewBestResult.Enter(obj.Item);
    //        }
    //    }

    //    double maxLength = 0;
    //    internal void EndPositionDiscovered(GWPosition<PositionData, ActionData> position)
    //    {
    //        maxLength = Math.Max(position.Depth, maxLength);
    //        position.IsEndPosition = true;
    //        ResultCenter.AddPosition(position);
    //    }

    //    private void CheckInbox()
    //    {
    //        while (Inbox.Any())
    //        {
    //            var message = Inbox.First.Value;
    //            Inbox.RemoveFirst();
    //            switch (message)
    //            {
    //                case SpeedChangeMessage.Stop:
    //                    ComputationInfo.IsActive = false;
    //                    break;
    //                case SpeedChangeMessage.SpeedUp:
    //                    PhalanxSize = (int)(PhalanxSize / 2);
    //                    break;
    //                case SpeedChangeMessage.SlowDown:
    //                    PhalanxSize *= 2;
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }
    //    }

    //    public static int NodesVisited = 0;
    //    internal void Work()
    //    {
    //        try
    //        {
    //            //prepare
    //            ComputationInfo.Progress = 0; maxLength = 0;
    //            Phalanx.Clear();
    //            NextGeneration.Clear();
    //            var startItem = new PhalanxItem(null, new GWPosition<PositionData, ActionData>(Input.CreateStartPosition(), 0, null));
    //            Phalanx.Add(startItem);
    //            startItem.Power = PhalanxSize;

    //            double iterationCounter = 0;
    //            while (Phalanx.NotNullOrEmpty() && ComputationInfo.IsActive)
    //            {
    //                CheckInbox();

    //                foreach (var item in Phalanx)
    //                {
    //                    //create Offspring
    //                    int numberOffspring = (int)item.Power + (Random.NextDouble() < item.Power - (int)item.Power ? 1 : 0);

    //                    if (numberOffspring > 0)
    //                    {
    //                        //gather options:
    //                        var options = new List<GWAction<PositionData, ActionData>>();
    //                        foreach (var actionFinder in ActionFinders)
    //                        {
    //                            var foundOptions = actionFinder.FindActions(item.Position);
    //                            options.AddRange(foundOptions);
    //                            //TODO: filter double actions (from several actionFinders)
    //                        }
    //                        if (options.NullOrEmpty())
    //                        {
    //                            //discovery of ending position
    //                            EndPositionDiscovered(item.Position);
    //                        }
    //                        else
    //                        {
    //                            options = actionEvaluatorCage.Choose(options, numberOffspring);
    //                        }

    //                        foreach (var option in options)
    //                        {
    //                            option.Position = new GWPosition<PositionData, ActionData>(option.Position.Data.Clone(), option.Position.Depth, null);
    //                            option.Position.Path.AddRange(item.Position.Path);
    //                            option.Position.Path.Add(option);
    //                            var newpos = ActionExecuter.CreateNewPosition(option);
    //                            //option.Position.Depth += 1;
    //                            var child = new PhalanxItem(item, newpos);
    //                            NextGeneration.Add(child);
    //                        }
    //                    }
    //                }

    //                //Analyze next Generation
    //                int runs = 6;
    //                int resultSize = 3 * PhalanxSize;
    //                LinkedList<AgO<PhalanxItem, GWPosition<PositionData, ActionData>>> results = new LinkedList<AgO<PhalanxItem, GWPosition<PositionData, ActionData>>>();
    //                double itemCounter = 1.0;
    //                foreach (var item in NextGeneration)
    //                {
    //                    for (int i = 0; i < runs; i++)
    //                    {
    //                        var runPosition = new GWPosition<PositionData, ActionData>(item.Position.Data.Clone(), item.Position.Depth, null);
    //                        runPosition.Path.AddRange(item.Position.Path);
    //                        var resPos = Run(runPosition);
    //                        item.Position = resPos;
    //                        results.SortedInsert(new AgO<PhalanxItem, GWPosition<PositionData, ActionData>>(item, resPos), resultSize, (rp) => rp.Data2.Score);
    //                    }
    //                    if (maxLength > 0)
    //                    {
    //                        double iterationProg = (iterationCounter + itemCounter / NextGeneration.Count) / maxLength;
    //                        ComputationInfo.Progress = (1.0 - 0.5 * iterationProg) * iterationProg;
    //                        CheckInbox();
    //                        if (!ComputationInfo.IsActive)
    //                            return;
    //                    }
    //                    itemCounter++;
    //                }
    //                // give Power
    //                var resultsList = results.ToList();
    //                for (int i = 0; i < results.Count; i++)
    //                {
    //                    resultsList[i].Data1.Power += ((double)(resultSize - i)) / PhalanxSize;
    //                }

    //                //fill new phalanx
    //                Phalanx.Clear();
    //                if (NextGeneration.NotNullOrEmpty())
    //                {
    //                    var phalanxTemp = new LinkedList<PhalanxItem>(NextGeneration.OrderBy(ph => ph.Power));
    //                    int throwAway = phalanxTemp.Count() - PhalanxSize;
    //                    int sortedOut = 0;
    //                    while (sortedOut < throwAway)
    //                    {
    //                        var last = phalanxTemp.Last.Value;
    //                        phalanxTemp.RemoveLast();

    //                        if (last.Bonus > 0)
    //                        {
    //                            last.Power += last.Bonus;
    //                            last.Bonus = 0;
    //                            phalanxTemp.SortedInsert(last, (l) => l.Power);
    //                            continue;
    //                        }

    //                        sortedOut++;
    //                        if (last.Parent != null)
    //                        {
    //                            var liveChildren = last.Parent.Children.Sum(c => c.SortedOut ? 0 : 1);
    //                            foreach (var child in last.Parent.Children.Where(c => !c.SortedOut))
    //                            {
    //                                child.Bonus += last.Power / liveChildren;
    //                            }
    //                        }
    //                    }
    //                    foreach (var item in phalanxTemp)
    //                    {
    //                        item.Power += item.Bonus;
    //                        item.Bonus = 0;
    //                        item.Parent = null;
    //                    }
    //                    Phalanx = new LinkedList<PhalanxItem>(phalanxTemp.OrderByDescending(p => p.Power));
    //                    //normalize:
    //                    var sum = Phalanx.Sum(p => p.Power);
    //                    var factor = resultSize / sum;
    //                    Phalanx.Each(p => p.Power *= factor);
    //                    NextGeneration.Clear();
    //                }
    //                actionEvaluatorCage.Age();
    //                iterationCounter++;
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            throw e;
    //        }
    //    }

    //    private GWPosition<PositionData, ActionData> Run(GWPosition<PositionData, ActionData> position)
    //    {
    //        var options = new LinkedList<GWAction<PositionData, ActionData>>();
    //        var resPos = position;
    //        do
    //        {
    //            options.Clear();
    //            foreach (var actionFinder in ActionFinders)
    //            {
    //                var foundOptions = actionFinder.FindActions(position);
    //                options.AddRange(foundOptions);
    //                //TODO: filter double actions (from several actionFinders)
    //            }
    //            if (options.NullOrEmpty())
    //            {
    //                //discovery of ending position
    //                EndPositionDiscovered(position);
    //            }
    //            else
    //            {
    //                var best = actionEvaluatorCage.Choose(options, 1).FirstOrDefault();
    //                if (best != null)
    //                {
    //                    resPos = ActionExecuter.CreateNewPosition(best);
    //                    position.Path.Add(best);
    //                }
    //            }
    //        } while (options.NotNullOrEmpty());
    //        return resPos;
    //    }



    //    class PhalanxItem
    //    {
    //        public PhalanxItem(PhalanxItem parent, GWPosition<PositionData, ActionData> position)
    //        {
    //            Children = new LinkedList<PhalanxItem>();
    //            Parent = parent;
    //            if (parent != null)
    //                parent.Children.Add(this);
    //            Position = position;
    //        }
    //        public GWPosition<PositionData, ActionData> Position { get; set; }
    //        public double Power { get; set; }
    //        public double Bonus { get; set; }
    //        public bool SortedOut { get; set; }
    //        public PhalanxItem Parent { get; set; }
    //        public LinkedList<PhalanxItem> Children { get; set; }
    //    }

    //}
}
