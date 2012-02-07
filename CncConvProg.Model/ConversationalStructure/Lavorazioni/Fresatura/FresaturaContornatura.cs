using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura
{
    [Serializable]
    public sealed class FresaturaContornatura : LavorazioneFresatura, IMillingPatternable, IMillWorkable
    {
        public FresaturaContornatura()
        {
            MillingPattern = EnumPatternMilling.PoligonoRegolare;

            Sgrossatura = new Operazione(this, LavorazioniEnumOperazioni.Sgrossatura);
            Finitura = new Operazione(this, LavorazioniEnumOperazioni.Finitura);
            Smussatura = new Operazione(this, LavorazioniEnumOperazioni.Smussatura);

            ProfonditaFresaSmussatura = 2;

            // poi incapsulero classe per creazione di polygono in modo da avere possilita di avere anche 
            // tondo senza troppi problemi..
            TrimRectangleStartPoint = SquareShapeHelper.SquareShapeStartPoint.Center;

        }

        public SquareShapeHelper.SquareShapeStartPoint TrimRectangleStartPoint { get; set; }

        public IMillingPattern Pattern { get; private set; }

        private EnumPatternMilling _millingPattern;
        public EnumPatternMilling MillingPattern
        {
            get
            {
                return _millingPattern;
            }
            set
            {
                _millingPattern = value;
                UpdatePattern();
            }
        }

        private void UpdatePattern()
        {
            Pattern = MillPatternHelper.GetPattern(MillingPattern);
        }

        public RawProfile Profile { get; set; }

        public double ProfonditaLavorazione { get; set; }

        public double Sovrametallo { get; set; }

        public double InizioLavorazioneZ { get; set; }

        public double SovrametalloFinituraProfilo { get; set; }

        public double SovrametalloFinituraZ { get; set; }

        public double ProfonditaFresaSmussatura
        {
            get;
            set;
        }

        public bool FinishWithCompensation { get; set; }

        public bool ChamferWithCompensation { get; set; }


        public override List<Operazione> Operazioni
        {
            get
            {
                var opList = new List<Operazione>();

                //if (Sgrossatura.Abilitata)
                    opList.Add(Sgrossatura);

                //if (Finitura.Abilitata)
                    opList.Add(Finitura);

                //if (Smussatura.Abilitata)
                    opList.Add(Smussatura);

                return opList;
            }
        }

        public Operazione Sgrossatura { get; set; }

        public Operazione Finitura { get; set; }

        public Operazione Smussatura { get; set; }

        //internal override Utensile CreateTool(LavorazioniEnumOperazioni enumOperationType)
        //{
        //    var unit = Singleton.Instance.MeasureUnit;

        //    return FresaturaHelper.CreateTool((LavorazioniEnumOperazioni)enumOperationType, unit);
        //}

        //internal override List<Utensile> GetCompatibleTools(LavorazioniEnumOperazioni operationType, MagazzinoUtensile magazzino)
        //{
        //    var unit = Singleton.Instance.MeasureUnit;

        //    return magazzino.GetTools<FresaCandela>(unit).Cast<Utensile>().ToList();
        //}


        public override void SetProfile(List<RawEntity2D> list)
        {
            Profile.Syncronize(list);

            base.SetProfile(list);
        }

        private Profile2D GetProfile()
        {
            if (Pattern != null)
                return Pattern.GetClosedProfile();

            return null;
        }

        //private static List<Profile2D> CalculateExternalOffset(Profile2D origin, double offsetValue, double diameterValue)
        //{
        //    offsetValue = -Math.Abs(offsetValue);

        //    var firstPaths = origin.Offset(-diameterValue, false);

        //    if (firstPaths == null)
        //        return null;

        //    var rslt = new List<Profile2D>();

        //    foreach (var firstPath in firstPaths)
        //    {
        //        rslt.Add(firstPath);

        //        RicorsivaGenerateInternOffset(firstPath, offsetValue, false, ref rslt);
        //    }


        //    return rslt;
        //}

        //private static void RicorsivaGenerateInternOffset(Profile2D profile2D, double offset, bool clockwise, ref List<Profile2D> profile2DsList)
        //{
        //    // Calcola offset , ritorna 1 o più contorni 
        //    var offsetRslt = profile2D.Offset(offset, clockwise);


        //    // se non ritorna più niente termina metodo 
        //    if (offsetRslt == null)
        //        return;

        //    foreach (var singleContour in offsetRslt)
        //    {
        //        profile2DsList.Add(singleContour);
        //        RicorsivaGenerateInternOffset(singleContour, offset, clockwise, ref profile2DsList);
        //    }
        //}

        /*
         * 03/08/2011
         * per ora ho rettangolo per trim path - poi dovro fare polygono 
         * cmq incapsulato in una classe apposita
         */

        public bool TrimPathAbilited { get; set; }
        public double RectTrimCenterX { get; set; }
        public double RectTrimCenterY { get; set; }
        public double RectTrimHeight { get; set; }
        public double RectTrimWidth { get; set; }



        protected override void CreateSpecificProgram(ProgramPhase programPhase, Operazione operazione)
        {
            var diametrable = operazione.Utensile as IDiametrable;

            var parametro = operazione.Utensile.ParametroUtensile as ParametroFresaCandela;

            if (diametrable == null) return;

            var diaFresa = diametrable.Diametro;


            double profPassata = 0;
            double larghezzaPassata = 0;

            if (parametro != null)
            {
                profPassata = parametro.GetProfonditaPassata();

                larghezzaPassata = parametro.GetLarghezzaPassata();
            }



            var centerTrimRect = SquareShapeHelper.GetCenterPoint(TrimRectangleStartPoint, RectTrimCenterX, RectTrimCenterY, RectTrimWidth,
                                                 RectTrimHeight);

            var trimRectPoly = SquareShapeHelper.GetSquareProfile(centerTrimRect.X, centerTrimRect.Y, RectTrimWidth,
                                                                  RectTrimHeight);


            /*
             * 
             */

            var moveCollection = new MoveActionCollection();

            var profileToWork = GetProfile();

            var trimAbilited = TrimPathAbilited;
            var trimPoly = trimRectPoly;
            var matToRemove = Sovrametallo;

            // Piccolo hack per gestire profili aperti.
            if (Pattern is IOpenMillingPattern)
            {
                var openPattern = Pattern as IOpenMillingPattern;
                trimAbilited = true;
                trimPoly = openPattern.GetTrimmingProfile();
                matToRemove = openPattern.MaterialToRemove;
            }

            switch (operazione.OperationType)
            {
                case LavorazioniEnumOperazioni.Sgrossatura:
                    {

                        var sovrametalloXy = SovrametalloFinituraProfilo;

                        //if (!Finitura.Abilitata)
                        //    sovrametalloXy = 0; // lascio sovrametallo anche senza finituraa

                        MillProgrammingHelper.GetExterRoughing(moveCollection, profileToWork, trimPoly, trimAbilited, ProfonditaLavorazione,
                            profPassata, matToRemove, larghezzaPassata, diaFresa, ExtraCorsa, InizioLavorazioneZ, SicurezzaZ, sovrametalloXy, SovrametalloFinituraZ);

                    } break;

                case LavorazioniEnumOperazioni.Finitura:
                    {
                        MillProgrammingHelper.GetExterFinish(moveCollection, profileToWork, trimPoly, trimAbilited, ProfonditaLavorazione, profPassata, diaFresa, ExtraCorsa, FinishWithCompensation, InizioLavorazioneZ, SicurezzaZ);
                    } break;

                case LavorazioniEnumOperazioni.Smussatura:
                    {
                        MillProgrammingHelper.GetExterChamfer(moveCollection, profileToWork, trimPoly,
                                                              trimAbilited, ProfonditaFresaSmussatura, diaFresa,
                                                              ExtraCorsa, ChamferWithCompensation, InizioLavorazioneZ,
                                                              SicurezzaZ);
                    } break;

                default:
                    {
                        Debug.Fail("FresaturaContornatura.GetOperationProgram");
                    } break;

            }

            var mm = base.GetFinalProgram(moveCollection);

            foreach (var variable in mm)
            {
                programPhase.AddMoveAction(variable);
            }
        }

        public override string Descrizione
        {
            get { return "Contour"; }
        }

        /// <summary>
        /// Ritorna anteprima della lavorazione
        /// </summary>
        /// <returns></returns>
        protected override List<IEntity3D> GetFinalPreview()
        {
            try
            {
                if (Pattern != null)
                {
                    var rslt = new List<IEntity3D>();

                    var preview = Pattern.GetClosedProfile().Source;

                    if (Pattern is IOpenMillingPattern)
                    {
                        /*
                         * trimma la preview, con stesso metodo che trimmo profilo..
                         */
                    }

                    if (TrimPathAbilited)
                        rslt.AddRange(GetTrimPath());


                    rslt.AddRange(Entity3DHelper.Get3DProfile(preview).ToList());

                    return rslt;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("FresaturaContornatura.GetFinalPreview");
            }

            return null;
        }

        private IEnumerable<IEntity3D> GetTrimPath()
        {
            //var trimRectPoly = SquareShapeHelper.GetSquareProfile(centerTrimRect.X, centerTrimRect.Y, RectTrimWidth,
            //                                                      RectTrimHeight);

            var centerPoint = SquareShapeHelper.GetCenterPoint(TrimRectangleStartPoint, RectTrimCenterX, RectTrimCenterY, RectTrimWidth,
                                            RectTrimHeight);


            var l1 = new Line3D
            {
                Start = new Point3D
                {
                    X = centerPoint.X + RectTrimWidth / 2,
                    Y = centerPoint.Y + RectTrimHeight / 2,
                    Z = 0,
                },

                End = new Point3D
                {
                    X = centerPoint.X + RectTrimWidth / 2,
                    Y = centerPoint.Y - RectTrimHeight / 2,
                    Z = 0,
                }
            };

            var l2 = new Line3D
            {
                Start = l1.End,

                End = new Point3D
                {
                    X = centerPoint.X - RectTrimWidth / 2,
                    Y = centerPoint.Y - RectTrimHeight / 2,
                    Z = 0,
                }
            };

            var l3 = new Line3D
            {
                Start = l2.End,

                End = new Point3D
                {
                    X = centerPoint.X - RectTrimWidth / 2,
                    Y = centerPoint.Y + RectTrimHeight / 2,
                    Z = 0,
                }
            };

            var l4 = new Line3D
            {
                Start = l3.End,
                End = l1.Start,
            };

            var rslt = new List<IEntity3D> { l1, l2, l3, l4 };

            foreach (var entity2D in rslt)
            {
                entity2D.PlotStyle = EnumPlotStyle.TrimPath;
            }

            return rslt;
        }

    }

}

// 02/08/2011 fresatura sgrossatura prima di stock trimming..
//private static void FresaturaSgrossaturaContornatura(MoveActionCollection programPhase, double diaFresa, double larghPassata, double profPassata, double extraCorsa,
//                                                        double sovraMetallo, double zIniziale, double profondita, double zSicurezza, Profile2D profile2D)
//    {
//        /*
//         * Controllo Valori
//         */
//        if (CheckValueHelper.GreatherOrEqualZero(new[] { extraCorsa }) ||
//            CheckValueHelper.GreatherThanZero(new[] { sovraMetallo, profondita, larghPassata, diaFresa, profPassata, }) ||
//            CheckValueHelper.GreatherThan(new[]
//                                          {
//                                              new KeyValuePair<double, double>(zSicurezza, zIniziale),
//                                              new KeyValuePair<double, double>(diaFresa, larghPassata),
//                                          })
//            ) return;

//        if (profile2D == null) return;

//        /*
//         * qui ci arrivo se sovrametallo è maggiore di 0
//         */

//        /*
//         * Le passate le costruisco dalla prima ( offset raggio fresa ), incrementando della larghezza passata fino a coprire tutto il sovrametallo
//         */

//        var raggioFresa = diaFresa / 2;

//        var distanzaOffsetCorrente = sovraMetallo + raggioFresa;

//        var continueCycle = distanzaOffsetCorrente > 0;

//        var offsetCountorns = new List<Profile2D>();

//        while (continueCycle)
//        {
//            distanzaOffsetCorrente -= larghPassata;

//            if (distanzaOffsetCorrente < raggioFresa)
//            {
//                distanzaOffsetCorrente = raggioFresa;
//                continueCycle = false;
//            }

//            var path = profile2D.Offset(distanzaOffsetCorrente, true);

//            offsetCountorns.AddRange(path);

//        }
//        // stock trimming experiment ..
//        var offsetCounterStockTrimmed = new List<Profile2D>();

//        var pp1 = new Point2D(-20, 0);
//        var pp2 = new Point2D(-20, 15);
//        var pp3 = new Point2D(20, 15);
//        var pp4 = new Point2D(20, 0);

//        var polygon = new Profile2D();

//        polygon.AddPnt(pp1);
//        polygon.AddPnt(pp2);
//        polygon.AddPnt(pp3);
//        polygon.AddPnt(pp4);
//        polygon.AddPnt(pp1);

//        foreach (var offsetCountorn in offsetCountorns)
//        {
//            var rs = GeometryHelper.TrimProfileByStockPolygon(offsetCountorn, polygon);

//            offsetCounterStockTrimmed.AddRange(rs);

//        }

//        // ----------------------------------
//        /*
//         * Calcolo punto attacco
//         */
//        var distanzaAttacco = sovraMetallo + raggioFresa + extraCorsa;

//        var attacPath = profile2D.Offset(distanzaAttacco, true);

//        if (attacPath == null)return;

//        var attacPoint = attacPath.First().Source.FirstOrDefault();

//        if (attacPoint == null)
//            throw new Exception();



//        // Vado fino a z di inizio lavoro 
//        //programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zIniziale);

//        var zCurrent = zIniziale;

//        var zFinale = zIniziale - profondita;

//        // Itero per tutte le passate in Z
//        while (zCurrent > zFinale)
//        {
//            zCurrent -= profPassata;
//            if (zCurrent < zFinale)
//                zCurrent = zFinale;

//            // Mi sposto in XY di attacco
//            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, attacPoint.GetFirstPnt().X, attacPoint.GetFirstPnt().Y, null);


//            // Mi sposto in z sicurezza
//            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);

//            // Movimento in piantata a z di lavoro..
//            programPhase.AddLinearMove(MoveType.PlungeFeed, AxisAbilited.Z, null, null, zCurrent);

//            //if (offsetCountorns.Count <= 0)
//            //    return;

//            if (offsetCounterStockTrimmed.Count <= 0)
//                return;

//            //for (var i = 0; i < offsetCountorns.Count; i++)
//            //{
//            //    var element = offsetCountorns[i];

//            for (var i = 0; i < offsetCounterStockTrimmed.Count; i++)
//            {
//                var element = offsetCounterStockTrimmed[i];

//                // Faccio parse per risolvere archi
//                var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

//                /**/
//                var source = parsedCountor;
//                // var source = element;


//                // todo : gestire meglio primo elemento 
//                // ogni volta che comincio profilo
//                //if (i == offsetCountorns.Count - 1 &&
//                //    element.Source[0] is Line2D)
//                //{
//                //    var firstMove = ((Line2D)element.Source[0]).Start;

//                //    programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);

//                //}

//                if (source.Source[0] is Line2D)
//                {
//                    var firstMove = ((Line2D)element.Source[0]).Start;

//                    programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//                }

//                foreach (var entity2D in source.Source)
//                {
//                    if (entity2D is Line2D)
//                    {
//                        var line = entity2D as Line2D;

//                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);
//                    }

//                    else if (entity2D is Arc2D)
//                    {
//                        // implementare archi 

//                        var arc = entity2D as Arc2D;

//                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, arc.Start.X, arc.Start.Y, zCurrent);

//                        programPhase.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, zCurrent, arc.Radius, arc.ClockWise, arc.Center);
//                    }
//                }

//            }

//            // Mi sposto in z sicurezza
//            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


//        }

//    }

//private static IEnumerable<IEntity2D> MillExternRoughingPreview(Profile2D profile, double larghPassata, double diaFresa, double sovraMetallo)
//{
//    // todo : controlli validità valori 
//    // todo : aggiungere materiale di finitura.

//    var numeroPassate = (int)Math.Ceiling(sovraMetallo / larghPassata);
//    var raggioFresa = diaFresa / 2;

//    // aggiungo una passata in + per avere il punto di attacco..
//    // mi servono metodi per avere subito profile2d 
//    // es . offset(10, profile) .> mi ritorna subito profilo

//    var countornPath = new List<Profile2D>();
//    // come primo punto metto punto attacco ,

//    // aggiungo prima passata, distanza dal profilo = raggio fresa ( fare anche più sovramettallo)
//    var firstPath = profile.Offset(raggioFresa);

//    countornPath.Add(firstPath);

//    var lastPath = firstPath;
//    // var p = new List<IEntity2D>();

//    for (var i = 0; i < numeroPassate; i++)
//    {
//        var path = lastPath.Offset(larghPassata);

//        countornPath.Add(path);

//        lastPath = path;

//        //foreach (var entity2D in path.Source)
//        //{
//        //    p.Add(entity2D);
//        //}
//    }

//    //  return p;


//    // calcolo ultima passata per avere punto di attacco.

//    var attacPath = lastPath.Offset(raggioFresa * 1.2);

//    var attacPoint = attacPath.Source.FirstOrDefault();

//    if (attacPoint == null)
//        throw new Exception();

//    // todo , gestire anche archi
//    var pntAt = ((Line2D)attacPoint).Start;

//    // ora ho tutto quello che serve , posso partire a costruire codice uni

//    var movements = new List<IEntity2D>();
//    for (var i = countornPath.Count - 1; i >= 0; i--)
//    {
//        var element = countornPath[i];

//        var source = element.Source;

//        // todo : gestire meglio primo elemento
//        // per primo elemento 
//        //if (i == countornPath.Count - 1 &&
//        //    element.Source[0] is Line2D)
//        //{
//        //    var elementZero = ((Line2D)element.Source[0]);

//        //    var firstAttac = new Line2D();

//        //    firstAttac.Start = pntAt;

//        //    firstAttac.End = elementZero.Start;

//        //    //var firstMove = new Line2D();

//        //    //firstMove.Start = elementZero.Start;

//        //    //firstMove.End = elementZero.End;

//        //    movements.Add(firstAttac);
//        //}

//        foreach (var entity2D in source)
//        {
//            if (entity2D is Line2D)
//            {
//                var line = entity2D as Line2D;

//                if (source.IndexOf(entity2D) == 0)
//                {
//                    var firstAttac = new Line2D();

//                    firstAttac.Start = pntAt;

//                    firstAttac.End = line.Start;

//                    movements.Add(firstAttac);
//                }

//                movements.Add(line);

//                pntAt = line.End;
//            }

//            // finito questo foreach dovrebbe avere terminato il giro e essere pronto a fare 
//            // contorno più stretto
//        }

//    }

//    return movements;
//}

//private static IEnumerable<IEntity2D> MillExternFinishPreview(Profile2D profile, double diaFresa)
//{
//    // todo : controlli validità valori 
//    // todo : aggiungere materiale di finitura.

//    var raggioFresa = diaFresa / 2;

//    // aggiungo una passata in + per avere il punto di attacco..
//    // mi servono metodi per avere subito profile2d 
//    // es . offset(10, profile) .> mi ritorna subito profilo

//    var countornPath = new List<Profile2D>();
//    // come primo punto metto punto attacco ,

//    // aggiungo prima passata, distanza dal profilo = raggio fresa ( fare anche più sovramettallo)
//    var firstPath = profile.Offset(raggioFresa);

//    countornPath.Add(firstPath);

//    var lastPath = firstPath;
//    // var p = new List<IEntity2D>();

//    var attacPath = lastPath.Offset(raggioFresa * 1.2);

//    var attacPoint = attacPath.Source.FirstOrDefault();

//    if (attacPoint == null)
//        throw new Exception();

//    // todo , gestire anche archi
//    var pntAt = ((Line2D)attacPoint).Start;

//    // ora ho tutto quello che serve , posso partire a costruire codice uni

//    var movements = new List<IEntity2D>();
//    for (var i = countornPath.Count - 1; i >= 0; i--)
//    {
//        var element = countornPath[i];

//        var source = element.Source;

//        foreach (var entity2D in source)
//        {
//            if (entity2D is Line2D)
//            {
//                var line = entity2D as Line2D;

//                if (source.IndexOf(entity2D) == 0)
//                {
//                    var firstAttac = new Line2D();

//                    firstAttac.Start = pntAt;

//                    firstAttac.End = line.Start;

//                    movements.Add(firstAttac);
//                }

//                movements.Add(line);

//                pntAt = line.End;
//            }

//            // finito questo foreach dovrebbe avere terminato il giro e essere pronto a fare 
//            // contorno più stretto
//        }

//    }

//    return movements;
//}

///// <summary>
//        /// Metodo per la generazione della finitura di un contorno.
//        /// - Per ora genero solamente il percorso del profilo, non riprendo materiale in z
//        /// - Creo con compensazione cnc - immetto profilo pari pari.
//        ///  oppure con compensazione computer . faccio offset pari al raggio della fresa.
//        /// - anche qui , se abilitato faccio trimming percorso utensile.
//        /// 
//        /// </summary>
//        /// <param name="programPhase"></param>
//        /// <param name="diaFresa"></param>
//        /// <param name="profPassata"></param>
//        /// <param name="extraCorsa"></param>
//        /// <param name="zIniziale"></param>
//        /// <param name="profondita"></param>
//        /// <param name="zSicurezza"></param>
//        /// <param name="profile2D"></param>
//        /// <param name="tpCx"></param>
//        /// <param name="tpCy"></param>
//        /// <param name="tpH"></param>
//        /// <param name="tpW"></param>
//        /// <param name="tpAbilited"></param>
//        private static void FresaturaSmussaturaContornatura(MoveActionCollection programPhase, double diaFresa, double profPassata, double extraCorsa,
//                                                        double zIniziale, double profondita, double zSicurezza, Profile2D profile2D, Profile2D trimProfile, bool tpAbilited)
//        {
//            /*
//             * Controllo Valori
//             */
//            if (CheckValueHelper.GreatherOrEqualZero(new[] { extraCorsa }) ||
//                CheckValueHelper.GreatherThanZero(new[] { profondita, diaFresa, profPassata, }) ||
//                CheckValueHelper.GreatherThan(new[]
//                                              {
//                                                  new KeyValuePair<double, double>(zSicurezza, zIniziale),
//                                              })
//                ) return;

//            if (profile2D == null) return;

//            /*
//             * OSSERVAZIONI PER STOCK TRIMMING
//             * - Lo stock deve essere minimo il bounding Box del profilo. per evitare complicazioni.
//             * - I Profili che genero devo avere già integrato lo spostamento di avvicinamento.
//             * - Magari i profili che genero non serve che siano della classe Profile2D ma basta che siano un set di punti.
//             */

//            /*
//             * Le passate le costruisco dalla prima ( offset raggio fresa ), incrementando della larghezza passata fino a coprire tutto il sovrametallo
//             */

//            var raggioFresa = diaFresa / 2;

//            var offsetCountorns = new List<Profile2D>();

//            var distanzaAttacco = raggioFresa + extraCorsa;

//            var attacPath = profile2D.Offset(distanzaAttacco, true);

//            if (attacPath == null) return;

//            var attacPoint = attacPath.First().Source.FirstOrDefault();

//            if (attacPoint == null)
//                throw new Exception();

//            var attachPoint = attacPoint.GetFirstPnt();
//            /*
//             * qui genero i profili di contornatura,
//             * 
//             * Ad ogni profilo generato aggiungo anche il punto di attacco
//             * ( il punto finale del profilo precedente..)
//             */
//            var path = profile2D.Offset(raggioFresa, true);

//            /*
//             * Nel caso della contornatura esterna prendo solamente il primo profilo
//             */
//            if (path.Count > 0)
//            {
//                var p = path[0];

//                p.ToolPathStartPoint = attachPoint;

//                offsetCountorns.Add(path[0]);

//                /*
//                 * Inserisco attac point,
//                 */

//                attachPoint = p.Source.Last().GetLastPnt();
//            }

//            //// stock trimming experiment ..
//            var offsetCounterStockTrimmed = new List<Profile2D>();

//            //var pp1 = new Point2D(tpCx + tpW / 2, tpCy + tpH / 2);
//            //var pp2 = new Point2D(tpCx + tpW / 2, tpCy - tpH / 2);
//            //var pp3 = new Point2D(tpCx - tpW / 2, tpCy - tpH / 2);
//            //var pp4 = new Point2D(tpCx - tpW / 2, tpCy + tpH / 2);

//            //var polygon = new Profile2D();

//            //polygon.AddPnt(pp1);
//            //polygon.AddPnt(pp2);
//            //polygon.AddPnt(pp3);
//            //polygon.AddPnt(pp4);
//            //polygon.AddPnt(pp1);

//            var stockPolygon = trimProfile;

//            if (tpAbilited) // se la funzione di trimm del profilo è abilitata
//            {
//                /*
//                 * verificare bb del profilo
//                 */
//                foreach (var offsetCountorn in offsetCountorns)
//                {
//                    var rs = GeometryHelper.TrimProfileByStockPolygon(offsetCountorn, stockPolygon, raggioFresa + extraCorsa);

//                    // aggiungere distanza di sicurezza , estendere linee
//                    if (rs != null)
//                        offsetCounterStockTrimmed.AddRange(rs);

//                }
//            }
//            else
//                offsetCounterStockTrimmed = offsetCountorns;

//            if (offsetCounterStockTrimmed.Count <= 0)
//                return;

//            var zLavoro = zIniziale - profondita;

//            for (var i = 0; i < offsetCounterStockTrimmed.Count; i++)
//            {
//                var element = offsetCounterStockTrimmed[i];

//                // Faccio parse per risolvere archi
//                var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

//                //var source = element;
//                var source = parsedCountor;

//                /*
//                 * se punto iniziale si trova dentro profilo fare :
//                 * - trovare nearest point da punto attacco e profilo grezzo  
//                 * - fare estensione segmento punto iniziale - punto vicino 
//                 * - punto attacco è punto esteso.
//                 */
//                var profileStartPoint = source.ToolPathStartPoint ?? source.Source.First().GetFirstPnt();



//                if (GeometryHelper.PointInPolygon(stockPolygon.GetPointListP2(), profileStartPoint))
//                {

//                }

//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y, null);
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zLavoro);

