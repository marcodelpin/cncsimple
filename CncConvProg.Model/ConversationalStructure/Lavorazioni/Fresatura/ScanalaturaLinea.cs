using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using Point3D = CncConvProg.Geometry.Point3D;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura
{
    /*
     * 
     * 20/07/2011
     * 
     * su operazione scanalatura devo mettere blocco devo evitare raggio di ritorno negatvoo.
     * 
     * quando step è maggiore di dvf..
     * -----------
     * 
     * dovrei fare operazione non standard, ovvero 
     * 
     * anche con altre cose dovrei fare non standard 
     * 
     * fare superclass operazione!.
     * 
     * operazione trochidal.
     * 
     * mi servirebbero indicazioni anche per altre cose per altri range consigliati..
     * 
     * -- per cosa , solamente 
     * 
     * anche per width 
     * 
     * anche qualcosa per hex lo spessore trucciolo..
     * 
     * 08/07/2011
     * 
     * alla fine quello che cambia è solamente la parte dei parametri di taglio che viene modificata.
     * 
     * utensile e toolholder restano invariati.
     * 
     */


    [Serializable]
    public sealed class ScanalaturaLinea : LavorazioneFresatura, IMillWorkable
    {
        public ScanalaturaLinea()
        {
            TrochoidalStep = 10;

            Sgrossatura = new Operazione(this, LavorazioniEnumOperazioni.Sgrossatura);

            Finitura = new Operazione(this, LavorazioniEnumOperazioni.Finitura);

            Smussatura = new Operazione(this, LavorazioniEnumOperazioni.Smussatura);
        }

        public override List<Operazione> Operazioni
        {
            get
            {
                var opList = new List<Operazione>();

                //if (Sgrossatura.Abilitata)
                //opList.Add(ModoSgrossatura == ScanalaturaCavaMetodoLavorazione.Trocoidale
                //? SgrossaturaTrocoidale
                //: Sgrossatura);
                opList.Add(Sgrossatura);


                //if (Finitura.Abilitata)
                opList.Add(Finitura);

                //if (Smussatura.Abilitata)
                opList.Add(Smussatura);

                return opList;
            }
        }

        public Operazione Sgrossatura { get; set; }

        //public Operazione SgrossaturaTrocoidale { get; set; }

        public Operazione Finitura { get; set; }

        public Operazione Smussatura { get; set; }

        public double ProfonditaFresaSmussatura { get; set; }

        public bool FinishWithCompensation { get; set; }

        public double SovrametalloFinituraProfilo { get; set; }

        public ScanalaturaCavaMetodoLavorazione ModoSgrossatura { get; set; }

        //public ScanalaturaCavaMetodoLavorazione ModoFinitura { get; set; }

        public double InizioLavorazioneZ { get; set; }

        public double ProfonditaLavorazione { get; set; }

        public double PuntoInizialeX { get; set; }

        public double PuntoInizialeY { get; set; }

        public double LunghezzaCava { get; set; }

        public double LarghezzaCava { get; set; }

        public double OrientationAngle { get; set; }

        //internal override Utensile CreateTool(LavorazioniEnumOperazioni enumOperationType)
        //{
        //    return new FresaCandela(MeasureUnit);
        //}

        //internal override List<Utensile> GetCompatibleTools(LavorazioniEnumOperazioni operationType, MagazzinoUtensile magazzino)
        //{
        //    // prendere diametro ..
        //    var tools = magazzino.GetTools<FresaCandela>(MeasureUnit);

        //    return new List<Utensile>(tools);
        //}


        public override string Descrizione
        {
            get { return "Open Slot" + LarghezzaCava + " x " + LunghezzaCava; }
        }

        public double TrochoidalStep { get; set; }

        protected override List<IEntity3D> GetFinalPreview()
        {
            /*
             * come antprima faccio rettangolo della cava
             * 
             * 
             */

            var rotationMatrix = new Matrix3D();


            //rotationMatrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), OrientationAngle));
            // rotationMatrix.Translate(new Vector3D(PuntoInizialeX, PuntoInizialeY,0));

            rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), OrientationAngle), new System.Windows.Media.Media3D.Point3D(PuntoInizialeX, PuntoInizialeY, 0));


            var rslt = new List<IEntity3D>();

            var l1 = new Line3D
            {
                Start = new Point3D
                {
                    X = PuntoInizialeX,
                    Y = PuntoInizialeY + LarghezzaCava / 2,
                    Z = InizioLavorazioneZ,
                },

                End = new Point3D
                {
                    X = PuntoInizialeX + LunghezzaCava,
                    Y = PuntoInizialeY + LarghezzaCava / 2,
                    Z = InizioLavorazioneZ,
                }
            };

            //var l2 = new Line3D
            //{
            //    Start = l1.End,

            //    End = new Point3D
            //    {
            //        X = PuntoInizialeX + LunghezzaCava,
            //        Y = PuntoInizialeY - LarghezzaCava / 2,
            //        Z = InizioLavorazioneZ,
            //    }
            //};

            var l3 = new Line3D
            {
                Start = new Point3D
                {
                    X = PuntoInizialeX + LunghezzaCava,
                    Y = PuntoInizialeY - LarghezzaCava / 2,
                    Z = InizioLavorazioneZ,
                },

                End = new Point3D
                {
                    X = PuntoInizialeX,
                    Y = PuntoInizialeY - LarghezzaCava / 2,
                    Z = InizioLavorazioneZ,
                }
            };

            //var l4 = new Line3D
            //{
            //    Start = l3.End,
            //    End = l1.Start,
            //};

            var l1Under = new Line3D
            {
                Start = new Point3D
                {
                    X = PuntoInizialeX,
                    Y = PuntoInizialeY + LarghezzaCava / 2,
                    Z = InizioLavorazioneZ - ProfonditaLavorazione,
                },

                End = new Point3D
                {
                    X = PuntoInizialeX + LunghezzaCava,
                    Y = PuntoInizialeY + LarghezzaCava / 2,
                    Z = InizioLavorazioneZ - ProfonditaLavorazione,
                }
            };

            var l2Under = new Line3D
            {
                Start = l1Under.End,

                End = new Point3D
                {
                    X = PuntoInizialeX + LunghezzaCava,
                    Y = PuntoInizialeY - LarghezzaCava / 2,
                    Z = InizioLavorazioneZ - ProfonditaLavorazione,
                }
            };

            var l3Under = new Line3D
            {
                Start = new Point3D
                {
                    X = PuntoInizialeX + LunghezzaCava,
                    Y = PuntoInizialeY - LarghezzaCava / 2,
                    Z = InizioLavorazioneZ - ProfonditaLavorazione,
                },

                End = new Point3D
                {
                    X = PuntoInizialeX,
                    Y = PuntoInizialeY - LarghezzaCava / 2,
                    Z = InizioLavorazioneZ - ProfonditaLavorazione,
                }
            };

            var l4Under = new Line3D
            {
                Start = l3Under.End,
                End = l1Under.Start,
            };

            var l11 = new Line3D()
                          {
                              Start = l1.Start,
                              End = l1Under.Start,
                          };

            var l22 = new Line3D()
            {
                Start = l1.End,
                End = l1Under.End,
            };

            var l32 = new Line3D()
            {
                Start = l3.Start,
                End = l3Under.Start,
            };

            var l34 = new Line3D()
            {
                Start = l3.End,
                End = l3Under.End,
            };



            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l1, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l3, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l1Under, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l2Under, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l3Under, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l4Under, rotationMatrix));

            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l11, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l22, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l32, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l34, rotationMatrix));


            foreach (var entity2D in rslt)
            {
                entity2D.PlotStyle = EnumPlotStyle.Element;
            }


            /* 
             * fare metodo astratto per ottenere profilo personalizzato di ogni lavorazione.
             * 
             * poi metodo base con getPersonalizzedProfile>> get final previw
             * 
             */
            return rslt;
        }

        protected override void CreateSpecificProgram(ProgramOperation programPhase, Operazione operazione)
        {
            var dia = operazione.Utensile as IDiametrable;

            if (dia == null) return;

            var moveList = new MoveActionCollection();

            switch (operazione.OperationType)
            {
                case LavorazioniEnumOperazioni.Sgrossatura:
                    {
                        switch (ModoSgrossatura)
                        {
                            case ScanalaturaCavaMetodoLavorazione.Trocoidale:
                                {
                                    var par = operazione.GetParametro<ParametroFresaCandela>();

                                    if (par == null)
                                        throw new NullReferenceException();

                                    var profPassata = par.GetProfonditaPassata();

                                    //                        var larghPassat = par.GetLarghezzaPassata();

                                    var larghPassat = (TrochoidalStep * dia.Diametro) / 100;

                                    /*
                                     * la larghezza passata sarebbe step.
                                     */

                                    if (LarghezzaCava <= par.DiametroFresa * 2)
                                    {
                                        ProcessTrochoidalMilling(programPhase, moveList, PuntoInizialeX, PuntoInizialeY,
                                                                 LarghezzaCava, LunghezzaCava, OrientationAngle,
                                                                 larghPassat, par.DiametroFresa, profPassata,
                                                                 InizioLavorazioneZ, ProfonditaLavorazione, SicurezzaZ,
                                                                 ExtraCorsa);

                                        //ProcessArcTrochoidalMilling(program, moveList, PuntoInizialeX, PuntoInizialeY, LarghezzaCava,
                                        //                            LunghezzaCava, OrientationAngle,
                                        //                            larghPassat, par.DiametroFresa, profPassata, InizioLavorazioneZ,
                                        //                            ProfonditaLavorazione, SicurezzaZ,
                                        //                            GeometryHelper.DegreeToRadian(135),
                                        //                            GeometryHelper.DegreeToRadian(60), new Point2D(0, 0), 52.5);

                                        //ProcessArcTrochoidalMilling(program, moveList, PuntoInizialeX, PuntoInizialeY, LarghezzaCava,
                                        //                            LunghezzaCava, OrientationAngle,
                                        //                            larghPassat, par.DiametroFresa, profPassata, InizioLavorazioneZ,
                                        //                            ProfonditaLavorazione, SicurezzaZ,
                                        //                            GeometryHelper.DegreeToRadian(135 + 120),
                                        //                            GeometryHelper.DegreeToRadian(60), new Point2D(0, 0), 52.5);

                                        //ProcessArcTrochoidalMilling(program, moveList, PuntoInizialeX, PuntoInizialeY, LarghezzaCava,
                                        //                            LunghezzaCava, OrientationAngle,
                                        //                            larghPassat, par.DiametroFresa, profPassata, InizioLavorazioneZ,
                                        //                            ProfonditaLavorazione, SicurezzaZ,
                                        //                            GeometryHelper.DegreeToRadian(135 + 240),
                                        //                            GeometryHelper.DegreeToRadian(60), new Point2D(0, 0), 52.5);
                                    }

                                    else
                                        ProcessTrochoidalMillingLarge(programPhase, moveList, PuntoInizialeX,
                                                                      PuntoInizialeY, LarghezzaCava, LunghezzaCava,
                                                                      OrientationAngle,
                                                                      larghPassat, par.DiametroFresa, profPassata,
                                                                      InizioLavorazioneZ, ProfonditaLavorazione,
                                                                      SicurezzaZ, ExtraCorsa);
                                }
                                break;

                            case ScanalaturaCavaMetodoLavorazione.Tradizionale:
                                {
                                    var par = operazione.GetParametro<ParametroFresaCandela>();

                                    if (par == null)
                                        throw new NullReferenceException();

                                    var profPassat = par.GetProfonditaPassata();

                                    var larghPassat = par.GetLarghezzaPassata();

                                    /*
                                     * aggiungere sovrametallo xy
                                     */

                                    ProcessRoughLineMilling(moveList, PuntoInizialeX, PuntoInizialeY, OrientationAngle,
                                                            LunghezzaCava, LarghezzaCava, ProfonditaLavorazione,
                                                            profPassat, larghPassat, par.DiametroFresa, SicurezzaZ,
                                                            ExtraCorsa, InizioLavorazioneZ);

                                }
                                break;

                        }
                    } break;

                case LavorazioniEnumOperazioni.Finitura:
                    {

                        var par = operazione.GetParametro<ParametroFresaCandela>();

                        if (par == null)
                            throw new NullReferenceException();

                        var profPassat = par.GetProfonditaPassata();

                        var larghPassat = par.GetLarghezzaPassata();

                        ProcessSingleLineFinish(moveList, PuntoInizialeX, PuntoInizialeY, OrientationAngle, LunghezzaCava, LarghezzaCava, ProfonditaLavorazione, profPassat,
    dia.Diametro, SicurezzaZ, ExtraCorsa, InizioLavorazioneZ, FinishWithCompensation);

                    } break;

                case LavorazioniEnumOperazioni.Smussatura:
                    {
                        var profPassata = ProfonditaFresaSmussatura;
                        ProcessSingleLineFinish(moveList, PuntoInizialeX, PuntoInizialeY, OrientationAngle, LunghezzaCava, LarghezzaCava, ProfonditaFresaSmussatura, profPassata,
                            dia.Diametro, SicurezzaZ, ExtraCorsa, InizioLavorazioneZ, false);

                    } break;

                default:
                    throw new NotImplementedException();

            }

            var mm = base.GetFinalProgram(moveList);

            foreach (var variable in mm)
            {
                programPhase.AggiungiAzioneMovimento(variable);
            }
        }
        //internal override ProgramPhase GetOperationProgram(Operazione operazione)
        //{
        //    /*
        //     * il richiamo degli utensili magari farlo e delle operazioni comuni magari farlo in astratto più a monte
        //     * 
        //     */

        //    var program = new ProgramPhase(SicurezzaZ);

        //    // cambio utensile // se 
        //    var toolChange = new ChangeToolAction(program, operazione);

        //    /*
        //     * potrei settare qui punto iniziale.. non vedo altre opzioni. rimane sempre una cosa da ricordarsi in più 
        //     * >> more bugs..
        //     */

        //    program.ActiveAsseC(true);

        //    var secureFeed = 1;

        //    var extraCorsa = 1;

        //    var parametro = operazione.GetParametro<ParametroFresaCandela>();

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

        //    /* .. -- diciamo che fino a qui può essere scritto in modo astratto. 
        //     quello che cambia è la parte del programma vera e propria.

        //     */


        //    if (parametro == null)
        //        throw new NullReferenceException();

        //    var moveList = new MoveActionCollection();

        //    switch (operazione.OperationType)
        //    {
        //        case LavorazioniEnumOperazioni.SgrossaturaTrocoidale:
        //            {



        //                /*    
        //        var dvf = GrooveWidth - DiametroFresa;

        //        return (dvf / GrooveWidth) * Feed;*
        //                feedDictionary[MoveType.Ccw] = 
        //                    */


        //                /*
        //                 * distinzione fra quello larghezza 2x diametro e quelle inferiori
        //                 */
        //                /*
        //                 * raccolgo variabili ..
        //                 */


        //                //var op = operazione as OperazioneFresaturaTrocoidale;

        //                //if (op == null)
        //                //    throw new NullReferenceException();



        //                var par = operazione.GetParametro<ParametroFresaCandela>();

        //                if (par == null)
        //                    throw new NullReferenceException();

        //                var profPassata = par.GetProfonditaPassata();

        //                var larghPassat = par.GetLarghezzaPassata();

        //                /*
        //                 * la larghezza passata sarebbe step.
        //                 */





        //                if (LarghezzaCava <= par.DiametroFresa * 2)
        //                {
        //                    ProcessTrochoidalMilling(program, moveList, PuntoInizialeX, PuntoInizialeY, LarghezzaCava, LunghezzaCava, OrientationAngle,
        //                                           larghPassat, par.DiametroFresa, profPassata, InizioLavorazioneZ, ProfonditaLavorazione, SicurezzaZ, extraCorsa);

        //                    //ProcessArcTrochoidalMilling(program, moveList, PuntoInizialeX, PuntoInizialeY, LarghezzaCava,
        //                    //                            LunghezzaCava, OrientationAngle,
        //                    //                            larghPassat, par.DiametroFresa, profPassata, InizioLavorazioneZ,
        //                    //                            ProfonditaLavorazione, SicurezzaZ,
        //                    //                            GeometryHelper.DegreeToRadian(135),
        //                    //                            GeometryHelper.DegreeToRadian(60), new Point2D(0, 0), 52.5);

        //                    //ProcessArcTrochoidalMilling(program, moveList, PuntoInizialeX, PuntoInizialeY, LarghezzaCava,
        //                    //                            LunghezzaCava, OrientationAngle,
        //                    //                            larghPassat, par.DiametroFresa, profPassata, InizioLavorazioneZ,
        //                    //                            ProfonditaLavorazione, SicurezzaZ,
        //                    //                            GeometryHelper.DegreeToRadian(135 + 120),
        //                    //                            GeometryHelper.DegreeToRadian(60), new Point2D(0, 0), 52.5);

        //                    //ProcessArcTrochoidalMilling(program, moveList, PuntoInizialeX, PuntoInizialeY, LarghezzaCava,
        //                    //                            LunghezzaCava, OrientationAngle,
        //                    //                            larghPassat, par.DiametroFresa, profPassata, InizioLavorazioneZ,
        //                    //                            ProfonditaLavorazione, SicurezzaZ,
        //                    //                            GeometryHelper.DegreeToRadian(135 + 240),
        //                    //                            GeometryHelper.DegreeToRadian(60), new Point2D(0, 0), 52.5);
        //                }

        //                else
        //                    ProcessTrochoidalMillingLarge(program, moveList, PuntoInizialeX, PuntoInizialeY, LarghezzaCava, LunghezzaCava, OrientationAngle,
        //                                           larghPassat, par.DiametroFresa, profPassata, InizioLavorazioneZ, ProfonditaLavorazione, SicurezzaZ, extraCorsa);





        //            } break;

        //        case LavorazioniEnumOperazioni.Sgrossatura:
        //            {

        //                var par = operazione.GetParametro<ParametroFresaCandela>();

        //                if (par == null)
        //                    throw new NullReferenceException();

        //                var profPassat = par.GetProfonditaPassata();

        //                var larghPassat = par.GetLarghezzaPassata();


        //                ProcessRoughLineMilling(moveList, PuntoInizialeX, PuntoInizialeY, OrientationAngle, LunghezzaCava, LarghezzaCava, ProfonditaLavorazione,
        //                    profPassat, larghPassat, par.DiametroFresa, SicurezzaZ, extraCorsa, InizioLavorazioneZ);

        //            } break;

        //        case LavorazioniEnumOperazioni.Finitura:
        //            {

        //                var par = operazione.GetParametro<ParametroFresaCandela>();

        //                if (par == null)
        //                    throw new NullReferenceException();

        //                var profPassat = par.GetProfonditaPassata();

        //                var larghPassat = par.GetLarghezzaPassata();


        //                ProcessRoughLineMilling(moveList, PuntoInizialeX, PuntoInizialeY, OrientationAngle, LunghezzaCava, LarghezzaCava, ProfonditaLavorazione,
        //                    profPassat, larghPassat, par.DiametroFresa, SicurezzaZ, extraCorsa, InizioLavorazioneZ);

        //            } break;

        //        case LavorazioniEnumOperazioni.Smussatura:
        //            {
        //                var par = operazione.GetParametro<ParametroFresaCandela>();

        //                if (par == null)
        //                    throw new NullReferenceException();

        //                ProcessChamferLineMilling(moveList, PuntoInizialeX, PuntoInizialeY, OrientationAngle, LunghezzaCava, LarghezzaCava, ProfonditaFresaSmussatura,
        //                    par.DiametroFresa, SicurezzaZ, extraCorsa, InizioLavorazioneZ);

        //            } break;

        //        default:
        //            throw new NotImplementedException();

        //    }

        //    var mm = base.GetFinalProgram(moveList);

        //    foreach (var variable in mm)
        //    {
        //        program.AddMoveAction(variable);
        //    }
        //    program.ActiveAsseC(false);


        //    return program;
        //}

        /// <summary>
        /// Programma fresatura trocoidale Larghezza Maggiore 2xDc
        /// </summary>
        /// <param name="program"></param>
        /// <param name="moveCollectionList"></param>
        /// <param name="puntoInizialeX"></param>
        /// <param name="puntoInizialeY"></param>
        /// <param name="larghezzaCava"></param>
        /// <param name="lunghezzaCava"></param>
        /// <param name="rotationAngle"></param>
        /// <param name="ae"></param>
        /// <param name="diametroFresa"></param>
        /// <param name="profonditaPassata"></param>
        /// <param name="zLavoro"></param>
        /// <param name="profonditaLavorazione"></param>
        /// <param name="zSicurezza"></param>
        /// <param name="extraCorsa"></param>
        private static void ProcessTrochoidalMillingLarge(ProgramOperation program, MoveActionCollection moveCollectionList,
                                                    double puntoInizialeX, double puntoInizialeY, double larghezzaCava, double lunghezzaCava, double rotationAngle,
                                                    double ae, double diametroFresa, double profonditaPassata, double zLavoro, double profonditaLavorazione, double zSicurezza, double extraCorsa)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { larghezzaCava, lunghezzaCava, ae, profonditaPassata, diametroFresa, profonditaLavorazione }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(zSicurezza, zLavoro),
                                                  new KeyValuePair<double, double>(larghezzaCava, diametroFresa), 
                                              })
              ) return;

            //1. Interpolazione in entrata del taglio – Raggio programmato (radm) = 50% di Dc.
            //2. G1 con ae = 0,1 x Dc.
            //3. Interpolazione in uscita dal taglio – raggio programmato (radm) = 50% di Dc.
            //4. Traslazione rapida alla posizione iniziale successiva.
            //5. Ripetizione del ciclo.
            /*
             * la cava base ha direzione +X
             * 
             * la larghezza va da +y a -y
             * 
             * Come schema prendere Sand B121
             */

            var raggioFresa = diametroFresa / 2;

            var dvf = larghezzaCava - diametroFresa;

            /* TODO : l'avanzamento al centro dell'utensile dovrebbe essere fatto
             * in fase di creazione programma, come sto facendo ora va bene solamente 
             * perchè ho solamente una misura di raggio
             * 
             * Setto avanzamento nel centro dell'utensile nei raccordi. */

            var feedWork = 0.0d;

            //var feedSetted = program.FeedDictionary.TryGetValue(MoveType.Work, out  feedWork);

            //if (feedSetted)
            //{
            //    //program.FeedDictionary[MoveType.Cw] = feedWork / 2;
            //    //program.FeedDictionary[MoveType.Ccw] = feedWork / 2;
            //}

            //var raggio = dvf / 2;
            //var raggioRit = raggio - ae;

            var metaLarghezzaCava = larghezzaCava / 2;

            var rotationMatrix = new Matrix3D();

            rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), rotationAngle), new System.Windows.Media.Media3D.Point3D(puntoInizialeX, puntoInizialeY, 0));

            /*
             * Setto punto iniziale.
             */

            var xIniPoint = puntoInizialeX - extraCorsa - raggioFresa;

            // Punto y Iniziale
            var yIniPoint = puntoInizialeY - metaLarghezzaCava + raggioFresa;

            var zCurrent = zLavoro;

            var zFinale = zLavoro - profonditaLavorazione;

            var xFin = puntoInizialeX + lunghezzaCava + raggioFresa + extraCorsa;

            // Y Finale Arco Entrata
            var entryArcEndY = puntoInizialeY - metaLarghezzaCava + raggioFresa + raggioFresa;

            // Y Finale Movimento Lineare
            var linearMoveEndY = puntoInizialeY + metaLarghezzaCava - raggioFresa - raggioFresa;

            // Y Finale Arco Uscita
            var exitArcEndY = puntoInizialeY + metaLarghezzaCava - raggioFresa;

            while (zCurrent > zFinale)
            {
                zCurrent -= profonditaPassata;

                if (zCurrent < zFinale)
                    zCurrent = zFinale;


                moveCollectionList.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniPoint, yIniPoint, null, rotationMatrix);

                moveCollectionList.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);

                moveCollectionList.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);

                var xCurrent = xIniPoint;



                while (xCurrent < xFin)
                {
                    // Avanzo in x in modo Lineare
                    moveCollectionList.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xCurrent, yIniPoint, null, rotationMatrix);

                    //1. Interpolazione in entrata del taglio – Raggio programmato (radm) = 50% di Dc.
                    moveCollectionList.AddArcMove(AxisAbilited.Xy,
                     xCurrent + raggioFresa,
                     entryArcEndY,
                     null,
                     raggioFresa,
                     false,
                     new Point2D(xCurrent, entryArcEndY),
                     rotationMatrix);

                    //2. G1 con ae = 0,1 x Dc.
                    moveCollectionList.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xCurrent + raggioFresa, linearMoveEndY, null, rotationMatrix);

                    //3. Interpolazione in uscita dal taglio – raggio programmato (radm) = 50% di Dc.
                    moveCollectionList.AddArcMove(AxisAbilited.Xy,
                     xCurrent,
                     exitArcEndY,
                     null,
                     raggioFresa,
                     false,
                     new Point2D(xCurrent, linearMoveEndY),
                     rotationMatrix);

                    //4. Traslazione rapida alla posizione iniziale successiva.
                    moveCollectionList.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Xy, xCurrent, yIniPoint, null, rotationMatrix);

                    xCurrent += ae;

                    if (xCurrent >= xFin)
                        xCurrent = xFin;

                }

                moveCollectionList.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


                moveCollectionList.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniPoint, yIniPoint, null, rotationMatrix);

            }

            // Vado a Z sicurezza
            moveCollectionList.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
        }

        /// <summary>
        /// Programma fresatura trocoidale Larghezza Minore 2xDc
        /// </summary>
        /// <param name="moveCollection"></param>
        /// <param name="puntoInizialeX"></param>
        /// <param name="puntoInizialeY"></param>
        /// <param name="larghezzaCava"></param>
        /// <param name="lunghezzaCava"></param>
        /// <param name="rotationAngle"></param>
        /// <param name="step"></param>
        /// <param name="diametroFresa"></param>
        /// <param name="profonditaPassata"></param>
        /// <param name="zLavoro"></param>
        /// <param name="profonditaLavorazione"></param>
        /// <param name="zSicurezza"></param>
        /// <param name="extraCorsa"></param>
        private static void ProcessTrochoidalMilling(ProgramOperation program, MoveActionCollection moveCollection,
                                                    double puntoInizialeX, double puntoInizialeY, double larghezzaCava, double lunghezzaCava, double rotationAngle,
                                                    double step, double diametroFresa, double profonditaPassata, double zLavoro, double profonditaLavorazione, double zSicurezza, double extraCorsa)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { larghezzaCava, lunghezzaCava, step, profonditaPassata, diametroFresa, profonditaLavorazione }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(zSicurezza, zLavoro),
                                                  new KeyValuePair<double, double>(larghezzaCava, diametroFresa), 
                                              })
              ) return;



            var dvf = larghezzaCava - diametroFresa;

            /* TODO : l'avanzamento al centro dell'utensile dovrebbe essere fatto
             * in fase di creazione programma, come sto facendo ora va bene solamente 
             * perchè ho solamente una misura di raggio
             * 
             * Setto avanzamento nel centro dell'utensile nei raccordi. */

            var feedWork = 0.0d;

            //var feedSetted = program.FeedDictionary.TryGetValue(MoveType.Work, out  feedWork);

            //if (feedSetted)
            //{
            //    var feedCenterTool = (dvf / larghezzaCava) * feedWork;
            //  //  program.FeedDictionary[MoveType.Cw] = feedCenterTool;
            //  //  program.FeedDictionary[MoveType.Ccw] = feedCenterTool;
            //}

            var raggio = dvf / 2;
            var raggioRit = raggio - step;

            if (raggioRit <= 0)
                return;
            /* todo : 
             * il raggio è troppo piccolo rispetto a step..
             */

            var xIniPoint = puntoInizialeX - extraCorsa - raggio;
            var yIniPoint = puntoInizialeY;

            var rotationMatrix = new Matrix3D();

            rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), rotationAngle), new System.Windows.Media.Media3D.Point3D(puntoInizialeX, puntoInizialeY, 0));


            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniPoint, yIniPoint, null, rotationMatrix);

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);



            var zCurrent = zLavoro;

            var zFinale = zLavoro - profonditaLavorazione;

            var xFin = puntoInizialeX + lunghezzaCava + raggio + extraCorsa;


            while (zCurrent > zFinale)
            {
                zCurrent -= profonditaPassata;

                if (zCurrent < zFinale)
                    zCurrent = zFinale;


                moveCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);


                var xCurrent = xIniPoint;



                while (xCurrent < xFin)
                {
                    //ARCO 3/4 GIRO..
                    /*
                     * il centro del primo raggio e a xCurrent - raggio e y è la stessa
                     */

                    // Arco 3/4 Giro
                    var xAnd = xCurrent + raggio;
                    var yAnd = yIniPoint + raggio;

                    var center1 = new Point2D(xCurrent + raggio, yIniPoint);

                    moveCollection.AddArcMove(AxisAbilited.Xy,
                        xAnd,
                        yAnd,
                        null,
                        raggio,
                        false, new Point2D(center1.X, center1.Y),
                        rotationMatrix);

                    //// Arco Minorato 1/4 Giro
                    var xRitArco = xCurrent + step;

                    var yRitArco = yIniPoint + step;

                    var center2 = new Point2D(center1);
                    center2.Y += step;

                    moveCollection.AddArcMove(AxisAbilited.Xy, xRitArco, yRitArco, null, raggioRit,
                                       false, new Point2D(center2.X, center2.Y), rotationMatrix);

                    //G1 LAVORO FINO Y = 0

                    moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xRitArco, puntoInizialeY, null, rotationMatrix);

                    xCurrent += step;

                    if (xCurrent >= xFin)
                        xCurrent = xFin;

                }

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniPoint, yIniPoint, null, rotationMatrix);

            }

            // Vado a Z sicurezza
            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
        }

        /// <summary>
        /// Programma fresatura trocoidale Larghezza Minore 2xDc
        /// </summary>
        /// <param name="moveCollection"></param>
        /// <param name="puntoInizialeX"></param>
        /// <param name="puntoInizialeY"></param>
        /// <param name="larghezzaCava"></param>
        /// <param name="lunghezzaCava"></param>
        /// <param name="rotationAngle"></param>
        /// <param name="step"></param>
        /// <param name="diametroFresa"></param>
        /// <param name="profonditaPassata"></param>
        /// <param name="zLavoro"></param>
        /// <param name="profonditaLavorazione"></param>
        /// <param name="zSicurezza"></param>
        /// <param name="angleStart"></param>
        private static void ProcessArcTrochoidalMilling(ProgramOperation program, MoveActionCollection moveCollection,
                                                    double puntoInizialeX, double puntoInizialeY, double larghezzaCava, double lunghezzaCava, double rotationAngle,
                                                    double step, double diametroFresa, double profonditaPassata, double zLavoro, double profonditaLavorazione, double zSicurezza, double angleStart, double angleWidth, Point2D arcCenter, double arcRadius)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { larghezzaCava, lunghezzaCava, step, profonditaPassata, diametroFresa, profonditaLavorazione }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(zSicurezza, zLavoro),
                                                  new KeyValuePair<double, double>(larghezzaCava, diametroFresa), 
                                              })
              ) return;


            /*
             *  step deve essere minore a dvf/2
              */
            //var angleStart = GeometryHelper.DegreeToRadian(135);

            //var angleWidth = GeometryHelper.DegreeToRadian(60);

            // var arcCenter = new Point2D(0, 0);

            //var arcRadius = 105.0 / 2.0; //52.5!!

            var dvf = larghezzaCava - diametroFresa;

            /* TODO : l'avanzamento al centro dell'utensile dovrebbe essere fatto
             * in fase di creazione programma, come sto facendo ora va bene solamente 
             * perchè ho solamente una misura di raggio
             * 
             * Setto avanzamento nel centro dell'utensile nei raccordi. */

            var feedWork = 0.0d;

            //var feedSetted = program.FeedDictionary.TryGetValue(MoveType.Work, out  feedWork);

            //if (feedSetted)
            //{
            //    var feedCenterTool = (dvf / larghezzaCava) * feedWork;
            //    program.FeedDictionary[MoveType.Cw] = feedCenterTool;
            //    program.FeedDictionary[MoveType.Ccw] = feedCenterTool;
            //}

            var raggio = dvf / 2;
            var raggioRit = raggio - step;

            if (raggioRit <= 0)
                return; // !!!
            /* todo : 
             * il raggio è troppo piccolo rispetto a step..
             */

            var iniPnt = GeometryHelper.GetCoordinate(angleStart, arcRadius);

            iniPnt.X += arcCenter.X;
            iniPnt.Y += arcCenter.Y;

            var rotationMatrix = new Matrix3D();
            rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), rotationAngle), new System.Windows.Media.Media3D.Point3D(puntoInizialeX, puntoInizialeY, 0));

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, iniPnt.X, iniPnt.Y, null, rotationMatrix);

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);

            var zCurrent = zLavoro;

            var zFinale = zLavoro - profonditaLavorazione;

            while (zCurrent > zFinale)
            {
                zCurrent -= profonditaPassata;

                if (zCurrent < zFinale)
                    zCurrent = zFinale;


                moveCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
                /*
                 * qui faccio movimento per allargare .. o vado giu in interpl.
                 */

                /*
                 * devo rag. sempre con centro ut.
                 */

                var incremento = dvf / 2;

                /*al posto di spostarsi da questa parte è meglio andare parte opposta*/
                var puntoInialeInizioInterpolazione = GeometryHelper.GetArcCoordinate(angleStart, arcCenter, arcRadius,
                                                                                      incremento, true);

                // mi sposto su punto iniziale inizio primo giro
                moveCollection.AddArcMove(AxisAbilited.Xyz, puntoInialeInizioInterpolazione.X, puntoInialeInizioInterpolazione.Y, zCurrent, arcRadius, true, arcCenter);

                // faccio interpolazione per portare preforo a diametro cava,
                moveCollection.AddArcMove(AxisAbilited.Xyz, puntoInialeInizioInterpolazione.X, puntoInialeInizioInterpolazione.Y, zCurrent, incremento, true, iniPnt);

                // mi riporto al centro del punto di partenza
                /*
                 * Poi devo andare a lato opposto 
                 * 
                 */
                var puntoInialeInizioTrochoid = GeometryHelper.GetArcCoordinate(angleStart, arcCenter, arcRadius,
                                                                                   incremento, false);

                moveCollection.AddArcMove(AxisAbilited.Xyz, puntoInialeInizioTrochoid.X, puntoInialeInizioTrochoid.Y, zCurrent, arcRadius, false, arcCenter);

                var angleCurrent = angleStart;

                var angleFin = angleStart - angleWidth;


                while (angleCurrent > angleFin)
                {
                    /* 
                     * ARCO 3/4 GIRO..
                     * 
                     */

                    /*
                     * come centro prendo angolo corrente per raggio
                     */

                    var puntoCentroArco = GeometryHelper.GetCoordinate(angleCurrent, arcRadius);

                    //        // Arco 3/4 Giro
                    //        var xAnd = xCurrent + raggio;
                    //        var yAnd = yIniPoint + raggio;

                    //        var center1 = new Point2D(xCurrent + raggio, yIniPoint);

                    var puntoFinaleArco3_4 = GeometryHelper.GetPointAtDistance(arcCenter, puntoCentroArco,
                        -incremento, false, false); // in pratica estendo il raggio ( da centro a centro arco 3/4 ) per avere il punto finale

                    moveCollection.AddArcMove(AxisAbilited.Xy,
                        puntoFinaleArco3_4.X,
                        puntoFinaleArco3_4.Y,
                        null,
                        raggio,
                        false, new Point2D(puntoCentroArco.X, puntoCentroArco.Y),
                        rotationMatrix);

                    //Arco Minorato 1/4 Giro

                    var centerArc1_4 = GeometryHelper.GetPointAtDistance(arcCenter, puntoCentroArco, -incremento + raggioRit,
                                                                               false, false); ////

                    // ovvero dal centro cerchio 3/4 meno raggioRit

                    var puntoFinaleArcoRitorno = GeometryHelper.GetArcCoordinate(angleCurrent, arcCenter, arcRadius + dvf / 2 - raggioRit, raggioRit, false);

                    //var puntoFinaleArcoRitorno = GeometryHelper.GetPointAtDistance(arcCenter, angoloPuntoFinaleArcoRitorno, arcRadius + dvf / 2 - raggioRit, true, false);

                    moveCollection.AddArcMove(AxisAbilited.Xy, puntoFinaleArcoRitorno.X, puntoFinaleArcoRitorno.Y, null, raggioRit,
                                       false, new Point2D(centerArc1_4.X, centerArc1_4.Y), rotationMatrix);

                    /* ora devo trovare punto */
                    //G1 LAVORO FINO Y = 0

                    var puntoArrivoMovLineareRitorno = GeometryHelper.GetArcCoordinate(angleCurrent, arcCenter, arcRadius, raggioRit, false);

                    moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, puntoArrivoMovLineareRitorno.X, puntoArrivoMovLineareRitorno.Y, null, rotationMatrix);

                    angleCurrent -= GeometryHelper.GetAngleCorrispondence(arcRadius, step);

                    if (angleCurrent < angleFin)
                        angleCurrent = angleFin;

                }

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, iniPnt.X, iniPnt.Y, null, rotationMatrix);

            }

            // Vado a Z sicurezza
            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
        }

        private static void ProcessSingleLineFinish(MoveActionCollection program, double startX, double startY,
                                            double angleRotation, double lunghezza, double larghezza,
                                            double profondita, double profPassata, double diaFresa, double secureZ, double extraCorsa, double zLavoro, bool finishWithComp)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { lunghezza, larghezza, profondita, diaFresa }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(secureZ, zLavoro),
                                              }) ||
                (diaFresa > larghezza)
             ) return;

            var rotationMatrix = new Matrix3D();

            rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), angleRotation), new System.Windows.Media.Media3D.Point3D(startX, startY, 0));
            /*
             * Creo le 2 linee della cava in non ruotate
             */

            var compType = 0;



            var l1 = new Line2D
              {
                  Start = new Point2D
                  {
                      X = startX,
                      Y = startY + larghezza / 2,
                  },

                  End = new Point2D
                  {
                      X = startX + lunghezza,
                      Y = startY + larghezza / 2,
                  }
              };

            if (finishWithComp)
                compType = 2;

            MillProgrammingHelper.ProcessSingleLineFinish(program, l1, profondita, profPassata, diaFresa, secureZ, extraCorsa, zLavoro, false, compType, rotationMatrix);

            var l2 = new Line2D
            {
                Start = new Point2D
                {
                    X = startX + lunghezza,
                    Y = startY - larghezza / 2,
                },

                End = new Point2D
                {
                    X = startX,
                    Y = startY - larghezza / 2,
                }
            };

            //if (finishWithComp)
            //    compType = 2;
            MillProgrammingHelper.ProcessSingleLineFinish(program, l2, profondita, profPassata, diaFresa, secureZ, extraCorsa, zLavoro, false, compType, rotationMatrix);



        }

        private static void ProcessRoughLineMilling(MoveActionCollection program, double startX, double startY,
                                                    double angleRotation, double lunghezza, double larghezza,
                                                    double profondita, double profPassata, double larghezzaPassata,
                                                    double diaFresa, double secureZ, double extraCorsa, double zLavoro)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { lunghezza, larghezza, profondita, profPassata, larghezzaPassata, diaFresa }) ||
               CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(secureZ, zLavoro),
                                              }) ||
                (diaFresa > larghezza)
             ) return;

            /*
             * 
             * xy iniziale 
             */
            /*
             * 
             */

            var zCurrent = zLavoro;
            var zFinale = zLavoro - profondita;

            while (zCurrent > zFinale)
            {
                zCurrent -= profPassata;
                if (zCurrent < zFinale)
                    zCurrent = zFinale;

                var yIniziale = startY;

                var xIniziale = startX - diaFresa / 2 - extraCorsa;

                var xFinale = startX + lunghezza + diaFresa / 2 + extraCorsa;

                var raggioFresa = diaFresa / 2;

                var rotationMatrix = new Matrix3D();

                rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), angleRotation), new System.Windows.Media.Media3D.Point3D(startX, startY, 0));

                /*
                 * ripartiamo daccapo senza ottimizzazione passate
                 * 
                 * la forma base è con lunghezza su asse x e larghezza su y.
                 * 
                 *    -------------------------------
                 *    |                             |
                 * o  |                             |
                 *    |                             |
                 *    -------------------------------
                 * 
                 * Il ciclo è posizionamento xy in o , 
                 * - apertura centrale
                 * - poi itero seguente ciclo 
                 * 
                 *  counter = yBase * counter * larghezzaPassata
                 *  
                 * -- up Ycorrente incrementato larghPassata .
                 *  work sx
                 * -- down Ycorrente diminuito larghPassata .
                 *  work dx
                 *    counter ++
                 */

                program.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniziale, yIniziale, null, rotationMatrix);

                program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);

                program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zCurrent);

                program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale, yIniziale, null, rotationMatrix);

                var counter = 1;

                var maxY = startY + (larghezza / 2) - raggioFresa;

                var minY = startY - (larghezza / 2) + raggioFresa;

                var continueCycle = true;

                /*
                 * todo : cercare di ottimizzare ciclo per evitare passate minime.
                 */
                while (continueCycle && larghezza > diaFresa)
                {
                    var currentY = yIniziale + (counter * larghezzaPassata);

                    if (currentY >= maxY)
                    {
                        currentY = maxY;
                        continueCycle = false;
                    }

                    program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale, currentY, null, rotationMatrix);

                    program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xIniziale, currentY, null, rotationMatrix);

                    currentY = yIniziale - (counter * larghezzaPassata);

                    if (currentY <= minY)
                    {
                        currentY = minY;
                        continueCycle = false;
                    }

                    program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xIniziale, currentY, null, rotationMatrix);

                    program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale, currentY, null, rotationMatrix);

                    counter++;
                }

                program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);
            }

        }

    }

    //public enum ScanalaturaOperazioniEnum
    //{
    //    Sgrossatura,
    //    Finitura,
    //    SgrossaturaTrocoidale,
    //    Smussatura
    //}

    public enum ScanalaturaCavaMetodoLavorazione
    {
        Tradizionale,
        Trocoidale,
    }
}

