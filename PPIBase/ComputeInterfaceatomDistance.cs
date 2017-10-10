using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
namespace PPIBase
{
    public class ComputeInterfaceatomDistance
    {
        public static Dictionary<string, LinkedList<Residue>> Do(PDBFile pdb, double distance, bool useVanDerWaalsRadii)
        {
            var iface = new Dictionary<string, LinkedList<Residue>>();

            for (int i = 0; i < pdb.Chains.Count; i++)
            {
                var chain1 = pdb.Chains[i];
                iface.Add(chain1.Name, new LinkedList<Residue>());
            }

            for (int i = 0; i < pdb.Chains.Count; i++)
            {
                var chain1 = pdb.Chains[i];
                var otherchains = pdb.Chains.ToList();
                otherchains.Remove(chain1);
                var allotheratoms = otherchains.SelectMany(chain => chain.Atoms);

                foreach (var residue in chain1.Residues)
                {
                    if (iface[chain1.Name].Contains(residue))
                        continue;

                    bool isInterface = false;
                    foreach (var atom in residue.Atoms)
                    {
                        foreach (var otherchain in otherchains)
                        {

                            foreach (var atomPartner in otherchain.Atoms)
                            {

                                if (isInterface)
                                    break;

                                if (useVanDerWaalsRadii)
                                {
                                    if (atom.Distance(atomPartner) <=
                                         +VanDerWaalsRadii.GetRadius(atom.Element)
                                         + VanDerWaalsRadii.GetRadius(atomPartner.Element) + distance)
                                    {
                                        isInterface = true;
                                        if (!iface[otherchain.Name].Contains(atomPartner.Residue))
                                            iface[otherchain.Name].Add(atomPartner.Residue);
                                    }
                                }
                                else
                                {
                                    if (atom.Distance(atomPartner) <= distance)
                                    {
                                        isInterface = true;
                                        if (!iface[otherchain.Name].Contains(atomPartner.Residue))
                                            iface[otherchain.Name].Add(atomPartner.Residue);
                                    }
                                }

                            }
                        }
                        if (isInterface)
                            iface[chain1.Name].Add(residue);
                    }
                }
            }

            return iface;
        }
    }

    public class ComputeInterfaceHandler : IRequestListener
    {
        public ComputeInterfaceHandler()
        {
            Register();
        }
        public IGWContext Context { get; set; }
        public void Register()
        {
            this.DoRegister<ComputeInterface>(OnRequest);
        }

        private void OnRequest(ComputeInterface obj)
        {
            obj.Result = ComputeInterfaceatomDistance.Do(obj.PDB, obj.Distance, obj.UseVanDerWaalsRadii);
        }
        public void Unregister()
        {
            this.DoUnregister<ComputeInterface>(OnRequest);
        }
    }

    public class ComputeInterface : IHas<IRequestLogic<ComputeInterface>>
    {
        public Guid GWId { get; set; }
        public ComputeInterface(PDBFile pdb, double distance, bool useVanDerWaalsRadii)
        {
            Distance = distance;
            UseVanDerWaalsRadii = useVanDerWaalsRadii;
            PDB = pdb;
        }
        public Dictionary<string, LinkedList<Residue>> Result { get; set; }
        public double Distance { get; set; }
        public bool UseVanDerWaalsRadii { get; set; }
        public PDBFile PDB { get; set; }

        private RequestLogic<ComputeInterface> logic = new RequestLogic<ComputeInterface>();
        public IRequestLogic<ComputeInterface> Logic
        {
            get { return logic; }
        }
    }
}

