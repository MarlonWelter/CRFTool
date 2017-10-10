using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public interface PositionGuide<PositionData>
    {
        bool CanOvercome(PositionData position, PositionData referencePosition);
        double Score(PositionData position);
        int PositionAreaId(PositionData position);
        bool IsEndPosition(PositionData position);
    }
}
