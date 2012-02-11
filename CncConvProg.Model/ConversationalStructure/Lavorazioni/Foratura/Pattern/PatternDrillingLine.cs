using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern
{
    [Serializable]
    public class PatternDrillingLine : IPatternDrilling
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }

        public double FirstLenght { get; set; }

        public double Angle { get; set; }

        public int NumeroFori { get; set; }

        public double Passo { get; set; }

        public List<Point2D> GetPointList()
        {
            var rslt = new List<Point2D>();

            for (int i = 0; i < NumeroFori; i++)
            {
                var distance = (i * Passo) + FirstLenght;

                var p = Geometry.GeometryHelper.GetCoordinate(Geometry.GeometryHelper.DegreeToRadian(Angle), distance);

                p.X += CenterX;
                p.Y += CenterY;

                rslt.Add(p);
            }

            return rslt;
        }
    }
}
