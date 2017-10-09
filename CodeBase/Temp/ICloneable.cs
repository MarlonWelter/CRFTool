using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase.Temp
{
    interface ICloneable<CloneType>
    {
        CloneType Clone();
    }

    public class Package : ICloneable<Package>
    {

        public Package Clone()
        {
            throw new NotImplementedException();
        }
    }
}
