
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRFGraph = CodeBase.IGWGraph<CRFBase.ICRFNodeData, CRFBase.ICRFEdgeData, CRFBase.ICRFGraphData>;

namespace CRFBase
{
    public class SolveInference : GWRequest<SolveInference>
    {
        public SolveInference(CRFGraph graph, IDictionary<IGWNode, int> preAssignment, int labels, int bufferSize = 0)
        {
            PreAssignment = preAssignment;
            Graph = graph;
            BufferSize = bufferSize;
            Labels = labels;
        }

        public int Labels { get; set; }

        public int BufferSize { get; set; }
        public CRFResult Solution { get; set; }
        public IDictionary<IGWNode, int> PreAssignment { get; set; }
        public CRFGraph Graph { get; set; }        
    }
}
