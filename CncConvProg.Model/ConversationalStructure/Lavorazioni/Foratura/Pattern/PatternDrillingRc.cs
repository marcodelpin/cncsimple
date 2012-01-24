using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern
{
    [Serializable]
    public class PatternDrillingRc : IPatternDrilling
    {
        public PatternDrillingRc()
        {
            PointRcList = new List<PointRc>();
        }

        public List<PointRc> PointRcList { get; set; }

        public double CenterX { get; set; }
        public double CenterY { get; set; }


        public List<Point2D> GetPointList()
        {
            var rslt = new List<Point2D>();

            foreach (var pointRc in PointRcList)
            {
                var p = pointRc.GetXyCoordinate();
                p.X += CenterX;
                p.Y += CenterY;

                rslt.Add(p);
            }

            return rslt;
        }
    }

    /// <summary>
    /// Punto con coordinate in formato raggio . Angolo
    /// </summary>
    [Serializable]
    public class PointRc
    {
        public double Radius { get; set; }
        public double Angle { get; set; }

        public Point2D GetXyCoordinate()
        {
            return GeometryHelper.GetCoordinate(GeometryHelper.DegreeToRadian(Angle), Radius);

        }
    }
}
