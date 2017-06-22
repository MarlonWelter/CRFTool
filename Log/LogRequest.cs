using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class LogRequest : IHas<IRequestLogic<LogRequest>>
    {
        public LogRequest(string message, LogCategory category = LogCategory.Detail)
        {
            Message = message;
            Category = category;
        }

        public readonly string Message;
        public readonly LogCategory Category;

        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }

        private RequestLogic<LogRequest> logic = new RequestLogic<LogRequest>();
        public IRequestLogic<LogRequest> Logic
        {
            get { return logic; }
        }
    }

    public class Log
    {
        public static void Post(string message, LogCategory category = LogCategory.Detail)
        {
            new LogRequest(message, category).RequestInDefaultContext();
        }

        public static void Error(Exception e)
        {
            new LogRequest(e.Message + " || " + e.StackTrace, LogCategory.Critical).RequestInDefaultContext();
        }
    }
}
