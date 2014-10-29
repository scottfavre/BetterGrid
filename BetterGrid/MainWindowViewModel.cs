using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterGrid
{
    public class MainWindowViewModel
    {
        public ICell[,] Cells { get; set; }

        public MainWindowViewModel()
        {
            Cells = new ICell[10, 10];

            for (int r = 0; r < Cells.GetLength(0); r++)
                for (int c = 0; c < Cells.GetLength(1); c++)
                    Cells[r, c] = new CellImpl() { Loc = new GridLoc(r,c) };
        }
    }
}
