using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class PDBInfoProvider : IRequestListener
    {
        public IGWContext Context { get; set; }

        public PDBInfoProvider()
        {
            Register();
        }
        public void Register()
        {
            this.DoRegister<ProvidePDBInfo>(OnProvidePDBInfo);
        }

        private void OnProvidePDBInfo(ProvidePDBInfo obj)
        {
            var options = obj.Options;
            var pdbId = options.PDBId;

            var pdbinfo = default(PDBInfo);
            var req = new GetPDBs(pdbId.ToIEnumerable());
            req.RequestInDefaultContext();
            var file = req.Files.FirstOrDefault();
            if (req.Files.NotNullOrEmpty())
            {
                pdbinfo = new PDBInfo(file);
            }

            if (options.ComputeRasa)
            {
                var reqR = new RequestRasa(file);
                reqR.RequestInDefaultContext();
                var rasa = reqR.Rasavalues;
            }

            if (options.ComputeInterface)
            {
                var request = new RequestInterface(pdbinfo.PDBFile.ToIEnumerable(), options.InterfaceDefDistance, options.InterfaceUseVanDerWaalsRadii);
                request.RequestInDefaultContext();
                pdbinfo.Interface.Clear();
                foreach (var residue in pdbinfo.PDBFile.Residues)
                {
                    pdbinfo.Interface.Add(residue, request.Interface[pdbinfo.PDBFile.Name][residue]);
                }
            }

            if (options.ComputeGraph)
            {
                pdbinfo.Graphs = CreateProteinGraph.Do(pdbinfo.PDBFile, options.GraphNeighbourDistance, options.GraphNodeCenterDef);

                if (!(pdbinfo.Interface.Count == 0))
                {
                    foreach (var pgraph in pdbinfo.Graphs)
                    {
                        pgraph.Value.Nodes.Where(n => pdbinfo.Interface[n.Data.Residue]).Each((node) => node.Data.IsRefInterface = true);
                    }
                }
            }

            obj.PDBInfo = pdbinfo;
        }
        public void Unregister()
        {
            this.DoUnregister<ProvidePDBInfo>(OnProvidePDBInfo);
        }
    }

    public class ProvidePDBInfo : IHas<IRequestLogic<ProvidePDBInfo>>
    {
        public static PDBInfo Do(ProvidePDBInfoOptions options, IGWContext context)
        {
            var request = new ProvidePDBInfo(options);
            var ccontext = context as ICRActionLogic<ProvidePDBInfo>;
            if (ccontext != null)
                ccontext.Enter(request);
            else
                request.RequestInDefaultContext();
            return request.PDBInfo;
        }

        public ProvidePDBInfo(ProvidePDBInfoOptions options)
        {
            Options = options;
        }
        public ProvidePDBInfoOptions Options { get; set; }
        public PDBInfo PDBInfo { get; set; }
        public Guid GWId { get; set; }

        private RequestLogic<ProvidePDBInfo> logic = new RequestLogic<ProvidePDBInfo>();
        public IRequestLogic<ProvidePDBInfo> Logic
        {
            get { return logic; }
        }
    }

    public class ProvidePDBInfoOptions
    {
        public ProvidePDBInfoOptions()
        {

        }
        public ProvidePDBInfoOptions(string pdbId)
        {
            PDBId = pdbId;
        }
        public ProvidePDBInfoOptions(string pdbId, bool computeRasa)
        {
            PDBId = pdbId;
            ComputeRasa = computeRasa;
        }
        public ProvidePDBInfoOptions(string pdbId, double interfaceDefDist, bool interfaceUseVanDerWaalsRadii)
        {
            PDBId = pdbId;
            ComputeRasa = true;
            ComputeInterface = true;
            InterfaceDefDistance = interfaceDefDist;
            InterfaceUseVanDerWaalsRadii = interfaceUseVanDerWaalsRadii;
        }
        public ProvidePDBInfoOptions(string pdbId, double interfaceDefDist, bool interfaceUseVanDerWaalsRadii, double graphNeighourDistance, ResidueNodeCenterDefinition nodeCenterDef)
        {
            PDBId = pdbId;
            ComputeRasa = true;
            ComputeInterface = true;
            InterfaceDefDistance = interfaceDefDist;
            InterfaceUseVanDerWaalsRadii = interfaceUseVanDerWaalsRadii;
            ComputeGraph = true;
            GraphNeighbourDistance = graphNeighourDistance;
            GraphNodeCenterDef = nodeCenterDef;
        }
        public string PDBId { get; set; }

        public bool ComputeRasa { get; set; }

        public bool ComputeInterface { get; set; }
        public double InterfaceDefDistance { get; set; }
        public bool InterfaceUseVanDerWaalsRadii { get; set; }

        public bool ComputeGraph { get; set; }
        public double GraphNeighbourDistance { get; set; }

        public ResidueNodeCenterDefinition GraphNodeCenterDef { get; set; }
    }
}
