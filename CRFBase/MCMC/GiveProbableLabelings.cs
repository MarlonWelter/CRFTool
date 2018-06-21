
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRFGraph = CodeBase.IGWGraph<CodeBase.ICRFNodeData, CodeBase.ICRFEdgeData, CodeBase.ICRFGraphData>;

namespace CRFBase.GibbsSampling
{
    public class GiveProbableLabelings : GWRequest<GiveProbableLabelings>
    {
        public int StartingPoints = 100;
        public int PreRunLength = 1000;
        public int RunLength = 99000;
        public int Labels { get; set; }

        public GiveProbableLabelings(CRFGraph graph)
        {
            Graph = graph;
        }

        public GiveProbableLabelings(CRFGraph graph, int labels, int startingpoints , int preRun, int runLength)
        {
            Graph = graph;
            Labels = labels;
            StartingPoints = startingpoints;
            PreRunLength = preRun;
            RunLength = runLength;
        }

        public Dictionary<IGWNode, double> Result { get; set; }
        public CRFGraph Graph { get; set; }
    }
}
