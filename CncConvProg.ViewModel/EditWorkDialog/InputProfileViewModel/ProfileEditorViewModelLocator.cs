using CncConvProg.ViewModel.MainViewModel;
using Framework.Implementors.Wpf.MVVM;

namespace CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel
{
    public class ProfileEditorViewModelLocator : ViewModelLocatorBase<IProfileEditorViewModel>
    {
        public ProfileEditorViewModelLocator()
        {
         //   DesigntimeViewModel = new DummyProfileEditorViewModel();
        }
    }
}