using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    [Serializable]
    public class RettangoloPattern : IMillingPattern
    {
        public double Larghezza { get; set; }

        public double Altezza { get; set; }

        public double PuntoStartX { get; set; }

        public double PuntoStartY { get; set; }

        public SquareShapeHelper.SquareShapeStartPoint StartPoint { get; set; }

        public double Chamfer { get; set; }

        public bool ChamferAbilited { get; set; }

        public Profile2D GetClosedProfile()
        {
            var p = new RawProfile(false);

            var centerPoint = SquareShapeHelper.GetCenterPoint(StartPoint, PuntoStartX, PuntoStartY, Larghezza, Altezza);


            var ini = new RawInitPoint2D(p);
            ini.X.SetValue(true, centerPoint.X + Larghezza / 2);
            ini.Y.SetValue(true, centerPoint.Y + Altezza / 2);
            p.Add(ini);

            var p1 = new RawLine2D(p);
            p1.X.SetValue(true, centerPoint.X + Larghezza / 2);
            p1.Y.SetValue(true, centerPoint.Y - Altezza / 2);
            p.Add(p1);

            var p2 = new RawLine2D(p);
            p2.X.SetValue(true, centerPoint.X - Larghezza / 2);
            p2.Y.SetValue(true, centerPoint.Y - Altezza / 2);
            p.Add(p2);

            var p3 = new RawLine2D(p);
            p3.X.SetValue(true, centerPoint.X - Larghezza / 2);
            p3.Y.SetValue(true, centerPoint.Y + Altezza / 2);
            p.Add(p3);

            var p4 = new RawLine2D(p);
            p4.X.SetValue(true, centerPoint.X + Larghezza / 2);
            p4.Y.SetValue(true, centerPoint.Y + Altezza / 2);
            p.Add(p4);

            if(Chamfer > 0 && ChamferAbilited)
            {
                p1.Chamfer.SetValue(true,Chamfer);
                p2.Chamfer.SetValue(true, Chamfer);
                p3.Chamfer.SetValue(true, Chamfer);
                p4.Chamfer.SetValue(true, Chamfer);
            }
            else if(Chamfer > 0 && !ChamferAbilited)
            {
                p1.EndRadius.SetValue(true, Chamfer);
                p2.EndRadius.SetValue(true, Chamfer);
                p3.EndRadius.SetValue(true, Chamfer);
                p4.EndRadius.SetValue(true, Chamfer);
            }

            var rslt = p.GetProfileResult(true);
            rslt.SetPlotStyle();

            return rslt;
        }
    }
}
