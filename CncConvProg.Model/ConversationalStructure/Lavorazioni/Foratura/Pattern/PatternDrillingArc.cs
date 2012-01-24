using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern
{
    [Serializable]
    public class PatternDrillingArc : IPatternDrilling
    {
        public double CircleCenterX { get; set; }

        public double CircleCenterY { get; set; }

        public double Radius { get; set; }

        public int DrillCount { get; set; }

        public double FirstAngle { get; set; }

        public double EndAngle { get; set; }

        public PatternDrillingArc()
        {
            DrillCount = 3;

            Radius = 30;

            EndAngle = 180;
        }
        public List<Point2D> GetPointList()
        {
            var rslt = new List<Point2D>();

            var ea = Geometry.GeometryHelper.DegreeToRadian(EndAngle);
            var firstAngle = Geometry.GeometryHelper.DegreeToRadian(FirstAngle);

            var endAngle = Geometry.GeometryHelper.GetPositiveAngle(ea);

            var startAngle = Geometry.GeometryHelper.GetPositiveAngle(firstAngle);

            if (DrillCount == 1) return rslt;

            var dc = DrillCount - 1;

            var angleStep = (endAngle - startAngle) / dc;

            for (var i = 0; i < DrillCount; i++)
            {
                var angleCurrent = (angleStep * i) + firstAngle;

                var point = new Point2D { X = Math.Cos(angleCurrent) * Radius, Y = Math.Sin(angleCurrent) * Radius };

                point.X += CircleCenterX;
                point.Y += CircleCenterY;

                rslt.Add(point);
            }
            return rslt;
        }
    }
}
