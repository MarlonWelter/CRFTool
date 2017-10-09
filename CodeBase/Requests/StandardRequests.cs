using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class Register<T> : IHas<IRequestLogic<Register<T>>>
        where T : IContextObject
    {
        public Register(Guid objectId)
        {
            ObjectId = objectId;
        }
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        private IRequestLogic<Register<T>> logic = new RequestLogic<Register<T>>();
        public IRequestLogic<Register<T>> Logic
        {
            get { return logic; }
        }

        public Guid ObjectId { get; set; }
        public T Object { get; set; }
        public IGWContext Context { get; set; }
    }
}
