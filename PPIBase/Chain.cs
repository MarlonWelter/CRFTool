using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class Chain
    {
        public string Name { get; set; }
        public PDBFile PDB { get; set; }

        private LinkedList<Residue> residues = new LinkedList<Residue>();

        public LinkedList<Residue> Residues
        {
            get { return residues; }
            set { residues = value; }
        }

        public IEnumerable<Atom> Atoms
        {
            get { return Residues.SelectMany(residue => residue.Atoms); }
        }
    }
}