//                if (source.Source[0] is Line2D)
//                {
//                    var firstMove = ((Line2D)element.Source[0]).Start;

//                    programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//                }
//                else if (element.Source[0] is Arc2D)
//                {
//                    /*
//                     * todo . controllare con arco come primo elemento.come nel caso sopra , dove spostamento è maggiore dia fresa.
//                     */
//                }

//                foreach (var entity2D in source.Source)
//                {
//                    if (entity2D is Line2D)
//                    {
//                        var line = entity2D as Line2D;

//                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);

//                    }

//                    else if (entity2D is Arc2D)
//                    {
//                        // implementare archi 

//                        var arc = entity2D as Arc2D;

//                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, arc.Start.X, arc.Start.Y, zLavoro);

//                        programPhase.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, zLavoro, arc.Radius, arc.ClockWise, arc.Center);

//                    }
//                }

//            }

//            // Mi sposto in z sicurezza
//            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


//        }

//        /// <summary>
//        /// Metodo per la generazione della finitura di un contorno.
//        /// - Per ora genero solamente il percorso del profilo, non riprendo materiale in z
//        /// - Creo con compensazione cnc - immetto profilo pari pari.
//        ///  oppure con compensazione computer . faccio offset pari al raggio della fresa.
//        /// - anche qui , se abilitato faccio trimming percorso utensile.
//        /// 
//        /// </summary>
//        /// <param name="programPhase"></param>
//        /// <param name="diaFresa"></param>
//        /// <param name="larghPassata"></param>
//        /// <param name="profPassata"></param>
//        /// <param name="extraCorsa"></param>
//        /// <param name="sovraMetallo"></param>
//        /// <param name="zIniziale"></param>
//        /// <param name="profondita"></param>
//        /// <param name="zSicurezza"></param>
//        /// <param name="profile2D"></param>
//        /// <param name="tpCx"></param>
//        /// <param name="tpCy"></param>
//        /// <param name="tpH"></param>
//        /// <param name="tpW"></param>
//        /// <param name="tpAbilited"></param>
//        /// <param name="cncCompensation"></param>
//        /// /// todo :: questa puo essere vista anche come sgrossatura con sovrametallo < larghezza passata.. e smussatura idem..
//        private static void FresaturaFinituraContornatura(MoveActionCollection programPhase, double diaFresa, double larghPassata, double profPassata, double extraCorsa,
//                                                        double sovraMetallo, double zIniziale, double profondita, double zSicurezza, Profile2D profile2D, Profile2D stockPolygon, bool tpAbilited, bool cncCompensation)
//        {
//            /*
//             * Controllo Valori
//             */
//            if (CheckValueHelper.GreatherOrEqualZero(new[] { extraCorsa }) ||
//                CheckValueHelper.GreatherThanZero(new[] { sovraMetallo, profondita, larghPassata, diaFresa, profPassata, }) ||
//                CheckValueHelper.GreatherThan(new[]
//                                                  {
//                                                      new KeyValuePair<double, double>(zSicurezza, zIniziale),
//                                                      new KeyValuePair<double, double>(diaFresa, larghPassata),
//                                                  })
//                ) return;

