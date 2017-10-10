using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class Atom : ICoordinated
    {
        public string Id { get; set; }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Element { get; set; }

        public double TemperatureFactor { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Residue Residue { get; set; }
    }
}
