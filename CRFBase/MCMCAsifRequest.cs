using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class MCMCAsifRequest : GWRequest<MCMCAsifRequest>
    {
        public MCMCAsifRequest(IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph, int mcmclength)
        {
            Graph = graph;
            MCMCLength = mcmclength;
        }
        public int MCMCLength { get; set; }

        public IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> Graph { get; set; }

        public List<int> StatesList { get; set; } = new List<int>();
    }
}
