using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public interface IGraph<NodeType, EdgeType> //
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
    {
        ICollection<NodeType> Nodes { get; }
        ICollection<EdgeType> Edges { get; }
    }

    public interface IGraph<NodeType> where NodeType : IHas<INodeLogic<NodeType>>
    {
        ICollection<NodeType> Nodes { get; }
    }
    public interface INodeLogic2<NodeType> : INodeLogic<NodeType>
        where NodeType : IHas<INodeLogic<NodeType>>
    {
        ICollection<NodeType> Neighbourhood(int depth);
    }
    public interface INodeLogic<NodeType> : INodeLogic
        where NodeType : IHas<INodeLogic<NodeType>>
    {
        new ICollection<NodeType> Neighbours { get; }
    }
    public interface INodeLogic : ILogic
    {
        IEnumerable<IHas<INodeLogic>> Neighbours { get; }

    }

    public class BasicNode : IHas<INodeLogic<BasicNode>>
    {

        private Guid mitId;
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        private NodeLogic<BasicNode> logic = new NodeLogic<BasicNode>();
        public INodeLogic<BasicNode> Logic
        {
            get { return logic; }
        }
    }
    public class Graph<NodeType> : IGraph<NodeType> where NodeType : IHas<INodeLogic<NodeType>>
    {
        private Guid mitId;
        public Guid MitId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        private LinkedList<NodeType> nodes = new LinkedList<NodeType>();
        public ICollection<NodeType> Nodes { get { return nodes; } set { nodes = new LinkedList<NodeType>(value); } }
    }

    public class Graph<NodeType, EdgeType> : IGraph<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
    {
        private Guid mitId;
        public Guid MitId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        private LinkedList<NodeType> nodes = new LinkedList<NodeType>();
        public ICollection<NodeType> Nodes { get { return nodes; } set { nodes = new LinkedList<NodeType>(value); } }

        private LinkedList<EdgeType> edges = new LinkedList<EdgeType>();
        public ICollection<EdgeType> Edges { get { return edges; } set { edges = new LinkedList<EdgeType>(value); } }

    }

    public class NodeLogic<NodeType> : INodeLogic<NodeType> where NodeType : IHas<INodeLogic<NodeType>>
    {
        private Guid mitId;
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }

        private LinkedList<NodeType> neighbours = new LinkedList<NodeType>();
        public ICollection<NodeType> Neighbours
        {
            get { return neighbours; }
            set { neighbours = new LinkedList<NodeType>(value); }
        }

        IEnumerable<IHas<INodeLogic>> INodeLogic.Neighbours
        {
            get { return neighbours.Cast<IHas<INodeLogic>>(); }
        }
    }

    public class NodeLogic2<NodeType> : INodeLogic2<NodeType> where NodeType : IHas<INodeLogic<NodeType>>
    {
        public NodeLogic2()
        {
            neighbours.Add(new LinkedList<NodeType>());
        }

        private Guid mitId;
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }

        private List<LinkedList<NodeType>> neighbours = new List<LinkedList<NodeType>>();
        public ICollection<NodeType> Neighbours
        {
            get { return neighbours[0]; }
            set { neighbours[0] = new LinkedList<NodeType>(value); }
        }

        IEnumerable<IHas<INodeLogic>> INodeLogic.Neighbours
        {
            get { return neighbours.Cast<IHas<INodeLogic>>(); }
        }

        ICollection<NodeType> INodeLogic2<NodeType>.Neighbourhood(int depth)
        {
            if (neighbours.Count < depth)
            {
                return null;
            }
            return neighbours[depth - 1];
        }

        public void AddNb(NodeType node, int depth)
        {
            while (!(neighbours.Count >= depth))
            {
                neighbours.Add(new LinkedList<NodeType>());
            }
            if (depth > 0)
                neighbours[depth - 1].Add(node);
        }
    }

    public interface INodeLogic<NodeType, EdgeType> : INodeLogic<NodeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
    {
        new IEnumerable<NodeType> Neighbours(IHas<INodeLogic<NodeType, EdgeType>> owner);
        IEnumerable<EdgeType> Edges { get; }
        IEnumerable<EdgeType> EdgesIn { get; }
        IEnumerable<EdgeType> EdgesOut { get; }
        void AddEdge(EdgeType edge);
        void RemoveEdge(EdgeType edge);
    }
    public interface IEdgeLogic<NodeType, EdgeType> : ILogic
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
    {
        NodeType Head { get; set; }
        NodeType Foot { get; set; }
    }
    public class NodeLogic<NodeType, EdgeType> : INodeLogic<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
    {
        private Guid mId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mId; }
            set { mId = value; }
        }

        private readonly LinkedList<EdgeType> edges = new LinkedList<EdgeType>();
        public IEnumerable<EdgeType> Edges { get { return edges; } }

        private readonly LinkedList<EdgeType> edgesIn = new LinkedList<EdgeType>();
        public IEnumerable<EdgeType> EdgesIn { get { return edgesIn; } }
        private readonly LinkedList<EdgeType> edgesOut = new LinkedList<EdgeType>();
        public IEnumerable<EdgeType> EdgesOut { get { return edgesOut; } }

        public void AddEdge(EdgeType edge)
        {
            edges.Add(edge);
            if (edge.Head().Logic.Equals(this))
            {
                edgesOut.Add(edge);
                neighbours.Add(edge.Foot());
            }
            else
            {
                edgesIn.Add(edge);
                neighbours.Add(edge.Head());
            }
        }
        public void RemoveEdge(EdgeType edge)
        {
            edges.Remove(edge);
            edgesIn.Remove(edge);
            edgesOut.Remove(edge);
        }

        private LinkedList<NodeType> neighbours = new LinkedList<NodeType>();
        public IEnumerable<NodeType> Neighbours(IHas<INodeLogic<NodeType, EdgeType>> node)
        {
            if (neighbours.Count == 0)
            {
                neighbours.AddRange(Edges.Select(e => node.Neighbour(e)));
            }
            return neighbours;
        }

        ICollection<NodeType> INodeLogic<NodeType>.Neighbours
        {
            get { return neighbours; }
        }

        IEnumerable<IHas<INodeLogic>> INodeLogic.Neighbours
        {
            get { return neighbours.Cast<IHas<INodeLogic>>(); }
        }
    }
    public class EdgeLogic<NodeType, EdgeType> : IEdgeLogic<NodeType, EdgeType>
        where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
        where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
    {
        private Guid mId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mId; }
            set { mId = value; }
        }
        public NodeType Head { get; set; }
        public NodeType Foot { get; set; }
    }
    public class SimpleEdgeData : IEdge3DInfo
    {
        public double Weight { get; set; }
    }
    public class SimpleEdge<NodeType> : IHas<IEdgeLogic<NodeType, SimpleEdge<NodeType>>>
        where NodeType : IHas<INodeLogic<NodeType, SimpleEdge<NodeType>>>
    {
        public SimpleEdge(NodeType head, NodeType foot)
        {
            this.Connect(head, foot);
        }
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }

        private EdgeLogic<NodeType, SimpleEdge<NodeType>> logic = new EdgeLogic<NodeType, SimpleEdge<NodeType>>();
        public IEdgeLogic<NodeType, SimpleEdge<NodeType>> Logic { get { return logic; } }
    }
    public static class GraphExtensions
    {
        public static IEnumerable<EdgeType> Edges<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            return node.Logic.Edges;
        }
        public static IEnumerable<EdgeType> ParentalEdges<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            return node.Logic.EdgesIn;
        }
        public static IEnumerable<EdgeType> ChildrenEdges<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            return node.Logic.EdgesOut;
        }
        public static IEnumerable<NodeType> Parents<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            return node.Logic.EdgesIn.Select<EdgeType, NodeType>((e) => { return e.Head(); });
        }
        public static IEnumerable<NodeType> Children<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            return node.Logic.EdgesOut.Select((e) => { return e.Foot(); });
        }
        public static void AddEdge<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node, EdgeType edge)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            node.Logic.AddEdge(edge);
        }
        public static void RemoveEdge<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node, EdgeType edge)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            node.Logic.RemoveEdge(edge);
        }
        public static NodeType Head<NodeType, EdgeType>(this IHas<IEdgeLogic<NodeType, EdgeType>> edge)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            return edge.Logic.Head;
        }
        public static NodeType Foot<NodeType, EdgeType>(this IHas<IEdgeLogic<NodeType, EdgeType>> edge)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            return edge.Logic.Foot;
        }
        public static NodeType Neighbour<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node, IHas<IEdgeLogic<NodeType, EdgeType>> edge)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            if (edge.Head().Equals(node))
                return edge.Foot();
            if (edge.Foot().Equals(node))
                return edge.Head();
            return default(NodeType);
        }
        public static IEnumerable<NodeType> Neighbours<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            return node.Logic.Neighbours(node);
        }
        public static IEnumerable<NodeType> Neighbourhood<NodeType>(this IHas<INodeLogic2<NodeType>> node, int depth)
            where NodeType : IHas<INodeLogic<NodeType>>
        {
            return node.Logic.Neighbourhood(depth);
        }
        public static ICollection<NodeType> Neighbours<NodeType>(this IHas<INodeLogic<NodeType>> node)
            where NodeType : IHas<INodeLogic<NodeType>>
        {
            return node.Logic.Neighbours;
        }
        //public static IEnumerable<IHas<INodeLogic>> Neighbours(this IHas<INodeLogic> node)
        //{
        //    return node.Logic.Neighbours;
        //}
        public static LinkedList<NodeType> Neighbourhood<NodeType, EdgeType>(this IHas<INodeLogic<NodeType, EdgeType>> node)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            var nbhood = new LinkedList<NodeType>();
            nbhood.AddRange(node.Neighbours());
            foreach (var nb in node.Logic.Neighbours(node))
            {
                nbhood.AddRange(nb.Neighbours().Where(nbnb => !nbhood.Contains(nbnb) && !nbnb.Equals(node)));
            }
            return nbhood;
        }

        public static void Connect<NodeType, EdgeType>(this EdgeType edge, NodeType head, NodeType foot)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            edge.Logic.Head = head;
            edge.Logic.Foot = foot;
            head.Logic.AddEdge(edge);
            foot.Logic.AddEdge(edge);
        }
        public static void Connect<NodeType>(this NodeType head, NodeType foot)
            where NodeType : IHas<INodeLogic<NodeType>>
        {
            head.Neighbours().Add(foot);
            foot.Neighbours().Add(head);
        }
        public static void Disconnect<NodeType, EdgeType>(this EdgeType edge)
            where NodeType : IHas<INodeLogic<NodeType, EdgeType>>
            where EdgeType : IHas<IEdgeLogic<NodeType, EdgeType>>
        {
            var oldhead = edge.Logic.Head;
            if (oldhead != null)
            {
                oldhead.RemoveEdge(edge);
            }
            edge.Logic.Head = default(NodeType);

            var oldfoot = edge.Logic.Foot;
            if (oldfoot != null)
            {
                oldfoot.RemoveEdge(edge);
            }
            edge.Logic.Foot = default(NodeType);
        }
    }


    public interface ITreeNodeLogic<T> : ILogic
    {
        T Parent { get; }
        IEnumerable<T> Children { get; }
        void ConnectChild(T child);
        void DisconnectChild(T child);
    }

    public static class TreeNodeX
    {
        public static void SubstituteSubTree<T>(T nodeToSubstitute, T substitutionNode) where T : IHas<ITreeNodeLogic<T>>
        {
            var parent = nodeToSubstitute.Parent();
            parent.DisconnectChild(nodeToSubstitute);

            parent.ConnectChild(substitutionNode);
        }
        public static LinkedList<T> CollectSubTree<T>(this T node, LinkedList<T> ll = null) where T : IHas<ITreeNodeLogic<T>>
        {
            ll = ll ?? new LinkedList<T>();

            ll.AddLast(node);

            foreach (var child in node.Children())
            {
                CollectSubTree(child, ll);
            }

            return ll;
        }

        public static T Parent<T>(this IHas<ITreeNodeLogic<T>> logicHolder)
        {
            return logicHolder.Logic.Parent;
        }
        public static IEnumerable<T> Children<T>(this IHas<ITreeNodeLogic<T>> logicHolder)
        {
            return logicHolder.Logic.Children;
        }
        public static void ConnectChild<T>(this IHas<ITreeNodeLogic<T>> logicHolder, T child)
        {
            logicHolder.Logic.ConnectChild(child);
        }
        public static void DisconnectChild<T>(this T logicHolder, T child) where T : IHas<ITreeNodeLogic<T>>
        {
            logicHolder.Logic.DisconnectChild(child);
        }
    }
}
