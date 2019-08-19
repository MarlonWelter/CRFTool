using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.Globalization;

namespace PPIBase
{
    public class PDBFile
    {
        public string Name { get; set; }

        public IEnumerable<Atom> Atoms
        {
            get { return Chains.SelectMany(chain => chain.Atoms); }
        }

        public IEnumerable<Residue> Residues
        {
            get { return Chains.SelectMany(chain => chain.Residues); }
        }

        private List<Chain> chains = new List<Chain>();

        public List<Chain> Chains
        {
            get { return chains; }
            set { chains = value; }
        }

        public override string ToString()
        {
            return Name;
        }

    }

    public static class PDBExt
    {

        public static PDBFile Parse(string proteinFile)
        {


            using (var reader = new StreamReader(proteinFile))
            {
                return pdbReadCore(reader);
            }

            //pdb.Chain = pdb.Residues.First().Chain;

            //return null;
        }

        private static PDBFile pdbReadCore(StreamReader reader)
        {
            var pdb = new PDBFile();
            var currentAA = new Residue(pdb);
            var currentChain = new Chain();
            var currentResidue = "";
            var chain = "";
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("ATOM"))
                {
                    var chaintemp = line.Substring(21, 1);
                    if (!chaintemp.Equals(chain))
                    {
                        chain = chaintemp;
                        currentChain = new Chain();
                        currentChain.Name = chain;
                        currentChain.PDB = pdb;
                        pdb.Chains.Add(currentChain);
                    }

                    var residue = line.Substring(22, 5).Trim();
                    if (!residue.Equals(currentResidue))
                    {
                        currentResidue = residue;
                        currentAA = new Residue(pdb);
                        currentChain.Residues.AddLast(currentAA);
                        currentAA.Code = line.Substring(17, 3);
                        currentAA.Id = residue;
                        currentAA.Chain = chain;
                        currentAA.ZScore = double.Parse(line.Substring(60, 6), CultureInfo.InvariantCulture);
                    }

                    var atom = new Atom();
                    atom.Residue = currentAA;
                    atom.Id = line.Substring(6, 5);
                    atom.Name = line.Substring(12, 4).Trim();
                    atom.Element = line.Substring(76, 2).Trim();
                    // read in Zellner score
                    atom.TemperatureFactor = double.Parse(line.Substring(60, 6), CultureInfo.InvariantCulture);

                    //check for CAlpha
                    if (atom.Name.Contains("CA"))
                        currentAA.CAlpha = atom;

                    atom.X = double.Parse(line.Substring(30, 8), CultureInfo.InvariantCulture);
                    atom.Y = double.Parse(line.Substring(38, 8), CultureInfo.InvariantCulture);
                    atom.Z = double.Parse(line.Substring(46, 8), CultureInfo.InvariantCulture);
                    
                    //pdb.Atoms.AddLast(atom);
                    currentAA.Atoms.AddLast(atom);
                }
                else if (line.StartsWith("HEADER"))
                {
                    pdb.Name = line.Substring(62, 4);
                }
            }
            return pdb;
        }

        public static PDBFile Parse(Stream stream)
        {

            using (var reader = new StreamReader(stream))
            {
                return pdbReadCore(reader);
            }

            //pdb.Chain = pdb.Residues.First().Chain;

        }



    }
}
