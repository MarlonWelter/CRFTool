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
    /// Interaction logic for ViterbiView.xaml
    /// </summary>
    public partial class ViterbiView : UserControl
    {
        private MainViewViewModel viewmodel;
        public MainViewViewModel ViewModel
        {
            get { return viewmodel; }
            set
            {
                viewmodel = value;
                DataContext = ViewModel;
            }
        }
        public ViterbiView()
        {
            InitializeComponent();
        }
    }
}