//            if (profile2D == null) return;

//            /*
//             * OSSERVAZIONI PER STOCK TRIMMING
//             * - Lo stock deve essere minimo il bounding Box del profilo. per evitare complicazioni.
//             * - I Profili che genero devo avere già integrato lo spostamento di avvicinamento.
//             * - Magari i profili che genero non serve che siano della classe Profile2D ma basta che siano un set di punti.
//             */

//            /*
//             * Le passate le costruisco dalla prima ( offset raggio fresa ), incrementando della larghezza passata fino a coprire tutto il sovrametallo
//             */

//            /*
//             * iter. 
//             * 
//             * trovo punto iniziale facendo offest del profilo source di una qunatita pari a raggio fresa + extracorsa.
//             * 
//             * prendo punto iniziale . magari invece di prendere punto a cazzo prendo punto più estremo.
//             * 
//             * il profilo di finitura sarà pari a profilo originale se il profilo è con comp tool . invece sarà pari offset raggio fresa in caso di comp da computer.
//             */

//            var raggioFresa = diaFresa / 2;

//            //var distanzaOffsetCorrente = sovraMetallo + raggioFresa;

//            //var continueCycle = distanzaOffsetCorrente > 0;

//            var offsetCountorns = new List<Profile2D>();

//            var distanzaAttacco = raggioFresa + extraCorsa;

