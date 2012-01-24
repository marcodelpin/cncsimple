using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CncConvProg.View.Controls
{
    /// <summary>
    /// Interaction logic for RawLineInputView.xaml
    /// </summary>
    public partial class RawArcInputView : UserControl
    {
        public RawArcInputView()
        {
            InitializeComponent();
            PreviewKeyDown += RawLineInputView_PreviewKeyDown;

            DataContextChanged += RawLineInputView_DataContextChanged;

            Loaded += RawArcInputView_Loaded;
        }

        void RawArcInputView_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                xTb.Focus();
        }

        void RawLineInputView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded)
                xTb.Focus();
        }

        void RawLineInputView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                xTb.Focus();
                e.Handled = true;
            }

            else if (e.Key == Key.Y)
            {
                yTb.Focus();
                e.Handled = true;
            }

            else if (e.Key == Key.U)
            {
                uTb.Focus();
                e.Handled = true;
            }

            else if (e.Key == Key.V)
            {
                vTb.Focus();
                e.Handled = true;

            }
            else if (e.Key == Key.R)
            {
                if (rTb.IsFocused)
                    r1Tb.Focus();
                else
                    rTb.Focus();

                e.Handled = true;
            }

            else if (e.Key == Key.C)
            {
                cTb.Focus();
                e.Handled = true;
            }

            else if (e.Key == Key.A)
            {
                if (cbAa.IsChecked.HasValue && cbAa.IsChecked.Value)
                    cbAa.IsChecked = false;
                else
                    cbAa.IsChecked = true;

                e.Handled = true;
            }

            else if (e.Key == Key.D)
            {
                if (rbCw.IsChecked.HasValue && rbCw.IsChecked.Value)
                    rbCcw.IsChecked = true;
                else
                    rbCw.IsChecked = true;

                e.Handled = true;
            }
        }
    }
}
