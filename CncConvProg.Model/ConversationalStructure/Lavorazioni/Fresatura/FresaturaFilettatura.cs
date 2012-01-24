using System;
using System.Collections.Generic;
using System.Linq;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.ThreadTable;
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
    /*
     * per ora considero solamente passo metrico , 
     */
    [Serializable]
    public sealed class FresaturaFilettatura : LavorazioneFresatura, IMillLeveable, IForaturaPatternable, ISvasaturaAble
    {
        /*
         * come scelta del filetto faccio scelta 
         *  - esterno interna 
         *  - sx - dx 
         *  - da tabella 
         *  - personalizzata ( immetto parametri dia - passo - conica - se conica prendo conicita) 
         *  - se scelgo da tabella immetto cmq i parametri ma disabilito i controlli, in questo modo riesco a vedere cmq i dati.
         */

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

        public IPatternDrilling PatternDrilling { get; private set; }

        public RigaTabellaFilettatura MaschiaturaSelezionata { get; set; }

        public bool FilettaturaConcorde { get; set; }

        public bool FilettaturaEsterna { get; set; }

        public bool FilettaturaSinistra { get; set; }

        public bool ParametriFilettaturaPersonalizzati { get; set; }

        public bool FilettaturaInPollici { get; set; }

        /*
         * non importa mettere input value qui..
         */
        public double DiametroMetricoFinale { get; set; }

        public double DiametroPolliciFinale { get; set; }

        public double FilettiPerPollice { get; set; }

        public double PassoMetrico { get; set; }

        public FresaturaFilettatura(Guid parent)
            : base(parent)
        {
            Pattern = PatternForatura.Circolare;

            //Finitura = new Operazione(this, LavorazioniEnumOperazioni.Finitura);
            //CentrinoForoApertura = new Operazione(this, (int)FresaturaEnumOperazioni.Centrino);
            //ForaturaApertura = new Operazione(this, (int)FresaturaEnumOperazioni.Foratura);
            FilettaturaOp = new Operazione(this, LavorazioniEnumOperazioni.FresaturaFilettare);
            Smussatura = new Operazione(this, LavorazioniEnumOperazioni.Smussatura);
        }

        public double ProfonditaLavorazione { get; set; }

        public double InizioLavorazioneZ { get; set; }

        public override List<Operazione> Operazioni
        {
            get
            {
                return new List<Operazione> { FilettaturaOp/*, Smussatura*/ };
            }
        }

        public Operazione FilettaturaOp { get; set; }

        public Operazione Finitura { get; set; }

        public Operazione Smussatura { get; set; }

        public Operazione CentrinoForoApertura { get; set; }

        public Operazione ForaturaApertura { get; set; }

        protected override void CreateSpecificProgram(ProgramPhase programPhase, Operazione operazione)
        {
            /*
             * utensile e parametro lo prendo all'interno del cambio switch..
             */
            var fresa = operazione.Utensile as FresaCandela;

            var parametro = operazione.Utensile.ParametroUtensile as ParametroFresaCandela;

            if (fresa == null || parametro == null)
                throw new NullReferenceException();

            if (PatternDrilling == null) return;
            var diameter = DiametroMetricoFinale;

            var pntList = PatternDrilling.GetPointList();

            if (pntList == null || pntList.Count <= 0) return;

            var diaFresa = fresa.Diametro;


            var moveCollection = new MoveActionCollection();

            var workUp = InizioLavorazioneZ;
            var workDown = InizioLavorazioneZ - ProfonditaLavorazione;

            foreach (var point2D in pntList)
            {
                switch (operazione.OperationType)
                {
                    case LavorazioniEnumOperazioni.Smussatura:
                        {
                            var profile = Profile2D.CreateCircle(DiametroMetricoFinale / 2, point2D);

                            MillProgrammingHelper.GetInternChamfer(moveCollection, profile, ProfonditaSvasatura, diaFresa, 0, false, InizioLavorazioneZ, SicurezzaZ);

                        } break;
                    case LavorazioniEnumOperazioni.FresaturaFilettare:
                        {
                            var helicalRadius = (diameter - diaFresa) / 2;

                            if (FilettaturaSinistra && !FilettaturaEsterna)
                            {
                                //MillProgrammingHelper.GetInternThreadSx(moveCollection, );
                                MillProgrammingHelper.GetInternThreadSx(moveCollection, workUp, workDown, SicurezzaZ, point2D, PassoMetrico, DiametroMetricoFinale / 2, true);

                            }
                            else if (!FilettaturaSinistra && !FilettaturaEsterna)
                            {
                                MillProgrammingHelper.GetInternThreadDx(moveCollection, workUp, workDown, SicurezzaZ, point2D, PassoMetrico, DiametroMetricoFinale / 2, true);
                            }

                            else if (!FilettaturaSinistra && FilettaturaEsterna)
                            {
                                var extracorsa = diaFresa/2 + ExtraCorsa;

                                MillProgrammingHelper.GetExternThreadDx(moveCollection, workUp, workDown, SicurezzaZ, point2D, PassoMetrico, DiametroMetricoFinale / 2, true,extracorsa);
                            }

                            else if (FilettaturaSinistra && FilettaturaEsterna)
                            {
                                var extracorsa = diaFresa / 2 + ExtraCorsa;

                                MillProgrammingHelper.GetExternThreadSx(moveCollection, workUp, workDown, SicurezzaZ, point2D, PassoMetrico, DiametroMetricoFinale / 2, true, extracorsa);
                            }

                        }
                        break;

                    default:
                        throw  new Exception("FresaturaFilettatura.CreateSpecificProgram");
                        break;

                }
            }
            var mm = base.GetFinalProgram(moveCollection);

            foreach (var variable in mm)
            {
                programPhase.AddMoveAction(variable);
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


        //    if (PatternDrilling == null) return null;
        //    var diameter = DiametroMetricoFinale;

        //    var pntList = PatternDrilling.GetPointList();

        //    if (pntList == null || pntList.Count <= 0) return null;

        //    var diaFresa = fresa.DiametroFresa;

        //    var larghezzaPassata = parametro.GetLarghezzaPassata();

        //    var profPassata = parametro.GetProfonditaPassata();

        //    var feed = parametro.GetFeed(FeedType.ASync);

        //    var plungeFeed = parametro.AvanzamentoAsincronoPiantata.Value.Value;

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
        //        default:
        //            {
        //                MaschiaturaFresataAction(pntList, true, FilettaturaEsterna, FilettaturaSinistra, diaFresa, 1, false, DiametroMetricoFinale, PassoMetrico, InizioLavorazioneZ, InizioLavorazioneZ - ProfonditaLavorazione, program, moveCollection, feed, SicurezzaZ);
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
            get { return "Thread Milling"; }
        }

        /// <summary>
        /// Ritorna anteprima della lavorazione
        /// </summary>
        /// <returns></returns>
        protected override List<IEntity3D> GetFinalPreview()
        {
            try
            {
                var rslt = new List<IEntity3D>();

                if (PatternDrilling != null)
                {
                    var diameter = DiametroMetricoFinale;

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

                    foreach (var entity3D in rslt)
                    {
                        entity3D.PlotStyle = EnumPlotStyle.Element;
                    }
                }

                return rslt;
            }
            catch (Exception)
            {
                throw new Exception("FresaturaFilettatura.GetPreview");
            }

            return null;

            //var cerchio = new Arc2D
            //{
            //    Center = new Point2D(CentroX, CentroY),
            //    Radius = Raggio,
            //    ClockWise = false,
            //    Start = { X = CentroX + Raggio },
            //    End = { X = CentroX + Raggio },
            //};


            //cerchio.Start.Y = CentroY;
            //cerchio.End.Y = CentroY;


            //var profile2D = new Profile2D();

            //profile2D.AddEntity(cerchio);

            //cerchio.PlotStyle = EnumPlotStyle.Element;
            //return profile2D;


            //try
            //{
            //    if (Pattern != null)
            //    {
            //        var preview = Pattern.GetClosedProfile().Source;

            //        return Entity3DHelper.Get3DProfile(preview).ToList();
            //    }

            //}
            //catch (Exception ex)
            //{
            //}

            return null;
        }

        public bool SvasaturaConFresa { get; set; }
        public bool SvasaturaConCompensazione { get; set; }
        public double ProfonditaSvasatura { get; set; }

        public bool SvasaturaAbilitata
        {
            get { return Smussatura.Abilitata; }
            set { Smussatura.Abilitata = value; }
        }
    }


}