using CodeBase;
using CodeBase.Graph;
using HelixToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MS.Internal.Media3D;
using System.Windows;
using System.Windows.Media.Composition;
using System.Windows.Input;
using System.Globalization;

namespace CRFToolApp
{
    [ValueConversion(typeof(MainViewViewModel), typeof(Model3DGroup))]
    public class MainViewGraph3DConverter : IValueConverter
    {
        Color[] colors = new Color[] { Colors.Blue, Colors.Green, Colors.Yellow, Colors.Red, Colors.Orange, Colors.LightGreen, Colors.Black, Colors.WhiteSmoke, Colors.Brown, Colors.AliceBlue, Colors.Lavender, Colors.Indigo, Colors.Gray, Colors.Goldenrod, Colors.LightCyan, Colors.LightPink, Colors.Moccasin, Colors.Azure, Colors.Firebrick, Colors.Fuchsia };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vm = value as MainViewViewModel;
            if (vm == null || vm.Graph == null)
                return null;

            var graph = vm.Graph;

            var visuals = new GeometryModel3D[colors.Length];
            var meshes = new MeshBuilder[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                visuals[i] = new GeometryModel3D();
                meshes[i] = new MeshBuilder();
            }

            var grayScaleColors = new Color[20];
            for (int i = 0; i < grayScaleColors.Length; i++)
            {
                grayScaleColors[i] = Color.FromRgb((byte)(250 - i * 25), (byte)(250 - i * 25), (byte)(250 - i * 25));
            }
            var useColors = colors;

            Model3DGroup modelGroup = new Model3DGroup();
            var Emesh = new MeshBuilder();


            foreach (var node in graph.Nodes)
            {
                if (!node.Neighbours.Any())
                    continue;

                Point3D center = new Point3D(node.Data.X, node.Data.Y, node.Data.Z);

                var mesh = default(MeshBuilder);
                switch (vm.ViewOption)
                {
                    case "Prediction":
                        if (node.Data.AssignedLabel == 1 && node.Data.ReferenceLabel == 1)
                            mesh = meshes[0 % colors.Length];
                        else if (node.Data.AssignedLabel == 0 && node.Data.ReferenceLabel == 0)
                            mesh = meshes[1 % colors.Length];
                        else if (node.Data.AssignedLabel == 0 && node.Data.ReferenceLabel == 1)
                            mesh = meshes[2 % colors.Length];
                        else
                            mesh = meshes[3 % colors.Length];
                        break;
                    case "Reference":
                        mesh = meshes[node.Data.ReferenceLabel % colors.Length];
                        break;
                    case "Observation":
                        mesh = meshes[node.Data.Observation % colors.Length];
                        break;
                    default:
                        if (vm.ViewOption.StartsWith("Characteristic_"))
                        {
                            var characteristicString = vm.ViewOption.Substring(15);
                            var characteristicNr = graph.Data.Characteristics.ToList().IndexOf(characteristicString);
                            var characteristicValue = node.Data.Characteristics[characteristicNr];
                            mesh = meshes[(int)(characteristicValue * 20)];
                            useColors = grayScaleColors;
                        }
                        break;
                }

                mesh?.AddSphere(center, 3, 8, 4);

            }

            foreach (var edge in graph.Edges)
            {
                var foot = edge.Foot;
                var head = edge.Head;
                Point3D center1 = new Point3D(foot.Data.X, foot.Data.Y, foot.Data.Z);
                Point3D center2 = new Point3D(head.Data.X, head.Data.Y, head.Data.Z);

                Emesh.AddCylinder(center1, center2, 1, 4);
            }
            for (int i = 0; i < useColors.Length; i++)
            {
                visuals[i] = new GeometryModel3D(meshes[i].ToMesh(), MaterialHelper.CreateMaterial(new SolidColorBrush(useColors[i])));
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
