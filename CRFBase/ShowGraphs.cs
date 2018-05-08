using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class ShowGraphs : GWRequest<ShowGraphs>
    {
        public List<GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>> Graphs { get; set; } = new List<GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>>();
    }
}
