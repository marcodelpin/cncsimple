using System;

namespace CncConvProg.Geometry.Entity
{
    [Serializable]
    public class Point2D : IEquatable<Point2D>
    {
        public EnumPlotStyle PlotStyle { get; set; }

        public Point2D(Point2D pnt)
        {
            X = pnt.X;
            Y = pnt.Y;

        }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point2D()
        {
            // TODO: Complete member initialization
        }
        public double X { get; set; }
        public double Y { get; set; }
        public bool Equals(Point2D other, int roundDecimal)
        {
            if (Math.Round(X, roundDecimal) == Math.Round(other.X, roundDecimal) && Math.Round(Y, roundDecimal) == Math.Round(other.Y, roundDecimal))
                return true;
            return false;

        }
        public bool Equals(Point2D other)
        {
            if (X == other.X && Y == other.Y)
                return true;
            return false;

        }
    }
}
