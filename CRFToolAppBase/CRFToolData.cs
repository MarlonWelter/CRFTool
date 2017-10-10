using CodeBase;
using CRFBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase
{
    public class CRFToolData
    {
        public UseCase UseCase { get; set; }
        public IsingData IsingData { get; set; } = new IsingData();

        public UserTrainingViewModel UserTrainingData { get; set; }

        public List<IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData>> CRFGraphs { get; set; }
        public IGWGraph<ICRFNodeData, ICRFEdgeData, ICRFGraphData> SelectedGraph { get; set; }
    }

    public enum UseCase
    {
        General,
        SoftwareGraph,
        ProteinGraph
    }

    public enum Model
    {
        Ising
    }

    public class ModelData
    {
    }

    public class IsingData : ModelData
    {
        public double CorrelationParameter { get; set; }
        public double ConformityParameter { get; set; }
    }
}
