
using System;

namespace CodeBase
{

    public interface IQuboidLogic
    {
        double Width { get; set; }
        double Length { get; set; }
        double Height { get; set; }
    }
    public class QuboidLogic : IQuboidLogic
    {
        public QuboidLogic()
        {

        }
        public QuboidLogic(double length, double width, double height)
        {
            Length = length;
            Width = width;
            Height = height;
        }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
    }

    public static class QuboidX
    {
        public static double Space(this IQuboidLogic logic)
        {
            return logic.Length * logic.Width * logic.Height;
        }
        public static double Width(this IHas<IQuboidLogic> logicHolder)
        {
            return logicHolder.Logic.Width;
        }

        public static double Length(this IHas<IQuboidLogic> logicHolder)
        {
            return logicHolder.Logic.Length;
        }

        public static double Height(this IHas<IQuboidLogic> logicHolder)
        {
            return logicHolder.Logic.Height;
        }


        public static double GetLength(this IHas<IQuboidLogic> logicHolder, int rotation)
        {
            var Logic = logicHolder.Logic;
            switch (rotation)
            {
                case 0:
                    return Logic.Length;

                case 1:
                    return Logic.Width;

                case 2:
                    return Logic.Height;

                case 3:
                    return Logic.Width;
                case 4:
                    return Logic.Length;
                case 5:
                    return Logic.Height;
                default:
                    return -1;
            }
        }
        public static double GetWidth(this IHas<IQuboidLogic> logicHolder, int rotation)
        {
            var Logic = logicHolder.Logic;
            switch (rotation)
            {
                case 0:
                    return Logic.Width;

                case 1:
                    return Logic.Length;

                case 2:
                    return Logic.Width;

                case 3:
                    return Logic.Height;
                case 4:
                    return Logic.Height;
                case 5:
                    return Logic.Length;
                default:
                    return -1;
            }
        }
        public static double GetHeight(this IHas<IQuboidLogic> logicHolder, int rotation)
        {
            var Logic = logicHolder.Logic;
            switch (rotation)
            {
                case 0:
                    return Logic.Height;

                case 1:
                    return Logic.Height;

                case 2:
                    return Logic.Length;

                case 3:
                    return Logic.Length;
                case 4:
                    return Logic.Width;
                case 5:
                    return Logic.Width;
                default:
                    return -1;
            }
        }
        public static double GetLength(this IQuboidLogic logicHolder, int rotation)
        {
            var Logic = logicHolder;
            switch (rotation)
            {
                case 0:
                    return Logic.Length;

                case 1:
                    return Logic.Width;

                case 2:
                    return Logic.Height;

                case 3:
                    return Logic.Width;
                case 4:
                    return Logic.Length;
                case 5:
                    return Logic.Height;
                default:
                    return -1;
            }
        }
        public static double GetWidth(this IQuboidLogic logicHolder, int rotation)
        {
            var Logic = logicHolder;
            switch (rotation)
            {
                case 0:
                    return Logic.Width;

                case 1:
                    return Logic.Length;

                case 2:
                    return Logic.Width;

                case 3:
                    return Logic.Height;
                case 4:
                    return Logic.Height;
                case 5:
                    return Logic.Length;
                default:
                    return -1;
            }
        }
        public static double GetHeight(this IQuboidLogic logicHolder, int rotation)
        {
            var Logic = logicHolder;
            switch (rotation)
            {
                case 0:
                    return Logic.Height;

                case 1:
                    return Logic.Height;

                case 2:
                    return Logic.Length;

                case 3:
                    return Logic.Length;
                case 4:
                    return Logic.Width;
                case 5:
                    return Logic.Width;
                default:
                    return -1;
            }
        }

        public static void Width(this IHas<IQuboidLogic> logicHolder, double value)
        {
            logicHolder.Logic.Width = value;
        }

        public static void Length(this IHas<IQuboidLogic> logicHolder, double value)
        {
            logicHolder.Logic.Length = value;
        }

        public static void Height(this IHas<IQuboidLogic> logicHolder, double value)
        {
            logicHolder.Logic.Height = value;
        }
        internal static void Rotate(this IQuboidLogic quboid, int i)
        {
            switch (i)
            {
                case 0:
                    break;

                case 1:
                    RotateAxisVertical(quboid);
                    break;

                case 2:
                    RotateAxisHorizontal(quboid);
                    break;

                case 3:
                    RotateAxisHorizontal(quboid);
                    RotateAxisVertical(quboid);
                    break;
                case 4:
                    RotateAxisParallel(quboid);
                    break;
                case 5:
                    RotateAxisParallel(quboid);
                    RotateAxisVertical(quboid);
                    break;
                default:
                    break;

            }
        }
        public static void Rotate(this IHas<IQuboidLogic> package, int i)
        {
            Rotate(package.Logic, i);
        }

        public static void RotateAxisHorizontal(this IQuboidLogic type)
        {
            double help = type.Length;
            type.Length = (type.Height);
            type.Height = (help);
        }
        public static void RotateAxisVertical(this IQuboidLogic type)
        {
            double help = type.Width;
            type.Width = (type.Length);
            type.Length = (help);
        }
        public static void RotateAxisParallel(this IQuboidLogic type)
        {
            double help = type.Width;
            type.Width = (type.Height);
            type.Height = (help);
        }

        /* •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
           | public static void ReworkRotation(IQuboid package)                                                                       |
           | {                                                                                                                        |
           |     //p.Rotation[0] = true;                                                                                              |
           |     package.Rotation[1] = !(package.Height == package.Width) && package.Rotation[1];                                     |
           |     package.Rotation[2] = !(package.Width == package.Length) && package.Rotation[2];                                     |
           |     package.Rotation[3] = !(package.Height == package.Length && package.Width == package.Length) && package.Rotation[3]; |
           |     package.Rotation[4] = !(package.Height == package.Width && package.Width == package.Length) && package.Rotation[4];  |
           |     package.Rotation[5] = !(package.Height == package.Length) && package.Rotation[5];                                    |
           | }                                                                                                                        |
           •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————• */


        // Diese Funktion kann ein Packet in sechs Richtungen drehen.
        public static void InvertRotate(this IHas<IQuboidLogic> p, int i)
        {
            InvertRotate(p.Logic, i);
        }
        public static void InvertRotate(this IQuboidLogic p, int i)
        {
            switch (i)
            {
                case 0:
                    break;
                case 1:
                    Rotate(p, 1);
                    break;
                case 2:
                    Rotate(p, 2);
                    break;
                case 3:
                    Rotate(p, 5);
                    break;
                case 4:
                    Rotate(p, 4);
                    break;
                case 5:
                    Rotate(p, 3);
                    break;
                default:
                    break;

            }
        }
        public static double MinSideValue(this IQuboidLogic logicHolder)
        {
            return Math.Min(logicHolder.Length, Math.Min(logicHolder.Width, logicHolder.Height));
        }
        public static double MaxSideValue(this IQuboidLogic logicHolder)
        {
            return Math.Max(logicHolder.Length, Math.Max(logicHolder.Width, logicHolder.Height));
        }
    }
}
