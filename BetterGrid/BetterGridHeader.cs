using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BetterGrid
{
    public class BetterGridHeader: ContentControl
    {



        public HeaderType HeaderType
        {
            get { return (HeaderType)GetValue(HeaderTypeProperty); }
            set { SetValue(HeaderTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTypeProperty =
            DependencyProperty.Register("HeaderType", typeof(HeaderType), typeof(BetterGridHeader), new PropertyMetadata(HeaderType.Row));




        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(BetterGridHeader), new PropertyMetadata(0));



    }

    public enum HeaderType
    {
        Row,
        Column,
    }
}
