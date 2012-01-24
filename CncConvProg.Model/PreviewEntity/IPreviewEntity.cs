using System;
using System.Collections.Generic;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.PreviewPathEntity;

namespace CncConvProg.Model.PreviewPathEntity
{
    public enum VelocitaType
    {
        Sync,
        ASync
    }

    [Serializable]
    public class ParametroVelocita
    {
        // 0 giri fissi 1 giri variabili
        public VelocitaType ModoVelocita { get; set; }

        // 0 mm/min 1 mm/giro
        public VelocitaType ModoAvanzamento { get; set; }

        public double ValoreVelocita { get; set; }

        public double ValoreFeed { get; set; }
    }

    public interface IPreviewEntity
    {

        ParametroVelocita ParametroVelocita { get; set; }
        // magari fare set move type speed type 
        double GetMoveLength();

        bool IsRapidMovement { get; }

        // TimeSpan GetTimeSpan();


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
