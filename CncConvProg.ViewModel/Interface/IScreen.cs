namespace CncConvProg.ViewModel.Interface
{
    public interface IScreen
    {
        IMainScreen Parent { get; }
        void Refresh();
    }
}
