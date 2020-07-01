using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace CRFBase.OLM
{
    public class IsingMerkmalNode : BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>
    {
        public int[] Observations { get; set; }
        public override int Count(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph, int[] labeling)
        {
            return graph.Nodes.Sum(n => n.Data.Observation == labeling[n.GraphId] ? 1 : 0);
        }

        public override double Score(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node, int label)
        {
            return node.Data.Observation == label ? 1 : -1;
        }

        public override double Score(IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge, int labelhead, int labelfoot)
        {
            return 0;
        }
    }
    public class IsingMerkmalEdge : BasisMerkmal<ICRFNodeData, ICRFEdgeData, ICRFGraphData>
    {
        public override int Count(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph, int[] labeling)
        {
            return graph.Edges.Sum(e => labeling[e.Foot.GraphId] == labeling[e.Head.GraphId] ? 1 : 0);
        }

        public override double Score(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node, int label)
        {
            return 0;
        }

        public override double Score(IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge, int labelhead, int labelfoot)
        {
            return (labelfoot == labelhead) ? 1 : -1;
        }
    }
}
