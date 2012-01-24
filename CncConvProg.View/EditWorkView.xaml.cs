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
using CncConvProg.ViewModel.EditWorkDialog;
using Framework.Abstractions.Wpf.Intefaces;

namespace CncConvProg.View
{
    /// <summary>
    /// Interaction logic for EditWorkView.xaml
    /// </summary>
    public partial class EditWorkView : Window, IModalWindow
    {
        public EditWorkView()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(EditWorkView_PreviewKeyDown);

            // farlo con imodalwindow ..
            Owner = Application.Current.MainWindow;



        }

        void EditWorkView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is EditWorkViewModel)
            {
                var vm = DataContext as EditWorkViewModel;

                if (vm.SelectedScreen != null && vm.SelectedScreen is IHandleKeyable)
                {
                    var screen = vm.SelectedScreen as IHandleKeyable;

                    // piccolo hack : se si tratta di pgup o pgdown ,( quando cambio elemento)  muovo il focus per aggiornare il campo, che altrimenti non andrebbe aggiornato
                    if (e.Key == Key.PageDown || e.Key == Key.PageUp)
                    {
                        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                    }

                    if (e.Key == Key.Enter)
                    {
                        e.Handled = true;
                        KeyEventArgs eInsertBack = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab);
                        eInsertBack.RoutedEvent = UIElement.KeyDownEvent;
                        InputManager.Current.ProcessInput(eInsertBack);
                    }

                    screen.HandleKeyDown(e);
                }
            }
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
