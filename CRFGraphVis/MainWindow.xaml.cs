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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CodeBase;

namespace CRFGraphVis
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graph3DViewModel viewModel;

        public Graph3DViewModel ViewModel
        {
            get { return viewModel; }
            set
            {
                viewModel = value;
                DataContext = ViewModel;
            }
        }

        public I3DEmbeddingControl EmbeddingControl { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new Graph3DViewModel();

            this.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (EmbeddingControl == null)
                    Embed();
                else
                {
                    EmbeddingControl.Stop();
                }
            }
        }

        public void Embed()
        {
            EmbeddingControl = EmbeddingX.CreateDefaultEmbeddingControl();
            var timer = new Timer((obj) => ViewModel.NotifyPropertyChanged(nameof(ViewModel.ViewModel)));
            timer.Change(0, 25);


            EmbeddingControl.Graph = ViewModel.Graph?.Convert((n) => new EDND(1.0, n.Data.CommunityId.ToString(), n.Data, 0), (edge) => new EDED(1.0), (g) => new EDGD());



            EmbeddingControl?.Start();
        }
    }
}
