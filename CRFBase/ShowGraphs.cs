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
        public List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> Graphs { get; set; } = new List<GWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>();
    }
}
