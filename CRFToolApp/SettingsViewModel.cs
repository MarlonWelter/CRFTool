using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRFToolApp
{
    public class SettingsViewModel : INotifyPropertyChanged
    {

        private string exportFolder;

        public string ExportFolder
        {
            get { return exportFolder; }
            set
            {
                exportFolder = value;
                NotifyPropertyChanged(nameof(ExportFolder));
            }
        }

        private int viterbiHeuristicFilterSize;

        public int ViterbiHeuristicFilterSize
        {
            get { return viterbiHeuristicFilterSize; }
            set
            {
                viterbiHeuristicFilterSize = value;
                NotifyPropertyChanged(nameof(ViterbiHeuristicFilterSize));
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
    }
}
