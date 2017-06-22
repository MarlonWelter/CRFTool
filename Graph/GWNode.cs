using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public interface IGWGraph : IGWObject
    {
        [GWStore]
        string Name { get; }
        [GWStore]
        IEnumerable<IGWNode> Nodes { get; }
        [GWStore]
        IEnumerable<IGWEdge> Edges { get; }
    }

    public interface IGWGraph<out NodeData, out EdgeData, out GraphData> : IGWGraph
    {
        GraphData Data { get; }
        GraphStyle GraphStyle { get; set; }
        new IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> Nodes { get; }
        new IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> Edges { get; }
    }
    public interface IGWNode<out NodeData, out EdgeData, out GraphData> : IGWNode
    {
        NodeData Data { get; }
        new IGWGraph<NodeData, EdgeData, GraphData> Graph { get; }
        new IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> Parents { get; }
        new IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> Children { get; }
        new IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> Neighbours { get; }
        new IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> InEdges { get; }
        new IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> OutEdges { get; }
        new IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> Edges { get; }
    }
    public interface IGWEdge<out NodeData, out EdgeData, out GraphData> : IGWEdge
    {
        EdgeData Data { get; }
        //IGWGraph<NodeData, EdgeData, GraphData> Graph { get; }
        new IGWNode<NodeData, EdgeData, GraphData> Foot { get; }
        new IGWNode<NodeData, EdgeData, GraphData> Head { get; }
    }
    public interface IGWNode : IGWObject
    {
        IGWGraph Graph { get; }

        [GWStore]
        int GraphId { get; }

        IEnumerable<IGWNode> Parents { get; }
        IEnumerable<IGWNode> Children { get; }
        IEnumerable<IGWNode> Neighbours { get; }
        IEnumerable<IGWEdge> InEdges { get; }
        IEnumerable<IGWEdge> OutEdges { get; }
        IEnumerable<IGWEdge> Edges { get; }
    }
    public interface IGWEdge : IGWObject
    {
        //IGWGraph Graph { get; }
        IGWNode Foot { get; }
        IGWNode Head { get; }
    }
    public class GWGraph<NodeData, EdgeData, GraphData> : IGWGraph<NodeData, EdgeData, GraphData>
    {
        public GWGraph()
        {

        }
        public GWGraph(string name)
        {
            Name = name;
        }
        private Guid gwId = Guid.NewGuid();
        public Guid GWId
        {
            get { return gwId; }
            set { gwId = value; }
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public GraphData Data { get; set; }
        public GraphStyle GraphStyle { get; set; }

        private LinkedList<GWNode<NodeData, EdgeData, GraphData>> nodes = new LinkedList<GWNode<NodeData, EdgeData, GraphData>>();
        public LinkedList<GWNode<NodeData, EdgeData, GraphData>> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        private LinkedList<GWEdge<NodeData, EdgeData, GraphData>> edges = new LinkedList<GWEdge<NodeData, EdgeData, GraphData>>();


        public LinkedList<GWEdge<NodeData, EdgeData, GraphData>> Edges
        {
            get { return edges; }
            set { edges = value; }
        }

        public int NodeCounter { get; private set; }
        public GWNode<NodeData, EdgeData, GraphData> CreateNode()
        {
            var node = new GWNode<NodeData, EdgeData, GraphData>(this);

            node.GraphId = NodeCounter;
            NodeCounter++;

            nodes.Add(node);

            return node;
        }

        public void RemoveNode(GWNode<NodeData, EdgeData, GraphData> node)
        {
            Nodes.Remove(node);
            foreach (GWEdge<NodeData, EdgeData, GraphData> edge in node.Edges.ToList())
            {
                edges.Remove(edge);
                (edge.Head as GWNode<NodeData, EdgeData, GraphData>).edges.Remove(edge);
                (edge.Foot as GWNode<NodeData, EdgeData, GraphData>).edges.Remove(edge);
            }
        }

        public GWEdge<NodeData, EdgeData, GraphData> CreateEdge(GWNode<NodeData, EdgeData, GraphData> foot, GWNode<NodeData, EdgeData, GraphData> head)
        {
            var newEdge = new GWEdge<NodeData, EdgeData, GraphData>(foot, head);
            foot.AddOutEdge(newEdge);
            head.AddInEdge(newEdge);

            edges.Add(newEdge);

            return newEdge;
        }

        IEnumerable<IGWNode> IGWGraph.Nodes
        {
            get { return Nodes; }
        }

        IEnumerable<IGWEdge> IGWGraph.Edges
        {
            get { return Edges; }
        }

        IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> IGWGraph<NodeData, EdgeData, GraphData>.Nodes
        {
            get { return Nodes; }
        }

        IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> IGWGraph<NodeData, EdgeData, GraphData>.Edges
        {
            get { return Edges; }
        }
    }

    public class GWNode<NodeData, EdgeData, GraphData> : IGWNode<NodeData, EdgeData, GraphData>
    {
        public GWNode()
        {
        }
        internal GWNode(IGWGraph<NodeData, EdgeData, GraphData> graph)
        {
            Graph = graph;

            switch (Graph.GraphStyle)
            {
                case GraphStyle.Directed:
                    parents = new LinkedList<GWNode<NodeData, EdgeData, GraphData>>();
                    children = new LinkedList<GWNode<NodeData, EdgeData, GraphData>>();
                    inEdges = new LinkedList<GWEdge<NodeData, EdgeData, GraphData>>();
                    outEdges = new LinkedList<GWEdge<NodeData, EdgeData, GraphData>>();
                    break;
                case GraphStyle.Undirected:
                    neighbours = new LinkedList<GWNode<NodeData, EdgeData, GraphData>>();
                    edges = new LinkedList<GWEdge<NodeData, EdgeData, GraphData>>();
                    break;
                default:
                    break;
            }
        }
        internal void AddOutEdge(GWEdge<NodeData, EdgeData, GraphData> edge)
        {
            switch (Graph.GraphStyle)
            {
                case GraphStyle.Directed:
                    outEdges.AddLast(edge);
                    children.Add(edge.Head);
                    break;
                case GraphStyle.Undirected:

                    edges.Add(edge);
                    neighbours.Add(edge.Head);
                    break;
                default:
                    break;
            }
        }
        internal void AddInEdge(GWEdge<NodeData, EdgeData, GraphData> edge)
        {
            switch (Graph.GraphStyle)
            {
                case GraphStyle.Directed:
                    inEdges.Add(edge);
                    parents.Add(edge.Foot);
                    break;
                case GraphStyle.Undirected:

                    edges.Add(edge);
                    neighbours.Add(edge.Foot);
                    break;
                default:
                    break;
            }
        }

        private Guid gwId = Guid.NewGuid();
        public Guid GWId
        {
            get { return gwId; }
            set { gwId = value; }
        }
        public IGWGraph<NodeData, EdgeData, GraphData> Graph { get; set; }
        public int GraphId { get; set; }

        public NodeData Data { get; set; }


        private LinkedList<GWNode<NodeData, EdgeData, GraphData>> parents;
        public IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> Parents
        {
            get { return parents; }
        }

        private LinkedList<GWNode<NodeData, EdgeData, GraphData>> children;
        public IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> Children
        {
            get { return children; }
        }
        private LinkedList<GWNode<NodeData, EdgeData, GraphData>> neighbours;
        public LinkedList<GWNode<NodeData, EdgeData, GraphData>> Neighbours
        {
            get { return neighbours; }
            set { neighbours = value; }
        }

        private LinkedList<GWEdge<NodeData, EdgeData, GraphData>> inEdges;
        public IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> InEdges
        {
            get { return inEdges; }
        }

        private LinkedList<GWEdge<NodeData, EdgeData, GraphData>> outEdges;
        public IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> OutEdges
        {
            get { return outEdges; }
        }

        internal LinkedList<GWEdge<NodeData, EdgeData, GraphData>> edges;
        public LinkedList<GWEdge<NodeData, EdgeData, GraphData>> Edges
        {
            get { return edges; }
            set { edges = value; }
        }

        IGWGraph IGWNode.Graph
        {
            get { return Graph; }
        }

        IEnumerable<IGWNode> IGWNode.Parents
        {
            get { return Parents; }
        }

        IEnumerable<IGWNode> IGWNode.Children
        {
            get { return Children; }
        }

        IEnumerable<IGWNode> IGWNode.Neighbours
        {
            get { return Neighbours; }
        }

        IEnumerable<IGWEdge> IGWNode.InEdges
        {
            get { return InEdges; }
        }

        IEnumerable<IGWEdge> IGWNode.OutEdges
        {
            get { return OutEdges; }
        }

        IEnumerable<IGWEdge> IGWNode.Edges
        {
            get { return Edges; }
        }

        IGWGraph<NodeData, EdgeData, GraphData> IGWNode<NodeData, EdgeData, GraphData>.Graph
        {
            get { return Graph; }
        }

        IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> IGWNode<NodeData, EdgeData, GraphData>.Parents
        {
            get { return Parents; }
        }

        IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> IGWNode<NodeData, EdgeData, GraphData>.Children
        {
            get { return Children; }
        }

        IEnumerable<IGWNode<NodeData, EdgeData, GraphData>> IGWNode<NodeData, EdgeData, GraphData>.Neighbours
        {
            get { return Neighbours; }
        }

        IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> IGWNode<NodeData, EdgeData, GraphData>.InEdges
        {
            get { return InEdges; }
        }

        IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> IGWNode<NodeData, EdgeData, GraphData>.OutEdges
        {
            get { return OutEdges; }
        }

        IEnumerable<IGWEdge<NodeData, EdgeData, GraphData>> IGWNode<NodeData, EdgeData, GraphData>.Edges
        {
            get { return Edges; }
        }
    }

    //public interface IEdgeData
    //{

    //}

    public class GWEdge<NodeData, EdgeData, GraphData> : IGWEdge<NodeData, EdgeData, GraphData>
    {
        public GWEdge()
        {

        }
        public GWEdge(EdgeData data, GWNode<NodeData, EdgeData, GraphData> foot, GWNode<NodeData, EdgeData, GraphData> head)
        {
            Head = head;
            Foot = foot;
            Data = data;
        }
        public GWEdge(GWNode<NodeData, EdgeData, GraphData> foot, GWNode<NodeData, EdgeData, GraphData> head)
        {
            Head = head;
            Foot = foot;
        }
        private Guid gwId = Guid.NewGuid();
        public Guid GWId
        {
            get { return gwId; }
            set { gwId = value; }
        }
        public EdgeData Data { get; set; }

        public GWNode<NodeData, EdgeData, GraphData> Foot { get; set; }
        public GWNode<NodeData, EdgeData, GraphData> Head { get; set; }

        //private IGWGraph<NodeData, EdgeData, GraphData> graph;
        //public IGWGraph Graph
        //{
        //    get { return graph; }
        //}

        IGWNode IGWEdge.Foot
        {
            get { return Foot; }
        }

        IGWNode IGWEdge.Head
        {
            get { return Head; }
        }

        //IGWGraph<NodeData, EdgeData, GraphData> IGWEdge<NodeData, EdgeData, GraphData>.Graph
        //{
        //    get { return graph; }
        //}

        IGWNode<NodeData, EdgeData, GraphData> IGWEdge<NodeData, EdgeData, GraphData>.Foot
        {
            get { return Foot; }
        }

        IGWNode<NodeData, EdgeData, GraphData> IGWEdge<NodeData, EdgeData, GraphData>.Head
        {
            get { return Head; }
        }
    }

    public static class GWGraphX
    {

        public static IGWNode Neighbour(this IGWNode node, IGWEdge edge)
        {
            if (edge.Head.GraphId == node.GraphId)
                return edge.Foot;
            return edge.Head;
        }
        public static IGWNode<NodeData, EdgeData, GraphData> Neighbour<NodeData, EdgeData, GraphData>(this IGWNode<NodeData, EdgeData, GraphData> node, IGWEdge<NodeData, EdgeData, GraphData> edge)
        {
            if (edge.Head.GraphId == node.GraphId)
                return edge.Foot;
            return edge.Head;
        }
        //public static GWNode<NodeData, EdgeData, GraphData> Neighbour<NodeData, EdgeData, GraphData>(this GWNode<NodeData, EdgeData, GraphData> node, GWEdge<NodeData, EdgeData, GraphData> edge)
        //{
        //    if (edge.Head.GraphId == node.GraphId)
        //        return edge.Head;
        //    return edge.Foot;
        //}
        //internal static GWEdge<NodeData, EdgeData, GraphData> CreateEdge<NodeData, EdgeData, GraphData>(IGWNode<NodeData, EdgeData, GraphData> foot, IGWNode<NodeData, EdgeData, GraphData> head, EdgeData data)
        //{
        //    var edge = new GWEdge<NodeData, EdgeData, GraphData>(data, foot, head);

        //    foot.AddOutEdge(edge);
        //    head.AddInEdge(edge);

        //    return edge;
        //}
        //internal static GWEdge<NodeData, EdgeData, GraphData> CreateEdge<NodeData, EdgeData, GraphData>(IGWNode<NodeData, EdgeData, GraphData> foot, IGWNode<NodeData, EdgeData, GraphData> head)
        //{
        //    var edge = new GWEdge<NodeData, EdgeData, GraphData>(foot, head);

        //    foot.AddOutEdge(edge);
        //    head.AddInEdge(edge);

        //    return edge;
        //}

        //public static GWGraph<NodeData2, EdgeData2, GraphData2> Convert<NodeData1, NodeData2, EdgeData1, EdgeData2, GraphData1, GraphData2>(this GWGraph<NodeData1, EdgeData1, GraphData1> originalGraph, Func<NodeData1, NodeData2> nodeDataConverter, Func<EdgeData1, EdgeData2> edgeDataConverter, Func<GraphData1, GraphData2> graphDataConverter)
        //{
        //    var newGraph = new GWGraph<NodeData2, EdgeData2, GraphData2>();
        //    newGraph.Data = graphDataConverter(originalGraph.Data);

        //    var nodesDict = new Dictionary<GWNode<NodeData1, EdgeData1, GraphData1>, GWNode<NodeData2, EdgeData2, GraphData2>>();

        //    foreach (var node in originalGraph.Nodes)
        //    {
        //        var newNode = newGraph.CreateNode();
        //        nodesDict.Add(node, newNode);

        //        newNode.Data = nodeDataConverter(node.Data);
        //    }

        //    foreach (var edge in originalGraph.Edges)
        //    {
        //        var newEdge = newGraph.CreateEdge(nodesDict[edge.Foot], nodesDict[edge.Head]);
        //        newEdge.Data = edgeDataConverter(edge.Data);
        //    }

        //    return newGraph;
        //}
        public static GWGraph<NodeData2, EdgeData2, GraphData2> Convert<NodeData1, NodeData2, EdgeData1, EdgeData2, GraphData1, GraphData2>(this IGWGraph<NodeData1, EdgeData1, GraphData1> originalGraph, Func<IGWNode<NodeData1, EdgeData1, GraphData1>, NodeData2> nodeDataConverter, Func<IGWEdge<NodeData1, EdgeData1, GraphData1>, EdgeData2> edgeDataConverter, Func<IGWGraph<NodeData1, EdgeData1, GraphData1>, GraphData2> graphDataConverter)
        {
            var newGraph = new GWGraph<NodeData2, EdgeData2, GraphData2>();
            newGraph.Data = graphDataConverter(originalGraph);

            var nodesDict = new Dictionary<IGWNode<NodeData1, EdgeData1, GraphData1>, GWNode<NodeData2, EdgeData2, GraphData2>>();

            foreach (var node in originalGraph.Nodes)
            {
                var newNode = newGraph.CreateNode();
                nodesDict.Add(node, newNode);

                newNode.Data = nodeDataConverter(node);
                newNode.GraphId = node.GraphId;
            }

            foreach (var edge in originalGraph.Edges)
            {
                var newEdge = newGraph.CreateEdge(nodesDict[edge.Foot], nodesDict[edge.Head]);
                newEdge.Data = edgeDataConverter(edge);
            }

            return newGraph;
        }
        public static GWGraph<NodeData, EdgeData, GraphData> Clone<NodeData, EdgeData, GraphData>(this IGWGraph<NodeData, EdgeData, GraphData> originalGraph, Func<IGWNode<NodeData, EdgeData, GraphData>, NodeData> nodeDataConverter, Func<IGWEdge<NodeData, EdgeData, GraphData>, EdgeData> edgeDataConverter, Func<IGWGraph<NodeData, EdgeData, GraphData>, GraphData> graphDataConverter)
        {
            var newGraph = new GWGraph<NodeData, EdgeData, GraphData>();
            newGraph.Data = graphDataConverter(originalGraph);

            var nodesDict = new Dictionary<IGWNode<NodeData, EdgeData, GraphData>, GWNode<NodeData, EdgeData, GraphData>>();

            foreach (var node in originalGraph.Nodes)
            {
                var newNode = newGraph.CreateNode();
                nodesDict.Add(node, newNode);

                newNode.Data = nodeDataConverter(node);
                newNode.GraphId = node.GraphId;
            }

            foreach (var edge in originalGraph.Edges)
            {
                var newEdge = newGraph.CreateEdge(nodesDict[edge.Foot], nodesDict[edge.Head]);
                newEdge.Data = edgeDataConverter(edge);
            }

            return newGraph;
        }
    }

    public enum GraphStyle
    {
        Undirected,
        Directed
    }

    public static class GraphX
    {
        public static void Store<NodeData, EdgeData, GraphData>(this IGWGraph<NodeData, EdgeData, GraphData> graph, StreamWriter writer)
            where NodeData : GWStoreable
        {
            writer.WriteLine("graph-start");
            writer.WriteLine("nodes");
            foreach (var node in graph.Nodes)
            {
                StoreX.StoreObj<IGWNode>(node, writer);
                node.Data.Store(writer);
            }
            writer.WriteLine("nodes-end");

            writer.WriteLine("edges");
            foreach (var edge in graph.Edges)
            {
                writer.WriteLine(edge.Foot.GraphId + StoreX.Delimiter + edge.Head.GraphId);
            }
            writer.WriteLine("edges-end");
            writer.WriteLine("graph-end");
            writer.WriteLine("");
        }

        public static IGWGraph<NodeData, EdgeData, GraphData> Load<NodeData, EdgeData, GraphData>(StreamReader reader)
    where NodeData : GWStoreable
        {
            var graph = new GWGraph<NodeData, EdgeData, GraphData>();

            var line = "";
            var inGraphMode = false;
            var inNodesMode = false;
            var inEdgesMode = false;
            while ((line = reader.ReadLine()) != null)
            {
                switch (line)
                {
                    case "graph-start":
                        inGraphMode = true;
                        break;
                    case "graph-end":
                        inGraphMode = false;
                        break;
                    case "nodes":
                        inNodesMode = inGraphMode;
                        break;
                    case "nodes-end":
                        inNodesMode = false;
                        break;
                    case "edges":
                        inEdgesMode = inGraphMode;
                        break;
                    case "edges-end":
                        inEdgesMode = false;
                        break;
                    default:
                        {
                            if (inGraphMode)
                            {
                                if (inNodesMode)
                                {

                                }
                                else if (inEdgesMode)
                                {

                                }
                            }
                        }
                        break;
                }
            }
            //foreach (var node in graph.Nodes)
            //{
            //    StoreX.StoreObj<IGWNode>(node, writer);
            //    node.Data.Store(writer);
            //}

            //writer.WriteLine("edges");
            //foreach (var edge in graph.Edges)
            //{
            //    writer.WriteLine(edge.Foot.GraphId + StoreX.Delimiter + edge.Head.GraphId);
            //}

            return graph;
        }
    }
}
