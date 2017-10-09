
using System;
using System.Collections.Generic;

namespace  CodeBase
{
    public static class QudoidExtent
    {
        public static void AddOffset2D(this IHas<IQuboidLogic> quboid, double length, double width, int direction, int side)
        {
            if (side == 0)
                quboid.AddOffset2D(length, 0, direction);
            else
                quboid.AddOffset2D(0, width, direction);
        }
        public static void AddOffset2D(this IHas<IQuboidLogic> quboid, double length, double width, int direction)
        {
            switch (direction)
            {
                case 0:
                    quboid.Length(quboid.Length() + length);
                    quboid.Width(quboid.Width() + width);
                    break;
                case 1:
                    quboid.Length(quboid.Length() - width);
                    quboid.Width(quboid.Width() + length);
                    break;
                case 2:
                    quboid.Length(quboid.Length() - length);
                    quboid.Width(quboid.Width() - width);
                    break;
                case 3:
                    quboid.Length(quboid.Length() + width);
                    quboid.Width(quboid.Width() - length);
                    break;
                default:
                    break;
            }
        }

        public static string DimensionToString(this IHas<IQuboidLogic> quboid)
        {
            return "(" + Math.Round(quboid.Length(), 2) + "x" + Math.Round(quboid.Width(), 2) + "x" + Math.Round(quboid.Height(), 2) + ")";
        }

        public static double Space(this IHas<IQuboidLogic> quboid)
        {
            if (quboid == null)
                return 0.0;
            return quboid.Length() * quboid.Width() * quboid.Height();
        }

        public static bool Contains(this IHas<IQuboidLogic> quboid, IHas<IQuboidLogic> other)
        {
            if (quboid.Length() >= other.Length()
                && quboid.Width() >= other.Width()
                && quboid.Height() >= other.Height())
                return true;
            return false;
        }
        public static double TotalSpace(this IEnumerable<IHas<IQuboidLogic>> quboids)
        {
            double totalSpace = 0;
            foreach (var item in quboids)
            {
                totalSpace += item.Space();
            }
            return totalSpace;
        }

        //public static void CloneQuboid(this IHas<IQuboidLogic> model, IHas<IQuboidLogic> clone)
        //{
        //    clone.Height( model.Height());
        //    clone.Length(model.Length());
        //    clone.Width ( model.Width());
        //}
        //public static void TakeValues(this IHas<IQuboidLogic> clone, IHas<IQuboidLogic> model)
        //{
        //    clone.Height( model.Height());
        //    clone.Length( model.Length());
        //    clone.Width( model.Width());
        //}
        //public static IQuboidPlace CloneQuboidPlace(this IQuboidPlace model, IQuboidPlace clone)
        //{
        //    clone.Back = model.Back;
        //    clone.Down = model.Down;
        //    clone.Length = model.Length;
        //    clone.Left = model.Left;
        //    clone.Width = model.Width;
        //    clone.Height = model.Height;
        //    return clone;
        //}

        //public static void StandardQuboidPlace(this IQuboidPlace quboid, IQuboid model)
        //{
        //    quboid.Height = model.Height;
        //    quboid.Width = model.Width;
        //    quboid.Length = model.Length;
        //    quboid.Left = 0;
        //    quboid.Down = 0;
        //    quboid.Back = 0;
        //}
        //public static IQuboidPlace StandardQuboidPlace(this IHas<IQuboidLogic> model)
        //{
        //    LWQuboidPlace quboidplace = new LWQuboidPlace();
        //    quboidplace.Height = model.Height();
        //    quboidplace.Width = model.Width();
        //    quboidplace.Left = 0;
        //    quboidplace.Length = model.Length();
        //    quboidplace.Down = 0;
        //    quboidplace.Back = 0;

        //    return quboidplace;
        //}
    }
}
