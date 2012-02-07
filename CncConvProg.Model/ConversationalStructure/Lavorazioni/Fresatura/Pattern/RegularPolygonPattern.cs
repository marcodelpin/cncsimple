using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    [Serializable]
    public class RegularPolygonPattern : IMillingPattern
    {
        public bool Circoscritto { get; set; }

        public double Radius { get; set; }

        public double CentroX { get; set; }

        public double CentroY { get; set; }

        public int SideNumber { get; set; }

        public double AngleInclination { get; set; }

        public double Chamfer { get; set; }

        public bool ChamferAbilited { get; set; }

        public Profile2D GetClosedProfile()
        {
            var raggio = Circoscritto ? Radius : GeometryHelper.GetRaggioCircoscritto(Radius, SideNumber);

            var pntList = GeometryHelper.CalculatePoygonInscribed(SideNumber, new Point2D(CentroX, CentroY), raggio);

            var p = new RawProfile(false);


            var iniPoint2D = new Point2D();

            for (int i = 0; i < pntList.Count; i++)
            {
                var point = pntList[i];

                // Initial point
                if (i == 0)
                {
                    iniPoint2D = point;

                    var ini = new RawInitPoint2D(p);
                    ini.X.SetValue(true, iniPoint2D.X + CentroX);
                    ini.Y.SetValue(true, iniPoint2D.Y + CentroY);
                    p.Add(ini);
                    continue;
                }

                var p1 = new RawLine2D(p);
                p1.X.SetValue(true, point.X + CentroX);
                p1.Y.SetValue(true, point.Y + CentroY);
                p.Add(p1);

                if (Chamfer > 0 && ChamferAbilited)
                {
                    p1.Chamfer.SetValue(true, Chamfer);
                }
                else if (Chamfer > 0 && !ChamferAbilited)
                {
                    p1.EndRadius.SetValue(true, Chamfer);
                }
            }

            var end = new RawLine2D(p);
            end.X.SetValue(true, iniPoint2D.X + CentroX);
            end.Y.SetValue(true, iniPoint2D.Y + CentroY);
            p.Add(end);

            if (Chamfer > 0 && ChamferAbilited)
            {
                end.Chamfer.SetValue(true, Chamfer);
            }
            else if (Chamfer > 0 && !ChamferAbilited)
            {
                end.EndRadius.SetValue(true, Chamfer);
            }

            //var profile2D = new Profile2D();

            //// devo mettere metodo , finalize o cose cosi nell'inserimento profilo
            //foreach (var point2D in pntList)
            //{
            //    if(point2D == pntList.FirstOrDefault())
            //    profile2D.AddPnt(point2D);
            //}

            //profile2D.AddPnt(pntList.FirstOrDefault());

            //profile2D.SetPlotStyle();

            var rslt = p.GetProfileResult(true);
            rslt.SetPlotStyle();

            return rslt;
        }
    }
}