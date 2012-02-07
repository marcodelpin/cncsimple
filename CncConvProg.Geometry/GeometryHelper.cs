using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry.Entity;
using MatrixLibrary;
using OffsetPath;

namespace CncConvProg.Geometry
{
    public static class GeometryHelper
    {
        public static bool Entity2DIntersection(IEntity2D entity, Line2D line2D, out Point2D intersectionPoint)
        {
            var l = entity as Line2D;
            var a = entity as Arc2D;

            if (l != null)
            {
                return SegmentIntersection(l, line2D, out intersectionPoint);
            }

            if (a != null)
            {
                return ArcSegmentIntersection(a, line2D, out intersectionPoint);
            }

            intersectionPoint = null;
            return false;
        }

        private static bool ArcSegmentIntersection(Arc2D a, Line2D line2D, out Point2D intersectionPoint)
        {
            Point2D firstC;
            Point2D secondC;

            var rslt = FindLineCircleIntersections(a.Center.X, a.Center.Y, a.Radius, line2D.Start,
                                                   line2D.End, out firstC, out secondC);

            if (rslt == 0)
            {
                intersectionPoint = null;
                return false;
            }

            /*
             * devo controllare se i punto trovati stanno dentro angolo del'arco
             */
            var angleStart = GetPositiveAngle(a.Start.X - a.Center.X, a.Start.Y - a.Center.Y);
            var angleEnd = GetPositiveAngle(a.End.X - a.Center.X, a.End.Y - a.Center.Y);

            if (rslt == 1)
            {
                if (firstC != null)
                {
                    var angolo = GetPositiveAngle(firstC.X - a.Center.X, firstC.Y - a.Center.Y);
                    if (AngoloRisiedeDentroRange(angolo, angleStart, angleEnd))
                    {
                        intersectionPoint = firstC;
                        return true;
                    }
                }
            }


            if (rslt == 2)
            {
                if (firstC != null)
                {
                    var angolo = GetPositiveAngle(firstC.X - a.Center.X, firstC.Y - a.Center.Y);
                    if (AngoloRisiedeDentroRange(angolo, angleStart, angleEnd))
                    {
                        intersectionPoint = firstC;
                        return true;
                    }
                }
                if (secondC != null)
                {
                    var angolo = GetPositiveAngle(secondC.X - a.Center.X, secondC.Y - a.Center.Y);
                    if (AngoloRisiedeDentroRange(angolo, angleStart, angleEnd))
                    {
                        intersectionPoint = secondC;
                        return true;
                    }
                }
            }

            intersectionPoint = null;
            return false;
        }

        private static bool AngoloRisiedeDentroRange(double angle, double rangeMin, double rangeMax)
        {
            if (rangeMin > rangeMax)
            {
                return angle <= rangeMin && angle >= rangeMax;

            }
            return angle >= rangeMin && angle <= rangeMax;
        }

        // altro metodo per intersezione segmenti.
        public static bool SegmentIntersection(Line2D line1, Line2D line2, out Point2D intersectionPoint)
        {
            intersectionPoint = null;
            Point2D start1 = line1.Start;
            Point2D end1 = line1.End;
            Point2D start2 = line2.Start;
            Point2D end2 = line2.End;

            var denom = ((end1.X - start1.X) * (end2.Y - start2.Y)) - ((end1.Y - start1.Y) * (end2.X - start2.X));

            //  AB & CD are parallel 
            if (denom == 0)
                return false;

            var numer = ((start1.Y - start2.Y) * (end2.X - start2.X)) - ((start1.X - start2.X) * (end2.Y - start2.Y));

            var r = numer / denom;

            var numer2 = ((start1.Y - start2.Y) * (end1.X - start1.X)) - ((start1.X - start2.X) * (end1.Y - start1.Y));

            var s = numer2 / denom;

            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return false;

            // Find intersection point
            var result = new Point2D
            {
                X = start1.X + (r * (end1.X - start1.X)),
                Y = start1.Y + (r * (end1.Y - start1.Y))
            };
            intersectionPoint = result;
            return true;
        }

        //http://blog.csharphelper.com/2010/01/04/reverse-a-polygons-orientation-in-c.aspx
        public static class PolygonHelper
        {
            // Find the polygon's centroid.

            #region "Orientation Routines"
            // Return True if the polygon is oriented clockwise.
            public static bool PolygonIsOrientedClockwise(List<PuntoDueD> pnts)
            {
                return (SignedPolygonArea(pnts) < 0);
            }

            // If the polygon is oriented counterclockwise,
            // reverse the order of its points.
            public static List<PuntoDueD> OrientPolygonClockwise(List<PuntoDueD> pnts)
            {

                if (!PolygonIsOrientedClockwise(pnts))
                {

                    var l = pnts.ToArray();
                    Array.Reverse(l);
                    return l.ToList();
                }

                return pnts;
            }
            #endregion // Orientation Routines

            #region "Area Routines"

            private static double SignedPolygonArea(List<PuntoDueD> Points)
            {
                // Add the first point to the end.
                int nuPoints = Points.Count;
                var pts = new PuntoDueD[nuPoints + 1];
                Points.CopyTo(pts, 0);
                pts[nuPoints] = Points[0];

                // Get the areas.
                double area = 0;
                for (int i = 0; i < nuPoints; i++)
                {
                    area +=
                        (pts[i + 1].X - pts[i].X) *
                        (pts[i + 1].Y + pts[i].Y) / 2;
                }

                // Return the result.
                return area;
            }
            #endregion // Area Routines
        }



        public static double Pitagora(double l1, double l2)
        {
            return Math.Sqrt(Math.Pow(l1, 2) + Math.Pow(l2, 2));
        }

        public static double PitagoraIpoNota(double ipo, double l1)
        {
            return Math.Sqrt(Math.Pow(ipo, 2) - Math.Pow(l1, 2));
        }
   

        /// <summary>
        /// Calcola punto finale su arco .
        /// + punto iniziale incrementato di un valore. increment
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="arcCenter"></param>
        /// <param name="arcRadius"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static Point2D GetArcCoordinate(double startAngle, Point2D arcCenter, double arcRadius, double increment, bool clockWise)
        {
            /*
             * + Si crea triangolo iso formato da lati uguali == raggio e altro lato == increment
             * + Divido angolo in 2 triangoli rett per risolvere angoli 
             * + mi serve il doppio di beta 
             * 
             * b = meta lato diverso
             * a = ipo -- raggio 
             * il sinBeta = b/a
             */

            var b = increment / 2;
            var a = arcRadius;

            var sinBeta = b / a;

            var halfCenterAngle = Math.Asin(sinBeta);

            var centerAngle = halfCenterAngle * 2;

            Point2D cooPoint;
            if (clockWise)
                cooPoint = GeometryHelper.GetCoordinate(startAngle - centerAngle, arcRadius);
            else
            {
                cooPoint = GeometryHelper.GetCoordinate(startAngle + centerAngle, arcRadius);
            }

            cooPoint.X += arcCenter.X;
            cooPoint.Y += arcCenter.Y;

            return cooPoint;
        }

        /// <summary>
        /// Ritorna angolo equivalente corrispondente a un'incremento sul arco.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static double GetAngleCorrispondence(double radius, double increment)
        {
            var b = increment / 2;
            var a = radius;

            var sinBeta = b / a;

            var halfCenterAngle = Math.Asin(sinBeta);

            var centerAngle = halfCenterAngle * 2;

            return centerAngle;
        }
        //public static bool SegmentIntersection(
        //Line2D line1,
        //Line2D line2,
        //out Point2D interesctionPnt)
        //{
        //    double Ax = line1.Start.X;
        //    double Ay = line1.Start.Y;
        //    double Bx = line1.End.X;
        //    double By = line1.End.Y;

        //    double Cx = line2.Start.X;
        //    double Cy = line2.Start.Y;
        //    double Dx = line2.End.X;
        //    double Dy = line2.End.Y;

        //    interesctionPnt = null;

        //    double distAB, theCos, theSin, newX, ABpos;

        //    //  Fail if either line is undefined.
        //    if (Ax == Bx && Ay == By || Cx == Dx && Cy == Dy)
        //        return false;