//            var attacPath = profile2D.Offset(distanzaAttacco, true);

//            if (attacPath == null) return;

//            var attacPoint = attacPath.First().Source.FirstOrDefault();

//            if (attacPoint == null)
//                throw new Exception();

//            var attachPoint = attacPoint.GetFirstPnt();
//            /*
//             * qui genero i profili di contornatura,
//             * 
//             * Ad ogni profilo generato aggiungo anche il punto di attacco
//             * ( il punto finale del profilo precedente..)
//             */

//            Profile2D profile = null;
//            if (cncCompensation)
//            {
//                profile = profile2D;

//                profile.ToolPathStartPoint = attachPoint;

//                offsetCountorns.Add(profile);

//            }
//            else
//            {
//                // toolpath + raggio .

//                var path = profile2D.Offset(raggioFresa, true);

//                /*
//                 * Nel caso della contornatura esterna prendo solamente il primo profilo
//                 */
//                if (path.Count > 0)
//                {
//                    var p = path[0];

//                    p.ToolPathStartPoint = attachPoint;

//                    offsetCountorns.Add(path[0]);

//                }
//            }


//            // stock trimming experiment ..
//            var offsetCounterStockTrimmed = new List<Profile2D>();

//            var polygon = stockPolygon;

//            if (tpAbilited) // se la funzione di trimm del profilo è abilitata
//            {
//                /*
//                 * verificare bb del profilo
//                 */

