using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CodeBase
{
    public class Show<ViewModel> : IHas<IRequestLogic<Show<ViewModel>>> 
    {
        private Guid mitId = Guid.NewGuid();
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }	
        public ViewModel VM { get; set; }
        public Show(ViewModel vm)
        {
            VM = vm;
        }

        private RequestLogic<Show<ViewModel>> logic = new RequestLogic<Show<ViewModel>>();
        public IRequestLogic<Show<ViewModel>> Logic { get { return logic; } }
    }
    public interface IViewModel : INotifyPropertyChanged
    {
        void Display(IViewModel model);
    }
    public class ViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
