
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    class ViterbiManager : GWManager<SolveInference>
    {
        public const int DefaultBufferSize = 1000;

        public ViterbiManager(int maxCombinationsBuffer = DefaultBufferSize) : base()
        {
            MaxCombinationsBuffer = maxCombinationsBuffer;
        }
        public int MaxCombinationsBuffer { get; set; }

        override protected void OnRequest(SolveInference obj)
        {
            var heuristik = new ViterbiHeuristic(obj.BufferSize > 0 ? obj.BufferSize : MaxCombinationsBuffer, obj.NumberLabels);
            var result = heuristik.Run(obj.Graph, obj.PreAssignment);
            obj.Solution = result;


        }
    }
}
