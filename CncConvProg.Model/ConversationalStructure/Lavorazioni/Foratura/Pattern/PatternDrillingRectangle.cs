using System;
using System.Collections.Generic;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern
{
    [Serializable]
    public class PatternDrillingRectangle : IPatternDrilling
    {
        public SquareShapeHelper.SquareShapeStartPoint StartPoint { get; set; }

        public double RefPointX { get; set; }

        public double RefPointY { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public int DrillCountX { get; set; }

        public int DrillCountY { get; set; }

        public PatternDrillingRectangle()
        {
        }



        public List<Point2D> GetPointList()
        {
            var rslt = new List<Point2D>();

            var center = SquareShapeHelper.GetCenterPoint(StartPoint, RefPointX, RefPointY, Width, Height);

            var upRight = new Point2D(center.X - Width / 2, center.Y + Height / 2);

            if (DrillCountX > 0 && DrillCountY > 0)
            {
                var stepCountX = (DrillCountX - 1);
                var stepCountY = (DrillCountY - 1);

                if (stepCountX == 0)
                    stepCountX = 1;

                if (stepCountY == 0)
                    stepCountY = 1;

                var stepX = Width / stepCountX;

                var stepY = Height / stepCountY;

                for (int i = 0; i < DrillCountX; i++)
                {
                    for (int j = 0; j < DrillCountY; j++)
                    {
                        var p = new Point2D(upRight);

                        p.X += i * stepX;
                        p.Y += -(j * stepY);

                        rslt.Add(p);
                    }
                }
            }

            return rslt;
        }
    }
}