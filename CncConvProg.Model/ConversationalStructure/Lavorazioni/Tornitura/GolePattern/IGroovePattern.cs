using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura.GolePattern
{
    /// <summary>
    /// Interfaccia per creare patter per gola.
    /// Simile a situazione per fresatura .
    /// Oltre ad avere profile editor ho pattern per immissione veloce delle gole.
    /// </summary>
    public interface IGroovePattern
    {
        Profile2D Profile { get; }
    }

    public enum TurnGroovePattern
    {
        ProfiloLibero,
        VShape,
        Standard,
    }

    /// <summary>
    /// Obsolete
    /// </summary>
    public enum GrooveDirection
    {
        Extern,
        Intern,
        Face
    }
}