// codice commentato 18/07/2011 working 3 metodi
///// <summary>
///// Programma fresatura trocoidale Larghezza Maggiore 2xDc
///// </summary>
///// <param name="program"></param>
///// <param name="puntoInizialeX"></param>
///// <param name="puntoInizialeY"></param>
///// <param name="larghezzaCava"></param>
///// <param name="lunghezzaCava"></param>
///// <param name="rotationAngle"></param>
///// <param name="ae"></param>
///// <param name="diametroFresa"></param>
///// <param name="profonditaPassata"></param>
///// <param name="zLavoro"></param>
///// <param name="profonditaLavorazione"></param>
///// <param name="zSicurezza"></param>
///// <param name="extraCorsa"></param>
//private static void ProcessTrochoidalMillingLarge(ProgramPhase program,
//                                            double puntoInizialeX, double puntoInizialeY, double larghezzaCava, double lunghezzaCava, double rotationAngle,
//                                            double ae, double diametroFresa, double profonditaPassata, double zLavoro, double profonditaLavorazione, double zSicurezza, double extraCorsa)
//{
//    if (CheckValueHelper.GreatherThanZero(new[] { larghezzaCava, lunghezzaCava, ae, profonditaPassata, diametroFresa, profonditaLavorazione }) ||
//        CheckValueHelper.GreatherThan(new[]
//                                      {
//                                          new KeyValuePair<double, double>(zSicurezza, zLavoro),
//                                          new KeyValuePair<double, double>(larghezzaCava, diametroFresa), 
//                                      })
//      ) return;

