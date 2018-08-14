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
    class Program
    {
        static void Main(string[] args)
        {
            CRFBase.Build.Do();

            // als Referenz folgende Klasse:
            var oldWorkFlow = new WorkflowPede();

            //WorkflowPede.Start(null);

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
