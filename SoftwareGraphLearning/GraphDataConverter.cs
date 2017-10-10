using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRFBase;
using CodeBase;

namespace SoftwareGraphLearning
{
    class GraphDataConverter
    {
        public static ICRFNodeData IGWNodeDataConvert(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node)
        {
            var newData = new CRFNodeData();
            newData.Id = node.Data.Id;
            newData.Scores = node.Data.Scores;
            newData.Observation = node.Data.Observation;

            return newData;
        }

        public static ICRFEdgeData IGWEdgeDataConvert(IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge)
        {
            var newData = new CRFEdgeData();
            newData.Scores = edge.Data.Scores;
            return newData;
        }

        public static ICRFGraphData IGWGraphDataConvert(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            var newData = new CRFGraphData();
            return newData;
        }


        public static SGLNodeData SGLNodeDataConvert(IGWNode<SGLNodeData, SGLEdgeData, SGLGraphData> node)
        {
            var newData = new SGLNodeData();
            newData.Id = node.Data.Id;
            newData.Scores = node.Data.Scores;
            newData.Observation = node.Data.Observation;
            newData.Category = node.Data.Category;

            return newData;
        }

        public static SGLEdgeData SGLEdgeDataConvert(IGWEdge<SGLNodeData, SGLEdgeData, SGLGraphData> edge)
        {
            var newData = new SGLEdgeData();
            newData.Scores = edge.Data.Scores;
            newData.Type = edge.Data.Type;
            return newData;

        }


        public static SGLGraphData SGLGraphDataConvert(IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> graph)
        {
            var newData = new SGLGraphData();
            newData.NumberCategories = graph.Data.NumberCategories;
            //newData.CategoryGraph = graph.Data.CategoryGraph.Clone(CGNodeDataConvert, CGEdgeDataConvert, CGGraphDataConvert);
            return newData;
        }


        public static CGNodeData CGNodeDataConvert(IGWNode<CGNodeData, CGEdgeData, CGGraphData> node)
        {
            var newData = new CGNodeData();
            newData.Category = node.Data.Category;
            newData.NumberNodes = node.Data.NumberNodes;
            newData.NumberEdges = node.Data.NumberEdges;
            newData.ObservationProbabilty = node.Data.ObservationProbabilty;
            //newData.Nodes = node.Data.Nodes;
            //newData.InterEdges = node.Data.InterEdges;
            return newData;
        }

        public static CGEdgeData CGEdgeDataConvert(IGWEdge<CGNodeData, CGEdgeData, CGGraphData> edge)
        {
            var newData = new CGEdgeData();
            newData.Weight = edge.Data.Weight;
            return newData;
        }

        public static CGGraphData CGGraphDataConvert(IGWGraph<CGNodeData, CGEdgeData, CGGraphData> graph)
        {
            var newData = new CGGraphData();
            newData.ErdösGraph = graph.Data.ErdösGraph;
            return newData;
        }
    }
}
