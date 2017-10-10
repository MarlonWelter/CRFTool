using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class MandatoryActionExecuter<PositionData, ActionData, CheckPointType>
           where PositionData : BasePositionData<PositionData, ActionData, CheckPointType>, Cloneable<PositionData>, IHasFingerprint
        where ActionData : BaseActionData<PositionData, ActionData, CheckPointType>
        where CheckPointType : CheckPoint<PositionData>
    {
        protected abstract IEnumerable<GWAction<PositionData, ActionData>> FindMandatoryActions(GWPosition<PositionData, ActionData> position);

        protected abstract PositionData ExecuteAction(GWAction<PositionData, ActionData> position);

        internal IEnumerable<GWPosition<PositionData, ActionData>> ExecuteMandatoryActions(GWPosition<PositionData, ActionData> position)
        {
            var mandatoryActions = FindMandatoryActions(position);

            if (mandatoryActions.NullOrEmpty())
            {
                yield return position;
            }
            else
            {
                foreach (var action in mandatoryActions)
                {
                    var clonePos = new GWPosition<PositionData, ActionData>(position.Data.Clone(), position.Depth, position.CategoryPath);
                    action.Position = clonePos;
                    ExecuteAction(action);
                    yield return clonePos;
                }
            }
        }
    }
}
