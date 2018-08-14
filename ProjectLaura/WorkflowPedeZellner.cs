using System;
using CRFBase;
using CRFBase.GradientDescent;
using CRFToolAppBase;
using CodeBase;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLaura

{
    class WorkflowPedeZellner
    {
        public static void Main(string[] args)
        {
            // manager erzeugen
            new ConsoleLogger();
            new FileLogger("Log/");
            //new RasaManager("../../Data/RASA/", @"../../Data/hermannData/");
            //new PDBFileManager(@"../../Data/hermannData/");

            //startTrainingCycle();
            Console.WriteLine("Hello");
            Console.ReadKey();

            BaseProgram.Exit.Enter();
        }
    }
}
