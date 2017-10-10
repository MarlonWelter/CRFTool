using CRFBase;
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class ProteinGraphData
    {
        public ResidueNodeCenterDefinition NodeCenterDefinition { get; set; }
        public double NeighbourDistance { get; set; }
        public PDBFile PDBFile { get; set; }
    }
    
    public class ProteinGraph : GWGraph<ResidueNodeData, SimpleEdgeData, ProteinGraphData>
    {

    }

    //public class ProteinGraph : IGraph<ResidueNode, SimpleEdge<ResidueNode>>
    //{
    //    public ResidueNodeCenterDefinition NodeCenterDefinition { get; set; }
    //    public double NeighbourDistance { get; set; }

    //    private LinkedList<ResidueNode> residues = new LinkedList<ResidueNode>();
    //    private LinkedList<SimpleEdge<ResidueNode>> edges = new LinkedList<SimpleEdge<ResidueNode>>();
    //    public PDBFile PDBFile { get; set; }


    //    public ICollection<ResidueNode> Nodes
    //    {
    //        get { return residues; }
    //    }

    //    public ICollection<SimpleEdge<ResidueNode>> Edges
    //    {
    //        get { return edges; }
    //    }
    //}
    public static class ProtGraphX
    {
        public static GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> CreateGraph(this ProteinGraph graph, IDictionary<ResidueNodeData, double[]> nodescores, IDictionary<SimpleEdgeData, double[,]> edgescores)
        {
            var clonegraph = new GWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>();

            foreach (var node in graph.Nodes)
            {
                var clone = clonegraph.CreateNode();
                clone.Data = new CRFNodeData() { Id = node.Data.Residue.Id, Scores = nodescores[node.Data] };
                clonegraph.Nodes.Add(clone);
            }
            foreach (var edge in graph.Edges)
            {
                var newEdge = new GWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData>(clonegraph.Nodes.First(n => n.Data.Id.Equals(edge.Head.Data.Residue.Id)),
                     clonegraph.Nodes.First(n => n.Data.Id.Equals(edge.Foot.Data.Residue.Id)));
                newEdge.Data = new CRFEdgeData();
                newEdge.Data.Scores = edgescores[edge.Data];
                clonegraph.Edges.Add(newEdge);
            }

            return clonegraph;
        }

    }

}
