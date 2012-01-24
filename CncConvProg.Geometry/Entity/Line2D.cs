using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CncConvProg.Geometry.Entity
{
    [Serializable]
    public class Line2D : IEntity2D
    {
        public Line2D()
        {
            Start = new Point2D();

            End = new Point2D();
        }

        public Line2D(Line2D line2D)
        {
            Start = new Point2D(line2D.Start);
            End = new Point2D(line2D.End);
        }

        public IEntity2D MultiplyMatrixCopy(Matrix3D matrix)
        {
            var line2D = new Line2D(this);
            line2D.Multiply(matrix);
            return line2D;
        }
        public EnumPlotStyle PlotStyle { get; set; }

        public Point2D Start { get; set; }
        public Point2D End { get; set; }

        public bool IsSelected { get; set; }

        public IEnumerable<Point2D> GetBoundingSquare()
        {
            var pointList = new List<Point2D> { Start, End };

            return pointList;
        }

        public Point2D GetFirstPnt()
        {
            return Start;
        }

        public Point2D GetLastPnt()
        {
            return End;
        }

        public void Multiply(Matrix3D matrix)
        {
            try
            {
                Start = GeometryHelper.MultiplyPoint(Start, matrix);

                End = GeometryHelper.MultiplyPoint(End, matrix);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public double GetLenght()
        {
            var rslt = GeometryHelper.Distance(Start, End);

            return rslt.HasValue ? rslt.Value : 0;
        }

        public double GetAngleLine()
        {
            return GeometryHelper.GetAngle(Start, End);
        }

        public Point2D GetMidPoint()
        {
            return GeometryHelper.GetMidPoint(Start, End);
        }


    }
}
