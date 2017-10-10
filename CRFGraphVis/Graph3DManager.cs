using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CRFGraphVis
{
    class Graph3DManager : GWManager<ShowGraph3D>
    {
        protected override void OnRequest(ShowGraph3D request)
        {
            var thread = new Thread(() =>
             {
                 var window = new MainWindow();
                 window.ViewModel.Graph = request.Graph;
                 window.ShowDialog();
             });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = false;
            thread.Start();
        }
    }
}
