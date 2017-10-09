using System;
using CodeBase;

namespace CRFGit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting CRF Tool.");

            new ConsoleLogger();
           /*  CRFBase.Build.Do();
            CRFGraphVis.Build.Do();

            var app = new CRFToolContext(); */

            /* var line = "";
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
            } */
        }
    }
}
