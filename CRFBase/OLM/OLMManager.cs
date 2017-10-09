using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.OLM
{
    class OLMManager : GWManager<OLMRequest>
    {
        protected override void OnRequest(OLMRequest request)
        {
            switch (request.Variante)
            {
                case OLMVariant.Default:
                    var olm = new OLM_III<ICRFNodeData, ICRFEdgeData, ICRFGraphData>(request.NumberLabels, request.BufferSizeCRF, request.BasisMerkmale, request.LossFunctionIteration, request.LossFunctionValidation, 0.02, "OLM_Default");
                    olm.Do(request.BasisMerkmale.Length, request.Graphs, request.MaxIterations, request);
                    request.Result = new OLMRequestResult(olm.ResultingWeights.ToArray());
                    break;
                case OLMVariant.AverageDistCriteria:
                    break;
                case OLMVariant.LastTwoMax:
                    break;
                case OLMVariant.Test: // this is a dummy for testing purposes
                    request.Result = new OLMRequestResult(new double[2] { Build.Random.NextDouble(), Build.Random.NextDouble() });
                    break;
                case OLMVariant.Ising:
                    var olmIsing = new OLM_Ising_I<ICRFNodeData, ICRFEdgeData, ICRFGraphData>(request.NumberLabels, request.BufferSizeCRF, request.BasisMerkmale, request.LossFunctionIteration, request.LossFunctionValidation, 0.02, "OLM_Default");
                    olmIsing.Do(request.BasisMerkmale.Length, request.Graphs, request.MaxIterations, request);
                    request.Result = new OLMRequestResult(olmIsing.ResultingWeights.ToArray());
                    break;
                default:
                    Log.Post("unknown OLM variante", LogCategory.Critical);
                    break;
            }
        }
    }
}
