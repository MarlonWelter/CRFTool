using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace SoftwareGraphLearning
{
    //data classes for Category Graph
    class CGNodeData
    {
        public int Category { get; set; }
        public int NumberNodes { get; set; }
        //nodes inside the category
        public List<IGWNode<SGLNodeData, SGLEdgeData, SGLGraphData>> Nodes { get; set; }
        public int NumberEdges { get; set; }
        //public List<IGWEdge<SGLNodeData, SGLEdgeData, SGLGraphData>> IncomingEdges { get; set; }
        //used to assign observation for each node from this category
        public double ObservationProbabilty { get; set; }

    }

    class CGEdgeData
    {
        //number of edges between 2 categories(nodes)
        public int Weight { get; set; }
    }

    class CGGraphData
    {
        public IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> ErdösGraph { get; set; }
    }
}
