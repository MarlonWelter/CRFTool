using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using CRFBase;

namespace CRFToolAppBase
{
    class Labeling {

        private static Random rdm = new Random();

        // delete graph
        public static void randomLabeling(GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> cloneGraph) {

            int count = cloneGraph.Data.ReferenceLabeling.Count();
            int[] label = new int[count];

            for (int j = 0; j < count; j++) {
                int l = rdm.Next(0, 2);
                label[j] = l;
                //Console.Write(label[i]);
            }

            cloneGraph.Data.ReferenceLabeling = label;
        }

        public static void assignedLabeling(GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> cloneGraph) {

            // Aufruf der Inferenz-Heuristik
            int numberOfLabels = 2;
            int bufferSize = 200;
            var request = new SolveInference(cloneGraph, numberOfLabels, bufferSize);
            request.RequestInDefaultContext();

            cloneGraph.Data.AssginedLabeling = request.Result.Labeling;
        }
    }
}
