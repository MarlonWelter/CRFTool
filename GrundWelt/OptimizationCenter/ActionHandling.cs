using CodeBase;
using GrundWelt;
using GrundWelt.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public class ActionHandling<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public ActionHandling(GWActionExecuter<PositionData, ActionData> actionExecuter, IEnumerable<GWActionFinder<PositionData, ActionData>> actionFinders, IEnumerable<GWActionEvaluator<PositionData, ActionData>> actionEvaluators, GWActionCategorizer<PositionData, ActionData> actionCategorizer)
        {
            ActionFinders = actionFinders;
            ActionEvaluators = actionEvaluators.ToArray();
            ActionExecuter = actionExecuter;

            ActionCategorizer = actionCategorizer;
        }
        public readonly GWActionExecuter<PositionData, ActionData> ActionExecuter;
        public readonly IEnumerable<GWActionFinder<PositionData, ActionData>> ActionFinders;
        public readonly GWActionEvaluator<PositionData, ActionData>[] ActionEvaluators;
        public readonly GWActionCategorizer<PositionData, ActionData> ActionCategorizer;
    }

    class ActionEvaluationNeuralNetwork<PositionData, ActionData> : GWNeuralNetwork<GWAction<PositionData, ActionData>>
        where PositionData : Cloneable<PositionData>
    {
        public ActionEvaluationNeuralNetwork(List<ActionEvaluationNeuralNetworkInputNode<PositionData, ActionData>> inputNodes, int[] innerLayers) : base(inputNodes.Cast<NeuralNetworkInputNode<GWAction<PositionData, ActionData>>>().ToList(), innerLayers)
        {

        }
    }

    class ActionEvaluationNeuralNetworkInputNode<PositionData, ActionData> : NeuralNetworkInputNode<GWAction<PositionData, ActionData>>
        where PositionData : Cloneable<PositionData>
    {
        public ActionEvaluationNeuralNetworkInputNode(GWActionEvaluator<PositionData, ActionData> evaluator)
        {
            Evaluator = evaluator;
        }
        public GWActionEvaluator<PositionData, ActionData> Evaluator { get; set; }
        public override void Input(GWAction<PositionData, ActionData> data)
        {
            Value = Evaluator.Evaluate(data);
        }
    }
}
