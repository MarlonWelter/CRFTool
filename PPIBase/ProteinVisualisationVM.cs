using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace PPIBase
{
    public class ProteinVisualisationVM : INotifyPropertyChanged
    {
        public ProteinVisualisationVM()
        {
            NeighbourDefinition = 6.0;
            InterfaceDefinitionDistance = 0.5;
            InterfaceUseVanDerWaalsRadii = true;
            ResidueNodeDefinition = ResidueNodeCenterDefinition.CAlpha;
            dirLight = false;
        }

        private bool dirLight;

        public bool DirLight
        {
            get { return dirLight; }
            set
            {
                dirLight = value;
                NotifyPropertyChanged("DirLight");
            }
        }


        public double InterfaceDefinitionDistance { get; set; }

        public bool InterfaceUseVanDerWaalsRadii { get; set; }



        private IHas<IPredictionLogic> predictor;

        public IHas<IPredictionLogic> Predictor
        {
            get { return predictor; }
            set
            {
                predictor = value;
                NotifyPropertyChanged("Predictor");
            }
        }

        private bool predictonCore;

        public bool PredictOnCore
        {
            get { return predictonCore; }
            set
            {
                predictonCore = value;
                NotifyPropertyChanged("PredictOnCore");
            }
        }



        private bool filterCore;

        public bool FilterCore
        {
            get { return filterCore; }
            set
            {
                filterCore = value;
                NotifyPropertyChanged("ViewModel");
            }
        }
        private double maxRasaCoreDef;

        public double MaxRasaCoreDef
        {
            get { return maxRasaCoreDef; }
            set
            {
                maxRasaCoreDef = value;
                NotifyPropertyChanged("MaxRasaCoreDef");
                NotifyPropertyChanged("ViewModel");
            }
        }


        private bool markInterface;

        public bool MarkInterface
        {
            get { return markInterface; }
            set
            {
                markInterface = value;

                NotifyPropertyChanged("ViewModel");
            }
        }



        private ViewTypes viewtype;

        public ViewTypes ViewType
        {
            get { return viewtype; }
            set
            {
                viewtype = value;
                NotifyPropertyChanged("ViewType");
                NotifyPropertyChanged("ViewModel");
            }
        }


        private double nbdef;

        public double NeighbourDefinition
        {
            get { return nbdef; }
            set
            {
                nbdef = value;
                NotifyPropertyChanged("NeighbourDefinition");
            }
        }

        private ResidueNodeCenterDefinition nodeDef;

        public ResidueNodeCenterDefinition ResidueNodeDefinition
        {
            get { return nodeDef; }
            set
            {
                nodeDef = value;
                NotifyPropertyChanged("ResidueNodeDefinition");
            }
        }


        private bool transparentNI;

        public bool TransparentNonInterface
        {
            get { return transparentNI; }
            set
            {
                transparentNI = value;
                NotifyPropertyChanged("TransparentNonInterface");
                NotifyPropertyChanged("ViewModel");
            }
        }





        private ObservableList<PDBInfo> pdbs = new ObservableList<PDBInfo>();

        public ObservableList<PDBInfo> PDBs
        {
            get { return pdbs; }
            set
            {
                pdbs = value;
                NotifyPropertyChanged("PDBs");
            }
        }

        private PDBInfo file1;

        public PDBInfo PDBInfo
        {
            get { return file1; }
            set
            {
                file1 = value;
                NotifyPropertyChanged("PDBInfo");
                NotifyPropertyChanged("ViewModel");
            }
        }




        public ProteinVisualisationVM ViewModel { get { return this; } }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
    public class PDBInfo
    {
        public PDBInfo(PDBFile file)
        {
            file1 = file;
            file.Chains.Each(chain => ShowChain.Add(chain.Name, true));
        }
        private readonly PDBFile file1;

        public PDBFile PDBFile
        {
            get { return file1; }
        }

        private Dictionary<Residue, bool> interface1 = new Dictionary<Residue, bool>();

        public Dictionary<Residue, bool> Interface
        {
            get { return interface1; }
            set
            {
                interface1 = value;
                NotifyPropertyChanged("ViewModel");
            }
        }

        public Dictionary<Residue, bool> FullPrediction()
        {
            var dict = new Dictionary<Residue, bool>();

            foreach (var chain in prediction)
            {
                dict.AddRange(chain.Value);
            }

            return dict;
        }

        private Dictionary<Residue, double> rasa1 = new Dictionary<Residue, double>();
        public Dictionary<Residue, double> Rasa
        {
            get { return rasa1; }
            set
            {
                rasa1 = value;
                NotifyPropertyChanged("ViewModel");
            }
        }

        private Dictionary<string, Dictionary<Residue, bool>> prediction = new Dictionary<string, Dictionary<Residue, bool>>();

        public Dictionary<string, Dictionary<Residue, bool>> Prediction
        {
            get { return prediction; }
            set
            {
                prediction = value;
                NotifyPropertyChanged("ViewModel");
            }
        }

        private LinkedList<ResidueNode> markedResidues = new LinkedList<ResidueNode>();

        public LinkedList<ResidueNode> MarkedResidues1
        {
            get { return markedResidues; }
            set
            {
                markedResidues = value;
                NotifyPropertyChanged("MarkedResidues1");
                NotifyPropertyChanged("ViewModel");
            }
        }

        private Dictionary<string, bool> showChain = new Dictionary<string, bool>();

        public Dictionary<string, bool> ShowChain
        {
            get { return showChain; }
            set
            {
                showChain = value;
                NotifyPropertyChanged("ShowProtOne");
                NotifyPropertyChanged("ViewModel");
            }
        }
        private List<string> chains = new List<string>();
        public IList<string> Chains
        {
            get { return chains; }
        }
        public void AddChain(string chain)
        {
            chains.Add(chain);
            showChain.Add(chain, true);
        }

        private Dictionary<string, ProteinGraph> graphs = new Dictionary<string, ProteinGraph>();

        public Dictionary<string, ProteinGraph> Graphs
        {
            get { return graphs; }
            set
            {
                graphs = value;
                NotifyPropertyChanged("ViewModel");
            }
        }

        private Dictionary<string, Dictionary<ProteinGraph, IHas<IResidueFeatureLogic>>> features = new Dictionary<string, Dictionary<ProteinGraph, IHas<IResidueFeatureLogic>>>();

        public Dictionary<string, Dictionary<ProteinGraph, IHas<IResidueFeatureLogic>>> Features
        {
            get { return features; }
            set { features = value; }
        }


        public double FeatureValue(ResidueNode node, string feature)
        {
            if (Features.ContainsKey(feature) && Features[feature].ContainsKey(node.Graph))
                return Features[feature][node.Graph].ValueOf(node.Residue);
            return 0.0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public override string ToString()
        {
            return PDBFile.Name;
        }
    }
}