//                foreach (var offsetCountorn in offsetCountorns)
//                {
//                    var rs = GeometryHelper.TrimProfileByStockPolygon(offsetCountorn, polygon,
//                                                                      raggioFresa + extraCorsa);

//                    // aggiungere distanza di sicurezza , estendere linee
//                    if (rs != null)
//                        offsetCounterStockTrimmed.AddRange(rs);

//                }
//            }
//            else
//                offsetCounterStockTrimmed = offsetCountorns;

//            var zCurrent = zIniziale;

//            var zFinale = zIniziale - profondita;

//            // Itero per tutte le passate in Z
//            while (zCurrent > zFinale)
//            {
//                zCurrent -= profPassata;
//                if (zCurrent < zFinale)
//                    zCurrent = zFinale;

//                if (offsetCounterStockTrimmed.Count <= 0)
//                    return;

//                Point2D lastMovePoint = null;

//                for (var i = 0; i < offsetCounterStockTrimmed.Count; i++)
//                {
//                    var element = offsetCounterStockTrimmed[i];

//                    // Faccio parse per risolvere archi
//                    var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

//                    //var source = element;
//                    var source = parsedCountor;

//                    /*
//                     * se punto iniziale si trova dentro profilo fare :
//                     * - trovare nearest point da punto attacco e profilo grezzo  
//                     * - fare estensione segmento punto iniziale - punto vicino 
//                     * - punto attacco è punto esteso.
//                     */
//                    var profileStartPoint = source.ToolPathStartPoint ?? source.Source.First().GetFirstPnt();



