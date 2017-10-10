using CodeBase;
using CRFBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CRFGraphVis
{
    class OLMResultManager : GWManager<ShowOLMResult>
    {
        protected override void OnRequest(ShowOLMResult request)
        {
            var thread = new Thread(() =>
            {
                var window = new OLMResultWindow();
                window.ViewModel = new OLMResultViewModel() { EvalResults = request.OlmResults };
                window.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = false;
            thread.Start();
        }
    }
}
