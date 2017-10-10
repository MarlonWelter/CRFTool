using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.Text.RegularExpressions;
using System.Net;

namespace PPIBase
{
    public class DownloadPDBHandler : IRequestListener
    {
        public DownloadPDBHandler(string storagePath, string fileEnding = "pdb")
        {
            StoragePath = storagePath;
            FileEnding = fileEnding;
            Register();
        }
        public IGWContext Context { get; set; }
        const string basicstring = @"http://pdb.org/pdb/download/downloadFile.do?fileFormat=pdb&compression=NO&structureId=";

        public string StoragePath { get; set; }
        public string FileEnding { get; set; }

        private void DownnloadPdb(DownLoadPDB request)
        {
            foreach (var pdb in request.PDBs)
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        Stream data = client.OpenRead(basicstring + pdb);

                        using (TextReader reader = new StreamReader(data))
                        {
                            using (TextWriter writer = new StreamWriter(StoragePath + pdb + "." + FileEnding))
                            {
                                string line = string.Empty;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    writer.WriteLine(line);
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        public void Register()
        {
            this.DoRegister<DownLoadPDB>(DownnloadPdb);
        }

        public void Unregister()
        {
            this.DoUnregister<DownLoadPDB>(DownnloadPdb);
        }

    }

    public class DownLoadPDB : IHas<IRequestLogic<DownLoadPDB>>
    {
        public Guid GWId { get; set; }
        public DownLoadPDB(IEnumerable<string> proteins)
        {
            PDBs = proteins;
        }
        public IEnumerable<string> PDBs { get; set; }
        private RequestLogic<DownLoadPDB> logic = new RequestLogic<DownLoadPDB>();
        public IRequestLogic<DownLoadPDB> Logic
        {
            get { return logic; }
        }
    }
}