        //    //  (1) Translate the system so that point A is on the origin.
        //    Bx -= Ax; By -= Ay;
        //    Cx -= Ax; Cy -= Ay;
        //    Dx -= Ax; Dy -= Ay;

        //    //  Discover the length of segment A-B.
        //    distAB = Math.Sqrt(Bx * Bx + By * By);

        //    //  (2) Rotate the system so that point B is on the positive X axis.
        //    theCos = Bx / distAB;
        //    theSin = By / distAB;
        //    newX = Cx * theCos + Cy * theSin;
        //    Cy = Cy * theCos - Cx * theSin; Cx = newX;
        //    newX = Dx * theCos + Dy * theSin;
        //    Dy = Dy * theCos - Dx * theSin; Dx = newX;

        //    //  Fail if the lines are parallel.
        //    if (Cy == Dy) return false;

        //    //  (3) Discover the position of the intersection point along line A-B.
        //    ABpos = Dx + (Cx - Dx) * Dy / (Dy - Cy);

        //    //  (4) Apply the discovered position to line A-B in the original coordinate system.
        //    interesctionPnt = new Point2D();
        //    interesctionPnt.X = Ax + ABpos * theCos;
        //    interesctionPnt.Y = Ay + ABpos * theSin;

        //    //  Success.
        //    return true;
        //}

        // ---------- cp

        public struct Line
        {
            public static Line Empty;

            private Point2D p1;
            private Point2D p2;

            public Line(Point2D p1, Point2D p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }

            public Point2D P1
            {
                get { return p1; }
                set { p1 = value; }
            }

            public Point2D P2
            {
                get { return p2; }
                set { p2 = value; }
            }

            public double X1
            {
                get { return p1.X; }
                set { p1.X = value; }
            }

            public double X2
            {
                get { return p2.X; }
                set { p2.X = value; }
            }

            public double Y1
            {
                get { return p1.Y; }
                set { p1.Y = value; }
            }

            public double Y2
            {
                get { return p2.Y; }
                set { p2.Y = value; }
            }
        }

        public struct Polygon : IEnumerable<Point2D>
        {
            private Point2D[] points;

            public Polygon(Point2D[] points)
            {
                this.points = points;
            }

            public Point2D[] Points
            {
                get { return points; }
                set { points = value; }
            }

            public int Length
            {
                get { return points.Length; }
            }

            public Point2D this[int index]
            {
                get { return points[index]; }
                set { points[index] = value; }
            }

            public static implicit operator Point2D[](Polygon polygon)
            {
                return polygon.points;
            }

            public static implicit operator Polygon(Point2D[] points)
            {
                return new Polygon(points);
            }

            IEnumerator<Point2D> IEnumerable<Point2D>.GetEnumerator()
            {
                return (IEnumerator<Point2D>)points.GetEnumerator();
            }

            public IEnumerator GetEnumerator()
            {
                return points.GetEnumerator();
            }
        }

        public enum Intersection
        {
            None,
            Tangent,
            Intersection,
            Containment
        }

        public static Point2D GetMidPoint(Point2D start, Point2D end)
        {
            var x = (start.X + end.X) / 2;
            var y = (start.Y + end.Y) / 2;

            return new Point2D(x, y);
        }

        public static class Geometry
        {

            // non funziona con quadrati
            //public static bool PointInPolygon(Point2D p, Point2D[] poly)
            // {

            //     Point2D p1, p2;



            //     bool inside = false;



            //     if (poly.Length < 3)
            //     {

            //         return inside;

            //     }



            //     var oldPoint = new Point2D(

            //     poly[poly.Length - 1].X, poly[poly.Length - 1].Y);



            //     for (int i = 0; i < poly.Length; i++)
            //     {

            //         var newPoint = new Point2D(poly[i].X, poly[i].Y);



            //         if (newPoint.X > oldPoint.X)
            //         {

            //             p1 = oldPoint;

            //             p2 = newPoint;

            //         }

            //         else
            //         {

            //             p1 = newPoint;

            //             p2 = oldPoint;

            //         }



            //         if ((newPoint.X < p.X) == (p.X <= oldPoint.X)

            //         && ((long)p.Y - (long)p1.Y) * (long)(p2.X - p1.X)

            //          < ((long)p2.Y - (long)p1.Y) * (long)(p.X - p1.X))
            //         {

            //             inside = !inside;

            //         }



            //         oldPoint = newPoint;

            //     }



            //     return inside;

            // }

            public static Intersection IntersectionOf(Line line, Polygon polygon)
            {
                if (polygon.Length == 0)
                {
                    return Intersection.None;
                }
                if (polygon.Length == 1)
                {
                    return IntersectionOf(polygon[0], line);
                }
                bool tangent = false;
                for (int index = 0; index < polygon.Length; index++)
                {
                    int index2 = (index + 1) % polygon.Length;
                    Intersection intersection = IntersectionOf(line, new Line(polygon[index], polygon[index2]));
                    if (intersection == Intersection.Intersection)
                    {
                        return intersection;
                    }
                    if (intersection == Intersection.Tangent)
                    {
                        tangent = true;
                    }
                }
                return tangent ? Intersection.Tangent : IntersectionOf(line.P1, polygon);
            }

            public static Intersection IntersectionOf(Line line1, Line line2)
            {
                //  Fail if either line segment is zero-length.
                if (line1.X1 == line1.X2 && line1.Y1 == line1.Y2 || line2.X1 == line2.X2 && line2.Y1 == line2.Y2)
                    return Intersection.None;

                if (line1.X1 == line2.X1 && line1.Y1 == line2.Y1 || line1.X2 == line2.X1 && line1.Y2 == line2.Y1)
                    return Intersection.Intersection;
                if (line1.X1 == line2.X2 && line1.Y1 == line2.Y2 || line1.X2 == line2.X2 && line1.Y2 == line2.Y2)
                    return Intersection.Intersection;

                //  (1) Translate the system so that point A is on the origin.
                line1.X2 -= line1.X1; line1.Y2 -= line1.Y1;
                line2.X1 -= line1.X1; line2.Y1 -= line1.Y1;
                line2.X2 -= line1.X1; line2.Y2 -= line1.Y1;

                //  Discover the length of segment A-B.
                double distAB = Math.Sqrt(line1.X2 * line1.X2 + line1.Y2 * line1.Y2);

                //  (2) Rotate the system so that point B is on the positive X axis.
                double theCos = line1.X2 / distAB;
                double theSin = line1.Y2 / distAB;
                double newX = line2.X1 * theCos + line2.Y1 * theSin;
                line2.Y1 = line2.Y1 * theCos - line2.X1 * theSin; line2.X1 = newX;
                newX = line2.X2 * theCos + line2.Y2 * theSin;
                line2.Y2 = line2.Y2 * theCos - line2.X2 * theSin; line2.X2 = newX;

                //  Fail if segment C-D doesn't cross line A-B.
                if (line2.Y1 < 0 && line2.Y2 < 0 || line2.Y1 >= 0 && line2.Y2 >= 0)
                    return Intersection.None;

                //  (3) Discover the position of the intersection point along line A-B.
                double posAB = line2.X2 + (line2.X1 - line2.X2) * line2.Y2 / (line2.Y2 - line2.Y1);

                //  Fail if segment C-D crosses line A-B outside of segment A-B.
                if (posAB < 0 || posAB > distAB)
                    return Intersection.None;

                //  (4) Apply the discovered position to line A-B in the original coordinate system.
                return Intersection.Intersection;
            }

