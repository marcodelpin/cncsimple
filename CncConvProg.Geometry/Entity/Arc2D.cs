using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.Entity
{
    [Serializable]
    public class Arc2D : IEntity2D
    {
        public Arc2D()
        {
            Start = new Point2D();
            End = new Point2D();
            Center = new Point2D();
        }
        public EnumPlotStyle PlotStyle { get; set; }

        public Point2D Start { get; set; }
        public Point2D End { get; set; }
        public Point2D Center { get; set; }
        public double Radius { get; set; }
        public bool ClockWise { get; set; }

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

        public Arc2D(Arc2D arc2D)
        {
            Start = new Point2D(arc2D.Start);
            End = new Point2D(arc2D.End);
            Center = new Point2D(arc2D.Center);
            Radius = arc2D.Radius;
            PlotStyle = arc2D.PlotStyle;
        }

        public void Multiply(Matrix3D matrix)
        {
            try
            {
                Start = GeometryHelper.MultiplyPoint(Start, matrix);

                Center = GeometryHelper.MultiplyPoint(Center, matrix);

                End = GeometryHelper.MultiplyPoint(End, matrix);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IEntity2D MultiplyMatrixCopy(Matrix3D matrix)
        {
            var arc = new Arc2D(this);
            arc.Multiply(matrix);
            return arc;
        }


        ///// <summary>
        ///// Inutile
        ///// </summary>
        ///// <param name="clockWise"></param>
        ///// <returns></returns>
        //public Point2D GetCircleCenter(bool clockWise)
        //{
        //    return CalculateTheCenter(Start, End, Radius, clockWise);
        //}
        ///// <summary>
        /////  inutile 
        ///// </summary>
        ///// <param name="pnt1"></param>
        ///// <param name="pnt2"></param>
        ///// <param name="radius"></param>
        ///// <param name="clockWise"></param>
        ///// <returns></returns>
        //private static Point2D CalculateTheCenter(Point2D pnt1, Point2D pnt2, double radius, bool clockWise)
        //{
        //    var x1 = pnt1.X;
        //    var x2 = pnt2.X;
        //    var y1 = pnt1.Y;
        //    var y2 = pnt2.Y;

        //    double center_a, center_b;
        //    //System.out.println( );
        //    //System.out.println( "EQUATION 1" );
        //    //System.out.println( "(a - " + x1 + ")^2 + (b - " + y1 + ")^2 = " + radius + "^2" );
        //    //System.out.println( );
        //    //System.out.println( "EQUATION 2" );
        //    //System.out.println( "(a - " + x2 + ")^2 + (b - " + y2 + ")^2 = " + radius + "^2" );
        //    //System.out.println( );
        //    //System.out.println( "******************************************************************" );
        //    //System.out.println( );
        //    double twoabx1 = -2 * x1;
        //    double twoaby1 = -2 * y1;
        //    double twoabx2 = -2 * x2;
        //    double twoaby2 = -2 * y2;
        //    double x1square = x1 * x1;
        //    double y1square = y1 * y1;
        //    double x2square = x2 * x2;
        //    double y2square = y2 * y2;
        //    double radiussquare = radius * radius;
        //    //System.out.println( "EXPANDING TWO EQUATION" );
        //    //System.out.println( "a^2 " + twoabx1 + "a + " + x1square + " + b^2 " + twoaby1 + "b + " + y1square + " = " + radiussquare );
        //    //System.out.println( );		
        //    //System.out.println( "a^2 " + twoabx2 + "a + " + x2square + " + b^2 " + twoaby2 + "b + " + y2square + " = " + radiussquare );
        //    //System.out.println( "--------------------------------------------------------------" );
        //    //System.out.println( );
        //    double x1_twoab_x2 = twoabx1 + (-1 * twoabx2);
        //    double y1_twoab_y2 = twoaby1 + (-1 * twoaby2);
        //    double x1_square_x2 = x1square + (-1 * x2square);
        //    double y1_square_y2 = y1square + (-1 * y2square);
        //    //System.out.println( "" + x1_twoab_x2 + "a +(" + x1_square_x2 + ") + " + y1_twoab_y2 + "b +(" + y1_square_y2 +") = 0" );
        //    //System.out.println( );
        //    //System.out.println( "--------------------------------------------------------------" );
        //    //System.out.println( );

        //    double a_square;
        //    double a;
        //    double constant;
        //    double b_square_coefficient = (y1_twoab_y2 * y1_twoab_y2);
        //    double temp;
        //    //double rounded_off_centera;
        //    if (y1_twoab_y2 < 0)
        //    {
        //        //System.out.println( "b = ( " + x1_twoab_x2 + "a " + ( x1_square_x2 + y1_square_y2 ) + " ) / " + ( -1 * y1_twoab_y2 ) );
        //        //System.out.println( );
        //        //System.out.println( "FORMING QUADRATIC EQUATION!!!!!!!" );
        //        a_square = (b_square_coefficient) + (x1_twoab_x2 * x1_twoab_x2);
        //        temp = (x1_square_x2 + y1_square_y2) - (y1 * -1 * y1_twoab_y2);
        //        a = (b_square_coefficient * twoabx1) + (2 * x1_twoab_x2 * temp);
        //        constant = (b_square_coefficient * x1square) - (b_square_coefficient * radiussquare) + (temp * temp);

        //        //	System.out.println( "" + a_square + "a^2 + (" + a + "a) + (" + constant + ") = 0" );
        //        //(b+sqrt(b^2 - 4ac))/2a
        //        center_a = ((-1 * a) + Math.Sqrt((a * a) - (4 * a_square * constant))) / (2 * a_square);
        //        center_b = ((x1_twoab_x2 * center_a) + (x1_square_x2 + y1_square_y2)) / (-1 * y1_twoab_y2);

        //        if (clockWise)
        //            return new Point2D(center_a, center_b);
        //        //System.out.println( );
        //        //System.out.println( );			
        //        //System.out.println( "Center( " + center_a + ", " + center_b + " )");
        //        //(b-sqrt(b^2 - 4ac))/2a
        //        center_a = ((-1 * a) - Math.Sqrt((a * a) - (4 * a_square * constant))) / (2 * a_square);
        //        center_b = ((x1_twoab_x2 * center_a) + (x1_square_x2 + y1_square_y2)) / (-1 * y1_twoab_y2);
        //        //	System.out.println( "Center( " + center_a + ", " + center_b + " )");

        //    }
        //    else
        //    {
        //        //System.out.println( "b = ( " + ( -1 * x1_twoab_x2 ) + "a +" + ( -1 * ( x1_square_x2 + y1_square_y2 ) ) + " ) / " + y1_twoab_y2 );
        //        //System.out.println( );
        //        //System.out.println( "FORMING QUADRATIC EQUATION!!!!!!!" );
        //        a_square = (b_square_coefficient) + (x1_twoab_x2 * x1_twoab_x2);
        //        temp = (-1 * (x1_square_x2 + y1_square_y2)) - (y1 * y1_twoab_y2);
        //        a = (b_square_coefficient * twoabx1) + (2 * -1 * x1_twoab_x2 * temp);
        //        constant = (b_square_coefficient * x1square) - (b_square_coefficient * radiussquare) + (temp * temp);

        //        //System.out.println( "" + a_square + "a^2 + (" + a + "a) + (" + constant + ") = 0" );
        //        //(b+sqrt(b^2 - 4ac))/2a
        //        center_a = ((-1 * a) + Math.Sqrt((a * a) - (4 * a_square * constant))) / (2 * a_square);
        //        center_b = ((-1 * (x1_square_x2 + y1_square_y2)) + (-1 * x1_twoab_x2 * center_a)) / (y1_twoab_y2);
        //        if (clockWise)
        //            return new Point2D(center_a, center_b);
        //        //System.out.println( );
        //        //System.out.println( );			
        //        //System.out.println( "Center( " + center_a + ", " + center_b + " )");
        //        //(b-sqrt(b^2 - 4ac))/2a
        //        center_a = ((-1 * a) - Math.Sqrt((a * a) - (4 * a_square * constant))) / (2 * a_square);
        //        center_b = ((-1 * (x1_square_x2 + y1_square_y2)) + (-1 * x1_twoab_x2 * center_a)) / (y1_twoab_y2);
        //        //System.out.println( "Center( " + center_a + ", " + center_b + " )");
        //    }
        //    //System.out.println( "******************************************************************" );
        //    //System.out.println( );
        //    return new Point2D(center_a, center_b);

        //}

        /*
         * private Point createCircle(Point p1, Point p2, double alpha) { 
        Coordinate centre = new Coordinate(); 

        double dx = (p2.getX() - p1.getX()); 
        double dy = (p2.getY() - p1.getY()); 
        double s2 = dx * dx + dy * dy; 

        double h = Math.sqrt(alpha * alpha / s2 - 0.25d); 

        centre.x = p1.getX() + dx / 2 + h * dy; 
        centre.y = p1.getY() + dy / 2 + h * (p1.getX() - p2.getX()); 

        return gf.createPoint(centre); 
    } 

         */

        //public static Point2D GetCircleCenter(Point2D p1, Point2D p2, double radius)
        //{
        //    var centre = new Point2D();

        //    var dx = (p2.X - p1.X);
        //    var dy = (p2.Y - p1.Y);
        //    var s2 = dx * dx + dy * dy;

        //    var h = Math.Sqrt(radius * radius / s2 - 0.25d);

        //    centre.X = p1.X + dx / 2 + h * dy;
        //    centre.Y = p1.Y + dy / 2 + h * (p1.X - p2.X);

        //    return centre;
        //}


        internal bool IsCircle()
        {
            var startAngle = Math.Atan2(Start.Y - Center.Y, Start.X - Center.X);

            var endAngle = Math.Atan2(End.Y - Center.Y, End.X - Center.X);

            //if (startAngle < 0)
            //{
            //    startAngle += Math.PI * 2;
            //    endAngle += Math.PI * 2;
            //}


            var deltaAngle = endAngle - startAngle;

            if (Math.Round(deltaAngle, 8) == 0)
            {
                /* è un cerchio completo*/
                return true;
            }

            return false;

        }
    }
}
