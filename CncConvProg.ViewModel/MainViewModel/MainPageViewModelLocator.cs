using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using Framework.Implementors.Wpf.MVVM;

namespace CncConvProg.ViewModel.MainViewModel
{
    public class MainPageViewModelLocator : ViewModelLocatorBase<IMainViewModel>
    {
        public MainPageViewModelLocator()
        {
            DesigntimeViewModel = new DummyMainPageViewModel();
        }
    }
}