using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CRFBase
{
    public class SeedingMethodPatchCreation
    {
        private static Random rdm = new Random();
        // in dieser Variablen werden alle Knoten gespeichert, die zu den erzeugten patches gehören.
        List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> patchNodes = new List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
        List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> border = new List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
        List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> nodes;   // outer
        List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> inner = new List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>();
        List<IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>[] outsideEdges;

        public SeedingMethodPatchCreation(int numberOfSeeds, int totalPatchesSize)
        {
            NumberOfSeeds = numberOfSeeds;
            TotalPatchesSize = totalPatchesSize;
        }
        public int NumberOfSeeds { get; set; }
        public int TotalPatchesSize { get; set; }

        public void CreatePatchAndSetAsReferenceLabel(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            var patch = CreatePatches(graph);

            graph.Data.ReferenceLabeling = new int[graph.Nodes.Count()];

            foreach (var node in patch)
            {
                graph.Data.ReferenceLabeling[node.GraphId] = 1;
                node.Data.ReferenceLabel = 1;
            }

        }
        public List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> CreatePatches(IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> graph)
        {
            border.Clear();
            patchNodes.Clear();
            inner.Clear();
            nodes = graph.Nodes.ToList();

            // save every edge for each node
            outsideEdges = new List<IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>[nodes.Count];
            foreach (IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node in nodes)
            {
                outsideEdges[node.GraphId] = new List<IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>(node.Edges);
            }

            //if (false)
            //{
            //    var graph3D = graph.Wrap3D(nd => new Node3DWrap<ICRFNodeData>(nd.Data) { ReferenceLabel = patchNodes.Any(n => n.GraphId == nd.GraphId) ? 1 : 0, X = nd.Data.X, Y = nd.Data.Y, Z = nd.Data.Z }, (ed) => new Edge3DWrap<ICRFEdgeData>(ed.Data) { Weight = 1.0 });
            //    new ShowGraph3D(graph3D).Request();

            //    Thread.Sleep(60000);
            //}
            // Laura: seeds setzen
            for (int i = 0; i < NumberOfSeeds; i++)
            {
                // chose a node randomly
                var chosen = nodes.RandomElement(rdm);
                addNode(chosen);
            }

            // Laura: patches wachsen lassen bis TotalPatchesSize erreicht ist.
            for (int i = NumberOfSeeds; i < TotalPatchesSize; i++)
            {
                // add neighbor from this node out of border to patchNodes
                var node = border.RandomElement(rdm);
                var edge = outsideEdges[node.GraphId].RandomElement(rdm);
                var nb = node.Neighbour(edge);
                addNode(nb);
                if (border.Count == 0)
                    return patchNodes;
            }

            return patchNodes;
        }

        private void checkInner(List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> border, List<IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> inner, List<IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>[] outsideEdges)
        {
            var borderCopy = new List<IGWNode>(border);
            foreach (IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> item in borderCopy)
            {
                if (outsideEdges[item.GraphId].Count == 0)
                {
                    inner.Add(item);
                    border.Remove(item);
                }
            }
        }

        private void updateOutsideEdges(List<IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData>>[] outsideEdges, IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node)
        {
            foreach (IGWEdge<ICRFNodeData, ICRFEdgeData, ICRFGraphData> edge in node.Edges)
            {
                outsideEdges[node.Neighbour(edge).GraphId].Remove(edge);
            }
        }

        private void addNode(IGWNode<ICRFNodeData, ICRFEdgeData, ICRFGraphData> node)
        {
            patchNodes.Add(node);
            border.Add(node);
            nodes.Remove(node);
            // remove outside edges to avoid that one node will be added multiple times
            updateOutsideEdges(outsideEdges, node);
            // inner?
            checkInner(border, inner, outsideEdges);
        }

    }
}
