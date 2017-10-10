using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class GWBranchingPointsControlUnit<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public abstract void AddBranchingPoint(GWPosition<PositionData, ActionData> position);
        public abstract void RemoveBranchingPoint(GWPosition<PositionData, ActionData> position);

        public abstract GWPosition<PositionData, ActionData> GetNextBranchingPoint();
    }

    public class RandomBranchingPointsControlUnit<PositionData, ActionData> : GWBranchingPointsControlUnit<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public RandomBranchingPointsControlUnit()
        {
            BufferSize = DefaultBufferSize;
        }

        private LinkedList<AgO<GWPosition<PositionData, ActionData>, double>> branchingPoints = new LinkedList<AgO<GWPosition<PositionData, ActionData>, double>>();

        public static int DefaultBufferSize = 30;
        public int BufferSize { get; set; }

        public override void AddBranchingPoint(GWPosition<PositionData, ActionData> position)
        {
            var score = Program.Random.NextDouble();
            branchingPoints.SortedInsert(new AgO<GWPosition<PositionData, ActionData>, double>(position, score), (ago) => ago.Data2);
            if (branchingPoints.Count > BufferSize)
                branchingPoints.RemoveLast();
        }

        public override void RemoveBranchingPoint(GWPosition<PositionData, ActionData> position)
        {
            branchingPoints.RemoveWhere(ago => ago.Data1.Equals(position));
        }

        public int[] BranchesByDepth = new int[1000];

        public override GWPosition<PositionData, ActionData> GetNextBranchingPoint()
        {
            var nextBP = branchingPoints.FirstOrDefault();
            if (nextBP == null)
                return null;

            BranchesByDepth[nextBP.Data1.Depth]++;

            branchingPoints.RemoveFirst();

            nextBP.Data2 = Program.Random.NextDouble();
            branchingPoints.SortedInsert(nextBP, (ago) => ago.Data2);

            return nextBP.Data1;
        }
    }
}
