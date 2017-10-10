using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrundWelt
{
    public abstract class GrundWeltAction
    {
        public GrundWeltAction(bool noAction = false)
        {
            NoAction = noAction;
        }
        public bool NoAction { get; set; }
    }
}
