using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.Input
{
    public class TrainingInputNodeData : ITrainingInputNodeData
    {
        public int Observation { get; set; }
    }

    public class TrainingInputEdgeData : ITrainingInputEdgeData
    {

    }
    public class TrainingInputGraphData : ITrainingInputGraphData
    {
        public double[,] ObservationToCRFScoreProbability { get; set; }
    }
}
