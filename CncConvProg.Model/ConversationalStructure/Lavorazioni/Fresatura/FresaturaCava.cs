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
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;

/*
 * 
 * meglio aggiungere pattern se poi ne ho occasione dare opt. abilitato o meno..
 * 
 */

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura
{
    [Serializable]
    public sealed class FresaturaCava : LavorazioneFresatura, IMillingPatternable, IMillWorkable
    {
        public FresaturaCava(Guid parent)
            : base(parent)
        {
            MillingPattern = EnumPatternMilling.PoligonoRegolare;

            Sgrossatura = new Operazione(this, LavorazioniEnumOperazioni.Sgrossatura);
            Finitura = new Operazione(this, LavorazioniEnumOperazioni.Finitura);
            //CentrinoForoApertura = new Operazione(this, (int)FresaturaEnumOperazioni.Centrino);
            //ForaturaApertura = new Operazione(this, (int)FresaturaEnumOperazioni.Foratura);
            Smussatura = new Operazione(this, LavorazioniEnumOperazioni.Smussatura);


        }

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

        public double InizioLavorazioneZ { get; set; }

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

        //public Operazione CentrinoForoApertura { get; set; }

        //public Operazione ForaturaApertura { get; set; }

        public double ProfonditaFresaSmussatura { get; set; }

        public bool FinishWithCompensation { get; set; }

        public double SovrametalloFinituraProfilo { get; set; }

        public double SovrametalloFinituraZ { get; set; }

        public bool ChamferWithCompensation { get; set; }

        //internal override Utensile CreateTool(LavorazioniEnumOperazioni enumOperationType)
        //{
        //    var unit = Singleton.Instance.MeasureUnit;

        //    return FresaturaHelper.CreateTool((LavorazioniEnumOperazioni)enumOperationType, unit);
        //}

        //internal override List<Utensile> GetCompatibleTools(LavorazioniEnumOperazioni operationType, MagazzinoUtensile magazzino)
        //{
        //    // todo su nessun utensile probabilmente crasha..
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

            var moveCollection = new MoveActionCollection();

            var workProfile = GetProfile();

            switch (operazione.OperationType)
            {
                case LavorazioniEnumOperazioni.Sgrossatura:
                    {
                        var sovraXy = SovrametalloFinituraProfilo;

                        if (!Finitura.Abilitata)
                            sovraXy = 0;

                        MillProgrammingHelper.GetInternRoughing(moveCollection, workProfile, ProfonditaLavorazione,
                                                                profPassata, larghezzaPassata, diaFresa, ExtraCorsa,
                                                                 InizioLavorazioneZ, SicurezzaZ,
                                                                sovraXy, SovrametalloFinituraZ);

                    } break;

                case LavorazioniEnumOperazioni.Finitura:
                    {
                        MillProgrammingHelper.GetInternFinish(moveCollection, workProfile, ProfonditaLavorazione, profPassata, diaFresa, ExtraCorsa, FinishWithCompensation, InizioLavorazioneZ, SicurezzaZ);
                    } break;

                case LavorazioniEnumOperazioni.Smussatura:
                    {
                        MillProgrammingHelper.GetInternChamfer(moveCollection, workProfile, ProfonditaFresaSmussatura,
                                                               diaFresa, ExtraCorsa, ChamferWithCompensation,
                                                               InizioLavorazioneZ, SicurezzaZ);
                    } break;

                default:
                    {
                        Debug.Fail("FresaturaCava.GetOperationProgram");
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
            get { return "Pocket"; }
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
                    var preview = Pattern.GetClosedProfile().Source;

                    return Entity3DHelper.Get3DProfile(preview).ToList();
                }

            }
            catch (Exception ex)
            {
            }

            return null;
        }

    }


}












//internal override ProgramPhase GetOperationProgram(Operazione operazione)
//{
//    var program = new ProgramPhase(SicurezzaZ);

//    var toolChange = new ChangeToolAction(program, operazione);

//    program.ActiveAsseC(true);


//    var fresa = operazione.Utensile as FresaCandela;

//    var parametro = operazione.Utensile.ParametroUtensile as ParametroFresaCandela;

//    if (fresa == null || parametro == null)
//        throw new NullReferenceException();

//    var diaFresa = fresa.DiametroFresa;

