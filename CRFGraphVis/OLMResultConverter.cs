using CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Globalization;
using HelixToolkit;

namespace CRFGraphVis
{
    [ValueConversion(typeof(OLMResultViewModel), typeof(Model3DGroup))]
    class OLMResultConverter : IValueConverter
    {
        Color[] colors = new Color[] { Colors.Green, Colors.Red, Colors.Blue, Colors.Orange, Colors.Yellow, Colors.LightGreen, Colors.Black, Colors.WhiteSmoke, Colors.Brown, Colors.AliceBlue, Colors.Lavender, Colors.Indigo, Colors.Gray, Colors.Goldenrod, Colors.LightCyan, Colors.LightPink, Colors.Moccasin };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vm = value as OLMResultViewModel;
            if (vm == null)
                return null;

            var graph = vm.GraphInFocus.Graph;
            var visuals = new GeometryModel3D[colors.Length];
            var meshes = new MeshBuilder[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                visuals[i] = new GeometryModel3D();
                meshes[i] = new MeshBuilder();
            }

            Model3DGroup modelGroup = new Model3DGroup();
            var Emesh = new MeshBuilder();


            foreach (var node in graph.Nodes)
            {
                if (!node.Neighbours.Any())
                    continue;

                Point3D center = new Point3D(node.Data.X, node.Data.Y, node.Data.Z);

                switch (vm.ViewType)
                {
                    case ViewType.Default:
                        if (node.Data.ReferenceLabel == 1 && vm.GraphInFocus.Prediction[node.GraphId] == 1)
                            meshes[0].AddSphere(center, 3, 8, 4);
                        else if (node.Data.ReferenceLabel == 1 && vm.GraphInFocus.Prediction[node.GraphId] == 0)
                            meshes[1].AddSphere(center, 3, 8, 4);
                        else if (node.Data.ReferenceLabel == 0 && vm.GraphInFocus.Prediction[node.GraphId] == 0)
                            meshes[2].AddSphere(center, 3, 8, 4);
                        else if (node.Data.ReferenceLabel == 0 && vm.GraphInFocus.Prediction[node.GraphId] == 1)
                            meshes[3].AddSphere(center, 3, 8, 4);
                        break;
                    case ViewType.Reference:
                        meshes[node.Data.ReferenceLabel % colors.Length].AddSphere(center, 3, 8, 4);
                        break;
                    case ViewType.Observation:
                        meshes[node.Data.Observation % colors.Length].AddSphere(center, 3, 8, 4);
                        break;
                    case ViewType.Prediction:
                        meshes[vm.GraphInFocus.Prediction[node.GraphId] % colors.Length].AddSphere(center, 3, 8, 4);
                        break;
                    default:
                        meshes[node.Data.ReferenceLabel % colors.Length].AddSphere(center, 3, 8, 4);
                        break;
                }
            }

            foreach (var edge in graph.Edges)
            {
                var foot = edge.Foot;
                var head = edge.Head;
                Point3D center1 = new Point3D(foot.Data.X, foot.Data.Y, foot.Data.Z);
                Point3D center2 = new Point3D(head.Data.X, head.Data.Y, head.Data.Z);

                Emesh.AddCylinder(center1, center2, 1, 4);
            }
            for (int i = 0; i < colors.Length; i++)
            {
                visuals[i] = new GeometryModel3D(meshes[i].ToMesh(), MaterialHelper.CreateMaterial(new SolidColorBrush(colors[i])));
                modelGroup.Children.Add(visuals[i]);
            }

            modelGroup.Children.Add(new GeometryModel3D(Emesh.ToMesh(), MaterialHelper.CreateMaterial(new SolidColorBrush(Colors.Gray))));

            return modelGroup;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
