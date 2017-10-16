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
using System.Globalization;

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var viewOption = ComboBox
            ViewModel.ChangeViewItems(viewOptionComboBox.SelectedValue as string);
        }
    }

    public class MainViewViewModel : INotifyPropertyChanged
    {
        public MainViewViewModel()
        {
            viewOptions.AddRange("Prediction", "Reference");
        }
        private ObservableList<string> viewOptions = new ObservableList<string>();

        public ObservableList<string> ViewOptions

        {
            get { return viewOptions; }
            set { viewOptions = value; }
        }
        private ObservableList<string> viewItems = new ObservableList<string>();

        public void ChangeViewItems(string viewOption)
        {
            ViewItems.Clear();
            switch (viewOption)
            {
                case "Prediction":
                    ViewItems.AddRange("True Positive", "True Negative", "False Positive", "False Negative");
                    break;
                case "Reference":
                    ViewItems.AddRange("Classification 0", "Classification 1");
                    break;
                default:
                    break;
            }
            NotifyPropertyChanged("ViewItems");
        }
        public ObservableList<string> ViewItems

        {
            get { return viewItems; }
            set { viewItems = value; }
        }
        public bool HasViewOptionOne => ViewItems.NotNullOrEmpty();
        public bool HasViewOptionTwo => (ViewItems.NotNullOrEmpty() && ViewItems.Count > 1);
        public bool HasViewOptionThree => (ViewItems.NotNullOrEmpty() && ViewItems.Count > 2);
        public bool HasViewOptionFour => (ViewItems.NotNullOrEmpty() && ViewItems.Count > 3);
        public string ViewOptionOne => HasViewOptionOne ? ViewItems[0] : string.Empty;
        public string ViewOptionTwo => HasViewOptionTwo ? ViewItems[1] : string.Empty;
        public string ViewOptionThree => HasViewOptionThree ? ViewItems[2] : string.Empty;
        public string ViewOptionFour => HasViewOptionFour ? ViewItems[3] : string.Empty;

        public Visibility ViewItemOne => HasViewOptionOne ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ViewItemTwo => HasViewOptionTwo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ViewItemThree => HasViewOptionThree ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ViewItemFour => HasViewOptionFour ? Visibility.Visible : Visibility.Collapsed;


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
                if (Graphs.NotNullOrEmpty())
                    graphIndex = (graphIndex + Graphs.Count) % Graphs.Count;
                NotifyPropertyChanged("Graph");
            }
        }

        private string graphName;

        public string GraphName
        {
            get { return Graph.Name; }
            set
            {
                graphName = value;
                NotifyPropertyChanged("GraphName");
            }
        }

        public MainViewViewModel ViewModel => this;


        public IGWGraph<CRFNodeData, CRFEdgeData, CRFGraphData> Graph
        {
            get { return Graphs.NotNullOrEmpty() ? Graphs[graphIndex] : null; }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                PropertyChanged(this, new PropertyChangedEventArgs("ViewModel"));
            }
        }

        #endregion
    }
    [ValueConversion(typeof(MainViewViewModel), typeof(string))]
    class GraphCountConverter : IValueConverter
    {
        Color[] colors = new Color[] { Colors.Blue, Colors.Green, Colors.Yellow, Colors.Red, Colors.Orange, Colors.LightGreen, Colors.Black, Colors.WhiteSmoke, Colors.Brown, Colors.AliceBlue, Colors.Lavender, Colors.Indigo, Colors.Gray, Colors.Goldenrod, Colors.LightCyan, Colors.LightPink, Colors.Moccasin };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vm = value as MainViewViewModel;
            if (vm == null)
                return null;


            return vm.GraphIndex + "/" + vm.Graphs.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
