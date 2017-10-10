
using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class RequestInterface : IHas<IRequestLogic<RequestInterface>>
    {
        public Guid GWId { get; set; }
        public RequestInterface(IEnumerable<PDBFile> files, double distance, bool useVanDerWaalsRadii)
        {
            Distance = distance;
            UseVanDerWaalsRadii = useVanDerWaalsRadii;
            PDBs = files;
            Interface = new Dictionary<string, IDictionary<Residue, bool>>();
        }
        public IEnumerable<PDBFile> PDBs { get; set; }

        public double Distance { get; set; }

        public bool UseVanDerWaalsRadii { get; set; }

        public Dictionary<string, IDictionary<Residue, bool>> Interface { get; set; }

        private RequestLogic<RequestInterface> logic = new RequestLogic<RequestInterface>();
        public IRequestLogic<RequestInterface> Logic
        {
            get { return logic; }
        }
    }
    public class InterfaceInfoProvider : IRequestListener
    {
        public IGWContext Context { get; set; }
        public InterfaceInfoProvider(string location, bool storeNewInterface = true, string fileEnding = "iface")
        {
            Location = location;
            StoreNewInterface = storeNewInterface;
            FileEnding = fileEnding;
            Register();
        }
        public string Location { get; set; }
        public string FileEnding { get; set; }

        public bool StoreNewInterface { get; set; }
        public void Register()
        {
            this.DoRegister<RequestInterface>(Do);
        }
        public void Unregister()
        {
            this.DoUnregister<RequestInterface>(Do);
        }
        public void Do(RequestInterface request)
        {
            foreach (var pdb in request.PDBs)
            {
                var Iface = new Dictionary<Residue, bool>();
                pdb.Residues.Each(r => Iface.Add(r, false));
                var filename = Location + pdb.Name + "_" + request.Distance + "_" + request.UseVanDerWaalsRadii + "." + FileEnding;
                if (Directory.GetFiles(Location).Contains(filename))
                {
                    using (var reader = new StreamReader(filename))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            var residue = pdb.Residues.First(res => (res.Chain + "_" + res.Id).Equals(line));
                            Iface[residue] = true;
                        }
                    }
                }
                else
                {
                    var comprequest = new ComputeInterface(pdb, request.Distance, request.UseVanDerWaalsRadii);
                    comprequest.RequestInDefaultContext();
                    var result = comprequest.Result;

                    using (var writer = new StreamWriter(Location + pdb.Name + "_" + request.Distance + "_" + request.UseVanDerWaalsRadii + "." + FileEnding))
                    {
                        foreach (var entry in result)
                        {
                            foreach (var residue in entry.Value)
                            {
                                writer.WriteLine(residue.Chain + "_" + residue.Id);
                                Iface[residue] = true;
                            }
                        }
                    }
                }
                request.Interface.Add(pdb.Name, Iface);
            }
        }

    }
}
