using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase
{
    public class ShowOLMResult : GWRequest<ShowOLMResult>
    {
        public ShowOLMResult(OLMEvaluationResult olmResult)
        {
            OlmResults = olmResult.ToIEnumerable().ToList();
        }
        public ShowOLMResult(List<OLMEvaluationResult> olmResults)
        {
            OlmResults = olmResults;
        }
        public List<OLMEvaluationResult> OlmResults { get; set; }
    }
}
