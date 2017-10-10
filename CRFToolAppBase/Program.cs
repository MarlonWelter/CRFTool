using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase
{
    class Program
    {
        static void Main(string[] args)
        {
            new ConsoleLogger();
            CRFBase.Build.Do();
            //CRFGraphVis.Build.Do();

            var app = new CRFToolContext();

            var line = "";
            while (true)
            {
                line = Console.ReadLine();
                switch (line)
                {
                    case "exit":
                        return;
                    default:
                        app.CommandIn(line);
                        break;
                }
            }
        }
    }
}
