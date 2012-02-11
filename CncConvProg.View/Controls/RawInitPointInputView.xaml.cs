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
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;

namespace CncConvProg.View.Controls
{
    /// <summary>
    /// Interaction logic for RawLineInputView.xaml
    /// </summary>
    public partial class RawInitPointInputView : UserControl
    {
        public RawInitPointInputView()
        {
            InitializeComponent();
            PreviewKeyDown += new KeyEventHandler(RawLineInputView_PreviewKeyDown);

            Loaded += new RoutedEventHandler(RawInitPointInputView_Loaded);
            DataContextChanged += new DependencyPropertyChangedEventHandler(RawLineInputView_DataContextChanged);
        }

        void RawInitPointInputView_Loaded(object sender, RoutedEventArgs e)
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
            var registredKey = e.Key;

            var d = DataContext as RawItemViewModel;

            if (d != null)
            {
                 var keys = d.KeyboardDictionary;

                var finded = keys.TryGetValue(e.Key, out registredKey);

                if (!finded)
                    registredKey = e.Key;
            }
            if (registredKey == Key.X)
            {
                xTb.Focus();
                e.Handled = true;
            }

            else if (registredKey == Key.Y)
            {
                yTb.Focus();
                e.Handled = true;
            }
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {

            var i = sender as Image;

            if (i == null) return;

            var d = DataContext as RawItemViewModel;



            if (d == null) return;


            var imageSource = d.GetImageUri(i.Tag);



            if (string.IsNullOrWhiteSpace(imageSource)) return;



            var uriSource = new Uri(imageSource, UriKind.RelativeOrAbsolute);

            i.Source = new BitmapImage(uriSource);

        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}
