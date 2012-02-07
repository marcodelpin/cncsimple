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
    public class CavaDrittaApertaPattern : IOpenMillingPattern
    {
        public double Radius { get; set; }

        public double CentroX { get; set; }

        public double CentroY { get; set; }

        public double LunghezzaCentroLato { get; set; }

        public double ChamferValue { get; set; }

        /*
         * Magari aggiungere inclinazione
         */

        public Profile2D GetClosedProfile()
        {
            /*
             * Come coordinate 0 prendo centro raggio.
             * 
             * Allora primo sono sulla faccia esterna del pezzo
             * 
             * poi scendo ( faccio smusso raccordo se esiste )
             * 
             * vado a fare raccordo 
             * 
             * esco sulla faccia del pezzo
             * 
             * poi aggirando tutto il profilo chiudo
             * 
             * in modo da creare profilo esterno da trimmare.
             */
            var rawProfile = new RawProfile(false);

            if (Radius <= 0 || LunghezzaCentroLato <= 0) return new Profile2D();

            /* Punto iniziale*/
            var initPoint2D = new RawInitPoint2D(rawProfile);
            initPoint2D.X.SetValue(true, CentroX + LunghezzaCentroLato);
            initPoint2D.Y.SetValue(true, CentroY + Radius + ChamferValue * 1.2 + 1);
            initPoint2D.PlotStyle = EnumPlotStyle.Element;


            /*Punto 1*/
            var line1 = new RawLine2D(rawProfile);
            line1.Y.SetValue(true, CentroY + Radius);
            line1.PlotStyle = EnumPlotStyle.Element;
            if (ChamferValue > 0)
                line1.Chamfer.SetValue(true, ChamferValue);

            /*Inizio Arco*/
            var line2 = new RawLine2D(rawProfile);
            line2.PlotStyle = EnumPlotStyle.Element;
            line2.X.SetValue(true, CentroX);

            /*Arco*/
            var arc = new RawArc2D(rawProfile);
            arc.PlotStyle = EnumPlotStyle.Element;

            arc.Radius.SetValue(true, Radius);
            arc.IsClockwise = false;
            arc.Y.SetValue(true, CentroY - Radius);
            arc.CenterX.SetValue(true, CentroX);
            arc.CenterY.SetValue(true, CentroY);

            /*Faccia pezzo*/
            var line3 = new RawLine2D(rawProfile);
            line3.PlotStyle = EnumPlotStyle.Element;
            line3.X.SetValue(true, CentroX + LunghezzaCentroLato);
            if (ChamferValue > 0)
                line3.Chamfer.SetValue(true, ChamferValue);

            /*Scendo pezzo*/
            var line4 = new RawLine2D(rawProfile);
            line4.PlotStyle = EnumPlotStyle.Element;
            line4.Y.SetValue(true, CentroY - Radius - ChamferValue * 1.2 - 1);

            /* Aggiro in 3 linee*/
            var line5 = new RawLine2D(rawProfile);
            line5.PlotStyle = EnumPlotStyle.Invisible;
            line5.X.SetValue(true, CentroX - Radius * 2);

            var line6 = new RawLine2D(rawProfile);
            line6.PlotStyle = EnumPlotStyle.Invisible;
            line6.Y.SetValue(true, initPoint2D.Y.Value);

            var line7 = new RawLine2D(rawProfile);
            line7.PlotStyle = EnumPlotStyle.Invisible;
            line7.X.SetValue(true, initPoint2D.X.Value);

            rawProfile.Add(initPoint2D);
            rawProfile.Add(line1);
            rawProfile.Add(line2);
            rawProfile.Add(arc);
            rawProfile.Add(line3);
            rawProfile.Add(line4);
            rawProfile.Add(line5);
            rawProfile.Add(line6);
            rawProfile.Add(line7);

            var profile = rawProfile.GetProfileResult(true);

            return profile;
        }

        public double MaterialToRemove
        {
            get { return Radius - Radius * .15; }
        }

        public Profile2D GetTrimmingProfile()
        {
            var lunghezzaPolygono = LunghezzaCentroLato + Radius;

            var altezzaPoly = Radius * 2;

            var centerPoint = SquareShapeHelper.GetCenterPoint(SquareShapeHelper.SquareShapeStartPoint.UpLeft,
                                                           CentroX - Radius, CentroY + Radius, lunghezzaPolygono, altezzaPoly);

            var profile = SquareShapeHelper.GetSquareProfile(centerPoint.X, centerPoint.Y, lunghezzaPolygono, altezzaPoly);

            profile.SetPlotStyle();


            return profile;
        }
    }
}