//    //1. Interpolazione in entrata del taglio – Raggio programmato (radm) = 50% di Dc.
//    //2. G1 con ae = 0,1 x Dc.
//    //3. Interpolazione in uscita dal taglio – raggio programmato (radm) = 50% di Dc.
//    //4. Traslazione rapida alla posizione iniziale successiva.
//    //5. Ripetizione del ciclo.
//    /*
//     * la cava base ha direzione +X
//     * 
//     * la larghezza va da +y a -y
//     * 
//     * Come schema prendere Sand B121
//     */

//    var raggioFresa = diametroFresa / 2;

//    var dvf = larghezzaCava - diametroFresa;

//    /* TODO : l'avanzamento al centro dell'utensile dovrebbe essere fatto
//     * in fase di creazione programma, come sto facendo ora va bene solamente 
//     * perchè ho solamente una misura di raggio
//     * 
//     * Setto avanzamento nel centro dell'utensile nei raccordi. */

//    var feedWork = 0.0d;

//    var feedSetted = program.FeedDictionary.TryGetValue(MoveType.Work, out  feedWork);

//    if (feedSetted)
//    {
//        program.FeedDictionary[MoveType.Cw] = feedWork / 2;
//        program.FeedDictionary[MoveType.Ccw] = feedWork / 2;
//    }

