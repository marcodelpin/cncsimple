using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.Entity
{
    [Serializable]
    public class Line3D : IEntity3D
    {
        public Line3D()
        {
            Start = new Point3D();

            End = new Point3D();
        }
        public Point3D Start;
        public Point3D End;

        public EnumPlotStyle PlotStyle { get; set; }

        public double GetLength()
        {
            return GeometryHelper.Distance(Start, End);
        }
        /// <summary>
        /// obs
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Obs
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Point2D> GetBoundingSquare()
        {
            return null;
        }

        public Rect3D GetBoundary()
        {
            var rect3D = new Rect3D(Start.X, Start.Y, Start.Z, Math.Abs(End.X - Start.X), Math.Abs(End.Y - Start.Y), Math.Abs(End.Z - Start.Z));

            return rect3D;
        }

        public Point2D GetFirstPnt()
        {
            throw new NotImplementedException();
        }

        public Point2D GetLastPnt()
        {
            throw new NotImplementedException();
        }

        public IEntity3D MultiplyMatrix(Matrix3D rotationMatrix)
        {
            var line = new Line3D()
            {
                PlotStyle = PlotStyle,
                Start =
                    GeometryHelper.MultiplyPoint(new Point3D(Start.X, Start.Y, Start.Z), rotationMatrix),
                End = GeometryHelper.MultiplyPoint(new Point3D(End.X, End.Y, End.Z), rotationMatrix),
                IsSelected = IsSelected,
            };

            return line;
        }
    }
}
