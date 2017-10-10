using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    abstract class OptUnit<PositionData, ActionData>
        where PositionData : Cloneable<PositionData>
    {
        public OptUnit()
        {
            ComputationInfo = new ComputationInfo();
        }
        public IComputationInfo ComputationInfo { get; set; }
        public GWInputData<PositionData> Input { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public MEvent RunFinished { get; } = new MEvent();
        public MEvent RunStarted { get; } = new MEvent();
    }
    
}
