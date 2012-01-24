using System;
using System.Windows.Input;

namespace Framework.Abstractions.Wpf.Intefaces
{
    public interface IHandleKeyable
    {
        void HandleKeyDown(KeyEventArgs e);

    }
}