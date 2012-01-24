using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry.Entity;
using OffsetPath;

namespace CncConvProg.Geometry.Entity
{
    /// <summary>
    /// Sarebbe utile anche classe typo polilinea da usare a basso livello, sempre per gestire offset
    /// </summary>
    [Serializable]
    public class Profile2D
    {
        // todo : 

        /*
         * tipo quando inserisco punti e voglio profilo chiuso devo chieudere profilo
         */
        // devo mettere metodo , finalize o cose cosi nell'inserimento profilo

        private readonly List<IEntity2D> _profile = new List<IEntity2D>();

        /// Little little hack 
        public Point2D ToolPathStartPoint { get; set; }

        public List<IEntity2D> Source
        {
            get { return _profile; }
        }

        public void AddEntity(IEntity2D _entity2D)
        {
            if (_entity2D is Arc2D)
            {
                // aggiungo linea che va fino a inizio arco .
                var arc = _entity2D as Arc2D;

                AddPnt(arc.Start);
            }
            _profile.Add(_entity2D);
        }


        ///// <summary>
        ///// Metterlo su profile2d
        ///// </summary>
        ///// <returns></returns>
        //public bool IsSimplePolygon()
        //{
        //    try
        //    {
        //        var profile = GetProfile();

        //        var rdr = new WKTReader();

        //        var lines = new List<IGeometry>();

        //        var profileS = profile.Source;
        //        for (int i = 0; i < profile.Source.Count; i++)
        //        {
        //            if (profileS[i] is Line2D)
        //            {
        //                var line = profileS[i] as Line2D;

        //                if (line == null)
        //                    throw new Exception();

        //                lines.Add(
        //                    rdr.Read("LINESTRING (" + line.Start.X + " " + line.Start.Y + " , " + line.End.X + " " +
        //                             line.End.Y + " )"));
        //            }
        //        }

        //        var polygonizer = new Polygonizer();

        //        polygonizer.Add(lines);

        //        if (polygonizer.Polygons.Count == 0) return false;

        //        var p = (Polygon)polygonizer.Polygons[0];

        //        return p.IsSimple;

        //    }
        //    catch (Exception exception)
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// restituisce lista dei percorsi paralleli,
        ///// </summary>
        //public List<ContenitoreLivello> CalculateOffsetPath(double offsetDistance, int offsetRipet)
        //{
        //    var offsetList = new List<ContenitoreLivello>();

        //    var pntList = GetPointList();

        //    var n2 = new ClassN();
        //    n2.MathodA1(pntList, true);

        //    //if (this._polys.Count > 0) // dentro qui ci entro se ci sono altri polygoni
        //    //{
        //    //    PolygonalShape polygonalShape = this._polys[this._polys.Count - 1];
        //    //    if (polygonalShape.c(n2))  // se questi poligoni possono essere isole
        //    //    // per esserre isole devono essere state disegnate dopo anello esterno
        //    //    {
        //    //        polygonalShape.MethodA1(n2);
        //    //        base.Invalidate();
        //    //        return;
        //    //    }
        //    //}
        //    var polygonalShape = new PolygonalShape(n2);

        //    var chamferAngle = 0.0d;

        //    OffsetPathMainClass.Elaborate(new ClassC(polygonalShape), offsetDistance, offsetRipet, chamferAngle, offsetList);

        //    /*
        //     * da polygonal shape a classe utilizzabile nel processp
        //     */

        //    return offsetList;

        //}

        private static double GetIncrement(double maxDistance, double radius)
        {
            /*
             *  nello schema ho angolo isoscele = i lati uguali sono i raggi
             *  il lato differente è la distanza max che voglio ottenere,
             */
            var ab = PitagoraIpoNota(radius, maxDistance / 2);

            var angle = Math.Acos(ab / radius);

            return angle;
        }

        static double Pitagora(double l1, double l2)
        {
            return Math.Sqrt(Math.Pow(l1, 2) + Math.Pow(l2, 2));
        }

        static double PitagoraIpoNota(double ipo, double l1)
        {
            return Math.Sqrt(Math.Pow(ipo, 2) - Math.Pow(l1, 2));
        }

        /// <summary>
        /// Taglia il profilo, in questo caso è il percorso utensile , con un polygono ( stock )
        /// estende il risualtato per la lunghezza di sicurezza data
        /// </summary>
        /// <param name="singleToolPath"></param>
        /// <param name="stockPolygon"></param>
        /// <param name="millExtension"></param>
        /// <returns></returns>
        public static List<Profile2D> GetTrimmedByPolygon(Profile2D singleToolPath, Profile2D stockPolygon, double millExtension)
        {
            /* supposto che : sia toolpath profile che stockpolygon sono formati da linee.
             * il 
             * per ogni linea del profilo del toolpath , controllo :
             * 
             * casi :
             * 1) se non è contenuta .. skippo
             * 2) se è contenuta totalmente aggiungo 
             * 3) se contenuta parzialmente o un estremita si trova all'interno e l'altra risiede proprio sul profilo estendo questultima.
             * 
             * 4) se fa da parte a parte estendo tutte e 2 estremita..
             * 
             */
            return null;
        }



