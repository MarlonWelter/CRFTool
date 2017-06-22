using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public abstract class Listener<ReqType> : IRequestListener
       where ReqType : IHas<IRequestLogic<ReqType>>
    {
        public Listener(IGWContext context = null)
        {
            Context = context;
            Register();
        }

        protected abstract void OnReq(ReqType obj);

        public void Register()
        {
            this.DoRegister<ReqType>(OnReq);
        }

        public void Unregister()
        {
            this.DoUnregister<ReqType>(OnReq);
        }
        public IGWContext Context { get; set; }
    }
    public abstract class Listener<ReqType1, ReqType2> : IRequestListener
        where ReqType1 : IHas<IRequestLogic<ReqType1>>
        where ReqType2 : IHas<IRequestLogic<ReqType2>>
    {
        public Listener(IGWContext context = null)
        {
            Context = context;
            Register();
        }

        protected abstract void OnReq1(ReqType1 obj);
        protected abstract void OnReq2(ReqType2 obj);

        public void Register()
        {
            this.DoRegister<ReqType1>(OnReq1);
            this.DoRegister<ReqType2>(OnReq2);
        }

        public void Unregister()
        {
            this.DoUnregister<ReqType1>(OnReq1);
            this.DoUnregister<ReqType2>(OnReq2);
        }
        public IGWContext Context { get; set; }
    }
}
