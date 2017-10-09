
using System;
using System.Collections.Generic;

namespace CodeBase
{

    public interface IPackageLogic : IQuboidLogic
    {
        string Name { get; set; }
        Guid TypeId { get; set; }
        string Description { get; set; }
        string ExternalNumber { get; set; }
        int TourstopPosition { get; set; }
        double[] CarryForce { get; set; }

        int TempTypeId { get; set; }
        int TempOrderId { get; set; }
        double MaxWeightOnTop { get; set; }
        double Weight { get; set; }
        bool CanTurnBack { get; set; }
        bool CanTurnSide { get; set; }

    }
    public class GWPackageLogic : IPackageLogic
    {
        public string Name { get; set; }
        public Guid TypeId { get; set; }
        public string Description { get; set; }
        public string ExternalNumber { get; set; }
        public int TourstopPosition { get; set; }
        public int TempTypeId { get; set; }
        public int TempOrderId { get; set; }
        public double MaxWeightOnTop { get; set; }
        public double Weight { get; set; }
        public bool CanTurnBack { get; set; }
        public bool CanTurnSide { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }

        private double[] carryForce = new double[6];
        public double[] CarryForce { get { return carryForce; } set { carryForce = value; } }



    }

    public static class PackageX
    {
        //public static void Add(this IHas<IPackageGroupLogic> logicHolder, IBasicPackageLogic packageType, ICollection<IHas<IPackageLogic>> instances)
        //{
        //    logicHolder.Logic.PackageGroupItems.Add(new PackageGroupItem(packageType, instances));
        //}
        //public static string Name(this IHas<IPackageGroupLogic> logicHolder)
        //{
        //    return logicHolder.Logic.Name;
        //}
        //public static void Name(this IHas<IPackageGroupLogic> logicHolder, string value)
        //{
        //    logicHolder.Logic.Name = value;
        //}

        public static int TempTypeId(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.TempTypeId;
        }
        public static void TempTypeId(this IHas<IPackageLogic> logicHolder, int value)
        {
            logicHolder.Logic.TempTypeId = value;
        }
        public static int TempOrderId(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.TempOrderId;
        }
        public static void TempOrderId(this IHas<IPackageLogic> logicHolder, int value)
        {
            logicHolder.Logic.TempOrderId = value;
        }
        public static double[] CarryForce(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.CarryForce;
        }
        public static void CarryForce(this IHas<IPackageLogic> logicHolder, double[] value)
        {
            logicHolder.Logic.CarryForce = value;
        }

        //public static ICollection<IPackageGroupLogicItem> PackageGroupItems(this IHas<IPackageGroupLogic> logicHolder)
        //{
        //    return logicHolder.Logic.PackageGroupItems;
        //}
        //public static double WeightForce(this IHas<IPackageLogic> logicHolder, int rotation)
        //{
        //    return logicHolder.Weight() / logicHolder.Ground(rotation);
        //}

        //public static IEnumerable<int> Rotations(this IHas<IBasicPackageLogic> logicHolder)
        //{
        //    yield return 0;
        //    yield return 1;

        //    if (logicHolder.Logic.CanTurnBack)
        //    {
        //        yield return 2;
        //        yield return 3;
        //    }
        //    if (logicHolder.Logic.CanTurnSide)
        //    {
        //        yield return 4;
        //        yield return 5;
        //    }
        //}

        //public static void PackageGroupItems(this IHas<IPackageGroupLogic> logicHolder, ICollection<IPackageGroupLogicItem> value)
        //{
        //    logicHolder.Logic.PackageGroupItems = value;
        //}

        //public static Guid GroupId(this IHas<IPackageLogic> logicHolder)
        //{
        //    return logicHolder.Logic.GroupId;
        //}
        //public static void GroupId(this IHas<IPackageLogic> logicHolder, Guid value)
        //{
        //    logicHolder.Logic.GroupId = value;
        //}


        public static int TourstopPosition(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.TourstopPosition;
        }
        public static void TourstopPosition(this IHas<IPackageLogic> logicHolder, int value)
        {
            logicHolder.Logic.TourstopPosition = value;
        }

        public static string ExternalNumber(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.ExternalNumber;
        }
        public static void ExternalNumber(this IHas<IPackageLogic> logicHolder, string value)
        {
            logicHolder.Logic.ExternalNumber = value;
        }

