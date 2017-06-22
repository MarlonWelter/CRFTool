using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class QuboidPlace : QuboidLogic, IQuboidPlace
    {
        public QuboidPlace()
        {

        }
        public QuboidPlace(double left, double back, double down, double width, double length, double height)
        {
            Left = left;
            Back = back;
            Down = down;
            Width = width;
            Length = length;
            Height = height;
        }
        public QuboidPlace(IQuboidPlace model)
        {
            if (model != null)
            {
                Left = model.Left;
                Back = model.Back;
                Down = model.Down;
                Width = model.Width;
                Length = model.Length;
                Height = model.Height;
            }
        }
        public double Back { get; set; }
        public double Down { get; set; }
        public double Left { get; set; }
        public double Right => Left + Width;
        public double Up => Down + Height;
        public double Front => Back + Length;
        public double Space => Length * Width * Height;
    }
    public static class QuboidPlaceX
    {
        public static IQuboidPlace Default()
        {
            return new QuboidPlace();
        }
        public static bool Intersect2D(this IQuboidPlace quboidPlace, IQuboidPlace otherQuboidPlace)
        {
            if (
                (quboidPlace.Back >= otherQuboidPlace.Front) || (quboidPlace.Front <= otherQuboidPlace.Back) ||
                (quboidPlace.Left >= otherQuboidPlace.Right) || (quboidPlace.Right <= otherQuboidPlace.Left)
                )
                return false;

            return true;
        }
        public static bool Contains(this IQuboidPlace qplace, IQuboidLogic package, int rotation)
        {
            if (qplace.Length >= package.GetLength(rotation)
              && qplace.Width >= package.GetWidth(rotation)
              && qplace.Height >= package.GetHeight(rotation))
                return true;
            return false;
        }
        public static double Ground(this IQuboidLogic logicHolder, int rotation)
        {
            return logicHolder.GetLength(rotation) * logicHolder.GetWidth(rotation);
        }
        public static bool IsTouching(this IQuboidPlace root, IQuboidPlace otherPlace, Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return root.Left == otherPlace.Right && !(root.Down >= otherPlace.Up || root.Up <= otherPlace.Down || root.Front <= otherPlace.Back || root.Back >= otherPlace.Front);
                case Direction.Right:
                    return otherPlace.IsTouching(root, Direction.Left); ;
                case Direction.Back:
                    return root.Back == otherPlace.Front && !(root.Down >= otherPlace.Up || root.Up <= otherPlace.Down || root.Right <= otherPlace.Left || root.Left >= otherPlace.Right);
                case Direction.Front:
                    return otherPlace.IsTouching(root, Direction.Back);
                case Direction.Down:
                    return root.Down == otherPlace.Up && !(root.Left >= otherPlace.Right || root.Right <= otherPlace.Left || root.Front <= otherPlace.Back || root.Back >= otherPlace.Front);
                case Direction.Up:
                    return otherPlace.IsTouching(root, Direction.Down);
                default:
                    break;
            }
            return false;
        }
    }
    public interface IQuboidPlace : IQuboidLogic
    {
        double Back { get; set; }
        double Down { get; set; }
        double Left { get; set; }
        double Right { get; }
        double Up { get; }
        double Front { get; }
        double Space { get; }
    }
}
