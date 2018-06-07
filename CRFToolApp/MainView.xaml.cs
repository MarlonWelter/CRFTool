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
using System.IO;
using CRFBase.GibbsSampling;

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
            CRFToolAppBase.Build.Do();
            CRFBase.Build.Do();
            Build.Do();
            ViewModel = new MainViewViewModel();
            ViewModel.Graphs.Add(GWGraphPackageTwo.CategoryGraph<SGLNodeData, SGLEdgeData, SGLGraphData>(50, 5, () => new SGLNodeData()));
            //ViewModel.ViewContent = ViewContent.GraphDetails;
            DataContext = ViewModel;
            graphsListView.ViewModel = ViewModel;
            view3DView.ViewModel = ViewModel;
            graphDetailsView.ViewModel = ViewModel;
            viterbiView.ViewModel = ViewModel;
            settings.DataContext = ViewModel;
        }

        private graphsList graphsListView = new graphsList();
        private View3D view3DView = new View3D();
        private graphDetailsList graphDetailsView = new graphDetailsList();
        private ViterbiView viterbiView = new ViterbiView();
        private Settings settings = new Settings();

        private void button1_Click(object sender, RoutedEventArgs e)
        { // load data
            var request = new LoadCRFGraph();
            request.Request();
            if (request.Graph != null)
            {
                ViewModel.Graphs.Add(request.Graph);
                Embed();
            }
        }
        I3DEmbeddingControl embeddingControl;
        Timer timer;
        bool isRunning = false;
        public void Embed()
        {
            if (ViewModel.Graph != null)
            {
                if (!isRunning)
                {
                    embeddingControl = EmbeddingX.CreateDefaultEmbeddingControl();
                    timer = new Timer((obj) => ViewModel.NotifyPropertyChanged("Graph"));
                    timer.Change(0, 25);

                    embeddingControl.Graph = ViewModel.Graph?.Convert((n) => new EDND(1.0, "default", n.Data, 0), (edge) => new EDED(1.0), (g) => new EDGD());

                    embeddingControl?.Start();
                    isRunning = true;
                }
                else
                {
                    embeddingControl.Stop();
                    timer.Dispose();
                    isRunning = false;
                }
            }
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Embed();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            // add characteristic and score

            var graph = ViewModel.Graph;
            if (graph?.Data == null) return;

            RandomData.AddDefault(graph);

            ViewModel.NotifyPropertyChanged("Graph");
        }

        private void buttonLeft1_Click(object sender, RoutedEventArgs e)
        { // shpw graphs
          /*   ViewModel.ViewContent = ViewContent.GraphsList;*/
          //contentBox.Content = graphsListView;
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(graphsListView);
        }

        private void buttonLeft2_Click(object sender, RoutedEventArgs e)
        { // show 3D View
            //ViewModel.ViewContent = ViewContent.View3D;
            //contentBox.Content = view3DView;
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(view3DView);
        }

        private void buttonLeft3_Click(object sender, RoutedEventArgs e)
        { // show sample
            //ViewModel.ViewContent = ViewContent.GraphDetails;
            //contentBox.Content = graphDetailsView;
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(graphDetailsView);
        }

        private void buttonLeft4_Click(object sender, RoutedEventArgs e)
        { // show viterbi result
            if (ViewModel?.Graph?.Data?.Viterbi != null)
            {
                // assign viterbi result node.assignedlabel
                foreach (var node in ViewModel.Graph.Nodes)
                {
                    node.Data.AssignedLabel = ViewModel.Graph.Data.Viterbi[node.GraphId];
                }

                ContentPanel.Children.Clear();
                ContentPanel.Children.Add(viterbiView);
            }
        }

        private void buttonLeft5_Click(object sender, RoutedEventArgs e)
        { // settings
            //ViewModel.ViewContent = ViewContent.Settings;
            //contentBox.Content = graphsListView;
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(settings);
        }

        private void buttonViterbi_Click(object sender, RoutedEventArgs e)
        { // run viterbi
            var request = new SolveInference(ViewModel.Graph, 2);
            request.Request();

            ViewModel.Graph.Data.Viterbi = request.Solution.Labeling;

            // assign viterbi result node.assignedlabel
            foreach (var node in ViewModel.Graph.Nodes)
            {
                node.Data.AssignedLabel = request.Solution.Labeling[node.GraphId];
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        { // save current graph.
            Directory.CreateDirectory("Output\\");

            JSONX.SaveAsJSON(ViewModel.Graph, "Output\\" + ViewModel.GraphName + ".json");
        }

        private void buttonMCMC_Click(object sender, RoutedEventArgs e)
        {// run mcmc
            var parameters = new MHSampler2Parameters();
            parameters.Graph = ViewModel.Graph;
            parameters.NumberChains = 10;
            parameters.PreRunLength = 100;
            parameters.MHSampler2StartPoint = MHSampler2StartPoint.Random;
            parameters.RunLength = 100;

            var sampler = new MHSampler2();
            sampler.Do(parameters);

            ViewModel.Graph.Data.Sample = sampler.FinalSample;

            if (ViewModel.Graph.Data.Sample.NotNullOrEmpty())
            {
                // assign viterbi result node.assignedlabel
                foreach (var node in ViewModel.Graph.Nodes)
                {
                    node.Data.AssignedLabel = ViewModel.Graph.Data.Sample[0][node.GraphId];
                }
            }
        }
    }

    public class MainViewViewModel : INotifyPropertyChanged
    {
        public MainViewViewModel()
        {
            UpdateViewOptions();
        }
        private ObservableList<string> viewOptions = new ObservableList<string>();

        public ObservableList<string> ViewOptions

        {
            get { return viewOptions; }
            set { viewOptions = value; }
        }
        private List<string> viewItems = new List<string>();

        private int viewOptionIndex;
        public int ViewOptionIndex
        {
            get { return viewOptionIndex; }
            set
            {
                viewOptionIndex = value;
                NotifyPropertyChanged("ViewOption");
            }
        }
        void UpdateViewOptions()
        {
            viewOptions.Clear();
            viewOptions.AddRange("Prediction", "Reference");
            if (Graph?.Data?.Characteristics != null)
            {
                foreach (var characteristic in Graph.Data.Characteristics)
                {
                    viewOptions.Add("Characteristic_" + characteristic);
                }
            }
            viewOptions.Add("Observation");
            NotifyPropertyChanged("ViewOptions");
        }

        public string ViewOption => (viewOptions.NullOrEmpty() || viewOptions.Count <= viewOptionIndex || viewOptionIndex < 0) ? "" : viewOptions[viewOptionIndex];


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
                case "Observation":
                    ViewItems.AddRange("Observation 0", "Observation 1");
                    break;
                default:
                    break;
            }
            ViewOptionIndex = ViewOptions.IndexOf(viewOption);
        }
        public List<string> ViewItems

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


        private ObservableList<IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>> graphs = new ObservableList<IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>>();

        public ObservableList<IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData>> Graphs
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
                UpdateViewOptions();
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

        private int samplePointer;

        public int SamplePointer
        {
            get { return samplePointer; }
            set
            {
                if (ViewModel?.Graph?.Data?.Sample == null)
                    return;

                samplePointer = Math.Max(0, value % ViewModel.Graph.Data.Sample.Count);

                // assign viterbi result node.assignedlabel
                foreach (var node in ViewModel.Graph.Nodes)
                {
                    node.Data.AssignedLabel = ViewModel.Graph.Data.Sample[SamplePointer][node.GraphId];
                }

                NotifyPropertyChanged("SamplePointer");
            }
        }


        public IGWGraph<SGLNodeData, SGLEdgeData, SGLGraphData> Graph
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
    public enum ViewContent
    {
        GraphsList,
        View3D,
        GraphDetails,
        Settings
    }
}
