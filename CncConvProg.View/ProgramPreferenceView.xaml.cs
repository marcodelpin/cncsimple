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
using System.Windows.Shapes;
using Framework.Abstractions.Wpf.Intefaces;

namespace CncConvProg.View
{
    /// <summary>
    /// Interaction logic for EditWorkView.xaml
    /// </summary>
    public partial class ProgramPreferenceView : Window , IModalWindow
    {
        public ProgramPreferenceView()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
