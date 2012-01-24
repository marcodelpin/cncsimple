using System.Collections.Generic;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Abstraction
{
    /// <summary>
    /// Interfaccia utilizzata quando certi screen necessita di anteprima personalizzata.
    /// </summary>
    public interface IPreviewable
    {
        IEnumerable<IEntity3D> GetPreview();
    }
}