using System;
using System.Collections.Generic;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern
{
    [Serializable]
    public class PatternDrillingCircle : IPatternDrilling
    {
        public double CircleCenterX { get; set; }
        public double CircleCenterY { get; set; }

        public double Radius { get; set; }

        public int DrillCount { get; set; }

        public double FirstAngle { get; set; }

        public PatternDrillingCircle()
        {
            CircleCenterX = 0;
            CircleCenterY = 0;

            DrillCount = 3;

            Radius = 30;
        }
        public List<Point2D> GetPointList()
        {
            var rslt = new List<Point2D>();

            var angleStep = (Math.PI * 2) / DrillCount;
            var rad = Geometry.GeometryHelper.DegreeToRadian(FirstAngle);
            for (var i = 1; i <= DrillCount; i++)
            {
                var angleCurrent = (angleStep * i) + rad;

                var point = new Point2D { X = Math.Cos(angleCurrent) * Radius, Y = Math.Sin(angleCurrent) * Radius };

                point.X += CircleCenterX;
                point.Y += CircleCenterY;

                rslt.Add(point);
            }
            return rslt;
        }
    }
}