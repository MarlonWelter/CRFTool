using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace CRFGraphVis
{
    public class Graph3DViewModel : INotifyPropertyChanged
    {
        private IGWGraph<ICRFNode3DInfo, IEdge3DInfo, object> graph;

        public IGWGraph<ICRFNode3DInfo, IEdge3DInfo, object> Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                NotifyPropertyChanged(nameof(Graph));
                NotifyPropertyChanged(nameof(ViewModel));
            }
        }


        public Graph3DViewModel ViewModel => this;

        private double temperatureValue;

        public double Temperature
        {
            get { return temperatureValue; }
            set
            {
                temperatureValue = value;
                NotifyPropertyChanged(nameof(Temperature));
            }
        }

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

        public Guid GWId
        {
            get;
            set;
        }
    }
}