//    //var raggio = dvf / 2;
//    //var raggioRit = raggio - ae;

//    var metaLarghezzaCava = larghezzaCava / 2;

//    var rotationMatrix = new Matrix3D();

//    rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), rotationAngle), new System.Windows.Media.Media3D.Point3D(puntoInizialeX, puntoInizialeY, 0));

//    /*
//     * Setto punto iniziale.
//     */

//    var xIniPoint = puntoInizialeX - extraCorsa - larghezzaCava / 2;

//    // Punto y Iniziale
//    var yIniPoint = puntoInizialeY - metaLarghezzaCava + raggioFresa;

//    var zCurrent = zLavoro;

//    var zFinale = zLavoro - profonditaLavorazione;

//    var xFin = puntoInizialeX + lunghezzaCava + larghezzaCava / 2 + extraCorsa;

//    // Y Finale Arco Entrata
//    var entryArcEndY = puntoInizialeY - metaLarghezzaCava + raggioFresa + raggioFresa;

//    // Y Finale Movimento Lineare
//    var linearMoveEndY = puntoInizialeY + metaLarghezzaCava - raggioFresa - raggioFresa;

//    // Y Finale Arco Uscita
//    var exitArcEndY = puntoInizialeY + metaLarghezzaCava - raggioFresa;