            public static Intersection IntersectionOf(Point2D point, Polygon polygon)
            {
                switch (polygon.Length)
                {
                    case 0:
                        return Intersection.None;
                    case 1:
                        if (polygon[0].X == point.X && polygon[0].Y == point.Y)
                        {
                            return Intersection.Tangent;
                        }
                        else
                        {
                            return Intersection.None;
                        }
                    case 2:
                        return IntersectionOf(point, new Line(polygon[0], polygon[1]));
                }

                int counter = 0;
                int i;
                Point2D p1;
                int n = polygon.Length;
                p1 = polygon[0];
                if (point == p1)
                {
                    return Intersection.Tangent;
                }

                for (i = 1; i <= n; i++)
                {
                    Point2D p2 = polygon[i % n];
                    if (point == p2)
                    {
                        return Intersection.Tangent;
                    }
                    if (point.Y > Math.Min(p1.Y, p2.Y))
                    {
                        if (point.Y <= Math.Max(p1.Y, p2.Y))
                        {
                            if (point.X <= Math.Max(p1.X, p2.X))
                            {
                                if (p1.Y != p2.Y)
                                {
                                    double xinters = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                                    if (p1.X == p2.X || point.X <= xinters)
                                        counter++;
                                }
                            }
                        }
                    }
                    p1 = p2;
                }

                return (counter % 2 == 1) ? Intersection.Containment : Intersection.None;
            }

            public static Intersection IntersectionOf(Point2D point, Line line)
            {
                var bottomY = Math.Min(line.Y1, line.Y2);
                var topY = Math.Max(line.Y1, line.Y2);
                bool heightIsRight = point.Y >= bottomY &&
                                     point.Y <= topY;
                //Vertical line, slope is divideByZero error!
                if (line.X1 == line.X2)
                {
                    if (point.X == line.X1 && heightIsRight)
                    {
                        return Intersection.Tangent;
                    }
                    else
                    {
                        return Intersection.None;
                    }
                }
                var slope = (line.X2 - line.X1) / (line.Y2 - line.Y1);
                bool onLine = (line.Y1 - point.Y) == (slope * (line.X1 - point.X));
                if (onLine && heightIsRight)
                {
                    return Intersection.Tangent;
                }
                else
                {
                    return Intersection.None;
                }
            }

        }

        // ------------ cp

        public static Line2D GetIntersection(Profile2D stockPoly, Line2D line, double extensionValue, out bool interHappened)
        {
            /*
             *todo :  controllare comportamento linee collineari
             */
            /*
             * devo restituire la linea risultante. la linea "clipped"
             * Assumo :
             * - Profilo sia costituito di tutte linee
             * - Lo Stock Polygon sia semplice ( convesso , cerchio o quadrato , cmq non concavo )
             */

            /*
             * possono esserci 3 casi
             * 
             * linea dentro 
             * linea fuori
             * intersezione
             */

            var lines = stockPoly.Source.Cast<Line2D>();
            var polyPoints = stockPoly.GetPointListP2();

            var intersectionPoints = new List<Point2D>();

            foreach (var line2D in lines)
            {
                if (line.End.X == 0 && line.End.Y == 0 && line2D.Start.X == -10 && line2D.Start.Y == -5)
                {
                    
                }

                if(IsLinesCollinear(line2D,line))
                {
                    interHappened = false;
                    return null;
                }

                Point2D intersectPoint = null;
                var intersectionSuccess = SegmentIntersection(line2D, line, out intersectPoint);



                if (intersectionSuccess)
                {
                    intersectionPoints.Add(intersectPoint);
                }
                else
                {
                }
            }

            /*
             * ora se nella lista intersezioni posso avere :
             * Caso 1) 0 punti ( linea tutta fuori o tutta dentro , controllare se i punti si trovano dentro o fuori profilo )
             * Caso 2) 1 punto ( linea entra o esce dal profilo ) 
             * Caso 3) 2 punti (la linea taglia , creero linea "clipped" , tramite distanza euclidea decidero l'inizio e la fine della linea..
             */

            if (intersectionPoints.Count == 0)
            {
                // Caso 1) 0 punti ( linea tutta fuori o tutta dentro , controllare se i punti si trovano dentro o fuori profilo )

                if (PointInPolygon(polyPoints, line.Start) && PointInPolygon(polyPoints, line.End))
                {
                    interHappened = false;

                    // se le estremita della linea si trovano all'interno restituisco tutta la linea 
                    return line;
                }
                else
                {
                    interHappened = true;

                    // restituisco null, ovvero la linea è fuori
                    return null;
                }
            }
            /*
             * problema quando punto finale cade su linea.
             */
            else if (intersectionPoints.Count == 1)
            {
                interHappened = true;

                var intPnt = intersectionPoints[0]; // unico punto intersezione
                /*
                 * Caso 2. la linea interseca il profilo
                 * devo capire se lo fa da ext >> int o int >> ext 
                 * 
                 * Il punto che si trova all'interno del profilo rimarra uguale , il punto che si trova fuori dal polygono
                 * diventera uguale al punto d'intersezione ( e sara esteso della lughezza della fresa + extracorsa )
                 */

                /*
                 * caso speciale..
                 * Nel caso che un segmento cominci proprio sul profilo del poligono di troncatura, come punto stabile devo prendere altro.
                 */

                if (PointInPolygon(polyPoints, line.Start) && !intPnt.Equals(line.Start))
                {
                    /*
                     * il punto finale è dentro al polygono, quindi il punto finale sara == al punto intersezione
                     */

                    var rsltLine = new Line2D { Start = new Point2D(line.Start), End = new Point2D(intPnt) };
                    var extension4 = GetPointAtDistance(line.Start, intPnt, -extensionValue, false, false);
                    rsltLine.End = extension4;

                    return rsltLine;
                }
                if (PointInPolygon(polyPoints, line.End) && !intPnt.Equals(line.End))
                {
                    /*
                     * il punto finale è dentro al polygono, quindi il punto finale sara == al punto intersezione
                     */

                    var rsltLine = new Line2D { Start = new Point2D(intPnt), End = new Point2D(line.End) };

                    var extension2 = GetPointAtDistance(intPnt, line.End, -extensionValue, true, false);

                    rsltLine.Start = extension2;

                    return rsltLine;
                }
            }

            else if (intersectionPoints.Count == 2)
            {
                interHappened = true;
                // caso 3 ( clipped line )
                var intPnt1 = intersectionPoints[0];
                var intPnt2 = intersectionPoints[1];

                /*
                 devo capire quale è lo start e quale è lo End
                 * . il punto più vicino a start sara start e punto più vicino end sara end
                 */
                var rsltLine = new Line2D();

                var ds1 = Distance(line.Start, intPnt1);
                var ds2 = Distance(line.Start, intPnt2);

                if (ds1 < ds2)
                {
                    rsltLine.Start = intPnt1;
                    rsltLine.End = intPnt2;

                    var extensionStart = GetPointAtDistance(intPnt2, intPnt1, -extensionValue, false, false);

                    rsltLine.Start = extensionStart;

                    var extensionEnd = GetPointAtDistance(intPnt2, intPnt1, -extensionValue, true, false);

                    rsltLine.End = extensionEnd;

                }
                else
                {
                    rsltLine.Start = intPnt2;
                    rsltLine.End = intPnt1;


                    var extensionStart = GetPointAtDistance(intPnt1, intPnt2, -extensionValue, false, false);

                    rsltLine.Start = extensionStart;

                    var extensionEnd = GetPointAtDistance(intPnt1, intPnt2, -extensionValue, true, false);

                    rsltLine.End = extensionEnd;
                }

                return rsltLine;

            }
            // mi sa che se arriva qui è colpa del mt
            throw new Exception("GeometryHelper.GetIntersection");
        }

        public static bool IsLinesCollinear(Line2D line1, Line2D line2)
        {
            var A = line1.Start;
            var B = line1.End;

            var C = line2.Start;
            var D = line2.End;

            //   Vector2 P = B - A;
            //   Vector2 Q = D - C;
            var AreaABC = C.X * (A.Y - B.Y) + A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y);
            var AreaABD = D.X * (A.Y - B.Y) + A.X * (B.Y - D.Y) + B.X * (D.Y - A.Y);

