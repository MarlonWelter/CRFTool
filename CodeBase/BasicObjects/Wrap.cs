using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class Wrap<T> : INotifyPropertyChanged
        where T : INotifyPropertyChanged
    {
        private T data;

        public T Data
        {
            get { return data; }
            set
            {
                if (data != null)
                    data.PropertyChanged -= OnPropChanged;
                data = value;
                if (data != null)
                    data.PropertyChanged += OnPropChanged;
            }
        }

        private void OnPropChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("Data");
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion


    }
}
