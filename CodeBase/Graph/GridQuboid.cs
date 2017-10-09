using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class GridQuboid<T> : IGridQuboid where T : IConnectedGridElement3D, ILocalizableGridElement3D
    {

        public readonly IEnumerable<T> EnclosedElements;
        public readonly int GridLeft;
        public readonly int GridBack;
        public readonly int GridDown;
        private readonly int gridWidth;
        private readonly int gridLength;
        private readonly int gridHeight;
        public int GridWidth { get { return gridWidth; } }
        public int GridLength { get { return gridLength; } }
        public int GridHeight { get { return gridHeight; } }

        public GridQuboid(IEnumerable<T> elements)
        {
            EnclosedElements = new List<T>(elements);

            bool init = true;
            foreach (var element in EnclosedElements)
            {
                if (init)
                {
                    GridLeft = element.Column;
                    GridBack = element.Row;
                    GridDown = element.Level;
                    gridWidth = 1;
                    gridLength = 1;
                    gridHeight = 1;
                    init = false;
                }
                else
                {
                    GridLeft = Math.Min(GridLeft, element.Column);
                    GridBack = Math.Min(GridBack, element.Row);
                    GridDown = Math.Min(GridDown, element.Level);
                    gridWidth = Math.Max(GridWidth, (element.Column - GridLeft) + 1);
                    gridLength = Math.Max(GridLength, (element.Row - GridBack) + 1);
                    gridHeight = Math.Max(GridHeight, (element.Level - GridDown) + 1);
                }
            }
        }
    }
    public interface IGridQuboid
    {
        int GridWidth { get; }
        int GridLength { get; }
        int GridHeight { get; }
    }

    public static class GridQuboidExtensions
    {
        public static LWQuboidPlace Quboid<T>(this GridQuboid<T> gquboid) where T : IConnectedGridElement3D, ILocalizableGridElement3D, IHas<IQuboidPlaceLogic>
        {
            LWQuboidPlace quboid = new LWQuboidPlace();
            bool init = true;
            foreach (var element in gquboid.EnclosedElements)
            {
                if (init)
                {
                    element.TakeSimpleValues<IQuboidPlaceLogic>(quboid);
                    init = false;
                }
                else
                {
                    quboid.Enclose(element);
                }
            }
            return quboid;
        }
        public static int TotalGridCells(this IEnumerable<IGridQuboid> gridQuboids)
        {
            int sum = 0;
            foreach (var item in gridQuboids)
            {
                sum += item.GridVolume();
            }
            return sum;
        }
        public static int GridVolume(this IGridQuboid gridQuboid)
        {
            return gridQuboid.GridLength * gridQuboid.GridWidth * gridQuboid.GridHeight;
        }
    }
}
