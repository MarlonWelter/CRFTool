using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFBase.OLM
{
    public class ShowOLMHistory : GWRequest<ShowOLMHistory>
    {
        public OLMRequestResult OLMRequestResult { get; set; }
    }
}
