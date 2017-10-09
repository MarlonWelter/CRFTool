using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public interface ILogWriter : IContextObject
    {
        string Name { get; }
        bool DoLog { get; set; }
    }
    public static class LogWriterExtension
    {
        public static void Log(this ILogWriter writer, string message, LogCategory category = LogCategory.Detail)
        {
            new LogRequest(writer.Name + ": " + message, LogCategory.Detail).RequestInDefaultContext();
        }
    }
}
