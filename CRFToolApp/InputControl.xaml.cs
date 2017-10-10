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
    /// Interaction logic for InputControl.xaml
    /// </summary>
    public partial class InputControl : UserControl
    {
        public CRFToolData CRFToolData { get; set; }

        public InputControl()
        {
            InitializeComponent();
        }

        private void InputB_Click(object sender, RoutedEventArgs e)
        {
            CRFToolData.UseCase = UseCase.General;
        }

        private void SoftwareB_Click(object sender, RoutedEventArgs e)
        {
            CRFToolData.UseCase = UseCase.SoftwareGraph;
        }

        private void ProteinB_Click(object sender, RoutedEventArgs e)
        {
            CRFToolData.UseCase = UseCase.ProteinGraph;
        }
    }
}
