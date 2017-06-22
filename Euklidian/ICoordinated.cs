using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public interface ICoordinated
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }
    public interface IImpulse
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }

    public class Vector : ICoordinated
    {
        public static Vector Null { get; } = new Vector();
        public Vector()
        {

        }
        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector(Vector force)
        {
            X = force.X;
            Y = force.Y;
            Z = force.Z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public static class CooX
    {
        public static void Multiply(this ICoordinated valueTaker, double multiplier)
        {
            valueTaker.X *= multiplier;
            valueTaker.Y *= multiplier;
            valueTaker.Z *= multiplier;
        }
        public static void Move(this ICoordinated valueTaker, ICoordinated vector)
        {
            valueTaker.X += vector.X;
            valueTaker.Y += vector.Y;
            valueTaker.Z += vector.Z;
        }
        public static void SetValuesTo(this ICoordinated valueTaker, ICoordinated model)
        {
            valueTaker.X = model.X;
            valueTaker.Y = model.Y;
            valueTaker.Z = model.Z;
        }
        public static double Length(this ICoordinated from)
        {
            return Math.Sqrt(Math.Pow(from.X, 2.0) + Math.Pow(from.Y, 2.0) + Math.Pow(from.Z, 2.0));
        }
        public static double Distance(this ICoordinated from, ICoordinated to)
        {
            return Math.Sqrt(Math.Pow(from.X - to.X, 2.0) + Math.Pow(from.Y - to.Y, 2.0) + Math.Pow(from.Z - to.Z, 2.0));
        }
        public static Vector CrossProduct(this ICoordinated a, ICoordinated b)
        {
            var cp = new Vector();

            cp.X = a.Y * b.Z - a.Z * b.Y;
            cp.Y = a.Z * b.X - a.X * b.Z;
            cp.Z = a.X * b.Y - a.Y * b.X;

            return cp;
        }
        public static double ScalarProduct(this ICoordinated a, ICoordinated b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public static ICoordinated Normalize(this ICoordinated vector)
        {
            var length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            vector.X /= length;
            vector.Y /= length;
            vector.Z /= length;
            return vector;
        }
        public static Vector Plus(this ICoordinated a, ICoordinated b)
        {
            var vector = new Vector();

            vector.X = a.X + b.X;
            vector.Y = a.Y + b.Y;
            vector.Z = a.Z + b.Z;

            return vector;
        }
        public static Vector Minus(this ICoordinated a, ICoordinated b)
        {
            var vector = new Vector();

            vector.X = a.X - b.X;
            vector.Y = a.Y - b.Y;
            vector.Z = a.Z - b.Z;

            return vector;
        }
        public static void Add(this ICoordinated a, ICoordinated b)
        {
            a.X = a.X + b.X;
            a.Y = a.Y + b.Y;
            a.Z = a.Z + b.Z;
        }
        //public static Vector Subtr(this ICoordinated a, ICoordinated b)
        //{
        //    var vector = new Vector();

        //    vector.X = a.X - b.X;
        //    vector.Y = a.Y - b.Y;
        //    vector.Z = a.Z - b.Z;

        //    return vector;
        //}
        public static double DistanceValue(this ICoordinated vector, HessePlane plane)
        {
            return vector.ScalarProduct(plane.NormalVector) - plane.RootDistance;
        }

        public static double determineCurvingCase(this ICoordinated point, IEnumerable<ICoordinated> otherPoints, double flatnessPlane)
        {
            var isConvex = false;
            var isFlat = false;
            int left = 0;
            int right = 0;
            int Inplane = 0;
            var neighbours = otherPoints.ToList();

            for (int i1 = neighbours.Count - 1; i1 >= 1; i1--)
            {
                var nb1 = neighbours[i1];

                for (int i2 = i1 - 1; i2 >= 0; i2--)
                {
                    var nb2 = neighbours[i2];

                    var plane = new HessePlane(nb1, nb2, point);

                    for (int i4 = 0; i4 < neighbours.Count; i4++)
                    {
                        if (i4 == i1 || i4 == i2)
                            continue;
                        var otherNb = neighbours[i4];

                        var dist = otherNb.DistanceValue(plane);

                        if (Math.Abs(dist) < flatnessPlane)
                            Inplane++;
                        else
                        {
                            if (dist < 0)
                                left++;
                            else
                                right++;
                        }
                    }

                    if (Inplane == 0)
                    {
                        if (left == 0 || right == 0)
                            isConvex = true;
                    }
                    else
                    {
                        if (left == 0 || right == 0)
                            isFlat = true;
                    }

                    if (isConvex)
                        break;
                }
                if (isConvex)
                    break;
            }
            if (isConvex)
            {
                return 1;
            }
            else if (isFlat)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public static double AngleBetween(this ICoordinated point, ICoordinated a, ICoordinated b)
        {
            var distA = point.Distance(a);
            var distB = point.Distance(b);
            var DistC = a.Distance(b);
            var angleCos = (Math.Pow(distA, 2) + Math.Pow(distB, 2) - Math.Pow(DistC, 2)) / (2 * distA * distB);
            var angle = Math.Acos(angleCos);
            return angle;
        }

        public static Vector BalancePoint(this IEnumerable<ICoordinated> points)
        {
            var x = points.Sum(p => p.X);
            var y = points.Sum(p => p.Y);
            var z = points.Sum(p => p.Z);
            var count = points.Count();

            return new Vector(x / count, y / count, z / count);
        }
    }
}
