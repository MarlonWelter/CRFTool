using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class ResidueNodeData : ICoordinated, ICRFNode3DInfo
    {
        public ResidueNodeData(Residue residue)
        {
            Residue = residue;
        }
        public Residue Residue { get; set; }
        public bool IsCore { get; set; }
        public bool IsRefInterface { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double ZScore { get; set; }

        public int CommunityId { get; set; }

        public int Labelling { get; set; }
        public int ReferenceLabel { get; set; }
    }

    public class ResidueNode : IHas<NodeLogic<ResidueNode, SimpleEdge<ResidueNode>>>, ICoordinated
    {
        public ResidueNode(Residue residue, ProteinGraph graph)
        {
            Graph = graph;
            Residue = residue;
        }
        public Residue Residue { get; set; }
        public bool IsCore { get; set; }
        public bool IsRefInterface { get; set; }

        public Guid GWId { get; set; }

        public ProteinGraph Graph { get; set; }

        private NodeLogic<ResidueNode, SimpleEdge<ResidueNode>> logic = new NodeLogic<ResidueNode, SimpleEdge<ResidueNode>>();
        public NodeLogic<ResidueNode, SimpleEdge<ResidueNode>> Logic
        {
            get { return logic; }
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
