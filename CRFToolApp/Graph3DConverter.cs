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
    [ValueConversion(typeof(IGWGraph<ICRFNode3DInfo, IEdge3DInfo, object>), typeof(Model3DGroup))]
    public class Graph3DConverter : IValueConverter
    {
        Color[] colors = new Color[] { Colors.Blue, Colors.Green, Colors.Yellow, Colors.Red, Colors.Orange, Colors.LightGreen, Colors.Black, Colors.WhiteSmoke, Colors.Brown, Colors.AliceBlue, Colors.Lavender, Colors.Indigo, Colors.Gray, Colors.Goldenrod, Colors.LightCyan, Colors.LightPink, Colors.Moccasin };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var graph = value as IGWGraph<ICRFNode3DInfo, IEdge3DInfo, object>;
            if (graph == null)
                return null;

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

                meshes[node.Data.ReferenceLabel % colors.Length].AddSphere(center, 3, 8, 4);

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
