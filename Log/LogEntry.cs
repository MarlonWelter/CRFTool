using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class LogEntry
    {
        public LogEntry(string message, LogCategory category = LogCategory.Detail)
        {
            Message = message;
            Category = category;
        }
        public LogEntry(string sender, string message, LogCategory category = LogCategory.Detail)
        {
            Message = message;
            Sender = sender;
            Category = category;
        }

        #region Properties

        public string Sender { get; set; }
        public string Message { get; set; }
        public LogCategory Category { get; set; }

        #endregion

        #region Methods



        #endregion

    }
}
