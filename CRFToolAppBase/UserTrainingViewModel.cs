using CodeBase;
using CRFBase;
using CRFBase.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase
{
    public class UserTrainingViewModel
    {
        public OLMVariant OLMVariant { get; set; }

        public List<IGWGraph<ITrainingInputNodeData, ITrainingInputEdgeData, ITrainingInputGraphData>> TrainingData { get; set; }



    }


}
