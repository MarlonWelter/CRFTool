using CodeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class PDBFileManager : IRequestListener
    {
        public PDBFileManager(string baseLocation)
        {
            BaseDirectory = baseLocation;
            Register();
        }
        public string BaseDirectory { get; set; }

        private LinkedList<PDBFile> filesCache = new LinkedList<PDBFile>();
        public IGWContext Context { get; set; }
        public void Register()
        {
            this.DoRegister<GetPDBs>(OnGetPDB);
        }

        private void OnGetPDB(GetPDBs obj)
        {
            foreach (var pdbname in obj.PDBNames)
            {
                PDBFile pdb;

                pdb = filesCache.FirstOrDefault(file => file.Name.Equals(pdbname));

                if (pdb != null)
                {
                    obj.Files.Add(pdb);
                    continue;
                }

                var pdbfile = Directory.GetFiles(BaseDirectory).FirstOrDefault(file => file.Contains(pdbname + ".pdb"));
                if (pdbfile != null)
                {
                    pdb = PDBExt.Parse(pdbfile);
                    obj.Files.Add(pdb);
                    filesCache.Add(pdb);
                    continue;
                }

                new DownLoadPDB(pdbname.ToIEnumerable()).RequestInDefaultContext();

                pdbfile = Directory.GetFiles(BaseDirectory).FirstOrDefault(file => file.Contains(pdbname + ".pdb"));
                if (pdbfile == null)
                {
                    new NotifyUser("couldnt get pdb " + pdbname);
                    Log.Post("couldnt get pdb " + pdbname);
                }
                else
                {
                    pdb = PDBExt.Parse(pdbfile);
                    obj.Files.Add(pdb);
                    filesCache.Add(pdb);
                }
            }
        }

        public void Unregister()
        {
            this.DoUnregister<GetPDBs>(OnGetPDB);
        }
    }

    public class GetPDBs : IHas<IRequestLogic<GetPDBs>>
    {
        public Guid GWId { get; set; }
        public GetPDBs(IEnumerable<string> pdbs)
        {
            PDBNames = pdbs;
            Files = new LinkedList<PDBFile>();
        }

        public LinkedList<PDBFile> Files { get; set; }

        public IEnumerable<string> PDBNames { get; set; }

        private RequestLogic<GetPDBs> logic = new RequestLogic<GetPDBs>();
        public IRequestLogic<GetPDBs> Logic
        {
            get { return logic; }
        }
    }
}
