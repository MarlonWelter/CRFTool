using CodeBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PPIBase
{
    public class PDBBaseViewModel : INotifyPropertyChanged
    {
        public const string PropPDBFiles = "PDBFiles";
        private ObservableList<PDBFile> pdbfiles = new ObservableList<PDBFile>();
        public ObservableList<PDBFile> PDBFiles
        {
            get { return pdbfiles; }
        }

        public void AddPDB(params PDBFile[] files)
        {
            pdbfiles.AddRange(files);
            //NotifyPropertyChanged("PDBFiles");
        }
        public void AddPDBs(IEnumerable<PDBFile> files)
        {
            pdbfiles.AddRange(files);
            //NotifyPropertyChanged("PDBFiles");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}
