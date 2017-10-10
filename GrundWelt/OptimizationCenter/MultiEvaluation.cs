using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace GrundWelt
{
    class MultiEvaluation
    {
        public static IEnumerable<GWAction<PositionData, ActionData>> Do<PositionData, ActionData>(IEnumerable<GWAction<PositionData, ActionData>> options, GWActionEvaluator<PositionData, ActionData>[] actionEvaluators, double[] weights, int returnCount, int EachEvaluatorOptions = GlobalParameters.DefaultMultiEvaluationOptions)
        where PositionData : Cloneable<PositionData>
        {
            int tempNr = 0;
            foreach (var option in options)
            {
                option.TempNumber = tempNr;
                tempNr++;
            }
            //evaluateOptions
            var optionsScore = new double[options.Count()];
            for (int i = 0; i < actionEvaluators.Length; i++)
            {
                var actionEvaluator = actionEvaluators[i];
                var bestOptions = new GWLinkedList<GWAction<PositionData, ActionData>>(EachEvaluatorOptions, 0.3);
                //var bestOptions = new LinkedList<GWAction<PositionData, ActionData>>();
                foreach (var option in options)
                {
                    option.Score = actionEvaluator.Evaluate(option);
                    if (option.Score == 0)
                        option.Score = Program.Random.NextDouble() * 0.0001;
                    bestOptions.SortedInsert(option);
                    //SortedInsertLocal(bestOptions, option, EachEvaluatorOptions);
                }
                var bestOptionsList = bestOptions.ToList();
                for (int k = 0; k < bestOptionsList.Count; k++)
                {
                    optionsScore[(bestOptionsList[k]).TempNumber] += (EachEvaluatorOptions - k) * weights[i];
                }
                if (weights[i] < 0)
                { }
            }
            foreach (var option in options)
            {
                option.Score = optionsScore[option.TempNumber];
            }
            return (options.MaxEntries(o => o.Score, returnCount));
        }

        public static S SortedInsertLocal<S>(LinkedList<S> List, S Data, int maxSize) where S : IScoreHolder
        {
            if (List == null)
                return default(S);
            if (List.Count == 0)
            {
                List.AddFirst(Data);
                if (List.Count > maxSize)
                {
                    var item = List.Last.Value;
                    List.RemoveLast();
                    return item;
                }
                return default(S);
            }
            LinkedListNode<S> node = List.Last;
            while (Data.Score > (node.Value).Score)
            {
                if (node.Previous != null)
                    node = node.Previous;
                else
                {
                    List.AddBefore(node, Data);
                    if (List.Count > maxSize)
                    {
                        var item = List.Last.Value;
                        List.RemoveLast();
                        return item;
                    }
                    return default(S);
                }
            }
            List.AddAfter(node, Data);
            if (List.Count > maxSize)
            {
                var item = List.Last.Value;
                List.RemoveLast();
                return item;
            }
            return default(S);
        }

        struct Eval
        {
            public Eval(object action, double score)
            {
                Action = action;
                Score = score;
            }
            public object Action { get; set; }
            public double Score { get; set; }
        }
    }
}