        /// <summary>
        /// Costruisce lista di punti 2D,
        /// archi li trasforma in sequenza di punti.
        /// </summary>
        private List<PuntoDueD> GetPointList(double offsetDistance = 10)
        {
            /*
             * provo a moltiplicare per 100000 sia xy e poi dividere alla fine..
             */
            var pntList = new List<PuntoDueD>();

            const float arcStartEndNormalLength = .000001f;

            for (int i = 0; i < Source.Count; i++)
            {
                //if (Source[i] is Arc2D && i == 0)
                //{
                //    var arc2D = Source[i] as Arc2D;
                //    if (arc2D != null)
                //    {
                //        //    pntList.Add(new PuntoDueD(line.Start.X, line.Start.Y));
                //        //    pntList.Add(new PuntoDueD(line.End.X, line.End.Y));

                //        pntList.Add(new PuntoDueD((float)arc2D.Start.X, (float)arc2D.Start.Y));
                //        //pntList.Add(new PuntoDueD((float)line.End.X, (float)line.End.Y));
                //    }
                //}

                // se il primo punto è linea..
                if (Source[i] is Line2D && i == 0)
                {
                    var line = Source[i] as Line2D;
                    if (line != null)
                    {
                        //    pntList.Add(new PuntoDueD(line.Start.X, line.Start.Y));
                        //    pntList.Add(new PuntoDueD(line.End.X, line.End.Y));

                        pntList.Add(new PuntoDueD(line.Start.X, line.Start.Y));
                        pntList.Add(new PuntoDueD(line.End.X, line.End.Y));
                    }
                }

                else if (Source[i] is Line2D && i > 0)
                {
                    var line = Source[i] as Line2D;
                    if (line != null)
                        pntList.Add(new PuntoDueD(line.End.X, line.End.Y));
                }

                    /*
                     */
                else if (Source[i] is Arc2D)
                {
                    //var factor = 10000;
                    var arc = Source[i] as Arc2D;

                    var center = arc.Center;

                    var startAngle = Math.Atan2(arc.Start.Y - center.Y, arc.Start.X - center.X);

                    var endAngle = Math.Atan2(arc.End.Y - center.Y, arc.End.X - center.X);

                    var a = GeometryHelper.RadianToDegree(endAngle);
                    var radius = arc.Radius;

                    // per vedere se questo è il problema agisco man
                    //var tollerance = 0.005;
                    // 

                    /*
                     * con offset maggiore ho bisogno di più risoluzione 
                     * vero , deve essere inversamente proporzionale a offset
                     */

                    var coeffOffset = 1 / (offsetDistance / 50);

                    // con raggio maggiore servono più punti e quindi e inver anche a raggio
                    var coeffRadius = 1 / (radius / 50);

                    // todo . fare proporzionato a raggio e offset
                    //var angleIncrement = (radius / 500) / offsetDistance;
                    var angleIncrement = .02;

                    ///*
                    // * più arco è grande più servono punti 
                    // */
                    //var angleIncrement = radius * tollerance;

                    //if (angleIncrement < .05)
                    //    angleIncrement = .05;

                    // alla lista di punti non aggiungo punto finale e punto iniziale ,
                    // creo un punto molto vicino - hack.
                    // evito cosi self-intersect profile 
                    // ma devo trovare il modo di immettere il punto finale 
                    // in figura a falce , ci sarebbe un risultato scorretto..

                    //pntList.Add(new PuntoDueD(arc.Start.X, arc.Start.Y));

                    if (arc.ClockWise)
                    {
                        /* 
                         * Se il senso è antiorario l'angolo finale dovra essere maggiore
                         */

                        if (startAngle < 0)
                        {
                            startAngle += Math.PI * 2;
                            endAngle += Math.PI * 2;
                        }

                        if (endAngle >= startAngle)
                            endAngle -= 2 * Math.PI;

                        var deltaAngle = endAngle - startAngle;

                        if (deltaAngle == 0)
                        {
                            /* è un cerchio completo*/
                            endAngle -= Math.PI * 2;
                            deltaAngle = endAngle - startAngle;

                        }
                        //var deltaAngle = endAngle - startAngle;

                        //if (deltaAngle == 0)
                        //    throw new Exception();

                        // Se è clocwise angolo diminuisce 
                        //startAngle -= angleIncrement;

                        // angolo partenza deve essere incrementato perchè il punto iniziale è già stato immesso 
                        // (( punto finale linea precedente ))



                        var primoP = true;

                        for (var j = startAngle; j > endAngle; )
                        {
                            var x = (Math.Cos(j) * radius) + center.X;
                            var y = (Math.Sin(j) * radius) + center.Y;

                            pntList.Add(new PuntoDueD(x, y));

                            if (primoP)
                            {
                                // aggiungo punto parellelo

                                var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(x, y));

                                var p2 = GeometryHelper.GetCoordinate(normalAngle, arcStartEndNormalLength, new Point2D(x, y));

                                pntList.Add(new PuntoDueD(p2.X, p2.Y));

                                primoP = false;
                            }

                            j -= angleIncrement;

                            if (j < endAngle)
                                j = endAngle;
                        }


                        //{

                        //    var x = (Math.Cos(endAngle - .0001) * radius) + center.X;
                        //    var y = (Math.Sin(endAngle - .0001) * radius) + center.Y;

                        //    pntList.Add(new PuntoDueD(x, y));

                        //}
                    }
                    else // Arco antiorario
                    {
                        /*
                         * Se arco è ccw , angolo è maggiore ,
                         * 
                         * si parte da angolo iniziale e si aumenta fino a angolo finale
                         * 
                         */

                        if (startAngle < 0)
                        {
                            startAngle += Math.PI * 2;
                            endAngle += Math.PI * 2;
                        }

                        /* 
                         * Se arco antiorario angolo finale dovra essere maggiore 
                         */

                        if (endAngle < startAngle)
                            endAngle += 2 * Math.PI;

                        var deltaAngle = endAngle - startAngle;

                        if (deltaAngle == 0)
                        {
                            /* è un cerchio completo*/
                            /* */

                            endAngle += Math.PI * 2;
                            deltaAngle = endAngle - startAngle;

                        }

                        //var deltaAngle = endAngle - startAngle;

                        //if (deltaAngle == 0)
                        //    throw new Exception();

                        // ho provato a levare angleIncrement..
                        //startAngle += angleIncrement;


                        /*
                         * Allora per risolvere il problema degli archi , ( oltre alla soluzione di fare primo e ultimo angolo compensatore,)
                         * 
                         * Provo ad aggiungere 2 punti in modo che il primo e ultimo segmento dell'arco siano delle "normali" all'arco.
                         * Cosi algoritmo per offset dovrebbe funzionare correttamente.
                         * 
                         * Quindi devo inserire punto iniziale ( con angolo 0).
                         * Il punto normale a questo.
                         * 
                         * ilPunto normale al punto finale 
                         * e il punto finale 
                         * 
                         * In questo modo l'algoritmo dovrebbe comportrsi correttamente
                         * 
                         * punto iniziale  
                         * 
                         * per inserire il punto finale mi basta inserire al penultimo posto il punto normale..
                         * 
                         */

                        /*
                         * Con ciclo for , viene inserito il primo punto , poi inserisco normale secondo punto.
                         * 
                         * Questo ciclo "non" dovrebbe inserire punto per angolo finale. Si dovrebbe interrompere prima
                         * . Incrementa variabile
                         * . Controlla Variabile , se non passa controllo si interrompe , quindi si ferma 
                         * 
                         * sembra +/- funzionare 
                         * 
                         * pero l'arco estratto o faccio una media.. che mi sa che basti oppure 
                         */
                        var normalFirstPoint = true;

                        for (var j = startAngle; j < endAngle - angleIncrement / 2; j += angleIncrement)
                        {
                            var x = (Math.Cos(j) * radius) + center.X;
                            var y = (Math.Sin(j) * radius) + center.Y;

                            pntList.Add(new PuntoDueD(x, y)); // riporto a double

                            if (normalFirstPoint)
                            {
                                // aggiungo punto parellelo

                                var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(x, y));

                                var p2 = GeometryHelper.GetCoordinate(normalAngle, arcStartEndNormalLength, new Point2D(x, y));

                                pntList.Add(new PuntoDueD(p2.X, p2.Y));

                                normalFirstPoint = false;
                            }
                        }

                        /*
                         * si potrebbe creare bug se questo punto crea si interseca con punto precedente 
                         * 
                         */
                        // qui inserisco il punto normale a ultimo punto
                        var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

                        normalAngleEndPnt -= Math.PI;

                        var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, arc.End);

                        pntList.Add(new PuntoDueD(normalEndPnt.X, normalEndPnt.Y));

                    }

