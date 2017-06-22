
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public interface Is<out Type>
    {
        Type Element { get; }
    }

    public interface ILogic : IGWObject
    {

    }

    public interface IHas<out Type> : IGWObject 
    {
        Type Logic { get; }
    }
    public class RequestLogic<Requesttype> : RequestLogic<Requesttype, bool>, IRequestLogic<Requesttype>
    {
    }
    public class RequestLogic<Requesttype, ResultType> : IRequestLogic<Requesttype, ResultType>
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        public bool IsOpen { get; set; }
        public bool ProcessedSuccessful { get; set; }
        private MEvent<Requesttype> reqClosed = new MEvent<Requesttype>();
        public MEvent<Requesttype> RequestClosed
        {
            get { return reqClosed; }
            set { reqClosed = value; }
        }
        public double Progress { get; set; }

        public ResultType Result { get; set; }

        ICRModificationLogic INotifyEnd.End
        {
            get { return RequestClosed; }
        }
    }
    public interface IRequestLogic<Requesttype> : IRequestLogic<Requesttype, bool>
    {

    }
    public interface IRequestLogic<Requesttype, ResultType> : BasicRequestLogic
    {
        ResultType Result { get; set; }
        MEvent<Requesttype> RequestClosed { get; }
    }
    public interface BasicRequestLogic : INotifyEnd, ILogic
    {
        bool IsOpen { get; set; }
        bool ProcessedSuccessful { get; set; }
        double Progress { set; get; }
    }
    public interface INotifyEnd
    {
        ICRModificationLogic End { get; }
    }

    public static class RequestExtension
    {

        public static void CloseRequest<Type>(this Type req, bool successful = true) where Type : IHas<IRequestLogic<Type>>
        {
            req.Logic.ProcessedSuccessful = successful;
            req.Logic.IsOpen = false;
            req.Logic.RequestClosed.Enter(req);
        }
        public static bool ProcessedSuccessful<Type, ResultType>(this IHas<IRequestLogic<Type, ResultType>> req)
        {
            return req.Logic.ProcessedSuccessful;
        }
        public static ResultType Result<Type, ResultType>(this IHas<IRequestLogic<Type, ResultType>> logicHolder)
        {
            return logicHolder.Logic.Result;
        }
        public static ResultType Result<Type, ResultType>(this IHas<IRequestLogic<Type, ResultType>> logicHolder, ResultType result)
        {
            return logicHolder.Logic.Result = result;
        }
        public static void DoOnRequestClosed<Type, ResultType>(this IHas<IRequestLogic<Type, ResultType>> logicHolder, Action<Type> action)
        {
            logicHolder.Logic.RequestClosed.AddPath(action);
        }
        public static void DoOnRequestClosed<Type>(this IHas<IRequestLogic<Type>> logicHolder, Action<Type> action)
        {
            logicHolder.Logic.RequestClosed.AddPath(action);
        }
        public static void Progress<Type>(this IHas<IRequestLogic<Type>> logicHolder, double progress)
        {
            logicHolder.Logic.Progress = progress;
        }
        public static double Progress<Type>(this IHas<IRequestLogic<Type>> logicHolder)
        {
            return logicHolder.Logic.Progress;
        }
    }
}
