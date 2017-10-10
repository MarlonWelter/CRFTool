using CodeBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class PredictorOverviewVM : INotifyPropertyChanged, IRequestListener
    {
        public IGWContext Context { get; set; }
        public PredictorOverviewVM()
        {
            Register();
            this.DoRequest(new Created<PredictorOverviewVM>(this));

        }

        public void Register()
        {
            this.DoRegister<Created<IHas<IPredictionLogic>>>(OnPredictorCreated);
        }
        public void Unregister()
        {
            this.DoUnregister<Created<IHas<IPredictionLogic>>>(OnPredictorCreated);
        }

        private void OnPredictorCreated(Created<IHas<IPredictionLogic>> obj)
        {
            Predictors.Add(obj.Item);
        }

        private ObservableCollection<IHas<IPredictionLogic>> predictors = new ObservableCollection<IHas<IPredictionLogic>>();

        public ObservableCollection<IHas<IPredictionLogic>> Predictors
        {
            get { return predictors; }
            set
            {
                predictors = value;
                NotifyPropertyChanged("Predictors");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
