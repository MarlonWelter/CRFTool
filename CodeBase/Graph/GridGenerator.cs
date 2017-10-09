using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class GridGenerator
    {
        public delegate T Factory<T>();
        public static T[, ,] GenerateGrid<T>(Factory<T> factory, int columns, int rows, int levels) where T : IConnectedGridElement3D
        {
            if (columns < 1 || rows < 1 || levels < 1)
                throw new ArgumentException("wrong input for ExecuteMany");

            T[, ,] GridArray = new T[columns, rows, levels];
            var rootColumn = default(T);
            for (int column = 0; column < columns; column++)
            {
                var rootRow = rootColumn;
                for (int row = 0; row < rows; row++)
                {
                    var currentElement = rootRow;
                    for (int level = 0; level < levels; level++)
                    {
                        currentElement = factory();
                        GridArray[column, row, level] = currentElement;
                        if (column > 0)
                        {
                            currentElement.Connect(GridArray[column - 1, row, level], Direction.Left);
                        }
                        if (row > 0)
                        {
                            currentElement.Connect(GridArray[column, row - 1, level], Direction.Back);
                        }
                        if (level > 0)
                        {
                            currentElement.Connect(GridArray[column, row, level - 1], Direction.Down);
                        }

                        currentElement = currentElement.NeighbourUp();
                    }
                    rootRow = rootRow.NeighbourFront();
                }
                rootColumn = rootColumn.NeighbourRight();
            }
            return GridArray;
        }

        public delegate T FactoryLocalizable<T>(int column, int row, int level);
        public static T[, ,] GenerateGridLocalizable<T>(FactoryLocalizable<T> factory, int columns, int rows, int levels) where T : IConnectedGridElement3D, ILocalizableGridElement3D
        {
            if (columns < 1 || rows < 1 || levels < 1)
                throw new ArgumentException("wrong input for ExecuteMany");

            T[, ,] GridArray = new T[columns, rows, levels];
            var rootColumn = default(T);
            for (int column = 0; column < columns; column++)
            {
                if (rootColumn == null)
                    rootColumn = CreateElement<T>(factory, GridArray, column, 0, rootColumn, 0);
                var rootRow = rootColumn;
                for (int row = 0; row < rows; row++)
                {
                    if (rootRow == null)
                        rootRow = CreateElement<T>(factory, GridArray, column, row, rootRow, 0);
                    var currentElement = rootRow;
                    for (int level = 0; level < levels; level++)
                    {
                        if (currentElement == null)
                            currentElement = CreateElement<T>(factory, GridArray, column, row, currentElement, level);

                        currentElement = currentElement.NeighbourUp();
                    }
                    rootRow = rootRow.NeighbourFront();
                }
                rootColumn = rootColumn.NeighbourRight();
            }
            return GridArray;
        }


        private static T CreateElement<T>(FactoryLocalizable<T> factory, T[, ,] GridArray, int column, int row, T currentElement, int level) where T : IConnectedGridElement3D, ILocalizableGridElement3D
        {
            currentElement = factory(column, row, level);

            GridArray[column, row, level] = currentElement;
            if (column > 0)
            {
                currentElement.Connect(GridArray[column - 1, row, level], Direction.Left);
            }
            if (row > 0)
            {
                currentElement.Connect(GridArray[column, row - 1, level], Direction.Back);
            }
            if (level > 0)
            {
                currentElement.Connect(GridArray[column, row, level - 1], Direction.Down);
            }
            return currentElement;
        }
    }
}
