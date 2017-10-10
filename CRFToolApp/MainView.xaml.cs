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
        MainViewViewModel ViewModel { get; set; }
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
            ViewModel.Graph = request.Graph;
            Embed();
        }
        public void Embed()
        {
           var  EmbeddingControl = EmbeddingX.CreateDefaultEmbeddingControl();
            var timer = new Timer((obj) => ViewModel.NotifyPropertyChanged("Graph"));
            timer.Change(0, 25);
            
            EmbeddingControl.Graph = ViewModel.Graph?.Convert((n) => new EDND(1.0, "default", n.Data, 0), (edge) => new EDED(1.0), (g) => new EDGD());
            

            EmbeddingControl?.Start();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var graph =ViewModel.Graph;
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

        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    class MainViewViewModel : INotifyPropertyChanged
    {
        private IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> graph;

        public IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                NotifyPropertyChanged("Graph");
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
