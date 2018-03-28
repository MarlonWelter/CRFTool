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
    /// Interaction logic for View3D.xaml
    /// </summary>
    public partial class View3D : UserControl
    {

        private MainViewViewModel viewmodel;
        public MainViewViewModel ViewModel { get { return viewmodel; } set { viewmodel = value; DataContext = ViewModel; } }
        public View3D()
        {
            InitializeComponent();
        }
        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GraphIndex--;
            //if (isRunning)
            //    Embed();
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GraphIndex++;
            //if (isRunning)
            //    Embed();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var viewOption = ComboBox
            if (viewOptionComboBox.SelectedValue != null)
                ViewModel.ChangeViewItems(viewOptionComboBox.SelectedValue as string);
        }
    }
}
