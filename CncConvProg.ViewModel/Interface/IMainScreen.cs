using System;

namespace CncConvProg.ViewModel.Interface
{
    public interface IMainScreen
    {
        IScreen CurrentScreen { get; set; }

        void ShowScreen(IScreen screen);
        void ShowDialog(IScreen screen);

        void DoWork(Action action);
        void ScreenLoaded();

    }
}