//                    if (lastMovePoint == null)
//                    {

//                        if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                        {

//                        }

//                        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X,
//                                                   profileStartPoint.Y, null);
//                        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                        programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);


//                    }
//                    else
//                    {
//                        var distance = GeometryHelper.Distance(profileStartPoint, lastMovePoint);

//                        if (distance > diaFresa)
//                        {
//                            if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                            {
//                                var closestPoint = GeometryHelper.GetClosestPoint(polygon.GetPointListP2(),
//                                                                                  profileStartPoint);

//                                var temp = new Point2D(profileStartPoint);

//                                var newStartPoint = GeometryHelper.GetPointAtDistance(profileStartPoint,
//                                                                                      closestPoint,
//                                                                                      extraCorsa + diaFresa, true,
//                                                                                      false);

//                                profileStartPoint = newStartPoint;

//                                if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                                {
//                                    // todo : gestire se punto trovato si trova cmq sempre dentro il polygono.
//                                }

//                            }

//                            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X,
//                                                       profileStartPoint.Y, null);
//                            programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null,
//                                                       zCurrent);

//                            // qui abilitare la compensazione utesile )
//                        }
//                        else
//                        {
//                            // qui abilitare la compensazione utesile )

//                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, profileStartPoint.X,
//                                                       profileStartPoint.Y,
//                                                       null);
//                        }
//                    }
//                    if (source.Source[0] is Line2D)
//                    {
//                        var firstMove = ((Line2D)element.Source[0]).Start;

//                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//                    }
//                    else if (element.Source[0] is Arc2D)
//                    {
//                        /*
//                         * todo . controllare con arco come primo elemento.come nel caso sopra , dove spostamento è maggiore dia fresa.
//                         */
//                    }

//                    foreach (var entity2D in source.Source)
//                    {
//                        if (entity2D is Line2D)
//                        {
//                            var line = entity2D as Line2D;

//                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);

//                            lastMovePoint = new Point2D(line.End);
//                        }

//                        else if (entity2D is Arc2D)
//                        {
//                            // implementare archi 

//                            var arc = entity2D as Arc2D;

//                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, arc.Start.X, arc.Start.Y,
//                                                       zCurrent);

//                            programPhase.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, zCurrent, arc.Radius,
//                                                    arc.ClockWise, arc.Center);

//                            lastMovePoint = new Point2D(arc.End);
//                        }
//                    }

//                }

//                // Mi sposto in z sicurezza
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


//            }

//        }


//        /// <summary>
//        /// Vers.2
//        /// - Sovrametallo finitura 
//        /// - Per polygono di trimming incapsularlo in una classe.
//        /// 
//        /// </summary>
//        /// <param name="programPhase"></param>
//        /// <param name="diaFresa"></param>
//        /// <param name="larghPassata"></param>
//        /// <param name="profPassata"></param>
//        /// <param name="extraCorsa"></param>
//        /// <param name="sovraMetallo"></param>
//        /// <param name="zIniziale"></param>
//        /// <param name="profondita"></param>
//        /// <param name="zSicurezza"></param>
//        /// <param name="profile2D"></param>
//        /// <param name="tpCx"></param>
//        /// <param name="tpCy"></param>
//        /// <param name="tpH"></param>
//        /// <param name="tpW"></param>
//        /// <param name="tpAbilited"></param>
//        private static void FresaturaSgrossaturaContornaturaV2(MoveActionCollection programPhase, double diaFresa, double larghPassata, double profPassata, double extraCorsa,
//                                                        double sovraMetallo, double zIniziale, double profondita, double zSicurezza, Profile2D profile2D, double tpCx, double tpCy, double tpH, double tpW, bool tpAbilited)
//        {
//            /*
//             * Controllo Valori
//             */
//            if (CheckValueHelper.GreatherOrEqualZero(new[] { extraCorsa }) ||
//                CheckValueHelper.GreatherThanZero(new[] { sovraMetallo, profondita, larghPassata, diaFresa, profPassata, }) ||
//                CheckValueHelper.GreatherThan(new[]
//                                              {
//                                                  new KeyValuePair<double, double>(zSicurezza, zIniziale),
//                                                  new KeyValuePair<double, double>(diaFresa, larghPassata),
//                                              })
//                ) return;

//            if (profile2D == null) return;

//            /*
//             * OSSERVAZIONI PER STOCK TRIMMING
//             * - Lo stock deve essere minimo il bounding Box del profilo. per evitare complicazioni.
//             * - I Profili che genero devo avere già integrato lo spostamento di avvicinamento.
//             * - Magari i profili che genero non serve che siano della classe Profile2D ma basta che siano un set di punti.
//             */

//            /*
//             * Le passate le costruisco dalla prima ( offset raggio fresa ), incrementando della larghezza passata fino a coprire tutto il sovrametallo
//             */

