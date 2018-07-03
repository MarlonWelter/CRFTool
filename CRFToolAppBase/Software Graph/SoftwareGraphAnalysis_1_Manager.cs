using CodeBase;
using CRFBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase.Software_Graph
{
    class SoftwareGraphAnalysis_1_Manager : GWManager<SoftwareGraphAnalysis_1>
    {
        public SoftwareGraphAnalysis_1_Manager()
        {

        }
        protected override void OnRequest(SoftwareGraphAnalysis_1 request)
        {
            // load data
            var graphs = new List<GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
            foreach (var file in Directory.GetFiles(request.GraphInputFolder))
            {
                using (var reader = new StreamReader(file))
                {
                    // parse graph
                }
            }

            // compute viterbi
            var results = new List<SolveInference>();
            var isingModel = new IsingModel(request.ConformityParameter, request.CorrelationParameter);
            foreach (var graph in graphs)
            {
                isingModel.CreateCRFScore(graph);
                var viterbiRequest = new SolveInference(graph, 2, request.ViterbiHeuristicMemoryParameter);
                viterbiRequest.Request();
                results.Add(viterbiRequest);
            }

            // store results
            Directory.CreateDirectory(request.OutputFolder);
            foreach (var result in results)
            {
                result.SaveAsJSON(result.Graph.Name + "-result.txt");
            }

        }
    }
}
