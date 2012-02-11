using System;
using System.Collections.Generic;
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
    public sealed class FresaturaScanalaturaChiusa : LavorazioneFresatura, IMillingPatternable, IMillWorkable
    {
        public FresaturaScanalaturaChiusa()
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

        public double Larghezza { get; set; }

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

        public double ProfonditaFresaSmussatura { get; set; }

        public bool FinishWithCompensation { get; set; }

        public double SovrametalloFinituraProfilo { get; set; }

        //public Operazione CentrinoForoApertura { get; set; }

        //public Operazione ForaturaApertura { get; set; }



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
        /* 
         * mi serve 
         * 1- metodo che mi restituisca i profili calcolati
         * 
         * 
         * 
         */

        //private static List<Profile2D> CalculateInternOffset(Profile2D origin, double offsetValue, double diameterValue)
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




        private static void FresaturaSmussaturaScanalaturaChiusaProgram(MoveActionCollection moveActionCollection, Profile2D profile2D, double profonditaSmusso, double zSicurezza, double zIniziale, double diaFresa, double larghezzaScanaltura)
        {
            /*
             * teoria.
             * - Prendo profilo 
             *  - Se valido faccio offset negativo del raggio della fresa
             * - Poi faccio offset della larghezza di passata fino a che il metodo non mi restituisce più niente. ( ho raggiunto il massimo ) 
             */

            /*
             * Controllo Valori
             */
            if (CheckValueHelper.GreatherThanZero(new[] { profonditaSmusso, diaFresa, larghezzaScanaltura }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(zSicurezza, zIniziale),
                                              })
                ) return;

            if (profile2D == null) return;

            /*
             * chiamo metodo comune per 2 profili (maggiore e minore della fresatura di scanalatura)
             */
            var offsetValue = larghezzaScanaltura / 2;

            var profileExt = profile2D.Offset(offsetValue, true);
            var profileInt = profile2D.Offset(-offsetValue, true);

            if ((profileExt == null || profileExt.Count == 0) ||
                (profileInt == null || profileInt.Count == 0)) return;

            MillProgrammingHelper.GetExterChamfer(moveActionCollection, profileInt[0], null, false, profonditaSmusso, diaFresa, 0, false, zIniziale, zSicurezza);

            MillProgrammingHelper.GetInternChamfer(moveActionCollection, profileExt[0], profonditaSmusso, diaFresa, 0, false, zIniziale, zSicurezza);

        }

        protected override void CreateSpecificProgram(ProgramOperation programPhase, Operazione operazione)
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


            switch (operazione.OperationType)
            {
                case LavorazioniEnumOperazioni.Sgrossatura:
                    {
                        MillProgrammingHelper.FresaturaScanalaturaChiusaProgram(moveCollection, GetProfile(), ProfonditaLavorazione, Larghezza, SicurezzaZ, InizioLavorazioneZ, diaFresa, larghezzaPassata, profPassata, ExtraCorsa, SovrametalloFinituraProfilo, FinishWithCompensation);
                    } break;

                case LavorazioniEnumOperazioni.Finitura:
                    {
                        MillProgrammingHelper.FresaturaScanalaturaChiusaProgram(moveCollection, GetProfile(), ProfonditaLavorazione, Larghezza, SicurezzaZ, InizioLavorazioneZ, diaFresa, larghezzaPassata, profPassata, ExtraCorsa, 0, FinishWithCompensation);
                    } break;

                case LavorazioniEnumOperazioni.Smussatura:
                    {
                        FresaturaSmussaturaScanalaturaChiusaProgram(moveCollection, GetProfile(), ProfonditaLavorazione, SicurezzaZ, InizioLavorazioneZ, diaFresa, Larghezza);

                    } break;

            }
            var mm = base.GetFinalProgram(moveCollection);

            foreach (var variable in mm)
            {
                programPhase.AggiungiAzioneMovimento(variable);
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

        //    var secureFeed = 1; // fare

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
        //                FresaturaScanalaturaChiusaProgram(moveCollection, GetProfile(), ProfonditaLavorazione, Larghezza, SicurezzaZ, InizioLavorazioneZ, diaFresa, larghezzaPassata, profPassata, extraCorsa);
        //            } break;

        //        case LavorazioniEnumOperazioni.Finitura:
        //            {
        //                FresaturaScanalaturaChiusaProgram(moveCollection, GetProfile(), ProfonditaLavorazione, Larghezza, SicurezzaZ, InizioLavorazioneZ, diaFresa, larghezzaPassata, profPassata, extraCorsa);
        //            } break;

        //        case LavorazioniEnumOperazioni.Smussatura:
        //            {
        //                FresaturaSmussaturaScanalaturaChiusaProgram(moveCollection, GetProfile(), ProfonditaLavorazione, SicurezzaZ, InizioLavorazioneZ, diaFresa, Larghezza);

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

        public override string Descrizione
        {
            get { return "Slot"; }
        }

        /// <summary>
        /// Ritorna anteprima della lavorazione
        /// </summary>
        /// <returns></returns>
        protected override List<IEntity3D> GetFinalPreview()
        {
            if (Pattern != null)
            {
                /*
                 * qui magari fare ovverride per schermo profilo ..
                 * oppure quando width è 0 creo solo un profilo orginale
                 * 
                 * per sbocciare magari fare la 3d wirewframe
                 * 
                 */
                var preview = Pattern.GetClosedProfile();
                /*
                 * faccio offset esterno e uno interno
                 */

                var offsetValue = Larghezza / 2;

                var profileExt = preview.Offset(offsetValue, true);
                var profileInt = preview.Offset(-offsetValue, true);

                if ((profileExt != null && profileExt.Count > 0) &&
                    (profileInt != null && profileInt.Count > 0))
                {
                    var rslt = new List<IEntity2D>();


                    profileExt[0].SetPlotStyle();
                    profileInt[0].SetPlotStyle();

                    rslt.AddRange(profileExt[0].Source);
                    rslt.AddRange(profileInt[0].Source);

                    return Entity3DHelper.Get3DProfile(rslt).ToList();
                }
            }


            return null;
        }

    }


}


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
