using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
namespace PPIBase
{
    public class RasaFeature : IHas<IResidueFeatureLogic>
    {
        public const double maxEntry = 1.0;
        public const string Name = "RasaFeature";
        public ProteinGraph Graph { get; set; }

        public Guid GWId { get; set; }

        //public void Compute(ProteinGraph graph)
        //{

        //    Graph = graph;
        //    foreach (ResidueNode node in graph.Nodes)
        //    {
        //        var totalHydrophobicity = Hydrophobicity.GetHydrophobicity(node.Residue.Code);
        //        foreach (var neighbour in node.Neighbours())
        //        {
        //            totalHydrophobicity += Hydrophobicity.GetHydrophobicity(neighbour.Residue.Code);
        //        }
        //        totalHydrophobicity /= (node.Neighbours().Count() + 1);
        //        logic.Values.Add(node.Residue, (totalHydrophobicity + maxEntry) / (2 * maxEntry));
        //    }
        //}

        private ResidueFeatureLogic logic = new ResidueFeatureLogic();
        public IResidueFeatureLogic Logic
        {
            get { return logic; }
        }
    }
}
