using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class SGLNodeData : CRFNodeData
    {
        public int Category { get; set; }
        public int LabelTemp { get; set; }
    }
    public class SGLEdgeData : CRFEdgeData, IEdge3DInfo
    {
        public EdgeType Type { get; set; }
    }
    public class SGLGraphData : CRFGraphData
    {
        public int NumberCategories { get; set; }
        public GWGraph<CGNodeData, CGEdgeData, CGGraphData> CategoryGraph { get; set; }
    }

    public enum EdgeType { Intra, Inter }

}
