
using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public interface IResidueFeatureLogic : ILogic
    {
        double ValueOf(Residue residue);
    }
    public class ResidueFeatureLogic : IResidueFeatureLogic
    {
        public Guid GWId { get; set; }
        private Dictionary<Residue, double> values = new Dictionary<Residue, double>();
        public Dictionary<Residue, double> Values
        {
            get { return values; }
        }
        public double ValueOf(Residue residue)
        {
            return values[residue];
        }
    }

    public static class FeatureLogicExtension
    {
        public static double ValueOf(this IHas<IResidueFeatureLogic> logicHolder, Residue residue)
        {
            return logicHolder.Logic.ValueOf(residue);
        }
    }
}
