
using System;

namespace CodeBase
{
    public enum Loadingtype
    {
        LoadingSpace,
        LoadingGround
    }

    //public interface IMITContainer : IQuboid, IMITObject
    //{
    //    double MaxPayload { get; set; }
    //    string Name { get; set; }
    //    double OwnWeight { get; set; }
    //    Loadingtype Loadingtype { get; set; }
    //    double OutsideHeight { get; set; }
    //    double OutsideLength { get; set; }
    //    double OutsideWidth { get; set; }
    //}
    public interface IContainerLogic : IQuboidLogic
    {
        Guid TypeId { get; set; }
        double MaxPayload { get; set; }
        string Name { get; set; }
        double OwnWeight { get; set; }
        Loadingtype Loadingtype { get; set; }
        double OutsideHeight { get; set; }
        double OutsideLength { get; set; }
        double OutsideWidth { get; set; }
        double Cost { get; set; }
    }
    public class ContainerLogic : QuboidLogic, IContainerLogic
    {	
        public Guid TypeId { get; set; }
        public double MaxPayload { get; set; }
        public string Name { get; set; }
        public double OwnWeight { get; set; }
        public Loadingtype Loadingtype { get; set; }
        public double OutsideHeight { get; set; }
        public double OutsideLength { get; set; }
        public double OutsideWidth { get; set; }
        public double Cost { get; set; }
    }
    public static class ContainerX
    {
        public const double DefaultCost = 100.0;
        public static double Cost(this IHas<IContainerLogic> logicHolder)
        {
            return logicHolder.Logic.Cost;
        }
        public static void Cost(this IHas<IContainerLogic> logicHolder, double value)
        {
            logicHolder.Logic.Cost = value;
        }


        public static Guid TypeId(this IHas<IContainerLogic> logicHolder)
        {
            return logicHolder.Logic.TypeId;
        }
        public static void TypeId(this IHas<IContainerLogic> logicHolder, Guid value)
        {
            logicHolder.Logic.TypeId = value;
        }

        public static double OutsideWidth(this IHas<IContainerLogic> logicHolder)
        {
            return logicHolder.Logic.OutsideWidth;
        }
        public static void OutsideWidth(this IHas<IContainerLogic> logicHolder, double value)
        {
            logicHolder.Logic.OutsideWidth = value;
        }

        public static double OutsideLength(this IHas<IContainerLogic> logicHolder)
        {
            return logicHolder.Logic.OutsideLength;
        }
        public static void OutsideLength(this IHas<IContainerLogic> logicHolder, double value)
        {
            logicHolder.Logic.OutsideLength = value;
        }

        public static double OutsideHeight(this IHas<IContainerLogic> logicHolder)
        {
            return logicHolder.Logic.OutsideHeight;
        }
        public static void OutsideHeight(this IHas<IContainerLogic> logicHolder, double value)
        {
            logicHolder.Logic.OutsideHeight = value;
        }

        public static Loadingtype Loadingtype(this IHas<IContainerLogic> logicHolder)
        {
            return logicHolder.Logic.Loadingtype;
        }
        public static void Loadingtype(this IHas<IContainerLogic> logicHolder, Loadingtype value)
        {
            logicHolder.Logic.Loadingtype = value;
        }

        public static double OwnWeight(this IHas<IContainerLogic> logicHolder)
        {
            return logicHolder.Logic.OwnWeight;
        }
        public static void OwnWeight(this IHas<IContainerLogic> logicHolder, double value)
        {
            logicHolder.Logic.OwnWeight = value;
        }

        public static string Name(this IHas<IContainerLogic> logicHolder)
        {
            return logicHolder.Logic.Name;
        }
        public static void Name(this IHas<IContainerLogic> logicHolder, string value)
        {
            logicHolder.Logic.Name = value;
        }

        public static double MaxPayload(this IHas<IContainerLogic> logicHolder)
        {
            return logicHolder.Logic.MaxPayload;
        }
        public static void MaxPayload(this IHas<IContainerLogic> logicHolder, double value)
        {
            logicHolder.Logic.MaxPayload = value;
        }

        //public static void TakeValuesOf(this IMITContainer container, IMITContainer model)
        //{
        //    if (model == null)
        //        return;
        //    container.Length = model.Length;
        //    container.Height = model.Height;
        //    container.Width = model.Width;
        //    container.MaxPayload = model.MaxPayload;
        //    container.OwnWeight = model.OwnWeight;
        //    container.Name = model.Name;
        //    container.Loadingtype = model.Loadingtype;
        //    container.OutsideHeight = model.OutsideHeight;
        //    container.OutsideLength = model.OutsideLength;
        //    container.OutsideWidth = model.OutsideWidth;
        //}
    }
}
