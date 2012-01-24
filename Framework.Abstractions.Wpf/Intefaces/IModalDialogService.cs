using System;
using System.Windows;

namespace Framework.Abstractions.Wpf.Intefaces
{
    public interface IModalDialogService
    {
        void ShowDialog<TViewModel>(IModalWindow view, TViewModel viewModel, Action<TViewModel> onDialogClose);

        void ShowDialog<TDialogViewModel>(IModalWindow view, TDialogViewModel viewModel);
    }
}