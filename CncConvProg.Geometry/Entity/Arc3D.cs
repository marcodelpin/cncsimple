using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.Entity
{
    [Serializable]
    public class Arc3D : IEntity3D
    {
        public Arc3D()
        {
            Start = new Point3D();
            End = new Point3D();
            Center = new Point3D();
        }
        public Point3D Start;
        public Point3D End;
        public double Radius;
        public Point3D Center;
        public bool ClockWise;

        public EnumPlotStyle PlotStyle { get; set; }

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

        /// <summary>
        /// Ritorna Lunghezza Elica o Arco
        /// </summary>
        /// <returns></returns>
        public double GetLength()
        {
            // mi devo assicurare che tutti i putni dell'arco siano definiti.
            var deltaZ = Math.Max(End.Z, Start.Z) - Math.Min(End.Z, Start.Z);

            /*
             * per avere passo devo fare proporzione tra ampiezza angolo e delta z e fra incognita passo e giro completo
             */

            var startAngle = Math.Atan2(Start.Y - Center.Y, Start.X - Start.X);
            var endAngle = Math.Atan2(End.Y - Center.Y, End.X - Start.X);

            startAngle = GeometryHelper.GetPositiveAngle(startAngle);
            endAngle = GeometryHelper.GetPositiveAngle(endAngle);
            /*
             * devo metterli in forma positiva
             */

            /*
             * se arco è CW mi muovo in maniera devo angleStart - angleEnd
             * se arco è CCW mi muovo in maniera devo angleEnd - angleStart
             */

            double deltaAngle;

            if (ClockWise)
            {
                if (startAngle == endAngle) // caso giro completo
                {
                    endAngle -= Math.PI * 2;
                }
                if (startAngle < endAngle)
                    startAngle += Math.PI * 2;

                deltaAngle = startAngle - endAngle;
            }
            else // counterclockwise
            {
                if (startAngle == endAngle) // caso giro completo
                {
                    endAngle += Math.PI * 2;

                }
                // nel caso di ccw il senso è sempre l'angolo finale deve essere sempre maggiore

                if (endAngle < startAngle)
                    endAngle += Math.PI * 2;

                deltaAngle = endAngle - startAngle;
            }

            if (deltaZ == 0)
            {
                // calcolo lunghezza arco
                // l = C * a / 2*pi

                var lunghezzaArco = ((2 * Math.PI * Radius) * deltaAngle) / (2 * Math.PI);

                return lunghezzaArco;
            }
            /*
             * Faccio la proporzione sopra detta
             * 
             * deltaZ : deltaAngle = passoIncognito : 2*Pi
             * 
             * da qui ..
             */

            var passo = ((Math.PI * 2) * deltaZ) / deltaAngle;

            /*
             * Formula lunghezza spirale
             * http://www.matematicamente.it/forum/lunghezza-elica-t21181.html
             * 
             * poi moltiplico per (altezzaElica / passo )
             */

            var lunghezzaElicaGiro = 2 * Math.PI * (Math.Sqrt(Math.Pow(Radius, 2) + Math.Pow(passo / Math.PI * 2, 2)));

            var coeff = deltaZ / passo;

            var lunghezza = lunghezzaElicaGiro * coeff;

            return lunghezza;

        }

        public Rect3D GetBoundary()
        {
            //            var rect3D = new Rect3D(Start.X, Start.Y, Start.Z, Math.Abs(End.X - Start.X), Math.Abs(End.Y - Start.Y), Math.Abs(End.Z - Start.Z));
            var rect3D = new Rect3D(Center.X - Radius, Center.Y - Radius, Center.Z, Radius * 2, Radius * 2, Math.Abs(End.Z - Start.Z));

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
            var arc = new Arc3D
                          {
                              ClockWise = ClockWise,
                              PlotStyle = PlotStyle,
                              Radius = Radius,
                              Start =
                                  GeometryHelper.MultiplyPoint(new Point3D(Start.X, Start.Y, Start.Z), rotationMatrix),
                              Center =
                                  GeometryHelper.MultiplyPoint(new Point3D(Center.X, Center.Y, Center.Z), rotationMatrix),
                              End = GeometryHelper.MultiplyPoint(new Point3D(End.X, End.Y, End.Z), rotationMatrix),
                              IsSelected = IsSelected,
                          };

            return arc;
        }
    }
}
