using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class NotifyUser : GWRequest<NotifyUser>
    {
        public NotifyUser(string msg)
        {
            Message = msg;
        }
        public string Message { get; set; }
    }
}
