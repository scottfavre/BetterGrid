using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BetterGrid
{
    public class BetterGridControl: Control
    {
        public const int RowArrayIndex = 0;
        public const int ColArrayIndex = 1;

        private const int SplitterSize = 1;

        private Grid _grid;

        private GridLoc? _selectOrigin;

        private Dictionary<ICell, BetterGridCell> _cellLookup;

        public BetterGridControl()
        {
            _cellLookup = new Dictionary<ICell, BetterGridCell>();
            SetValue(SelectedCellsProperty, new ObservableCollection<ICell>());
            Focusable = true;
            FocusManager.SetIsFocusScope(this, true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _grid = GetTemplateChild("PART_Grid") as Grid;
        }


        public ICell this[GridLoc loc]
        {
            get { return Cells[loc.Row, loc.Col]; }
        }


        public ObservableCollection<ICell> SelectedCells
        {
            get { return (ObservableCollection<ICell>)GetValue(SelectedCellsProperty.DependencyProperty); }
        }

        // Using a DependencyProperty as the backing store for SelectedCells.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey SelectedCellsProperty =
            DependencyProperty.RegisterReadOnly("SelectedCells", typeof(ObservableCollection<ICell>), typeof(BetterGridControl), new PropertyMetadata(null));

        
        
        
        
        
        public ICell EditingCell
        {
            get { return (ICell)GetValue(EditingCellProperty.DependencyProperty); }
            private set { SetValue(EditingCellProperty, value); }
        }
        public static readonly DependencyPropertyKey EditingCellProperty =
            DependencyProperty.RegisterReadOnly("EditingCell", typeof(ICell), typeof(BetterGridControl), new PropertyMetadata(null));



        public ICell[,] Cells
        {
            get { return (ICell[,])GetValue(CellsProperty); }
            set { SetValue(CellsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cells.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellsProperty =
            DependencyProperty.Register("Cells", typeof(ICell[,]), typeof(BetterGridControl), new FrameworkPropertyMetadata(null, OnCellsChanged));

        private static void OnCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bg = d as BetterGridControl;

            if (bg == null) return;

            var cells = bg.Cells;
            var grid = bg._grid;

            int numRows = cells.GetLength(RowArrayIndex);
            int numCols = cells.GetLength(ColArrayIndex);

            var rowHeight = new GridLength(25);
            var colWidth = new GridLength(120);
            var cellMargin = new Thickness(1);

            var tb = new TextBlock()
            {
                Text = "Hello World",
            };

            tb.SetValue(Grid.RowProperty, 1);
            tb.SetValue(Grid.ColumnProperty, 1);

            grid.Children.Add(tb);

            grid.RowDefinitions.Add(new RowDefinition() { Height = rowHeight });                // row header
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });  // col header


            for(int row = 1; row <= numRows; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = rowHeight });

                if(row > 1)
                    grid.Children.Add(CreateRowResize(numCols, row, GridResizeBehavior.PreviousAndCurrent, VerticalAlignment.Top));
                grid.Children.Add(CreateRowResize(numCols, row, GridResizeBehavior.CurrentAndNext, VerticalAlignment.Bottom));
                

                var header = new BetterGridHeader() 
                {
                    Content = row, 
                    Margin = cellMargin,
                    HeaderType = HeaderType.Row,
                    Index = row - 1,
                };
                Grid.SetRow(header, row);
                Grid.SetColumn(header, 0);
                grid.Children.Add(header);
            }
            for(int col = 1; col <= numCols; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = colWidth });

                if(col > 1)
                    grid.Children.Add(CreateColResize(numRows, col, HorizontalAlignment.Left, GridResizeBehavior.PreviousAndCurrent));
                grid.Children.Add(CreateColResize(numRows, col, HorizontalAlignment.Right, GridResizeBehavior.CurrentAndNext));
                

                var header = new BetterGridHeader()
                {
                    Content = col,
                    Margin = cellMargin,
                    HeaderType = HeaderType.Column,
                    Index = col - 1,
                };
                Grid.SetColumn(header, col);
                grid.Children.Add(header);
            }

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0) });       // row for resizing the last row
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0) });  // col for resizing the last col

            foreach(var cell in cells)
            {
                var bgc = new BetterGridCell();
                bgc.Cell = cell;
                bgc.Margin = cellMargin;

                grid.Children.Add(bgc);
                bg._cellLookup.Add(cell, bgc);
            }
        }

        private static GridSplitter CreateColResize(int numRows, int col, HorizontalAlignment alignment, GridResizeBehavior behavior)
        {
            var splitter = new GridSplitter()
            {
                ResizeBehavior = behavior,
                ResizeDirection = GridResizeDirection.Columns,
                Width = SplitterSize,
                Cursor = Cursors.SizeWE,
                HorizontalAlignment = alignment,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            splitter.SetValue(Grid.RowSpanProperty, numRows + 2);
            splitter.SetValue(Grid.ColumnProperty, col);
            return splitter;
        }

        private static GridSplitter CreateRowResize(int numCols, int row, GridResizeBehavior behavior, VerticalAlignment align)
        {
            var splitter = new GridSplitter()
            {
                ResizeBehavior = behavior,
                ResizeDirection = GridResizeDirection.Rows,
                Height = SplitterSize,
                Cursor = Cursors.SizeNS,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = align,
            };
            splitter.SetValue(Grid.ColumnSpanProperty, numCols + 2);
            splitter.SetValue(Grid.RowProperty, row);
            return splitter;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();

            var hitLoc = e.GetPosition(this);

            BetterGridCell cell = null;
            BetterGridHeader header = null;

            DependencyObject control = e.OriginalSource as DependencyObject;

            while (control != null && cell == null && header == null)
            {
                if (control is BetterGridCell)
                    cell = control as BetterGridCell;
                else if (control is BetterGridHeader)
                    header = control as BetterGridHeader;
                else
                    control = VisualTreeHelper.GetParent(control);
            }

            if (cell != null)
            {
                ClearSelection();
                SelectCell(cell.Cell);
                cell.Focus();
                _selectOrigin = cell.Cell.Loc;
            }
            else if (header != null)
            {
                switch (header.HeaderType)
                {
                    case HeaderType.Row:
                        ClearSelection();
                        for (int col = 0; col < Cells.GetLength(ColArrayIndex); col++)
                        {
                            SelectCell(Cells[header.Index, col]);
                        }
                        break;
                    case HeaderType.Column:
                        ClearSelection();
                        for (int row = 0; row < Cells.GetLength(RowArrayIndex); row++)
                        {
                            SelectCell(Cells[row, header.Index]);
                        }
                        break;
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            GridLoc newLoc;

            Console.WriteLine("B: Select Origin: " + _selectOrigin);

            if(CheckArrowKeys(e, out newLoc))
            {
                StopEditing();
                if(Keyboard.Modifiers == ModifierKeys.None)
                {
                    _selectOrigin = null;
                    ClearSelection();
                    SelectCell(newLoc);
                    _selectOrigin = newLoc;
                }
                else if(Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    SelectCell(newLoc);
                }
                e.Handled = true;
            }
            else if(e.Key == Key.E)
            {
                var focusedCell = FocusManager.GetFocusedElement(this) as BetterGridCell;
                if (focusedCell == null)
                    return;

                EditCell(focusedCell.Cell);
            }
            Console.WriteLine("E: Select Origin: " + _selectOrigin);
        }




        private bool CheckArrowKeys(KeyEventArgs e, out GridLoc newLoc)
        {
            newLoc = new GridLoc(0, 0);

            var fe = FocusManager.GetFocusedElement(this);
            var focusedCell = FocusManager.GetFocusedElement(this) as BetterGridCell;

            if(focusedCell == null) 
                return false;

            newLoc = focusedCell.Cell.Loc;

            switch (e.Key)
            {
                case Key.Right:
                    if (newLoc.Col < Cells.GetLength(ColArrayIndex) - 1)
                    {
                        newLoc = newLoc.Offset(0, 1);
                    }
                    break;
                case Key.Left:
                    if (newLoc.Col > 0)
                    {
                        newLoc = newLoc.Offset(0, -1);
                    }
                    break;
                case Key.Down:
                    if (newLoc.Row < Cells.GetLength(RowArrayIndex) - 1)
                    {
                        newLoc = newLoc.Offset(1, 0);
                    }
                    break;
                case Key.Up:
                    if (newLoc.Row > 0)
                    {
                        newLoc = newLoc.Offset(-1, 0);
                    }
                    break;
            }

            return newLoc != focusedCell.Cell.Loc;
        }
       

        private void SelectCell(ICell cell)
        {
            cell.IsSelected = true;
            SelectedCells.Add(cell);
            _cellLookup[cell].Focus();
        }

        private void SelectCell(GridLoc loc)
        {
            var cell = this[loc];

            if (_selectOrigin.HasValue)
            {
                ClearSelection();

                GridLoc start;
                GridLoc end;
                GridLoc.Rect(_selectOrigin.Value, loc, out start, out end);

                for(int r = start.Row; r <= end.Row; r++)
                {
                    for(int c = start.Col ; c <= end.Col; c++)
                    {
                        SelectCell(Cells[r, c]);
                    }
                }

                _cellLookup[cell].Focus();
            }
            else
            {
                SelectCell(cell);
                _selectOrigin = cell.Loc;
            }
        }

        private void DeselectCell(ICell cell)
        {
            cell.IsSelected = false;
            SelectedCells.Remove(cell);
        }

        private void ClearSelection()
        {
            foreach(var cell in SelectedCells)
            {
                cell.IsSelected = false;
            }
            SelectedCells.Clear();
        }

        private void EditCell(ICell cell)
        {
            ClearSelection();

            StopEditing();

            cell.IsEditing = true;

            EditingCell = cell;

            _selectOrigin = cell.Loc;
        }

        private void StopEditing()
        {
            if (EditingCell != null)
            {
                EditingCell.IsEditing = false;
                EditingCell = null;
            }
        }
    }
}
