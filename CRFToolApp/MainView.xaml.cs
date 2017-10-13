using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CRFToolAppBase;
using CRFBase;
using CodeBase;
using System.ComponentModel;
using System.Threading;

namespace CRFToolApp
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainViewViewModel ViewModel { get; set; }
        public MainView()
        {
            InitializeComponent();
            ViewModel = new MainViewViewModel();
            DataContext = ViewModel;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        { // load data
            var request = new LoadCRFGraph();
            request.Request();
            ViewModel.Graphs.Add(request.Graph);
            Embed();
        }
        public void Embed()
        {
            var EmbeddingControl = EmbeddingX.CreateDefaultEmbeddingControl();
            var timer = new Timer((obj) => ViewModel.NotifyPropertyChanged("Graph"));
            timer.Change(0, 25);

            EmbeddingControl.Graph = ViewModel.Graph?.Convert((n) => new EDND(1.0, "default", n.Data, 0), (edge) => new EDED(1.0), (g) => new EDGD());


            EmbeddingControl?.Start();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var graph = ViewModel.Graph;
            if (graph == null) return;
            var request = new SolveInference(graph, null, graph.Data.NumberOfLabels);
            request.Request();
            foreach (var item in graph.Nodes)
            {
                item.Data.AssignedLabel = request.Solution?.Labeling[item.Data.Ordinate] ?? 0;
            }
            ViewModel.NotifyPropertyChanged("Graph");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Embed();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GraphIndex--;
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GraphIndex++;
        }
    }

    public class MainViewViewModel : INotifyPropertyChanged
    {
        private ObservableList<IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> graphs = new ObservableList<IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>>();

        public ObservableList<IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData>> Graphs
        {
            get { return graphs; }
        }

        private int graphIndex;

        public int GraphIndex
        {
            get { return graphIndex; }
            set
            {
                graphIndex = value;
                NotifyPropertyChanged("Graph");
            }
        }



        public IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> Graph
        {
            get { return Graphs.NotNullOrEmpty() ? Graphs[graphIndex % Graphs.Count] : null; }
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
