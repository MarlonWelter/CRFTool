using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class HessePlane
    {
        public HessePlane(ICoordinated a, ICoordinated b, ICoordinated c)
        {
            NormalVector = (b.Minus(a)).CrossProduct(c.Minus(a));
            NormalVector.Normalize();

            RootDistance = a.ScalarProduct(NormalVector);
            if(RootDistance < 0)
            {
                RootDistance *= -1;
                NormalVector.X *= -1;
                NormalVector.Y *= -1;
                NormalVector.Z *= -1;
            }
        }
        public Vector NormalVector { get; set; }
        public double RootDistance { get; set; }
    }
}
