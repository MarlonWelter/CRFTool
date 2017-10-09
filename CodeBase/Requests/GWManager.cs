using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public abstract class GWManager<RequestType> : IContextObject
        where RequestType : GWRequest<RequestType>
    {
        public GWManager()
        {
            Register();
        }

        public IGWContext Context { get; set; }

        public void Register()
        {
            this.DoRegister<RequestType>(OnRequest);
        }
        public void Unregister()
        {
            this.DoUnregister<RequestType>(OnRequest);
        }

        protected abstract void OnRequest(RequestType request);
    }
}
