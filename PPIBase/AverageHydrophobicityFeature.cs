
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace PPIBase
{
    public class AverageHydrophobicityFeature : IHas<IResidueFeatureLogic>
    {
        public const double maxEntry = 5.0;
        public const string Name = "AverageHydroFeature";

        public Guid GWId { get; set; }
        public ProteinGraph Graph { get; set; }
        public void Compute(ProteinGraph graph)
        {
            Graph = graph;
            foreach (var node in graph.Nodes)
            {
                var totalHydrophobicity = Hydrophobicity.GetHydrophobicity(node.Data.Residue.Code);
                foreach (var neighbour in node.Neighbours)
                {
                    totalHydrophobicity += Hydrophobicity.GetHydrophobicity(neighbour.Data.Residue.Code);
                }
                totalHydrophobicity /= (node.Neighbours.Count() + 1);
                logic.Values.Add(node.Data.Residue, (totalHydrophobicity + maxEntry) / (2 * maxEntry));
            }
        }

        private ResidueFeatureLogic logic = new ResidueFeatureLogic();
        public IResidueFeatureLogic Logic
        {
            get { return logic; }
        }
    }

    public class RasaAverageHydrophobicityFeature : IHas<IResidueFeatureLogic>
    {
        public const double maxEntry = 5.0;
        public const string Name = "RasaAverageHydroFeature";
        public Guid GWId { get; set; }
        public ProteinGraph Graph { get; set; }
        public void Compute(ProteinGraph graph, IDictionary<Residue, double> rasaValues)
        {
            Graph = graph;
            foreach (var node in graph.Nodes)
            {
                var rasasum = rasaValues[node.Data.Residue];
                var totalHydrophobicity = Hydrophobicity.GetHydrophobicity(node.Data.Residue.Code) * rasaValues[node.Data.Residue];
                foreach (var neighbour in node.Neighbours)
                {
                    rasasum += rasaValues[neighbour.Data.Residue];
                    totalHydrophobicity += Hydrophobicity.GetHydrophobicity(neighbour.Data.Residue.Code) * rasaValues[neighbour.Data.Residue];
                }
                totalHydrophobicity /=rasasum;
                logic.Values.Add(node.Data.Residue, (totalHydrophobicity + maxEntry) / (2 * maxEntry));
            }
        }

        private ResidueFeatureLogic logic = new ResidueFeatureLogic();
        public IResidueFeatureLogic Logic
        {
            get { return logic; }
        }
    }
}
