using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class AminoAcid
    {
        public AminoAcid()
        {

        }
        public string Code { get; set; }
        private LinkedList<Atom> atoms = new LinkedList<Atom>();

        public LinkedList<Atom> Atoms
        {
            get { return atoms; }
            set { atoms = value; }
        }
        public Atom CAlpha { get; set; }

    }

    public class Residue : AminoAcid
    {
        public Residue(PDBFile pdb)
        {
            PDB = pdb;
        }
        public string Id { get; set; }
        public string Chain { get; set; }

        public PDBFile PDB { get; set; }

        public bool IsCore { get; set; }

        public double ZScore { get; set; }
    }
}
