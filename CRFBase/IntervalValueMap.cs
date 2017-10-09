using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    class IntervalValueMap<NodeData, EdgeData, GraphData>
    {
        public double[] values;
        public List<Divisor<NodeData, EdgeData, GraphData>> divisors = new List<Divisor<NodeData, EdgeData, GraphData>>();
        public LinkedList<DivisorChange> lastChanges = new LinkedList<DivisorChange>();

        public void ApplyChange(DivisorChange change)
        {
            var divisor = divisors[change.Divisor];
            divisor.ChangePosition(change.Direction);
            lastChanges.AddLast(change);
        }
        public void UndoChange(DivisorChange change)
        {
            var divisor = divisors[change.Divisor];
            divisor.ChangePosition(change.Direction == Direction.Left ? Direction.Right : Direction.Left);
        }
        public void ApplyChanges(IEnumerable<DivisorChange> changes)
        {

        }
        public void UndoLastChanges()
        {
            foreach (var change in lastChanges)
            {
                UndoChange(change);
            }
            lastChanges.Clear();
        }
        public void Init()
        {
            values = new double[divisors.Count + 1];
            for (int i = 0; i < divisors.Count; i++)
            {
                var divisor = divisors[i];
                if (i == 0)
                    divisor.MinIndex = 0;
                else
                {
                    divisor.LeftNb = divisors[i - 1];
                    divisors[i - 1].RightNb = divisor;
                    divisor.MinIndex = divisors[i - 1].Index + 1;
                    divisors[i - 1].MaxIndex = divisor.Index - 1; ;
                }
                if (i == divisors.Count - 1)
                    divisor.MaxIndex = values.Length - 1;
            }
        }
    }


    class Divisor<NodeData, EdgeData, GraphData>
    {
        public Divisor(double value, BasisMerkmal<NodeData, EdgeData, GraphData> upper, BasisMerkmal<NodeData, EdgeData, GraphData> lower)
        {
            UpperBoundary = upper;
            LowerBoundary = lower;
            Value = value;
        }
        public BasisMerkmal<NodeData, EdgeData, GraphData> UpperBoundary { get; set; }
        public BasisMerkmal<NodeData, EdgeData, GraphData> LowerBoundary { get; set; }

        public double Value { get; set; }

        public int Index { get; set; }

        public int MinIndex { get; set; }
        public int MaxIndex { get; set; }

        public double[] Values { get; set; }

        public Divisor<NodeData, EdgeData, GraphData> LeftNb { get; set; }
        public Divisor<NodeData, EdgeData, GraphData> RightNb { get; set; }



        public void ChangePosition(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    if (Index > MinIndex)
                    {
                        Index--;
                        Value = (Values[Index] + Values[Index + 1]) / 2;
                        //foreach (var item in UpperBoundary)
                        {
                            UpperBoundary.UpperBoundary = Value;
                        }
                        //foreach (var item in LowerBoundary)
                        {
                            LowerBoundary.LowerBoundary = Value;
                        }
                        if (LeftNb != null)
                            LeftNb.MaxIndex--;
                        if (RightNb != null)
                            RightNb.MinIndex--;
                    }
                    break;
                case Direction.Right:
                    if (Index < MaxIndex - 1)
                    {
                        Index++;
                        Value = (Values[Index] + Values[Index + 1]) / 2;
                        {
                            UpperBoundary.UpperBoundary = Value;
                        }
                        {
                            LowerBoundary.LowerBoundary = Value;
                        }
                        if (LeftNb != null)
                            LeftNb.MaxIndex++;
                        if (RightNb != null)
                            RightNb.MinIndex++;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    class DivisorChange
    {

        private static Random Random = new Random();

        public DivisorChange(CodeBase.Direction direction, int divisor)
        {
            this.Direction = direction;
            this.Divisor = divisor;
        }

        public static DivisorChange RandomInstance(int divisors)
        {
            return new DivisorChange(Random.Next() == 0 ? Direction.Left : Direction.Right, Random.Next(divisors));
        }

        public static IEnumerable<DivisorChange> RandomInstances(int divisors)
        {
            do
            {
                yield return new DivisorChange(Random.Next() == 0 ? Direction.Left : Direction.Right, Random.Next(divisors));
            } while (Random.Next() == 0);
        }

        public Direction Direction { get; set; }
        public int Divisor { get; set; }


    }
}
