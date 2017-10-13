using CodeBase;
using CRFBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolApp
{
    public class ShowGraphsManager : GWManager<ShowGraphs>
    {
        protected override void OnRequest(ShowGraphs request)
        {
            var window = new MainView();
            window.ViewModel.Graphs.AddRange(request.Graphs);
            window.ShowDialog();
        }
    }
}
