using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;
using MatrixLibrary;

namespace CncConvProg.Geometry.RawProfile2D
{
    /*
     * 27/07/2011
     * 
     * - mettere apposto centro arco e angolo linea 
     * - completare endradius e endchamfer fra le entita rimanenti.
     * - non sarebbe male anche dire dove ci sono overdefined..
     * 
     */

    [Serializable]
    public class RawProfile : List<RawEntity2D>
    {
        //private readonly List<RawEntity2D> _rawProfile = new List<RawEntity2D>();

        // distinguere tra profillo chiuso e profilo aperto..

        public bool IsOpenProfile { get; private set; }

        public RawProfile(bool isOpenProfile)
        {
            IsOpenProfile = isOpenProfile;
        }

        /*
         * se come risultato desse solamente dei punti sarebbe meglio ??
         */

        public RawProfile(IEnumerable<RawEntity2D> source)
        {
            foreach (var rawEntity2D in source)
            {
                rawEntity2D.Profile = this;
                Add(rawEntity2D);
            }
        }

        internal RawEntity2D GetPrev(RawEntity2D rawEntity2D)
        {
            if (!Contains(rawEntity2D)) return null;

            var index = IndexOf(rawEntity2D);

            //// se è il primo, ritorno ultimo elemento  
            //if (index == 0 && Count > 0)
            //{
            //    return this[Count - 1];
            //}

            return this.ElementAtOrDefault(index - 1);
        }

        public RawEntity2D GetNext(RawEntity2D rawEntity2D)
        {
            if (!Contains(rawEntity2D)) return null;

            var index = IndexOf(rawEntity2D);

            if (!IsOpenProfile)
                //// se è l'ultimo, ritorna primo elemento  
                if (index == Count - 1 && Count > 1)
                {
                    // il primo è sempre initPoint , quindi ritorno secondo..
                    return this[1];
                }

            return this.ElementAtOrDefault(index + 1);
        }



        /*
         * riepilogando conclusione.
         * 
         * itero elementi del profilo una volta per riempire variabili vuote.
         * 
         * 
         * seconda per estrarre profilo comprensivo di smussi.
         * quando raggiungo ultimo elemento e ce raccordo che si congiunge con primo 
         * devo andare a modificare elementi temporanei, : sara il primo elento della lista del risultato..
         * 
         * prima estraggo line2d, poi smusso con altra line2d o arc2d a seconda..
         * 
         * per profilo parziale ho visto che basta raccogliere incrementali.
         * 
         * x? y9  
         * ix = -20; ( angolo noto) ( se ci sono piu elementi definiti da incrementale , fare metodo ricorsivo )
         * x33 y33 a45 caso ok
         * 
          * x? y9   ( punto incrocio)
         * x33 y33 a45 caso ok
         * 
         * x? y9   
         * iy = 10  ( non possibile )
         * x33 y33 a45 caso ok
         * 
         * fare in modo che si estensibile..
         * 
         */

        /// <summary>
        /// Metodo campione
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IEntity2D> UpdateCycle()
        {
            // todo : se ultimo e primo elemento non hanno punto teorico in comune, non si può smussare..
            // ciclo di aggiornamento, qui compilo i raw con valori calcolati

            {
                /*
                 * metodo che lancia update
                 */
                RawEntity2D prevEntity = null;
                foreach (var rawEntity in this)
                {
                    rawEntity.SolveElement();
                    //if (prevEntity != null)
                    //    TrySolve(prevEntity, rawEntity);

                    //prevEntity = rawEntity;
                }
            }

            var rslt = new List<IEntity2D>();

            foreach (var element in this)
            {
                // ritorna singola linea o arco.
                var geometry = element.ResultGeometry();

                if (geometry == null) continue;

                if (element.Chamfer.Value.HasValue ||
                    element.EndRadius.Value.HasValue)
                {


                    var nextRaw = GetNext(element);

                    if (nextRaw == null) continue;

                    var nextGeometry = nextRaw.ResultGeometry();

                    if (nextGeometry == null) continue;

                    if (element.Chamfer.Value.HasValue)
                    {
                        bool chamferSuccess;

                        var chamferRslt = CreateChamfer(geometry, nextGeometry, element.Chamfer.Value.Value,
                                                        out chamferSuccess);

                        if (chamferSuccess)
                        {
                            rslt.AddRange(chamferRslt);
                            continue;
                        }
                        else
                        {
                            // setta errore su element,
                        }
                    }

                    if (element.EndRadius.Value.HasValue)
                    {
                        bool radiusSuccess;

                        var chamferRslt = CreateRadius(geometry, nextGeometry, element.EndRadius.Value.Value,
                                                        out radiusSuccess);

                        if (radiusSuccess)
                        {
                            rslt.AddRange(chamferRslt);
                            continue;
                        }
                        else
                        {
                            // setta errore su element,
                        }

                    }
                }

                rslt.Add(geometry);

            }

            return rslt;
        }