//    while (zCurrent > zFinale)
//    {
//        zCurrent -= profonditaPassata;

//        if (zCurrent < zFinale)
//            zCurrent = zFinale;


//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniPoint, yIniPoint, null, rotationMatrix);

//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);

//        program.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);

//        var xCurrent = xIniPoint;



//        while (xCurrent < xFin)
//        {
//            // Avanzo in x in modo Lineare
//            program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xCurrent, yIniPoint, null, rotationMatrix);

//            //1. Interpolazione in entrata del taglio – Raggio programmato (radm) = 50% di Dc.
//            program.AddArcMove(AxisAbilited.Xy,
//             xCurrent + raggioFresa,
//             entryArcEndY,
//             null,
//             raggioFresa,
//             false,
//             new Point2D(xCurrent, entryArcEndY),
//             rotationMatrix);

//            //2. G1 con ae = 0,1 x Dc.
//            program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xCurrent + raggioFresa, linearMoveEndY, null, rotationMatrix);

//            //3. Interpolazione in uscita dal taglio – raggio programmato (radm) = 50% di Dc.
//            program.AddArcMove(AxisAbilited.Xy,
//             xCurrent,
//             exitArcEndY,
//             null,
//             raggioFresa,
//             false,
//             new Point2D(xCurrent, linearMoveEndY),
//             rotationMatrix);

