using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class UserDecision : GWRequest<UserDecision>
    {
        public  UserDecision(params string[] options)
        {
            Options = options;
        }
        public string[] Options { get; set; }
        public int Decision { get; set; }
    }
}