        private static IEnumerable<IEntity2D> CreateRadius(IEntity2D geometry, IEntity2D nextGeometry, double radiusValue, out bool radiusSuccess)
        {
            var line1 = geometry as Line2D;
            var arc1 = geometry as Arc2D;
            var line2 = nextGeometry as Line2D;
            var arc2 = nextGeometry as Arc2D;

            if (line1 != null && line2 != null)
                return CreateRadius(line1, line2, radiusValue, out radiusSuccess);

            if (line1 != null && arc2 != null)
                return CreateRadius(line1, arc2, radiusValue, out radiusSuccess);

            if (arc1 != null && line2 != null)
                return CreateRadius(arc1, line2, radiusValue, out radiusSuccess);

            if (arc1 != null && arc2 != null)
                return CreateRadius(arc1, arc2, radiusValue, out radiusSuccess);

            throw new NotImplementedException();
        }


        /// <summary>
        ///  todo  _ cambia a seconda se si tratta di senso orario o antiorario
        /// </summary>
        /// <param name="angle1"></param>
        /// <param name="angle2"></param>
        /// <returns></returns>
        private static double GetAverageAngle(double angle1, double angle2)
        {
            //if (angle2 == 0)
            //    angle2 = Math.PI;

            var x = Math.Cos(angle1);
            var y = Math.Sin(angle1);

            x += Math.Cos(angle2);
            y += Math.Sin(angle2);

            var angle = Math.Atan2(y, x);

            //if (angle < 0)
            //    angle = (Math.PI * 2) - Math.Abs(angle);

            return angle;
        }
        private static double GetDistanceTrimmedByRadius(double angle, double radius)
        {
            angle = Math.Abs(angle);

            radius = Math.Abs(radius);

            var tan = Math.Tan(angle);

            return Math.Abs(tan * radius);
        }

        private static Point2D GetPointAtAngleAndDistance(double distance, double angle)
        {
            var pnt = new Point2D
                          {
                              Y = Math.Sin(angle) * distance,
                              X = Math.Cos(angle) * distance
                          };

            return pnt;

        }

        /// <summary>
        /// Raccorda elementi 2D dove primo elemento è linea e secondo è arco.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="arc"></param>
        /// <param name="radiusValue"></param>
        /// <param name="radiusSuccess"></param>
        /// <returns></returns>
        private static IEnumerable<IEntity2D> CreateRadius(Line2D line, Arc2D arc, double radiusValue, out bool radiusSuccess)
        {
            Arc2D filletArc = null;

            var rsl = GeometryHelper.FilletEntity(arc, line, radiusValue, true, out filletArc);


            if (rsl)
            {
                var lineWithRadius = new List<IEntity2D>();

                arc.Start = filletArc.End;
                line.End = filletArc.Start;

                lineWithRadius.Add(line);
                lineWithRadius.Add(filletArc);

                foreach (var entity2D in lineWithRadius)
                    entity2D.PlotStyle = line.PlotStyle;


                radiusSuccess = true;
                return lineWithRadius;
            }

            radiusSuccess = false;
            return null;

        }

