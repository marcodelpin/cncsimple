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

}
