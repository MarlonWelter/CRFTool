using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class DoPrediction : IHas<IRequestLogic<DoPrediction>>
    {
        public Guid GWId { get; set; }
        public DoPrediction(PDBFile file, IDictionary<string, ProteinGraph> graphs)
        {
            File = file;
            Graphs = graphs;
        }
        public Dictionary<string, Dictionary<Residue, bool>> Prediction { get; set; }

        public PDBFile File { get; set; }

        public IDictionary<string, ProteinGraph> Graphs { get; set; }

        private IRequestLogic<DoPrediction> logic = new RequestLogic<DoPrediction>();
        public IRequestLogic<DoPrediction> Logic
        {
            get { return logic; }
        }
    }
}
