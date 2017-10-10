//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CodeBase;

//namespace GrundWelt
//{
//    class GWRunner<PositionData, ActionData>
//        where PositionData : Cloneable<PositionData>
//    {
//        public GWRunner(double moveCost, double mateValue, double energy, GWActionExecuter<PositionData, ActionData> actionExecuter, GWActionEvaluator<PositionData, ActionData>[] actionSensors, GWActionFinder<PositionData, ActionData> actionFinder, IEnumerable<GWProperty> properties)
//        {
//            MoveCost = moveCost;
//            MateValue = mateValue;
//            Energy = energy;
//            ActionExecuter = actionExecuter;
//            ActionEvaluators = actionSensors;
//            ActionFinder = actionFinder;
//            ActionProperties = properties.ToArray();
//        }
//        public readonly static Random RunnerRandom = new Random();

//        public readonly double MoveCost;
//        public readonly double MateValue;
//        public double Energy { get; set; }
//        public GWPosition<PositionData, ActionData> CurrentPosition { get; set; }

//        public readonly GWActionExecuter<PositionData, ActionData> ActionExecuter;
//        private readonly GWActionFinder<PositionData, ActionData> ActionFinder;
//        private readonly GWActionEvaluator<PositionData, ActionData>[] ActionEvaluators;
//        private GWProperty[] ActionProperties;
//        public bool OpenForMating { get; internal set; }
//        private void Move()
//        {
//            var moveCost = RunnerRandom.NextDouble() * MoveCost;
//            Energy -= MoveCost;
//            if (Energy < 0)
//            {
//                return;
//            }

//            var options = new List<GWAction<PositionData, ActionData>>();

//            var foundOptions = ActionFinder.FindActions(CurrentPosition);
//            options.AddRange(foundOptions.Select(op => new GWAction<PositionData, ActionData>(CurrentPosition, op)));

//            if (options.NullOrEmpty())
//            {
//                //discovery of ending position
//                EndPositionDiscovered(CurrentPosition);
//            }
//            else
//            {
//                var bestOption = default(GWAction<PositionData, ActionData>);
//                foreach (var option in options)
//                {
//                    double[] charackterization = Charackterize(option);
//                    foreach (var property in ActionProperties)
//                    {
//                        option.Score += property.Score(charackterization);
//                    }
//                    if (bestOption == null || bestOption.Score <= option.Score)
//                        bestOption = option;
//                }

//                bestOption.Position = CurrentPosition;
//                bestOption.Position.Path.Add(bestOption);
//                ActionExecuter.ExecuteAction(bestOption);
//                CurrentPosition.Depth += 1;
//                //signalNewPosition.Stimulate(CurrentPosition);
//            }
//        }

//        private double[] Charackterize(GWAction<PositionData, ActionData> option)
//        {
//            var charackterization = new double[ActionEvaluators.Length];
//            for (int i = 0; i < ActionEvaluators.Length; i++)
//            {
//                charackterization[i] = ActionEvaluators[i].Evaluate(option);
//            }
//            return charackterization;
//        }

//        private void EndPositionDiscovered(GWPosition<PositionData, ActionData> CurrentPosition)
//        {
//            //signalEndPosition.Stimulate();
//        }

//        public void Feed(double food)
//        {
//            Energy += food;
//        }

//        public void Act()
//        {
//            Move();

//            if (RunnerRandom.NextDouble() * MateValue <= Energy)
//            {
//                OpenForMating = true;
//            }
//        }

//        public MatingInfo Mate()
//        {
//            var childPower = Energy / 2;
//            Energy = Energy / 2;
//            OpenForMating = false;
//            return new MatingInfo(ActionProperties.ToArray(), childPower);
//        }
//    }

//    class MatingInfo
//    {
//        public MatingInfo(GWProperty[] properties, double energy)
//        {
//            Properties = properties;
//            Energy = energy;
//        }
//        public readonly GWProperty[] Properties;
//        public readonly double Energy;
//    }

//    public abstract class GWProperty : Cloneable<GWProperty>
//    {
//        public GWProperty(double[] baseProps)
//        {
//            BaseProperties = baseProps.ToArray();
//        }
//        protected readonly double[] BaseProperties;
//        public double Weight { get; set; }

//        public abstract double Score(double[] baseproperties);

//        protected abstract GWProperty ProtClone();
//        public GWProperty Clone()
//        {
//            return ProtClone();
//        }
//    }


//    class Donator<PositionData, ActionData>
//        where PositionData : Cloneable<PositionData>
//    {
//        public double Food { get; set; }
//        public int DonatorID { get; set; }
//        private double ValueSum = 0.0;
//        public GWProperty Property { get; set; }

//        private LinkedList<RunnerTicket<PositionData, ActionData>> items = new LinkedList<RunnerTicket<PositionData, ActionData>>();

//        public void AddItem(AgO<GWRunner<PositionData, ActionData>, double[]> item)
//        {
//            var value = Property.Score(item.Data2);
//            ValueSum += value;
//            items.SortedInsert(new RunnerTicket<PositionData, ActionData>(item.Data1, item.Data2, value), (r) => r.Value);
//        }

//        public void Feed()
//        {
//            var factor = Food / ValueSum;
//            foreach (var item in items)
//            {
//                item.Runner.Feed(item.Value * factor);
//            }
//        }
//    }

//    class RunnerTicket<PositionData, ActionData>
//        where PositionData : Cloneable<PositionData>
//    {
//        public RunnerTicket(GWRunner<PositionData, ActionData> runner, double[] charackteristic, double value)
//        {
//            Runner = runner;
//            Charackteristic = charackteristic;
//            Value = value;
//        }
//        public readonly GWRunner<PositionData, ActionData> Runner;
//        public readonly double[] Charackteristic;
//        public readonly double Value;
//    }
//}