        private static IEnumerable<IEntity2D> CreateRadius(Arc2D arc1, Arc2D arc2, double radiusValue, out bool radiusSuccess)
        {
            radiusSuccess = false;
            return null;
            //var arc = new Arc2D()
            //{
            //    Start = new Point2D(19.5002, 6.878),
            //    End = new Point2D(0, 10),
            //    Center = new Point2D(10, 10),
            //    Radius = 10,

            //};

            //var line = new Line2D()
            //{
            //    Start = new Point2D(40.43, 0),
            //    End = new Point2D(19.5, 6.878),
            //};

            //var offsetArc = GeometryHelper.GetParallel(arc, 4, profileLeft);
            //var offsetLine = GeometryHelper.GetParallel(line, 4, profileLeft);

            //var pIncontro = GeometryHelper.GetIntersectPoint(offsetArc, offsetLine);

            //var raccordo = new Arc2D();
            //raccordo.Center = pIncontro;
            //raccordo.Radius = 4;

            //var pAttaccoLinea = GeometryHelper.GetIntersectPoint(raccordo, line);
            //var pAttaccoRaggio = GeometryHelper.GetIntersectPoint(raccordo, arc, pIncontro);

        }

        private static IEnumerable<IEntity2D> CreateRadius(Arc2D arc, Line2D line, double radiusValue, out bool radiusSuccess)
        {
            Arc2D filletArc = null;

            var rsl = GeometryHelper.FilletEntity(arc, line, radiusValue, false, out filletArc);

            if (rsl)
            {
                var lineWithRadius = new List<IEntity2D>();

                /*
                 * 
                 * 
                 */
                arc.End = filletArc.Start;
                line.Start = filletArc.End;

                lineWithRadius.Add(arc);
                lineWithRadius.Add(filletArc);

                foreach (var entity2D in lineWithRadius)
                    entity2D.PlotStyle = arc.PlotStyle;


                radiusSuccess = true;
                return lineWithRadius;
            }

            radiusSuccess = false;
            return null;

        }

        private static IEnumerable<IEntity2D> CreateRadius(Line2D geometry, Line2D line, double radiusValue, out bool radiusSuccess)
        {
            var l2P1 = line.Start;
            var l2P2 = line.End;

            var l1A = GeometryHelper.GetAngle(geometry.End, geometry.Start);
            var l2A = GeometryHelper.GetAngle(l2P1, l2P2);

            var teoricPnt = geometry.End;

            //var angleResult = GetAverageAngle(l1A, l2A);

            /// codice preso da internet , non ci ho pensato su molto forse buggy
            double difference = l2A - l1A;
            while (difference < -Math.PI) difference += Math.PI * 2;
            while (difference > Math.PI) difference -= Math.PI * 2;

            var angleAtRadius = Math.PI - Math.Abs(difference);

            var distanceTrimmed = GetDistanceTrimmedByRadius(angleAtRadius / 2, radiusValue);

            var puntoFinaleLinea = GeometryHelper.GetPointAtDistance(geometry.Start, geometry.End, distanceTrimmed, false);

            var puntoFinaleRaggio = GeometryHelper.GetPointAtDistance(l2P1, l2P2, distanceTrimmed, true);

            if (puntoFinaleLinea == null || puntoFinaleRaggio == null)
            {
                radiusSuccess = false;
                return null;
            }

            var lineWithRadius = new List<IEntity2D>();

            geometry.End = puntoFinaleLinea;
            line.Start = puntoFinaleRaggio;

            var raccordo = new Arc2D { Start = new Point2D(puntoFinaleLinea), End = new Point2D(puntoFinaleRaggio), Radius = radiusValue };

            raccordo.Center = CalcCenterRadius(geometry.Start, teoricPnt, l2P2, radiusValue);


            raccordo.ClockWise = GeometryHelper.IsClockWise(puntoFinaleLinea, puntoFinaleRaggio, raccordo.Center);

            // raccordo.Center = RawArc2D.GetCenterFromTwoPointsAndRadius(geometry.Start, geometry.End,radiusValue, false);


            lineWithRadius.Add(geometry);
            lineWithRadius.Add(raccordo);

            foreach (var entity2D in lineWithRadius)
                entity2D.PlotStyle = geometry.PlotStyle;


            radiusSuccess = true;

            return lineWithRadius;


        }



