using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class Created<T> : IHas<IRequestLogic<Created<T>>>
    {
        public Guid GWId { get; set; }
        public Created(T item)
        {
            Item = item;
        }
        public T Item { get; set; }

        private RequestLogic<Created<T>> logic = new RequestLogic<Created<T>>();
        public IRequestLogic<Created<T>> Logic
        {
            get { return logic; }
        }
    }

    public class Deleted<T> : IHas<IRequestLogic<Deleted<T>>>
    {
        public Guid GWId { get; set; }
        public Deleted(T item)
        {
            Item = item;
        }
        public T Item { get; set; }

        private RequestLogic<Deleted<T>> logic = new RequestLogic<Deleted<T>>();
        public IRequestLogic<Deleted<T>> Logic
        {
            get { return logic; }
        }
    }

    public class Get<T> : IHas<IRequestLogic<Get<T>>>
    {
        private Guid mId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mId; }
            set { mId = value; }
        }
        public Get()
        {

        }

        public IEnumerable<T> Elements { get; set; }

        private RequestLogic<Get<T>> logic = new RequestLogic<Get<T>>();
        public IRequestLogic<Get<T>> Logic
        {
            get { return logic; }
        }
    }

    public static class Get
    {
        public static IEnumerable<T> All<T>()
        {
            var request = new Get<T>();
            request.RequestInDefaultContext();
            return request.Elements;
        }
    }
}
