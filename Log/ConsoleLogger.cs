
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class ConsoleLogger : IRequestListener
    {
        public IGWContext Context { get; set; }
        public ConsoleLogger()
        {
            Register();
        }

        private void NewLogRequest(LogRequest obj)
        {
            Console.WriteLine(obj.Message);
        }

        public void Register()
        {
            this.DoRegister<LogRequest>(NewLogRequest);
        }

        public void Unregister()
        {
            this.DoUnregister<LogRequest>(NewLogRequest);
        }
    }
}
