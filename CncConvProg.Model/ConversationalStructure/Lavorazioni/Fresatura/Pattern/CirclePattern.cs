using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    [Serializable]
    public class CirclePattern : IMillingPattern
    {
        public double Raggio { get; set; }

        public double CentroX { get; set; }

        public double CentroY { get; set; }

        public Profile2D GetClosedProfile()
        {

            var cerchio = new Arc2D
            {
                Center = new Point2D(CentroX, CentroY),
                Radius = Raggio,
                ClockWise = false,
                Start = { X = CentroX +  Raggio},
                End = { X = CentroX + Raggio },
            };


            cerchio.Start.Y = CentroY ;
            cerchio.End.Y = CentroY ;


            var profile2D = new Profile2D();

            profile2D.AddEntity(cerchio);

            cerchio.PlotStyle = EnumPlotStyle.Element;
            return profile2D;
        }
    }
}
