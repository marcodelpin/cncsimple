using System;
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
    public abstract class DrillBaseClass : LavorazioneFresatura, IForaturaPatternable, IForaturaAble, ICentrinoAble, ISvasaturaAble
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
        /// <summary>
        /// Metodo utilizzato nella selezione automatica dell'utensile.
        /// </summary>
        /// <param name="operazione"></param>
        /// <param name="utensiles"></param>
        /// <param name="matGuid"></param>
        /// <returns></returns>
        public override Utensile PickBestTool(Operazione operazione, IEnumerable<Utensile> utensiles, Guid matGuid)
        {
            var d = GetDiameter(operazione.OperationType);

            /* Fra utensili IDiameter */
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


        private void ElaborateAllargaturaBareno(ProgramOperation programPhase, Operazione operazione)
        {
            var opBarenatura = operazione.Lavorazione as IBarenoAble;

            if (opBarenatura == null)
                throw new NullReferenceException();

            var pntListBar = GetDrillPointList();

            var moveCollection = new MoveActionCollection();

            var iniZ = InizioZ;

            var endZ = InizioZ - opBarenatura.ProfonditaBareno;

            var fresa = operazione.Utensile as FresaCandela;

            var parametro = operazione.Utensile.ParametroUtensile as ParametroFresaCandela;

            var larghezzaPassata = parametro.GetLarghezzaPassata();

            var profPassata = parametro.GetProfonditaPassata();

            var diaAllargatura = opBarenatura.DiametroBarenatura - opBarenatura.MaterialePerFinitura;

            if (InizioZ <= endZ) return;
            if (fresa.Diametro > diaAllargatura) return;

            foreach (var point2D in pntListBar)
            {
                // Semplice)
                if (opBarenatura.ModalitaAllargatura == 0)
                {
                    var profile = Profile2D.CreateCircle(diaAllargatura / 2, point2D);

                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, point2D.X,
                                                     point2D.Y, null, null);

                    MillProgrammingHelper.GetInternRoughing(moveCollection, profile, opBarenatura.ProfonditaBareno,
                        profPassata, larghezzaPassata, fresa.Diametro, 0, InizioZ, SicurezzaZ, 0, 0);

                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, point2D.X,
                                 point2D.Y, null, null);

                }

                // Interpolazione
                else if (opBarenatura.ModalitaAllargatura == 1)
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
        /// Raccoglie dati per creare macro specifiche.
        /// Per allargatura foro nella barenatura e lamatura faccio programma ad hoc
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

            var preference = Singleton.Preference.GetPreference(Singleton.Instance.MeasureUnit);

            var distanzaSicurezza = preference.DistanzaSicurezzaCicliForatura;

            var macro = new MacroForaturaAzione(programPhase)
                            {
                                PuntiForatura = GetDrillPointList(),
                                SicurezzaZ = SicurezzaZ,
                                StartZ = InizioZ,
                                TipologiaLavorazione = operazione.OperationType,
                                CodiceR = InizioZ + distanzaSicurezza,
                            };

            macro.ParametriTaglio = new ParametroVelocita();

            ParametroVelocita parametroVelocita;

            if (programPhase.FeedDictionary.TryGetValue(MoveType.Work, out parametroVelocita))
                macro.ParametriTaglio = parametroVelocita;

            /*
             * il punto r è la distanza fra z di sicurezza e z iniziale, - distanza di avvicinamento.. tipo 2mm
             */

            // macro)
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

            foreach (var variable in macro.MoveActionCollection)
            {
                programPhase.SettaValoreAvanzamento(variable);
            }
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

                case LavorazioniEnumOperazioni.ForaturaMaschiaturaDx:
                case LavorazioniEnumOperazioni.ForaturaMaschiaturaSx:
                    {

                        move.AddLinearMove(MoveType.Work, AxisAbilited.Z, null, null, macro.EndZ);
                        move.AddLinearMove(MoveType.Work, AxisAbilited.Z, null, null, macro.SicurezzaZ);

                    } break;

                case LavorazioniEnumOperazioni.ForaturaAlesatore:
                    {
                        move.AddLinearMove(MoveType.Work, AxisAbilited.Z, null, null, macro.EndZ);
                        move.AddLinearMove(MoveType.Work, AxisAbilited.Z, null, null, macro.SicurezzaZ);
                    } break;


                /*
                 * In questi casi l'utensile va giu in lavoro e ritorna a punto iniziale in rapido
                 */
                case LavorazioniEnumOperazioni.ForaturaBareno:
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


        public List<Point2D> GetDrillPointList()
        {
            if (PatternDrilling == null)
                throw new NullReferenceException();

            var pntList = PatternDrilling.GetPointList();

            return pntList;
        }

        //public double GetDrillDiameter(LavorazioniEnumOperazioni enumOperazioniForatura)
        //{
        //    switch (enumOperazioniForatura)
        //    {
        //        case LavorazioniEnumOperazioni.ForaturaPunta:
        //            {
        //                return DiametroForatura;

        //            } break;

        //        default:
        //            return 0;
        //    }

        //    return 0;
        //}

    }
}