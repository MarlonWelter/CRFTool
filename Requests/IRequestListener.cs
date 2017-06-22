using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public interface IRequestListener : IContextObject
    {
        void Register();
        void Unregister();
    }

    public abstract class RequestManager<RequestType> : IRequestListener
        where RequestType : IHas<IRequestLogic<RequestType>>
    {
        public IGWContext Context { get; set; }

        protected abstract void HandleRequestCore(RequestType request);
        protected void HandleRequest(RequestType request)
        {
            try
            {
                HandleRequestCore(request);
            }
            catch (Exception ex)
            {
                Log.Post(ex.Message, LogCategory.Critical);
            }
        }

        public void Register()
        {
            this.DoRegister<RequestType>(HandleRequest);
        }

        public void Unregister()
        {
            this.DoUnregister<RequestType>(HandleRequest);
        }
    }

    public class DefaultRequestManager<RequestType> : RequestManager<RequestType>
       where RequestType : IHas<IRequestLogic<RequestType>>
    {
        public DefaultRequestManager(Action<RequestType> requestHandleLogic)
        {
            RequestHandleLogic = requestHandleLogic;
        }

        private Action<RequestType> RequestHandleLogic;

        protected override void HandleRequestCore(RequestType request)
        {
            if (RequestHandleLogic != null)
                RequestHandleLogic(request);
        }
    }
}
