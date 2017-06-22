
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeBase
{
    public interface IQuboidPlaceLogic : IPosition3D, IQuboidLogic
    {
    }
    public interface IPosition3D : ILogic
    {
        double Back { get; set; }
        double Down { get; set; }
        double Left { get; set; }
    }
    public class PositionLogic : IPosition3D
    {
        public PositionLogic(double left, double back, double down)
        {
            Left = left;
            Back = back;
            Down = down;
        }
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        public double Back { get; set; }
        public double Down { get; set; }
        public double Left { get; set; }
    }
    //public class QuboidPlaceLogic : IQuboidPlaceLogic
    //{
    //    private Guid mitId;
    //    public Guid GWId
    //    {
    //        get { return mitId; }
    //        set { mitId = value; }
    //    }
    //    public double Back { get; set; }
    //    public double Down { get; set; }
    //    public double Left { get; set; }
    //    public double Width { get; set; }
    //    public double Length { get; set; }
    //    public double Height { get; set; }

    //    public double Right
    //    {
    //        get { return Left + Width; }
    //    }
    //    public double Front
    //    {
    //        get { return Back + Length; }
    //    }
    //    public double Up
    //    {
    //        get { return Down + Height; }
    //    }
    //    public void Fill(double left, double back, double down, double width, double length, double height)
    //    {
    //        Left = left;
    //        Back = back;
    //        Down = down;
    //        Width = width;
    //        Length = length;
    //        Height = height;
    //    }
    //}
    public static class QPlaceX
    {
        public static double Left(this IHas<IPosition3D> logicHolder)
        {
            return logicHolder.Logic.Left;
        }
        public static void Left(this IHas<IPosition3D> logicHolder, double value)
        {
            logicHolder.Logic.Left = value;
        }

        public static double Down(this IHas<IPosition3D> logicHolder)
        {
            return logicHolder.Logic.Down;
        }
        public static void Down(this IHas<IPosition3D> logicHolder, double value)
        {
            logicHolder.Logic.Down = value;
        }

        public static double Back(this IHas<IPosition3D> logicHolder)
        {
            return logicHolder.Logic.Back;
        }
        public static void Back(this IHas<IPosition3D> logicHolder, double value)
        {
            logicHolder.Logic.Back = value;
        }
        public static double Right(this IHas<IQuboidPlaceLogic> qplace)
        {
            return qplace.Left() + qplace.Width();
        }
        public static double Front(this IHas<IQuboidPlaceLogic> qplace)
        {
            return qplace.Back() + qplace.Length();
        }
        public static double Up(this IHas<IQuboidPlaceLogic> qplace)
        {
            return qplace.Down() + qplace.Height();
        }
        public static void Move(this IHas<IPosition3D> quboidPlace, double x, double y, double z)
        {
            quboidPlace.Left(quboidPlace.Left() + x);
            quboidPlace.Back(quboidPlace.Back() + y);
            quboidPlace.Down(quboidPlace.Down() + z);
        }
        public static void Move(this IHas<IPosition3D> quboidPlace, IHas<IQuboidLogic> offset)
        {
            quboidPlace.Left(quboidPlace.Left() + offset.Width());
            quboidPlace.Back(quboidPlace.Back() + offset.Length());
            quboidPlace.Down(quboidPlace.Down() + offset.Height());
        }

        //public static void Rotate(this IHas<IQuboidPlaceLogic> place, int direction)
        //{
        //    var left = place.Left;
        //    var back = place.Back;
        //    var length = place.Length;
        //    switch (direction)
        //    {
        //        case 0:
        //            break;
        //        case 1:
        //            place.Back = -left - place.Width;
        //            place.Left = back;
        //            place.Length = place.Width;
        //            place.Width = length;
        //            break;
        //        case 2:
        //            place.Back = -back - place.Length;
        //            place.Left = -left - place.Width;
        //            break;
        //        case 3:
        //            place.Back = left;
        //            place.Left = -back - place.Length;
        //            place.Length = place.Width;
        //            place.Width = length;
        //            break;
        //        default:
        //            break;
        //    }
        //}
        public static void Enclose(this IHas<IQuboidPlaceLogic> quboidplace, IHas<IQuboidPlaceLogic> placeToEnclose)
        {
            double right = quboidplace.Right();
            double front = quboidplace.Front();
            double up = quboidplace.Up();

            quboidplace.Left(Math.Min(quboidplace.Left(), placeToEnclose.Left()));
            quboidplace.Back(Math.Min(quboidplace.Back(), placeToEnclose.Back()));
            quboidplace.Down(Math.Min(quboidplace.Down(), placeToEnclose.Down()));
            quboidplace.Width(Math.Max(right, placeToEnclose.Right()) - quboidplace.Left());
            quboidplace.Length(Math.Max(front, placeToEnclose.Front()) - quboidplace.Back());
            quboidplace.Height(Math.Max(up, placeToEnclose.Up()) - quboidplace.Down());
        }
        //public static LWQuboidPlace EncloseCopy(this IHas<IQuboidPlaceLogic> qplace, IHas<IQuboidPlaceLogic> placeToEnclose)
        //{
        //    LWQuboidPlace quboidplace = new LWQuboidPlace();
        //    double right = qplace.Right();
        //    double front = qplace.Front();
        //    double up = qplace.Up();

        //    quboidplace.Left = Math.Min(qplace.Left, placeToEnclose.Left);
        //    quboidplace.Back = Math.Min(qplace.Back, placeToEnclose.Back);
        //    quboidplace.Down = Math.Min(qplace.Down, placeToEnclose.Down);
        //    quboidplace.Width = Math.Max(right, placeToEnclose.Right()) - quboidplace.Left;
        //    quboidplace.Length = Math.Max(front, placeToEnclose.Front()) - quboidplace.Back;
        //    quboidplace.Height = Math.Max(up, placeToEnclose.Up()) - quboidplace.Down;

        //    return quboidplace;
        //}

        public static LWQuboidPlace MoveCopy(this IHas<IQuboidPlaceLogic> quboidPlace, double x, double y, double z)
        {
            LWQuboidPlace qplace = new LWQuboidPlace(quboidPlace);
            qplace.Move(x, y, z);
            return qplace;
        }
        //public static IHas<IQuboidLogic> ScaleCopy(this IHas<IQuboidLogic> quboid, double x, double y, double z)
        //{
        //    if (x < 0 || y < 0 || z < 0)
        //        throw new ArgumentException("Negative scaling not allowed.");

        //    var qplace = new LWQuboid();
        //    qplace.Width(quboid.Width() * x);
        //    qplace.Length(quboid.Length() * y);
        //    qplace.Height(quboid.Height() * z);

        //    return qplace;
        //}
        //public static LWQuboid ScaleCopy(this IHas<IQuboidLogic> quboid, double factor)
        //{
        //    if (factor < 0)
        //        throw new ArgumentException("Negative scaling not allowed.");

        //    LWQuboid qplace = new LWQuboid();
        //    qplace.Width(quboid.Width() * factor);
        //    qplace.Length(quboid.Length() * factor);
        //    qplace.Height(quboid.Height() * factor);

        //    return qplace;
        //}
        public static void Scale(this IHas<IQuboidLogic> quboid, double factor)
        {
            if (factor < 0)
                throw new ArgumentException("Negative scaling not allowed.");

            quboid.Width(quboid.Width() * factor);
            quboid.Length(quboid.Length() * factor);
            quboid.Height(quboid.Height() * factor);
        }
        public static void Scale(this IHas<IQuboidLogic> quboid, double x, double y, double z)
        {
            if (x < 0 || y < 0 || z < 0)
                throw new ArgumentException("Negative scaling not allowed.");

            quboid.Width(quboid.Width() * x);
            quboid.Length(quboid.Length() * y);
            quboid.Height(quboid.Height() * z);
        }
        public static bool Contains(this IHas<IQuboidPlaceLogic> quboidPlace, IHas<IQuboidPlaceLogic> otherQuboidPlace)
        {

            if (otherQuboidPlace.Back() >= quboidPlace.Back())
                if (otherQuboidPlace.Left() >= quboidPlace.Left())
                    if (otherQuboidPlace.Front() <= quboidPlace.Front())
                        if (otherQuboidPlace.Right() <= quboidPlace.Right())
                            if (otherQuboidPlace.Up() <= quboidPlace.Up())
                                if (otherQuboidPlace.Down() >= quboidPlace.Down())
                                    return true;
            return false;
        }
        public static bool IsInside(this IHas<IQuboidPlaceLogic> quboidPlace, IHas<IQuboidPlaceLogic> otherQuboidPlace)
        {

            if (otherQuboidPlace.Back() <= quboidPlace.Back())
                if (otherQuboidPlace.Front() >= quboidPlace.Front())
                    if (otherQuboidPlace.Left() <= quboidPlace.Left())
                        if (otherQuboidPlace.Right() >= quboidPlace.Right())
                            return true;
            return false;
        }

        //public static IQuboidPlace BiggestRest(this IQuboidPlace quboidplace, IQuboidPlace occupiedPlace)
        //{

        //}

        public static bool Intersect(this IHas<IQuboidPlaceLogic> quboidPlace, IHas<IQuboidPlaceLogic> otherQuboidPlace)
        {
            if (
                (quboidPlace.Back() >= otherQuboidPlace.Front()) || (quboidPlace.Front() <= otherQuboidPlace.Back()) ||
                (quboidPlace.Left() >= otherQuboidPlace.Right()) || (quboidPlace.Right() <= otherQuboidPlace.Left()) ||
                (quboidPlace.Down() >= otherQuboidPlace.Up()) || (quboidPlace.Up() <= otherQuboidPlace.Down()))
                return false;

            return true;
        }
        public static bool Intersect2D(this IHas<IQuboidPlaceLogic> quboidPlace, IHas<IQuboidPlaceLogic> otherQuboidPlace)
        {
            if (
                (quboidPlace.Back() >= otherQuboidPlace.Front()) || (quboidPlace.Front() <= otherQuboidPlace.Back()) ||
                (quboidPlace.Left() >= otherQuboidPlace.Right()) || (quboidPlace.Right() <= otherQuboidPlace.Left())
                )
                return false;

            return true;
        }


        //public static void Init(this IHas<IQuboidPlaceLogic> quboidplace, IHas<IPlaceLogic> place, IHas<IPackageLogic> package, bool alignLeft)
        //{
        //    if (alignLeft)
        //    {
        //        quboidplace.Left(place.Left());
        //    }
        //    else
        //    {
        //        quboidplace.Left(place.Right() - package.Logic.Width());
        //    }
        //    quboidplace.Back(place.Back());
        //    quboidplace.Down(place.Down());
        //    quboidplace.Length(package.Logic.Length());
        //    quboidplace.Height(package.Logic.Height());
        //    quboidplace.Width(package.Logic.Width());
        //}

        public static double GravityLength(this IHas<IQuboidPlaceLogic> qplace)
        {
            return (qplace.Back() + qplace.Front()) / 2;
        }
        public static double GravityWidth(this IHas<IQuboidPlaceLogic> qplace)
        {
            return (qplace.Left() + qplace.Right()) / 2;
        }
        public static double GravityHeight(this IHas<IQuboidPlaceLogic> qplace)
        {
            return (qplace.Down() + qplace.Up()) / 2;
        }

        public static IPosition3D GravityCenter(this IEnumerable<IHas<IQuboidPlaceLogic>> qplaces)
        {
            double length = 0, width = 0, height = 0;
            foreach (var qplace in qplaces)
            {
                height += (qplace.Down() + qplace.Up()) / 2;
                width += (qplace.Left() + qplace.Right()) / 2;
                length += (qplace.Back() + qplace.Front()) / 2;
            }
            int placecount = qplaces.Count();
            height /= placecount;
            length /= placecount;
            width /= placecount;
            return new PositionLogic(width, length, height);
        }
        public static double GravityLength(this IEnumerable<IHas<IQuboidPlaceLogic>> qplaces)
        {
            double length = 0;
            foreach (var qplace in qplaces)
            {
                length += (qplace.Back() + qplace.Front()) / 2;
            }
            length /= qplaces.Count();

            return length;
        }
        
    }
}
