using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using CRFBase;

namespace CRFBase
{
    class SGLNodeData : CRFNodeData
    {
        public int Category { get; set; }
        public int LabelTemp { get; set; }
    }
    class SGLEdgeData : CRFEdgeData, IEdge3DInfo
    {   
        public EdgeType Type { get; set; }
    }
    class SGLGraphData : CRFGraphData
    {
        public int NumberCategories { get; set; }
        public IGWGraph<CGNodeData, CGEdgeData, CGGraphData> CategoryGraph { get; set; }
    }

    enum EdgeType { Intra, Inter}

}