            if (AreaABC == 0 && AreaABD == 0)
            {
                return true;
            }
            return false;
        }
        public static List<Profile2D> TrimProfileByStockPolygon(Profile2D pathProfile, Profile2D stockPolygon, double extensionValue)
        {
            // assumo che i due profili siano formati solamente da linee..
            try
            {

                var lineList = new List<Line2D>();

                var intersectionHappened = false;
                foreach (var pathElement in pathProfile.Source)
                {
                    bool lineInters;
                    var trimResult = GetIntersection(stockPolygon, (Line2D)pathElement, extensionValue, out lineInters);

                    if (lineInters)
                        intersectionHappened = true;

                    if (trimResult != null)
                        lineList.Add(trimResult);
                }

                if (lineList.Count == 0) // se è stato tagliato via tutto..
                    return null;

                var rslt = MergeLines(lineList);

                /*
                 * Se non ci sono stati tagli , il punto iniziale non varia..
                 * Non ci sono stati tagli quando il numero di linee del rslt == numeroLinee del pathProfile
                 */
                if (rslt.Count > 0)
                {
                    if (!intersectionHappened) // se non ci sono stati tagli ..
                    {
                        rslt[0].ToolPathStartPoint = pathProfile.ToolPathStartPoint;
                        rslt[0].ToolPathEndPoint = pathProfile.ToolPathEndPoint;
                    }
                    foreach (var profile2D in rslt)
                    {
                        profile2D.ToolPathStartPoint = profile2D.Source.First().GetFirstPnt();
                        profile2D.ToolPathEndPoint = profile2D.Source.Last().GetLastPnt();
                    }

                }


                return rslt;
            }

            catch (Exception ex)
            {

                return new List<Profile2D>();
            }

        }

        /// <summary>
        /// Unisce in profilo le linee consecutive ( endPrev == startNext)
        /// , quando 2 linee non sono consecutive , crea nuovo profilo.
        /// In questo metodo viene passato solamente in percorso alla volta
        /// Con percorso intenso un livello del profilo di offset.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static List<Profile2D> MergeLines(IEnumerable<Line2D> lines)
        {
            var profiles = new List<Profile2D>();

            Line2D prevLine = null;

            var currentProfile = new Profile2D();

            profiles.Add(currentProfile);

            foreach (var line in lines)
            {
                // primo elemento
                if (prevLine == null)
                {
                    currentProfile.AddPnt(line.Start);
                    currentProfile.AddPnt(line.End);
                }

                // linea consecutiva 
                else if (prevLine.End.Equals(line.Start))
                {
                    currentProfile.AddPnt(line.End);
                }

                // Linea non consecutiva , creazione nuovo profilo
                else if (!prevLine.End.Equals(line.Start))
                {
                    currentProfile = new Profile2D();
                    profiles.Add(currentProfile);

                    currentProfile.AddPnt(line.Start);
                    currentProfile.AddPnt(line.End);
                }

                prevLine = line;

            }

            /*
             * faccio un piccolo hack per gestire caso particolare di profilo "trimmato", diviso in 2 , che comincia da parte dentro invece di partire da fuori
             */

            var copy = profiles.ToList();


            /*
             * se il punto finale di un profilo coincide con punto iniziale di un'altro profilo,
             * attacco il secondo profilo al primo e lo rimuovo dalla lista profiles..
             */
            try
            {
                if (profiles.Count > 1)
                    for (int i = 0; i < profiles.Count; i++)
                    {
                        var profileStart = profiles[i];
                        var puntoFinale = profileStart.Source.Last().GetLastPnt();

                        for (int j = 0; j < profiles.Count; j++)
                        {
                            var profileEnd = profiles[j];
                            var puntoIniziale = profileEnd.Source.First().GetFirstPnt();

                            if (puntoFinale.Equals(puntoIniziale, 7))
                            {
                                var pnts = profileEnd.GetPointListP2();

                                foreach (var point2D in pnts)
                                {
                                    // in teoria potrei saltare il primo punto in quanto equivale all'ultimo
                                    // ma poi il codice per fare il parse degli archi mi spezza l'arco
                                    // cosi lo lascio inserito..

                                    profileStart.AddPnt(point2D);
                                }

                                profiles.Remove(profileEnd);
                            }
                        }
                    }

                return profiles;
            }
            catch (Exception ex)
            {
                return copy;
            }
        }

        //int pnpoly(int nvert, float* vertx, float* verty, float testx, float testy)
        //{
        //    int i, j, c = 0;
        //    for (i = 0, j = nvert - 1; i < nvert; j = i++)
        //    {
        //        if (((verty[i] > testy) != (verty[j] > testy)) &&
        //         (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
        //            c = !c;
        //    }
        //    return c;
        //}

        public static bool PointInPolygon(List<Point2D> pnts, Point2D pnt)
        {
            double testx = pnt.X;
            double testy = pnt.Y;

            int i, j = 0;

            bool isContained = false;

            for (i = 0, j = pnts.Count - 1; i < pnts.Count; j = i++)
            {
                if (((pnts[i].Y > testy) != (pnts[j].Y > testy)) &&
                 (testx < (pnts[j].X - pnts[i].X) * (testy - pnts[i].Y) / (pnts[j].Y - pnts[i].Y) + pnts[i].X))
                    isContained = !isContained;
            }
            return isContained;
        }

        /// <summary>
        /// Determina se un poligono è contenuto in un'altro poligono.
        /// </summary>
        /// <param name="polygonExtern"></param>
        /// <param name="polygonIntern"></param>
        /// <returns></returns>
        public static bool PolygonInPolygon(List<Point2D> polygonExtern, List<Point2D> polygonIntern)
        {
            /*
             * Per determinare se un poligono è incluso dentro un'altro poligono,
             * controllo tutti i punti del poligono presunto interno.
             * 
             * Devono stare tutti all'interno.
             */

            foreach (var point2D in polygonIntern)
            {
                if (!PointInPolygon(polygonExtern, point2D))
                    return false;
            }

            return true;
        }

        public static bool FilletEntity(Arc2D arc, Line2D line2D, double filletValue, bool lineFirst, out Arc2D filletArc)
        {
            Point2D intersectionPnt = null;

            if (lineFirst)
                intersectionPnt = line2D.End;
            else
            {
                intersectionPnt = arc.End;
            }

            /*
             * Todo : 
             * il codice sopra è corretto ma settando line.end mi da risultati , prendo questo per ora..
             */
            // var intersectionPnt = line2D.End;

            var normalAngle = getNormalAngle(arc, intersectionPnt);

            var normalEndpoint = GetCoordinate(normalAngle, 1);
            normalEndpoint.X += intersectionPnt.X;
            normalEndpoint.Y += intersectionPnt.Y;

            const bool boolFalse = false;

            var offsetArc = GeometryHelper.GetParallel(arc, filletValue, boolFalse);
            var offsetLine = GeometryHelper.GetParallel(line2D, filletValue, boolFalse);

            var pIncontro = GeometryHelper.GetIntersectPoint(offsetArc.Center, offsetArc.Radius, offsetLine);


            const bool boolTrue = true;


            var offsetArc2 = GeometryHelper.GetParallel(arc, filletValue, boolTrue);
            var offsetLine2 = GeometryHelper.GetParallel(line2D, filletValue, boolTrue);

            var pIncontro2 = GeometryHelper.GetIntersectPoint(offsetArc2.Center, offsetArc2.Radius, offsetLine2);


            var d1 = GeometryHelper.Distance(normalEndpoint, pIncontro);
            var d2 = GeometryHelper.Distance(normalEndpoint, pIncontro2);

            filletArc = new Arc2D();

            /*
             * mi rimane da determinare senso direzione 
             */

            if (d1 == null && d2 == null)
                return false;

            filletArc.Radius = filletValue;

            if (d1 < d2 || d2 == null)
            {
                filletArc.Center = pIncontro;

            }
            else if (d1 > d2 || d1 == null)
            {
                filletArc.Center = pIncontro2;
            }

            if (lineFirst)
            {
                filletArc.Start = GeometryHelper.GetIntersectPoint(filletArc.Center, filletValue, line2D);

                filletArc.End = GeometryHelper.GetIntersectPoint(filletArc, arc, filletArc.Center);

                /*
                 * qui il clockwise lavora come dovrebbe.. 
                 */
                filletArc.ClockWise = GeometryHelper.IsClockWise(filletArc.Start, filletArc.End, filletArc.Center);

            }

            else
            {
                filletArc.End = GeometryHelper.GetIntersectPoint(filletArc.Center, filletValue, line2D);

                filletArc.Start = GeometryHelper.GetIntersectPoint(filletArc, arc, filletArc.Center);

                /*
                 * qui il clockwise lavora come dovrebbe.. 
                 */
                filletArc.ClockWise = GeometryHelper.IsClockWise(filletArc.Start, filletArc.End, filletArc.Center);

            }


            return true;

        }