//            //4. Traslazione rapida alla posizione iniziale successiva.
//            program.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Xy, xCurrent, yIniPoint, null, rotationMatrix);

//            xCurrent += ae;

//            if (xCurrent >= xFin)
//                xCurrent = xFin;

//        }

//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniPoint, yIniPoint, null, rotationMatrix);

//    }

//    // Vado a Z sicurezza
//    program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//}

///// <summary>
///// Programma fresatura trocoidale Larghezza Minore 2xDc
///// </summary>
///// <param name="program"></param>
///// <param name="puntoInizialeX"></param>
///// <param name="puntoInizialeY"></param>
///// <param name="larghezzaCava"></param>
///// <param name="lunghezzaCava"></param>
///// <param name="rotationAngle"></param>
///// <param name="step"></param>
///// <param name="diametroFresa"></param>
///// <param name="profonditaPassata"></param>
///// <param name="zLavoro"></param>
///// <param name="profonditaLavorazione"></param>
///// <param name="zSicurezza"></param>
///// <param name="extraCorsa"></param>
//private static void ProcessTrochoidalMilling(ProgramPhase program,
//                                            double puntoInizialeX, double puntoInizialeY, double larghezzaCava, double lunghezzaCava, double rotationAngle,
//                                            double step, double diametroFresa, double profonditaPassata, double zLavoro, double profonditaLavorazione, double zSicurezza, double extraCorsa)
//{
//    if (CheckValueHelper.GreatherThanZero(new[] { larghezzaCava, lunghezzaCava, step, profonditaPassata, diametroFresa, profonditaLavorazione }) ||
//        CheckValueHelper.GreatherThan(new[]
//                                      {
//                                          new KeyValuePair<double, double>(zSicurezza, zLavoro),
//                                          new KeyValuePair<double, double>(larghezzaCava, diametroFresa), 
//                                      })
//      ) return;



