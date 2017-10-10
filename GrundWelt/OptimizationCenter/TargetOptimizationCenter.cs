using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    class TargetOptimizationCenter<PositionData, ActionData, CheckPointType> : OptUnit<PositionData, ActionData>, ITargetOptCenter<PositionData, ActionData, CheckPointType>
            where PositionData : BasePositionData<PositionData, ActionData, CheckPointType>, Cloneable<PositionData>, IHasFingerprint
        where ActionData : BaseActionData<PositionData, ActionData, CheckPointType>
        where CheckPointType : CheckPoint<PositionData>
    {
        public string Id { get; set; }

        public TargetOptCenterParams TargetOptCenterParams { get; set; }

        public TargetOptimizationCenter(GWActionExecuter<PositionData, ActionData> actionExecuter, IEnumerable<GWActionFinder<PositionData, ActionData>> actionFinders, IEnumerable<GWActionEvaluator<PositionData, ActionData>> actionEvaluators, GWPositionEvaluator<PositionData, ActionData> endPositionEvaluator, GWProgressEstimator<PositionData> progressEstimator, GWPositionCategorizer<PositionData, ActionData> positionCategorizer, GWActionCategorizer<PositionData, ActionData> actionCategorizer, CategoryEvaluator<PositionData, ActionData, CheckPointType> categoryEvaluationLogic, CancelCriteria cancelCriteria, string id, TargetOptCenterParams optCenterParams, PositionGuide<PositionData> areaGuide, MandatoryActionExecuter<PositionData, ActionData, CheckPointType> mandatoryActionExecuter, RunnerPool<PositionData, ActionData, CheckPointType> runnerPool) : base()
        {
            MandatoryActionExecuter = mandatoryActionExecuter;
            AreaGuide = areaGuide;
            Id = id;
            ProgressEstimator = progressEstimator;
            PositionCategorizer = positionCategorizer;
            CancelCriteria = cancelCriteria;
            TargetOptCenterParams = optCenterParams;

            EndPositionEvaluator = endPositionEvaluator;
            CategoryEvaluationLogic = categoryEvaluationLogic;

            Pool = runnerPool;
            Pool.OnEndpositionDiscovered = (item) => EndPositionDiscovered(item.Position);

        }
        MandatoryActionExecuter<PositionData, ActionData, CheckPointType> MandatoryActionExecuter { get; set; }
        readonly GWProgressEstimator<PositionData> ProgressEstimator;
        readonly GWPositionCategorizer<PositionData, ActionData> PositionCategorizer;
        PartialTarget<PositionData>[] Targets { get; set; }

        public PositionGuide<PositionData> AreaGuide { get; set; }

        public CancelCriteria CancelCriteria { get; set; }

        List<RunItem<PositionData, ActionData, CheckPointType>> RunnerPool { get; set; } = new List<RunItem<PositionData, ActionData, CheckPointType>>();
        //List<RunItem<PositionData, ActionData, CheckPointType>> CorePopulation { get; set; } = new List<RunItem<PositionData, ActionData, CheckPointType>>();
        //List<RunItem<PositionData, ActionData, CheckPointType>> WildCardPopulation { get; set; } = new List<RunItem<PositionData, ActionData, CheckPointType>>();
        private List<LocalCheckPoint> CheckPoints = new List<LocalCheckPoint>();
        private IEnumerable<PreSolution<PositionData, ActionData, CheckPointType>> LastGeneration;

        private readonly RunnerPool<PositionData, ActionData, CheckPointType> Pool;

        public MEvent<GWPosition<PositionData, ActionData>> NewBestResult { get; } = new MEvent<GWPosition<PositionData, ActionData>>();

        private GWPosition<PositionData, ActionData> BestResult { get; set; }
        public GWPositionEvaluator<PositionData, ActionData> EndPositionEvaluator { get; set; }

        public CategoryEvaluator<PositionData, ActionData, CheckPointType> CategoryEvaluationLogic { get; set; }


        public LinkedList<SpeedChangeMessage> Inbox { get; } = new LinkedList<SpeedChangeMessage>();
        List<PreSolution<PositionData, ActionData, CheckPointType>> PreSolutions { get; set; }

        public void Start(GWInputData<PositionData> input)
        {
            ComputationInfo.IsActive = true;
            Input = input;
            Targets = Input.Targets;
            StartTime = DateTime.Now;
            RunStarted.Enter();
            Work();
            EndTime = DateTime.Now;
            ComputationInfo.IsActive = false;
            RunFinished.Enter();
        }

        internal void EndPositionDiscovered(GWPosition<PositionData, ActionData> position)
        {
            if (position.IsEndPosition)
            {
                CategoryEvaluationLogic.AddPositionFound(position);
                position.Score = EndPositionEvaluator.Evaluate(position);

                if (BestResult == null || position.Score > BestResult.Score)
                {
                    BestResult = position;
                    NewBestResult.Enter(position);
                }
            }
        }

        private void CheckInbox()
        {
            while (Inbox.Any())
            {
                var message = Inbox.First.Value;
                Inbox.RemoveFirst();
                switch (message)
                {
                    case SpeedChangeMessage.Stop:
                        ComputationInfo.IsActive = false;
                        break;
                    case SpeedChangeMessage.SpeedUp:
                        TargetOptCenterParams.RunnerPoolSize = (int)(TargetOptCenterParams.RunnerPoolSize / 2);
                        break;
                    case SpeedChangeMessage.SlowDown:
                        TargetOptCenterParams.RunnerPoolSize *= 2;
                        break;
                    default:
                        break;
                }
            }
        }

        Dictionary<double, RunItem<PositionData, ActionData, CheckPointType>> Fingerprints = new Dictionary<double, RunItem<PositionData, ActionData, CheckPointType>>();
        internal void Work()
        {
            try
            {
                Prepare();

                while (CheckForAnotherIteration())
                {
                    CheckInbox();

                    PrepareIteration();

                    foreach (var champ in RunnerPool)
                    {
                        for (int positionIndex = 0; positionIndex < champ.PreSolutions.Count; positionIndex++)
                        {
                            //champ.RunUntilMeetTargetOrEnd(Targets, CancelCriteria, TargetOptCenterParams.RunItemSearchDepth);
                            //champ.RunUntilNewArea(AreaGuide, positionIndex);
                            champ.RunDoOneThing(AreaGuide, positionIndex);
                            addToNextGenForFollowUp(champ);
                        }
                        champ.PreSolutions.Clear();
                    }
                    ComputationInfo.Progress = RunnerPool.Average(it => ProgressEstimator.ExecuteAction(it?.Position?.Data));

                    EvaluateCheckpoints();

                    FilterCheckPoints();

                    //feedback to runItems
                    //foreach (var checkpoint in CheckPoints)
                    //{
                    //    foreach (var item in checkpoint.Positions)
                    //    {
                    //        //item.RunItemPath.Last().Gold++;
                    //    }
                    //}

                    finishIteration();

                    //GatherMetaInfo();
                }

                LastGeneration.Each(i => EndPositionDiscovered(i.Position));

                //PostRunFeedbackToChamps();

                Pool.RefreshPoolPopulation(TargetOptCenterParams.PoolPopRefreshRatio);
                //save champs
                Pool.Store();

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }


        //private void PostRunFeedbackToChamps()
        //{
        //    //most simple variant:
        //    foreach (var item in PreSolutions)
        //    {
        //        foreach (var champ in item.RunItemPath)
        //        {
        //            champ.Gold++;
        //        }
        //    }
        //}

        private void finishIteration()
        {
            var preSolutions = CheckPoints.SelectMany(ng => ng.Positions).ToList();

            LastGeneration = CheckPoints.SelectMany(ng => ng.Positions).ToList();

            //execute mandatory actions:
            PreSolutions.Clear();
            foreach (var pre in preSolutions)
            {
                PreSolutions.AddRange(MandatoryActionExecuter.ExecuteMandatoryActions(pre.Position).Select(pos => new PreSolution<PositionData, ActionData, CheckPointType>(pos, pre.RunItemPath)));
            }
        }

        private void FilterCheckPoints()
        {
            //filter out checkpoints that cannot improve the result anyway
            CheckPoints = CheckPoints.Where(cp => cp.Evaluation.CanImproveBestResult).ToList();

            ////filter out positions that are definitely worse than others
            //foreach (var cp in CheckPoints)
            //{
            //    var runItems = cp.Positions.ToList();
            //    for (int i = 1; i < cp.Positions.Count; i++)
            //    {
            //        for (int i2 = 0; i2 < i; i2++)
            //        {
            //            if (!AreaGuide.CanOvercome(runItems[i].Position.Data, runItems[i2].Position.Data))
            //            {
            //                cp.Positions.Remove(runItems[i]);
            //            }
            //        }
            //    }
            //}

            //filter out to maximum checkpoints
            var orderedCPs = CheckPoints.OrderByDescending(cp => cp.Evaluation.EstimatedScore).ToList();
            CheckPoints = orderedCPs.Take(12).ToList();

            double numberCheckpoints = CheckPoints.Count;
            int centerCP = (int)Math.Ceiling((numberCheckpoints - 1) / 2.0);
            double totalRunItems = CheckPoints.Sum(cp => cp.Positions.Count);
            var targetSizeCP = (int)Math.Ceiling(TargetOptCenterParams.NextGenerationSize / numberCheckpoints);
            var freePlaces = CheckPoints.Sum(cp => Math.Max(0, targetSizeCP - cp.Positions.Count));
            double reduceFactor = 1.0 - ((double)TargetOptCenterParams.NextGenerationSize) / totalRunItems;
            int counter = 0;
            foreach (var checkPoint in CheckPoints.OrderByDescending(cp => cp.Evaluation.EstimatedScore))
            {
                var reduce = checkPoint.Positions.Count - targetSizeCP;
                reduce += counter - centerCP;
                reduce = Math.Min(reduce, checkPoint.Positions.Count);

                if (reduce > 0 && freePlaces > 0)
                {
                    var takeFP = Math.Min(freePlaces, reduce);
                    reduce -= takeFP;
                    freePlaces -= takeFP;
                }

                for (int i = 0; i < reduce; i++)
                {
                    //if (checkPoint.Positions.Last().InternalCanBeFiltered)
                        checkPoint.Positions.RemoveLast();
                }
                counter++;
            }
        }

        private void EvaluateCheckpoints()
        {
            foreach (var checkPoint in CheckPoints)
            {
                var pos = checkPoint.Positions.First.Value.Position;
                checkPoint.Evaluation = EvaluateCategory(pos);
            }
        }

        private void PrepareIteration()
        {
            Fingerprints = new Dictionary<double, RunItem<PositionData, ActionData, CheckPointType>>();
            CheckPoints = new List<LocalCheckPoint>();

            RunnerPool.Clear();
            RunnerPool.AddRange(Pool.RequestRunItems(PreSolutions.Select(ps => ps.Position)));

            RunnerPool.Each(rI => rI.OnTargetReached = (item) => EndPositionDiscovered(item.Position));

        }

        private bool CheckForAnotherIteration()
        {
            return PreSolutions.NotNullOrEmpty() && ComputationInfo.IsActive;
        }

        private void Prepare()
        {
            random = new Random(2016);
            ComputationInfo.Progress = 0;
            BestResult = null;
            RunnerPool.Clear();
            CheckPoints.Clear();
            CategoryEvaluationLogic.Reset();
            LastGeneration = new List<PreSolution<PositionData, ActionData, CheckPointType>>();
            PreSolutions = new List<PreSolution<PositionData, ActionData, CheckPointType>>();
            var startPos = MandatoryActionExecuter.ExecuteMandatoryActions(new GWPosition<PositionData, ActionData>(Input.CreateStartPosition(), 0, null));
            PreSolutions.AddRange(startPos.Select(pos => new PreSolution<PositionData, ActionData, CheckPointType>(pos)));
            ProgressEstimator.Init(PreSolutions.First().Position.Data);
        }

        private CategoryEvaluation EvaluateCategory(GWPosition<PositionData, ActionData> pos)
        {
            var score = CategoryEvaluationLogic.Evaluate(pos);

            return score;
        }

        private Random random = new Random(2016);
        private void addToNextGenForFollowUp(RunItem<PositionData, ActionData, CheckPointType> item)
        {
            //var category = PositionCategorizer.Categorize(item.Position);

            var area = AreaGuide.PositionAreaId(item.Position.Data);

            //item.PreSolution.CategoryPath.Add(category);
            item.Position.CategoryPath.Add(area);

            if (item.HasMoved && !Fingerprints.ContainsKey(item.Position.Data.Fingerprint) && !(Targets.All(t => t.ReachTarget(item.Position) || t.IsOptional) && CancelCriteria == CancelCriteria.TargetsReached) && !(CancelCriteria == CancelCriteria.MaxSteps && item.Position.Depth >= TargetOptCenterParams.MaxSearchDepth))
            {
                //item.Score = item.Position.Data.NextCheckPoint.Score(item.Position.Data);
                item.PreSolution.Score = AreaGuide.Score(item.Position.Data) + random.NextDouble() * 0.00001;

                var nextGen = CheckPoints.FirstOrDefault(ng => ng.CategoryNr == area);
                if (nextGen == null)
                {
                    nextGen = new LocalCheckPoint();
                    nextGen.CategoryNr = area;
                    CheckPoints.Add(nextGen);
                }

                //if (nextGen.Positions.All(rI => AreaGuide.CanOvercome(item.Position.Data, rI.Position.Data)))
                {
                    nextGen.Positions.SortedInsert(item.PreSolution, TargetOptCenterParams.NextGenerationSize, (c) => c.Score);
                    Fingerprints.Add(item.Position.Data.Fingerprint, item);
                }
            }
            else
            {

            }
        }

        //private void distributeStartingPositionsToRunners()
        //{
        //    foreach (var item in RunnerPool)
        //    {
        //        item.Clean();
        //        item.AssignPreSolution
        //    }

        //    foreach (var pos in PreSolutions)
        //    {
        //        foreach (var runItem in Pool.RequestCore(pos.Position.Data))
        //        {
        //            var clonePos = new PreSolution<PositionData, ActionData, CheckPointType>(new GWPosition<PositionData, ActionData>(pos.Position.Data.Clone(), pos.Position.Depth + 1, pos.Position.FallBackPosition), pos.RunItemPath, pos.CategoryPath);

        //            runItem.AssignPreSolution(clonePos);
        //        }
        //    }

        //    for (int i = 0; i < WildCardPopulation.Count; i++)
        //    {
        //        if (i >= PreSolutions.Count)
        //        {
        //            var pos = PreSolutions[i % PreSolutions.Count];
        //            var clonePos = new PreSolution<PositionData, ActionData, CheckPointType>(new GWPosition<PositionData, ActionData>(pos.Position.Data.Clone(), pos.Position.Depth + 1, pos.Position.FallBackPosition), pos.RunItemPath, pos.CategoryPath);

        //            WildCardPopulation[i].AssignPreSolution(clonePos);
        //        }
        //        else
        //            WildCardPopulation[i].AssignPreSolution(PreSolutions[i]);
        //    }
        //}

        class LocalCheckPoint
        {
            public int CategoryNr { get; set; }
            public LinkedList<PreSolution<PositionData, ActionData, CheckPointType>> Positions { get; } = new LinkedList<PreSolution<PositionData, ActionData, CheckPointType>>();
            public CategoryEvaluation Evaluation
            {
                get; set;
            }
        }
    }

    public enum CancelCriteria
    {
        TargetsReached,
        MaxTime,
        MaxSteps
    }

    public abstract class CategoryEvaluator<PositionData, ActionData, CheckpointData>
       where PositionData : BasePositionData<PositionData, ActionData, CheckpointData>, Cloneable<PositionData>, IHasFingerprint
       //where ActionData : BaseActionData<PositionData, ActionData, CheckPointType>
       where CheckpointData : CheckPoint<PositionData>
    {
        public void Reset()
        {
            positionsFound = new LinkedList<GWPosition<PositionData, ActionData>>();
            currentBestScore = double.MinValue;
        }
        private LinkedList<GWPosition<PositionData, ActionData>> positionsFound { get; set; }

        private double currentBestScore = double.MinValue;
        public void AddPositionFound(GWPosition<PositionData, ActionData> position)
        {
            positionsFound.Add(position);
            currentBestScore = Math.Max(currentBestScore, Evaluate(position).EstimatedScore);
        }

        public CategoryEvaluation Evaluate(GWPosition<PositionData, ActionData> position)
        {
            var evaluation = EvaluateInternal(position);
            if (evaluation.BestPossibleScore <= currentBestScore)
                evaluation.CanImproveBestResult = false;
            return evaluation;
        }

        protected abstract CategoryEvaluation EvaluateInternal(GWPosition<PositionData, ActionData> position);
    }
    public sealed class CategoryEvaluation
    {
        public CategoryEvaluation(double estimatedScore, double bestPossibleScore, bool canImproveBestResult = true)
        {
            EstimatedScore = estimatedScore;
            BestPossibleScore = bestPossibleScore;
            CanImproveBestResult = canImproveBestResult;
        }
        public double EstimatedScore { get; set; }
        public double BestPossibleScore { get; set; }
        public bool CanImproveBestResult { get; set; }
    }


    public abstract class CheckPoint<Position>
    {
        public abstract bool Check(Position position);
        public abstract double Score(Position position);
    }

    public class PreSolution<PositionData, ActionData, CheckpointData>
       where PositionData : BasePositionData<PositionData, ActionData, CheckpointData>, Cloneable<PositionData>, IHasFingerprint
       where ActionData : BaseActionData<PositionData, ActionData, CheckpointData>
       where CheckpointData : CheckPoint<PositionData>
    {
        public PreSolution(GWPosition<PositionData, ActionData> position, LinkedList<RunItem<PositionData, ActionData, CheckpointData>> runItemPath)
        {
            Position = position;
            if (runItemPath.NotNullOrEmpty())
                RunItemPath.AddRange(runItemPath);
        }
        public PreSolution(GWPosition<PositionData, ActionData> position)
        {
            Position = position;
        }
        public double Score { get; set; }
        public GWPosition<PositionData, ActionData> Position { get; set; }
        public LinkedList<RunItem<PositionData, ActionData, CheckpointData>> RunItemPath { get; } = new LinkedList<RunItem<PositionData, ActionData, CheckpointData>>();
        internal bool InternalCanBeFiltered { get; set; }
    }

    public static class PreSolX
    {
        public static void AssignPreSolution<PositionData, ActionData, CheckpointData>(this RunItem<PositionData, ActionData, CheckpointData> champ, PreSolution<PositionData, ActionData, CheckpointData> solution)
        where PositionData : BasePositionData<PositionData, ActionData, CheckpointData>, Cloneable<PositionData>, IHasFingerprint
       where ActionData : BaseActionData<PositionData, ActionData, CheckpointData>
        where CheckpointData : CheckPoint<PositionData>
        {
            solution.RunItemPath.Add(champ);
            champ.PreSolutions.Add(solution);
        }
    }

    public class TargetOptCenterParams
    {
        public TargetOptCenterParams()
        {

        }
        public TargetOptCenterParams(int runnerPoolSize, int nextGenSize, double poolPopRefreshRatio, int maxSearchDepth, int runItemSearchDepth, int multiEvalOptions, bool useNeuralNetwork)
        {
            RunnerPoolSize = runnerPoolSize;
            NextGenerationSize = nextGenSize;
            PoolPopRefreshRatio = poolPopRefreshRatio;
            MaxSearchDepth = maxSearchDepth;
            RunItemSearchDepth = runItemSearchDepth;
            MultiEvaluationOptions = multiEvalOptions;
            UseNeuralNetwork = useNeuralNetwork;
        }
        //the number of threads followed each iteration.
        public int RunnerPoolSize { get; set; }
        //the max number of positions being further researched for each iteration. 
        public int NextGenerationSize { get; set; }
        //this one is important in combination with cancelcriteria.maxSearchDepth. It is intended for cases with no natural cancel criteria.
        public int MaxSearchDepth { get; set; }
        //same thinking as the above case. This time the limitation concerns the runItems.
        public int RunItemSearchDepth { get; set; } = 3;
        //the ratio of runItems being substituted after one run
        public double PoolPopRefreshRatio { get; set; }
        //number of options that each evaluator gives score > 0
        public int MultiEvaluationOptions { get; set; }
        //self explanatory
        public bool UseNeuralNetwork { get; set; }
    }

    public class DefaultTargetOptCenterParams : TargetOptCenterParams
    {
        public DefaultTargetOptCenterParams()
        {
            RunnerPoolSize = 300;
            NextGenerationSize = 180;
            PoolPopRefreshRatio = 0.1;
            MaxSearchDepth = 50;
            RunItemSearchDepth = 20;
            MultiEvaluationOptions = GlobalParameters.DefaultMultiEvaluationOptions;
            UseNeuralNetwork = false;
        }
    }
}
