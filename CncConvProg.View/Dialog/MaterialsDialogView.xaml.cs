using System.Windows;
using Framework.Abstractions.Wpf.Intefaces;

namespace CncConvProg.View.Dialog
{
    /// <summary>
    /// Interaction logic for EditWorkView.xaml
    /// </summary>
    public partial class MaterialsDialogView : Window, IModalWindow
    {
        public MaterialsDialogView()
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
