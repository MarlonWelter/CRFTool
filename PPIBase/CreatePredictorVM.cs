using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.Globalization;

namespace PPIBase
{
    public class CreatePredictorVM : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }


        private double interfaceDist;

        public double InterfaceDist
        {
            get { return interfaceDist; }
            set
            {
                interfaceDist = value;
                NotifyPropertyChanged("InterfaceDist");
            }
        }


        public string InterfaceDistString
        {
            get { return InterfaceDist.ToString(); }
            set
            {
                try
                {
                    double res = double.Parse(value, CultureInfo.InvariantCulture);
                    InterfaceDist = res;
                    NotifyPropertyChanged("InterfaceDistString");
                }
                catch
                {

                }
            }
        }


        private bool useVanDerWaalsRadiiIface = true;

        public bool UseVanDerWaalsRadiiIface
        {
            get { return useVanDerWaalsRadiiIface; }
            set
            {
                useVanDerWaalsRadiiIface = value;
                NotifyPropertyChanged("UseVanDerWaalsRadiiIface");
            }
        }


        private double neighbourDistance;

        public double NeighbourDistance
        {
            get { return neighbourDistance; }
            set
            {
                neighbourDistance = value;
                NotifyPropertyChanged("NeighbourDistance");
                NotifyPropertyChanged("CanCreate");
            }
        }


        public string NeighbourDistanceString
        {
            get { return NeighbourDistance.ToString(); }
            set
            {
                try
                {
                    double res = double.Parse(value, CultureInfo.InvariantCulture);
                    NeighbourDistance = res;
                    NotifyPropertyChanged("NeighbourDistanceString");
                }
                catch
                {

                }
            }
        }

        public string DivisionIntervalsString
        {
            get { return DivisionIntervals.ToString(); }
            set
            {
                try
                {
                    int res = int.Parse(value, CultureInfo.InvariantCulture);
                    DivisionIntervals = res;
                    NotifyPropertyChanged("DivisionIntervalsString");
                }
                catch
                {

                }
            }
        }

        private double threshold;
        public double Threshold
        {
            get { return threshold; }
            set
            {
                threshold = value;
                NotifyPropertyChanged("Threshold");
            }
        }

        public string ThresholdString
        {
            get { return Threshold.ToString(); }
            set
            {
                try
                {
                    double res = double.Parse(value, CultureInfo.InvariantCulture);
                    Threshold = res;
                    NotifyPropertyChanged("ThresholdString");
                }
                catch
                {

                }
            }
        }

        private int divisionIntervals;
        public int DivisionIntervals
        {
            get { return divisionIntervals; }
            set
            {
                divisionIntervals = value;
                NotifyPropertyChanged("DivisionIntervals");
                NotifyPropertyChanged("CanCreate");
            }
        }


        private string trainingPDBsFile;
        public string TrainingPDBsFile
        {
            get { return trainingPDBsFile; }
            set
            {
                trainingPDBsFile = value;
                NotifyPropertyChanged("TrainingPDBsFile");
                NotifyPropertyChanged("CanCreate");
            }
        }


        private ResidueNodeCenterDefinition graphNodeDef;
        public ResidueNodeCenterDefinition GraphNodeDefinition
        {
            get { return graphNodeDef; }
            set
            {
                graphNodeDef = value;
                NotifyPropertyChanged("GraphNodeDefinition");
            }
        }

        private PredictorTypes predictionType;
        public PredictorTypes PredictionType
        {
            get { return predictionType; }
            set
            {
                predictionType = value;
                NotifyPropertyChanged("PredictionType");
            }
        }


        public bool CanCreate
        {
            get { return canCreate(); }
        }

        private bool canCreate()
        {
            return NeighbourDistance > 0 && trainingPDBsFile.NotNullOrEmpty() && DivisionIntervals > 0;
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

    public class CreatePredictor : IHas<IRequestLogic<CreatePredictor>>
    {
        public Guid GWId { get; set; }
        public CreatePredictor(CreatePredictorVM vm)
        {
            ViewModel = vm;
        }

        public IHas<IPredictionLogic> Predictor { get; set; }

        public CreatePredictorVM ViewModel { get; set; }

        private RequestLogic<CreatePredictor> logic = new RequestLogic<CreatePredictor>();
        public IRequestLogic<CreatePredictor> Logic
        {
            get { return logic; }
        }
    }

}
