using System;
using System.Windows;

namespace Framework.Abstractions.Wpf.Intefaces
{
    public interface IModalWindow
    {
        bool? DialogResult { get; set; }
        event EventHandler Closed;
        void Show();
        bool? ShowDialog();
        object DataContext { get; set; }
        void Close();
    }
}