//    var larghezzaPassata = parametro.GetLarghezzaPassata();

//    var profPassata = parametro.GetProfonditaPassata();

//    var feed = parametro.GetFeed(FeedType.ASync);

//    var plungeFeed = parametro.AvanzamentoAsincronoPiantata.Value.Value;

//    //var secureFeed = FaseDiLavoro.Model.GetSecureFeed();

//    //var extraCorsa = FaseDiLavoro.Model.GetExtracorsa();
//    var secureFeed = 1;

//    var extraCorsa = 1;

//    var moveCollection = new MoveActionCollection();

//    if (feed <= 0)
//        return null;

//    var feedDictionary = new Dictionary<MoveType, double>
//                             {
//                                 {MoveType.Rapid, 10000},
//                                 {MoveType.SecureRapidFeed, secureFeed},
//                                 {MoveType.Work, feed},
//                                 {MoveType.Cw, feed},
//                                 {MoveType.Ccw, feed},
//                                 {MoveType.PlungeFeed, plungeFeed}
//                             };

//    program.SetFeedDictionary(feedDictionary);


//    switch ((LavorazioniEnumOperazioni)operazione.OperationType)
//    {
//        case LavorazioniEnumOperazioni.Sgrossatura:
//            {
//                FresaturaSgrossaturaCava(moveCollection, GetProfile(), ProfonditaLavorazione, SicurezzaZ, InizioLavorazioneZ, diaFresa, larghezzaPassata, profPassata, extraCorsa);
//            } break;

//        case LavorazioniEnumOperazioni.Finitura:
//            {
//                FresaturaFinituraCava(moveCollection, GetProfile(), ProfonditaLavorazione, SicurezzaZ, InizioLavorazioneZ, diaFresa, larghezzaPassata, profPassata, extraCorsa, FinishWithCompensation);
//            } break;

//        case LavorazioniEnumOperazioni.Smussatura:
//            {
//                MillProgrammingHelper.ProcessProfileChamferMilling(moveCollection, GetProfile(),
//                                                                   ProfonditaLavorazione, diaFresa, SicurezzaZ,
//                                                                   extraCorsa, InizioLavorazioneZ, false);
//            } break;

//        default:
//            {
//                Debug.Fail("FresaturaCava.GetOperationProgram");
//            } break;

//    }
//    var mm = base.GetFinalProgram(moveCollection);

//    foreach (var variable in mm)
//    {
//        program.AddMoveAction(variable);
//    }

//    program.ActiveAsseC(false);
//    return program;
//}


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

//       /* 
//        * mi serve 
//        * 1- metodo che mi restituisca i profili calcolati
//        * 
//        * 
//        * 
//        */

//       private static List<Profile2D> CalculateInternOffset(Profile2D origin, double offsetValue, double radiusValue)
//       {
//           offsetValue = -Math.Abs(offsetValue);

//           var firstPaths = origin.Offset(-radiusValue, false);

//           if (firstPaths == null)
//               return null;

//           var rslt = new List<Profile2D>();

//           foreach (var firstPath in firstPaths)
//           {
//               rslt.Add(firstPath);

//               RicorsivaGenerateInternOffset(firstPath, offsetValue, false, ref rslt);
//           }


//           return rslt;
//       }

//       private static void RicorsivaGenerateInternOffset(Profile2D profile2D, double offset, bool clockwise, ref List<Profile2D> profile2DsList)
//       {
//           // Calcola offset , ritorna 1 o più contorni 
//           var offsetRslt = profile2D.Offset(offset, clockwise);


//           // se non ritorna più niente termina metodo 
//           if (offsetRslt == null)
//               return;

//           foreach (var singleContour in offsetRslt)
//           {
//               profile2DsList.Add(singleContour);
//               RicorsivaGenerateInternOffset(singleContour, offset, clockwise, ref profile2DsList);
//           }
//       }

//       private static void FresaturaSgrossaturaCava(MoveActionCollection programPhase, Profile2D profile2D, double profondita, double zSicurezza, double zIniziale, double diaFresa, double larghPassata, double profPassata)
//       {
//           /*
//            * teoria.
//            * - Prendo profilo 
//            *  - Se valido faccio offset negativo del raggio della fresa
//            * - Poi faccio offset della larghezza di passata fino a che il metodo non mi restituisce più niente. ( ho raggiunto il massimo ) 
//            */

