using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class Node3DWrap<NodeData> : Node3DInfo, ICRFNode3DInfo
    {
        public Node3DWrap(NodeData data)
        {
            Data = data;
        }

        public NodeData Data { get; set; }
    }

    public class Edge3DWrap<EdgeData> : IEdge3DInfo
    {
        public Edge3DWrap(EdgeData data)
        {
            Data = data;
        }
        public EdgeData Data { get; set; }
        public double Weight { get; set; }
    }

    public static class Wrap3DX
    {
        //public static GWGraph<Node3DWrap<NodeData>, EdgeData, GraphData> Wrap3D<NodeData, EdgeData, GraphData>(this IGWGraph<NodeData, EdgeData, GraphData> graph)
        //{
        //    return graph.Convert<NodeData, Node3DWrap<NodeData>, EdgeData, EdgeData, GraphData, GraphData>((n) => new Node3DWrap<NodeData>(n.Data), (e) => e.Data, (g) => g.Data);
        //}
        public static GWGraph<Node3DWrap<NodeData>, Edge3DWrap<EdgeData>, GraphData> Wrap3D<NodeData, EdgeData, GraphData>(this IGWGraph<NodeData, EdgeData, GraphData> graph)
        {
            return graph.Convert<NodeData, Node3DWrap<NodeData>, EdgeData, Edge3DWrap<EdgeData>, GraphData, GraphData>((n) => new Node3DWrap<NodeData>(n.Data), (e) => new Edge3DWrap<EdgeData>(e.Data), (g) => g.Data);
        }
        public static GWGraph<Node3DWrap<NodeData>, EdgeData, GraphData> Wrap3D<NodeData, EdgeData, GraphData>(this IGWGraph<NodeData, EdgeData, GraphData> graph, Func<IGWNode<NodeData, EdgeData, GraphData>, Node3DWrap<NodeData>> convertNodeData)
        {
            return graph.Convert<NodeData, Node3DWrap<NodeData>, EdgeData, EdgeData, GraphData, GraphData>(convertNodeData, (e) => e.Data, (g) => g.Data);
        }
        public static GWGraph<Node3DWrap<NodeData>, Edge3DWrap<EdgeData>, GraphData> Wrap3D<NodeData, EdgeData, GraphData>(this IGWGraph<NodeData, EdgeData, GraphData> graph, Func<IGWNode<NodeData, EdgeData, GraphData>, Node3DWrap<NodeData>> convertNodeData, Func<IGWEdge<NodeData, EdgeData, GraphData>, Edge3DWrap<EdgeData>> convertEdgeData)
        {
            return graph.Convert<NodeData, Node3DWrap<NodeData>, EdgeData, Edge3DWrap<EdgeData>, GraphData, GraphData>(convertNodeData, convertEdgeData, (g) => g.Data);
        }
    }
}
