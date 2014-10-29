using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BetterGrid
{
    public interface ICell: INotifyPropertyChanged
    {
        GridLoc Loc { get; }
        Thickness Border { get; }
        bool IsSelected { get; set; }
        bool IsFocused { get; set; }
        bool IsEditing { get; set; }
    }
}
