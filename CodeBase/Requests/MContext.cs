
using System;
using System.Threading.Tasks;

namespace CodeBase
{
    public interface IContextObject
    {
        IGWContext Context { get; set; }
    }

    public abstract class ContextObject : IContextObject
    {
        public IGWContext Context { get; set; }
    }

    public interface IGWContext
    {

    }

    static class DefaultContext<RequestType, ResultType> where RequestType : IHas<IRequestLogic<RequestType, ResultType>>
    {
        private static CrossroadsLogic<RequestType> instance = new CrossroadsLogic<RequestType>();
        public static CrossroadsLogic<RequestType> Instance
        {
            get { return instance; }
            set { instance = value; }
        }
    }

    public enum RequestRunType
    {
        Default,
        Background
    }

    public static class ContextExtensions
    {

        public static void RequestInDefaultContext<RequestType>(this RequestType request, RequestRunType runtype = RequestRunType.Default)
            where RequestType : IHas<IRequestLogic<RequestType, bool>>
        {
            switch (runtype)
            {
                case RequestRunType.Default:
                    DefaultContext<RequestType, bool>.Instance.Enter(request);
                    break;
                case RequestRunType.Background:
                    var task = new Task(() => DefaultContext<RequestType, bool>.Instance.Enter(request));
                    task.Start();
                    break;
                default:
                    break;
            }
        }
        public static ResultType Request<RequestType, ResultType>(this RequestType request, RequestRunType runtype = RequestRunType.Default)
           where RequestType : IHas<IRequestLogic<RequestType, ResultType>>
        {
            switch (runtype)
            {
                case RequestRunType.Default:
                    DefaultContext<RequestType, ResultType>.Instance.Enter(request);
                    break;
                case RequestRunType.Background:
                    var task = new Task(() => DefaultContext<RequestType, ResultType>.Instance.Enter(request));
                    task.Start();
                    break;
                default:
                    break;
            }
            return request.Result();
        }
        public static void DefaultRequest<RequestType, ResultType>(this RequestType request)
            where RequestType : IHas<IRequestLogic<RequestType, ResultType>>
        {
            DefaultContext<RequestType, ResultType>.Instance.Enter(request);
        }
        public static void DoRequest<RequestType>(this IContextObject sender, RequestType request)
            where RequestType : IHas<IRequestLogic<RequestType>>
        {
            sender.DoRequest<RequestType, bool>(request);
        }
        public static void DoRequest<RequestType>(this IGWContext context, RequestType request)
            where RequestType : IHas<IRequestLogic<RequestType>>
        {
            context.DoRequest<RequestType, bool>(request);
        }
        public static void DoRequest<RequestType, ResultType>(this IGWContext senderCTX, RequestType request)
            where RequestType : IHas<IRequestLogic<RequestType, ResultType>>
        {
            var context = senderCTX as IHas<ICRActionLogic<RequestType>>;
            if (context != null)
                context.Enter(request);
            else
                DefaultContext<RequestType, ResultType>.Instance.Enter(request);
        }
        public static void DoRequest<RequestType, ResultType>(this IContextObject sender, RequestType request)
            where RequestType : IHas<IRequestLogic<RequestType, ResultType>>
        {
            var context = sender.Context as IHas<ICRActionLogic<RequestType>>;
            if (context != null)
                context.Enter(request);
            else
                DefaultContext<RequestType, ResultType>.Instance.Enter(request);
        }

        public static void DoRegister<RequestType>(this IContextObject sender, Action<RequestType> callBackMethod)
            where RequestType : IHas<IRequestLogic<RequestType, bool>>
        {
            sender.DoRegister<RequestType, bool>(callBackMethod);
        }
        public static void DoRegister<RequestType, ResultType>(this IContextObject sender, Action<RequestType> callBackMethod)
            where RequestType : IHas<IRequestLogic<RequestType, ResultType>>
        {
            var context = sender.Context as IHas<ICRModificationLogic<RequestType>>;
            if (context != null)
                context.AddPath(callBackMethod);
            else
                DefaultContext<RequestType, ResultType>.Instance.AddPath(callBackMethod);
        }
        public static void DoUnregister<RequestType>(this IContextObject sender, Action<RequestType> callBackMethod)
            where RequestType : IHas<IRequestLogic<RequestType, bool>>
        {
            sender.DoUnregister<RequestType, bool>(callBackMethod);
        }
        public static void DoUnregister<RequestType, ResultType>(this IContextObject sender, Action<RequestType> callBackMethod)
            where RequestType : IHas<IRequestLogic<RequestType, ResultType>>
        {
            var context = sender.Context as ICRModificationLogic<RequestType>;
            if (context != null)
                context.RemovePath(callBackMethod);
            else
                DefaultContext<RequestType, ResultType>.Instance.RemovePath(callBackMethod);
        }
    }
}
