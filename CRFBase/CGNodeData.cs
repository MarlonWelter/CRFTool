using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace CRFBase
{
    //data classes for Category Graph
    public class CGNodeData
    {
        public int Category { get; set; }
        public int NumberNodes { get; set; }
        //nodes inside the category
        public List<GWNode<SGLNodeData, SGLEdgeData, SGLGraphData>> Nodes { get; set; }
        public int NumberEdges { get; set; }
        public List<IGWEdge<SGLNodeData, SGLEdgeData, SGLGraphData>> InterEdges { get; set; }
        //used to assign observation for each node from this category
        public double ObservationProbabilty { get; set; }

    }

    public class CGEdgeData
    {
        //number of edges between 2 categories(nodes)
        public int Weight { get; set; }
    }

    public class CGGraphData
    {
        public GWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> ErdösGraph { get; set; }
    }
}
