using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace PPIBase
{
    public class CreateProteinGraph
    {
        public static Dictionary<string, ProteinGraph> Do(PDBFile file, double neighbourDistance, ResidueNodeCenterDefinition centerdef)
        {
            var dict = new Dictionary<string, ProteinGraph>();
            foreach (var chain in file.Chains)
            {

                var graph = new ProteinGraph();
                graph.Data = new ProteinGraphData();

                graph.Data.PDBFile = file;
                graph.Data.NeighbourDistance = neighbourDistance;
                graph.Data.NodeCenterDefinition = centerdef;

                //Compute Nodes:
                foreach (var residue in chain.Residues)
                {
                    if (residue.CAlpha != null && residue.ZScore >= 0)
                    {
                        var node = graph.CreateNode();
                        node.Data = new ResidueNodeData(residue);
                        node.Data.IsCore = residue.IsCore;
                        node.Data.ZScore = residue.ZScore;
                        switch (centerdef)
                        {
                            case ResidueNodeCenterDefinition.CAlpha:
                                node.Data.SetValuesTo(residue.CAlpha);
                                break;
                            case ResidueNodeCenterDefinition.BalancePoint:
                                node.Data.SetValuesTo(residue.Atoms.BalancePoint());
                                break;
                            default:
                                break;
                        }
                    }
                }

                var nodeList = graph.Nodes.ToList();
                //compute edges:
                for (int i = 0; i < nodeList.Count - 1; i++)
                {
                    for (int k = i + 1; k < nodeList.Count; k++)
                    {
                        var nodeOne = nodeList[i];
                        var nodetwo = nodeList[k];
                        var dist = nodeOne.Data.Distance(nodetwo.Data);
                        if (dist <= neighbourDistance)
                        {
                            var edge = graph.CreateEdge(nodeOne, nodetwo);
                            edge.Data = new SimpleEdgeData();
                        }
                    }
                }

                dict.Add(chain.Name, graph);
            }
            return dict;
        }
        //public static Dictionary<string, ProteinGraph> Do(PDBFile file, double neighbourDistance, ResidueNodeCenterDefinition centerdef)
        //{
        //    var dict = new Dictionary<string, ProteinGraph>();
        //    foreach (var chain in file.Chains)
        //    {

        //        var graph = new ProteinGraph();

        //        graph.PDBFile = file;
        //        graph.NeighbourDistance = neighbourDistance;
        //        graph.NodeCenterDefinition = centerdef;

        //        //Compute Nodes:
        //        foreach (var residue in chain.Residues)
        //        {
        //            var node = new ResidueNode(residue, graph);
        //            switch (centerdef)
        //            {
        //                case ResidueNodeCenterDefinition.CAlpha:
        //                    node.SetValuesTo(residue.CAlpha);
        //                    break;
        //                case ResidueNodeCenterDefinition.BalancePoint:
        //                    node.SetValuesTo(residue.Atoms.BalancePoint());
        //                    break;
        //                default:
        //                    break;
        //            }
        //            graph.Nodes.Add(node);
        //        }

        //        var nodeList = graph.Nodes.ToList();
        //        //compute edges:
        //        for (int i = 0; i < nodeList.Count - 1; i++)
        //        {
        //            for (int k = i + 1; k < nodeList.Count; k++)
        //            {
        //                var nodeOne = nodeList[i];
        //                var nodetwo = nodeList[k];
        //                var dist = nodeOne.Distance(nodetwo);
        //                if (dist <= neighbourDistance)
        //                {
        //                    var edge = new SimpleEdge<ResidueNode>(nodeOne, nodetwo);
        //                    graph.Edges.Add(edge);
        //                }
        //            }
        //        }

        //        dict.Add(chain.Name, graph);
        //    }
        //    return dict;
        //}
        public static Dictionary<string, IDictionary<string, ProteinGraph>> Do(IEnumerable<PDBFile> files, double neighbourDistance, ResidueNodeCenterDefinition centerdef)
        {
            var dict = new Dictionary<string, IDictionary<string, ProteinGraph>>();
            foreach (var file in files)
            {
                dict.Add(file.Name, Do(file, neighbourDistance, centerdef));
            }
            return dict;
        }
    }
    public enum ResidueNodeCenterDefinition
    {
        CAlpha,
        BalancePoint
    }
}
