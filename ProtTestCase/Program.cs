using CRFBase;
using CRFBase.GradientDescent;
using CRFToolAppBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace ProtTestCase
{
    class Program
    {
        static void Main(string[] args)
        {

            CRFBase.Build.Do();

            // als Referenz folgende Klasse:
            //var oldWorkFlow = new WorkflowPede();

            //WorkflowPede.Start(null);

            { // example Gradient Descent
                //var request = new ExecuteGradientDescent();


                //request.Request();

                //Console.Write(request.Result);
            }

            { // example viterbi
                var graph = GWGraphpackageOne.GWString<CRFNodeData, CRFEdgeData, CRFGraphData>(12);
                foreach (var node in graph.Nodes)
                {
                    node.Data = new CRFNodeData();
                    node.Data.Scores = new double[] { 1, 2 };
                }
                foreach (var edge in graph.Edges)
                {
                    edge.Data = new CRFEdgeData();
                    edge.Data.Scores = new double[,] { { -11, 1 }, { 2, -22 } };
                }
                var request = new SolveInference(graph, 2, 100);
                request.Request();

                foreach (var entry in request.Solution.Labeling)
                {
                    Console.Write(entry);
                }
            }

        }
    }
}
