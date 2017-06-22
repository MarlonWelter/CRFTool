
using System;

namespace CodeBase
{
    public class ExecuteInUIThread : IHas<IRequestLogic<ExecuteInUIThread>>
    {
        public ExecuteInUIThread(Action action, bool blocking = true)
        {
            Action = action;
            IsBlocking = blocking;
        }        
        public Action Action { get; set; }
        public bool IsBlocking { get; set; }
        private Guid mitId = Guid.NewGuid();
        public Guid GWId { get { return mitId; } set { mitId = value; } }


        private RequestLogic<ExecuteInUIThread> logic = new RequestLogic<ExecuteInUIThread>();
        public IRequestLogic<ExecuteInUIThread> Logic
        {
            get { return logic; }
        }
    }

    public class ExecuteInBackground : IHas<IRequestLogic<ExecuteInBackground>>
    {
        public ExecuteInBackground(Action action, bool blocking = true)
        {
            Action = action;
            IsBlocking = blocking;
        }
        public Action Action { get; set; }
        public bool IsBlocking { get; set; }
        private Guid mitId = Guid.NewGuid();
        public Guid GWId { get { return mitId; } set { mitId = value; } }


        private RequestLogic<ExecuteInBackground> logic = new RequestLogic<ExecuteInBackground>();
        public IRequestLogic<ExecuteInBackground> Logic
        {
            get { return logic; }
        }
    }
}
