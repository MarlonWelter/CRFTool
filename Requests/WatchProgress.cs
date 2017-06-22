
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class WatchProgress : IHas<IRequestLogic<WatchProgress>>
    {
       
        public WatchProgress(IHas<BasicRequestLogic> request)
        {
            Request = request;
        }
        public IHas<BasicRequestLogic> Request { get; set; }

        private RequestLogic<WatchProgress> logic = new RequestLogic<WatchProgress>();
        public IRequestLogic<WatchProgress> Logic
        {
            get { return logic; }
        }
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }	
    }
}
