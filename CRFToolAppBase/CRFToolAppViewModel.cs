using CodeBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolAppBase
{
    public class CRFToolAppViewModel : INotifyPropertyChanged
    {
        private List<IGWGraph<ICRFNode3DInfo, IEdge3DInfo, object>> graphs = new List<IGWGraph<ICRFNode3DInfo, IEdge3DInfo, object>>();

        public List<IGWGraph<ICRFNode3DInfo, IEdge3DInfo, object>> Graphs
        {
            get { return graphs; }
            set
            {
                graphs = value;
                NotifyPropertyChanged(nameof(Graphs));
                NotifyPropertyChanged(nameof(ViewModel));
            }
        }


        public CRFToolAppViewModel ViewModel => this;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
