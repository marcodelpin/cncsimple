using System.Windows.Input;

namespace CncConvProg.ViewModel
{
    public interface IProgramDevelopViewModel
    {
        ICommand StartSimulationCmd { get; }
    }
}