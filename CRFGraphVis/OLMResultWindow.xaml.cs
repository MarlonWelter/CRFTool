using CRFBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CodeBase;

namespace CRFGraphVis
{
    /// <summary>
    /// Interaction logic for OLMResultWindow.xaml
    /// </summary>
    public partial class OLMResultWindow : Window
    {
        private OLMResultViewModel viewModel;

        public OLMResultViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                viewModel = value;
                DataContext = viewModel;
            }
        }

        public OLMResultWindow()
        {
            InitializeComponent();

            viewModel = new OLMResultViewModel();
            DataContext = viewModel;
            this.KeyDown += MainWindow_KeyDown;
        }

        public I3DEmbeddingControl EmbeddingControl { get; set; }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    if (EmbeddingControl == null)
                        Embed();
                    else
                    {
                        EmbeddingControl.Stop();
                        EmbeddingControl = null;
                    }
                    break;
                case Key.Tab:
                    ViewModel.ViewType = ViewModel.ViewType.Next();
                    break;
                case Key.S:
                    viewModel.EvalResult.SaveAsJSON("testFile.txt");
                    break;
                case Key.L:
                    ViewModel.EvalResults.Add(JSONX.LoadFromJSON<OLMEvaluationResult>("testFile.txt"));
                    break;
                case Key.N:
                    ViewModel.EvalResPointer++;
                    break;
                case Key.P:
                    ViewModel.EvalResPointer--;
                    break;
                default:
                    break;
            }
        }

        public void Embed()
        {
            EmbeddingControl = EmbeddingX.CreateDefaultEmbeddingControl();
            var timer = new Timer((obj) => ViewModel.NotifyPropertyChanged(nameof(ViewModel.ViewModel)));
            timer.Change(0, 25);


            EmbeddingControl.Graph = ViewModel.GraphInFocus.Graph?.Convert((n) => new EDND(1.0, 0.ToString(), n.Data, 0), (edge) => new EDED(1.0), (g) => new EDGD());



            EmbeddingControl?.Start();
        }

        private void PrevGraph_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.GraphPointer > 0)
                viewModel.GraphPointer--;
        }

        private void NextGraph_Click(object sender, RoutedEventArgs e)
        {
            viewModel.GraphPointer++;
        }
    }
}