        private static IEnumerable<IEntity2D> CreateChamfer(IEntity2D geometry, IEntity2D nextGeometry, double chamferValue, out bool chamferSuccess)
        {
            var line1 = geometry as Line2D;
            var arc1 = geometry as Arc2D;
            var line2 = nextGeometry as Line2D;
            var arc2 = nextGeometry as Arc2D;

            if (line1 != null && line2 != null)
                return CreateChamfer(line1, line2, chamferValue, out chamferSuccess);

            if (line1 != null && arc2 != null)
                return CreateChamfer(line1, arc2, chamferValue, out chamferSuccess);

            if (arc1 != null && line2 != null)
                return CreateChamfer(arc1, line2, chamferValue, out chamferSuccess);

            if (arc1 != null && arc2 != null)
                return CreateChamfer(arc1, arc2, chamferValue, out chamferSuccess);

            throw new NotImplementedException();
        }

        private static IEnumerable<IEntity2D> CreateChamfer(Line2D firstLine, Line2D secondLine, double chamferValue, out bool chamferSuccess)
        {
            var l2P1 = secondLine.Start;
            var l2P2 = secondLine.End;

            var puntoFinaleLinea = GeometryHelper.GetPointAtDistance(firstLine.Start, firstLine.End, chamferValue, false);

            var puntoFinaleSmusso = GeometryHelper.GetPointAtDistance(l2P1, l2P2, chamferValue, true);

            var lineChamfered = new List<IEntity2D>();

            if (puntoFinaleLinea == null || puntoFinaleSmusso == null)
            {
                chamferSuccess = false;
                return null;
            }

            firstLine.End = puntoFinaleLinea;
            secondLine.Start = puntoFinaleSmusso;

            var chamfer = new Line2D { Start = new Point2D(puntoFinaleLinea), End = new Point2D(puntoFinaleSmusso) };

            lineChamfered.Add(firstLine);
            lineChamfered.Add(chamfer);

            foreach (var entity2D in lineChamfered)
                entity2D.PlotStyle = firstLine.PlotStyle;

            chamferSuccess = true;
            return lineChamfered;
        }
        private static IEnumerable<IEntity2D> CreateChamfer(Arc2D arc, Arc2D arc2, double radiusValue, out bool radiusSuccess)
        {
            radiusSuccess = false;
            return null;

        }

        private static IEnumerable<IEntity2D> CreateChamfer(Arc2D arc, Line2D line, double radiusValue, out bool radiusSuccess)
        {
            var rsl = ChamferEntity(arc, line, radiusValue, false, out radiusSuccess);

            if (radiusSuccess)
            {
                return rsl;
            }

            return null;

        }

        private static IEnumerable<IEntity2D> CreateChamfer(Line2D line, Arc2D arc2D, double radiusValue, out bool radiusSuccess)
        {
            var rsl = ChamferEntity(arc2D, line, radiusValue, true, out radiusSuccess);

            if (radiusSuccess)
            {
                return rsl;
            }

            return null;

        }

        public static double GetAngleDifference(double angle1, double angle2)
        {
            var rslt1 = GeometryHelper.GetPositiveAngle(angle1);
            var rslt2 = GeometryHelper.GetPositiveAngle(angle2);

            if (rslt2 < rslt1)
                rslt2 += 2 * Math.PI;

            return rslt2 - rslt1;
        }

