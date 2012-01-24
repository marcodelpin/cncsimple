using System.Collections.Generic;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.Entity

{
    public interface IEntity2D
    {
        EnumPlotStyle PlotStyle { get; set; }

        bool IsSelected { get; set; } 
        IEnumerable<Point2D> GetBoundingSquare();

        /// <summary>
        ///  magari inserire start e end in classe base.
        /// </summary>
        /// <returns></returns>
        Point2D GetFirstPnt();

        Point2D GetLastPnt();

        //IEntity2D MultiplyMatrix(Matrix3D rotationMatrix);

        void Multiply(Matrix3D matrix);
        IEntity2D MultiplyMatrixCopy(Matrix3D matrix);

    }
}