        ///// <summary>
        ///// Restiuisce normale del segmento da centro cerchio a punto cfr
        ///// </summary>
        ///// <param name="arc2D"></param>
        ///// <param name="interPnt">Deve risiedere sulla cfr</param>
        ///// <returns></returns>
        //public static double getNormalAngle(Arc2D arc2D, Point2D interPnt)
        //{
        //    var line = new Line2D();
        //    line.Start = arc2D.Center;
        //    line.End = interPnt;

        //    /*
        //     * todo : controllare che punto risieda sulla cfr
        //     */

        //    var angle = line.GetAngleLine();

        //    // devo tenere angolo positivo.
        //    if (angle < 0)
        //        angle += Math.PI * 2;
        //    /*
        //     * ora posso avere 2 normali
        //     */

        //    if (arc2D.ClockWise)
        //        angle -= Math.PI / 2;
        //    else
        //        angle += Math.PI / 2;

        //    return angle;
        //}

        /// <summary>
        /// Restiuisce normale del segmento da centro cerchio a punto cfr
        /// Restituisce la linea normale tra il centro della cfr e un punto situato su essa.
        /// è rivolta dalla parte della cfr.
        /// 
        /// </summary>
        /// <param name="arc2D"></param>
        /// <param name="interPnt">Deve risiedere sulla cfr</param>
        /// <returns></returns>
        public static double getNormalAngle(Arc2D arc2D, Point2D interPnt)
        {
            /*
             * Considerazioni:
             * - Centro ampiezza angolo tra punto finale , se è minore o uguale a PiGreco ok. altrimenti cambia direzione.
             */
            var line = new Line2D();
            line.Start = arc2D.Center;
            line.End = interPnt;

            /*
             * todo : controllare che punto risieda sulla cfr
             */

            var angle = line.GetAngleLine();

            // devo tenere angolo positivo.
            if (angle < 0)
                angle += Math.PI * 2;
            /*
             * ora posso avere 2 normali
             * 
             * Per capire quale è delle due devo vedere dove inizia
             * In ogni caso deve puntare dalla parte dove è cfr.
             * 
             * Se è 
             */

            if (arc2D.ClockWise)
                angle -= Math.PI / 2;
            else
                angle += Math.PI / 2;

            return angle;
        }

        ///// <summary>
        ///// Restiuisce normale del segmento da centro cerchio a punto cfr
        ///// </summary>
        ///// <param name="line"></param>
        ///// <param name="interPnt">Deve risiedere sulla cfr</param>
        ///// <param name="clockWise"></param>
        ///// <returns></returns>
        //public static double getNormalAngle(Line2D line, bool clockWise)
        //{
        //    /*
        //     * todo : controllare che punto risieda sulla cfr
        //     */

        //    var angle = line.GetAngleLine();

        //    // devo tenere angolo positivo.
        //    if (angle < 0)
        //        angle += Math.PI * 2;
        //    /*
        //     * ora posso avere 2 normali
        //     */

        //    if (clockWise)
        //        angle -= Math.PI / 2;
        //    else
        //        angle += Math.PI / 2;

        //    return angle;
        //}
        public static double GetAngle(Point2D p1, Point2D p2)
        {
            var rslt = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);