        private static Point2D CalcCenterRadius(Point2D p1, Point2D p2, Point2D p3, double radius)
        {
            // Angoli li misuro dal punto teorico all'altra estremità
            var angle1 = GeometryHelper.GetAngle(p2, p1);
            var angle2 = GeometryHelper.GetAngle(p2, p3);

            var difference = GetAngleDifference(angle1, angle2);

            var angleResult = GetAverageAngle(angle1, angle2);

            var angoloDelRaccordo = Math.PI - Math.Abs(difference);
            // ottengo la distanza fra il punto d'incrocio teorico e il centro del raccordo
            // come angolo mi serve meta angolo arco 
            var distance = 1 / (Math.Cos(angoloDelRaccordo / 2));

            distance *= radius;

            var deltaPnt = GetPointAtAngleAndDistance(Math.Abs(distance), angleResult);

            deltaPnt.X += p2.X;
            deltaPnt.Y += p2.Y;

            return deltaPnt;
        }

        private static Point2D GetInitChamferPointOnCfr(Arc2D cfrToChamfer, Point2D intersectPoint, double chamferValue , bool lineFirst)
        {
            // creo linea normale di lunghezza 1
            var normalAngle = GeometryHelper.getNormalAngle(cfrToChamfer, intersectPoint);

            var normalEndpoint = GeometryHelper.GetCoordinate(normalAngle, 1);
            normalEndpoint.X += intersectPoint.X;
            normalEndpoint.Y += intersectPoint.Y;

            // trovo intersezioni tra cerchio da smussare  e cerchio costruito sul punto di intersect
            Point2D p1 = null;
            Point2D p2 = null;

            var pFlag = GeometryHelper.FindCircleCircleIntersections(cfrToChamfer.Center.X, cfrToChamfer.Center.Y, cfrToChamfer.Radius, intersectPoint.X, intersectPoint.Y, chamferValue, out p1, out p2);

            if (pFlag == 2) // in questo metodo dovrebbe sempre restituire 2 punti ..
            {
                var d1 = GeometryHelper.Distance(normalEndpoint, p1);
                var d2 = GeometryHelper.Distance(normalEndpoint, p2);

                if (lineFirst)
                {
                    if (d1 < d2)
                        return p1;
                    return p2;
                }
                else
                {
                    if (d1 > d2)
                        return p1;
                    return p2;
                }

            }
            else
            {
                throw new Exception("RawProfile.GetInitChamferPointOnCfr");
            }
        }

        /// <summary>
        /// Smusso fra arco - linea 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="line2D"></param>
        /// <param name="chamferValue"></param>
        /// <param name="lineFirst"></param>
        /// <param name="chamferSuccess"></param>
        /// <returns></returns>
        public static IEnumerable<IEntity2D> ChamferEntity(Arc2D arc, Line2D line2D, double chamferValue, bool lineFirst, out bool chamferSuccess)
        {
            /*
             * Per trovare punto sulla cfr 
             * - creo cerchio nel punto incontro con raggio == valore dello smusso
             * - per linea faccio uguale metodo linea linea
             * 
             */

            Point2D intersectionPnt = null;

            if (lineFirst)
                intersectionPnt = line2D.End;
            else
            {
                intersectionPnt = arc.End;
            }

            //Point2D intersectionPnt = arc.Start;

            var startArcChamfer = GetInitChamferPointOnCfr(arc, intersectionPnt, chamferValue, lineFirst);

            var flagEnd = !lineFirst;

            var puntoLineaRslt = GeometryHelper.GetPointAtDistance(line2D.Start, line2D.End, chamferValue, flagEnd);

            if (puntoLineaRslt == null)
            {
                chamferSuccess = false;
                return null;
            }

            var lineChamfered = new List<IEntity2D>();

            if (lineFirst)
            {
                line2D.End = puntoLineaRslt;
                arc.Start = startArcChamfer;

                var chamfer = new Line2D { Start = new Point2D(puntoLineaRslt), End = new Point2D(startArcChamfer) };
                lineChamfered.Add(line2D);
                lineChamfered.Add(chamfer);

                foreach (var entity2D in lineChamfered)
                    entity2D.PlotStyle = line2D.PlotStyle;
            }
            else
            {
                line2D.Start = puntoLineaRslt;
                arc.End = startArcChamfer;

                var chamfer = new Line2D { Start = new Point2D(startArcChamfer), End = new Point2D(puntoLineaRslt) };
                lineChamfered.Add(arc);
                lineChamfered.Add(chamfer);

                foreach (var entity2D in lineChamfered)
                    entity2D.PlotStyle = arc.PlotStyle;

            }

            chamferSuccess = true;
            return lineChamfered;

        }


       