//           /*
//            * Controllo Valori
//            */
//           if (CheckValueHelper.GreatherThanZero(new[] { profondita, larghPassata, diaFresa, profPassata, }) ||
//               CheckValueHelper.GreatherThan(new[]
//                                             {
//                                                 new KeyValuePair<double, double>(zSicurezza, zIniziale),
//                                                 new KeyValuePair<double, double>(diaFresa, larghPassata),
//                                             })
//               ) return;

//           if (profile2D == null) return;

//           var raggioFresa = diaFresa / 2;

//           var profile = profile2D;

//           var offsetCountorns = CalculateInternOffset(profile, larghPassata, diaFresa / 2);

//           if (offsetCountorns == null)
//               return;

//           /*
//* Devo spostarmi in xy del primo punto 
//*/
//           var attacPath = offsetCountorns.FirstOrDefault();

//           if (attacPath == null)
//               return;

//           var attacPoint = attacPath.Source.FirstOrDefault();

//           if (attacPoint == null)
//               throw new Exception();

//           programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, attacPoint.GetFirstPnt().X, attacPoint.GetFirstPnt().Y, null);

//           var zCurrent = zIniziale;

//           var zFinale = zIniziale - profondita;

//           programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


//           // Itero per tutte le passate in Z
//           while (zCurrent > zFinale)
//           {
//               zCurrent -= profPassata;
//               if (zCurrent < zFinale)
//                   zCurrent = zFinale;

//               if (offsetCountorns.Count <= 0)
//                   return;

//               Point2D lastMovePoint = null;

//               for (var i = offsetCountorns.Count - 1; i >= 0; i--)
//               {
//                   var element = offsetCountorns[i];

//                   var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

//                   var source = parsedCountor;

//                   //var source = element; todo , su elementi roto traslati spostarsi in rapido , probabilmente il codice da mdificare non [ qua lo trovi nella ripetiozone programm , non forse [ questo..

//                   if (source.Source[0] is Line2D)
//                   {
//                       var firstMove = ((Line2D)element.Source[0]).Start;

//                       if (lastMovePoint != null)
//                       {
//                           /* Qui guardo che se lo spostamento che devo effettuare è maggiore del raggio fresa , 
//                            * 1)  mi alzo 
//                            * 2) mi sposto in rapido
//                            * 3) mi pianto
//                            */

//                           var distance = GeometryHelper.Distance(firstMove, lastMovePoint);

//                           if (distance > diaFresa)
//                           {
//                               programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                               programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//                           }
//                           else

//                               programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);


//                       }
//                       programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);

//                       // Movimento in piantata a z di lavoro..
//                       programPhase.AddLinearMove(MoveType.PlungeFeed, AxisAbilited.Z, null, null, zCurrent);
//                   }
//                   else if (element.Source[0] is Arc2D)
//                   {
//                       /*
//                        * todo . controllare con arco come primo elemento.come nel caso sopra , dove spostamento è maggiore dia fresa.
//                        */
//                   }

//                   foreach (var entity2D in source.Source)
//                   {
//                       if (entity2D is Line2D)
//                       {
//                           var line = entity2D as Line2D;

//                           programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);

//                           lastMovePoint = new Point2D(line.End.X, line.End.Y);
//                       }

//                       else if (entity2D is Arc2D)
//                       {
//                           // implementare archi 

//                           var arc = entity2D as Arc2D;
//                           programPhase.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, null, arc.Radius, arc.ClockWise, arc.Center);

//                           /*
//                            * todo anche arco puo essere primo elemento
//                            */
//                           lastMovePoint = new Point2D(arc.End.X, arc.End.Y);
//                       }
//                   }

//               }

//               //    /*
//               //     * todo: qui a seconda di cosa ho scelto esco a zIniziale oppure mi pianto direttamente
//               //     */

//               // Vado a z iniziale
//               programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//           }

//       }

//       private static void FresaturaFinituraCava(MoveActionCollection programPhase, Profile2D profile2D, double profondita, double zSicurezza, double zIniziale, double diaFresa, double larghPassata, double profPassata, bool isCncCompensated)
//       {
//           /*
//            * teoria.
//            * - Prendo profilo 
//            *  - Se valido faccio offset negativo del raggio della fresa
//            * - Poi faccio offset della larghezza di passata fino a che il metodo non mi restituisce più niente. ( ho raggiunto il massimo ) 
//            */

