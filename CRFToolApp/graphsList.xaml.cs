using CodeBase;
using CRFBase.Input;
using CRFToolAppBase;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRFToolApp
{
    /// <summary>
    /// Interaction logic for graphsList.xaml
    /// </summary>
    public partial class graphsList : UserControl
    {
        private MainViewViewModel viewmodel;
        public MainViewViewModel ViewModel { get { return viewmodel; } set { viewmodel = value; DataContext = ViewModel; } }
        public graphsList()
        {
            InitializeComponent();
            //ViewModel = new MainViewViewModel();
            //ViewModel.Graphs.Add(GWGraphPackageTwo.CategoryGraph<CRFNodeData, CRFEdgeData, CRFGraphData>(50, 5));
            //DataContext = ViewModel;
        }
    }
}
