using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class BasePositionData<PositionData, ActionData, CheckPointType>
        where PositionData : BasePositionData<PositionData, ActionData, CheckPointType>
        //where ActionData : BaseActionData<PositionData, ActionData, CheckPointType>
    {
        public CheckPointType NextCheckPoint { get; set; }
        public LinkedList<int> ActionsTried { get; set; }
    }

    public abstract class BaseActionData<PositionData, ActionData, CheckPointType>
      where PositionData : BasePositionData<PositionData, ActionData, CheckPointType>
        where ActionData : BaseActionData<PositionData, ActionData, CheckPointType>
    {
        //public bool HasImpactOnTarget { get; set; }
        public int FingerPrint { get; set; }
    }

}
