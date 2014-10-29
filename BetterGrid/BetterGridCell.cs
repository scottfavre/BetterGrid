using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace BetterGrid
{
    public class BetterGridCell: Control
    {
        public BetterGridCell()
        {
            Focusable = true;
        }

        public ICell Cell
        {
            get { return (ICell)GetValue(CellProperty); }
            set { SetValue(CellProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cell.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellProperty =
            DependencyProperty.Register("Cell", typeof(ICell), typeof(BetterGridCell), new FrameworkPropertyMetadata(null, OnCellChanged));

        private static void OnCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bgc = d as BetterGridCell;

            var cell = bgc.Cell;
            bgc.DataContext = cell;

            Grid.SetColumn(bgc, cell.Loc.Col + 1);
            Grid.SetRow(bgc, cell.Loc.Row + 1);
        }



        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(BetterGridCell), new PropertyMetadata(false));




        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(BetterGridCell), new PropertyMetadata(false));


    }
}
