using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using CRFBase;

namespace CRFToolAppBase
{
    class ConvertData
    {
        public static ICRFGraphData convertGraphData(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {

            var newData = new CRFGraphData();
            try
            {
                newData.ReferenceLabeling = graph.Data.ReferenceLabeling;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Exception caught: " + e.Message);
            }

            return newData;
        }

        public static ICRFEdgeData convertEdgeData(IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> arg)
        {
            // scores übernehmen
            var newData = new CRFEdgeData();
            newData.Scores = arg.Data.Scores;

            return newData;
        }

        public static ICRFNodeData convertNodeData(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> arg)
        {
            var newData = new CRFNodeData();
            newData.Scores = arg.Data.Scores;
            newData.ReferenceLabel = arg.Data.ReferenceLabel;
            newData.Id = arg.Data.Id;

            return newData;
        }
    }
}
