using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;

namespace CncConvProg.Model.ConversationalStructure.Abstraction.IPattern
{
    public interface IForaturaPatternable
    {
        PatternForatura Pattern { get; set; }
    }
}