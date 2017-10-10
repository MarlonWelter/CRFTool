using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    //public class GWRunnerCenter<InputData, PositionData, ActionData>
    //    where PositionData : Cloneable<PositionData>
    //{
    //    public GWRunnerCenter(int resultsSize, double[] feedBack, GWInputConverter<InputData, PositionData> inputConverter, GWActionExecuter<PositionData, ActionData> actionExecuter, GWActionFinder<PositionData, ActionData> actionFinder, IEnumerable<GWActionEvaluator<PositionData, ActionData>> actionEvaluators, IEnumerable<GWPositionEvaluator<PositionData, ActionData>> positionEvaluators)
    //    {
    //        InputConverter = inputConverter;
    //        ActionExecuter = actionExecuter;
    //        PositionEvaluators = positionEvaluators.ToArray();
    //        ActionFinder = actionFinder;
    //        ActionEvaluators = actionEvaluators.ToArray();

    //        ResultCenter = new ResultCenter<PositionData, ActionData>(resultsSize, feedBack, PositionEvaluators);
    //        //ResultCenter.FeedPipe.Connect(OnResultCenterFeed);

    //    }

    //    public readonly Random Random = new Random();

    //    private LinkedList<GWRunner<PositionData, ActionData>> Population = new LinkedList<GWRunner<PositionData, ActionData>>();

    //    private GWPosition<PositionData, ActionData> startPosition;

    //    private readonly GWPositionEvaluator<PositionData, ActionData>[] PositionEvaluators;
    //    private readonly LinkedList<Donator<PositionData, ActionData>> donators;

    //    public readonly GWInputConverter<InputData, PositionData> InputConverter;

    //    public readonly GWActionFinder<PositionData, ActionData> ActionFinder;

    //    public readonly GWActionExecuter<PositionData, ActionData> ActionExecuter;

    //    private readonly GWActionEvaluator<PositionData, ActionData>[] ActionEvaluators;

    //    private readonly ResultCenter<PositionData, ActionData> ResultCenter;

    //    public InputData Input { get; set; }

    //    public DateTime StartTime { get; set; }
    //    public DateTime EndTime { get; set; }

    //    private void ExecuteIteration()
    //    {
    //        foreach (var item in Population)
    //        {
    //            item.Act();
    //        }
    //    }

    //    private void FeedPopulation()
    //    {
    //        foreach (var item in Population)
    //        {
    //            var charackteristic = new double[PositionEvaluators.Length];
    //            for (int i = 0; i < PositionEvaluators.Length; i++)
    //            {
    //                charackteristic[i] = PositionEvaluators[i].Evaluate(item.CurrentPosition);
    //            }
    //            foreach (var donator in donators)
    //            {
    //                donator.AddItem(new AgO<GWRunner<PositionData, ActionData>, double[]>(item, charackteristic));
    //            }
    //        }
    //    }

    //    private void PopulationRecombination()
    //    {
    //        var newGeneration = new LinkedList<GWRunner<PositionData, ActionData>>();
    //        var matingList = new List<GWRunner<PositionData, ActionData>>();

    //        foreach (var item in Population)
    //        {
    //            if (item.Energy > 0)
    //            {
    //                newGeneration.Add(item);

    //                if (item.OpenForMating)
    //                {
    //                    var insertionIndex = Random.Next(matingList.Count + 1);
    //                    matingList.Insert(insertionIndex, item);
    //                }
    //            }
    //        }

    //        for (int i = 0; i < matingList.Count - 1; i += 2)
    //        {
    //            var parentOne = matingList[i];
    //            var parentTwo = matingList[i + 1];
    //            var child = Mate(parentOne.Mate(), parentTwo.Mate());
    //            newGeneration.Add(child);
    //        }

    //        Population = newGeneration;
    //    }

    //    private double moveCost = 1.0;
    //    private double mateCost = 3.0;
    //    private GWRunner<PositionData, ActionData> Mate(MatingInfo parentOne, MatingInfo parentTwo)
    //    {
    //        var actionProps = new List<GWProperty>();

    //        foreach (var item in parentOne.Properties)
    //        {
    //            if (Random.NextDouble() < 0.45)
    //            {
    //                actionProps.Add(item);
    //            }
    //        }

    //        while (Random.NextDouble() < 0.5)
    //        {
    //            var newProp = Properties.CreateRandomProperty(ActionEvaluators.Length);
    //            actionProps.Add(newProp);
    //        }

    //        var child = new GWRunner<PositionData, ActionData>(moveCost, mateCost, parentOne.Energy + parentTwo.Energy, ActionExecuter, ActionEvaluators, ActionFinder, actionProps);
    //        //child.SignalEndPosition.Connect(() => EndPositionDiscovered(child));

    //        return child;
    //    }


    //    public void Start(InputData input, TimeSpan computationTime)
    //    {
    //        Input = input;
    //        ComputationTime = computationTime;
    //        StartTime = DateTime.Now;
    //        Work();
    //        EndTime = DateTime.Now;
    //    }
    //    public MEvent<GWPosition<PositionData, ActionData>> NewBestResult = new MEvent<GWPosition<PositionData, ActionData>>();
    //    internal void EndPositionDiscovered(GWRunner<PositionData, ActionData> runner)
    //    {
    //        runner.CurrentPosition.IsEndPosition = true;
    //        ResultCenter.AddPosition(runner.CurrentPosition);
    //        Population.Remove(runner);
    //    }

    //    public static int NodesVisited = 0;
    //    private TimeSpan ComputationTime;
    //    internal void Work()
    //    {
    //        try
    //        {
    //            //prepare    
    //            startPosition = new GWPosition<PositionData, ActionData>(InputConverter.Convert(Input), 0);
    //            Population = new LinkedList<GWRunner<PositionData, ActionData>>();
    //            var startProps = 4;
    //            var startPop = 4;
    //            var actionProps = new GWProperty[startProps];
    //            for (int i = 0; i < startProps; i++)
    //            {
    //                actionProps[i] = Properties.CreateRandomProperty(ActionEvaluators.Length);
    //            }
    //            for (int i = 0; i < startPop; i++)
    //            {
    //                var startRunner = new GWRunner<PositionData, ActionData>(moveCost, mateCost, 1.0, ActionExecuter, ActionEvaluators, ActionFinder, actionProps);
    //                Population.Add(startRunner);
    //            }

    //            while (DateTime.Now - StartTime < ComputationTime)
    //            {
    //                ExecuteIteration();
    //                FeedPopulation();
    //                PopulationRecombination();
    //            }
    //        }
    //        catch (Exception e)
    //        {

    //        }
    //    }
    //}
}
