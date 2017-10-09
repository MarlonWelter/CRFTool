using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class Provide<T> : IHas<IRequestLogic<Provide<T>, T>>
    {
        public Guid GWId { get; set; }

        private RequestLogic<Provide<T>, T> logic = new RequestLogic<Provide<T>, T>();

        public IRequestLogic<Provide<T>, T> Logic
        {
            get { return logic; }
        }

    }
}