        public static string Description(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.Description;
        }
        public static void Description(this IHas<IPackageLogic> logicHolder, string value)
        {
            logicHolder.Logic.Description = value;
        }

        public static Guid TypeId(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.TypeId;
        }
        public static void TypeId(this IHas<IPackageLogic> logicHolder, Guid value)
        {
            logicHolder.Logic.TypeId = value;
        }


        public static double MaxWeightOnTop(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.MaxWeightOnTop;
        }
        public static void MaxWeightOnTop(this IHas<IPackageLogic> logicHolder, double value)
        {
            logicHolder.Logic.MaxWeightOnTop = value;
        }

        public static string Name(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.Name;
        }
        public static void Name(this IHas<IPackageLogic> logicHolder, string value)
        {
            logicHolder.Logic.Name = value;
        }

        public static double Weight(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.Weight;
        }
        public static void Weight(this IHas<IPackageLogic> logicHolder, double value)
        {
            logicHolder.Logic.Weight = value;
        }
        public static bool CanTurnBack(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.CanTurnBack;
        }
        public static void CanTurnBack(this IHas<IPackageLogic> logicHolder, bool value)
        {
            logicHolder.Logic.CanTurnBack = value;
        }
        public static bool CanTurnSide(this IHas<IPackageLogic> logicHolder)
        {
            return logicHolder.Logic.CanTurnSide;
        }
        public static void CanTurnSide(this IHas<IPackageLogic> logicHolder, bool value)
        {
            logicHolder.Logic.CanTurnSide = value;
        }

        public static bool Rotation(this IHas<IPackageLogic> package, int rotation)
        {
            if (rotation == 0 || rotation == 1)
                return true;
            else if (rotation == 2 || rotation == 3)
            {
                return package.CanTurnBack();
            }
            else if (rotation == 4 || rotation == 5)
                return package.CanTurnSide();

            return false;
        }

        public static bool CanRotate(this IPackageLogic package, int rotation)
        {
            switch (rotation)
            {
                case 0:
                    return true;
                case 1:
                    return true;
                case 2:
                    return package.CanTurnBack;
                case 3:
                    return package.CanTurnBack;
                case 4:
                    return package.CanTurnSide;
                case 5:
                    return package.CanTurnSide;
                default:
                    return false;
            }
        }
        public static bool CanRotate(this IHas<IPackageLogic> package, int rotation)
        {
            return package.Logic.CanRotate(rotation);
        }
        //public static MITPackage NewInstance(this IHas<IPackageLogic> model)
        //{
        //    return new MITPackage(model);
        //}
        public static void GetRotatedSides(this IPackageLogic package, int rotation, out double packageWidth, out double packageLength, out double packageHeight)
        {
            switch (rotation)
            {
                case 0:
                    packageWidth = package.Width;
                    packageLength = package.Length;
                    packageHeight = package.Height;
                    break;
                case 1:
                    packageWidth = package.Length;
                    packageLength = package.Width;
                    packageHeight = package.Height;
                    break;
                case 2:
                    packageWidth = package.Width;
                    packageLength = package.Height;
                    packageHeight = package.Length;
                    break;
                case 3:
                    packageWidth = package.Height;
                    packageLength = package.Width;
                    packageHeight = package.Length;
                    break;
                case 4:
                    packageWidth = package.Height;
                    packageLength = package.Length;
                    packageHeight = package.Width;
                    break;
                case 5:
                    packageWidth = package.Length;
                    packageLength = package.Height;
                    packageHeight = package.Width;
                    break;
                default:
                    packageWidth = package.Width;
                    packageLength = package.Length;
                    packageHeight = package.Height;
                    break;
            }

        }

        public static void GetRotatedSides(this IHas<IPackageLogic> package, int rotation, out double packageWidth, out double packageLength, out double packageHeight)
        {
            GetRotatedSides(package.Logic, rotation, out packageWidth, out packageLength, out packageHeight);
        }

        public static bool FitsInto(this IHas<IPackageLogic> package, IHas<IContainerLogic> container)
        {
            for (int i = 0; i < 6; i++)
            {
                if (package.CanRotate(i))
                {
                    package.Rotate(i);

                    if (container.Contains(package))
                        return true;
                    package.InvertRotate(i);
                }
            }
            return false;
        }
    }
}
