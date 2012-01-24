using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    [Serializable]
    public class RettangoloPattern : IMillingPattern
    {
        public bool Circoscritto { get; set; }

        public double Radius { get; set; }

        public double CentroX { get; set; }

        public double CentroY { get; set; }

        public int SideNumber { get; set; }

        public double AngleInclination { get; set; }

        public double ChamferValue { get; set; }

        public bool Chamfer { get; set; }

        public Profile2D GetClosedProfile()
        {
            var raggio = Circoscritto ? Radius : GeometryHelper.GetRaggioCircoscritto(Radius, SideNumber);

            var pntList = GeometryHelper.CalculatePoygonInscribed(SideNumber, new Point2D(CentroX, CentroY), raggio);

            var profile2D = new Profile2D();


            // devo mettere metodo , finalize o cose cosi nell'inserimento profilo
            foreach (var point2D in pntList)
            {
                profile2D.AddPnt(point2D);
            }

            profile2D.AddPnt(pntList.FirstOrDefault());

            /*
             * todo : 
             * per calcolare anche raggi fare rawProfile e poi ottenere rawProfile..
             * 
             * magari in rawProfile fare metodi addPnt e addChamfer o cose cosi..
             */

            return profile2D;
        }
    }
}