//           /*
//            * Controllo Valori
//            */
//           if (CheckValueHelper.GreatherThanZero(new[] { profondita, larghPassata, diaFresa, profPassata, }) ||
//               CheckValueHelper.GreatherThan(new[]
//                                             {
//                                                 new KeyValuePair<double, double>(zSicurezza, zIniziale),
//                                                 new KeyValuePair<double, double>(diaFresa, larghPassata),
//                                             })
//               ) return;

//           if (profile2D == null) return;

//           var raggioFresa = diaFresa / 2;

//           var profile = profile2D;

//           var finishContour = new List<Profile2D>();

//           if (isCncCompensated)
//           {
//               finishContour.Add(profile);
//           }
//           else
//           {
//               var firstPaths = profile.Offset(-raggioFresa, false);

//               if (firstPaths == null)
//                   return;

//               foreach (var firstPath in firstPaths)
//               {
//                   finishContour.Add(firstPath);
//               }
//           }


//           // Devo spostarmi in xy del primo punto 

//           var attacPath = finishContour.FirstOrDefault();

//           if (attacPath == null)
//               return;

//           var attacPoint = attacPath.Source.FirstOrDefault();

//           if (attacPoint == null)
//               throw new Exception();

//           programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, attacPoint.GetFirstPnt().X, attacPoint.GetFirstPnt().Y, null);

//           var zCurrent = zIniziale;

//           var zFinale = zIniziale - profondita;

//           programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


//           // Itero per tutte le passate in Z
//           while (zCurrent > zFinale)
//           {
//               zCurrent -= profPassata;
//               if (zCurrent < zFinale)
//                   zCurrent = zFinale;

//               if (finishContour.Count <= 0)
//                   return;

//               Point2D lastMovePoint = null;

//               for (var i = finishContour.Count - 1; i >= 0; i--)
//               {
//                   var element = finishContour[i];

//                   var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

//                   var source = parsedCountor;

//                   //var source = element; todo , su elementi roto traslati spostarsi in rapido , probabilmente il codice da mdificare non [ qua lo trovi nella ripetiozone programm , non forse [ questo..

//                   if (source.Source[0] is Line2D)
//                   {
//                       var firstMove = ((Line2D)element.Source[0]).Start;

//                       if (lastMovePoint != null)
//                       {
//                           /* Qui guardo che se lo spostamento che devo effettuare è maggiore del raggio fresa , 
//                            * 1)  mi alzo 
//                            * 2) mi sposto in rapido
//                            * 3) mi pianto
//                            */

//                           var distance = GeometryHelper.Distance(firstMove, lastMovePoint);

//                           if (distance > diaFresa)
//                           {
//                               programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                               programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//                           }
//                           else

//                               programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);


//                       }
//                       programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);

//                       // Movimento in piantata a z di lavoro..
//                       programPhase.AddLinearMove(MoveType.PlungeFeed, AxisAbilited.Z, null, null, zCurrent);
//                   }
//                   else if (element.Source[0] is Arc2D)
//                   {
//                       /*
//                        * todo . controllare con arco come primo elemento.come nel caso sopra , dove spostamento è maggiore dia fresa.
//                        */
//                   }

//                   foreach (var entity2D in source.Source)
//                   {
//                       if (entity2D is Line2D)
//                       {
//                           var line = entity2D as Line2D;

//                           programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);

//                           lastMovePoint = new Point2D(line.End.X, line.End.Y);
//                       }

//                       else if (entity2D is Arc2D)
//                       {
//                           // implementare archi 

//                           var arc = entity2D as Arc2D;
//                           programPhase.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, null, arc.Radius, arc.ClockWise, arc.Center);

//                           /*
//                            * todo anche arco puo essere primo elemento
//                            */
//                           lastMovePoint = new Point2D(arc.End.X, arc.End.Y);
//                       }
//                   }

//               }

//               //    /*
//               //     * todo: qui a seconda di cosa ho scelto esco a zIniziale oppure mi pianto direttamente
//               //     */

//               // Vado a z iniziale
//               programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//           }

//       }