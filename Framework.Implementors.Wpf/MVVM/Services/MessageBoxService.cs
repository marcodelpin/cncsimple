using System.Windows;
using Framework.Abstractions.Wpf.GenericMessageBox;
using Framework.Abstractions.Wpf.Intefaces;

namespace Framework.Implementors.Wpf.MVVM.Services
{
  public class MessageBoxService : IMessageBoxService
  {
    public GenericMessageBoxResult Show(string message, string caption, GenericMessageBoxButton buttons)
    {
      var slButtons = buttons == GenericMessageBoxButton.Ok
                        ? MessageBoxButton.OK
                        : MessageBoxButton.OKCancel;

      var result = MessageBox.Show(message, caption, slButtons);

      return result == MessageBoxResult.OK ? GenericMessageBoxResult.Ok : GenericMessageBoxResult.Cancel;
    }

      public void Show(string message, string caption)
      {
          MessageBox.Show(message, caption, MessageBoxButton.OK);
      }
  }
}