                    // In ultimo aggiungo endpoint
                    pntList.Add(new PuntoDueD(arc.End.X, arc.End.Y));

                }




            }

            return pntList;
        }

        private const double ArcParsingCenterTollerance = 0.1d;
        private const double ArcParsingDistanceTollerance = 0.1d;

        private Point2D _startPoint = null;

        /// <summary>
        /// Metodo per aggiungere punto al profilo,
        /// creera linea con ultimo punto del profilo.
        /// </summary>
        /// <param name="pnt"></param>
        public void AddPnt(Point2D pnt)
        {
            if (_startPoint == null)
            {
                _startPoint = new Point2D(pnt);
                return;
            }

            var lastPnt = GetLastPoint();

            if (lastPnt == null)
                throw new NullReferenceException();

            var line = new Line2D { Start = new Point2D(lastPnt), End = new Point2D(pnt) };

            AddEntity(line);
        }

        public void AddPnt(Point2D pnt, Matrix3D multiplyMatrix)
        {
            var rotatedPnt = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(pnt.X, pnt.Y, 0), multiplyMatrix);

            AddPnt(new Point2D(rotatedPnt.X, rotatedPnt.Y));
        }

        private Point2D GetLastPoint()
        {
            var lastElement = _profile.LastOrDefault();

            return lastElement != null ? lastElement.GetLastPnt() : new Point2D(_startPoint);
        }

        /// <summary>
        /// Classe per trasformare sequenza di punti in arco.
        /// Non devo più considerari i punti . ma devo analizzare il punto centrake dei vari segmenti
        /// </summary>
        private class ParsedArc
        {
            /// <summary>
            /// Ottiene il raggio dell'arco ,dopo che ho trovato un segmento valido
            /// </summary>
            /// <param name="segmentParsed">Segmento che risiede su CFR</param>
            /// <param name="centerArc">Centro Arco.</param>
            /// <returns></returns>
            private double GetRadius(Line2D segmentParsed, Point2D centerArc)
            {
                /*
                 * Prima la misura del raggio la misurava erroneamente tra centroArco e punto trovato.
                 * 
                 * La distanza che devo misurare e centro segmento e centro Arco
                 * Per fare questo mi basta pitagora.
                 * Dove Ipotenusa == Centro <> PuntoEstremoSegmento 
                 * Cateto == lunghezzaSegmento /2
                 */

                var ipo = GeometryHelper.Distance(segmentParsed.Start, centerArc);

                var cateto = segmentParsed.GetLenght() / 2;

                if (cateto == 0 || ipo == null) return 0;

                var radius = Math.Sqrt(Math.Pow(ipo.Value, 2) - Math.Pow(cateto, 2));

                return radius;

            }
            private readonly double _arcTollerance;
            private readonly double _distanceTollerance;

            public ParsedArc(double arcTollerance, double distanceTollerance)
            {
                _arcTollerance = arcTollerance;
                _distanceTollerance = distanceTollerance;
            }

            /*
             * aux class
             * per creare parsedArc.
             * 
             * 
             * prende in ingresso punti
             * 
             * mi dice se ok , se lo mangia. se no lo sputo 
             * 
             * quando sputa arco  è finito.
             * 
             * deve avere almeno 3 punti ok
             * 
             */


            //public void ShutDown()
            //{

            //}

            public bool IsArcStarted { get; set; }

            private List<Point2D> bufferPnt = new List<Point2D>();

            public double? Radius { get; set; }

            public Point2D Center { get; set; }

            //public bool TryStartArcPrev(Point2D p1, Point2D p2, Point2D p3, Point2D p4, ref List<Point2D> externBufferPnt)
            //{

            //    if (IsOk(p1, p2, p3, p4))
            //    {

            //        bufferPnt.Add(p1);
            //        bufferPnt.Add(p2);
            //        bufferPnt.Add(p3);
            //        bufferPnt.Add(p4);

            //        externBufferPnt.Clear();

            //        /*
            //         * setto centro arco e raggio trovato
            //         * 
            //         * se aggiungo punti e non sono compatibili con questo arco e raggio li scarico.
            //         */
            //        Center = GeometryHelper.Circocentro(p1, p2, p3);

            //        Radius = GeometryHelper.Distance(Center, p1);

            //        IsArcStarted = true;

            //        return true;
            //    }

            //    return false;
            //}


            public bool TryStartArc(Point2D p1, Point2D p2, Point2D p3, Point2D p4, Point2D newPoint, ref List<Point2D> externBufferPnt)
            {

                if (IsOk(p1, p2, p3, p4, newPoint))
                {

                    bufferPnt.Add(p1);
                    bufferPnt.Add(p2);
                    bufferPnt.Add(p3);
                    bufferPnt.Add(p4);
                    bufferPnt.Add(newPoint);

                    externBufferPnt.Clear();

                    /*
                     * setto centro arco e raggio trovato
                     * 
                     * se aggiungo punti e non sono compatibili con questo arco e raggio li scarico.
                     */

                    var midPoint1 = GeometryHelper.GetMidPoint(p1, p2);
                    var midPoint2 = GeometryHelper.GetMidPoint(p2, p3);
                    var midPoint3 = GeometryHelper.GetMidPoint(p3, p4);

                    Center = GeometryHelper.Circocentro(midPoint1, midPoint2, midPoint3);

                    Radius = GetRadius(new Line2D() { Start = p1, End = p2 }, Center);

                    IsArcStarted = true;

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Controllo che i 4 punti forniti possano creare arco :
            ///  Seguo iter seguente:
            ///  I 4 punti risiedono sulla stessa cfr ?
            ///     Prendo il raggio tra il primo e centro e in proporzione a questo raggio creo un valore di tolleranza
            /// </summary>
            /// <param name="p1"></param>
            /// <param name="p2"></param>
            /// <param name="p3"></param>
            /// <param name="p4"></param>
            /// <returns></returns>
            private bool IsOk(Point2D p1, Point2D p2, Point2D p3, Point2D p4, Point2D newPoint)
            {
                /*
                 * prima dovrei vedere se sono sullo stesso arco , 
                 * 
                 * qui posso gia prendere raggio presunto.
                 * 
                 * e creare un valore di comparazione rapportato alle dimensioni del raggio.
                 * 
                 * , in secondo ambito controllo che la loro angolo sia vicino 
                 * 
                 * e terzo devono essere in ordine.
                 * 
                 * a partire dal primo l'angolo creato dal punto deve essere compatibile.
                 */
                // Devo creare archi in modo che da un punto e l'altro ci sia distanze definita

                Point2D presuntCenterPoint;
                double presuntArc;

                if (GeometryHelper.IsInArc(p1, p2, p3, p4, newPoint, out presuntCenterPoint, out presuntArc))
                {
                    /*
                     * a scopo istruttivo provo a metter e 5° deg == 
                     */
                    if (GeometryHelper.SonoVicini(p1, p2, p3, p4, presuntCenterPoint, presuntArc))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Mi dice se i punti seguono la stessa direzione
            /// </summary>
            /// <param name="p1"></param>
            /// <param name="p2"></param>
            /// <param name="p3"></param>
            /// <param name="p4"></param>
            /// <returns></returns>
            private static bool IsPointListOrder(Point2D p1, Point2D p2, Point2D p3, Point2D p4)
            {
                //var angle1 = GeometryHelper.GetPositiveAngle(p1.X, p1.Y);
                //var angle2 = GeometryHelper.GetPositiveAngle(p2.X, p2.Y);
                //var angle3 = GeometryHelper.GetPositiveAngle(p3.X, p3.Y);
                //var angle4 = GeometryHelper.GetPositiveAngle(p4.X, p4.Y);

                //if (angle1 < angle2)
                //{
                //    if (angle2 < angle3 && angle3 < angle4)
                //        return true;

                //    return false;
                //}
                //else if (angle1 > angle2)
                //{
                //    if (angle2 > angle3 && angle3 > angle4)
                //        return true;
                //    return false;
                //}

                //// a1 == a2
                //return false;
                return true;

            }

            public bool EatPoint(Point2D point2D)
            {
                if (IsOk(point2D))
                {
                    bufferPnt.Add(point2D);

                    return true;
                }

                return false;
            }
            /*
             * se arriva qui l'arco deve essere gia cominciato,
             * quindi conterra raggio e centro.
             */
            //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

            // * 
            //if (bufferPnt.Count < 3)
            //    throw new Exception("ParsedArc");

            //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

            //return IsOk(last3[0], last3[1], last3[2], point2D);
            // */

            //// Devo creare archi in modo che da un punto e l'altro ci sia distanze definita
            //if (GeometryHelper.SonoVicini(last3[0], last3[1], last3[2], point2D, _distanceTollerance))
            //{
            //    //if (GeometryHelper.IsInArcV_2(last3[0], last3[1], last3[2], point2D, Center, Radius.Value, _arcTollerance))
            //    if (GeometryHelper.IsInArc(last3[0], last3[1], last3[2], point2D, _arcTollerance))
            //    {
            //        bufferPnt.Add(point2D);

            //        return true;
            //    }
            //}
            //return false;



            public Arc2D CreateArc()
            {
                /* controllo che sia possibile creare arco.*/

                // questo metodo viene chiamato su chiusura arco.
                // setto arco chiuso
                IsArcStarted = false;

                if (bufferPnt.Count < 4)
                    throw new Exception("ParsedArc");

                var secondPoint = bufferPnt[1];
                var penultimoPoint = bufferPnt[bufferPnt.Count - 2];

                var firstPnt = bufferPnt.FirstOrDefault();
                var lastPnt = bufferPnt.LastOrDefault();

                /*
                 * considerare anche di fare la media del buffer..
                 */
                //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

                /*
                 * Un 'altro problema che si presenta è in caso ci sia cfr.
                 * 
                 * qui i punti iniziale  e finale coincidono,
                 * 
                 * todo . tenere conto anche delle spirali, permettere massimo 1 giro 
                 * 
                 * per ovviare a questo problema provero a prendere il secondo , meta e penultimo
                 * cosi i punti estremi non coincideranno
                 * 03/08/2011
                 * se faccio cosi su arco di 4 punti il midPoint e penultimo point coincidono
                 */
                var midPoint = bufferPnt[(int)bufferPnt.Count / 2];

                var circoCentro = GeometryHelper.Circocentro(secondPoint, midPoint, lastPnt);

                var raggio = GetRadius(new Line2D() { Start = secondPoint, End = lastPnt }, circoCentro);

                if (raggio == 0)
                    return null;

                var arc = new Arc2D
                {
                    Center = new Point2D(circoCentro),
                    Start = new Point2D(firstPnt),
                    End = new Point2D(lastPnt),
                    Radius = raggio,
                };

                arc.ClockWise = IsClockWise(circoCentro);

                return arc;
            }

            private bool IsClockWise(Point2D center)
            {
                /*
                 * Guardo come incrementano i punti : 
                 * se angolo cresce >> antiorario
                 * se angolo diminuisce << orario
                 */
                if (bufferPnt.Count < 4)
                    throw new Exception("ParsedArc");

                var p1 = bufferPnt[1];
                var p2 = bufferPnt[3];

                /*
                 * Ho noto punto iniziale . p finale e centro e i vari punti.
                 * 
                 * devo determinare se è cw o ccw.
                 * 
                 * Prendo 2 punti consecutivi.
                 * - se angolo cresce è ccw
                 * - se angolo diminuisce è cw
                 * 
                 */
                var angle1 = Math.Atan2(p1.Y - center.Y, p1.X - center.X);
                var angle2 = Math.Atan2(p2.Y - center.Y, p2.X - center.X);

                //if (angle1 < 0)
                //{
                //    angle1 += Math.PI * 2;
                //    angle2 += Math.PI * 2;
                //}
                //   angle1 = GeometryHelper.GetPositiveAngle(p1.X - center.X, p1.Y - center.Y);
                // angle2 = GeometryHelper.GetPositiveAngle(p2.X - center.X, p2.Y - center.Y);

                var dAngle = angle1 - angle2;

                if (dAngle < 0)
                    return false;

                return true;
            }


            private bool IsOk(Point2D point2D)
            {
                /*
                 * prendo ultimi 3 elementi dal buffer e faccio prova con questo
                 */

                if (bufferPnt.Count < 4)
                    throw new Exception("ParsedArc");

                var last4 = bufferPnt.GetRange(bufferPnt.Count - 4, 4);

                return IsOk(last4[0], last4[1], last4[2], last4[3], point2D);
            }

            ////public double GetRadius()
            ////{
            ////    return 9;
            ////}

            ////public Point2D GetCentre()
            ////{
            ////    return new Point2D();
            ////}

            ////public Point2D GetStart()
            ////{
            ////    return new Point2D();
            ////}

            //public Point2D GetEnd()
            //{
            //    return new Point2D();
            //}


        }



        /// <summary>
        /// -Trasformo il profilo in lista di punti
        /// 
        /// -- il problema è che non considero raggi descritti con meno di 4 punti.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arcTollerance"></param>
        /// <param name="distanceTollerance"></param>
        /// <returns></returns>
        public static Profile2D ParseArcIn2DProfile(Profile2D source, double arcTollerance = ArcParsingCenterTollerance, double distanceTollerance = ArcParsingDistanceTollerance)
        {
            var pntList = source.GetPointListP2();

            if (pntList.Count <= 5)
                return source;

            var profile = new Profile2D();

            var bufferPnt = new List<Point2D>();

            //var arcStarted = false;

            //Point2D initPoint = null;

            for (int i = 0; i < pntList.Count; i++)
            {
                var point = pntList[i];

                bufferPnt.Add(point);

                if (bufferPnt.Count >= 5)
                {
                    var lastFourElement = bufferPnt.GetRange(bufferPnt.Count - 5,5 );

                    var parsedArc = new ParsedArc(arcTollerance, distanceTollerance);

                    if (parsedArc.TryStartArc(lastFourElement[0], lastFourElement[1], lastFourElement[2],
                                              lastFourElement[3],lastFourElement[4], ref bufferPnt))
                    {


                        i++;
                        for (; i < pntList.Count; i++)
                        {
                            /*
                             * proseguo fino a interrompere ciclo
                             */

                            var newPnt = pntList[i];

                            if (!parsedArc.EatPoint(newPnt))
                            {
                                // l'ultimo punto gli è stato indigesto e devo terminare arco

                                profile.AddEntity(parsedArc.CreateArc());

                                // bufferPnt.Clear();

                                // l'ultimo punto 
                                bufferPnt.Add(newPnt);

                                break; // voglio che esca da questo ultimo ciclo for.
                            }
                        }
                        /*
                         * Se il profilo è un cerchio si "mangia" tutti i punti. 
                         * Quindi se arrivo qui che il cerchio è ancora aperto, lo chiudo..
                         */
                        if (parsedArc.IsArcStarted)
                            profile.AddEntity(parsedArc.CreateArc());

                    }

                    else
                    {
                        // se arriva qui il metodo TryStartArc ha restituito false

                        profile.AddPnt(lastFourElement[0]);

                        bufferPnt.Remove(lastFourElement[0]);
                    }
                }
            }

            // possono esserci ancora 3 elementi nel buffer
            // e non puo essere arco, sarebbe continuato grazie a parsedArc che prende punti in continui

            while (bufferPnt.Count > 0)
            {
                profile.AddPnt(bufferPnt.First());
                bufferPnt.Remove(bufferPnt.First());
            }

            // setto il punto di inizio profilo

            profile.ToolPathStartPoint = source.ToolPathStartPoint;
            return profile;
        }


        //                    if (!arcStarted)
        //                    {
        //                        arcStarted = true;
        //                        initPoint = lastFourElement[0];
        //                    }



        //                    continue;
        //                }
        //            }

        //            else
        //            {
        //                // Se arrivo qui vuol dire che i punti trovati non possono formare arco

        //                /* Caso 1  . Arco iniziato . 
        //                 * Terminare e inserire arco trovato nel profile .
        //                 * e inserire ultimo punto ( quello che ha fatto terminare l'arco)
        //                 */
        //                if (arcStarted)
        //                {
        //                    arcStarted = false;

        //                    var lastPoint = lastFourElement[2]; // ultimo punto valido arco

        //                    var circoCentro = GeometryHelper.Circocentro(lastFourElement[0], lastFourElement[1], lastFourElement[2]);

        //                    var raggio = GeometryHelper.Distance(circoCentro, lastPoint);

        //                    var arc = new Arc2D()
        //                    {
        //                        Center = new Point2D(circoCentro.X, circoCentro.Y),
        //                        Start = new Point2D(initPoint.X, initPoint.Y),
        //                        End = new Point2D(lastPoint.X, lastPoint.Y),
        //                        Radius = Math.Round(raggio, 3),
        //                    };

        //                    // settare clockwise oppure no..
        //                    profile.AddEntity(arc);

        //                    /*
        //                     * qui non aggiungo ultimo punto ( quello che ha fatto terminare arco )
        //                     * 
        //                     * perchè potrebbe essere parte di un'altro arco 
        //                     * 
        //                     * Caso -> 2 archi consecutivi
        //                     */

        //                    // Lascio nel buffer ultimo punto perchè potrebbe essere parte di un'eventuale altro raggio
        //                    bufferPnt.RemoveRange(0, 3);
        //                }

        //                // Caso 2 . Arco non iniziato 
        //                else
        //                {
        //                    profile.AddPnt(lastFourElement[0]);
        //                    bufferPnt.RemoveRange(0, 1);
        //                }

        //            }

        //    }

        //    return profile;
        //}
        //}

        //public static Profile2D ParseArcIn2DProfile(Profile2D source, double arcTollerance = ArcParsingCenterTollerance, double distanceTollerance = ArcParsingDistanceTollerance)
        //{
        //    var profile = new Profile2D();
        //    var pntList = source.GetPointListP2();

        //    var bufferPnt = new List<Point2D>();

        //    var arcStarted = false;

        //    Point2D initPoint = null;


        //    foreach (var point in pntList)
        //    {
        //        bufferPnt.Add(point);

        //        if (bufferPnt.Count >= 4)
        //        {
        //            var lastFourElement = bufferPnt.GetRange(bufferPnt.Count - 4, 4);

        //            // Controllo che siano punti vicini 
        //            if (GeometryHelper.SonoVicini(lastFourElement[0], lastFourElement[1], lastFourElement[2], lastFourElement[3], distanceTollerance))
        //            {
        //                // Controllo il circoncentro
        //                if (GeometryHelper.IsInArc(lastFourElement[0], lastFourElement[1], lastFourElement[2], lastFourElement[3], arcTollerance))
        //                {
        //                    /* 
        //                     * Se arrivo qui vuol dire che i punti trovati 
        //                     * possono formare un arco
        //                     */

        //                    // se l'arco non era cominciato

        //                    if (!arcStarted)
        //                    {
        //                        arcStarted = true;
        //                        initPoint = lastFourElement[0];
        //                    }
        //                    continue;
        //                }
        //            }

        //            else
        //            {
        //                // Se arrivo qui vuol dire che i punti trovati non possono formare arco

        //                /* Caso 1  . Arco iniziato . 
        //                 * Terminare e inserire arco trovato nel profile .
        //                 * e inserire ultimo punto ( quello che ha fatto terminare l'arco)
        //                 */
        //                if (arcStarted)
        //                {
        //                    arcStarted = false;

        //                    var lastPoint = lastFourElement[2]; // ultimo punto valido arco

        //                    var circoCentro = GeometryHelper.Circocentro(lastFourElement[0], lastFourElement[1], lastFourElement[2]);

        //                    var raggio = GeometryHelper.Distance(circoCentro, lastPoint);

        //                    var arc = new Arc2D()
        //                    {
        //                        Center = new Point2D(circoCentro.X, circoCentro.Y),
        //                        Start = new Point2D(initPoint.X, initPoint.Y),
        //                        End = new Point2D(lastPoint.X, lastPoint.Y),
        //                        Radius = Math.Round(raggio, 3),
        //                    };

        //                    // settare clockwise oppure no..
        //                    profile.AddEntity(arc);

        //                    /*
        //                     * qui non aggiungo ultimo punto ( quello che ha fatto terminare arco )
        //                     * 
        //                     * perchè potrebbe essere parte di un'altro arco 
        //                     * 
        //                     * Caso -> 2 archi consecutivi
        //                     */

        //                    // Lascio nel buffer ultimo punto perchè potrebbe essere parte di un'eventuale altro raggio
        //                    bufferPnt.RemoveRange(0, 3);
        //                }

        //                // Caso 2 . Arco non iniziato 
        //                else
        //                {
        //                    profile.AddPnt(lastFourElement[0]);
        //                    bufferPnt.RemoveRange(0, 1);
        //                }

        //            }

        //        }
        //    }


        //    return profile;
        //}

        public List<Point2D> GetPointListP2()
        {
            /*
             * richiamato da metodo del parse arc, in questo ambito il profilo è composto 
             * solamente da linee essendo il risultato dell'offset
             */
            var p = GetPointList();

            var rslt = new List<Point2D>();

            foreach (var puntoDueD in p)
            {
                rslt.Add(new Point2D(puntoDueD.X, puntoDueD.Y));
            }

            return rslt;
        }

        //public static double GetPositiveAngle(double radian)
        //{
        //    if (radian < 0)
        //        radian = (Math.PI * 2) - Math.Abs(radian);

        //    return radian;
        //}

        //public static double GetPositiveAngle(double x, double y)
        //{
        //    var rslt = Math.Atan2(y, x);

        //    if (rslt < 0)
        //        rslt = (Math.PI * 2) - Math.Abs(rslt);

        //    return rslt;
        //}

        //public List<IEntity2D> p(double distance)
        //{

        //    var polygonizer = new Polygonizer();

        //    var rdr = new WKTReader();

        //    var list = new List<IGeometry>();

        //    foreach (var entity2D in _profile)
        //    {
        //        if (entity2D is Line2D)
        //        {
        //            var lineSource = entity2D as Line2D;
        //            list.Add(rdr.Read("LINESTRING (" + lineSource.Start.X + " "
        //                                             + lineSource.Start.Y + " , "
        //                                             + lineSource.End.X + " "
        //                                             + lineSource.End.Y + ")"));


        //        }

        //    }

        //    polygonizer.Add(list); // todo _ su errore di profilo scatena inferno..

        //    var polys = polygonizer.Polygons;

        //    var poly = (Polygon)polys[0];

        //    var polyOffset = poly.Buffer(distance);//, BufferStyle.CapSquare); reference at source

        //    var o = new List<IEntity2D>();
        //    for (int i = 0; i < polyOffset.Coordinates.Length - 1; i++)
        //    {
        //        var l1 = polyOffset.Coordinates[i];
        //        var l2 = polyOffset.Coordinates[i + 1];
        //        o.Add(new Line2D
        //                  {
        //                      Start = new Point2D { X = l1.X, Y = l1.Y },
        //                      End = new Point2D { X = l2.X, Y = l2.Y }
        //                  });

        //    }

        //    return o;



        //}


        public List<Profile2D> Offset(double offsetDistance, bool counterClockwise)
        {
            var rslt = new List<ContenitoreLivello>();

            // int molFact = 1;

            //offsetDistance = offsetDistance * molFact;
            var pntList = GetPointList(offsetDistance);

            //foreach (var puntoDueD in pntList)
            //{
            //    puntoDueD.X = puntoDueD.X * molFact;
            //    puntoDueD.Y = puntoDueD.Y * molFact;
            //}

            int num3 = 0;

            if (pntList.Count >= 3)
            {
                ClassO classO = new ClassO();
                for (int j = 0; j < pntList.Count; j++)
                {
                    classO.Add(new ForseLineaClassA(pntList[j], pntList[(j + 1) % pntList.Count]));
                }
                List<PuntoDueD> list6 = new List<PuntoDueD>();
                List<int> list7 = new List<int>();
                List<int> list8 = new List<int>();
                classO.MethodA(list6, list7, list8);
                num3 += list6.Count;
            }

            bool flag2 = false;
            if ((num3 > 0))
            {
                flag2 = true;
                /* devo resituire che eccezzione di profilo autointersecante */
                return null;

            }

            var n2 = new ClassN();

            n2.MathodA1(pntList, false); //

            // todo : 
            //if (this._polys.Count > 0) // dentro qui ci entro se ci sono altri polygoni
            //{
            //    PolygonalShape polygonalShape = this._polys[this._polys.Count - 1];
            //    if (polygonalShape.c(n2))  // se questi poligoni possono essere isole
            //    // per esserre isole devono essere state disegnate dopo anello esterno
            //    {
            //        polygonalShape.MethodA1(n2);
            //        base.Invalidate();
            //        return;
            //    }
            //}
            var polygonalShape1 = new PolygonalShape(n2);

            var chamferAngle = Math.PI / 4;
            //  var chamferAngle = 0;

            OffsetPathMainClass.Elaborate(new ClassC(polygonalShape1), offsetDistance, 1, chamferAngle, rslt);

            /*
             * da polygonal shape a classe utilizzabile nel processp
             */


            var offsetResultPointProfile = new List<List<Point2D>>();



            /*
             * Gli ho chiesto 1 offset, se mi restituisce un numero diverso da 0 o 1 , probabilmente c'è errore
             */
            Debug.Assert(rslt.Count <= 1);

            if (rslt.Count == 0)
                return null;

            for (int i = 0; i < rslt.Count; i++)
            {
                var contenitoreLivello = rslt[i];

                /*
                 * Possono esserci più profili per lo stesso livello di offset,
                 * li devo tenere separati, 
                 */

                for (int m = 0; m < contenitoreLivello.Count; m++)
                {
                    var pntListRslt = new List<Point2D>();

                    var polygonalShape = contenitoreLivello[m];

                    var a0 = polygonalShape.GetExternPolygon();

                    for (int k = 0; k < a0.ac().Count; k++)
                    {
                        if (a0.ac()[k] is ForseLineaClassA)
                        {
                            var classA0 = a0.ac()[k] as ForseLineaClassA;


                            var puntoDueD = classA0.h();
                            var x2 = classA0.i();

                            if (k == 0)
                                pntListRslt.Add(new Point2D(puntoDueD.X, puntoDueD.Y));

                            pntListRslt.Add(new Point2D(x2.X, x2.Y));

                        }
                        else if (a0.ac()[i] is ForseArco2ClassQ) // forse classq è la chiave
                        {
                            // this.MethodA(A_0.ac()[i] as ClassQ, A_1, A_2);
                        }
                    }

                    offsetResultPointProfile.Add(pntListRslt);
                }
            }


            var profileResulList = new List<Profile2D>();


            foreach (var singleProfilePoints in offsetResultPointProfile)
            {
                //foreach (var singleProfilePoint in singleProfilePoints)
                //{
                //    singleProfilePoint.X = singleProfilePoint.X / molFact;
                //    singleProfilePoint.Y = singleProfilePoint.Y / molFact;
                //}

                var list = new Profile2D();

                if (counterClockwise)
                {
                    for (int i = singleProfilePoints.Count - 1; i >= 0; i--)
                    {
                        list.AddPnt(singleProfilePoints[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < singleProfilePoints.Count; i++)
                    {
                        list.AddPnt(singleProfilePoints[i]);
                    }
                }

                profileResulList.Add(list);

            }


            return profileResulList;
        }


        public static Profile2D CreateCircle(double radius, Point2D center)
        {
            var arc = new Arc2D { Radius = radius, Center = center };

            arc.Start = new Point2D(arc.Center);

            arc.Start.X += arc.Radius;

            arc.End = new Point2D(arc.Center);

            arc.End.X += arc.Radius;

            var profile = new Profile2D();

            profile.AddEntity(arc);

            return profile;
        }


        public void SetAllToElement()
        {
            foreach (var entity2D in _profile)
            {
                entity2D.PlotStyle = EnumPlotStyle.Element;
            }
        }

        public void Multiply(Matrix3D matrix)
        {
            foreach (var entity2D in Source)
            {
                entity2D.Multiply(matrix);
            }
        }
    }
}
//19/08/2011 metodo getpoinlist prima delle modifiche 
///// <summary>
//       /// Costruisce lista di punti 2D,
//       /// archi li trasforma in sequenza di punti.
//       /// </summary>
//       private List<PuntoDueD> GetPointList(double offsetDistance = 10)
//       {
//           /*
//            * provo a moltiplicare per 100000 sia xy e poi dividere alla fine..
//            */
//           var pntList = new List<PuntoDueD>();

//           for (int i = 0; i < Source.Count; i++)
//           {
//               //if (Source[i] is Arc2D && i == 0)
//               //{
//               //    var arc2D = Source[i] as Arc2D;
//               //    if (arc2D != null)
//               //    {
//               //        //    pntList.Add(new PuntoDueD(line.Start.X, line.Start.Y));
//               //        //    pntList.Add(new PuntoDueD(line.End.X, line.End.Y));

//               //        pntList.Add(new PuntoDueD((float)arc2D.Start.X, (float)arc2D.Start.Y));
//               //        //pntList.Add(new PuntoDueD((float)line.End.X, (float)line.End.Y));
//               //    }
//               //}

//               // se il primo punto è linea..
//               if (Source[i] is Line2D && i == 0)
//               {
//                   var line = Source[i] as Line2D;
//                   if (line != null)
//                   {
//                       //    pntList.Add(new PuntoDueD(line.Start.X, line.Start.Y));
//                       //    pntList.Add(new PuntoDueD(line.End.X, line.End.Y));

//                       pntList.Add(new PuntoDueD(line.Start.X, line.Start.Y));
//                       pntList.Add(new PuntoDueD(line.End.X, line.End.Y));
//                   }
//               }

//               else if (Source[i] is Line2D && i > 0)
//               {
//                   var line = Source[i] as Line2D;
//                   if (line != null)
//                       pntList.Add(new PuntoDueD(line.End.X, line.End.Y));
//               }

//               else if (Source[i] is Arc2D)
//               {
//                   //var factor = 10000;
//                   var arc = Source[i] as Arc2D;

//                   var center = arc.Center;

//                   var startAngle = Math.Atan2(arc.Start.Y - center.Y, arc.Start.X - center.X);

//                   var endAngle = Math.Atan2(arc.End.Y - center.Y, arc.End.X - center.X);

//                   var a = GeometryHelper.RadianToDegree(endAngle);
//                   var radius = arc.Radius;

//                   // per vedere se questo è il problema agisco man
//                   //var tollerance = 0.005;
//                   // 

//                   /*
//                    * con offset maggiore ho bisogno di più risoluzione 
//                    * vero , deve essere inversamente proporzionale a offset
//                    */

//                   var coeffOffset = 1 / (offsetDistance / 50);

//                   // con raggio maggiore servono più punti e quindi e inver anche a raggio
//                   var coeffRadius = 1 / (radius / 50);

//                   // todo . fare proporzionato a raggio e offset
//                   //var angleIncrement = (radius / 500) / offsetDistance;
//                   var angleIncrement = .055;

//                   ///*
//                   // * più arco è grande più servono punti 
//                   // */
//                   //var angleIncrement = radius * tollerance;

//                   //if (angleIncrement < .05)
//                   //    angleIncrement = .05;

//                   // alla lista di punti non aggiungo punto finale e punto iniziale ,
//                   // creo un punto molto vicino - hack.
//                   // evito cosi self-intersect profile 
//                   // ma devo trovare il modo di immettere il punto finale 
//                   // in figura a falce , ci sarebbe un risultato scorretto..

//                   //pntList.Add(new PuntoDueD(arc.Start.X, arc.Start.Y));

//                   if (arc.ClockWise)
//                   {
//                       /* 
//                        * Se il senso è antiorario l'angolo finale dovra essere maggiore
//                        */

//                       if (startAngle < 0)
//                       {
//                           startAngle += Math.PI * 2;
//                           endAngle += Math.PI * 2;
//                       }

//                       if (endAngle >= startAngle)
//                           endAngle -= 2 * Math.PI;

//                       var deltaAngle = endAngle - startAngle;

//                       if (deltaAngle == 0)
//                       {
//                           /* è un cerchio completo*/
//                           endAngle -= Math.PI * 2;
//                           deltaAngle = endAngle - startAngle;

//                       }
//                       //var deltaAngle = endAngle - startAngle;

//                       //if (deltaAngle == 0)
//                       //    throw new Exception();

//                       // Se è clocwise angolo diminuisce 
//                       //startAngle -= angleIncrement;

//                       // angolo partenza deve essere incrementato perchè il punto iniziale è già stato immesso 
//                       // (( punto finale linea precedente ))

//                       var primoP = true;

//                       for (var j = startAngle; j > endAngle; )
//                       {



//                           var x = (Math.Cos(j) * radius) + center.X;
//                           var y = (Math.Sin(j) * radius) + center.Y;

//                           pntList.Add(new PuntoDueD(x, y));

//                           if (primoP)
//                           {
//                               // aggiungo punto parellelo

//                               var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(x, y));

//                               var p2 = GeometryHelper.GetCoordinate(normalAngle, .001, new Point2D(x, y));

//                               pntList.Add(new PuntoDueD(p2.X, p2.Y));

//                               primoP = false;
//                           }

//                           j -= angleIncrement;

//                           if (j < endAngle)
//                               j = endAngle;
//                       }


//                       //{

//                       //    var x = (Math.Cos(endAngle - .0001) * radius) + center.X;
//                       //    var y = (Math.Sin(endAngle - .0001) * radius) + center.Y;

//                       //    pntList.Add(new PuntoDueD(x, y));

//                       //}
//                   }
//                   else // Arco antiorario
//                   {
//                       /*
//                        * Se arco è ccw , angolo è maggiore ,
//                        * 
//                        * si parte da angolo iniziale e si aumenta fino a angolo finale
//                        * 
//                        */

//                       if (startAngle < 0)
//                       {
//                           startAngle += Math.PI * 2;
//                           endAngle += Math.PI * 2;
//                       }

//                       /* 
//                        * Se arco antiorario angolo finale dovra essere maggiore 
//                        */

//                       if (endAngle < startAngle)
//                           endAngle += 2 * Math.PI;

//                       var deltaAngle = endAngle - startAngle;

//                       if (deltaAngle == 0)
//                       {
//                           /* è un cerchio completo*/
//                           /* */

//                           endAngle += Math.PI * 2;
//                           deltaAngle = endAngle - startAngle;

//                       }

//                       //var deltaAngle = endAngle - startAngle;

//                       //if (deltaAngle == 0)
//                       //    throw new Exception();

//                       // ho provato a levare angleIncrement..
//                       //startAngle += angleIncrement;

//                       var secondoP = true;

//                       /**/

//                       var j = startAngle;

//                       while (j < endAngle)
//                       {
//                           //for (var j = startAngle; j < endAngle; )
//                           //{



//                           var x = (Math.Cos(j) * radius) + center.X;
//                           var y = (Math.Sin(j) * radius) + center.Y;

//                           pntList.Add(new PuntoDueD(x, y)); // riporto a double


//                           if (secondoP)
//                           {
//                               // aggiungo punto parellelo

//                               var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(x, y));

//                               var p2 = GeometryHelper.GetCoordinate(normalAngle, .001, new Point2D(x, y));

//                               pntList.Add(new PuntoDueD(p2.X, p2.Y));

//                               secondoP = false;
//                           }

//                           j += angleIncrement;

//                           if (j > endAngle)
//                               j = endAngle;

//                       }

//                       //{

//                       //    var x = (Math.Cos(endAngle - .0001) * radius) + center.X;
//                       //    var y = (Math.Sin(endAngle - .0001) * radius) + center.Y;

//                       //    pntList.Add(new PuntoDueD(x, y));

//                       //}




//                   }

//                   pntList.Add(new PuntoDueD(arc.End.X, arc.End.Y));

//               }




//           }

//           return pntList;
//       }




//public Profile2D Offset(double offsetDistance, bool counterClockwise)
//        {
//            var rslt = new List<ContenitoreLivello>();

//            var pntList = GetPointList(offsetDistance);

//            int num3 = 0;

//            if (pntList.Count >= 3)
//            {
//                ClassO classO = new ClassO();
//                for (int j = 0; j < pntList.Count; j++)
//                {
//                    classO.Add(new ClassA(pntList[j], pntList[(j + 1) % pntList.Count]));
//                }
//                List<PuntoDueD> list6 = new List<PuntoDueD>();
//                List<int> list7 = new List<int>();
//                List<int> list8 = new List<int>();
//                classO.MethodA(list6, list7, list8);
//                num3 += list6.Count;
//            }

//            bool flag2 = false;
//            if ((num3 > 0))
//            {
//                flag2 = true;
//                /* devo resituire che eccezzione di profilo autointersecante */ 
//                return null;

//            }

//            var n2 = new ClassN();

//            n2.MathodA1(pntList, false);

//            // todo : 
//            //if (this._polys.Count > 0) // dentro qui ci entro se ci sono altri polygoni
//            //{
//            //    PolygonalShape polygonalShape = this._polys[this._polys.Count - 1];
//            //    if (polygonalShape.c(n2))  // se questi poligoni possono essere isole
//            //    // per esserre isole devono essere state disegnate dopo anello esterno
//            //    {
//            //        polygonalShape.MethodA1(n2);
//            //        base.Invalidate();
//            //        return;
//            //    }
//            //}
//            var polygonalShape1 = new PolygonalShape(n2);

//            var chamferAngle = Math.PI / 4;
//            //  var chamferAngle = 0;

//            OffsetPathMainClass.Elaborate(new ClassC(polygonalShape1), offsetDistance, 1, chamferAngle, rslt);

//            /*
//             * da polygonal shape a classe utilizzabile nel processp
//             */

//            var pntListRslt = new List<Point2D>();

//            /*
//             * Gli ho chiesto 1 offset, se mi restituisce un numero diverso da 0 o 1 , probabilmente c'è errore
//             */
//            Debug.Assert(rslt.Count <= 1);

//            if (rslt.Count == 0)
//                return null;

//            for (int i = 0; i < rslt.Count; i++)
//            {
//                var contenitoreLivello = rslt[i];

//                /*
//                 * Possono esserci più profili per lo stesso livello di offset,
//                 * li devo tenere separati, 
//                 */
//                for (int m = 0; m < contenitoreLivello.Count; m++)
//                {
//                    var polygonalShape = contenitoreLivello[m];

//                    var a0 = polygonalShape.GetExternPolygon();

//                    for (int k = 0; k < a0.ac().Count; k++)
//                    {
//                        if (a0.ac()[k] is ClassA)
//                        {
//                            var classA0 = a0.ac()[k] as ClassA;


//                            var puntoDueD = classA0.h();
//                            var x2 = classA0.i();

//                            if (k == 0)
//                                pntListRslt.Add(new Point2D(puntoDueD.X, puntoDueD.Y));

//                            pntListRslt.Add(new Point2D(x2.X, x2.Y));

//                        }
//                        else if (a0.ac()[i] is ClassQ)
//                        {
//                            // this.MethodA(A_0.ac()[i] as ClassQ, A_1, A_2);
//                        }
//                    }
//                }
//            }

//            var list = new Profile2D();

//            if (counterClockwise)
//            {
//                for (int i = pntListRslt.Count - 1; i >= 0; i--)
//                {
//                    list.AddPnt(pntListRslt[i]);
//                }
//            }
//            else
//            {
//                for (int i = 0; i < pntListRslt.Count; i++)
//                {
//                    list.AddPnt(pntListRslt[i]);
//                }

//            }


//            return list;
//        }

//public static Point2D Circumcentre(Point2D a, Point2D b, Point2D c)
//{
//    double cx = c.X;
//    double cy = c.Y;
//    double ax = a.X - cx;
//    double ay = a.Y - cy;
//    double bx = b.X - cx;
//    double by = b.Y - cy;
//    double denom = 2 * Det(ax, ay, bx, by);
//    double numx = Det(ay, ax * ax + ay * ay, by, bx * bx + by * by);
//    double numy = Det(ax, ax * ax + ay * ay, bx, bx * bx + by * by);
//    double ccx = cx - numx / denom;
//    double ccy = cy + numy / denom;
//    return new Point2D(ccx, ccy);
//}
///**           *            *            *            *            * @param m00 the [0,0] entry of the matrix           * @param m01 the [0,1] entry of the matrix           * @param m10 the [1,0] entry of the matrix           * @param m11 the [1,1] entry of the matrix           * @return the determinant           */
/////<summary>          
///// Computes the determinant of a 2x2 matrix. Uses standard double-precision arithmetic,           
///// so is susceptible to round-off error.          
/////</summary>          
//private static double Det(double m00, double m01, double m10, double m11)
//{
//    return m00 * m11 - m01 * m10;
//}



//    var center = arc.Center;

//    var startAngle = Math.Atan2(arc.Start.Y - center.Y, arc.Start.X - center.X );

//    var endAngle = Math.Atan2(arc.End.X - center.X, arc.End.Y - center.Y);


//    if (startAngle < 0)
//    {
//        startAngle += Math.PI * 2;
//        endAngle += Math.PI * 2;
//    }

//    if (endAngle >= startAngle)
//        endAngle -= 2 * Math.PI;


//    var radius = arc.Radius;

//    if (endAngle < startAngle)
//        endAngle += 2 * Math.PI;

//   // var angleIncrement = GetIncrement(ArcParsingDistanceTollerance, radius);

//    // troppo oneroso con valori minori..
//    var angleIncrement = 0.1;
//    /*
//     * uso ciclo while al posto di for cosi ultimo elemento è uguale a angolo finale e non minore di poco
//     */

//    while (startAngle < endAngle)
//    {
//        startAngle += angleIncrement;

//        if (startAngle > endAngle)
//            startAngle = endAngle;

//        var x = (Math.Cos(startAngle) * radius) + center.X;
//        var y = (Math.Sin(startAngle) * radius) + center.Y;
//        pntList.Add(new PuntoDueD(x, y));

//    }

//    //for (var j = startAngle; j <= endAngle; j += angleIncrement)
//    //{
//    //    var x = (Math.Cos(j) * radius) + center.X;
//    //    var y = (Math.Sin(j) * radius) + center.Y;
//    //    pntList.Add(new PuntoDueD(x, y));
//    //}

//}

//private class ParsedArc
//{
//    private readonly double _arcTollerance;
//    private readonly double _distanceTollerance;

//    public ParsedArc(double arcTollerance, double distanceTollerance)
//    {
//        _arcTollerance = arcTollerance;
//        _distanceTollerance = distanceTollerance;
//    }

//    /*
//     * aux class
//     * per creare parsedArc.
//     * 
//     * 
//     * prende in ingresso punti
//     * 
//     * mi dice se ok , se lo mangia. se no lo sputo 
//     * 
//     * quando sputa arco  è finito.
//     * 
//     * deve avere almeno 3 punti ok
//     * 
//     */


//    //public void ShutDown()
//    //{

//    //}

//    public bool IsArcStarted { get; set; }

//    private List<Point2D> bufferPnt = new List<Point2D>();

//    public double? Radius { get; set; }

//    public Point2D Center { get; set; }

//    public bool TryStartArc(Point2D p1, Point2D p2, Point2D p3, Point2D p4, ref List<Point2D> externBufferPnt)
//    {

//        if (IsOk(p1, p2, p3, p4))
//        {

//            bufferPnt.Add(p1);
//            bufferPnt.Add(p2);
//            bufferPnt.Add(p3);
//            bufferPnt.Add(p4);

//            externBufferPnt.Clear();

//            /*
//             * setto centro arco e raggio trovato
//             * 
//             * se aggiungo punti e non sono compatibili con questo arco e raggio li scarico.
//             */
//            Center = GeometryHelper.Circocentro(p1, p2, p3);

//            Radius = GeometryHelper.Distance(Center, p1);

//            IsArcStarted = true;

//            return true;
//        }

//        return false;
//    }

//    private bool IsOk(Point2D p1, Point2D p2, Point2D p3, Point2D p4)
//    {
//        /*
//         * prima dovrei vedere se sono sullo stesso arco , 
//         * 
//         * qui posso gia prendere raggio presunto.
//         * 
//         * e creare un valore di comparazione rapportato alle dimensioni del raggio.
//         * 
//         * , in secondo ambito controllo che la loro angolo sia vicino 
//         * 
//         * e terzo devono essere in ordine.
//         * 
//         * a partire dal primo l'angolo creato dal punto deve essere compatibile.
//         */
//        // Devo creare archi in modo che da un punto e l'altro ci sia distanze definita
//        if (GeometryHelper.SonoVicini(p1, p2, p3, p4, _distanceTollerance))
//        {
//            if (GeometryHelper.IsInArc(p1, p2, p3, p4, _arcTollerance))
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    public bool EatPoint(Point2D point2D)
//    {
//        /*
//         * se arriva qui l'arco deve essere gia cominciato,
//         * quindi conterra raggio e centro.
//         */
//        var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

//        // Devo creare archi in modo che da un punto e l'altro ci sia distanze definita
//        if (GeometryHelper.SonoVicini(last3[0], last3[1], last3[2], point2D, _distanceTollerance))
//        {
//            //if (GeometryHelper.IsInArcV_2(last3[0], last3[1], last3[2], point2D, Center, Radius.Value, _arcTollerance))
//            if (GeometryHelper.IsInArc(last3[0], last3[1], last3[2], point2D, _arcTollerance))
//            {
//                bufferPnt.Add(point2D);

//                return true;
//            }
//        }
//        return false;

//        //if (IsOk(point2D))
//        //{
//        //    bufferPnt.Add(point2D);



//        //    return true;
//        //}
//        ///*
//        // * comportamenti elegibile 
//        // * 
//        // * se ritorno false , gestisco la creazione dell'arco.
//        // * e nego ulteriori modifiche .
//        // */

//        //return false;
//    }

//    public Arc2D CreateArc()
//    {
//        /* controllo che sia possibile creare arco.*/

//        if (bufferPnt.Count < 4)
//            throw new Exception("ParsedArc");

//        var firstPoint = bufferPnt.FirstOrDefault();
//        var lastPoint = bufferPnt.LastOrDefault();

//        /*
//         * considerare anche di fare la media del buffer..
//         */
//        //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);
//        var midPoint = bufferPnt[(int)bufferPnt.Count / 2];

//        var circoCentro = GeometryHelper.Circocentro(firstPoint, midPoint, lastPoint);
//        //var circoCentro1 = GeometryHelper.Circocentro(last3[0], last3[1], last3[2]);

//        var raggio = GeometryHelper.Distance(circoCentro, lastPoint);

//        if (raggio == null)
//            return null;

//        var arc = new Arc2D
//        {
//            Center = new Point2D(circoCentro),
//            Start = new Point2D(firstPoint),
//            End = new Point2D(lastPoint),
//            Radius = raggio.Value,
//        };

//        arc.ClockWise = IsClockWise(circoCentro);

//        return arc;
//    }

//    private bool IsClockWise(Point2D center)
//    {
//        /*
//         * Guardo come incrementano i punti : 
//         * se angolo cresce >> antiorario
//         * se angolo diminuisce << orario
//         */
//        if (bufferPnt.Count < 4)
//            throw new Exception("ParsedArc");

//        var p1 = bufferPnt[0];
//        var p2 = bufferPnt[2];

//        var angle1 = Math.Atan2(p1.Y - center.Y, p1.X - center.X);
//        var angle2 = Math.Atan2(p2.Y - center.Y, p2.X - center.X);

//        angle1 = GeometryHelper.GetPositiveAngle(p1.X - center.X, p1.Y - center.Y);
//        angle2 = GeometryHelper.GetPositiveAngle(p2.X - center.X, p2.Y - center.Y);

//        if (angle1 < angle2)
//            return false;

//        return true;
//    }


//    private bool IsOk(Point2D point2D)
//    {
//        /*
//         * prendo ultimi 3 elementi dal buffer e faccio prova con questo
//         */

//        if (bufferPnt.Count < 3)
//            throw new Exception("ParsedArc");

//        var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

//        return IsOk(last3[0], last3[1], last3[2], point2D);
//    }

//    ////public double GetRadius()
//    ////{
//    ////    return 9;
//    ////}

//    ////public Point2D GetCentre()
//    ////{
//    ////    return new Point2D();
//    ////}

//    ////public Point2D GetStart()
//    ////{
//    ////    return new Point2D();
//    ////}

//    //public Point2D GetEnd()
//    //{
//    //    return new Point2D();
//    //}


//}