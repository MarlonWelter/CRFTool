
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    class InferenceManager : GWManager<SolveInference>
    {
        public const int DefaultBufferSize = 1000;

        public InferenceManager(int maxCombinationsBuffer = DefaultBufferSize) : base()
        {
            MaxCombinationsBuffer = maxCombinationsBuffer;
        }
        public int MaxCombinationsBuffer { get; set; }

        override protected void OnRequest(SolveInference obj)
        {
            //if (obj.Labels == 2)
            //{
            //    GWGraph<ICRFNodeDataBinary, ICRFEdgeDataBinary, ICRFGraphData> newGraph = obj.Graph.Convert<ICRFNodeData, ICRFNodeDataBinary, ICRFEdgeData, ICRFEdgeDataBinary, ICRFGraphData, ICRFGraphData>((nd) => new CRFNodeDataBinary(nd.Data.Scores[0], nd.Data.Scores[1]) { Id = nd.Data.Id, Ordinate = nd.Data.Ordinate,  }, (ed) => new CRFEdgeDataBinary(ed.Data.Scores[0, 0], ed.Data.Scores[0, 1], ed.Data.Scores[1, 0], ed.Data.Scores[1, 1]), (gd) => gd.Data);
            //    var heuristik = new InferenceHeuristikBinary(obj.BufferSize > 0 ? obj.BufferSize : MaxCombinationsBuffer);
            //    var result = heuristik.Run(newGraph);
            //    obj.Solution = result;
            //}
            //else
            {
                var heuristik = new InferenceHeuristik(obj.BufferSize > 0 ? obj.BufferSize : MaxCombinationsBuffer);
                var result = heuristik.Run(obj.Graph, obj.PreAssignment);
                obj.Solution = result;
            }

        }
    }
}
