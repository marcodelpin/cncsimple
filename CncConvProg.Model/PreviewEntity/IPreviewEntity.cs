using System.Collections.Generic;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.PreviewPathEntity;

namespace CncConvProg.Model.PreviewEntity
{
  
    public interface IPreviewEntity
    {
        ParametroVelocita ParametroVelocita { get; set; }

        double GetMoveLength();

        bool IsRapidMovement { get; }
    }

    public static class PreviewEntityHelper
    {
        public static List<IEntity3D> GetIEntity3DFromIPreviewEntity(List<IPreviewEntity> preview)
        {
            var rslt = new List<IEntity3D>();

            if (preview != null)
                foreach (var entity3D in preview)
                {
                    if (entity3D is PreviewArc3D)
                        rslt.Add(entity3D as Arc3D);

                    else if (entity3D is PreviewLine3D)
                        rslt.Add(entity3D as Line3D);
                }

            return rslt;

        }
    }
}
