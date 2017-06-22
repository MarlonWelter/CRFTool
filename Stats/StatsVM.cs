using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class StatsVM : INotifyPropertyChanged
    {
        public StatsVM()
        {


        }

        private List<double> stats = new List<double>();

        public List<double> DataSeries
        {
            get { return stats; }
            set { stats = value; }
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
    }
}
