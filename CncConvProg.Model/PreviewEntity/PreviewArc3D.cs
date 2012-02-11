using System;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.PreviewPathEntity;

namespace CncConvProg.Model.PreviewEntity
{
    [Serializable]
    public class PreviewArc3D : Arc3D, IPreviewEntity
    {
        public double Feed { get; set; }

        public ParametroVelocita ParametroVelocita { get; set; }

        public double GetMoveLength()
        {
            return GetLength();
        }

        public bool IsRapidMovement
        {
            get
            {
                return this.PlotStyle == EnumPlotStyle.RapidMove;
            }
        }

        //public TimeSpan GetTimeSpan()
        //{

        //    /**/
        //    return TimeHelper.CalcTime(this, Feed);
        //}
    }
}
