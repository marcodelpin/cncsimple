using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern
{
    [Serializable]
    public class PatternDrillingXy : IPatternDrilling
    {
        public void AddPnt(Point2D point2D)
        {
            PointList.Add(point2D);
        }

        public double ReferencePntX { get; set; }
        public double ReferencePntY { get; set; }

        public readonly List<Point2D> PointList = new List<Point2D>();

        public List<Point2D> GetPointList()
        {
            var p = new List<Point2D>();
            foreach (var point2D in PointList)
            {
                p.Add(new Point2D(point2D.X + ReferencePntX, point2D.Y + ReferencePntY));
            }
            return p;
        }
    }
}