            return rslt;
        }

        public static double? Distance(Point2D p1, Point2D p2)
        {
            if (p1 == null || p2 == null)
                return null;

            return (Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y)));
        }

        public static double Distance(Point3D p1, Point3D p2)
        {
            double part1 = Math.Pow((p2.X - p1.X), 2);
            double part2 = Math.Pow((p2.Y - p1.Y), 2);
            double part3 = Math.Pow((p2.Z - p1.Z), 2);
            double underRadical = part1 + part2 + part3;
            var result = Math.Sqrt(underRadical);
            return result;
        }


        public static Point2D Circumcentre(Point2D a, Point2D b, Point2D c)
        {
            //double cx = c.X;
            //double cy = c.Y;
            //double ax = a.X - cx;
            //double ay = a.Y - cy;
            //double bx = b.X - cx;
            //double by = b.Y - cy;
            //double denom = 2 * Det(ax, ay, bx, by);
            //double numx = Det(ay, ax * ax + ay * ay, by, bx * bx + by * by);
            //double numy = Det(ax, ax * ax + ay * ay, bx, bx * bx + by * by);
            //double ccx = cx - numx / denom;
            //double ccy = cy + numy / denom;
            //return new Point2D(ccx, ccy);

            var cx = c.X;
            var cy = c.Y;
            var ax = a.X - cx;
            var ay = a.Y - cy;
            var bx = b.X - cx;
            var by = b.Y - cy;
            var denom = (2 * Det(ax, ay, bx, by));
            var numx = Det(ay, ax * ax + ay * ay, by, bx * bx + by * by);
            var numy = Det(ax, ax * ax + ay * ay, bx, bx * bx + by * by);
            var ccx = cx - numx / denom;
            var ccy = cy + numy / denom;
            return new Point2D(ccx, ccy);
        }
        /**           *            *            *            *            * @param m00 the [0,0] entry of the matrix           * @param m01 the [0,1] entry of the matrix           * @param m10 the [1,0] entry of the matrix           * @param m11 the [1,1] entry of the matrix           * @return the determinant           */
        ///<summary>          
        /// Computes the determinant of a 2x2 matrix. Uses standard double-precision arithmetic,           
        /// so is susceptible to round-off error.          
        ///</summary>          
        private static double Det(double m00, double m01, double m10, double m11)
        {
            return m00 * m11 - m01 * m10;
        }

        //public static bool IsInArcV_2(Point2D firstPoint, Point2D secondPoint, Point2D thirdPoint, Point2D newPoint, Point2D center, double radius, double tollerance)
        //{
        //    /* Nel buffer ci devono essere + di 4 elementi ,
        //     * prendo gli ultimi 3 e provo a vedere se ultimo punto è compatibile con raggio e centro trovato all'inizio.
        //     * 
        //     */
        //    var circoCentro = Circocentro(firstPoint, secondPoint, thirdPoint);
        //    // var radius = Distance(circoCentro, firstPoint);

        //    var newCircoCentro = Circocentro(secondPoint, thirdPoint, newPoint);
        //    //var newRadius = Distance(circoCentro, newPoint);

        //    if (newCircoCentro == null || circoCentro == null)
        //        return false;

        //    var distance = Distance(circoCentro, newCircoCentro);

        //    var distanceFromCenterBase = Distance(center, newCircoCentro);

        //    var radiusNew = Distance(newPoint, center);

        //    var radiusDiff = Math.Abs(radiusNew.Value - radius);

        //    return distance <= tollerance && radiusDiff <= tollerance && distanceFromCenterBase <= tollerance;
        //}


        //19/08/2011 10:54
        public static bool IsInArc(Point2D firstPoint, Point2D secondPoint, Point2D thirdPoint, Point2D fourthPnt, Point2D newPoint, out Point2D presuntCenterPoint, out double presuntArc)
        {
            //var midPnt1 = GetMidPoint(firstPoint, secondPoint);
            //var midPnt2 = GetMidPoint(secondPoint, thirdPoint);
            //var midPnt3 = GetMidPoint(thirdPoint, fourthPnt);
            //var newMidPoint = GetMidPoint(fourthPnt, newPoint);

            var circoCentro = Circocentro(firstPoint, secondPoint, thirdPoint);

            var newCircoCentro = Circocentro(secondPoint, thirdPoint, fourthPnt);

            if (newCircoCentro != null && circoCentro != null)
            {
                var distance = Distance(circoCentro, newCircoCentro);

                var radius = Distance(firstPoint, circoCentro);

                if (radius.HasValue && distance.HasValue)
                {
                    presuntArc = radius.Value;

                    presuntCenterPoint = circoCentro;
                    // anche questo non ok
                    var tolleranceProportional = radius / 5;

                    var rslt = Math.Abs(distance.Value) <= tolleranceProportional;

                    return rslt;
                }
            }

            presuntArc = 0;
            presuntCenterPoint = null;
            return false;
        }

        // 19/08
        //public static bool IsInArc(Point2D firstPoint, Point2D secondPoint, Point2D thirdPoint, Point2D newPoint, out Point2D presuntCenterPoint, out double presuntArc)
        //{
        //    var circoCentro = Circocentro(firstPoint, secondPoint, thirdPoint);
        //    //var circoCentro2 = Circocentro(firstPoint, secondPoint, thirdPoint);

        //    // var radius = Distance(circoCentro, firstPoint);

        //    var newCircoCentro = Circocentro(secondPoint, thirdPoint, newPoint);

        //    //var newRadius = Distance(circoCentro, newPoint);

        //    if (newCircoCentro != null && circoCentro != null)
        //    {
        //        var distance = Distance(circoCentro, newCircoCentro);

        //        var radius = Distance(firstPoint, circoCentro);

        //        if (radius.HasValue && distance.HasValue)
        //        {
        //            presuntArc = radius.Value;

        //            presuntCenterPoint = circoCentro;
        //            // anche questo non ok
        //            var tolleranceProportional = radius / 10;

        //            return Math.Abs(distance.Value) <= tolleranceProportional;

        //        }
        //    }

        //    presuntArc = 0;
        //    presuntCenterPoint = null;
        //    return false;
        //}

        public static Point2D GetCoordinate(double angle, double distance, Point2D startPnt = null)
        {
            var x = Math.Cos(angle) * distance;
            var y = Math.Sin(angle) * distance;
            if (startPnt != null)
            {
                x += startPnt.X;
                y += startPnt.Y;
            }
            return new Point2D(x, y);
        }

        public static Arc2D GetParallel(Arc2D arc2D, double distance, bool onLeftSide)
        {
            // Per evitare errori di immissione
            distance = Math.Abs(distance);

            var onRightSide = !onLeftSide;

            // todo che la distanza non sia maggiore del raggio quando offset interno.
            //var angleStart = Math.Atan2(arc2D.Start.Y - arc2D.Center.Y, arc2D.Start.X - arc2D.Center.X);

            //var angleEnd = Math.Atan2(arc2D.End.Y - arc2D.Center.Y, arc2D.End.X - arc2D.Center.X);

            /*
             * Se è a leftSide (SX) la distanza è negativa
             * Se è a DX la distanza è positiva
             */

            if ((onLeftSide == true && arc2D.ClockWise == false) ||
                (onRightSide && arc2D.ClockWise))
                distance = -distance;

            var start = GetPointAtDistance(arc2D.Center, arc2D.Start, -distance, false);
            var end = GetPointAtDistance(arc2D.Center, arc2D.End, -distance, false);
            var radius = arc2D.Radius + distance;

            var arcRslt = new Arc2D()
                              {
                                  Start = start,
                                  End = end,
                                  ClockWise = arc2D.ClockWise,
                                  Radius = radius,
                                  Center = arc2D.Center
                              };
            return arcRslt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="distance"></param>
        /// <param name="firstPoint"></param>
        /// <param name="nullOnOverDistance"></param>
        /// <returns>Null se distance è maggiore di total distance</returns>
        public static Point2D GetPointAtDistance(Point2D p1, Point2D p2, double distance, bool firstPoint, bool nullOnOverDistance = true)
        {
            var totalDistance = Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));

            if (distance > totalDistance && nullOnOverDistance)
                return null;

            var rap = firstPoint ? distance / totalDistance : (totalDistance - distance) / totalDistance;
            return new Point2D(p1.X + (rap * (p2.X - p1.X)), p1.Y + (rap * (p2.Y - p1.Y)));
        }

        public static Line2D GetParallel(Line2D line2D, double distance, bool onLeftSide)
        {
            double lineAngle = line2D.GetAngleLine();

            var angle = lineAngle + Math.PI / 2;

            if (onLeftSide == false)
                angle += Math.PI;

            var p1 = GetCoordinate(angle, distance);
            var p2 = GetCoordinate(angle, distance);

            var lineRslt = new Line2D
                               {
                                   Start = { X = line2D.Start.X + p1.X, Y = line2D.Start.Y + p1.Y },
                                   End =
                                       {
                                           X = line2D.End.X + p2.X,
                                           Y = line2D.End.Y + p2.Y
                                       }
                               };

            return lineRslt;

        }


        public static Point2D Circocentro(Point2D p1, Point2D p2, Point2D p3)
        {
            /*
             * procedure Circumcenter(const x1, y1, x2, y2, x3, y3: Double; out Px, Py: Double); 
var 
  A: Double; 
  C: Double; 
  B: Double; 
  D: Double; 
  E: Double; 
  F: Double; 
  G: Double; 
begin 
  A := x2 - x1; 
  B := y2 - y1; 
  C := x3 - x1; 
  D := y3 - y1; 
  E := A * (x1 + x2) + B * (y1 + y2); 
  F := C * (x1 + x3) + D * (y1 + y3); 
  G := 2.0 * (A * (y3 - y2) - B * (x3 - x2)); 
  if IsEqual(G, 0.0) then Exit; 
  Px := (D * E - B * F) / G; 
  Py := (A * F - C * E) / G; 
end;
             * 
             */
            var a = p2.X - p1.X;
            var b = p2.Y - p1.Y;
            var c = p3.X - p1.X;
            var d = p3.Y - p1.Y;
            var e = a * (p1.X + p2.X) + b * (p1.Y + p2.Y);
            var f = c * (p1.X + p3.X) + d * (p1.Y + p3.Y);
            var g = 2.0 * (a * (p3.Y - p2.Y) - b * (p3.X - p2.X));

            if (g == 0) return null;

            var rslt = new Point2D { X = (d * e - b * f) / g, Y = (a * f - c * e) / g };

            return rslt;
        }


        public static double GetPositiveAngle(double radian)
        {

            if (radian < 0)
                radian = (Math.PI * 2) - Math.Abs(radian);

            return radian;
        }

        public static double GetMinAngleDifference(double angle1, double angle2)
        {
            // Prendo solamente valori positivi 
            var rslt1 = GetPositiveAngle(angle1);
            var rslt2 = GetPositiveAngle(angle2);

            if (rslt2 < rslt1)
                rslt2 += 2 * Math.PI;

            var angleMax = Math.Max(rslt1, rslt2);
            var angleMin = Math.Min(rslt1, rslt2);

            var dA = angleMax - angleMin;

            if (dA < Math.PI)
                return dA;

            return Math.PI * 2 - dA;
        }



        public static double GetPositiveAngle(double x, double y)
        {
            /* es : 
             * rslt  - 45
             * 
             * se rslt < 0
             * rslt  = 360 - abs(45)
             * 
             * rslt = 315 ok
             */
            var rslt = Math.Atan2(y, x);

            if (rslt < 0)
                rslt = (Math.PI * 2) - Math.Abs(rslt);

            return rslt;
        }

        public static double GetRaggioCircoscritto(double raggioInscritto, int numeroLati)
        {
            var r = raggioInscritto / (Math.Cos(Math.PI / numeroLati));

            return r;
        }


        public static List<Point2D> CalculatePoygonInscribed(int numberSide, Point2D center, double radius)
        {
            var angle = 2 * Math.PI / numberSide;

            var list = new List<Point2D>();

            for (var i = 0; i < numberSide; i++)
            {
                var point = new Point2D
                {
                    X = center.X + radius * Math.Sin(i * angle),
                    Y = center.Y + radius * Math.Cos(i * angle)
                };

                list.Add(point);
            }

            return list;
        }

        public static Point3D MultiplyPoint(Point3D point3D, Matrix3D matrix3D)
        {
            var pnt = new System.Windows.Media.Media3D.Point3D(point3D.X, point3D.Y, point3D.Z);

            var rslt = System.Windows.Media.Media3D.Point3D.Multiply(pnt, matrix3D);

            return new Point3D(rslt.X, rslt.Y, rslt.Z);
        }
        internal static Point2D MultiplyPoint(Point2D start, Matrix3D matrix)
        {
            var pnt = new System.Windows.Media.Media3D.Point3D(start.X, start.Y, 0);

            var rslt = System.Windows.Media.Media3D.Point3D.Multiply(pnt, matrix);

            return new Point2D(rslt.X, rslt.Y);
        }

        public static Line3D MultiplyLine(Line3D line3D, Matrix3D matrix3D)
        {
            var start = MultiplyPoint(line3D.Start, matrix3D);
            var end = MultiplyPoint(line3D.End, matrix3D);

            return new Line3D() { Start = start, End = end };
        }

        /*
         * template <typename Real>
bool IntrSegment2Arc2<Real>::Find ()
{
    Real t[2];
    int quantity;
    bool intersects = IntrLine2Circle2<Real>::Find(mSegment->Center,
        mSegment->Direction, mArc->Center, mArc->Radius, quantity, t);

    mQuantity = 0;
    if (intersects)
    {
        // Reduce root count if line-circle intersections are not on segment.
        if (quantity == 1)
        {
            if (Math<Real>::FAbs(t[0]) > mSegment->Extent)
            {
                quantity = 0;
            }
        }
        else
        {
            if (t[1] < -mSegment->Extent || t[0] > mSegment->Extent)
            {
                quantity = 0;
            }
            else
            {
                if (t[1] <= mSegment->Extent)
                {
                    if (t[0] < -mSegment->Extent)
                    {
                        quantity = 1;
                        t[0] = t[1];
                    }
                }
                else
                {
                    quantity = (t[0] >= -mSegment->Extent ? 1 : 0);
                }
            }
        }

        for (int i = 0; i < quantity; ++i)
        {
            Vector2<Real> point = mSegment->Center +
                mSegment->Direction*t[i];

            if (mArc->Contains(point))
            {
                mPoint[mQuantity++] = point;
            }
        }
    }

    mIntersectionType = (mQuantity > 0 ? IT_POINT : IT_EMPTY);
    return mIntersectionType != IT_EMPTY;
}
//----------------------------------------------------------------------------
template <typename Real>
int IntrSegment2Arc2<Real>::GetQuantity () const
{
    return mQuantity;
}
//----------------------------------------------------------------------------
template <typename Real>
const Vector2<Real>& IntrSegment2Arc2<Real>::GetPoint (int i) const
{
    return mPoint[i];
}

         */

        public static bool IsClockWise(Point2D p1, Point2D p2, Point2D center)
        {
            // todo: gestire i 3 punti collineari

            // Se il determinante della matrice è positivo il senso è ccw , se negativo è cw
            var matrix = new Matrix(3, 3);

            matrix[0, 0] = p1.X;
            matrix[0, 1] = p1.Y;
            matrix[0, 2] = 1;

            matrix[1, 0] = p2.X;
            matrix[1, 1] = p2.Y;
            matrix[1, 2] = 1;

            matrix[2, 0] = center.X;
            matrix[2, 1] = center.Y;
            matrix[2, 2] = 1;

            var det = Matrix.Det(matrix);

            return (det < 0);
        }

        public static Point2D GetIntersectPoint(Point2D arcCenterPnt, double radiusValue, Line2D offsetLine)
        {
            Point2D firstC;
            Point2D secondC;

            var rslt = FindLineCircleIntersections(arcCenterPnt.X, arcCenterPnt.Y, radiusValue, offsetLine.Start,
                                                   offsetLine.End, out firstC, out secondC);

            if (rslt == 0)
                return null;

            if (rslt == 1)
                return firstC;

            if (rslt == 2)
            {
                var d1 = Distance(offsetLine.End, firstC);
                var d2 = Distance(offsetLine.End, secondC);

                if (d1 < d2)
                    return firstC;
                return secondC;
            }

            return null;
        }

        public static Point2D GetIntersectPoint(Arc2D arc1, Arc2D arc2, Point2D centerRadius)
        {
            Point2D firstC;
            Point2D secondC;

            var rslt = FindCircleCircleIntersections(arc1.Center.X, arc1.Center.Y, arc1.Radius,
                                                     arc2.Center.X, arc2.Center.Y, arc2.Radius,
                                                     out firstC, out secondC);

            if (rslt == 0)
                return null;

            if (rslt == 1)
                return firstC;

            if (rslt == 2)
            {
                var d1 = Distance(centerRadius, firstC);
                var d2 = Distance(centerRadius, secondC);

                if (d1 < d2)
                    return firstC;
                return secondC;
            }

            return null;
        }

        // Find the points of intersection.
        public static int FindLineCircleIntersections(double cx, double cy, double radius, Point2D point1, Point2D point2, out Point2D intersection1, out Point2D intersection2)
        {
            double dx, dy, A, B, C, det;
            double t = 0;
            dx = point2.X - point1.X;
            dy = point2.Y - point1.Y;
            A = dx * dx + dy * dy;
            B = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
            C = (point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - radius * radius;
            det = B * B - 4 * A * C;

            // Arrotonda 

            det = Math.Round(det, 4);
            //var roundValue = 0.0001;

            //if (det < roundValue && det > -roundValue)
            //    det = 0;

            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.  
                intersection1 = new Point2D(double.NaN, double.NaN);
                intersection2 = new Point2D(double.NaN, double.NaN);
                return 0;
            }
            else if (det == 0)
            {
                // One solution.       
                t = -B / (2 * A);
                intersection1 = new Point2D(point1.X + t * dx, point1.Y + t * dy);
                intersection2 = new Point2D(double.NaN, double.NaN);
                return 1;
            }
            else
            {
                // Two solutions.     
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                intersection1 = new Point2D(point1.X + t * dx, point1.Y + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                intersection2 = new Point2D(point1.X + t * dx, point1.Y + t * dy);
                return 2;
            }
        }


        public static Point2D GetCenterFromTwoPointsAndRadius(Point2D p1, Point2D p2, double radius, bool clockWise)
        {
            //points (x1,y1) and (x2,y2)
            //radius r

            var mx = (p1.X + p2.X) / 2;
            var my = (p1.Y + p2.Y) / 2;

            var leg1x = mx - p1.X;
            var leg1y = my - p1.Y;

            var leg1 = Math.Sqrt(leg1x * leg1x + leg1y * leg1y);

            if (leg1 > Math.Abs(radius))
                return null; //no solution

            var leg2 = Math.Sqrt(radius * radius - leg1 * leg1);
            var leg2x = leg1y * leg2 / leg1;
            var leg2y = -leg1x * leg2 / leg1;

            if (clockWise)
            {
                var c1x = mx + leg2x;
                var c1y = my + leg2y;

                return new Point2D(c1x, c1y);
            }


            var c2x = mx - leg2x;
            var c2y = my - leg2y;

            return new Point2D(c2x, c2y);
        }

        // Find the points where the two circles intersect.
        public static int FindCircleCircleIntersections(double cx0, double cy0, double radius0, double cx1, double cy1, double radius1, out Point2D intersection1, out Point2D intersection2)
        {
            // Find the distance between the centers. 
            double dx = cx0 - cx1;
            double dy = cy0 - cy1;
            double dist1 = Math.Sqrt(dx * dx + dy * dy);

            //dist = (float)Math.Round(dist, 4);
            float dist = (float)dist1;


            // See how manhym solutions there are.    
            if (dist > radius0 + radius1)
            {
                // No solutions, the circles are too far apart.
                intersection1 = new Point2D(float.NaN, float.NaN);
                intersection2 = new Point2D(float.NaN, float.NaN);
                return 0;
            }
            else if (dist < Math.Abs(radius0 - radius1))
            {
                // No solutions, one circle contains the other.  
                intersection1 = new Point2D(float.NaN, float.NaN);
                intersection2 = new Point2D(float.NaN, float.NaN);
                return 0;
            }
            else if ((dist == 0) && (radius0 == radius1))
            {
                // No solutions, the circles coincide.    
                intersection1 = new Point2D(float.NaN, float.NaN);
                intersection2 = new Point2D(float.NaN, float.NaN);
                return 0;
            }
            else
            {
                // Find a and h.  
                double a = (radius0 * radius0 - radius1 * radius1 + dist * dist) / (2 * dist);
                double h = Math.Sqrt(radius0 * radius0 - a * a);
                // Find P2.   
                double cx2 = cx0 + a * (cx1 - cx0) / dist;
                double cy2 = cy0 + a * (cy1 - cy0) / dist;
                // Get the points P3.      
                intersection1 = new Point2D((float)(cx2 + h * (cy1 - cy0) / dist), (float)(cy2 - h * (cx1 - cx0) / dist));
                intersection2 = new Point2D((float)(cx2 - h * (cy1 - cy0) / dist), (float)(cy2 + h * (cx1 - cx0) / dist));
                // See if we have 1 or 2 solutions.    
                if (dist == radius0 + radius1)
                    return 1;
                return 2;
            }
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        /// <summary>
        /// Restituisce differenza fra primo e secondo angolo
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="clockWise"></param>
        /// <returns></returns>
        public static object GetDeltaAngle(double startAngle, double endAngle, bool clockWise)
        {
            if (startAngle < 0)
            {
                startAngle += Math.PI * 2;
                endAngle += Math.PI * 2;
            }

            var d = endAngle - startAngle;

            if (clockWise)
                return d;

            return Math.PI * 2 - d;


        }

        /*
         * Trigonometria , risoluzione triangoli
         * http://www.math.it/formulario/trigonometria.htm
         */
        public static double TrigonometricFindX(Point2D prevTeoricalPnt, double angle, double endY)
        {
            var b = (endY - prevTeoricalPnt.Y);

            var c = (1 / Math.Tan(angle)) * b;

            return c + prevTeoricalPnt.X;
        }

        public static double TrigonometricFindY(Point2D prevTeoricalPnt, double angle, double endY)
        {
            /*
             * devo gestire angolo > 90
             */

            //            var c = GetDelta(prevTeoricalPnt.X, prevTeoricalPnt.X + incrementX);
            var c = endY - prevTeoricalPnt.X;

            //var gamma = Math.PI / 2 - angle;

            var b = c * Math.Tan(angle);

            //var deltaY = Math.Tan(angle) * deltaX;

            return b + prevTeoricalPnt.Y;

        }

        //        public static double TrigonometricFindX(Point2D prevTeoricalPnt, double angle, double incrementY)
        //        {
        //            var b = (incrementY + prevTeoricalPnt.Y) - prevTeoricalPnt.Y;

        //            var c = (1 / Math.Tan(angle)) * b;

        //            return c + prevTeoricalPnt.X;
        //        }

        //        public static double TrigonometricFindY(Point2D prevTeoricalPnt, double angle, double incrementX)
        //        {
        //            /*
        //             * devo gestire angolo > 90
        //             */

        ////            var c = GetDelta(prevTeoricalPnt.X, prevTeoricalPnt.X + incrementX);
        //            var c = (prevTeoricalPnt.X + incrementX) - prevTeoricalPnt.X;

        //            //var gamma = Math.PI / 2 - angle;

        //            var b = c * Math.Tan(angle);

        //            //var deltaY = Math.Tan(angle) * deltaX;

        //            return b + prevTeoricalPnt.Y;

        //        }
        /// <summary>
        /// Restituisce la differenza fra 2 numeri.
        /// Definisco min - max poi prenda la differenza tra max - min
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static double GetDelta(double d1, double d2)
        {
            var max = Math.Max(d1, d2);
            var min = Math.Min(d1, d2);

            return max - min;
        }

        internal static bool ValutateRadiusCorrectness(double raggio, double centroX, double centroY, double p1, double p2)
        {
            var distance = Distance(new Point2D(centroX, centroY), new Point2D(p1, p2));

            if (distance.HasValue)
            {
                var d = (float)distance.Value;

                if (d == raggio) return true;
            }

            return false;
        }

        public static Point2D GetClosestPoint(List<Point2D> polygonPnts, Point2D point)
        {
            Point2D prevPoint = null;

            var minDistance = double.MaxValue;

            Point2D closestPoint = null;

            foreach (var polygonPnt in polygonPnts)
            {
                if (prevPoint != null)
                {
                    Point2D pointClosest;

                    var distance = FindDistanceToSegment(point, prevPoint, polygonPnt, out pointClosest);

                    if (distance < minDistance)
                    {
                        minDistance = distance;

                        closestPoint = pointClosest;
                    }
                }

                prevPoint = polygonPnt;
            }

            return closestPoint;


        }

        #region Minimum distance Point - Segment


        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        public static double FindDistanceToSegment(Point2D pt, Point2D p1, Point2D p2, out Point2D closest)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            var t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Point2D(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Point2D(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Point2D(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        //public static Point2D GetClosestPoint(Point2D a, Point2D b, Point2D p)
        //{
        //    var vectorAP = new Point2D(p.X - a.X, p.Y - a.Y);   //Vector from A to P
        //    var vectorAB = new Point2D(b.X - a.X, b.Y - a.Y); //Vector from A to B

        //    var magnitudeAB = Math.Pow(vectorAB.X, 2) + Math.Pow(vectorAB.Y, 2);
        //    //Magnitude of AB vector (it's length)


        //    var ABAPproduct = vectorAB.X * vectorAP.X + vectorAB.Y * vectorAP.Y;
        //    //The product of a_to_p and a_to_b


        //    var distance = ABAPproduct / magnitudeAB;
        //    //The normalized "distance" from a to your closest point
        //    Point2D returnPoint = new Point2D();

        //    if (distance < 0)     //Check if P projection is over vectorAB
        //    {
        //        returnPoint.X = a.X;
        //        returnPoint.Y = a.Y;
        //    }
        //    else if (distance > magnitudeAB)
        //    {
        //        returnPoint.X = b.X;
        //        returnPoint.Y = b.Y;
        //    }
        //    else
        //    {
        //        returnPoint.X = a.X + vectorAB.X * distance;
        //        returnPoint.Y = a.Y + vectorAB.Y * distance;
        //    }

        //    return returnPoint;
        //}

        ////Compute the dot product AB . AC
        //private static double DotProduct(Point2D pointA, Point2D pointB, Point2D pointC)
        //{
        //    var AB = new Point2D();
        //    var BC = new Point2D();
        //    AB.X = pointB.X - pointA.X;
        //    AB.Y = pointB.Y - pointA.Y;
        //    BC.X = pointC.X - pointB.X;
        //    BC.Y = pointC.Y - pointB.Y;
        //    double dot = AB.X * BC.X + AB.Y * BC.Y;

        //    return dot;
        //}

        ////Compute the cross product AB x AC
        //private static double CrossProduct(Point2D pointA, Point2D pointB, Point2D pointC)
        //{
        //    var AB = new Point2D();
        //    var AC = new Point2D();
        //    AB.X = pointB.X - pointA.X;
        //    AB.Y = pointB.Y - pointA.Y;
        //    AC.X = pointC.X - pointA.X;
        //    AC.Y = pointC.Y - pointA.Y;
        //    double cross = AB.X * AC.Y - AB.Y * AC.X;

        //    return cross;
        //}

        ////Compute the distance from A to B
        //private static double Distance2DPoint(Point2D pointA, Point2D pointB)
        //{
        //    double d1 = pointA.X - pointB.X;
        //    double d2 = pointA.Y - pointB.Y;

        //    return Math.Sqrt(d1 * d1 + d2 * d2);
        //}

        ////Compute the distance from AB to C
        ////if isSegment is true, AB is a segment, not a line.
        //public static double LineToPointDistance2D(Point2D pointA, Point2D pointB, Point2D pointC,
        //    bool isSegment)
        //{
        //    double dist = CrossProduct(pointA, pointB, pointC) / Distance2DPoint(pointA, pointB);
        //    if (isSegment)
        //    {
        //        double dot1 = DotProduct(pointA, pointB, pointC);
        //        if (dot1 > 0)
        //            return Distance2DPoint(pointB, pointC);

        //        double dot2 = DotProduct(pointB, pointA, pointC);
        //        if (dot2 > 0)
        //            return Distance2DPoint(pointA, pointC);
        //    }
        //    return Math.Abs(dist);
        //}


        #endregion

    }


}

