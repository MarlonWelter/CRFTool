
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRFGraph = CodeBase.IGWGraph<CodeBase.ICRFNodeData, CodeBase.ICRFEdgeData, CodeBase.ICRFGraphData>;

namespace CRFBase
{
    public class SolveInference : GWRequest<SolveInference>
    {
        public SolveInference(CRFGraph graph, int numberLabels, int bufferSize)
        {
            Graph = graph;
            BufferSize = bufferSize;
            NumberLabels = numberLabels;
        }
        public SolveInference(CRFGraph graph, int numberLabels, IDictionary<IGWNode, int> preAssignment = null, int bufferSize = 0)
        {
            PreAssignment = preAssignment;
            Graph = graph;
            BufferSize = bufferSize;
            NumberLabels = numberLabels;
        }

        public int NumberLabels { get; set; }
        public int BufferSize { get; set; }
        public CRFResult Solution { get; set; }
        public IDictionary<IGWNode, int> PreAssignment { get; set; }
        public CRFGraph Graph { get; set; }
    }
}
