﻿using System;
using System.Collections.Generic;
using System.Linq;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura
{
    [Serializable]
    public abstract class DrillBaseClass : LavorazioneFresatura, IForaturaPatternable, IDrillDiameter, IForaturaAble, ICentrinoAble, ISvasaturaAble
    {
        public override FaseDiLavoro.TipoFaseLavoro[] FasiCompatibili
        {
            get
            {
                if (ForaturaCentraleTornio)
                {
                    return new[]
                               {
                                   Abstraction.FaseDiLavoro.TipoFaseLavoro.Tornio3Assi,
                                   Abstraction.FaseDiLavoro.TipoFaseLavoro.Tornio2Assi,

                               };
                }

                return new[]
                               {
                                   Abstraction.FaseDiLavoro.TipoFaseLavoro.Centro,
                               };
            }
        }

        public IPatternDrilling PatternDrilling { get; private set; }

        public double DiametroForatura { get; set; }

        public double ProfonditaForatura { get; set; }

        public double InizioZ { get; set; }

        public double ProfonditaCentrino { get; set; }

        public bool SvasaturaConFresa { get; set; }

        public bool SvasaturaConCompensazione { get; set; }

        public double ProfonditaSvasatura { get; set; }

        private PatternForatura _pattern;
        public PatternForatura Pattern
        {
            get
            {
                return _pattern;
            }
            set
            {
                _pattern = value;
                PatternDrilling = DrillPatternHelper.UpdatePattern(Pattern);
            }
        }

        //internal static IPatternDrilling UpdatePattern(PatternForatura patternForatura)
        //{
        //    switch (patternForatura)
        //    {
        //        case PatternForatura.Circolare:
        //            {
        //                return new PatternDrillingCircle();

        //            } break;

        //        case PatternForatura.Rettangolare:
        //            {
        //                return  new PatternDrillingRectangle();

        //            } break;

        //        case PatternForatura.Arco:
        //            {
        //                return new PatternDrillingArc();

        //            } break;

        //        case PatternForatura.Linea:
        //            {
        //                return new PatternDrillingLine();

        //            } break;

        //        case PatternForatura.CoordinateRc:
        //            {
        //                return new PatternDrillingRc();

        //            } break;

        //        case PatternForatura.CoordinateXy:
        //            {
        //                return new PatternDrillingXy();

        //            } break;

        //        case PatternForatura.TornioForaturaCentrale:
        //            {
        //                return new PatternDrillingLatheCenter();

        //            } break;

        //        default:
        //            throw new NotImplementedException("ForaturaSemplice.UpdatePattern");
        //    }
        //}

        public bool ForaturaCentraleTornio { get; private set; }

        protected DrillBaseClass(bool foraturaCentraleTornio)
        {
            ForaturaCentraleTornio = foraturaCentraleTornio;

            if (ForaturaCentraleTornio)
            {
                Pattern = PatternForatura.TornioForaturaCentrale;
            }
            else
            {
                Pattern = PatternForatura.Circolare;
            }

            Centrinatura = new Operazione(this, LavorazioniEnumOperazioni.ForaturaCentrino);

            Foratura = new Operazione(this, LavorazioniEnumOperazioni.ForaturaPunta);

            Svasatura = new Operazione(this, LavorazioniEnumOperazioni.ForaturaSmusso);

        }

        public bool CentrinoAbilitato
        {
            get { return Centrinatura.Abilitata; }
            set { Centrinatura.Abilitata = value; }
        }

        public bool ForaturaAbilitata
        {
            get { return Foratura.Abilitata; }
            set { Foratura.Abilitata = value; }
        }

        public bool SvasaturaAbilitata
        {
            get { return Svasatura.Abilitata; }
            set { Svasatura.Abilitata = value; }
        }

        public Operazione Centrinatura { get; set; }

        public Operazione Foratura { get; set; }

        public Operazione Svasatura { get; set; }

        public abstract double DiametroPreview { get; }

        public ModalitaForatura ModalitaForatura { get; set; }

        protected override List<IEntity3D> GetFinalPreview()
        {
            var rslt = new List<IEntity3D>();

            if (PatternDrilling != null)
            {
                var diameter = DiametroPreview;// fare proprieta comuine iun modo da personalizzare diametro per ogni lavorazione

                if (diameter == 0)
                    diameter = 1;

                var pntList = PatternDrilling.GetPointList();

                foreach (var point2D in pntList)
                {
                    var arc = new Arc3D { Radius = diameter / 2, Center = new Point3D(point2D.X, point2D.Y, 0) };

                    arc.Start = new Point3D(arc.Center);

                    arc.Start.X += arc.Radius;

                    arc.End = new Point3D(arc.Center);

                    arc.End.X += arc.Radius;

                    rslt.Add(arc);
                }
            }

            foreach (var entity2D in rslt)
                entity2D.PlotStyle = EnumPlotStyle.Element;

            return rslt;
        }

        private double GetDiameter(LavorazioniEnumOperazioni lavorazioniEnumOperazioni)
        {
            switch (lavorazioniEnumOperazioni)
            {
                case LavorazioniEnumOperazioni.ForaturaPunta:
                case LavorazioniEnumOperazioni.ForaturaCentrino:
                case LavorazioniEnumOperazioni.ForaturaSmusso:
                case LavorazioniEnumOperazioni.ForaturaAlesatore:
                    {
                        var lav = this as Foratura.Maschiatura;
                        if (lav != null && lav.MaschiaturaSelezionata != null)
                        {
                            var d = lav.MaschiaturaSelezionata.Preforo;
                            return d;
                        }

                        return DiametroForatura;
                    } break;

                case LavorazioniEnumOperazioni.ForaturaBareno:
                    {
                        var lav = this as Foratura.Barenatura;
                        if (lav != null) return lav.DiametroBarenatura;
                    } break;

                case LavorazioniEnumOperazioni.ForaturaLamatore:
                    {
                        var lav = this as Foratura.Lamatura;
                        if (lav != null) return lav.DiametroLamatura;
                    } break;

                //Maschio
                case LavorazioniEnumOperazioni.ForaturaMaschiaturaDx:
                    {
                        var lav = this as Foratura.Maschiatura;
                        if (lav != null && lav.MaschiaturaSelezionata != null)
                        {
                            //Todo : gestire diametro metrico, oppure creare diametro e basta..
                            var d = lav.MaschiaturaSelezionata.DiametroMetrico;
                            return d;
                        }
                    } break;

            }

            return 0;
        }
        public override Utensile PickBestTool(Operazione operazione, IEnumerable<Utensile> utensiles, Guid matGuid)
        {
            var d = GetDiameter(operazione.OperationType);

            /* Fra utensili IDiameter */
            //  var dias = utensiles.OfType<IDiametrable>();

            // var tools = dias.OrderBy(n => Math.Abs(n.Diametro - d)).First();

            var tool = (from utensile in utensiles
                        from parametro in utensile.ParametriUtensile
                        where parametro.MaterialGuid == matGuid
                        orderby utensile.SaveTime descending
                        //where utensile.OperazioneTipo == operazione.OperationType
                        select utensile);

            var t = (from t1 in tool.OfType<IDiametrable>()
                     orderby Math.Abs(t1.Diametro - d)
                     select t1).FirstOrDefault();

            if (t == null)
                base.PickBestTool(operazione, utensiles, matGuid);

            return t as Utensile;
        }

        ///// <summary>
        ///// Metodo per creare actionCollection per lavorazione fori
        ///// L'iter nelle varie lavorazioni è identico.
        ///// Mi sposto sopra foro,
        ///// Lavorazione ( qui fare metodi differenti in base a necessita)
        ///// Posizione Iniziale
        ///// Mi Sposto su foro successivo
        ///// </summary>
        //public MoveActionCollection ElaborateMoveActionCollection(MacroDrillingAction macroDrillingAction, Operazione operazione)
        //{
        //    var rslt = new MoveActionCollection();

        //    var pntList = macroDrillingAction.DrillPoints;





        //    foreach (var point2D in pntList)
        //    {
        //        rslt.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, point2D.X, point2D.Y, null);
        //        rslt.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, macroDrillingAction.SecureZ);

        //        /*
        //         * fare r del ciclo.
        //         */

        //        rslt.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, macroDrillingAction.SecureZ + extraCorsa);

        //    }
        //}

        ///// <summary>
        ///// Restituisce programma universale per operazione di foratura.
        ///// </summary>
        ///// <param name="operazione"></param>
        ///// <returns></returns>
        //internal override ProgramPhase GetOperationProgram(Operazione operazione)
        //{
        //    /*
        //     * una cosa è sicuro !!
        //     * 
        //     * posso astrarre ulteriormente il questo metodo
        //     * 
        //     * in comune ho 
        //     * 
        //     * - init program
        //     * - toolChange ( poi in fase di calcolo programma vedo se saltarlo o meno )
        //     * - settaggio feed. ( vedere meglio)
        //     * 
        //     * -- calcolo programma ( questo è l'unica parte diversa )
        //     * 
        //     * - rototraslazione operazioni
        //     */
        //    var program = new ProgramPhase(SicurezzaZ);

        //    // cambio utensile 
        //    var toolChange = new ChangeToolAction(program, operazione);

        //    var parametro = operazione.Utensile.ParametroUtensile as ParametroPunta;

        //    var fresa = operazione.Utensile as DrillTool;

        //    if (fresa == null || parametro == null)
        //        throw new NullReferenceException();

        //    /*
        //     * come preference devo prendere da file ,
        //     * cosi sono sicuro che è aggiornato.
        //     * 
        //     * non posso prendere elemento alla volta.
        //     */

        //    /*
        //     * preference sarebbe meglio settarlo non qui perche avrei 10 accessi al file se ho 10 operazioni.
        //     * ma bensi al momento della richiesta di anteprima e generazione codice..
        //     * 
        //     * da operazione quindi..
        //     * 
        //     * per generazione programma posso, la , invece per generazione anteprima è da altra parte.
        //     * 
        //     * quando aggiornare preferenze..
        //     */

        //    var secureFeed = 1;

        //    var extraCorsa = 1;

        //    var feed = parametro.GetFeed(FeedType.ASync);

        //    if (feed <= 0)
        //        return null;

        //    var feedDictionary = new Dictionary<MoveType, double>
        //                             {
        //                                 {MoveType.Rapid, 10000},
        //                                 {MoveType.SecureRapidFeed, secureFeed},
        //                                 {MoveType.Work, feed},
        //                                 {MoveType.Cw, feed},
        //                                 {MoveType.Ccw, feed},
        //                             };

        //    program.SetFeedDictionary(feedDictionary);

        //    var macro = new MacroDrillingAction(program)
        //    {
        //        DrillPoints = GetDrillPointList(),
        //        SecureZ = SicurezzaZ,
        //        StartZ = InizioZ,
        //        TipologiaLavorazione = operazione.OperationType,
        //    };

        //    // macro
        //    switch (operazione.OperationType)
        //    {
        //        case LavorazioniEnumOperazioni.ForaturaMaschiaturaDx:
        //            {
        //                var opMaschiatura = operazione.Lavorazione as IMaschiaturaAble;

        //                if (opMaschiatura == null)
        //                    throw new NullReferenceException();

        //                macro.EndZ = InizioZ - opMaschiatura.ProfonditaMaschiatura;

        //            } break;

        //        case LavorazioniEnumOperazioni.ForaturaCentrino:
        //            {
        //                var opCentrinable = operazione.Lavorazione as ICentrinoAble;

        //                if (opCentrinable == null)
        //                    throw new NullReferenceException();

        //                macro.EndZ = InizioZ - ProfonditaCentrino;

        //            } break;

        //        case LavorazioniEnumOperazioni.ForaturaSmusso:
        //            {
        //                var opSvasatura = operazione.Lavorazione as ISvasaturaAble;

        //                if (opSvasatura == null)
        //                    throw new NullReferenceException();

        //                macro.EndZ = InizioZ - ProfonditaSvasatura;

        //            } break;

        //        case LavorazioniEnumOperazioni.ForaturaPunta:
        //            {
        //                var opForatura = operazione.Lavorazione as IForaturaAble;

        //                if (opForatura == null)
        //                    throw new NullReferenceException();

        //                macro.ModalitaForatura = ModalitaForatura;

        //                macro.EndZ = InizioZ - ProfonditaForatura;
        //                macro.Step = parametro.Step;

        //            } break;
        //    }

        //    var move = new MoveActionCollection();

        //    var pntList = macro.DrillPoints;

        //    foreach (var point2D in pntList)
        //    {
        //        move.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, point2D.X, point2D.Y, null);
        //        move.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, macro.SecureZ);

        //        ElaborateCycle(move, macro);
        //        /*
        //         * fare r del ciclo.
        //         */

        //        move.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, macro.SecureZ);

        //    }

        //    macro.MoveActionCollection = move;

        //    //var mm = base.GetFinalProgram(macro.MoveActionCollection);

        //    foreach (var variable in macro.MoveActionCollection)
        //    {
        //        program.SetFeedMoveAction(variable);
        //    }
        //    // disimpegno

        //    return program;

        //}

        private void ElaborateAllargaturaBareno(ProgramOperation programPhase, Operazione operazione)
        {
            var opMaschiatura = operazione.Lavorazione as IBarenoAble;

            if (opMaschiatura == null)
                throw new NullReferenceException();

            var pntListBar = GetDrillPointList();

            var moveCollection = new MoveActionCollection();

            var iniZ = InizioZ;

            var endZ = InizioZ - opMaschiatura.ProfonditaBareno;

            var fresa = operazione.Utensile as FresaCandela;

            var parametro = operazione.Utensile.ParametroUtensile as ParametroFresaCandela;

            var larghezzaPassata = parametro.GetLarghezzaPassata();

            var profPassata = parametro.GetProfonditaPassata();

            var diaAllargatura = opMaschiatura.DiametroAllargatura;

            if (InizioZ <= endZ) return;
            if (fresa.Diametro > diaAllargatura) return;

            foreach (var point2D in pntListBar)
            {

                //  moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, point2D.X, point2D.Y, null);

                // Semplice)
                if (opMaschiatura.ModalitaAllargatura == 0)
                {
                    var profile = Profile2D.CreateCircle(diaAllargatura / 2, point2D);

                    MillProgrammingHelper.GetInternRoughing(moveCollection, profile, opMaschiatura.ProfonditaBareno,
                        profPassata, larghezzaPassata, fresa.Diametro, 0, InizioZ, SicurezzaZ, 0, 0);

                }

                // Interpolazione
                else if (opMaschiatura.ModalitaAllargatura == 1)
                {
                    MillProgrammingHelper.GetRoughHelicalInterpolation(moveCollection, InizioZ, endZ, SicurezzaZ, point2D,
                        diaAllargatura, fresa.Diametro, profPassata, larghezzaPassata);
                }
            }

            foreach (var a in moveCollection)
                programPhase.AggiungiAzioneMovimento(a);
        }
        /// <summary>
        /// Creo Programma per lavorazioni di foratura.
        /// Per quasi tutte le operazioni creo una macro ,
        /// Invece per allargatura foro nella barenatura e lamatura
        /// faccio programma ad hoc
        /// 
        /// </summary>
        /// <param name="programPhase"></param>
        /// <param name="operazione"></param>
        protected override void CreateSpecificProgram(ProgramOperation programPhase, Operazione operazione)
        {
            if (operazione.OperationType == LavorazioniEnumOperazioni.AllargaturaBareno)
            {
                ElaborateAllargaturaBareno(programPhase, operazione);
                return;
            }

            var macro = new MacroForaturaAzione(programPhase)
                            {
                                PuntiForatura = GetDrillPointList(),
                                SicurezzaZ = SicurezzaZ,
                                StartZ = InizioZ,
                                TipologiaLavorazione = operazione.OperationType,
                                PuntoRitorno = SicurezzaZ - InizioZ,
                            };

            /*
             * il punto r è la distanza fra z di sicurezza e z iniziale, - distanza di avvicinamento.. tipo 2mm
             * 
             * 
             */

            // macro
            switch (operazione.OperationType)
            {

                case LavorazioniEnumOperazioni.ForaturaAlesatore:
                    {
                        var alesatura = operazione.Lavorazione as IAlesatoreAble;

                        if (alesatura == null)
                            throw new NullReferenceException();

                        macro.EndZ = InizioZ - alesatura.ProfonditaAlesatore;

                    } break;

                case LavorazioniEnumOperazioni.ForaturaBareno:
                    {
                        var opMaschiatura = operazione.Lavorazione as IBarenoAble;

                        if (opMaschiatura == null)
                            throw new NullReferenceException();

                        macro.EndZ = InizioZ - opMaschiatura.ProfonditaBareno;

                    } break;

                case LavorazioniEnumOperazioni.ForaturaLamatore:
                    {
                        var opMaschiatura = operazione.Lavorazione as ILamaturaAble;

                        if (opMaschiatura == null)
                            throw new NullReferenceException();

                        macro.EndZ = InizioZ - opMaschiatura.ProfonditaLamatura;
                        macro.Sosta = 500;

                    } break;

                case LavorazioniEnumOperazioni.ForaturaMaschiaturaDx:
                    {
                        var opMaschiatura = operazione.Lavorazione as IMaschiaturaAble;

                        if (opMaschiatura == null)
                            throw new NullReferenceException();
                        macro.Sosta = 500;


                        macro.EndZ = InizioZ - opMaschiatura.ProfonditaMaschiatura;

                    } break;

                case LavorazioniEnumOperazioni.ForaturaCentrino:
                    {
                        var opCentrinable = operazione.Lavorazione as ICentrinoAble;

                        if (opCentrinable == null)
                            throw new NullReferenceException();

                        macro.EndZ = InizioZ - ProfonditaCentrino;

                    } break;

                case LavorazioniEnumOperazioni.ForaturaSmusso:
                    {
                        var opSvasatura = operazione.Lavorazione as ISvasaturaAble;

                        if (opSvasatura == null)
                            throw new NullReferenceException();

                        macro.EndZ = InizioZ - ProfonditaSvasatura;
                        macro.Sosta = 500;


                    } break;

                case LavorazioniEnumOperazioni.ForaturaPunta:
                    {
                        var opForatura = operazione.Lavorazione as IForaturaAble;

                        if (opForatura == null)
                            throw new NullReferenceException();

                        macro.ModalitaForatura = ModalitaForatura;

                        macro.EndZ = InizioZ - ProfonditaForatura;

                        /* Qui mi serve step, il parametro deve essere parametro punta,
                         Nel caso non lo fosse non scoppia niente.
                         */
                        var parametro = operazione.GetParametro<ParametroPunta>();

                        if (parametro != null)
                            macro.Step = parametro.Step;

                    } break;
            }

            /*
             * Ora devo " sviluppare" l'azione macro per avere un'anteprima corretta e un corretto tempo macchina..
             */

            var move = new MoveActionCollection();

            var pntList = macro.PuntiForatura;

            foreach (var point2D in pntList)
            {
                move.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, point2D.X, point2D.Y, null);
                move.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, macro.SicurezzaZ);

                ElaborateCycle(move, macro);
                /*
                 * fare r del ciclo.
                 */

                move.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, macro.SicurezzaZ);

            }

            macro.MoveActionCollection = move;

            //var mm = base.GetFinalProgram(macro.MoveActionCollection);

            foreach (var variable in macro.MoveActionCollection)
            {
                programPhase.SettaValoreAvanzamento(variable);
            }

            // disimpegno
        }

        private static void ElaborateCycle(MoveActionCollection move, MacroForaturaAzione macro)
        {
            var enumOperazioniForatura = macro.TipologiaLavorazione;
            var modalitaForatura = macro.ModalitaForatura;

            switch (enumOperazioniForatura)
            {
                case LavorazioniEnumOperazioni.ForaturaPunta:
                    {
                        switch (modalitaForatura)
                        {
                            case Lavorazioni.Foratura.ModalitaForatura.StepScaricoTruciolo:
                                {
                                    if (macro.Step == 0)
                                        macro.Step = -macro.EndZ - macro.StartZ;

                                    var currentZ = macro.StartZ;

                                    while (currentZ > macro.EndZ)
                                    {
                                        currentZ -= macro.Step;
                                        if (currentZ < macro.EndZ)
                                            currentZ = macro.EndZ;

                                        move.AddLinearMove(MoveType.Work, AxisAbilited.Z, null, null, currentZ);
                                        move.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, macro.StartZ);
                                        move.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, currentZ);
                                    }

                                } break;

                            default:
                            case Lavorazioni.Foratura.ModalitaForatura.StepSenzaScaricoTruciolo:
                            case Lavorazioni.Foratura.ModalitaForatura.Semplice:
                                {
                                    move.AddLinearMove(MoveType.Work, AxisAbilited.Z, null, null, macro.EndZ);
                                    move.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, macro.SicurezzaZ);

                                } break;
                        }
                    } break;

                case LavorazioniEnumOperazioni.ForaturaAlesatore:
                    {
                        move.AddLinearMove(MoveType.Work, AxisAbilited.Z, null, null, macro.EndZ);
                        move.AddLinearMove(MoveType.Work, AxisAbilited.Z, null, null, macro.SicurezzaZ);
                    } break;


                /*
                 * In questi casi l'utensile va giu in lavoro e ritorna a punto iniziale in rapido
                 */
                case LavorazioniEnumOperazioni.ForaturaLamatore:
                case LavorazioniEnumOperazioni.ForaturaCentrino:
                case LavorazioniEnumOperazioni.ForaturaSmusso:
                    {
                        move.AddLinearMove(MoveType.Work, AxisAbilited.Z, null, null, macro.EndZ);
                        move.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, macro.SicurezzaZ);

                    } break;

                default:
                    throw new NotImplementedException("DrillBaseClass.ElaborateCycle");
            }
        }

        //internal override Utensile CreateTool(LavorazioniEnumOperazioni enumOperationType)
        //{ /*
        //     * per ora restituisco solamente punta , poi implementare centrino e svasatore in base a enum
        //     */

        //    switch ((LavorazioniEnumOperazioni)enumOperationType)
        //    {
        //        case LavorazioniEnumOperazioni.ForaturaCentrino:
        //            return new Centrino(MeasureUnit);

        //        case LavorazioniEnumOperazioni.ForaturaPunta:
        //            return new Punta(MeasureUnit);

        //        case LavorazioniEnumOperazioni.ForaturaSmusso:
        //            return new Svasatore(MeasureUnit);

        //        case LavorazioniEnumOperazioni.ForaturaMaschiaturaDx:
        //            return new Maschio(MeasureUnit);

        //        case LavorazioniEnumOperazioni.ForaturaBareno:
        //            return new Bareno(MeasureUnit);

        //        case LavorazioniEnumOperazioni.ForaturaAlesatore:
        //            return new Alesatore(MeasureUnit);

        //        case LavorazioniEnumOperazioni.ForaturaLamatore:
        //            return new Lamatore(MeasureUnit);

        //        //case EnumOperazioniForatura.FresaFilettare:
        //        //    return new FresaFilettare(unit);

        //        default:
        //            throw new NotImplementedException("DrillBaseClass.CreateTool");

        //    }

        //    throw new NotImplementedException();
        //}

        //internal override List<Utensile> GetCompatibleTools(LavorazioniEnumOperazioni operationType, MagazzinoUtensile magazzino)
        //{
        //    IEnumerable<Utensile> tools = null;

        //    switch ((LavorazioniEnumOperazioni)operationType)
        //    {
        //        //case EnumOperazioniForatura.FresaFilettare:
        //        //    {
        //        //        tools = magazzino.GetTools<FresaFilettare>(FaseDiLavoro.Model.MeasureUnit);
        //        //    } break;

        //        case LavorazioniEnumOperazioni.ForaturaSmusso:
        //            {
        //                tools = magazzino.GetTools<Svasatore>(MeasureUnit);
        //            } break;

        //        case LavorazioniEnumOperazioni.ForaturaPunta:
        //            {
        //                tools = magazzino.GetTools<Punta>(MeasureUnit);
        //                /* filtro punta diametro */
        //            } break;

        //        case LavorazioniEnumOperazioni.ForaturaCentrino:
        //            {
        //                tools = magazzino.GetTools<Centrino>(MeasureUnit);

        //            } break;


        //        case LavorazioniEnumOperazioni.ForaturaLamatore:
        //            {
        //                tools = magazzino.GetTools<Lamatore>(MeasureUnit);

        //            } break;


        //        case LavorazioniEnumOperazioni.ForaturaBareno:
        //            {
        //                tools = magazzino.GetTools<Bareno>(MeasureUnit);

        //            } break;

        //        case LavorazioniEnumOperazioni.ForaturaAlesatore:
        //            {
        //                tools = magazzino.GetTools<Alesatore>(MeasureUnit);

        //            } break;

        //        default:
        //            {
        //                throw new NotImplementedException("DrBaseClass.GetCompTools");
        //                //return magazzino.GetDrill(DiametroForatura, unit);
        //            } break;
        //    }

        //    if (tools != null)
        //        return tools.ToList();

        //    return null;
        //}

        public List<Point2D> GetDrillPointList()
        {
            if (PatternDrilling == null)
                throw new NullReferenceException();

            var pntList = PatternDrilling.GetPointList();

            return pntList;
        }

        public double GetDrillDiameter(LavorazioniEnumOperazioni enumOperazioniForatura)
        {
            switch (enumOperazioniForatura)
            {
                case LavorazioniEnumOperazioni.ForaturaPunta:
                    {
                        return DiametroForatura;

                    } break;

                default:
                    return 0;
            }

            return 0;
        }

    }
}