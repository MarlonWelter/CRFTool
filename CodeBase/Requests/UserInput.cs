using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class UserInput : GWRequest<UserInput>
    {
        public string UserText { get; set; }
        public string TextForUser { get; set; }
    }
}
