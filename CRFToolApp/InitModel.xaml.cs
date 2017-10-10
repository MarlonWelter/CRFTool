
using CodeBase;
using CRFBase.Input;
using CRFToolAppBase;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRFToolApp
{
    /// <summary>
    /// Interaction logic for InitModel.xaml
    /// </summary>
    public partial class InitModel : System.Windows.Controls.UserControl
    {
        public CRFToolData CRFToolData { get; set; } = new CRFToolData();

        public InitModel()
        {
            InitializeComponent();
        }

        private void LoadParametersetB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Parameter Files|*.par";
            openFileDialog1.Title = "Select a Parameter File";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CRFToolData.IsingData = JSONX.LoadFromJSON<IsingData>(openFileDialog1.FileName);
            }
        }

        private void ParametersetSGB_Click(object sender, RoutedEventArgs e)
        {
            CRFToolData.IsingData = new SoftwareGraphIsing();

            var window = new MainView();
            window.Show();
        }

        private void ParametersetPGB_Click(object sender, RoutedEventArgs e)
        {
            CRFToolData.IsingData = new ProteinGraphIsing();
        }

        private void UserTrainingB_Click(object sender, RoutedEventArgs e)
        {
            var exampleGraph = UserTrainingX.ExampleData();
            exampleGraph.SaveAsJSON("exampleGraph.txt", new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            var testgraph = UserTrainingX.ParseTrainingData("exampleGraph.txt");
        }
    }
}
