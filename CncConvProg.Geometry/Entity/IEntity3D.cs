using System.Collections.Generic;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.Entity
{
    public static class Entity3DHelper
    {
        public static IEnumerable<IEntity3D> Get3DProfile(IEnumerable<IEntity2D> source)
        {
            var rslt = new List<IEntity3D>();

            foreach (var entity2D in source)
            {
                if (entity2D is Line2D)
                {
                    var line2 = entity2D as Line2D;

                    var line = new Line3D
                    {
                        Start =
                        {
                            X = line2.Start.X,
                            Y = line2.Start.Y,
                            Z = 0
                        },
                        End =
                        {
                            X = line2.End.X,
                            Y = line2.End.Y,
                            Z = 0
                        }
                    };

                    line.PlotStyle = line2.PlotStyle;

                    line.IsSelected = line2.IsSelected;

                    rslt.Add(line);

                }
                else if (entity2D is Arc2D)
                {
                    var arc2D = entity2D as Arc2D;


                    var arc3D = new Arc3D()
                    {
                        Start =
                        {
                            X = arc2D.Start.X,
                            Y = arc2D.Start.Y,
                            Z = 0
                        },
                        End =
                        {
                            X = arc2D.End.X,
                            Y = arc2D.End.Y,
                            Z = 0
                        },

                        Center =
                        {
                            X = arc2D.Center.X,
                            Y = arc2D.Center.Y,
                            Z = 0,
                        },

                        Radius = arc2D.Radius,
                        ClockWise = arc2D.ClockWise,
                    };


                    arc3D.PlotStyle = arc2D.PlotStyle;

                    arc3D.IsSelected = arc2D.IsSelected;

                    rslt.Add(arc3D);

                }
            }

            return rslt;
        }
        public static void SetPlotStyle(IEnumerable<IEntity3D> l, EnumPlotStyle enumPlotStyle = EnumPlotStyle.Element)
        {
            foreach (var entity2D in l)
            {
                entity2D.PlotStyle = enumPlotStyle;
            }
        }
    }
    public interface IEntity3D
    {
        EnumPlotStyle PlotStyle { get; set; }

        //bool IsSelected { get; set; } 

        Rect3D GetBoundary();
        /// <summary>
        ///  magari inserire start e end in classe base.
        /// </summary>
        /// <returns></returns>
        Point2D GetFirstPnt();

        Point2D GetLastPnt();

        IEntity3D MultiplyMatrix(Matrix3D rotationMatrix);
    }
}
