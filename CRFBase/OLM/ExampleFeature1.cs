using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.OLM
{
    class ExampleFeature1 : BasisMerkmal<CRFNodeData, CRFEdgeData, CRFGraphData>
    {
        public override int Count(IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph, int[] labeling)
        {
            return (int)graph.Nodes.Sum(node => Score(node, labeling[node.GraphId]));
        }

        public override double Score(IGWNode<CRFNodeData, CRFEdgeData, CRFGraphData> node, int label)
        {
            if (node.Data.Characteristics[0] < 0.5 && label == 1)
                return 1;
            return 0;
        }

        public override double Score(IGWEdge<CRFNodeData, CRFEdgeData, CRFGraphData> edge, int labelhead, int labelfoot)
        {
            return 0;
        }
    }
}
