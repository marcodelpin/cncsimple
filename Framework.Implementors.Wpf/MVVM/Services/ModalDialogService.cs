using System;
using System.Windows;
using Framework.Abstractions.Wpf.Intefaces;

namespace Framework.Implementors.Wpf.MVVM.Services
{
    public class ModalDialogService : IModalDialogService
    {
        public void ShowDialog<TDialogViewModel>(IModalWindow view, TDialogViewModel viewModel, Action<TDialogViewModel> onDialogClose)
        {
            try
            {
                view.DataContext = viewModel;
                if (onDialogClose != null)
                {
                    view.Closed += (sender, e) => onDialogClose(viewModel);
                }
                view.ShowDialog();
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public void ShowDialog<TDialogViewModel>(IModalWindow view, TDialogViewModel viewModel)
        {
            this.ShowDialog(view, viewModel, null);
        }
    }
}