        ///// <summary>
        ///// Raccorda elementi 2D dove primo elemento è linea e secondo è arco.
        ///// </summary>
        ///// <param name="line"></param>
        ///// <param name="arc"></param>
        ///// <param name="radiusValue"></param>
        ///// <param name="radiusSuccess"></param>
        ///// <returns></returns>
        //private static IEnumerable<IEntity2D> CreateChamfer(Line2D line, Arc2D arc, double radiusValue, out bool radiusSuccess)
        //{
        //    Arc2D filletArc = null;

        //    var rsl = GeometryHelper.FilletEntity(arc, line, radiusValue, true, out filletArc);


        //    if (rsl)
        //    {
        //        var lineWithRadius = new List<IEntity2D>();

        //        arc.Start = filletArc.End;
        //        line.End = filletArc.Start;

        //        lineWithRadius.Add(line);
        //        lineWithRadius.Add(filletArc);

        //        foreach (var entity2D in lineWithRadius)
        //            entity2D.PlotStyle = line.PlotStyle;


        //        radiusSuccess = true;
        //        return lineWithRadius;
        //    }

        //    radiusSuccess = false;
        //    return null;

        //}

        /// <summary>
        /// Risolve il profilo, se ci sono problemi ritorna profilo fino al problema.
        /// Poi dice dove è errore, dove metto errore ? nel IRawMove2D ??
        /// </summary>
        /// <returns></returns>
        public Profile2D GetProfileResult(bool closed)
        {
            /*
             * closed indica se il profilo che serve è chiuso o meno..
             * 
             * è già settato con costruttore.
             * 
             */

            if (Count == 0) return null;

            var rslt = UpdateCycle();

            var profile = new Profile2D();

            foreach (var entity2D in rslt)
            {
                profile.AddEntity(entity2D);
            }

            return profile;
        }


        ///// <summary>
        ///// Metterlo su profile2d
        ///// </summary>
        ///// <returns></returns>
        //public bool IsSimplePolygon()
        //{
        //    //try
        //    //{
        //    //    var profile = GetProfileResult();

        //    //    var rdr = new WKTReader();

        //    //    var lines = new List<IGeometry>();

        //    //    var profileS = profile.Source;
        //    //    for (int i = 0; i < profile.Source.Count; i++)
        //    //    {
        //    //        if (profileS[i] is Line2D)
        //    //        {
        //    //            var line = profileS[i] as Line2D;

        //    //            if (line == null)
        //    //                throw new Exception();

        //    //            lines.Add(
        //    //                rdr.Read("LINESTRING (" + line.Start.X + " " + line.Start.Y + " , " + line.End.X + " " +
        //    //                         line.End.Y + " )"));
        //    //        }
        //    //    }

        //    //    var polygonizer = new Polygonizer();

        //    //    polygonizer.Add(lines);

        //    //    if (polygonizer.Polygons.Count == 0) return false;

        //    //    var p = (Polygon)polygonizer.Polygons[0];

        //    //    return p.IsSimple;

        //    //}
        //    //catch (Exception exception)
        //    //{
        //    return false;
        //    //}
        //}

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <returns></returns>
        public List<RawEntity2D> GetMoveList()
        {
            return this;
        }

        public void Syncronize(IEnumerable<RawEntity2D> moveList)
        {
            if (Count > 0)
                Clear();

            foreach (var rawEntity2D in moveList)
                Add(rawEntity2D);
        }


    }
}