//    var dvf = larghezzaCava - diametroFresa;

//    /* TODO : l'avanzamento al centro dell'utensile dovrebbe essere fatto
//     * in fase di creazione programma, come sto facendo ora va bene solamente 
//     * perchè ho solamente una misura di raggio
//     * 
//     * Setto avanzamento nel centro dell'utensile nei raccordi. */

//    var feedWork = 0.0d;

//    var feedSetted = program.FeedDictionary.TryGetValue(MoveType.Work, out  feedWork);

//    if (feedSetted)
//    {
//        var feedCenterTool = (dvf / larghezzaCava) * feedWork;
//        program.FeedDictionary[MoveType.Cw] = feedCenterTool;
//        program.FeedDictionary[MoveType.Ccw] = feedCenterTool;
//    }

//    var raggio = dvf / 2;
//    var raggioRit = raggio - step;

//    var xIniPoint = puntoInizialeX - extraCorsa - larghezzaCava / 2;
//    var yIniPoint = puntoInizialeY;

//    var rotationMatrix = new Matrix3D();

//    rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), rotationAngle), new System.Windows.Media.Media3D.Point3D(puntoInizialeX, puntoInizialeY, 0));


//    program.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniPoint, yIniPoint, null, rotationMatrix);

//    program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);



//    var zCurrent = zLavoro;

//    var zFinale = zLavoro - profonditaLavorazione;

//    while (zCurrent > zFinale)
//    {
//        zCurrent -= profonditaPassata;

//        if (zCurrent < zFinale)
//            zCurrent = zFinale;


//        program.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);


//        var xCurrent = xIniPoint;

