using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBase
{
    public class TwoDGrid
    {
        public List<double> Rows { get; set; }
        public List<double> Columns { get; set; }

        public GridCell<Two2DGridCellInfo>[,] Cells;

        public void Stance(double left, double front, double right, double back, double height)
        {
            AddColumn(left);
            AddRow(front);
            AddColumn(right);
            AddRow(back);

            for (int c = 0; c < Columns.Count; c++)
            {
                for (int r = 0; r < Rows.Count; r++)
                {
                    Cells[c, r].Data.Height = height;
                }
            }

            MergeCells();
        }

        private void MergeCells()
        {
            for (int row = 0; row < Rows.Count - 1; row++)
            {
                bool mergeable = true;
                for (int col = 0; col < Columns.Count; col++)
                {
                    if (Cells[col, row].Data.Height != Cells[col, row + 1].Data.Height)
                    {
                        mergeable = false;
                        break;
                    }
                }
                if (mergeable)
                {
                    RemoveRow(Rows[row + 1]);
                }
            } 
            for (int col = 0; col < Columns.Count-1; col++)            
            {
                bool mergeable = true;
                for (int row = 0; row < Rows.Count; row++)
                {
                    if (Cells[col, row].Data.Height != Cells[col+1, row ].Data.Height)
                    {
                        mergeable = false;
                        break;
                    }
                }
                if (mergeable)
                {
                    RemoveColumn(Columns[col + 1]);
                }
            }
        }
        public void AddRow(double row)
        {
            if (!Columns.Contains(row))
            {
                var newCells = new GridCell<Two2DGridCellInfo>[Cells.GetLength(0), Cells.GetLength(1) + 1];

                //add new row
                Rows.Add(row);
                //divide cells
                var index = Rows.IndexOf(row);
                //insert new row
                for (int rw = 0; rw < newCells.GetLength(1); rw++)
                {
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        if (rw <= index)
                        {
                            newCells[i, rw] = Cells[i, rw];
                        }
                        else if (rw == index + 1)
                        {
                            newCells[i, rw] = new GridCell<Two2DGridCellInfo>(newCells[i, rw - 1].Data.Clone());
                        }
                        else
                        {
                            newCells[i, rw] = Cells[i, rw - 1];
                        }
                    }
                }
                //connect new cells
                for (int i = 0; i < Rows.Count; i++)
                {
                    newCells[i, index + 1].Connect(newCells[i, index], GridDirection.Front);
                    if (index + 2 < Columns.Count)
                        newCells[i, index + 1].Connect(newCells[i, index + 2], GridDirection.Back);
                    if (i > 0)
                        newCells[i, index + 1].Connect(newCells[i - 1, index + 1], GridDirection.Left);
                }
                Cells = newCells;
            }
        }
        public void AddColumn(double column)
        {
            if (!Columns.Contains(column))
            {
                var newCells = new GridCell<Two2DGridCellInfo>[Cells.GetLength(0), Cells.GetLength(1) + 1];

                //add new column
                Columns.Add(column);
                //divide cells
                var index = Columns.IndexOf(column);
                //insert new column
                for (int col = 0; col < newCells.GetLength(0); col++)
                {
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        if (col <= index)
                        {
                            newCells[col, i] = Cells[col, i];
                        }
                        else if (col == index + 1)
                        {
                            newCells[col, i] = new GridCell<Two2DGridCellInfo>(newCells[col - 1, i].Data.Clone());
                        }
                        else
                        {
                            newCells[col, i] = Cells[col - 1, i];
                        }
                    }
                }
                //connect new cells
                for (int i = 0; i < Rows.Count; i++)
                {
                    newCells[index + 1, i].Connect(newCells[index, i], GridDirection.Left);
                    if (index + 2 < Columns.Count)
                        newCells[index + 1, i].Connect(newCells[index + 2, i], GridDirection.Right);
                    if (i > 0)
                        newCells[index + 1, i].Connect(newCells[index + 1, i - 1], GridDirection.Front);
                }
                Cells = newCells;
            }
        }
        public void RemoveColumn(double column)
        {
            if (Columns.Contains(column))
            {
                var newCells = new GridCell<Two2DGridCellInfo>[Cells.GetLength(0) - 1, Cells.GetLength(1)];

                var index = Columns.IndexOf(column);
                //remove column
                Columns.Remove(column);

                for (int col = 0; col < newCells.GetLength(0); col++)
                {
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        if (col < index)
                        {
                            newCells[col, i] = Cells[col, i];
                        }
                        else if (col == index && index > 0)
                        {
                            newCells[col, i] = Cells[col + 1, i];
                            newCells[col, i].Connect(newCells[col - 1, i], GridDirection.Left);
                        }
                        else
                        {
                            newCells[col, i] = Cells[col + 1, i];
                        }
                    }
                }
                Cells = newCells;
            }
        }
        public void RemoveRow(double row)
        {
            if (Rows.Contains(row))
            {
                var newCells = new GridCell<Two2DGridCellInfo>[Cells.GetLength(0), Cells.GetLength(1) - 1];

                var index = Rows.IndexOf(row);
                //remove column
                Rows.Remove(row);

                for (int col = 0; col < Columns.Count; col++)
                {
                    for (int r = 0; r < Rows.Count; r++)
                    {
                        if (r < index)
                        {
                            newCells[col, r] = Cells[col, r];
                        }
                        else if (r == index && index > 0)
                        {
                            newCells[col, r] = Cells[col, r + 1];
                            newCells[col, r].Connect(newCells[col, r - 1], GridDirection.Front);
                        }
                        else
                        {
                            newCells[col, r] = Cells[col, r + 1];
                        }
                    }
                }
                Cells = newCells;
            }
        }
    }

    public class Two2DGridCellInfo
    {
        public double Height { get; set; }

        public Two2DGridCellInfo Clone()
        {
            var newinfo = new Two2DGridCellInfo();
            newinfo.Height = Height;
            return newinfo;
        }
    }

    public enum GridDirection
    {
        Left, Front, Right, Back
    }

    public class GridCell<T>
    {
        public GridCell(T data)
        {
            Data = data;
        }
        public T Data { get; set; }

        public GridCell<T> LeftNB { get; set; }
        public GridCell<T> FrontNB { get; set; }
        public GridCell<T> RightNB { get; set; }
        public GridCell<T> BackNB { get; set; }

        public void Connect(GridCell<T> other, GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.Left:
                    if (LeftNB != null)
                        LeftNB.RightNB = null;
                    LeftNB = other;
                    LeftNB.RightNB = this;
                    break;
                case GridDirection.Front:
                    if (FrontNB != null)
                        FrontNB.BackNB = null;
                    FrontNB = other;
                    FrontNB.BackNB = this;
                    break;
                case GridDirection.Right:
                    if (RightNB != null)
                        RightNB.LeftNB = null;
                    RightNB = other;
                    RightNB.LeftNB = this;
                    break;
                case GridDirection.Back:
                    if (BackNB != null)
                        BackNB.FrontNB = null;
                    BackNB = other;
                    BackNB.FrontNB = this;
                    break;
                default:
                    break;
            }
        }
    }

    //public static class GridX
    //{
    //    public static TwoDGrid Stance
    //}
}
