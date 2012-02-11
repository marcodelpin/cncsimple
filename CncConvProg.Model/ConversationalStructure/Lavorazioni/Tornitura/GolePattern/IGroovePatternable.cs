using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura.GolePattern
{
    public interface IGroovePatternable
    {
        TurnGroovePattern GroovePattern { get; set; }

        IGroovePattern Pattern { get; }

    }
}