//        var xFin = puntoInizialeX + lunghezzaCava + larghezzaCava / 2 + extraCorsa;


//        while (xCurrent < xFin)
//        {
//            //ARCO 3/4 GIRO..
//            /*
//             * il centro del primo raggio e a xCurrent - raggio e y è la stessa
//             */

//            // Arco 3/4 Giro
//            var xAnd = xCurrent + raggio;
//            var yAnd = yIniPoint + raggio;

//            var center1 = new Point2D(xCurrent + raggio, yIniPoint);

//            program.AddArcMove(AxisAbilited.Xy,
//                xAnd,
//                yAnd,
//                null,
//                raggio,
//                false, new Point2D(center1.X, center1.Y),
//                rotationMatrix);

//            //// Arco Minorato 1/4 Giro
//            var xRitArco = xCurrent + step;

//            var yRitArco = yIniPoint + step;

//            var center2 = new Point2D(center1);
//            center2.Y += step;

//            program.AddArcMove(AxisAbilited.Xy, xRitArco, yRitArco, null, raggioRit,
//                               false, new Point2D(center2.X, center2.Y), rotationMatrix);

//            //G1 LAVORO FINO Y = 0

//            program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xRitArco, puntoInizialeY, null, rotationMatrix);

//            xCurrent += step;

//            if (xCurrent >= xFin)
//                xCurrent = xFin;

//        }

//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniPoint, yIniPoint, null, rotationMatrix);

//    }

//    // Vado a Z sicurezza
//    program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//}


//public static void ProcessRoughLineMilling(ProgramPhase program, double startX, double startY,
//                                            double angleRotation, double lunghezza, double larghezza,
//                                            double profondita, double profPassata, double larghezzaPassata,
//                                            double diaFresa, double secureZ, double extraCorsa, double zLavoro)
//{
//    if (CheckValueHelper.GreatherThanZero(new[] { lunghezza, larghezza, profondita, profPassata, larghezzaPassata, diaFresa }) ||
//       CheckValueHelper.GreatherThan(new[]
//                                      {
//                                          new KeyValuePair<double, double>(secureZ, zLavoro),
//                                      }) ||
//        (diaFresa > larghezza)
//     ) return;

//    /*
//     * 
//     * xy iniziale 
//     */
//    /*
//     * 
//     */

//    var zCurrent = zLavoro;
//    var zFinale = zLavoro - profondita;

//    while (zCurrent > zFinale)
//    {
//        zCurrent -= profPassata;
//        if (zCurrent < zFinale)
//            zCurrent = zFinale;

//        var yIniziale = startY;

//        var xIniziale = startX - diaFresa / 2 - extraCorsa;

//        var xFinale = startX + lunghezza + diaFresa / 2 + extraCorsa;

//        var raggioFresa = diaFresa / 2;

//        var rotationMatrix = new Matrix3D();

//        rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), angleRotation), new System.Windows.Media.Media3D.Point3D(startX, startY, 0));

//        /*
//         * ripartiamo daccapo senza ottimizzazione passate
//         * 
//         * la forma base è con lunghezza su asse x e larghezza su y.
//         * 
//         *    -------------------------------
//         *    |                             |
//         * o  |                             |
//         *    |                             |
//         *    -------------------------------
//         * 
//         * Il ciclo è posizionamento xy in o , 
//         * - apertura centrale
//         * - poi itero seguente ciclo 
//         * 
//         *  counter = yBase * counter * larghezzaPassata
//         *  
//         * -- up Ycorrente incrementato larghPassata .
//         *  work sx
//         * -- down Ycorrente diminuito larghPassata .
//         *  work dx
//         *    counter ++
//         */

//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniziale, yIniziale, null, rotationMatrix);

//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);

//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zCurrent);

//        program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale, yIniziale, null, rotationMatrix);

//        var counter = 1;

//        var maxY = startY + (larghezza / 2) - raggioFresa;

//        var minY = startY - (larghezza / 2) + raggioFresa;

//        var continueCycle = true;

//        /*
//         * todo : cercare di ottimizzare ciclo per evitare passate minime.
//         */
//        while (continueCycle && larghezza > diaFresa)
//        {
//            var currentY = yIniziale + (counter * larghezzaPassata);

//            if (currentY >= maxY)
//            {
//                currentY = maxY;
//                continueCycle = false;
//            }

//            program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale, currentY, null, rotationMatrix);

//            program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xIniziale, currentY, null, rotationMatrix);

//            currentY = yIniziale - (counter * larghezzaPassata);

//            if (currentY <= minY)
//            {
//                currentY = minY;
//                continueCycle = false;
//            }

//            program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xIniziale, currentY, null, rotationMatrix);

//            program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale, currentY, null, rotationMatrix);

//            counter++;
//        }

//        program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);


//    }

//}

///*
// * todo: potrei mettere un valore di feed diverso per cava e contornatura..
// */

//// Variabile del numero di passate ( il +1 aggiunge passata di apertura.)
//var nPassateTeorical = ((larghezza - diaFresa) / larghezzaPassata) + 1;

//var nPassate = Math.Ceiling(nPassateTeorical);

///*
// * Ragionamento : 
// * 
// * Cava con 2 Passate :
// * -------------------------
// * |           |            |
// * |           |            |
// * |           |            |
// * |           |            |
// * |           |            |
// * -------------------------
// *
// * Per trovare y devo dividere la (larghezza ) per il doppio del numero passate
// * -------------------------
// * |     |     |      |     |
// * |     |     |      |     |
// * |     |     |      |     |
// * |     |     |      |     |
// * |     |     |      |     |
// * -------------------------
// */

///*
// * todo : qui ottimizzo, se mi lascia una passata minima lo inglobo nelle altre.
// */

//var larghezzaUtile = larghezza - diaFresa;

//var step = larghezzaUtile / nPassate;

//var yMin = yIniziale - larghezzaUtile / 2;

//yMin += step / 2;

//var passateValues = new List<double>();

//passateValues.Add(yMin);

//for (var i = 1; i < nPassate; i++)
//{
//    passateValues.Add(step * i);
//}

//var currentIndex = (int)Math.Truncate(passateValues.Count() / 2.0d);

//var currentIndexUp = currentIndex;
//var currentIndexDown = currentIndex;

//for (var i = 0; i < nPassate; i++)
//{
//    var t = i % 2;

//    var currentY = 0.0d;

//    if (t == 0)
//    {
//        currentIndexUp++;
//        currentY = passateValues.ElementAtOrDefault(currentIndexUp);

//    }
//    else
//    {
//        currentIndexDown--;
//        currentY = passateValues.ElementAtOrDefault(currentIndexDown);
//    }

//    var iniPoint = new Point3D(xIniziale, currentY, zCurrent);

//    var endPoint = new Point3D(xFinale, currentY, zCurrent);

//    /*
//     * Qui ho tutti i valori . quindi faccio i 4 movimenti
//     * 
//     * 1) Muovo XY Partenza
//     * 2) Muovo Z Sicurezza
//     * 3) Muovo Z Lavoro
//     * 4) Muovo XY Finale
//     * 5) Muovo Z Sicurezza
//     * 
//     * Per ora non ho contato concordanza e discordanza.
//     */

//    // 1) 

//    program.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniziale, currentY, null);

//    // 2)
//    program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);

//    // 3)
//    program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zCurrent);

//    // 4)
//    program.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale, currentY, zCurrent);

//    // 5)
//    program.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);

//}