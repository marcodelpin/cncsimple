using System;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.PreviewPathEntity;

namespace CncConvProg.Model.PreviewPathEntity
{
    [Serializable]

    public class PreviewLine3D : Line3D, IPreviewEntity
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
        //    /*
        //     * ora qui ho limitatore giri, 
        //     */
        //    return TimeHelper.CalcTime(this, Feed);
        //}
    }
}
