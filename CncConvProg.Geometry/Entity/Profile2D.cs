using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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

        /// <summary>
        /// Punto inizio toolpath
        /// </summary>
        //private Point2D _toolpathSp;
        //public Point2D ToolPathStartPoint
        //{
        //    get
        //    {

        //        if (_toolpathSp == null)
        //        {
        //            var e = _profile.FirstOrDefault();

        //            if (e != null)
        //                _toolpathSp = new Point2D(e.GetFirstPnt());

        //        }
        //        return _toolpathSp;
        //    }

        //    set
        //    {
        //        _toolpathSp = value;
        //    }

        //}
        public Point2D ToolPathStartPoint { get; set; }

        public Point2D ToolPathEndPoint { get; set; }


        /// <summary>
        /// Punto Finale ToolPath
        /// </summary>
        //private Point2D __toolpathEp;
        //public Point2D ToolPathEndPoint
        //{
        //    get
        //    {

        //        if (__toolpathEp == null)
        //        {
        //            var e = _profile.LastOrDefault();

        //            if (e != null)
        //                __toolpathEp = new Point2D(e.GetLastPnt());

        //        }
        //        return __toolpathEp;
        //    }

        //    set
        //    {
        //        __toolpathEp = value;
        //    }

        //}


        public List<IEntity2D> Source
        {
            get { return _profile; }
        }

        public static Profile2D GetMovedProfile(Profile2D profile2D, Matrix3D matrix3D)
        {
            var preview = profile2D.Source;

            var copy = new Profile2D();

            foreach (var entity2D in preview)
            {
                copy.AddEntity(entity2D.MultiplyMatrixCopy(matrix3D));
            }

            return copy;
        }

        public void AddEntity(object ob)
        {
            if (ob is List<Point2D>)
            {
                var listPnt = ob as List<Point2D>;

                foreach (var d in listPnt)
                {
                    AddPnt(d);
                }

                return;
            }

            var entity2D = ob as IEntity2D;

            if (entity2D is Arc2D)
            {
                // Se inizio arco non coincide con ultimo punto , devo inserire linea
                var arc = entity2D as Arc2D;

                var lastPnt = GetLastPoint();

                var iniPoint = arc.Start;

                if (lastPnt != null && !lastPnt.Equals(iniPoint, 10))
                    AddPnt(iniPoint);
            }

            // devo settare primo punto
            //if (_startPoint == null)
            //{
            //    _startPoint = entity2D.GetFirstPnt();
            //}

            _profile.Add(entity2D);
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
            var ab = GeometryHelper.PitagoraIpoNota(radius, maxDistance / 2);

            var angle = Math.Acos(ab / radius);

            return angle;
        }

        /// <summary>
        /// Metodo usato durante la generazione di punti 
        /// da arco a punti.
        /// Restituisce il valore raggio corretto, in modo che dopo offset, estremita dei segmenti risiedano su cfr e non altrove.
        /// </summary>
        /// <param name="arcRadius"></param>
        /// <param name="offsetValue"></param>
        /// <param name="angoloIncremento"></param>
        /// <param name="centerArc"></param>
        /// <returns>Public for test purpose</returns> 
        public static double GetCorrectArcRadius(double arcRadius, double offsetValue, double angoloIncremento, Point2D centerArc)
        {
            /*
             * Per trigonometria usata in questo metodo
             * http://www.ing.unibas.it/uploads/precorsi/lezioni/lez10.pdf
             */

            /*
             * beta = a * sinB
             * a = ipo <> raggio
             * beta = angolo opposto b 
             */

            var angoloAlCentro = angoloIncremento / 2;
            var beta = arcRadius * Math.Sin(angoloAlCentro);

            var catetoPerpendicolareLato = GeometryHelper.PitagoraIpoNota(arcRadius, beta);


            var puntoPerpCatetoPerpendicolareDopoOffset = GeometryHelper.GetCoordinate(angoloAlCentro,
                                                                             catetoPerpendicolareLato + offsetValue, centerArc);

            var lunghezzaCatetoPerpendicolare = GeometryHelper.Distance(puntoPerpCatetoPerpendicolareDopoOffset, centerArc);

            /*
             * ora ritrovo beta del triangolo costruito su offset eseguito
             * 
             * beta = c * tanBeta
             * 
             * dove beta è cateto minore e c e altro cateto
             */

            var betaTriMaggiore = lunghezzaCatetoPerpendicolare * Math.Tan(angoloAlCentro);

            var raggioEffettivoDopoOffset = GeometryHelper.Pitagora(betaTriMaggiore.Value, lunghezzaCatetoPerpendicolare.Value);

            // questo è il raggio effettivo se costruisco i punti come sto facendo ora..

            var deltaR = raggioEffettivoDopoOffset - (arcRadius + offsetValue);

            return arcRadius - deltaR;
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
        private List<PuntoDueD> GetPointList_lll(double offsetDistance = 0)
        {
            var pntList = new List<PuntoDueD>();

            var arcStartEndNormalLength = .0001;

            for (int i = 0; i < Source.Count; i++)
            {
                // se il primo punto è linea Inserisco anche il punto iniziale
                // altrimenti inserisco solamente il punto finale
                if (Source[i] is Line2D && i == 0)
                {
                    var line = Source[i] as Line2D;
                    if (line != null)
                    {
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
                     * Se invece l'elemento è un'arco so cazzi..
                     */
                else if (Source[i] is Arc2D)
                {
                    var arc = Source[i] as Arc2D;

                    var center = arc.Center;

                    var startAngle = Math.Atan2(arc.Start.Y - center.Y, arc.Start.X - center.X);

                    var endAngle = Math.Atan2(arc.End.Y - center.Y, arc.End.X - center.X);

                    // pensavo di fare questo valore proporzionato alla misura del raggio,
                    // per ora lo lascio cosi..
                    const double angleIncrement = .02;

                    var radius = arc.Radius;

                    var radiusCorrective = radius;

                    if (offsetDistance != 0)
                        radiusCorrective = GetCorrectArcRadius(arc.Radius, offsetDistance, angleIncrement, center);



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

                        var primoP = true;
                        var first = true;
                        for (var j = startAngle; j > endAngle; j -= angleIncrement)
                        {
                            if (first)
                            {
                                var x = (Math.Cos(j) * radius) + center.X;
                                var y = (Math.Sin(j) * radius) + center.Y;
                                pntList.Add(new PuntoDueD(x, y));
                                first = false;

                                if (primoP) // ignoro punto parallelo, non "dovrebbe" più servire
                                {
                                    //aggiungo punto parellelo

                                    var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(x, y));

                                    //normalAngle -= Math.PI;// todo . controllare con figura semplice--

                                    var p2 = GeometryHelper.GetCoordinate(normalAngle, arcStartEndNormalLength, new Point2D(x, y));

                                    pntList.Add(new PuntoDueD(p2.X, p2.Y));

                                    //primoP = false;
                                }
                                continue;
                            }

                            var x1 = (Math.Cos(j) * radiusCorrective) + center.X;
                            var y1 = (Math.Sin(j) * radiusCorrective) + center.Y;
                            pntList.Add(new PuntoDueD(x1, y1));




                        }

                        var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

                        normalAngleEndPnt -= Math.PI;

                        var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, arc.End);

                        pntList.Add(new PuntoDueD(normalEndPnt.X, normalEndPnt.Y));

                        // var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

                        //// normalAngleEndPnt += Math.PI;

                        // var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, arc.End);

                        // pntList.Add(new PuntoDueD(normalEndPnt.X, normalEndPnt.Y));

                        // {

                        //     var x = (Math.Cos(endAngle - .0001) * radius) + center.X;
                        //     var y = (Math.Sin(endAngle - .0001) * radius) + center.Y;

                        //     pntList.Add(new PuntoDueD(x, y));

                        // }
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

                        //for (var j = startAngle; j < endAngle - angleIncrement / 2; j += angleIncrement)

                        var first2 = true;
                        for (var j = startAngle; j < endAngle; j += angleIncrement)
                        {
                            if (first2)
                            {
                                var x = (Math.Cos(j) * radius) + center.X;
                                var y = (Math.Sin(j) * radius) + center.Y;

                                pntList.Add(new PuntoDueD(x, y)); // riporto a double
                                first2 = false;
                                if (normalFirstPoint)
                                {
                                    //aggiungo punto parellelo

                                    var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(x, y));

                                    var p2 = GeometryHelper.GetCoordinate(normalAngle, arcStartEndNormalLength, new Point2D(x, y));

                                    pntList.Add(new PuntoDueD(p2.X, p2.Y));

                                    normalFirstPoint = false;
                                }
                                continue;
                            }

                            var x1 = (Math.Cos(j) * radiusCorrective) + center.X;
                            var y1 = (Math.Sin(j) * radiusCorrective) + center.Y;

                            pntList.Add(new PuntoDueD(x1, y1)); // riporto a double


                        }

                        /*
                         * Devo controllare che punto normale non sia maggiore del punto finale ,
                         * magari devo guardare in termini di angolo
                         */
                        // qui inserisco il punto normale a ultimo punto
                        var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

                        normalAngleEndPnt -= Math.PI;

                        var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, arc.End);

                        pntList.Add(new PuntoDueD(normalEndPnt.X, normalEndPnt.Y));

                    }

                    // In ultimo aggiungo endpoint
                    // serve che sia moltiplicato per radisu anche endpoinr
                    var endX = (Math.Cos(endAngle) * radius) + center.X;
                    var endY = (Math.Sin(endAngle) * radius) + center.Y;

                    pntList.Add(new PuntoDueD(endX, endY));

                }
            }

            return pntList;
        }


        /// <summary>
        /// Costruisce lista di punti 2D,
        /// archi li trasforma in sequenza di punti.
        /// 
        /// Ritornano problemi.
        /// 
        /// Proviamo a rifarlo.
        /// 
        /// Per evitare problemi di arrotondamento , l'arrotondamento a float ci vuole.
        /// Angolo compensatore per primo punto è una bischerata.. Molto meglio fare sistema normal, con valore estramente piccolo.
        /// </summary>



        //// 23/08/ funzionante per contorno esterno..
        /////// <summary> 
        /////// Costruisce lista di punti 2D,
        /////// archi li trasforma in sequenza di punti.
        /////// 
        /////// Ritornano problemi.
        /////// 
        /////// Proviamo a rifarlo.
        /////// 
        /////// Per evitare problemi di arrotondamento , l'arrotondamento a float ci vuole.
        /////// Angolo compensatore per primo punto è una bischerata.. Molto meglio fare sistema normal, con valore estramente piccolo.
        /////// </summary>
        //private List<PuntoDueD> GetPointList(double offsetDistance = 0)
        //{
        //    var pntList = new List<PuntoDueD>();


        //    for (int i = 0; i < Source.Count; i++)
        //    {
        //        // se il primo punto è linea Inserisco anche il punto iniziale
        //        // altrimenti inserisco solamente il punto finale
        //        if (Source[i] is Line2D && i == 0)
        //        {
        //            var line = Source[i] as Line2D;
        //            if (line != null)
        //            {
        //                float xIni = (float)line.Start.X;
        //                float yIni = (float)line.Start.Y;

        //                float xEnd = (float)line.End.X;
        //                float yEnd = (float)line.End.Y;

        //                pntList.Add(new PuntoDueD(xIni, yIni));
        //                pntList.Add(new PuntoDueD(xEnd, yEnd));
        //            }
        //        }

        //        else if (Source[i] is Line2D && i > 0)
        //        {
        //            var line = Source[i] as Line2D;
        //            if (line != null)
        //                pntList.Add(new PuntoDueD((float)line.End.X, (float)line.End.Y));
        //        }

        //            /*
        //             * Se invece l'elemento è un'arco so cazzi..
        //             */
        //        else if (Source[i] is Arc2D)
        //        {
        //            var arc = Source[i] as Arc2D;

        //            /*
        //             * centro off
        //             */


        //            var center = arc.Center;

        //            var startAngle = Math.Atan2(arc.Start.Y - center.Y, arc.Start.X - center.X);

        //            var endAngle = Math.Atan2(arc.End.Y - center.Y, arc.End.X - center.X);

        //            // pensavo di fare questo valore proporzionato alla misura del raggio,
        //            // per ora lo lascio cosi..
        //            float angleIncrement = .02f;

        //            // l'angolo dipende anche da valore di offset ?

        //            /*
        //             * Più offset va lontano dal centro , più puo essere piccolo 
        //             * Piu offset va in direzione del centro più deve essere maggiore.
        //             * 
        //             * Come faccio a determinare quando offset è in direzione del centro.
        //             * Devo sapere senso direzione profilo.
        //             * 
        //             * Ossumo che il profilo lo scrivo in senso orario quindi. ( tenendo il centro del poligono / profilo alla mia sx)
        //             * 
        //             * Quando trovo arco cw e offset positivo = offset min 0.0001
        //             * 
        //             * Quando trovo arco ccw e offset positivo = arc.Radius
        //             * 
        //             * Con offset negativo il senso è inverto soluzioni
        //             */

        //            var distance = arc.Radius;

        //            //if ((!arc.ClockWise && offsetDistance > 0) || (arc.ClockWise && offsetDistance < 0))
        //            //    distance = .00001;

        //            var arcStartEndNormalLength = GetNormalLength(angleIncrement, distance);

        //            var radius = arc.Radius;

        //            var radiusCorrective = radius;

        //            // peggiora prestazioni...
        //            //if (offsetDistance != 0)
        //            //    radiusCorrective = GetCorrectArcRadius(arc.Radius, offsetDistance, angleIncrement, center);


        //            /*
        //             * Vedere se è il secondo punto è da omettere visto che il secondo è usato per la tangente
        //             */

        //            if (arc.ClockWise)
        //            {
        //                /* 
        //                 * Se il senso è antiorario l'angolo finale dovra essere maggiore
        //                 */

        //                if (startAngle < 0)
        //                {
        //                    startAngle += Math.PI * 2;
        //                    endAngle += Math.PI * 2;
        //                }

        //                if (endAngle >= startAngle)
        //                    endAngle -= 2 * Math.PI;

        //                var deltaAngle = endAngle - startAngle;

        //                if (deltaAngle == 0)
        //                {
        //                    /* è un cerchio completo*/
        //                    endAngle -= Math.PI * 2;
        //                    deltaAngle = endAngle - startAngle;

        //                }

        //                var primoP = true;
        //                var first = true;

        //                for (var j = startAngle; j > endAngle + angleIncrement; j -= angleIncrement)
        //                {
        //                    if (first)
        //                    {
        //                        var x = (Math.Cos(j) * radius) + center.X;
        //                        var y = (Math.Sin(j) * radius) + center.Y;

        //                        var addPnt = new PuntoDueD((float)x, (float)y);
        //                        pntList.Add(addPnt);

        //                        /*
        //                         * per rimuovere ultimo centisimi di errore provare a rimuovere float..
        //                         */
        //                        first = false;

        //                        if (primoP)
        //                        {
        //                            //aggiungo punto parellelo

        //                            var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(addPnt.X, addPnt.Y));

        //                            //normalAngle -= Math.PI;// todo . controllare con figura semplice--

        //                            var p2 = GeometryHelper.GetCoordinate(normalAngle, arcStartEndNormalLength, new Point2D(addPnt.X, addPnt.Y));
        //                            //var p3 = GeometryHelper.GetCoordinate(normalAngle, -arcStartEndNormalLength, new Point2D(addPnt.X, addPnt.Y));
        //                            ////------------
        //                            //   pntList.Add(new PuntoDueD((float)p3.X, (float)p3.Y));
        //                            //      pntList.Add(addPnt);


        //                            //--------------

        //                            pntList.Add(new PuntoDueD((float)p2.X, (float)p2.Y));

        //                            primoP = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var x1 = (Math.Cos(j) * radiusCorrective) + center.X;
        //                        var y1 = (Math.Sin(j) * radiusCorrective) + center.Y;
        //                        pntList.Add(new PuntoDueD((float)x1, (float)y1));
        //                    }
        //                }

        //                //qui inserisco il punto normale a ultimo punto
        //                var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

        //                normalAngleEndPnt += Math.PI;

        //                var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, new Point2D(arc.End.X, arc.End.Y));

        //                pntList.Add(new PuntoDueD(normalEndPnt.X, normalEndPnt.Y));
        //            }
        //            else // Arco antiorario
        //            {
        //                /*
        //                 * Se arco è ccw , angolo è maggiore ,
        //                 * 
        //                 * si parte da angolo iniziale e si aumenta fino a angolo finale
        //                 * 
        //                 */

        //                if (startAngle < 0)
        //                {
        //                    startAngle += Math.PI * 2;
        //                    endAngle += Math.PI * 2;
        //                }

        //                /* 
        //                 * Se arco antiorario angolo finale dovra essere maggiore 
        //                 */

        //                if (endAngle < startAngle)
        //                    endAngle += 2 * Math.PI;

        //                var deltaAngle = endAngle - startAngle;

        //                if (deltaAngle == 0)
        //                {
        //                    /* è un cerchio completo*/
        //                    /* */

        //                    endAngle += Math.PI * 2;
        //                    deltaAngle = endAngle - startAngle;

        //                }

        //                //var deltaAngle = endAngle - startAngle;

        //                //if (deltaAngle == 0)
        //                //    throw new Exception();

        //                // ho provato a levare angleIncrement..
        //                //startAngle += angleIncrement;


        //                /*
        //                 * Allora per risolvere il problema degli archi , ( oltre alla soluzione di fare primo e ultimo angolo compensatore,)
        //                 * 
        //                 * Provo ad aggiungere 2 punti in modo che il primo e ultimo segmento dell'arco siano delle "normali" all'arco.
        //                 * Cosi algoritmo per offset dovrebbe funzionare correttamente.
        //                 * 
        //                 * Quindi devo inserire punto iniziale ( con angolo 0).
        //                 * Il punto normale a questo.
        //                 * 
        //                 * ilPunto normale al punto finale 
        //                 * e il punto finale 
        //                 * 
        //                 * In questo modo l'algoritmo dovrebbe comportrsi correttamente
        //                 * 
        //                 * punto iniziale  
        //                 * 
        //                 * per inserire il punto finale mi basta inserire al penultimo posto il punto normale..
        //                 * 
        //                 */

        //                /*
        //                 * Con ciclo for , viene inserito il primo punto , poi inserisco normale secondo punto.
        //                 * 
        //                 * Questo ciclo "non" dovrebbe inserire punto per angolo finale. Si dovrebbe interrompere prima
        //                 * . Incrementa variabile
        //                 * . Controlla Variabile , se non passa controllo si interrompe , quindi si ferma 
        //                 * 
        //                 * sembra +/- funzionare 
        //                 * 
        //                 * pero l'arco estratto o faccio una media.. che mi sa che basti oppure 
        //                 */
        //                var normalFirstPoint = true;
        //                var first2 = true;

        //                // Con skipSecondPoint mi salta classe parsed arc , in quanto dopo ho troppa differenza fra 1 e terzo punto..
        //                //var skipSecondPoint = false;

        //                for (var j = startAngle; j < endAngle - angleIncrement; j += angleIncrement)
        //                {
        //                    //if (skipSecondPoint)
        //                    //{
        //                    //    skipSecondPoint = false;
        //                    //    continue;
        //                    //}
        //                    if (first2)
        //                    {
        //                        var x = (Math.Cos(j) * radius) + center.X;
        //                        var y = (Math.Sin(j) * radius) + center.Y;

        //                        //var addedpntPrev = new PuntoDueD(x + arcStartEndNormalLength, y);
        //                        //pntList.Add(addedpntPrev);

        //                        var addedpnt = new PuntoDueD((float)x, (float)y);
        //                        pntList.Add(addedpnt);

        //                        first2 = false;
        //                        if (normalFirstPoint)
        //                        {
        //                            //   aggiungo punto parellelo

        //                            var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(addedpnt.X, addedpnt.Y));

        //                            var p2 = GeometryHelper.GetCoordinate(normalAngle, arcStartEndNormalLength, new Point2D(addedpnt.X, addedpnt.Y));

        //                            // pntList.Add(new PuntoDueD(p2.X, p2.Y));
        //                            pntList.Add(new PuntoDueD((float)p2.X, (float)p2.Y));


        //                            //  pntList.Add(addedpnt);


        //                            normalFirstPoint = false;

        //                            //skipSecondPoint = true;
        //                        }
        //                        //continue;
        //                    }

        //                    else
        //                    {
        //                        var x1 = (Math.Cos(j) * radiusCorrective) + center.X;
        //                        var y1 = (Math.Sin(j) * radiusCorrective) + center.Y;

        //                        pntList.Add(new PuntoDueD((float)x1, (float)y1)); // riporto a double
        //                    }


        //                }

        //                /*
        //                 * Devo controllare che punto normale non sia maggiore del punto finale ,
        //                 * magari devo guardare in termini di angolo
        //                 */
        //                // qui inserisco il punto normale a ultimo punto
        //                var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

        //                normalAngleEndPnt += Math.PI;

        //                var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, arc.End);

        //                pntList.Add(new PuntoDueD((float)normalEndPnt.X, (float)normalEndPnt.Y));

        //            }

        //            // In ultimo aggiungo endpoint
        //            // serve che sia moltiplicato per radisu anche endpoinr
        //            //var endX = (Math.Cos(endAngle) * radius) + center.X;
        //            //var endY = (Math.Sin(endAngle) * radius) + center.Y;

        //            pntList.Add(new PuntoDueD((float)arc.End.X, (float)arc.End.Y));

        //        }
        //    }

        //    return pntList;
        //}

        // 03/09/2011 ultimo
        ///// <summary> 
        ///// Costruisce lista di punti 2D,
        ///// archi li trasforma in sequenza di punti.
        ///// 
        ///// Ritornano problemi.
        ///// 
        ///// Proviamo a rifarlo.
        ///// 
        ///// Per evitare problemi di arrotondamento , l'arrotondamento a float ci vuole.
        ///// Angolo compensatore per primo punto è una bischerata.. Molto meglio fare sistema normal, con valore estramente piccolo.
        ///// </summary>
        private List<PuntoDueD> GetPointList()
        {
            var pntList = new List<PuntoDueD>();

            ArcPatch.CreateNewModelClass();

            for (int i = 0; i < Source.Count; i++)
            {
                // se il primo punto è linea Inserisco anche il punto iniziale
                // altrimenti inserisco solamente il punto finale
                if (Source[i] is Line2D && i == 0)
                {
                    var line = Source[i] as Line2D;
                    if (line != null)
                    {
                        float xIni = (float)line.Start.X;
                        float yIni = (float)line.Start.Y;

                        float xEnd = (float)line.End.X;
                        float yEnd = (float)line.End.Y;

                        pntList.Add(new PuntoDueD(xIni, yIni));
                        pntList.Add(new PuntoDueD(xEnd, yEnd));
                    }
                }

                else if (Source[i] is Line2D && i > 0)
                {
                    var line = Source[i] as Line2D;
                    if (line != null)
                        pntList.Add(new PuntoDueD((float)line.End.X, (float)line.End.Y));
                }

                    /*
                     * Se invece l'elemento è un'arco so cazzi..
                     */
                else if (Source[i] is Arc2D)
                {
                    var arc = Source[i] as Arc2D;

                    /*
                     * centro off
                     */


                    var center = arc.Center;

                    var startAngle = Math.Atan2(arc.Start.Y - center.Y, arc.Start.X - center.X);

                    var endAngle = Math.Atan2(arc.End.Y - center.Y, arc.End.X - center.X);

                    // pensavo di fare questo valore proporzionato alla misura del raggio,
                    // per ora lo lascio cosi..
                    float angleIncrement = .01f;

                    // l'angolo dipende anche da valore di offset ?

                    /*
                     * Più offset va lontano dal centro , più puo essere piccolo 
                     * Piu offset va in direzione del centro più deve essere maggiore.
                     * 
                     * Come faccio a determinare quando offset è in direzione del centro.
                     * Devo sapere senso direzione profilo.
                     * 
                     * Ossumo che il profilo lo scrivo in senso orario quindi. ( tenendo il centro del poligono / profilo alla mia sx)
                     * 
                     * Quando trovo arco cw e offset positivo = offset min 0.0001
                     * 
                     * Quando trovo arco ccw e offset positivo = arc.Radius
                     * 
                     * Con offset negativo il senso è inverto soluzioni
                     */

                    //   var distance = arc.Radius;

                    //if ((!arc.ClockWise && offsetDistance > 0) || (arc.ClockWise && offsetDistance < 0))
                    //    distance = .00001;

                    // var arcStartEndNormalLength = GetNormalLength(angleIncrement, distance);

                    var radius = arc.Radius;

                    var radiusCorrective = radius;

                    //   var angleMinIncrement = .0000001; // non fà un caxxo..

                    // peggiora prestazioni...
                    //if (offsetDistance != 0)
                    //    radiusCorrective = GetCorrectArcRadius(arc.Radius, offsetDistance, angleIncrement, center);


                    /*
                     * Vedere se è il secondo punto è da omettere visto che il secondo è usato per la tangente
                     */

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

                        var primoP = true;
                        var first = true;

                        for (var j = startAngle; j > endAngle; j -= angleIncrement)
                        {
                            if (j < endAngle + angleIncrement)
                                j = endAngle + angleIncrement;

                            if (first)
                            {
                                var x = (Math.Cos(j) * radius) + center.X;
                                var y = (Math.Sin(j) * radius) + center.Y;

                                var addPnt = new PuntoDueD((float)x, (float)y, true);
                                pntList.Add(addPnt);

                                var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, new Point2D(addPnt.X, addPnt.Y));

                                //normalAngleEndPnt -= Math.PI;

                                //var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, .2, new Point2D(arc.End.X, arc.End.Y));

                                //pntList.Add(new PuntoDueD(normalEndPnt.X, normalEndPnt.Y));

                                ArcPatch.AddPunto(addPnt,
                                                  new AuxArc()
                                                      {
                                                          ArcCenter = new PuntoDueD(arc.Center.X, arc.Center.Y),
                                                          ArcRadius = arc.Radius,
                                                          ClockWise = arc.ClockWise,
                                                          CurrentAngle = j,
                                                          IsAngleStarting = true,
                                                          NormalAngle = normalAngleEndPnt,
                                                          EndPoint = new PuntoDueD(addPnt.X, addPnt.Y),
                                                          //   PreviousPoint = pntList.LastOrDefault()

                                                      });

                                first = false;

                                if (primoP)
                                {
                                    //var x1 = (Math.Cos(j - angleMinIncrement) * radius) + center.X;
                                    //var y1 = (Math.Sin(j - angleMinIncrement) * radius) + center.Y;

                                    //var addPnt1 = new PuntoDueD(x1, y1);
                                    //pntList.Add(addPnt1);

                                    primoP = false;
                                }
                            }
                            else
                            {
                                var x1 = (Math.Cos(j) * radiusCorrective) + center.X;
                                var y1 = (Math.Sin(j) * radiusCorrective) + center.Y;
                                pntList.Add(new PuntoDueD((float)x1, (float)y1));
                            }
                        }

                        //qui inserisco il punto normale a ultimo punto
                        //var x2 = (Math.Cos(endAngle + angleMinIncrement) * radius) + center.X;
                        //var y2 = (Math.Sin(endAngle + angleMinIncrement) * radius) + center.Y;

                        //var addPnt2 = new PuntoDueD(x2, y2);
                        //pntList.Add(addPnt2);
                        //var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

                        //normalAngleEndPnt += Math.PI;

                        //var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, new Point2D(arc.End.X, arc.End.Y));

                        //pntList.Add(new PuntoDueD(normalEndPnt.X, normalEndPnt.Y));
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
                        var first2 = true;

                        // Con skipSecondPoint mi salta classe parsed arc , in quanto dopo ho troppa differenza fra 1 e terzo punto..
                        //var skipSecondPoint = false;

                        for (var j = startAngle; j < endAngle; j += angleIncrement)
                        {
                            if (j > endAngle - angleIncrement)
                                j = endAngle - angleIncrement;
                            //if (skipSecondPoint)
                            //{
                            //    skipSecondPoint = false;
                            //    continue;
                            //}
                            if (first2)
                            {
                                var x = (Math.Cos(j) * radius) + center.X;
                                var y = (Math.Sin(j) * radius) + center.Y;

                                //var addedpntPrev = new PuntoDueD(x + arcStartEndNormalLength, y);
                                //pntList.Add(addedpntPrev);

                                var addedpnt = new PuntoDueD((float)x, (float)y, true);
                                pntList.Add(addedpnt);

                                var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, new Point2D(addedpnt.X, addedpnt.Y));

                                normalAngleEndPnt += Math.PI;
                                ArcPatch.AddPunto(addedpnt,
                                              new AuxArc()
                                              {
                                                  ArcCenter = new PuntoDueD(arc.Center.X, arc.Center.Y),
                                                  ArcRadius = arc.Radius,
                                                  ClockWise = arc.ClockWise,
                                                  CurrentAngle = j,
                                                  IsAngleStarting = true,
                                                  NormalAngle = normalAngleEndPnt,
                                                  EndPoint = new PuntoDueD(addedpnt.X, addedpnt.Y),
                                                  //  PreviousPoint = pntList.LastOrDefault()
                                              });

                                first2 = false;
                                if (normalFirstPoint)
                                {
                                    //var x1 = (Math.Cos(j + angleMinIncrement) * radius) + center.X;
                                    //var y1 = (Math.Sin(j + angleMinIncrement) * radius) + center.Y;

                                    //var addPnt1 = new PuntoDueD(x1, y1);
                                    //pntList.Add(addPnt1);
                                    //   aggiungo punto parellelo

                                    //var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(addedpnt.X, addedpnt.Y));

                                    //var p2 = GeometryHelper.GetCoordinate(normalAngle, arcStartEndNormalLength, new Point2D(addedpnt.X, addedpnt.Y));

                                    //// pntList.Add(new PuntoDueD(p2.X, p2.Y));
                                    //pntList.Add(new PuntoDueD((float)p2.X, (float)p2.Y));


                                    //  pntList.Add(addedpnt);


                                    normalFirstPoint = false;

                                    //skipSecondPoint = true;
                                }
                                //continue;
                            }

                            else
                            {
                                var x1 = (Math.Cos(j) * radiusCorrective) + center.X;
                                var y1 = (Math.Sin(j) * radiusCorrective) + center.Y;

                                pntList.Add(new PuntoDueD((float)x1, (float)y1)); // riporto a double
                            }


                        }

                        /*
                         * Devo controllare che punto normale non sia maggiore del punto finale ,
                         * magari devo guardare in termini di angolo
                         */
                        // qui inserisco il punto normale a ultimo punto
                        //var x2 = (Math.Cos(endAngle - angleMinIncrement) * radius) + center.X;
                        //var y2 = (Math.Sin(endAngle - angleMinIncrement) * radius) + center.Y;

                        //var addPnt2 = new PuntoDueD(x2, y2);
                        //pntList.Add(addPnt2);
                        //var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

                        //normalAngleEndPnt += Math.PI;

                        //var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, arc.End);

                        //pntList.Add(new PuntoDueD((float)normalEndPnt.X, (float)normalEndPnt.Y));

                    }

                    // In ultimo aggiungo endpoint
                    // serve che sia moltiplicato per radisu anche endpoinr
                    //var endX = (Math.Cos(endAngle) * radius) + center.X;
                    //var endY = (Math.Sin(endAngle) * radius) + center.Y;

                    var endPnt = new PuntoDueD((float)arc.End.X, (float)arc.End.Y, true);


                    var previousPoint = pntList.LastOrDefault();
                    previousPoint.AngleStart = true;

                    ArcPatch.AddPunto(previousPoint,
                                      new AuxArc()
                                          {
                                              ArcCenter = new PuntoDueD(arc.Center.X, arc.Center.Y),
                                              PreviousPoint = previousPoint,
                                              EndPoint = endPnt,
                                              ArcRadius = arc.Radius,
                                              ClockWise = arc.ClockWise,
                                              CurrentAngle = endAngle,
                                              IsAngleStarting = false,
                                              //NormalAngle = normalAngleEndPnt,
                                          });

                    pntList.Add(endPnt);

                    //  ArcPatch.AddPunto(endPnt);
                }
            }

            return pntList;
        }

        /// <summary>
        /// Calcola la lunghezza del segmento tangente che uso per costruire correttamente archi
        /// </summary>
        /// <param name="angleIncrement"></param>
        /// <param name="arcRadius"></param>
        /// <returns></returns>
        private static double GetNormalLength(float angleIncrement, double arcRadius)
        {
            // Per rif della trigonometri
            // dove b è la lunghezza che devo trovare
            // beta = angolo incremento
            // c = raggio
            var b = Math.Tan(angleIncrement) * arcRadius;

            return b;
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
            var lastPnt = GetLastPoint();

            if (lastPnt == null)
            {
                _startPoint = new Point2D(pnt);
                return;
            }

            //var lastPnt = GetLastPoint();

            //if (lastPnt == null)
            //    throw new NullReferenceException();

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

            if (lastElement != null)
                return lastElement.GetLastPnt();

            if (_startPoint != null)

                return new Point2D(_startPoint);

            return null;

        }

        /// <summary>
        /// </summary>
        private class ParsedArc
        {
            /// <summary>
            /// Ottiene il raggio dell'arco ,dopo che ho trovato un segmento valido
            /// </summary>
            /// <param name="segmentParsed">Segmento che risiede su CFR</param>
            /// <param name="centerArc">Centro Arco.</param>
            /// <returns></returns>
            private static double GetRadius(Line2D segmentParsed, Point2D centerArc)
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


            /// <summary>
            /// Metodo utilizzato per vedere se è possibile cominciare arco.
            /// Viene utilizzato solamente in partenza visto che il secondo punto non ha lo stesso circocentro..
            /// </summary>
            /// <param name="p1">Punto Dovra essere p Iniziale</param>
            /// <param name="p2">Punto collineare al primo, aggiunto apposta , questo deve essere controllato solamente angolo.</param>
            /// <param name="p3">Punto 2 effettivo</param>
            /// <param name="p4">Punto 3 effettivo</param>
            /// <param name="p5">Nuovo Punto..</param>
            /// <param name="externBufferPnt"></param>
            /// <returns></returns>
            internal bool TryStartArc(Point2D p1, Point2D p2, Point2D p3, Point2D p4, ref List<Point2D> externBufferPnt)
            {
                /*
                 * per come lo costruisco ora 
                 * 
                 * il secondo punto deve essere allineato con 1 punto, quindi non sarà in centro con altri.
                 * 
                 * dovra cmq essere nel +- angolo 5 gradi..
                 */
                if (IsOk(p1, p2, p3, p4))
                {

                    bufferPnt.Add(p1);
                    bufferPnt.Add(p2);
                    bufferPnt.Add(p3);
                    bufferPnt.Add(p4);

                    externBufferPnt.Clear();

                    /*
                     * setto centro arco e raggio trovato
                     * 
                     * se aggiungo punti e non sono compatibili con questo arco e raggio li scarico.
                     */

                    //var midPoint1 = GeometryHelper.GetMidPoint(p1, p);
                    //var midPoint2 = GeometryHelper.GetMidPoint(p2, p3);
                    //var midPoint3 = GeometryHelper.GetMidPoint(p3, p4);

                    Center = GeometryHelper.Circocentro(p2, p3, p4);

                    Radius = GetRadius(new Line2D() { Start = p3, End = p4 }, Center);

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
            /// <param name="newPoint"></param>
            /// <param name="flag">Flag = 1, Controlla se 2nd punto è la "normale" ; Flag = 2 , Controlla se penultimo punto o ultimo è la "normale" ; Default = 0 Niente Controlli</param>
            /// <returns></returns>
            private static bool IsOk(Point2D p1, Point2D p2, Point2D p3, Point2D newPoint)
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

                /*
                 * Se sono sulla stessa circonferenza il punto viene aggiunto nel buffer
                 */
                // questi 2 metodi vanno nested, non possono andare singolarmente
                if (IsInArcNoMidPoint(p1, p2, p3, newPoint, out presuntCenterPoint))
                {
                    if (SonoViciniAngolarmente(p1, p2, p3, newPoint, presuntCenterPoint))
                    {
                        return true;
                    }
                }

                return false;
            }


            /// <summary>
            /// Determina se i punti sono vicini angolarmente.
            /// </summary>
            /// <param name="p1"></param>
            /// <param name="p2"></param>
            /// <param name="p3"></param>
            /// <param name="p4"></param>
            /// <param name="newPoint"></param>
            /// <param name="center"></param>
            /// <returns></returns>
            private static bool SonoViciniAngolarmente(Point2D p1, Point2D p2, Point2D p3, Point2D newPoint, Point2D center)
            {
                var angle1 = Math.Atan2(p1.Y - center.Y, p1.X - center.X);
                var angle2 = Math.Atan2(p2.Y - center.Y, p2.X - center.X);
                var angle3 = Math.Atan2(p3.Y - center.Y, p3.X - center.X);
                var angle4 = Math.Atan2(newPoint.Y - center.Y, newPoint.X - center.X);

                /* qui c'è problema , se angolo è 628 e altro è 6.33 */
                var d1 = GeometryHelper.GetMinAngleDifference(angle2, angle1);
                var d2 = GeometryHelper.GetMinAngleDifference(angle3, angle2);
                var d3 = GeometryHelper.GetMinAngleDifference(angle4, angle3);

                var distanceTollerance = 2 / 57.3;

                var rslt = (Math.Abs(d1) <= distanceTollerance && Math.Abs(d2) <= distanceTollerance && Math.Abs(d3) <= distanceTollerance);

                if (!rslt)
                {

                }
                return rslt;

                //var distance1 = Distance(p1, p2);
                //var distance2 = Distance(p2, p3);
                //var distance3 = Distance(p3, p4);

                //return (distance1 <= distanceTollerance && distance2 <= distanceTollerance && distance3 <= distanceTollerance);
            }

            /// <summary>
            /// Metodo Per verificare che 4 punti siano sullo stesso arco ,
            /// </summary>
            /// <param name="firstPoint"></param>
            /// <param name="secondPoint"></param>
            /// <param name="thirdPoint"></param>
            /// <param name="fourthPnt"></param>
            /// <returns></returns>
            private static bool IsInArcNoMidPoint(Point2D firstPoint, Point2D secondPoint, Point2D thirdPoint, Point2D fourthPnt, out Point2D circoCentre)
            {
                circoCentre = GeometryHelper.Circocentro(firstPoint, secondPoint, thirdPoint);

                var newCircoCentro = GeometryHelper.Circocentro(secondPoint, thirdPoint, fourthPnt);

                if (newCircoCentro != null && circoCentre != null)
                {
                    var distance = GeometryHelper.Distance(circoCentre, newCircoCentro);

                    if (distance.HasValue)
                    {
                        var approxRadius = GeometryHelper.Distance(firstPoint, circoCentre);
                        // anche questo non ok
                        // fare che man mano che prendo punti faccio cerchio . e guardo se il punto in questione puo stare dentro questo cerchio..
                        var tolleranceProportional = approxRadius / 5;

                        var rslt = Math.Abs(distance.Value) <= tolleranceProportional;

                        if (!rslt)
                        {
                        }
                        return rslt;
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

            /// <summary>
            /// "Mangia" punto, e mi dice se è potenzialmente da inserire nell'arco..
            /// 
            /// Il punto potrebbe essere il punto tangente, 
            /// </summary>
            /// <param name="point2D"></param>
            /// <returns></returns>
            internal bool EatPoint(Point2D point2D)
            {
                //                 se il punto e uguale al precedente lo skippo
                // altrimenti calcola male 
                var prevPoint = bufferPnt.LastOrDefault();

                if (prevPoint != null)
                {
                    if (prevPoint.Equals(point2D, 8))
                    {
                        // Do Nothing == Lo skippo
                        return true;
                    }
                }
                var x = point2D.X;

                var rslt = IsOk(point2D);
                if (!rslt)
                {
                    IsOk(point2D);
                    //IsOk(point2D);

                }
                if (rslt)
                {
                    //var last2Element = bufferPnt.GetRange(bufferPnt.Count - 2, 2);

                    //var nuovoCentro = GeometryHelper.Circocentro(last2Element[0], last2Element[1], point2D);

                    //Center = GeometryHelper.GetMidPoint(Center, nuovoCentro);

                    // faccio nuovo centro e 
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

            /// <summary>
            /// Se riesce a creare arco ok , altrimenti svuota il buffer di punti
            /// </summary>
            /// <returns></returns>
            private IEnumerable<Point2D> GetBufferPointElaborated()
            {
                /* controllo che sia possibile creare arco.*/

                // questo metodo viene chiamato su chiusura arco.
                // setto arco chiuso
                IsArcStarted = false;


                if (bufferPnt.Count < 5)
                    return bufferPnt;

                /*
                 * rimuovo 2nd e penultimo in quanto sono quelli aggiunti apposta per riuscire a ricreare arco con offset.
                 */

                bufferPnt.Remove(bufferPnt[1]);
                bufferPnt.RemoveAt(bufferPnt.Count - 2);

                // cerco di evitare di fare media ..
                return bufferPnt;
            }

            /// <summary>
            /// Se riesce a creare arco ok , altrimenti svuota il buffer di punti
            /// </summary>
            /// <returns></returns>
            public object CreateArc()
            {
                /* controllo che sia possibile creare arco.*/

                // questo metodo viene chiamato su chiusura arco.
                // setto arco chiuso
                IsArcStarted = false;

                if (bufferPnt.Count < 7)
                    return bufferPnt;


                var firstPnt = bufferPnt.FirstOrDefault();
                var lastPnt = bufferPnt.LastOrDefault();

                /*
                 * Non prendo primo e ultimo punto,
                 */
                var second = bufferPnt[1];
                var secondLast = bufferPnt[bufferPnt.Count - 2];

                /*
                 * rimuovo 2nd e penultimo in quanto sono quelli aggiunti apposta per riuscire a ricreare arco con offset.
                 */
                // cerco di evitare di fare media ..

                //  var first = bufferPnt.First();

                var medianIndex = (int)Math.Ceiling((decimal)bufferPnt.Count / 2);
                var median = bufferPnt[medianIndex];

                //    var last = bufferPnt.Last();

                // forse è ancora buona cosa usare la media.. per la media dei circocentri..
                var circocentro = GeometryHelper.Circocentro(second, median, secondLast);

                Point2D center;
                double radius;

                var rsl = Circle(second, median, secondLast, out center, out radius);

                if (!rsl)
                {
                    return bufferPnt;
                    //   throw new Exception("ParsedArc.CreateArc");
                }

                var arc = new Arc2D
                {
                    Center = new Point2D(center),
                    Start = new Point2D(firstPnt),
                    End = new Point2D(lastPnt),
                    Radius = radius,
                };

                arc.ClockWise = IsClockWise(center);

                return arc;
            }
            private static bool Circle(Point2D p1, Point2D p2, Point2D p3, out Point2D center, out double radius)
            {
                double t = p2.X * p2.X + p2.Y * p2.Y;
                double bc = (p1.X * p1.X + p1.Y * p1.Y - t) / 2.0;
                double cd = (t - p3.X * p3.X - p3.Y * p3.Y) / 2.0;
                double det = (p1.X - p2.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p2.Y);

                if (Math.Abs(det) > 1.0e-6) // Determinant was found. Otherwise, radius will be left as zero.
                {
                    det = 1 / det;
                    double x = (bc * (p2.Y - p3.Y) - cd * (p1.Y - p2.Y)) * det;
                    double y = ((p1.X - p2.X) * cd - (p2.X - p3.X) * bc) * det;
                    double r = Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y));

                    center = new Point2D(x, y);
                    radius = r;

                    return true;
                }
                radius = 0;
                center = null;

                return false;
            }

            //01/09/2011
            ///// <summary>
            ///// Se riesce a creare arco ok , altrimenti svuota il buffer di punti
            ///// </summary>
            ///// <returns></returns>
            //public object CreateArc()
            //{
            //    /* controllo che sia possibile creare arco.*/

            //    // questo metodo viene chiamato su chiusura arco.
            //    // setto arco chiuso
            //    IsArcStarted = false;

            //    /*
            //     * 
            //     */


            //    if (bufferPnt.Count < 8)
            //        return bufferPnt;

            //    //var secondPoint = bufferPnt[1];
            //    //var penultimoPoint = bufferPnt[bufferPnt.Count - 2];

            //    var firstPnt = bufferPnt.FirstOrDefault();
            //    var lastPnt = bufferPnt.LastOrDefault();

            //    /*
            //     * considerare anche di fare la media del buffer..
            //     */
            //    //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

            //    /*
            //     * Un 'altro problema che si presenta è in caso ci sia cfr.
            //     * 
            //     * qui i punti iniziale  e finale coincidono,
            //     * 
            //     * todo . tenere conto anche delle spirali, permettere massimo 1 giro 
            //     * 
            //     * per ovviare a questo problema provero a prendere il secondo , meta e penultimo
            //     * cosi i punti estremi non coincideranno
            //     * 03/08/2011
            //     * se faccio cosi su arco di 4 punti il midPoint e penultimo point coincidono
            //     */

            //    /*
            //     * Come spiegato nel commento della classe parsedArc, mi servono almeno arco di 8 punti per avere 
            //     * valori attendibili.
            //     * 
            //     * Nonostante questo il valore del circocentro non è perfetto, intendo quindi fare una media di tutti i valori
            //     * contenuti dentro il buffer
            //     */

            //    // - I  primi 2 e ultimi 2 non mi interessano,
            //    // - Prendo 3 Punti alla volta , quindi mi devo fermare a -6 rispetto al count
            //    // es.  su buffer di 80 , mi devo fermare a i = 73..
            //    var counter = 0;
            //    Point2D circoCentro = null;
            //    var raggio = 00.0d;

            //    /*
            //     * rimuovo 2nd e penultimo in quanto sono quelli aggiunti apposta per riuscire a ricreare arco con offset.
            //     * 

            //     */

            //    bufferPnt.RemoveAt(0);
            //    bufferPnt.RemoveAt(0);
            //    bufferPnt.RemoveAt(0);
            //    bufferPnt.RemoveAt(bufferPnt.Count - 1);
            //    bufferPnt.RemoveAt(bufferPnt.Count - 1);
            //    bufferPnt.RemoveAt(bufferPnt.Count - 1);


            //    for (int i = 0; i < bufferPnt.Count - 7; i++)
            //    {
            //        counter++;
            //        var midPoint1 = GeometryHelper.GetMidPoint(bufferPnt[i + 2], bufferPnt[i + 3]);
            //        var midPoint2 = GeometryHelper.GetMidPoint(bufferPnt[i + 3], bufferPnt[i + 4]);
            //        var midPoint3 = GeometryHelper.GetMidPoint(bufferPnt[i + 4], bufferPnt[i + 5]);

            //        //var midPoint = bufferPnt[(int)bufferPnt.Count / 2];
            //        var nCircocentro = GeometryHelper.Circocentro(midPoint1, midPoint2, midPoint3);

            //        var nCircocentro0 = GeometryHelper.Circocentro(bufferPnt[2], bufferPnt[29], bufferPnt[34]);
            //        var nCircocentro1 = GeometryHelper.Circocentro(bufferPnt[3], bufferPnt[30], bufferPnt[35]);
            //        var nCircocentro2 = GeometryHelper.Circocentro(bufferPnt[4], bufferPnt[31], bufferPnt[36]);

            //        if (circoCentro == null)
            //            circoCentro = nCircocentro; // prima inizializzazione


            //        var l = GeometryHelper.GetMidPoint(nCircocentro0, nCircocentro1);
            //        l = GeometryHelper.GetMidPoint(l, nCircocentro2);

            //        // faccio media circocentro
            //        circoCentro = GeometryHelper.GetMidPoint(circoCentro, nCircocentro);

            //        Point2D ce;
            //        double ar1c;

            //        var rsl = Circle(bufferPnt[2], bufferPnt[29], bufferPnt[34], out ce, out ar1c);
            //        rsl = Circle(bufferPnt[i], bufferPnt[i + 1], bufferPnt[i + 2], out ce, out ar1c);

            //        circoCentro = l;
            //        // mi servono 2 punti consecutivi
            //        raggio += GeometryHelper.Distance(bufferPnt[i], circoCentro).Value;
            //        raggio += GeometryHelper.Distance(bufferPnt[i + 1], circoCentro).Value;
            //        raggio += GeometryHelper.Distance(bufferPnt[i + 2], circoCentro).Value;

            //    }

            //    raggio = raggio / (counter * 3); // media

            //    if (raggio == 0)
            //        return null;

            //    var arc = new Arc2D
            //    {
            //        Center = new Point2D(circoCentro),
            //        Start = new Point2D(firstPnt),
            //        End = new Point2D(lastPnt),
            //        Radius = raggio,
            //    };

            //    arc.ClockWise = IsClockWise(circoCentro);

            //    return arc;
            //}
            //public static bool Circle(Point2D p1, Point2D p2, Point2D p3, out Point2D center, out double radius)
            //{
            //    double t = p2.X * p2.X + p2.Y * p2.Y;
            //    double bc = (p1.X * p1.X + p1.Y * p1.Y - t) / 2.0;
            //    double cd = (t - p3.X * p3.X - p3.Y * p3.Y) / 2.0;
            //    double det = (p1.X - p2.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p2.Y);

            //    if (Math.Abs(det) > 1.0e-6) // Determinant was found. Otherwise, radius will be left as zero.
            //    {
            //        det = 1 / det;
            //        double x = (bc * (p2.Y - p3.Y) - cd * (p1.Y - p2.Y)) * det;
            //        double y = ((p1.X - p2.X) * cd - (p2.X - p3.X) * bc) * det;
            //        double r = Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y));

            //        center = new Point2D(x, y);
            //        radius = r;

            //        return true;
            //    }
            //    radius = 0;
            //    center = null;

            //    return false;
            //}


            /// 22/08/2011
            /// Ora per costruiore archi immetto gia i putno che andranno sulla cfr .
            /// non mi serfve più trovare i punti mediani..
            ///// <summary>
            ///// Se riesce a creare arco ok , altrimenti svuota il buffer di punti
            ///// </summary>
            ///// <returns></returns>
            //public object CreateArc()
            //{
            //    /* controllo che sia possibile creare arco.*/

            //    // questo metodo viene chiamato su chiusura arco.
            //    // setto arco chiuso
            //    IsArcStarted = false;

            //    /*
            //     * ora non importa più che prendo angolo mediano.
            //     * 
            //     * quindi i punti possono essre anche meno di 8
            //     * 
            //     * in ogni caso è bene levare i primi 2 punti e ultimi 2 dalla conta in quanto prendono valori sballati..
            //     * 
            //     * il primo e ultimo perche dipende dalla bisettrice con elemento adiacente
            //     * il secondo e penultimo sono la normale per fare si che il prog si generato correttamente
            //     */

            //    if (bufferPnt.Count < 8)
            //        return bufferPnt;

            //    //var secondPoint = bufferPnt[1];
            //    //var penultimoPoint = bufferPnt[bufferPnt.Count - 2];

            //    var firstPnt = bufferPnt.FirstOrDefault();
            //    var lastPnt = bufferPnt.LastOrDefault();

            //    /*
            //     * considerare anche di fare la media del buffer..
            //     */
            //    //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

            //    /*
            //     * Un 'altro problema che si presenta è in caso ci sia cfr.
            //     * 
            //     * qui i punti iniziale  e finale coincidono,
            //     * 
            //     * todo . tenere conto anche delle spirali, permettere massimo 1 giro 
            //     * 
            //     * per ovviare a questo problema provero a prendere il secondo , meta e penultimo
            //     * cosi i punti estremi non coincideranno
            //     * 03/08/2011
            //     * se faccio cosi su arco di 4 punti il midPoint e penultimo point coincidono
            //     */

            //    /*
            //     * Come spiegato nel commento della classe parsedArc, mi servono almeno arco di 8 punti per avere 
            //     * valori attendibili.
            //     * 
            //     * Nonostante questo il valore del circocentro non è perfetto, intendo quindi fare una media di tutti i valori
            //     * contenuti dentro il buffer
            //     */

            //    // - I  primi 2 e ultimi 2 non mi interessano,
            //    // - Prendo 3 Punti alla volta , quindi mi devo fermare a -6 rispetto al count
            //    // es.  su buffer di 80 , mi devo fermare a i = 73..
            //    var counter = 0;
            //    Point2D circoCentro = null;
            //    var raggio = 00.0d;
            //    for (int i = 0; i < bufferPnt.Count - 7; i++)
            //    {
            //        counter++;
            //        var midPoint1 = GeometryHelper.GetMidPoint(bufferPnt[i + 2], bufferPnt[i + 3]);
            //        var midPoint2 = GeometryHelper.GetMidPoint(bufferPnt[i + 3], bufferPnt[i + 4]);
            //        var midPoint3 = GeometryHelper.GetMidPoint(bufferPnt[i + 4], bufferPnt[i + 5]);

            //        //var midPoint = bufferPnt[(int)bufferPnt.Count / 2];
            //        var nCircocentro = GeometryHelper.Circocentro(midPoint1, midPoint2, midPoint3);

            //        if (circoCentro == null)
            //            circoCentro = nCircocentro; // prima inizializzazione

            //        // faccio media circocentro
            //        circoCentro = GeometryHelper.GetMidPoint(circoCentro, nCircocentro);

            //        // mi servono 2 punti consecutivi
            //        raggio += GetRadius(new Line2D() { Start = bufferPnt[i + 2], End = bufferPnt[i + 3] }, circoCentro);

            //        // non ho miglioramenti con 3 inserimenti per raggio
            //        raggio += GetRadius(new Line2D() { Start = bufferPnt[i + 3], End = bufferPnt[i + 4] }, circoCentro);
            //        raggio += GetRadius(new Line2D() { Start = bufferPnt[i + 4], End = bufferPnt[i + 5] }, circoCentro);

            //    }

            //    raggio = raggio / (counter * 3); // media

            //    if (raggio == 0)
            //        return null;

            //    var arc = new Arc2D
            //    {
            //        Center = new Point2D(circoCentro),
            //        Start = new Point2D(firstPnt),
            //        End = new Point2D(lastPnt),
            //        Radius = raggio,
            //    };

            //    arc.ClockWise = IsClockWise(circoCentro);

            //    return arc;
            //}

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

                if (bufferPnt.Count < 3)
                    throw new Exception("ParsedArc");

                var last4 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

                var rslt = IsOk(last4[0], last4[1], last4[2], point2D);

                if (!rslt)
                {
                    rslt = IsOk(last4[0], last4[1], last4[2], point2D);

                }


                return rslt;
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



            internal Point2D GetLastPoint()
            {
                return bufferPnt.LastOrDefault();
            }
        }


        /// <summary>
        /// -Trasformo il profilo in lista di punti
        /// 
        /// -- il problema è che non considero raggi descritti con meno di 4 punti.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arcTollerance"></param>
        /// <param name="distanceTollerance"></param>
        /// <param name="createArc"></param>
        /// <returns></returns>
        public static Profile2D ParseArcIn2DProfile(Profile2D source, double arcTollerance = ArcParsingCenterTollerance, double distanceTollerance = ArcParsingDistanceTollerance, bool createArc = true)
        {
            /*
             * se ci riesco è meglio levare anche i secondo e penultimo punto per archi 
             */
            // Se c'è un arco nel profilo non faccio niente..
            if (source.Source.Any(o => o is Arc2D))
                return source;

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

                if (bufferPnt.Count >= 4)
                {
                    var lastFourElement = bufferPnt.GetRange(bufferPnt.Count - 4, 4);

                    var parsedArc = new ParsedArc();

                    if (parsedArc.TryStartArc(lastFourElement[0], lastFourElement[1], lastFourElement[2],
                                              lastFourElement[3], ref bufferPnt))
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
                                var arc = parsedArc.CreateArc();

                                profile.AddEntity(arc);

                                // bufferPnt.Clear();

                                // inserisco ultimo punto dell'arco e ultimo punto non nell'arco 

                                bufferPnt.Add(parsedArc.GetLastPoint());
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
            profile.ToolPathEndPoint = source.ToolPathEndPoint;
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

        //private List<Point2D> GetPointFromLineList()
        //{
        //    var lineL = Source.Cast<Line2D>();

        //    var rslt = new List<Point2D>();

        //    foreach (var line2D in lineL)
        //    {
        //        if (line2D == Source.First())
        //            rslt.Add(new Point2D(line2D.Start));

        //        rslt.Add(new Point2D(line2D.End));
        //    }

        //    return rslt;
        //}

        /// <summary>
        /// Se il profilo è un cerchio restituisce true
        /// </summary>
        /// <returns></returns>
        private bool IsCircle(double offsetDistance, bool counterClockwise, out Arc2D arc2D)
        {
            var ce = Source.FirstOrDefault();

            if (Source.Count == 1 && ce is Arc2D)
            {
                var arc = ce as Arc2D;

                if (arc.IsCircle())
                {
                    // Lo setto per convenzione 
                    arc.ClockWise = true;

                    var radiusEnd = arc.Radius + offsetDistance;

                    if (radiusEnd <= 0)
                    {
                        arc2D = null;
                        return false;
                    }

                    arc2D = GeometryHelper.GetParallel(arc, offsetDistance, false);

                    arc2D.ClockWise = !counterClockwise;

                    return true;
                }
            }

            arc2D = null;
            return false;
        }


        //27/08/2011
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="offsetDistance"></param>
        ///// <param name="counterClockwise"></param>
        ///// <param name="parseCircle">Per ora è abilitato solo nelle operazioni di tasca, per le esterne potrbbe creare conflitti con trimPath..</param>
        ///// <returns></returns>
        //public List<Profile2D> Offset(double offsetDistance, bool counterClockwise, bool parseCircle = false)
        //{
        //    var rslt = new List<ContenitoreLivello>();

        //    if (parseCircle)
        //    {
        //        Arc2D arc2D;
        //        if (IsCircle(offsetDistance, counterClockwise, out arc2D))
        //        {
        //            if (arc2D == null)
        //                return null;

        //            var profile = new Profile2D();
        //            profile.AddEntity(arc2D);

        //            return new List<Profile2D>() { profile };
        //        }
        //    }

        //    var pntList = GetPointList(offsetDistance);

        //    int num3 = 0;

        //    if (pntList.Count >= 3)
        //    {
        //        ClassO classO = new ClassO();
        //        for (int j = 0; j < pntList.Count; j++)
        //        {
        //            classO.Add(new ForseLineaClassA(pntList[j], pntList[(j + 1) % pntList.Count]));
        //        }
        //        List<PuntoDueD> list6 = new List<PuntoDueD>();
        //        List<int> list7 = new List<int>();
        //        List<int> list8 = new List<int>();
        //        classO.MethodA(list6, list7, list8);
        //        num3 += list6.Count;
        //    }

        //    bool flag2 = false;
        //    if ((num3 > 0))
        //    {
        //        flag2 = true;
        //        /* devo resituire che eccezzione di profilo autointersecante */
        //        return null;

        //    }
        //    var n2 = new ClassN();

        //    n2.MathodA1(pntList, false); // questo c'era.
        //    //var p = new PuntoDueD(0,0);
        //    //n2.d(p);

        //    //n2.r(p);

        //    //n2.MethodA(linea);
        //    //n2.MethodB(linea1);
        //    //n2.MethodC(linea2);

        //    // todo : 
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
        //    var polygonalShape1 = new PolygonalShape(n2);

        //    var chamferAngle = Math.PI / 4;
        //    //  chamferAngle = 0;

        //    OffsetPathMainClass.Elaborate(new ClassC(polygonalShape1), offsetDistance, 1, chamferAngle, rslt);

        //    /*
        //     * da polygonal shape a classe utilizzabile nel processp
        //     */
        //    if (!snuffRslt(rslt))
        //        return null;

        //    var offsetResultPointProfile = new List<List<Point2D>>();



        //    /*
        //     * Gli ho chiesto 1 offset, se mi restituisce un numero diverso da 0 o 1 , probabilmente c'è errore
        //     */
        //    Debug.Assert(rslt.Count <= 1);

        //    if (rslt.Count == 0)
        //        return null;

        //    for (int i = 0; i < rslt.Count; i++)
        //    {
        //        var contenitoreLivello = rslt[i];

        //        /*
        //         * Possono esserci più profili per lo stesso livello di offset,
        //         * li devo tenere separati, 
        //         */

        //        for (int m = 0; m < contenitoreLivello.Count; m++)
        //        {
        //            var pntListRslt = new List<Point2D>();

        //            var polygonalShape = contenitoreLivello[m];

        //            var a0 = polygonalShape.GetExternPolygon();

        //            for (int k = 0; k < a0.ac().Count; k++)
        //            {
        //                if (a0.ac()[k] is ForseLineaClassA)
        //                {
        //                    var classA0 = a0.ac()[k] as ForseLineaClassA;


        //                    var puntoDueD = classA0.h();
        //                    var x2 = classA0.i();

        //                    if (k == 0)
        //                        pntListRslt.Add(new Point2D(puntoDueD.X, puntoDueD.Y));

        //                    pntListRslt.Add(new Point2D(x2.X, x2.Y));

        //                }
        //                else if (a0.ac()[i] is ForseArco2ClassQ) // forse classq è la chiave
        //                {
        //                    // this.MethodA(A_0.ac()[i] as ClassQ, A_1, A_2);
        //                }
        //            }

        //            offsetResultPointProfile.Add(pntListRslt);
        //        }
        //    }


        //    var profileResulList = new List<Profile2D>();


        //    foreach (var singleProfilePoints in offsetResultPointProfile)
        //    {
        //        //foreach (var singleProfilePoint in singleProfilePoints)
        //        //{
        //        //    singleProfilePoint.X = singleProfilePoint.X / molFact;
        //        //    singleProfilePoint.Y = singleProfilePoint.Y / molFact;
        //        //}

        //        var list = new Profile2D();

        //        if (counterClockwise)
        //        {
        //            for (int i = singleProfilePoints.Count - 1; i >= 0; i--)
        //            {
        //                list.AddPnt(singleProfilePoints[i]);
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < singleProfilePoints.Count; i++)
        //            {
        //                list.AddPnt(singleProfilePoints[i]);
        //            }
        //        }

        //        profileResulList.Add(list);

        //    }


        //    return profileResulList;
        //}


        /// <summary>
        /// Riguardo le prestazioni:
        /// Lo scheletro per fare offset viene creato una sola volta.
        /// Se io trovo quel metodo , per poi una volta creato scheletro chiedere offset a determinata distanza risparmio calcoli.
        /// Nella maniera che eseguo adesso lo skeletro viene creato tutte le volte che chiedo offset.
        /// </summary>
        /// <param name="offsetDistance"></param>
        /// <param name="counterClockwise"></param>
        /// <returns></returns>
        public List<Profile2D> Offset(double offsetDistance, bool counterClockwise)
        {
            var rslt = new List<ContenitoreLivello>();

            //if (parseCircle)
            //{
            //    Arc2D arc2D;
            //    if (IsCircle(offsetDistance, counterClockwise, out arc2D))
            //    {
            //        if (arc2D == null)
            //            return null;

            //        var profile = new Profile2D();
            //        profile.AddEntity(arc2D);

            //        return new List<Profile2D>() { profile };
            //    }
            //}

            var pntList = GetPointList();

            //  pntList  =GeometryHelper.PolygonHelper.OrientPolygonClockwise(pntList);

            // decommentare
            //int num3 = 0;

            //if (pntList.Count >= 3)
            //{
            //    ClassO_List_LineArco classO = new ClassO_List_LineArco();
            //    for (int j = 0; j < pntList.Count; j++)
            //    {
            //        classO.Add(new ForseLineaClassA(pntList[j], pntList[(j + 1) % pntList.Count]));
            //    }
            //    List<PuntoDueD> list6 = new List<PuntoDueD>();
            //    List<int> list7 = new List<int>();
            //    List<int> list8 = new List<int>();
            //    classO.MethodA(list6, list7, list8);
            //    num3 += list6.Count;
            //}

            //bool flag2 = false;
            //if ((num3 > 0))
            //{
            //    flag2 = true;
            //    /* devo resituire che eccezzione di profilo autointersecante */
            //    return null;

            //}
            var n2 = new ClassN();

            n2.MathodA1(pntList, false); // questo c'era.
            //var p = new PuntoDueD(0,0);
            //n2.d(p);

            //n2.r(p);

            //n2.MethodA(linea);
            //n2.MethodB(linea1);
            //n2.MethodC(linea2);

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

            var chamferAngle = Math.PI / 3;
            //  chamferAngle = 0;
            OffsetPathMainClass.Elaborate(new ClassC(polygonalShape1), offsetDistance, 1, chamferAngle, rslt);

            /*
             * da polygonal shape a classe utilizzabile nel processp
             */
            if (!snuffRslt(rslt))
                return null;

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

                    for (int k = 0; k < a0.GetListaLineaArco().Count; k++)
                    {
                        if (a0.GetListaLineaArco()[k] is ForseLineaClassA)
                        {
                            var classA0 = a0.GetListaLineaArco()[k] as ForseLineaClassA;


                            var puntoDueD = classA0.h();
                            var x2 = classA0.i();

                            if (k == 0)
                                pntListRslt.Add(new Point2D(puntoDueD.X, puntoDueD.Y));

                            pntListRslt.Add(new Point2D(x2.X, x2.Y));

                        }
                        else if (a0.GetListaLineaArco()[i] is ForseArco2ClassQ) // forse classq è la chiave
                        {
                            // this.MethodA(A_0.ac()[i] as ClassQ, A_1, A_2);
                        }
                    }

                    /*
                     * Devo rimuovere i punti aggiunti apposta per parsed Arc punti del parsed arc..
                     */

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
                        list.AddPnt(new Point2D(singleProfilePoints[i].X, singleProfilePoints[i].Y));

                        //                        list.AddPnt(singleProfilePoints[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < singleProfilePoints.Count; i++)
                    {
                        list.AddPnt(new Point2D(singleProfilePoints[i].X, singleProfilePoints[i].Y));
                        //  list.AddPnt(singleProfilePoints[i]);
                    }
                }

                profileResulList.Add(list);

            }


            return profileResulList;
        }
        private bool snuffRslt(List<ContenitoreLivello> rslt)
        {
            var counter = 0;
            var c1 = 0;
            //if (rslt.Count == 0) return false;

            //var first = rslt.First();
            foreach (var VARIABLE in rslt)
            {
                c1++;
                foreach (var c in VARIABLE)
                {
                    var l = c.g(); // side counte
                    if (l < 4)
                        counter++;
                }

                if (counter > 6)
                    return false;
            }
            return true;

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

        /// <summary>
        /// Restituisce profilo per cava dritta.
        /// Disegnato in senso orario.
        /// Il primo semicerchio è quello a sx
        /// il secondo si sposta in x della lunghezza..
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="centerLenght"></param>
        /// <param name="centerLeft"></param>
        /// <returns></returns>
        public static Profile2D CreateCavaLineare(double radius, double centerLenght, Point2D centerLeft)
        {
            if (radius <= 0 || centerLenght <= 0) return new Profile2D();
            var primoArco = new Arc2D() { Center = new Point2D(centerLeft) };

            var secondoArco = new Arc2D() { Center = new Point2D(centerLeft) };
            secondoArco.Center.X += centerLenght;

            primoArco.Radius = secondoArco.Radius = radius;
            primoArco.ClockWise = secondoArco.ClockWise = true;

            primoArco.Start = GeometryHelper.GetCoordinate(Math.PI + Math.PI / 2, radius, primoArco.Center);
            primoArco.End = GeometryHelper.GetCoordinate(Math.PI / 2, radius, primoArco.Center);

            secondoArco.Start = GeometryHelper.GetCoordinate(Math.PI / 2, radius, secondoArco.Center);
            secondoArco.End = GeometryHelper.GetCoordinate(Math.PI + Math.PI / 2, radius, secondoArco.Center);

            var profile2D = new Profile2D();

            profile2D.AddEntity(primoArco);
            profile2D.AddPnt(secondoArco.Start);
            profile2D.AddEntity(secondoArco);
            profile2D.AddPnt(primoArco.Start);

            profile2D.SetPlotStyle();

            return profile2D;
        }


        public void SetPlotStyle(EnumPlotStyle plotStyle = EnumPlotStyle.Element)
        {
            foreach (var entity2D in _profile)
            {
                entity2D.PlotStyle = plotStyle;
            }
        }

        public void Multiply(Matrix3D matrix)
        {
            foreach (var entity2D in Source)
            {
                entity2D.Multiply(matrix);
            }
        }


        public bool TryAssign(Profile2D profile2D)
        {
            throw new NotImplementedException();
        }
    }
}

///* 07/09/2011 -- parsed arc backup..
// * /// <summary>
//        /// </summary>
//        private class ParsedArc
//        {
//            /// <summary>
//            /// Ottiene il raggio dell'arco ,dopo che ho trovato un segmento valido
//            /// </summary>
//            /// <param name="segmentParsed">Segmento che risiede su CFR</param>
//            /// <param name="centerArc">Centro Arco.</param>
//            /// <returns></returns>
//            private static double GetRadius(Line2D segmentParsed, Point2D centerArc)
//            {
//                /*
//                 * Prima la misura del raggio la misurava erroneamente tra centroArco e punto trovato.
//                 * 
//                 * La distanza che devo misurare e centro segmento e centro Arco
//                 * Per fare questo mi basta pitagora.
//                 * Dove Ipotenusa == Centro <> PuntoEstremoSegmento 
//                 * Cateto == lunghezzaSegmento /2
//                 */

//                var ipo = GeometryHelper.Distance(segmentParsed.Start, centerArc);

//                var cateto = segmentParsed.GetLenght() / 2;

//                if (cateto == 0 || ipo == null) return 0;

//                var radius = Math.Sqrt(Math.Pow(ipo.Value, 2) - Math.Pow(cateto, 2));

//                return radius;

//            }
//            private readonly double _arcTollerance;
//            private readonly double _distanceTollerance;

//            public ParsedArc(double arcTollerance = ArcParsingCenterTollerance, double distanceTollerance = ArcParsingDistanceTollerance)
//            {
//                _arcTollerance = arcTollerance;
//                _distanceTollerance = distanceTollerance;
//            }

//            /*
//             * aux class
//             * per creare parsedArc.
//             * 
//             * 
//             * prende in ingresso punti
//             * 
//             * mi dice se ok , se lo mangia. se no lo sputo 
//             * 
//             * quando sputa arco  è finito.
//             * 
//             * deve avere almeno 3 punti ok
//             * 
//             */


//            //public void ShutDown()
//            //{

//            //}

//            public bool IsArcStarted { get; set; }

//            private List<Point2D> bufferPnt = new List<Point2D>();

//            public double? Radius { get; set; }

//            public Point2D Center { get; set; }

//            //public bool TryStartArcPrev(Point2D p1, Point2D p2, Point2D p3, Point2D p4, ref List<Point2D> externBufferPnt)
//            //{

//            //    if (IsOk(p1, p2, p3, p4))
//            //    {

//            //        bufferPnt.Add(p1);
//            //        bufferPnt.Add(p2);
//            //        bufferPnt.Add(p3);
//            //        bufferPnt.Add(p4);

//            //        externBufferPnt.Clear();

//            //        /*
//            //         * setto centro arco e raggio trovato
//            //         * 
//            //         * se aggiungo punti e non sono compatibili con questo arco e raggio li scarico.
//            //         */
//            //        Center = GeometryHelper.Circocentro(p1, p2, p3);

//            //        Radius = GeometryHelper.Distance(Center, p1);

//            //        IsArcStarted = true;

//            //        return true;
//            //    }

//            //    return false;
//            //}


//            /// <summary>
//            /// Metodo utilizzato per vedere se è possibile cominciare arco.
//            /// Viene utilizzato solamente in partenza visto che il secondo punto non ha lo stesso circocentro..
//            /// </summary>
//            /// <param name="p1">Punto Dovra essere p Iniziale</param>
//            /// <param name="p2">Punto collineare al primo, aggiunto apposta , questo deve essere controllato solamente angolo.</param>
//            /// <param name="p3">Punto 2 effettivo</param>
//            /// <param name="p4">Punto 3 effettivo</param>
//            /// <param name="p5">Nuovo Punto..</param>
//            /// <param name="externBufferPnt"></param>
//            /// <returns></returns>
//            internal bool TryStartArc(Point2D p1, Point2D p2, Point2D p3, Point2D p4, ref List<Point2D> externBufferPnt)
//            {
//                /*
//                 * per come lo costruisco ora 
//                 * 
//                 * il secondo punto deve essere allineato con 1 punto, quindi non sarà in centro con altri.
//                 * 
//                 * dovra cmq essere nel +- angolo 5 gradi..
//                 */
//                if (IsOk(p1, p2, p3, p4))
//                {

//                    bufferPnt.Add(p1);
//                    bufferPnt.Add(p2);
//                    bufferPnt.Add(p3);
//                    bufferPnt.Add(p4);

//                    externBufferPnt.Clear();

//                    /*
//                     * setto centro arco e raggio trovato
//                     * 
//                     * se aggiungo punti e non sono compatibili con questo arco e raggio li scarico.
//                     */

//                    //var midPoint1 = GeometryHelper.GetMidPoint(p1, p);
//                    //var midPoint2 = GeometryHelper.GetMidPoint(p2, p3);
//                    //var midPoint3 = GeometryHelper.GetMidPoint(p3, p4);

//                    Center = GeometryHelper.Circocentro(p2, p3, p4);

//                    Radius = GetRadius(new Line2D() { Start = p3, End = p4 }, Center);

//                    IsArcStarted = true;

//                    return true;
//                }

//                return false;
//            }

//            /// <summary>
//            /// Controllo che i 4 punti forniti possano creare arco :
//            ///  Seguo iter seguente:
//            ///  I 4 punti risiedono sulla stessa cfr ?
//            ///     Prendo il raggio tra il primo e centro e in proporzione a questo raggio creo un valore di tolleranza
//            /// </summary>
//            /// <param name="p1"></param>
//            /// <param name="p2"></param>
//            /// <param name="p3"></param>
//            /// <param name="p4"></param>
//            /// <param name="newPoint"></param>
//            /// <param name="flag">Flag = 1, Controlla se 2nd punto è la "normale" ; Flag = 2 , Controlla se penultimo punto o ultimo è la "normale" ; Default = 0 Niente Controlli</param>
//            /// <returns></returns>
//            private static bool IsOk(Point2D p1, Point2D p2, Point2D p3, Point2D newPoint)
//            {
//                /*
//                 * prima dovrei vedere se sono sullo stesso arco , 
//                 * 
//                 * qui posso gia prendere raggio presunto.
//                 * 
//                 * e creare un valore di comparazione rapportato alle dimensioni del raggio.
//                 * 
//                 * , in secondo ambito controllo che la loro angolo sia vicino 
//                 * 
//                 * e terzo devono essere in ordine.
//                 * 
//                 * a partire dal primo l'angolo creato dal punto deve essere compatibile.
//                 */
//                // Devo creare archi in modo che da un punto e l'altro ci sia distanze definita

//                Point2D presuntCenterPoint;

//                /*
//                 * Se sono sulla stessa circonferenza il punto viene aggiunto nel buffer
//                 */
//                // questi 2 metodi vanno nested, non possono andare singolarmente
//                if (IsInArcNoMidPoint(p1, p2, p3, newPoint, out presuntCenterPoint))
//                {
//                    if (SonoViciniAngolarmente(p1, p2, p3, newPoint, presuntCenterPoint))
//                    {
//                        return true;
//                    }
//                }

//                return false;
//            }


//            /// <summary>
//            /// Determina se i punti sono vicini angolarmente.
//            /// </summary>
//            /// <param name="p1"></param>
//            /// <param name="p2"></param>
//            /// <param name="p3"></param>
//            /// <param name="p4"></param>
//            /// <param name="newPoint"></param>
//            /// <param name="center"></param>
//            /// <returns></returns>
//            private static bool SonoViciniAngolarmente(Point2D p1, Point2D p2, Point2D p3, Point2D newPoint, Point2D center)
//            {
//                var angle1 = Math.Atan2(p1.Y - center.Y, p1.X - center.X);
//                var angle2 = Math.Atan2(p2.Y - center.Y, p2.X - center.X);
//                var angle3 = Math.Atan2(p3.Y - center.Y, p3.X - center.X);
//                var angle4 = Math.Atan2(newPoint.Y - center.Y, newPoint.X - center.X);

//                /* qui c'è problema , se angolo è 628 e altro è 6.33 */
//                var d1 = GeometryHelper.GetMinAngleDifference(angle2, angle1);
//                var d2 = GeometryHelper.GetMinAngleDifference(angle3, angle2);
//                var d3 = GeometryHelper.GetMinAngleDifference(angle4, angle3);

//                var distanceTollerance = 1 / 57.3;

//                var rslt = (Math.Abs(d1) <= distanceTollerance && Math.Abs(d2) <= distanceTollerance && Math.Abs(d3) <= distanceTollerance);

//                if (!rslt)
//                {

//                }
//                return rslt;

//                //var distance1 = Distance(p1, p2);
//                //var distance2 = Distance(p2, p3);
//                //var distance3 = Distance(p3, p4);

//                //return (distance1 <= distanceTollerance && distance2 <= distanceTollerance && distance3 <= distanceTollerance);
//            }

//            /// <summary>
//            /// Metodo Per verificare che 4 punti siano sullo stesso arco ,
//            /// </summary>
//            /// <param name="firstPoint"></param>
//            /// <param name="secondPoint"></param>
//            /// <param name="thirdPoint"></param>
//            /// <param name="fourthPnt"></param>
//            /// <returns></returns>
//            private static bool IsInArcNoMidPoint(Point2D firstPoint, Point2D secondPoint, Point2D thirdPoint, Point2D fourthPnt, out Point2D circoCentre)
//            {
//                circoCentre = GeometryHelper.Circocentro(firstPoint, secondPoint, thirdPoint);

//                var newCircoCentro = GeometryHelper.Circocentro(secondPoint, thirdPoint, fourthPnt);

//                if (newCircoCentro != null && circoCentre != null)
//                {
//                    var distance = GeometryHelper.Distance(circoCentre, newCircoCentro);

//                    if (distance.HasValue)
//                    {
//                        var approxRadius = GeometryHelper.Distance(firstPoint, circoCentre);
//                        // anche questo non ok
//                        // fare che man mano che prendo punti faccio cerchio . e guardo se il punto in questione puo stare dentro questo cerchio..
//                        var tolleranceProportional = approxRadius/3;

//                        var rslt = Math.Abs(distance.Value) <= tolleranceProportional;

//                        return rslt;
//                    }
//                }

//                return false;
//            }



//            /// <summary>
//            /// Mi dice se i punti seguono la stessa direzione
//            /// </summary>
//            /// <param name="p1"></param>
//            /// <param name="p2"></param>
//            /// <param name="p3"></param>
//            /// <param name="p4"></param>
//            /// <returns></returns>
//            private static bool IsPointListOrder(Point2D p1, Point2D p2, Point2D p3, Point2D p4)
//            {
//                //var angle1 = GeometryHelper.GetPositiveAngle(p1.X, p1.Y);
//                //var angle2 = GeometryHelper.GetPositiveAngle(p2.X, p2.Y);
//                //var angle3 = GeometryHelper.GetPositiveAngle(p3.X, p3.Y);
//                //var angle4 = GeometryHelper.GetPositiveAngle(p4.X, p4.Y);

//                //if (angle1 < angle2)
//                //{
//                //    if (angle2 < angle3 && angle3 < angle4)
//                //        return true;

//                //    return false;
//                //}
//                //else if (angle1 > angle2)
//                //{
//                //    if (angle2 > angle3 && angle3 > angle4)
//                //        return true;
//                //    return false;
//                //}

//                //// a1 == a2
//                //return false;
//                return true;

//            }

//            /// <summary>
//            /// "Mangia" punto, e mi dice se è potenzialmente da inserire nell'arco..
//            /// 
//            /// Il punto potrebbe essere il punto tangente, 
//            /// </summary>
//            /// <param name="point2D"></param>
//            /// <returns></returns>
//            internal bool EatPoint(Point2D point2D)
//            {
//                // se il punto e uguale al precedente lo skippo
//                var prevPoint = bufferPnt.LastOrDefault();

//                if (prevPoint != null)
//                {
//                    if (prevPoint.Equals(point2D, 8))
//                    {
//                        // Do Nothing == Lo skippo
//                        return true;
//                    }
//                }
//                var rslt = IsOk(point2D);
//                if (!rslt)
//                {
//                 //   IsOk(point2D);
//                }
//                if (rslt)
//                {
//                    bufferPnt.Add(point2D);

//                    return true;
//                }

//                return false;
//            }
//            /*
//             * se arriva qui l'arco deve essere gia cominciato,
//             * quindi conterra raggio e centro.
//             */
//            //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

//            // * 
//            //if (bufferPnt.Count < 3)
//            //    throw new Exception("ParsedArc");

//            //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

//            //return IsOk(last3[0], last3[1], last3[2], point2D);
//            // */

//            //// Devo creare archi in modo che da un punto e l'altro ci sia distanze definita
//            //if (GeometryHelper.SonoVicini(last3[0], last3[1], last3[2], point2D, _distanceTollerance))
//            //{
//            //    //if (GeometryHelper.IsInArcV_2(last3[0], last3[1], last3[2], point2D, Center, Radius.Value, _arcTollerance))
//            //    if (GeometryHelper.IsInArc(last3[0], last3[1], last3[2], point2D, _arcTollerance))
//            //    {
//            //        bufferPnt.Add(point2D);

//            //        return true;
//            //    }
//            //}
//            //return false;

//            /// <summary>
//            /// Se riesce a creare arco ok , altrimenti svuota il buffer di punti
//            /// </summary>
//            /// <returns></returns>
//            private IEnumerable<Point2D> GetBufferPointElaborated()
//            {
//                /* controllo che sia possibile creare arco.*/

//                // questo metodo viene chiamato su chiusura arco.
//                // setto arco chiuso
//                IsArcStarted = false;


//                if (bufferPnt.Count < 5)
//                    return bufferPnt;

//                /*
//                 * rimuovo 2nd e penultimo in quanto sono quelli aggiunti apposta per riuscire a ricreare arco con offset.
//                 */

//                bufferPnt.Remove(bufferPnt[1]);
//                bufferPnt.RemoveAt(bufferPnt.Count - 2);

//                // cerco di evitare di fare media ..
//                return bufferPnt;
//            }

//            /// <summary>
//            /// Se riesce a creare arco ok , altrimenti svuota il buffer di punti
//            /// </summary>
//            /// <returns></returns>
//            public object CreateArc()
//            {
//                /* controllo che sia possibile creare arco.*/

//                // questo metodo viene chiamato su chiusura arco.
//                // setto arco chiuso
//                IsArcStarted = false;

//                /*
//                 * 01/09/2011
//                 * Al momento sto tenendo i punti con raggio modificato.
//                 * Quindi i punti risultanti dopo l'operazione di offset sono proprio quelli che ricadono sulla cfr.
//                 * Non deve trovare il punto mediano dei vari segmenti.
//                 * 
//                 * Devo inoltre eliminare il 2nd punto e n-1 punto, in quanto sono punti che ho creato apposta per creare correttamente arco
//                 * 
//                 * Il buffer quindi deve contenere almeno 5 elementi
//                 */

//                if (bufferPnt.Count < 7)
//                    return bufferPnt;


//                var firstPnt = bufferPnt.FirstOrDefault();
//                var lastPnt = bufferPnt.LastOrDefault();

//                /*
//                 * Non prendo primo e ultimo punto,
//                 */
//                var second = bufferPnt[1];
//                var secondLast = bufferPnt[bufferPnt.Count - 2];

//                /*
//                 * rimuovo 2nd e penultimo in quanto sono quelli aggiunti apposta per riuscire a ricreare arco con offset.
//                 */
//                // cerco di evitare di fare media ..

//                //  var first = bufferPnt.First();

//                var medianIndex = (int)Math.Ceiling((decimal)bufferPnt.Count / 2);
//                var median = bufferPnt[medianIndex];

//                //    var last = bufferPnt.Last();

//                // forse è ancora buona cosa usare la media.. per la media dei circocentri..
//                var circocentro = GeometryHelper.Circocentro(second, median, secondLast);

//                Point2D center;
//                double radius;

//                var rsl = Circle(second, median, secondLast, out center, out radius);

//                if (!rsl)
//                {
//                    return bufferPnt;
//                    //   throw new Exception("ParsedArc.CreateArc");
//                }

//                var arc = new Arc2D
//                {
//                    Center = new Point2D(center),
//                    Start = new Point2D(firstPnt),
//                    End = new Point2D(lastPnt),
//                    Radius = radius,
//                };

//                arc.ClockWise = IsClockWise(center);

//                return arc;
//            }
//            private static bool Circle(Point2D p1, Point2D p2, Point2D p3, out Point2D center, out double radius)
//            {
//                double t = p2.X * p2.X + p2.Y * p2.Y;
//                double bc = (p1.X * p1.X + p1.Y * p1.Y - t) / 2.0;
//                double cd = (t - p3.X * p3.X - p3.Y * p3.Y) / 2.0;
//                double det = (p1.X - p2.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p2.Y);

//                if (Math.Abs(det) > 1.0e-6) // Determinant was found. Otherwise, radius will be left as zero.
//                {
//                    det = 1 / det;
//                    double x = (bc * (p2.Y - p3.Y) - cd * (p1.Y - p2.Y)) * det;
//                    double y = ((p1.X - p2.X) * cd - (p2.X - p3.X) * bc) * det;
//                    double r = Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y));

//                    center = new Point2D(x, y);
//                    radius = r;

//                    return true;
//                }
//                radius = 0;
//                center = null;

//                return false;
//            }

//            //01/09/2011
//            ///// <summary>
//            ///// Se riesce a creare arco ok , altrimenti svuota il buffer di punti
//            ///// </summary>
//            ///// <returns></returns>
//            //public object CreateArc()
//            //{
//            //    /* controllo che sia possibile creare arco.*/

//            //    // questo metodo viene chiamato su chiusura arco.
//            //    // setto arco chiuso
//            //    IsArcStarted = false;

//            //    /*
//            //     * 
//            //     */


//            //    if (bufferPnt.Count < 8)
//            //        return bufferPnt;

//            //    //var secondPoint = bufferPnt[1];
//            //    //var penultimoPoint = bufferPnt[bufferPnt.Count - 2];

//            //    var firstPnt = bufferPnt.FirstOrDefault();
//            //    var lastPnt = bufferPnt.LastOrDefault();

//            //    /*
//            //     * considerare anche di fare la media del buffer..
//            //     */
//            //    //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

//            //    /*
//            //     * Un 'altro problema che si presenta è in caso ci sia cfr.
//            //     * 
//            //     * qui i punti iniziale  e finale coincidono,
//            //     * 
//            //     * todo . tenere conto anche delle spirali, permettere massimo 1 giro 
//            //     * 
//            //     * per ovviare a questo problema provero a prendere il secondo , meta e penultimo
//            //     * cosi i punti estremi non coincideranno
//            //     * 03/08/2011
//            //     * se faccio cosi su arco di 4 punti il midPoint e penultimo point coincidono
//            //     */

//            //    /*
//            //     * Come spiegato nel commento della classe parsedArc, mi servono almeno arco di 8 punti per avere 
//            //     * valori attendibili.
//            //     * 
//            //     * Nonostante questo il valore del circocentro non è perfetto, intendo quindi fare una media di tutti i valori
//            //     * contenuti dentro il buffer
//            //     */

//            //    // - I  primi 2 e ultimi 2 non mi interessano,
//            //    // - Prendo 3 Punti alla volta , quindi mi devo fermare a -6 rispetto al count
//            //    // es.  su buffer di 80 , mi devo fermare a i = 73..
//            //    var counter = 0;
//            //    Point2D circoCentro = null;
//            //    var raggio = 00.0d;

//            //    /*
//            //     * rimuovo 2nd e penultimo in quanto sono quelli aggiunti apposta per riuscire a ricreare arco con offset.
//            //     * 

//            //     */

//            //    bufferPnt.RemoveAt(0);
//            //    bufferPnt.RemoveAt(0);
//            //    bufferPnt.RemoveAt(0);
//            //    bufferPnt.RemoveAt(bufferPnt.Count - 1);
//            //    bufferPnt.RemoveAt(bufferPnt.Count - 1);
//            //    bufferPnt.RemoveAt(bufferPnt.Count - 1);


//            //    for (int i = 0; i < bufferPnt.Count - 7; i++)
//            //    {
//            //        counter++;
//            //        var midPoint1 = GeometryHelper.GetMidPoint(bufferPnt[i + 2], bufferPnt[i + 3]);
//            //        var midPoint2 = GeometryHelper.GetMidPoint(bufferPnt[i + 3], bufferPnt[i + 4]);
//            //        var midPoint3 = GeometryHelper.GetMidPoint(bufferPnt[i + 4], bufferPnt[i + 5]);

//            //        //var midPoint = bufferPnt[(int)bufferPnt.Count / 2];
//            //        var nCircocentro = GeometryHelper.Circocentro(midPoint1, midPoint2, midPoint3);

//            //        var nCircocentro0 = GeometryHelper.Circocentro(bufferPnt[2], bufferPnt[29], bufferPnt[34]);
//            //        var nCircocentro1 = GeometryHelper.Circocentro(bufferPnt[3], bufferPnt[30], bufferPnt[35]);
//            //        var nCircocentro2 = GeometryHelper.Circocentro(bufferPnt[4], bufferPnt[31], bufferPnt[36]);

//            //        if (circoCentro == null)
//            //            circoCentro = nCircocentro; // prima inizializzazione


//            //        var l = GeometryHelper.GetMidPoint(nCircocentro0, nCircocentro1);
//            //        l = GeometryHelper.GetMidPoint(l, nCircocentro2);

//            //        // faccio media circocentro
//            //        circoCentro = GeometryHelper.GetMidPoint(circoCentro, nCircocentro);

//            //        Point2D ce;
//            //        double ar1c;

//            //        var rsl = Circle(bufferPnt[2], bufferPnt[29], bufferPnt[34], out ce, out ar1c);
//            //        rsl = Circle(bufferPnt[i], bufferPnt[i + 1], bufferPnt[i + 2], out ce, out ar1c);

//            //        circoCentro = l;
//            //        // mi servono 2 punti consecutivi
//            //        raggio += GeometryHelper.Distance(bufferPnt[i], circoCentro).Value;
//            //        raggio += GeometryHelper.Distance(bufferPnt[i + 1], circoCentro).Value;
//            //        raggio += GeometryHelper.Distance(bufferPnt[i + 2], circoCentro).Value;

//            //    }

//            //    raggio = raggio / (counter * 3); // media

//            //    if (raggio == 0)
//            //        return null;

//            //    var arc = new Arc2D
//            //    {
//            //        Center = new Point2D(circoCentro),
//            //        Start = new Point2D(firstPnt),
//            //        End = new Point2D(lastPnt),
//            //        Radius = raggio,
//            //    };

//            //    arc.ClockWise = IsClockWise(circoCentro);

//            //    return arc;
//            //}
//            //public static bool Circle(Point2D p1, Point2D p2, Point2D p3, out Point2D center, out double radius)
//            //{
//            //    double t = p2.X * p2.X + p2.Y * p2.Y;
//            //    double bc = (p1.X * p1.X + p1.Y * p1.Y - t) / 2.0;
//            //    double cd = (t - p3.X * p3.X - p3.Y * p3.Y) / 2.0;
//            //    double det = (p1.X - p2.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p2.Y);

//            //    if (Math.Abs(det) > 1.0e-6) // Determinant was found. Otherwise, radius will be left as zero.
//            //    {
//            //        det = 1 / det;
//            //        double x = (bc * (p2.Y - p3.Y) - cd * (p1.Y - p2.Y)) * det;
//            //        double y = ((p1.X - p2.X) * cd - (p2.X - p3.X) * bc) * det;
//            //        double r = Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y));

//            //        center = new Point2D(x, y);
//            //        radius = r;

//            //        return true;
//            //    }
//            //    radius = 0;
//            //    center = null;

//            //    return false;
//            //}


//            /// 22/08/2011
//            /// Ora per costruiore archi immetto gia i putno che andranno sulla cfr .
//            /// non mi serfve più trovare i punti mediani..
//            ///// <summary>
//            ///// Se riesce a creare arco ok , altrimenti svuota il buffer di punti
//            ///// </summary>
//            ///// <returns></returns>
//            //public object CreateArc()
//            //{
//            //    /* controllo che sia possibile creare arco.*/

//            //    // questo metodo viene chiamato su chiusura arco.
//            //    // setto arco chiuso
//            //    IsArcStarted = false;

//            //    /*
//            //     * ora non importa più che prendo angolo mediano.
//            //     * 
//            //     * quindi i punti possono essre anche meno di 8
//            //     * 
//            //     * in ogni caso è bene levare i primi 2 punti e ultimi 2 dalla conta in quanto prendono valori sballati..
//            //     * 
//            //     * il primo e ultimo perche dipende dalla bisettrice con elemento adiacente
//            //     * il secondo e penultimo sono la normale per fare si che il prog si generato correttamente
//            //     */

//            //    if (bufferPnt.Count < 8)
//            //        return bufferPnt;

//            //    //var secondPoint = bufferPnt[1];
//            //    //var penultimoPoint = bufferPnt[bufferPnt.Count - 2];

//            //    var firstPnt = bufferPnt.FirstOrDefault();
//            //    var lastPnt = bufferPnt.LastOrDefault();

//            //    /*
//            //     * considerare anche di fare la media del buffer..
//            //     */
//            //    //var last3 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

//            //    /*
//            //     * Un 'altro problema che si presenta è in caso ci sia cfr.
//            //     * 
//            //     * qui i punti iniziale  e finale coincidono,
//            //     * 
//            //     * todo . tenere conto anche delle spirali, permettere massimo 1 giro 
//            //     * 
//            //     * per ovviare a questo problema provero a prendere il secondo , meta e penultimo
//            //     * cosi i punti estremi non coincideranno
//            //     * 03/08/2011
//            //     * se faccio cosi su arco di 4 punti il midPoint e penultimo point coincidono
//            //     */

//            //    /*
//            //     * Come spiegato nel commento della classe parsedArc, mi servono almeno arco di 8 punti per avere 
//            //     * valori attendibili.
//            //     * 
//            //     * Nonostante questo il valore del circocentro non è perfetto, intendo quindi fare una media di tutti i valori
//            //     * contenuti dentro il buffer
//            //     */

//            //    // - I  primi 2 e ultimi 2 non mi interessano,
//            //    // - Prendo 3 Punti alla volta , quindi mi devo fermare a -6 rispetto al count
//            //    // es.  su buffer di 80 , mi devo fermare a i = 73..
//            //    var counter = 0;
//            //    Point2D circoCentro = null;
//            //    var raggio = 00.0d;
//            //    for (int i = 0; i < bufferPnt.Count - 7; i++)
//            //    {
//            //        counter++;
//            //        var midPoint1 = GeometryHelper.GetMidPoint(bufferPnt[i + 2], bufferPnt[i + 3]);
//            //        var midPoint2 = GeometryHelper.GetMidPoint(bufferPnt[i + 3], bufferPnt[i + 4]);
//            //        var midPoint3 = GeometryHelper.GetMidPoint(bufferPnt[i + 4], bufferPnt[i + 5]);

//            //        //var midPoint = bufferPnt[(int)bufferPnt.Count / 2];
//            //        var nCircocentro = GeometryHelper.Circocentro(midPoint1, midPoint2, midPoint3);

//            //        if (circoCentro == null)
//            //            circoCentro = nCircocentro; // prima inizializzazione

//            //        // faccio media circocentro
//            //        circoCentro = GeometryHelper.GetMidPoint(circoCentro, nCircocentro);

//            //        // mi servono 2 punti consecutivi
//            //        raggio += GetRadius(new Line2D() { Start = bufferPnt[i + 2], End = bufferPnt[i + 3] }, circoCentro);

//            //        // non ho miglioramenti con 3 inserimenti per raggio
//            //        raggio += GetRadius(new Line2D() { Start = bufferPnt[i + 3], End = bufferPnt[i + 4] }, circoCentro);
//            //        raggio += GetRadius(new Line2D() { Start = bufferPnt[i + 4], End = bufferPnt[i + 5] }, circoCentro);

//            //    }

//            //    raggio = raggio / (counter * 3); // media

//            //    if (raggio == 0)
//            //        return null;

//            //    var arc = new Arc2D
//            //    {
//            //        Center = new Point2D(circoCentro),
//            //        Start = new Point2D(firstPnt),
//            //        End = new Point2D(lastPnt),
//            //        Radius = raggio,
//            //    };

//            //    arc.ClockWise = IsClockWise(circoCentro);

//            //    return arc;
//            //}

//            private bool IsClockWise(Point2D center)
//            {
//                /*
//                 * Guardo come incrementano i punti : 
//                 * se angolo cresce >> antiorario
//                 * se angolo diminuisce << orario
//                 */
//                if (bufferPnt.Count < 4)
//                    throw new Exception("ParsedArc");

//                var p1 = bufferPnt[1];
//                var p2 = bufferPnt[3];

//                /*
//                 * Ho noto punto iniziale . p finale e centro e i vari punti.
//                 * 
//                 * devo determinare se è cw o ccw.
//                 * 
//                 * Prendo 2 punti consecutivi.
//                 * - se angolo cresce è ccw
//                 * - se angolo diminuisce è cw
//                 * 
//                 */
//                var angle1 = Math.Atan2(p1.Y - center.Y, p1.X - center.X);
//                var angle2 = Math.Atan2(p2.Y - center.Y, p2.X - center.X);

//                //if (angle1 < 0)
//                //{
//                //    angle1 += Math.PI * 2;
//                //    angle2 += Math.PI * 2;
//                //}
//                //   angle1 = GeometryHelper.GetPositiveAngle(p1.X - center.X, p1.Y - center.Y);
//                // angle2 = GeometryHelper.GetPositiveAngle(p2.X - center.X, p2.Y - center.Y);

//                var dAngle = angle1 - angle2;

//                if (dAngle < 0)
//                    return false;

//                return true;
//            }


//            private bool IsOk(Point2D point2D)
//            {
//                /*
//                 * prendo ultimi 3 elementi dal buffer e faccio prova con questo
//                 */

//                if (bufferPnt.Count < 3)
//                    throw new Exception("ParsedArc");

//                var last4 = bufferPnt.GetRange(bufferPnt.Count - 3, 3);

//                var rslt = IsOk(last4[0], last4[1], last4[2], point2D);

//                if (!rslt)
//                {
//               //     rslt = IsOk(last4[0], last4[1], last4[2], point2D);

//                }


//                return rslt;
//            }

//            ////public double GetRadius()
//            ////{
//            ////    return 9;
//            ////}

//            ////public Point2D GetCentre()
//            ////{
//            ////    return new Point2D();
//            ////}

//            ////public Point2D GetStart()
//            ////{
//            ////    return new Point2D();
//            ////}

//            //public Point2D GetEnd()
//            //{
//            //    return new Point2D();
//            //}


//        }
// */
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


// 19/08/2011
/// <summary>
/// Costruisce lista di punti 2D,
/// archi li trasforma in sequenza di punti.
/// </summary>
//private List<PuntoDueD> GetPointList(double offsetDistance)
//{
//    /*
//     * provo a moltiplicare per 100000 sia xy e poi dividere alla fine..
//     */
//    var pntList = new List<PuntoDueD>();

//    const float arcStartEndNormalLength = .000001f;

//    for (int i = 0; i < Source.Count; i++)
//    {
//        //if (Source[i] is Arc2D && i == 0)
//        //{
//        //    var arc2D = Source[i] as Arc2D;
//        //    if (arc2D != null)
//        //    {
//        //        //    pntList.Add(new PuntoDueD(line.Start.X, line.Start.Y));
//        //        //    pntList.Add(new PuntoDueD(line.End.X, line.End.Y));

//        //        pntList.Add(new PuntoDueD((float)arc2D.Start.X, (float)arc2D.Start.Y));
//        //        //pntList.Add(new PuntoDueD((float)line.End.X, (float)line.End.Y));
//        //    }
//        //}

//        // se il primo punto è linea..
//        if (Source[i] is Line2D && i == 0)
//        {
//            var line = Source[i] as Line2D;
//            if (line != null)
//            {
//                //    pntList.Add(new PuntoDueD(line.Start.X, line.Start.Y));
//                //    pntList.Add(new PuntoDueD(line.End.X, line.End.Y));

//                pntList.Add(new PuntoDueD(line.Start.X, line.Start.Y));
//                pntList.Add(new PuntoDueD(line.End.X, line.End.Y));
//            }
//        }

//        else if (Source[i] is Line2D && i > 0)
//        {
//            var line = Source[i] as Line2D;
//            if (line != null)
//                pntList.Add(new PuntoDueD(line.End.X, line.End.Y));
//        }

//            /*
//             */
//        else if (Source[i] is Arc2D)
//        {
//            //var factor = 10000;
//            var arc = Source[i] as Arc2D;

//            var center = arc.Center;

//            var startAngle = Math.Atan2(arc.Start.Y - center.Y, arc.Start.X - center.X);

//            var endAngle = Math.Atan2(arc.End.Y - center.Y, arc.End.X - center.X);

//            var a = GeometryHelper.RadianToDegree(endAngle);
//            var radius = arc.Radius;

//            // per vedere se questo è il problema agisco man
//            //var tollerance = 0.005;
//            // 

//            /*
//             * con offset maggiore ho bisogno di più risoluzione 
//             * vero , deve essere inversamente proporzionale a offset
//             */

//            var coeffOffset = 1 / (offsetDistance / 50);

//            // con raggio maggiore servono più punti e quindi e inver anche a raggio
//            var coeffRadius = 1 / (radius / 50);

//            // todo . fare proporzionato a raggio e offset
//            //var angleIncrement = (radius / 500) / offsetDistance;
//            var angleIncrement = .055;

//            ///*
//            // * più arco è grande più servono punti 
//            // */
//            //var angleIncrement = radius * tollerance;

//            //if (angleIncrement < .05)
//            //    angleIncrement = .05;

//            // alla lista di punti non aggiungo punto finale e punto iniziale ,
//            // creo un punto molto vicino - hack.
//            // evito cosi self-intersect profile 
//            // ma devo trovare il modo di immettere il punto finale 
//            // in figura a falce , ci sarebbe un risultato scorretto..

//            //pntList.Add(new PuntoDueD(arc.Start.X, arc.Start.Y));

//            if (arc.ClockWise)
//            {
//                /* 
//                 * Se il senso è antiorario l'angolo finale dovra essere maggiore
//                 */

//                if (startAngle < 0)
//                {
//                    startAngle += Math.PI * 2;
//                    endAngle += Math.PI * 2;
//                }

//                if (endAngle >= startAngle)
//                    endAngle -= 2 * Math.PI;

//                var deltaAngle = endAngle - startAngle;

//                if (deltaAngle == 0)
//                {
//                    /* è un cerchio completo*/
//                    endAngle -= Math.PI * 2;
//                    deltaAngle = endAngle - startAngle;

//                }
//                //var deltaAngle = endAngle - startAngle;

//                //if (deltaAngle == 0)
//                //    throw new Exception();

//                // Se è clocwise angolo diminuisce 
//                //startAngle -= angleIncrement;

//                // angolo partenza deve essere incrementato perchè il punto iniziale è già stato immesso 
//                // (( punto finale linea precedente ))



//                var primoP = true;

//                for (var j = startAngle; j > endAngle; j -= angleIncrement)
//                {
//                    var x = (Math.Cos(j) * radius) + center.X;
//                    var y = (Math.Sin(j) * radius) + center.Y;

//                    pntList.Add(new PuntoDueD(x, y));

//                    if (primoP)
//                    {
//                        // aggiungo punto parellelo

//                        //var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(x, y));

//                        //normalAngle -= Math.PI;// todo . controllare con figura semplice--

//                        //var p2 = GeometryHelper.GetCoordinate(normalAngle, arcStartEndNormalLength, new Point2D(x, y));

//                        //pntList.Add(new PuntoDueD(p2.X, p2.Y));

//                        //primoP = false;
//                    }


//                }

//                //var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

//                //normalAngleEndPnt += Math.PI;

//                //var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, arc.End);

//                //pntList.Add(new PuntoDueD(normalEndPnt.X, normalEndPnt.Y));

//                //{

//                //    var x = (Math.Cos(endAngle - .0001) * radius) + center.X;
//                //    var y = (Math.Sin(endAngle - .0001) * radius) + center.Y;

//                //    pntList.Add(new PuntoDueD(x, y));

//                //}
//            }
//            else // Arco antiorario
//            {
//                /*
//                 * Se arco è ccw , angolo è maggiore ,
//                 * 
//                 * si parte da angolo iniziale e si aumenta fino a angolo finale
//                 * 
//                 */

//                if (startAngle < 0)
//                {
//                    startAngle += Math.PI * 2;
//                    endAngle += Math.PI * 2;
//                }

//                /* 
//                 * Se arco antiorario angolo finale dovra essere maggiore 
//                 */

//                if (endAngle < startAngle)
//                    endAngle += 2 * Math.PI;

//                var deltaAngle = endAngle - startAngle;

//                if (deltaAngle == 0)
//                {
//                    /* è un cerchio completo*/
//                    /* */

//                    endAngle += Math.PI * 2;
//                    deltaAngle = endAngle - startAngle;

//                }

//                //var deltaAngle = endAngle - startAngle;

//                //if (deltaAngle == 0)
//                //    throw new Exception();

//                // ho provato a levare angleIncrement..
//                //startAngle += angleIncrement;


//                /*
//                 * Allora per risolvere il problema degli archi , ( oltre alla soluzione di fare primo e ultimo angolo compensatore,)
//                 * 
//                 * Provo ad aggiungere 2 punti in modo che il primo e ultimo segmento dell'arco siano delle "normali" all'arco.
//                 * Cosi algoritmo per offset dovrebbe funzionare correttamente.
//                 * 
//                 * Quindi devo inserire punto iniziale ( con angolo 0).
//                 * Il punto normale a questo.
//                 * 
//                 * ilPunto normale al punto finale 
//                 * e il punto finale 
//                 * 
//                 * In questo modo l'algoritmo dovrebbe comportrsi correttamente
//                 * 
//                 * punto iniziale  
//                 * 
//                 * per inserire il punto finale mi basta inserire al penultimo posto il punto normale..
//                 * 
//                 */

//                /*
//                 * Con ciclo for , viene inserito il primo punto , poi inserisco normale secondo punto.
//                 * 
//                 * Questo ciclo "non" dovrebbe inserire punto per angolo finale. Si dovrebbe interrompere prima
//                 * . Incrementa variabile
//                 * . Controlla Variabile , se non passa controllo si interrompe , quindi si ferma 
//                 * 
//                 * sembra +/- funzionare 
//                 * 
//                 * pero l'arco estratto o faccio una media.. che mi sa che basti oppure 
//                 */
//                var normalFirstPoint = true;

//                //for (var j = startAngle; j < endAngle - angleIncrement / 2; j += angleIncrement)

//                for (var j = startAngle; j < endAngle; j += angleIncrement)
//                {
//                    var x = (Math.Cos(j) * radius) + center.X;
//                    var y = (Math.Sin(j) * radius) + center.Y;

//                    pntList.Add(new PuntoDueD(x, y)); // riporto a double

//                    if (normalFirstPoint)
//                    {
//                        // aggiungo punto parellelo

//                        //var normalAngle = GeometryHelper.getNormalAngle(arc, new Point2D(x, y));

//                        //var p2 = GeometryHelper.GetCoordinate(normalAngle, arcStartEndNormalLength, new Point2D(x, y));

//                        //pntList.Add(new PuntoDueD(p2.X, p2.Y));

//                        //normalFirstPoint = false;
//                    }
//                }

//                /*
//                 * Devo controllare che punto normale non sia maggiore del punto finale ,
//                 * magari devo guardare in termini di angolo
//                 */
//                // qui inserisco il punto normale a ultimo punto
//                //var normalAngleEndPnt = GeometryHelper.getNormalAngle(arc, arc.End);

//                //normalAngleEndPnt -= Math.PI;

//                //var normalEndPnt = GeometryHelper.GetCoordinate(normalAngleEndPnt, arcStartEndNormalLength, arc.End);

//                //pntList.Add(new PuntoDueD(normalEndPnt.X, normalEndPnt.Y));

//            }

//            // In ultimo aggiungo endpoint
//            pntList.Add(new PuntoDueD(arc.End.X, arc.End.Y));

//        }




//    }

//    return pntList;
//}
//26/08/02011
//public List<Profile2D> Offset(double offsetDistance, bool counterClockwise, bool parseCircle = false)
//       {
//           var rslt = new List<ContenitoreLivello>();

//           if (parseCircle)
//           {
//               Arc2D arc2D;
//               if (IsCircle(offsetDistance, counterClockwise, out arc2D))
//               {
//                   if (arc2D == null)
//                       return null;

//                   var profile = new Profile2D();
//                   profile.AddEntity(arc2D);

//                   return new List<Profile2D>() { profile };
//               }
//           }

//           //if(!IsSimple())
//           //{
//           // //   return null;
//           //}
//           // int molFact = 1;

//           //offsetDistance = offsetDistance * molFact;
//           var pntList = GetPointList(offsetDistance);

//           //foreach (var puntoDueD in pntList)
//           //{
//           //    puntoDueD.X = puntoDueD.X * molFact;
//           //    puntoDueD.Y = puntoDueD.Y * molFact;
//           //}

//           //int num3 = 0;

//           //if (pntList.Count >= 3)
//           //{
//           //    ClassO classO = new ClassO();
//           //    for (int j = 0; j < pntList.Count; j++)
//           //    {
//           //        classO.Add(new ForseLineaClassA(pntList[j], pntList[(j + 1) % pntList.Count]));
//           //    }
//           //    List<PuntoDueD> list6 = new List<PuntoDueD>();
//           //    List<int> list7 = new List<int>();
//           //    List<int> list8 = new List<int>();
//           //    classO.MethodA(list6, list7, list8);
//           //    num3 += list6.Count;
//           //}

//           //bool flag2 = false;
//           //if ((num3 > 0))
//           //{
//           //    flag2 = true;
//           //    /* devo resituire che eccezzione di profilo autointersecante */
//           //    return null;

//           //}


//           var n2 = new ClassN();

//           n2.MathodA1(pntList, false); //

//           var profi = new OffsetProfilo2D();

//           var linea = new ForseLineaClassA(0, 0, 10, 0);
//           var linea1 = new ForseLineaClassA(10, 0, 10, 10);
//           var linea2 = new ForseLineaClassA(10, 10, 0, 10);
//           var linea3 = new ForseLineaClassA(0, 10, 0, 0);

//           profi.MethodA(linea);
//           profi.MethodA(linea1);
//           profi.MethodA(linea2);
//           profi.MethodA(linea3);


//           n2.MathodA1(profi);

//           //n2.MathodA1();


//           // todo : 
//           //if (this._polys.Count > 0) // dentro qui ci entro se ci sono altri polygoni
//           //{
//           //    PolygonalShape polygonalShape = this._polys[this._polys.Count - 1];
//           //    if (polygonalShape.c(n2))  // se questi poligoni possono essere isole
//           //    // per esserre isole devono essere state disegnate dopo anello esterno
//           //    {
//           //        polygonalShape.MethodA1(n2);
//           //        base.Invalidate();
//           //        return;
//           //    }
//           //}
//           var polygonalShape1 = new PolygonalShape(n2);

//           var chamferAngle = Math.PI / 4;
//           //  chamferAngle = 0;

//           OffsetPathMainClass.Elaborate(new ClassC(polygonalShape1), offsetDistance, 1, chamferAngle, rslt);

//           /*
//            * da polygonal shape a classe utilizzabile nel processp
//            */
//           if (!snuffRslt(rslt))
//               return null;

//           var offsetResultPointProfile = new List<List<Point2D>>();



//           /*
//            * Gli ho chiesto 1 offset, se mi restituisce un numero diverso da 0 o 1 , probabilmente c'è errore
//            */
//           Debug.Assert(rslt.Count <= 1);

//           if (rslt.Count == 0)
//               return null;

//           for (int i = 0; i < rslt.Count; i++)
//           {
//               var contenitoreLivello = rslt[i];

//               /*
//                * Possono esserci più profili per lo stesso livello di offset,
//                * li devo tenere separati, 
//                */

//               for (int m = 0; m < contenitoreLivello.Count; m++)
//               {
//                   var pntListRslt = new List<Point2D>();

//                   var polygonalShape = contenitoreLivello[m];

//                   var a0 = polygonalShape.GetExternPolygon();

//                   for (int k = 0; k < a0.ac().Count; k++)
//                   {
//                       if (a0.ac()[k] is ForseLineaClassA)
//                       {
//                           var classA0 = a0.ac()[k] as ForseLineaClassA;


//                           var puntoDueD = classA0.h();
//                           var x2 = classA0.i();

//                           if (k == 0)
//                               pntListRslt.Add(new Point2D(puntoDueD.X, puntoDueD.Y));

//                           pntListRslt.Add(new Point2D(x2.X, x2.Y));

//                       }
//                       else if (a0.ac()[i] is ForseArco2ClassQ) // forse classq è la chiave
//                       {
//                           // this.MethodA(A_0.ac()[i] as ClassQ, A_1, A_2);
//                       }
//                   }

//                   offsetResultPointProfile.Add(pntListRslt);
//               }
//           }


//           var profileResulList = new List<Profile2D>();


//           foreach (var singleProfilePoints in offsetResultPointProfile)
//           {
//               //foreach (var singleProfilePoint in singleProfilePoints)
//               //{
//               //    singleProfilePoint.X = singleProfilePoint.X / molFact;
//               //    singleProfilePoint.Y = singleProfilePoint.Y / molFact;
//               //}

//               var list = new Profile2D();

//               if (counterClockwise)
//               {
//                   for (int i = singleProfilePoints.Count - 1; i >= 0; i--)
//                   {
//                       list.AddPnt(singleProfilePoints[i]);
//                   }
//               }
//               else
//               {
//                   for (int i = 0; i < singleProfilePoints.Count; i++)
//                   {
//                       list.AddPnt(singleProfilePoints[i]);
//                   }
//               }

//               profileResulList.Add(list);

//           }


//           return profileResulList;
//       }
