using CodeBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPIBase
{
    public class PredictionAnalysisVM : INotifyPropertyChanged, IHas<IClassificationResult>
    {
        public Guid GWId { get; set; }
        public PredictionAnalysisVM()
        {
        }

        public PredictionAnalysisVM(IClassificationResult result)
        {
            logic.TruePositives = result.TruePositives;
            logic.TrueNegatives = result.TrueNegatives;
            logic.FalseNegatives = result.FalseNegatives;
            logic.FalsePositives = result.FalsePositives;
        }
        public int TruePositives
        {
            get { return logic.TruePositives; }
            set
            {
                logic.TruePositives = value;
                NotifyPropertyChanged("TruePositives");
                NotifyPropertyChanged("TruePositiveRate");
            }
        }


        public int TrueNegatives
        {
            get { return logic.TrueNegatives; }
            set
            {
                logic.TrueNegatives = value;
                NotifyPropertyChanged("TrueNegatives");
                NotifyPropertyChanged("FalsePositiveRate");
            }
        }


        public int FalsePositives
        {
            get { return logic.FalsePositives; }
            set
            {
                logic.FalsePositives = value;
                NotifyPropertyChanged("FalsePositives");
                NotifyPropertyChanged("FalsePositiveRate");
            }
        }


        public int FalseNegatives
        {
            get { return logic.FalseNegatives; }
            set
            {
                logic.FalseNegatives = value;
                NotifyPropertyChanged("FalseNegatives");
                NotifyPropertyChanged("TruePositiveRate");
            }
        }

        public double TruePositiveRate
        {
            get { return ((double)logic.TruePositives) / (logic.TruePositives + logic.FalseNegatives); }
        }
        public double FalsePositiveRate
        {
            get { return ((double)logic.TrueNegatives) / (logic.TrueNegatives + logic.FalsePositives); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private ClassificationResultLogic logic = new ClassificationResultLogic();
        public IClassificationResult Logic
        {
            get { return logic; }
        }
    }
}
