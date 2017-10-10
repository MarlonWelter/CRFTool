using CodeBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PPIBase
{
    public class RequestRasa : IHas<IRequestLogic<RequestRasa>>
    {
        public RequestRasa(PDBFile file)
        {
            File = file;
            Rasavalues = new Dictionary<Residue, double>();
        }
        public Guid GWId { get; set; }
        public PDBFile File { get; set; }
        public Dictionary<Residue, double> Rasavalues { get; set; }

        private RequestLogic<RequestRasa> logic = new RequestLogic<RequestRasa>();
        public IRequestLogic<RequestRasa> Logic
        {
            get { return logic; }
        }
    }


    public class RasaManager : IRequestListener
    {
        private Dictionary<PDBFile, Dictionary<Residue, double>> buffer = new Dictionary<PDBFile, Dictionary<Residue, double>>();

        public Dictionary<PDBFile, Dictionary<Residue, double>> Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        public string PDBFiles { get; set; }
        public string RasaFiles { get; set; }
        public IGWContext Context { get; set; }
        public RasaManager(string rasaFiles, string pdbfiles)
        {
            RasaFiles = rasaFiles;
            PDBFiles = pdbfiles;
            Register();
        }
        public void Register()
        {
            Directory.CreateDirectory(RasaFiles);
            this.DoRegister<RequestRasa>(OnRequest);
        }
        public void Unregister()
        {
            this.DoUnregister<RequestRasa>(OnRequest);
        }

        private void OnRequest(RequestRasa obj)
        {
            if (buffer.ContainsKey(obj.File))
            {
                obj.Rasavalues = new Dictionary<Residue, double>(buffer[obj.File]);
            }
            else
            {
                foreach (var chain in obj.File.Chains)
                {
                    var rasaFile = Directory.GetFiles(RasaFiles).FirstOrDefault(file => file.Contains(obj.File.Name + "_" + chain.Name));
                    if (rasaFile == null)
                    {

                        //compute Rasa:
                        var processInfo = new ProcessStartInfo("java", " -jar protein-pdb-asa.jar -pdbfile "
                            + PDBFiles + obj.File.Name + "_" + chain.Name + ".pdb" + " -pdb " + obj.File.Name + " -chain " + chain.Name + " -rasafile "
                            + RasaFiles + obj.File.Name + "_" + chain.Name + ".rasa")
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };
                        Process proc;

                        if ((proc = Process.Start(processInfo)) == null)
                        {
                            throw new InvalidOperationException("??");
                        }

                        proc.WaitForExit();
                        int exitCode = proc.ExitCode;
                        proc.Close();
                    }
                    //else
                    {
                        rasaFile = rasaFile ?? Directory.GetFiles(RasaFiles).FirstOrDefault(file => file.Contains(obj.File.Name + "_" + chain.Name));
                        using (var reader = new StreamReader(rasaFile))
                        {
                            string line = "";
                            while ((line = reader.ReadLine()) != null)
                            {
                                var words = Regex.Split(line, "\\s");
                                var nodeid = words[0].Substring(7);
                                var rasa = Math.Min(1.0, double.Parse(words[3], CultureInfo.InvariantCulture));
                                var residue = chain.Residues.First(res => res.Id.Equals(nodeid));
                                obj.Rasavalues.Add(residue, rasa);
                            }
                        }
                    }
                }
                buffer.Add(obj.File, new Dictionary<Residue, double>(obj.Rasavalues));
            }
        }
    }
}
