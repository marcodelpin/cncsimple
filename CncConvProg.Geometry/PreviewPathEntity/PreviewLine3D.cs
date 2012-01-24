using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.PreviewPathEntity
{
    [Serializable]

    public class PreviewLine3D : Line3D, IPreviewEntity
    {
        public double Feed { get; set; }

        public double GetMoveLength
        {
            get { return GetLength(); }
        }

        public bool IsRapidMovement
        {
            get
            {
                return this.PlotStyle == EnumPlotStyle.RapidMove;
            }
        }

        public TimeSpan GetTimeSpan()
        {
            return TimeHelper.CalcTime(this, Feed);
        }
    }
}
