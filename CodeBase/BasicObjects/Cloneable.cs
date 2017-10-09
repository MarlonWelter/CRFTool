using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public interface BCloneable<T> : IGWObject
    {
        T Clone(Dictionary<Guid, IGWObject> buffer);
    }
    public interface Cloneable<T> 
    {
        T Clone();
    }

    //public static class CloneX
    //{
    //    public static void 
    //}
}
