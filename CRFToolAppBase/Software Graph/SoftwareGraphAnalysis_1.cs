using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase.Software_Graph
{
    /*
     *  model: Ising
     *  pre trained data for conformity and correlation
     * 
     * */
    public class SoftwareGraphAnalysis_1 : GWRequest<SoftwareGraphAnalysis_1>
    {
        public SoftwareGraphAnalysis_1(string graphInputFolder, string outputFolder, double conformityParameter, double correlationParameter, int viterbiHeuristicMemoryParameter)
        {
            GraphInputFolder = graphInputFolder;
            OutputFolder = outputFolder;
            CorrelationParameter = correlationParameter;
            ConformityParameter = conformityParameter;
            ViterbiHeuristicMemoryParameter = viterbiHeuristicMemoryParameter;
        }
        public double ConformityParameter { get; set; }
        public double CorrelationParameter { get; set; }
        public string GraphInputFolder { get; set; }
        public string OutputFolder { get; set; }
        public int ViterbiHeuristicMemoryParameter { get; set; }

    }
}