//            var raggioFresa = diaFresa / 2;

//            var distanzaOffsetCorrente = sovraMetallo + raggioFresa;

//            var continueCycle = distanzaOffsetCorrente > 0;

//            var offsetCountorns = new List<Profile2D>();

//            var distanzaAttacco = sovraMetallo + raggioFresa + extraCorsa;

//            var attacPath = profile2D.Offset(distanzaAttacco, true);

//            if (attacPath == null) return;

//            var attacPoint = attacPath.First().Source.FirstOrDefault();

//            if (attacPoint == null)
//                throw new Exception();

//            var attachPoint = attacPoint.GetFirstPnt();
//            /*
//             * qui genero i profili di contornatura,
//             * 
//             * Ad ogni profilo generato aggiungo anche il punto di attacco
//             * ( il punto finale del profilo precedente..)
//             */
//            while (continueCycle)
//            {
//                distanzaOffsetCorrente -= larghPassata;

//                if (distanzaOffsetCorrente < raggioFresa)
//                {
//                    distanzaOffsetCorrente = raggioFresa;
//                    continueCycle = false;
//                }

//                var path = profile2D.Offset(distanzaOffsetCorrente, true);

//                /*
//                 * Nel caso della contornatura esterna prendo solamente il primo profilo
//                 */
//                if (path.Count > 0)
//                {
//                    var p = path[0];

//                    p.ToolPathStartPoint = attachPoint;

//                    offsetCountorns.Add(path[0]);

//                    /*
//                     * Inserisco attac point,
//                     */

//                    attachPoint = p.Source.Last().GetLastPnt();
//                }
//            }
//            // stock trimming experiment ..
//            var offsetCounterStockTrimmed = new List<Profile2D>();

//            var pp1 = new Point2D(tpCx + tpW / 2, tpCy + tpH / 2);
//            var pp2 = new Point2D(tpCx + tpW / 2, tpCy - tpH / 2);
//            var pp3 = new Point2D(tpCx - tpW / 2, tpCy - tpH / 2);
//            var pp4 = new Point2D(tpCx - tpW / 2, tpCy + tpH / 2);

//            var polygon = new Profile2D();

//            polygon.AddPnt(pp1);
//            polygon.AddPnt(pp2);
//            polygon.AddPnt(pp3);
//            polygon.AddPnt(pp4);
//            polygon.AddPnt(pp1);


//            if (tpAbilited) // se la funzione di trimm del profilo è abilitata
//            {
//                /*
//                 * verificare bb del profilo
//                 */


//                foreach (var offsetCountorn in offsetCountorns)
//                {
//                    var rs = GeometryHelper.TrimProfileByStockPolygon(offsetCountorn, polygon, raggioFresa + extraCorsa);

//                    // aggiungere distanza di sicurezza , estendere linee
//                    if (rs != null)
//                        offsetCounterStockTrimmed.AddRange(rs);

//                }
//            }
//            else
//                offsetCounterStockTrimmed = offsetCountorns;

//            var zCurrent = zIniziale;

//            var zFinale = zIniziale - profondita;

//            // Itero per tutte le passate in Z
//            while (zCurrent > zFinale)
//            {
//                zCurrent -= profPassata;
//                if (zCurrent < zFinale)
//                    zCurrent = zFinale;

//                if (offsetCounterStockTrimmed.Count <= 0)
//                    return;

//                Point2D lastMovePoint = null;

//                for (var i = 0; i < offsetCounterStockTrimmed.Count; i++)
//                {
//                    var element = offsetCounterStockTrimmed[i];

//                    // Faccio parse per risolvere archi
//                    var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

//                    //var source = element;
//                    var source = parsedCountor;

//                    /*
//                     * se punto iniziale si trova dentro profilo fare :
//                     * - trovare nearest point da punto attacco e profilo grezzo  
//                     * - fare estensione segmento punto iniziale - punto vicino 
//                     * - punto attacco è punto esteso.
//                     */
//                    var profileStartPoint = source.ToolPathStartPoint ?? source.Source.First().GetFirstPnt();


//                    if (lastMovePoint == null)
//                    {

//                        if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                        {

//                        }

//                        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y, null);
//                        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                        programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
//                    }
//                    else
//                    {
//                        var distance = GeometryHelper.Distance(profileStartPoint, lastMovePoint);

//                        if (distance > diaFresa)
//                        {
//                            if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                            {
//                                var closestPoint = GeometryHelper.GetClosestPoint(polygon.GetPointListP2(), profileStartPoint);

//                                var temp = new Point2D(profileStartPoint);

//                                var newStartPoint = GeometryHelper.GetPointAtDistance(profileStartPoint, closestPoint,
//                                                                                      extraCorsa + diaFresa, true, false);

//                                profileStartPoint = newStartPoint;

//                                if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                                {
//                                    // todo : gestire se punto trovato si trova cmq sempre dentro il polygono.
//                                }

//                            }

//                            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y, null);
//                            programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
//                        }
//                        else
//                        {
//                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y,
//                                                       null);
//                        }
//                    }
//                    if (source.Source[0] is Line2D)
//                    {
//                        var firstMove = ((Line2D)element.Source[0]).Start;

//                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//                    }
//                    else if (element.Source[0] is Arc2D)
//                    {
//                        /*
//                         * todo . controllare con arco come primo elemento.come nel caso sopra , dove spostamento è maggiore dia fresa.
//                         */
//                    }

//                    foreach (var entity2D in source.Source)
//                    {
//                        if (entity2D is Line2D)
//                        {
//                            var line = entity2D as Line2D;

//                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);

//                            lastMovePoint = new Point2D(line.End);
//                        }

//                        else if (entity2D is Arc2D)
//                        {
//                            // implementare archi 

//                            var arc = entity2D as Arc2D;

//                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, arc.Start.X, arc.Start.Y, zCurrent);

//                            programPhase.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, zCurrent, arc.Radius, arc.ClockWise, arc.Center);

//                            lastMovePoint = new Point2D(arc.End);
//                        }
//                    }

//                }

//                // Mi sposto in z sicurezza
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


//            }

//        }

