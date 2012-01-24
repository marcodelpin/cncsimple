namespace CncConvProg.Model.ConversationalStructure.Abstraction
{
    /// <summary>
    /// Interfaccia che contiene proprietà comuni per le lavorazioni di fresatura / foratura
    /// </summary>
    public interface IMillLeveable
    {
        double InizioLavorazioneZ { get; set; }

        double SicurezzaZ { get; set; }

        double ProfonditaLavorazione { get; set; }
    }

    /// <summary>
    /// Interfaccia che contiene proprietà comuni per le lavorazioni di fresatura
    /// </summary>
    public interface IMillWorkable : IMillLeveable
    {
        Operazione Sgrossatura { get; set; }

        Operazione Finitura { get; set; }

        Operazione Smussatura { get; set; }

        double ProfonditaFresaSmussatura { get; set; }

        bool FinishWithCompensation { get; set; }

        double SovrametalloFinituraProfilo { get; set; }

    }
}