using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    [Serializable]
    public class CavaDrittaPattern : IMillingPattern
    {
        public double Radius { get; set; }

        public double CentroX { get; set; }

        public double CentroY { get; set; }

        public double LunghezzaCentri { get; set; }

        /*
         * Magari aggiungere inclinazione
         */

        public Profile2D GetClosedProfile()
        {

            var profile = Profile2D.CreateCavaLineare(Radius, LunghezzaCentri, new Point2D(CentroX, CentroY));

            return profile;
        }
    }
}