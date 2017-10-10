using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    class GWCrossroad<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {

        public GWCrossroad(GWPosition<PositionData, ActionData> position)
        {
            Position = position;
            PositionDataCopy = position.Data.Clone();
        }

        public void Try(int category)
        {
            OptionCategoriesTried.Add(category);
        }

        public LinkedList<int> OptionCategoriesTried { get; } = new LinkedList<int>();

        public GWPosition<PositionData, ActionData> Position { get; set; }
        public PositionData PositionDataCopy { get; set; }
    }
}