//        private static void FresaturaSgrossaturaContornatura(MoveActionCollection programPhase, double diaFresa, double larghPassata, double profPassata, double extraCorsa,
//                                                            double sovraMetallo, double zIniziale, double profondita, double zSicurezza, Profile2D profile2D, Profile2D stockProfile, bool tpAbilited)
//        {
//            /*
//             * Controllo Valori
//             */
//            if (CheckValueHelper.GreatherOrEqualZero(new[] { extraCorsa }) ||
//                CheckValueHelper.GreatherThanZero(new[] { sovraMetallo, profondita, larghPassata, diaFresa, profPassata, }) ||
//                CheckValueHelper.GreatherThan(new[]
//                                              {
//                                                  new KeyValuePair<double, double>(zSicurezza, zIniziale),
//                                                  new KeyValuePair<double, double>(diaFresa, larghPassata),
//                                              })
//                ) return;

//            if (profile2D == null) return;

//            /*
//             * OSSERVAZIONI PER STOCK TRIMMING
//             * - Lo stock deve essere minimo il bounding Box del profilo. per evitare complicazioni.
//             * - I Profili che genero devo avere già integrato lo spostamento di avvicinamento.
//             * - Magari i profili che genero non serve che siano della classe Profile2D ma basta che siano un set di punti.
//             */

//            /*
//             * Le passate le costruisco dalla prima ( offset raggio fresa ), incrementando della larghezza passata fino a coprire tutto il sovrametallo
//             */

//            var raggioFresa = diaFresa / 2;

//            var distanzaOffsetCorrente = sovraMetallo + raggioFresa;

//            var continueCycle = distanzaOffsetCorrente > 0;

//            var offsetCountorns = new List<Profile2D>();

//            var distanzaAttacco = sovraMetallo + raggioFresa + extraCorsa;

//            var attacPath = profile2D.Offset(distanzaAttacco, true);

//            if (attacPath == null) return;

//            var attacPoint = attacPath.First().Source.FirstOrDefault();

//            if (attacPoint == null)
//                throw new Exception();

//            var attachPoint = attacPoint.GetFirstPnt();
//            /*
//             * qui genero i profili di contornatura,
//             * 
//             * Ad ogni profilo generato aggiungo anche il punto di attacco
//             * ( il punto finale del profilo precedente..)
//             */
//            while (continueCycle)
//            {
//                distanzaOffsetCorrente -= larghPassata;

//                if (distanzaOffsetCorrente < raggioFresa)
//                {
//                    distanzaOffsetCorrente = raggioFresa;
//                    continueCycle = false;
//                }

//                var path = profile2D.Offset(distanzaOffsetCorrente, true);

//                /*
//                 * Nel caso della contornatura esterna prendo solamente il primo profilo
//                 */
//                if (path.Count > 0)
//                {
//                    var p = path[0];

//                    p.ToolPathStartPoint = attachPoint;

//                    offsetCountorns.Add(path[0]);

//                    /*
//                     * Inserisco attac point,
//                     */

//                    attachPoint = p.Source.Last().GetLastPnt();
//                }
//            }
//            // stock trimming experiment ..
//            var offsetCounterStockTrimmed = new List<Profile2D>();

//            var polygon = stockProfile;

//            if (tpAbilited) // se la funzione di trimm del profilo è abilitata
//            {
//                /*
//                 * verificare bb del profilo
//                 */


//                foreach (var offsetCountorn in offsetCountorns)
//                {
//                    var rs = GeometryHelper.TrimProfileByStockPolygon(offsetCountorn, polygon, raggioFresa + extraCorsa);

//                    // aggiungere distanza di sicurezza , estendere linee
//                    if (rs != null)
//                        offsetCounterStockTrimmed.AddRange(rs);

//                }
//            }
//            else
//                offsetCounterStockTrimmed = offsetCountorns;

//            var zCurrent = zIniziale;

//            var zFinale = zIniziale - profondita;

//            // Itero per tutte le passate in Z
//            while (zCurrent > zFinale)
//            {
//                zCurrent -= profPassata;
//                if (zCurrent < zFinale)
//                    zCurrent = zFinale;

//                if (offsetCounterStockTrimmed.Count <= 0)
//                    return;

//                Point2D lastMovePoint = null;

//                for (var i = 0; i < offsetCounterStockTrimmed.Count; i++)
//                {
//                    var element = offsetCounterStockTrimmed[i];

//                    // Faccio parse per risolvere archi
//                    var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

//                    //var source = element;
//                    var source = parsedCountor;

//                    /*
//                     * se punto iniziale si trova dentro profilo fare :
//                     * - trovare nearest point da punto attacco e profilo grezzo  
//                     * - fare estensione segmento punto iniziale - punto vicino 
//                     * - punto attacco è punto esteso.
//                     */
//                    var profileStartPoint = source.ToolPathStartPoint ?? source.Source.First().GetFirstPnt();


//                    if (lastMovePoint == null)
//                    {

//                        if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                        {

//                        }

//                        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y, null);
//                        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                        programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
//                    }
//                    else
//                    {
//                        var distance = GeometryHelper.Distance(profileStartPoint, lastMovePoint);

//                        if (distance > diaFresa)
//                        {
//                            if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                            {
//                                var closestPoint = GeometryHelper.GetClosestPoint(polygon.GetPointListP2(), profileStartPoint);

//                                var temp = new Point2D(profileStartPoint);

//                                var newStartPoint = GeometryHelper.GetPointAtDistance(profileStartPoint, closestPoint,
//                                                                                      extraCorsa + diaFresa, true, false);

//                                profileStartPoint = newStartPoint;

//                                if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                                {
//                                    // todo : gestire se punto trovato si trova cmq sempre dentro il polygono.
//                                }

//                            }

//                            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y, null);
//                            programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
//                        }
//                        else
//                        {
//                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y,
//                                                       null);
//                        }
//                    }
//                    if (source.Source[0] is Line2D)
//                    {
//                        var firstMove = ((Line2D)element.Source[0]).Start;

//                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//                    }
//                    else if (element.Source[0] is Arc2D)
//                    {
//                        /*
//                         * todo . controllare con arco come primo elemento.come nel caso sopra , dove spostamento è maggiore dia fresa.
//                         */
//                    }

//                    foreach (var entity2D in source.Source)
//                    {
//                        if (entity2D is Line2D)
//                        {
//                            var line = entity2D as Line2D;

//                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);

//                            lastMovePoint = new Point2D(line.End);
//                        }

//                        else if (entity2D is Arc2D)
//                        {
//                            // implementare archi 

//                            var arc = entity2D as Arc2D;

//                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, arc.Start.X, arc.Start.Y, zCurrent);

//                            programPhase.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, zCurrent, arc.Radius, arc.ClockWise, arc.Center);

//                            lastMovePoint = new Point2D(arc.End);
//                        }
//                    }

//                }

//                // Mi sposto in z sicurezza
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


//            }

//        }