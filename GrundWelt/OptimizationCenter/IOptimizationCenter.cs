using CodeBase;
using System;
using System.Collections.Generic;

namespace GrundWelt
{
    public interface IOptimizationCenter<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        void Start(GWInputData<PositionData> input);
        IComputationInfo ComputationInfo { get; set; }
        LinkedList<SpeedChangeMessage> Inbox { get; }
        MEvent<GWPosition<PositionData, ActionData>> NewBestResult { get; }
        MEvent RunFinished { get; }
        MEvent RunStarted { get; }
        GWPositionEvaluator<PositionData, ActionData> EndPositionEvaluator { get; set; }
    }

    public interface ITargetOptCenter<PositionData, ActionData, CheckPointType> : IOptimizationCenter<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
    }

    public static class OptimizationCenterX
    {
        //public static IOptimizationCenter<PositionData, ActionData> PhalanxCenter<PositionData, ActionData>(int phalanxSize, int resultsSize, double[] feedBack, GWActionExecuter<PositionData, ActionData> actionExecuter, IEnumerable<GWActionFinder<PositionData, ActionData>> actionFinders, IEnumerable<GWActionEvaluator<PositionData, ActionData>> actionEvaluators, IEnumerable<GWPositionEvaluator<PositionData, ActionData>> endPositionEvaluators)
        //     where PositionData : Cloneable<PositionData>
        //{
        //    var phalanxCenter = new GWPhalanxOptimizationCenter<PositionData, ActionData>(phalanxSize, resultsSize, feedBack, actionExecuter, actionFinders, actionEvaluators, endPositionEvaluators);

        //    return phalanxCenter;
        //}
        public static ITargetOptCenter<PositionData, ActionData, CheckPointType> TargetOptimizationCenter<PositionData, ActionData, CheckPointType>(GWActionExecuter<PositionData, ActionData> actionExecuter, IEnumerable<GWActionFinder<PositionData, ActionData>> actionFinders, IEnumerable<GWActionEvaluator<PositionData, ActionData>> actionEvaluators, GWPositionEvaluator<PositionData, ActionData> endPositionEvaluator, GWProgressEstimator<PositionData> progressEstimator, GWPositionCategorizer<PositionData, ActionData> positionCategorizer, GWActionCategorizer<PositionData, ActionData> actionCategorizer, CategoryEvaluator<PositionData, ActionData, CheckPointType> categoryEvaluationLogic, CancelCriteria cancelCriteria, string id, TargetOptCenterParams optCenterParams, PositionGuide<PositionData> areaGuide, MandatoryActionExecuter<PositionData, ActionData, CheckPointType> mandatoryActionExecuter, RunnerPool<PositionData, ActionData, CheckPointType> runnerPool)
            where PositionData : BasePositionData<PositionData, ActionData, CheckPointType>, Cloneable<PositionData>, IHasFingerprint
           where ActionData : BaseActionData<PositionData, ActionData, CheckPointType>
            where CheckPointType : CheckPoint<PositionData>
        {
            var optCenter = new TargetOptimizationCenter<PositionData, ActionData, CheckPointType>(actionExecuter, actionFinders, actionEvaluators, endPositionEvaluator, progressEstimator, positionCategorizer, actionCategorizer, categoryEvaluationLogic, cancelCriteria, id, optCenterParams, areaGuide, mandatoryActionExecuter, runnerPool);

            return optCenter;
        }
    }
}