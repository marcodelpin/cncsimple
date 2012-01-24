using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    [Serializable]
    public class CavaArcoPattern : IMillingPattern
    {
        public double AngoloStart { get; set; }

        public double AngoloAmpiezza { get; set; }

        public double RaggioInterasse { get; set; }

        public double CentroX { get; set; }

        public double CentroY { get; set; }

        public double Larghezza { get; set; }

        public Profile2D GetClosedProfile()
        {
            var interasseArc = new Arc2D() { Center = new Point2D(CentroX, CentroY) };

            var clockWise = false;

            var angoloStartRad = GeometryHelper.DegreeToRadian(AngoloStart);
            var angoloEndRad = GeometryHelper.DegreeToRadian(AngoloStart + AngoloAmpiezza);

            interasseArc.Radius = RaggioInterasse;
            interasseArc.ClockWise = false;

            interasseArc.Start = GeometryHelper.GetCoordinate(angoloStartRad, RaggioInterasse);
            interasseArc.End = GeometryHelper.GetCoordinate(angoloEndRad, RaggioInterasse);

            var offset = Larghezza / 2;
            var arc1 = GeometryHelper.GetParallel(interasseArc, offset, true);
            arc1.ClockWise = false;
            var arc2 = GeometryHelper.GetParallel(interasseArc, offset, false);
            arc2.ClockWise = true;

            var temp = arc2.Start;

            arc2.Start = arc2.End;
            arc2.End = temp;

            var arcEndCaps = new Arc2D()
                                 {
                                     Center = interasseArc.End,
                                     ClockWise = true,
                                     Start = arc1.End,
                                     End = arc2.Start,
                                     Radius = Larghezza / 2,
                                 };

            var arcStartCaps = new Arc2D()
            {
                Center = interasseArc.Start,
                ClockWise = true,
                Start = arc2.End,
                End = arc1.Start,
                Radius = Larghezza / 2,
            };

            var profile2D = new Profile2D();

            profile2D.AddEntity(arc1);
            profile2D.AddEntity(arcEndCaps);
            profile2D.AddEntity(arc2);
            profile2D.AddEntity(arcStartCaps);

           arc1.PlotStyle = arc2.PlotStyle = arcStartCaps.PlotStyle = arcEndCaps.PlotStyle = EnumPlotStyle.Element;
            return profile2D;
        }
    }
}