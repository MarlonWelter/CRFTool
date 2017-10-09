using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class Start : GWRequest<Start>
    {
        public Start(string program)
        {
            Program = program;
        }
        public string Program { get; set; }
    }

    public class Quit : GWRequest<Quit>
    {
        public Quit(string program)
        {
            Program = program;
        }
        public string Program { get; set; }
    }

    public enum Programs
    {
        AnalyseGraph
    }
}
