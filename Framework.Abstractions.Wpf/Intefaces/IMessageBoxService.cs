using Framework.Abstractions.Wpf.GenericMessageBox;

namespace Framework.Abstractions.Wpf.Intefaces
{
  public interface IMessageBoxService
  {
    GenericMessageBoxResult Show(string message, string caption, GenericMessageBoxButton buttons);
    void Show(string message, string caption);
  }
}