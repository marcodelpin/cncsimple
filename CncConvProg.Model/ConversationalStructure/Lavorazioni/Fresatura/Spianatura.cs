using System;
using System.Collections.Generic;
using System.Linq;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Geometry;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura
{
    [Serializable]
    public sealed class Spianatura : LavorazioneFresatura
    {
        public Spianatura()
        {
            StartPoint = SquareShapeHelper.SquareShapeStartPoint.Center;

            Sgrossatura = new Operazione(this, LavorazioniEnumOperazioni.FresaturaSpianaturaSgrossatura);

            Finitura = new Operazione(this, LavorazioniEnumOperazioni.FresaturaSpianaturaFinitura);
        }

        public override List<Operazione> Operazioni
        {
            get
            {
                return new List<Operazione> { Sgrossatura, Finitura };
            }
        }

        public SquareShapeHelper.SquareShapeStartPoint StartPoint { get; set; }

        public Operazione Sgrossatura { get; set; }

        public Operazione Finitura { get; set; }

        public SpiantaturaMetodologia ModoSgrossatura { get; set; }
        public SpiantaturaMetodologia ModoFinitura { get; set; }
        public double SovrametalloPerFinitura { get; set; }

        public double Sovrametallo { get; set; }

        public double LivelloZ { get; set; }

        public double Larghezza { get; set; }

        public double Altezza { get; set; }

        public double PuntoStartX { get; set; }

        public double PuntoStartY { get; set; }

        //internal override Utensile CreateTool(LavorazioniEnumOperazioni enumOperationType)
        //{
        //    var unit = Singleton.Instance.MeasureUnit;

        //    //switch ((SpianaturaEnumOperazioni)(enumOperationType))
        //    //{

        //    //    case SpianaturaEnumOperazioni.Sgrossatura:
        //    //    case SpianaturaEnumOperazioni.Finitura:
        //            return new FresaSpianare(unit);

        //    //}

        //    throw new NotImplementedException();
        //}

        //internal override List<Utensile> GetCompatibleTools(LavorazioniEnumOperazioni operationType, MagazzinoUtensile magazzino)
        //{
        //    var unit = Singleton.Instance.MeasureUnit;

        //    var tools = magazzino.GetTools<FresaSpianare>(unit);

        //    return new List<Utensile>(tools);
        //}


        public override string Descrizione
        {
            get { return "Face Milling " + Larghezza + " x " + Altezza; }
        }

        //private Point2D GetCenterPoint()
        //{
        //    switch (StartPoint)
        //    {
        //        case SquareShapeStartPoint.UpLeft:
        //            return new Point2D { X = PuntoStartX + Larghezza / 2, Y = PuntoStartY - Altezza / 2 };

        //        case SquareShapeStartPoint.UpRight:
        //            return new Point2D { X = PuntoStartX - Larghezza / 2, Y = PuntoStartY - Altezza / 2 };

        //        case SquareShapeStartPoint.DownLeft:
        //            return new Point2D { X = PuntoStartX + Larghezza / 2, Y = PuntoStartY + Altezza / 2 };

        //        case SquareShapeStartPoint.DownRight:
        //            return new Point2D { X = PuntoStartX - Larghezza / 2, Y = PuntoStartY + Altezza / 2 };

        //        case SquareShapeStartPoint.Center:
        //        default:
        //            return new Point2D(PuntoStartX, PuntoStartY);
        //    }
        //}

        protected override List<IEntity3D> GetFinalPreview()
        {
            var centerPoint = GetCenterPoint();

            var l1 = new Line3D
                         {
                             Start = new Point3D
                                         {
                                             X = centerPoint.X + Larghezza / 2,
                                             Y = centerPoint.Y + Altezza / 2,
                                             Z = LivelloZ,
                                         },

                             End = new Point3D
                                       {
                                           X = centerPoint.X + Larghezza / 2,
                                           Y = centerPoint.Y - Altezza / 2,
                                           Z = LivelloZ,
                                       }
                         };

            var l2 = new Line3D
                         {
                             Start = l1.End,

                             End = new Point3D
                                       {
                                           X = centerPoint.X - Larghezza / 2,
                                           Y = centerPoint.Y - Altezza / 2,
                                           Z = LivelloZ,
                                       }
                         };

            var l3 = new Line3D
                         {
                             Start = l2.End,

                             End = new Point3D
                                       {
                                           X = centerPoint.X - Larghezza / 2,
                                           Y = centerPoint.Y + Altezza / 2,
                                           Z = LivelloZ,
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
                entity2D.PlotStyle = EnumPlotStyle.Element;
            }

            return rslt;
        }

        private Point2D GetCenterPoint()
        {
            return SquareShapeHelper.GetCenterPoint(StartPoint, PuntoStartX, PuntoStartY, Larghezza, Altezza);
        }

        protected override void CreateSpecificProgram(ProgramPhase programPhase, Operazione operazione)
        {
            var parametro = operazione.GetParametro<ParametroFresaSpianare>();

            if (parametro == null)
                throw new NullReferenceException();

            var larghezzaPassata = parametro.GetLarghezzaPassata();

            var profPassata = parametro.GetProfonditaPassata();

            var diaFresa = parametro.DiametroMinimoFresa;

            diaFresa -= parametro.GetRaggioInserto() * 2;

            var diaIngombroFresa = parametro.DiametroIngombroFresa;

            var sovraMetallo = Sovrametallo - SovrametalloPerFinitura;

            var ottimizza = false; // per ora non lo lascio modificare da utente 

            var passataMinimaPercentuale = 20.0d; // -- ,, valori sballati potrebbero causare bug non previsti


            var moveCollection = new MoveActionCollection();

            var centerPoint = GetCenterPoint();


            switch ((LavorazioniEnumOperazioni)operazione.OperationType)
            {
                case LavorazioniEnumOperazioni.FresaturaSpianaturaSgrossatura:
                    {
                        var zLavoro = LivelloZ + SovrametalloPerFinitura;

                        switch (ModoSgrossatura)
                        {
                            case SpiantaturaMetodologia.Tradizionale:
                                {
                                    OneDirectionFaceMilling(moveCollection, centerPoint.X, centerPoint.Y, Larghezza, Altezza, larghezzaPassata, diaFresa, diaIngombroFresa, sovraMetallo, profPassata, zLavoro, SicurezzaZ, ExtraCorsa);
                                } break;

                            case SpiantaturaMetodologia.Spirale:
                                {
                                    SpiralFaceMillingV2(moveCollection, centerPoint.X, centerPoint.Y, Larghezza, Altezza, larghezzaPassata, diaFresa, diaIngombroFresa, sovraMetallo, profPassata, zLavoro, SicurezzaZ, ExtraCorsa, ottimizza, passataMinimaPercentuale);
                                } break;
                        }

                    } break;

                case LavorazioniEnumOperazioni.FresaturaSpianaturaFinitura:
                    {
                        switch (ModoFinitura)
                        {
                            case SpiantaturaMetodologia.Tradizionale:
                                {
                                    OneDirectionFaceMilling(moveCollection,
                                        centerPoint.X, centerPoint.Y,
                                        Larghezza, Altezza, larghezzaPassata,
                                        diaFresa,
                                        diaIngombroFresa,
                                        SovrametalloPerFinitura,
                                        profPassata,
                                        LivelloZ,
                                        SicurezzaZ,
                                        ExtraCorsa
                                        );

                                } break;

                            case SpiantaturaMetodologia.Spirale:
                                {
                                    // richiamo stesso procedimento , metto solo profondita == sovrametallo
                                    // magari fare spiarale singolo in un metodo e richiamarla con sgrossatura più volte
                                    SpiralFaceMillingV2(moveCollection,
                                        centerPoint.X, centerPoint.Y,
                                        Larghezza, Altezza,
                                        larghezzaPassata,
                                        diaFresa,
                                        diaIngombroFresa,
                                        SovrametalloPerFinitura,
                                        profPassata,
                                        LivelloZ, SicurezzaZ, ExtraCorsa,
                                        ottimizza,
                                        passataMinimaPercentuale);

                                } break;
                        }

                    } break;

            }

            var mm = base.GetFinalProgram(moveCollection);

            foreach (var variable in mm)
            {
                programPhase.AddMoveAction(variable);
            }
        }
        //internal override ProgramPhase GetOperationProgram(Operazione operazione)
        //{
        //    /*
        //     * il richiamo degli utensili magari farlo e delle operazioni comuni magari farlo in astratto più a monte
        //     * 
        //     */

        //    /*
        //     * 
        //     * su spianatura tradizionale se rimane ultima passata piccola lo togle con il centro della fresa.
        //     */
        //    var program = new ProgramPhase(SicurezzaZ);

        //    // cambio utensile // se 
        //    var toolChange = new ChangeToolAction(program, operazione);

        //    program.ActiveAsseC(true);

        //    var parametro = operazione.GetParametro<ParametroFresaSpianare>();

        //    if (parametro == null)
        //        throw new NullReferenceException();

        //    var larghezzaPassata = parametro.GetLarghezzaPassata();

        //    var profPassata = parametro.GetProfonditaPassata();

        //    var diaFresa = parametro.DiametroMinimoFresa;

        //    diaFresa -= parametro.GetRaggioInserto() * 2;

        //    var diaIngombroFresa = parametro.DiametroIngombroFresa;

        //    var secureFeed = 1;

        //    var extraCorsa = 1;

        //    var feed = parametro.GetFeed(FeedType.ASync);

        //    var sovraMetallo = Sovrametallo - SovrametalloPerFinitura;


        //    var ottimizza = false; // per ora non lo lascio modificare da utente 

        //    var passataMinimaPercentuale = 20.0d; // -- ,, valori sballati potrebbero causare bug non previsti



        //    if (feed <= 0)
        //        return null;

        //    var feedDictionary = new Dictionary<MoveType, double>
        //                             {
        //                                 {MoveType.Rapid, 10000},

        //                                 {MoveType.SecureRapidFeed, secureFeed},
        //                                 {MoveType.Work, feed},
        //                                 {MoveType.Cw, feed},
        //                                 {MoveType.Ccw, feed},
        //                                 {MoveType.PlungeFeed, feed /2}
        //                             };

        //    program.SetFeedDictionary(feedDictionary);

        //    var moveCollection = new MoveActionCollection();

        //    var centerPoint = GetCenterPoint();


        //    switch ((LavorazioniEnumOperazioni)operazione.OperationType)
        //    {
        //        case LavorazioniEnumOperazioni.FresaturaSpianaturaSgrossatura:
        //            {
        //                var zLavoro = LivelloZ + SovrametalloPerFinitura;

        //                switch (ModoSgrossatura)
        //                {
        //                    case SpiantaturaMetodologia.Tradizionale:
        //                        {
        //                            OneDirectionFaceMilling(moveCollection, centerPoint.X, centerPoint.Y, Larghezza, Altezza, larghezzaPassata, diaFresa, diaIngombroFresa, sovraMetallo, profPassata, zLavoro, SicurezzaZ, extraCorsa);
        //                        } break;

        //                    case SpiantaturaMetodologia.Spirale:
        //                        {
        //                            SpiralFaceMillingV2(moveCollection, centerPoint.X, centerPoint.Y, Larghezza, Altezza, larghezzaPassata, diaFresa, diaIngombroFresa, sovraMetallo, profPassata, zLavoro, SicurezzaZ, extraCorsa, ottimizza, passataMinimaPercentuale);
        //                        } break;
        //                }

        //            } break;

        //        case LavorazioniEnumOperazioni.FresaturaSpianaturaFinitura:
        //            {
        //                switch (ModoFinitura)
        //                {
        //                    case SpiantaturaMetodologia.Tradizionale:
        //                        {
        //                            OneDirectionFaceMilling(moveCollection,
        //                                centerPoint.X, centerPoint.Y,
        //                                Larghezza, Altezza, larghezzaPassata,
        //                                diaFresa,
        //                                diaIngombroFresa,
        //                                SovrametalloPerFinitura,
        //                                profPassata,
        //                                LivelloZ,
        //                                SicurezzaZ,
        //                                extraCorsa
        //                                );

        //                        } break;

        //                    case SpiantaturaMetodologia.Spirale:
        //                        {
        //                            // richiamo stesso procedimento , metto solo profondita == sovrametallo
        //                            // magari fare spiarale singolo in un metodo e richiamarla con sgrossatura più volte
        //                            SpiralFaceMillingV2(moveCollection,
        //                                centerPoint.X, centerPoint.Y,
        //                                Larghezza, Altezza,
        //                                larghezzaPassata,
        //                                diaFresa,
        //                                diaIngombroFresa,
        //                                SovrametalloPerFinitura,
        //                                profPassata,
        //                                LivelloZ, SicurezzaZ, extraCorsa,
        //                                ottimizza,
        //                                passataMinimaPercentuale);

        //                        } break;
        //                }

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

        private static void OneDirectionFaceMilling(MoveActionCollection programPhase,
                                            double offsetX, double offsetY,
                                            double larghezza, double altezza,
                                            double larghPassata, double diaFresa, double diaIngombroFresa,
                                            double sovraMetallo, double profPassata,
                                            double zFinale, double zSicurezza,
                                            double extraCorsaSicurezza)
        {
            if (CheckValueHelper.GreatherOrEqualZero(new[] { sovraMetallo, extraCorsaSicurezza }) ||
                CheckValueHelper.GreatherThanZero(new[] { larghezza, altezza, larghPassata, diaFresa, diaIngombroFresa, profPassata, }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(zSicurezza, zFinale + sovraMetallo),
                                                  new KeyValuePair<double, double>(diaFresa, larghPassata),
                                              })
              ) return;




            var zCurrent = zFinale + sovraMetallo;

            var xIni = offsetX - larghezza / 2;
            var xFin = offsetX + larghezza / 2;

            var yIni = offsetY + altezza / 2 + diaFresa / 2;
            var yFin = offsetY - altezza / 2;

            xIni -= (diaIngombroFresa / 2) + extraCorsaSicurezza;
            xFin += (diaIngombroFresa / 2) + extraCorsaSicurezza;

            // Itero per tutte le passate in Z
            while (zCurrent > zFinale)
            {

                zCurrent -= profPassata;

                if (zCurrent < zFinale)
                    zCurrent = zFinale;
                {
                    OneDirectionHorizontal(programPhase, zCurrent, zSicurezza, xIni, xFin, yIni, yFin, larghPassata);
                }

            }
            /*
             * Pensare a come riutilizzarla per direzione orizzontale e verticale
             */
        }

        private static void OneDirectionHorizontal(MoveActionCollection programPhase, double currentZ, double returnZ, double xIni, double xFin, double yIni, double yFin, double larghezzaPassata)
        {
            var currentY = yIni;

            while (currentY > yFin)
            {
                currentY -= larghezzaPassata;

                if (currentY < yFin)
                    currentY = yFin;


                // Mi sposto sopra punto attaco 
                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIni, currentY, null);

                // Vado a Z sicurezza
                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, returnZ);


                // Vado a z di lavoro
                programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, currentZ);

                // Movimento di lavoro
                programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFin, null, null);

                // Stacco in rapido
                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, returnZ);

            }

        }

        /// <summary>
        /// Calcola percorso di spianatura a spirale
        /// </summary>
        /// <param name="programPhase"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="larghezza"></param>
        /// <param name="altezza"></param>
        /// <param name="larghPassata"></param>
        /// <param name="diaFresaEffettivo">Diametro Effettivo Fresa</param>
        /// <param name="diaFresaIngombro">Diametro Ingombro Fressa</param>
        /// <param name="sovraMetallo"></param>
        /// <param name="profPassata"></param>
        /// <param name="zFinale"></param>
        /// <param name="zSicurezza">Z Sicurezza</param>
        /// <param name="extraCorsaSicurezza"></param>
        /// <param name="ottimizzaPassate">Con true calcolo nuova larghezza di passata minima</param>
        /// <param name="passataMinimaPercentuale">Passata Minima Percentuale</param>
        private static void SpiralFaceMillingV2(MoveActionCollection programPhase,
                                            double offsetX, double offsetY,
                                            double larghezza, double altezza,
                                            double larghPassata, double diaFresaEffettivo,
                                            double diaFresaIngombro,
                                            double sovraMetallo, double profPassata,
                                            double zFinale, double zSicurezza,
                                            double extraCorsaSicurezza, bool ottimizzaPassate, double passataMinimaPercentuale)
        {
            /*
             * todo : considerare anche raggio inserto--
             */
            /*
             * é meglio se avanzamenti li registro qui..
             */


            if (CheckValueHelper.GreatherOrEqualZero(new[] { sovraMetallo, extraCorsaSicurezza }) ||
                CheckValueHelper.GreatherThanZero(new[] { larghezza, altezza, larghPassata, diaFresaEffettivo, profPassata, diaFresaIngombro }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(zSicurezza, zFinale + sovraMetallo),
                                                  new KeyValuePair<double, double>(diaFresaEffettivo, larghPassata),
                                              })
                ) return;


            /*
             * Per creare spianatura divido l'area di lavoro in griglia a quadrati con lato == larghezzaPassata
             */

            var larghezzaPassataX = larghPassata;

            var larghezzaPassataY = larghPassata;

            var nColonneTeoriche = larghezza / larghPassata;

            var nRigheTeoriche = altezza / larghPassata;

            var nColonne = (int)Math.Floor(nColonneTeoriche);

            var nRighe = (int)Math.Floor(nRigheTeoriche);

            /*
             * devo per forza cambiare larghPassata, mantenendo il valore compreso fra un min e max.
             * 
             * una volta che ho questo valore , proseguo per il mio ciclo come solito..
             * 
             */
            if (ottimizzaPassate)
            {
                var passataMin = (larghPassata * passataMinimaPercentuale) / 100;

                var restoX = larghezza % larghezzaPassataX;
                var restoY = altezza % larghezzaPassataY;

                if (restoX < passataMin)
                {
                    // modifico larghezza passata X
                    larghezzaPassataX = larghezza / nColonne;

                    nColonne--;

                }

                if (restoY < passataMin)
                {
                    // modifico larghezza passata Y
                    larghezzaPassataY = altezza / nRighe;

                    // dimuisco numero di righe, 
                    nRighe--;

                }
            }


            /*
             * Raggio della fresa, sara il raggio che usero durante gli angoli nella spianatura
             */

            var raggioEffettivoFresa = diaFresaEffettivo / 2;

            /*
             * todo: gestire anche diversi direzioni lavorazione e quindi punti entrata
             */

            var sx = offsetX - larghezza / 2;
            var dx = offsetX + larghezza / 2;
            var down = offsetY - altezza / 2;
            var up = offsetY + altezza / 2;

            /*
             * in pratica per trovare punto arrivo moltiplico la larghezza di Passata per le celle delle griglia rimaste..
             */

            var zCurrent = zFinale + sovraMetallo;

            // Itero per tutte le passate in Z
            while (zCurrent > zFinale)
            {
                zCurrent -= profPassata;
                if (zCurrent < zFinale)
                    zCurrent = zFinale;



                // rimuovere
                var minElement = (int)Math.Min(nColonne, nRighe);


                /*
                 *  1.1 ) Punto attacco XY 
                 * 
                 *  Il punto di inizio in alto a dx , tiene presente dell'ingombro massimo fresa e diametro effettivo di lavorazione + extraCorsaSicurezza
                 */
                var raggioIngombro = diaFresaIngombro / 2;

                var puntoInizio = new Point2D
                {
                    X = sx + (nColonne) * larghezzaPassataX,
                    Y = up + raggioIngombro + extraCorsaSicurezza
                };

                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, puntoInizio.X, puntoInizio.Y, null);

                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


                //  1.2 ) Andare a z Corrent 

                programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);

                // 1.3 ) Punto avvicinamento diametro effettivo fresa 

                var puntoAvvi = new Point2D
                {
                    X = puntoInizio.X,
                    Y = up + raggioEffettivoFresa
                };

                programPhase.AddLinearMove(MoveType.PlungeFeed, AxisAbilited.Xy, puntoAvvi.X, puntoAvvi.Y, null);

                // 1.4) Interpolazione per fare entrata 

                var puntoFinaleArcoEntrata = new Point2D
                {
                    X = sx + (nColonne * larghezzaPassataX) + raggioEffettivoFresa,
                    Y = up,
                };

                programPhase.AddArcMove(AxisAbilited.Xy, puntoFinaleArcoEntrata.X, puntoFinaleArcoEntrata.Y, null, raggioEffettivoFresa, true, new Point2D(puntoAvvi.X, up));


                /*
                 *  Ora magari vorrei cercare di cambiare ciclo in qualcosa di più chiaro e gestibile..
                 *  
                 * qui a posso usare metodi aux..
                 * 
                 * poso
                 */

                // 1.5) Lavorazione fino a termine materiale

                var continueCycle = true;

                var moveEnum = SpiralMoveEnum.ToDown;

                var startMove = moveEnum;

                var tempX = puntoFinaleArcoEntrata.X;

                var tempY = puntoFinaleArcoEntrata.Y;

                // cambiare se cambia senso direzione
                // counterCelleY++;

                var tempConterColonne = nColonne;
                var tempCounterRighe = nRighe;



                while (continueCycle)
                {
                    continueCycle = SpiralMovementV2(programPhase, ref moveEnum, ref tempX, ref tempY, up, down, sx, dx, larghezzaPassataX, larghezzaPassataY, ref tempConterColonne, ref tempCounterRighe, nRighe, nColonne, minElement, raggioEffettivoFresa);

                    // anche questo va cambiato se si decide di fare scegliere punto attacco
                    //if (moveEnum == SpiralMoveEnum.ToDown)
                    //{
                    //    tempConterColonne--;
                    //    tempCounterRighe--;
                    //}
                }

                // 1.6) Porto fresa a Z Sicurezza
                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
            }
        }


        private static bool SpiralMovementV2(MoveActionCollection programPhase, ref  SpiralMoveEnum moveEnum, ref double tempX, ref double tempY, double up, double down, double sx, double dx, double larghPassataX, double larghezzaPassataY, ref int counterX, ref int counterY, int nRighe, int nColonne, int minCounter, double raggioFresa)
        {
            switch (moveEnum)
            {
                /*
                 *  teoria v2: 
                 * 
                 *  dopo che ho fatto la divisione in celle ( come solito )
                 *  ho lo scarto nella parte dx e down , 
                 *  
                 *  considero di partire da alto a dx 
                 *  mi serve convenzione..
                 *  
                 * per convenzione punto finale è punto "teorico" ovvero punto arrivo se non ci fosse raggio.
                 * 
                 * in modo che òa coordinata che non cambia si apposto per il prossimo movimento
                 */

                case SpiralMoveEnum.ToDown:
                    {
                        var nextY = up - (counterY * larghezzaPassataY) - raggioFresa;

                        var delta = tempY - nextY;


                        var yFinale = tempY;

                        var xFinale = tempX;


                        if (Math.Abs(delta) < raggioFresa * 2)
                        {
                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale, yFinale - raggioFresa, null);

                            return false;
                        }

                        yFinale = tempY = nextY;

                        var startArc = new Point2D { X = xFinale, Y = yFinale + raggioFresa };

                        var endArc = new Point2D { X = xFinale - raggioFresa, Y = yFinale };

                        var center = new Point2D { X = endArc.X, Y = startArc.Y };

                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, startArc.X, startArc.Y, null);

                        programPhase.AddArcMove(AxisAbilited.Xy, endArc.X, endArc.Y, null, raggioFresa, true, center);

                        moveEnum = SpiralMoveEnum.ToLeft;

                        counterX--;


                    } break;

                case SpiralMoveEnum.ToLeft:
                    {

                        var passi = nColonne - counterX;

                        var nextX = sx + (passi * larghPassataX) - raggioFresa;

                        var delta = tempX - nextX;

                        var yFinale = tempY;

                        var xFinale = tempX;

                        if (Math.Abs(delta) < raggioFresa * 2)
                        {
                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale - raggioFresa, yFinale, null);

                            return false;
                        }

                        xFinale = tempX = nextX;


                        var startArc = new Point2D { X = xFinale + raggioFresa, Y = yFinale };

                        var endArc = new Point2D { X = xFinale, Y = yFinale + raggioFresa };

                        var center = new Point2D { X = startArc.X, Y = endArc.Y };

                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, startArc.X, startArc.Y, null);

                        programPhase.AddArcMove(AxisAbilited.Xy, endArc.X, endArc.Y, null, raggioFresa, true, center);

                        moveEnum = SpiralMoveEnum.ToUp;

                        counterY--;

                    } break;


                case SpiralMoveEnum.ToUp:
                    {
                        var passi = nRighe - counterY;

                        var nextY = up - (passi * larghezzaPassataY) + raggioFresa;

                        var delta = tempY - nextY;

                        var yFinale = tempY;

                        var xFinale = tempX;

                        if (Math.Abs(delta) < raggioFresa * 2)
                        {
                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale, yFinale + raggioFresa, null);

                            return false;
                        }
                        yFinale = tempY = nextY;



                        var startArc = new Point2D { X = xFinale, Y = yFinale - raggioFresa };

                        var endArc = new Point2D { X = xFinale + raggioFresa, Y = yFinale };

                        var center = new Point2D { X = endArc.X, Y = startArc.Y };

                        /*
                         * devo controllare che la distanza che devo coprire sia maggiore del raggio fresa
                         */

                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, startArc.X, startArc.Y, null);

                        programPhase.AddArcMove(AxisAbilited.Xy, endArc.X, endArc.Y, null, raggioFresa, true, center);


                        moveEnum = SpiralMoveEnum.ToRight;

                        //  counterX--;


                    } break;

                case SpiralMoveEnum.ToRight:
                    {
                        var nextX = sx + (counterX * larghPassataX) + raggioFresa;

                        var delta = tempX - nextX;

                        var xFinale = tempX;

                        var yFinale = tempY;

                        if (Math.Abs(delta) < raggioFresa * 2)
                        {
                            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xFinale + raggioFresa, yFinale, null);

                            return false;
                        }

                        xFinale = tempX = nextX;


                        var startArc = new Point2D { X = xFinale - raggioFresa, Y = yFinale };

                        var endArc = new Point2D { X = xFinale, Y = yFinale - raggioFresa };

                        var center = new Point2D { X = startArc.X, Y = endArc.Y };

                        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, startArc.X, startArc.Y, null);

                        programPhase.AddArcMove(AxisAbilited.Xy, endArc.X, endArc.Y, null, raggioFresa, true, center);


                        moveEnum = SpiralMoveEnum.ToDown;

                    } break;

                default:
                    throw new NotImplementedException();

            }

            /*
             * in pratica deve restituirmi linea dritta e movimento angolare
             */

            return true;
        }


        private enum SpiralMoveEnum
        {
            ToUp,
            ToDown,
            ToLeft,
            ToRight
        }

    }

    //public enum SpianaturaEnumOperazioni : byte
    //{
    //    Sgrossatura,
    //    Finitura,
    //}

    public enum SpiantaturaMetodologia
    {
        Tradizionale,
        Spirale,
    }


}

// 30/06/2011 - prima della passata minima..
///// <summary>
///// Calcola percorso di spianatura a spirale
///// </summary>
///// <param name="programPhase"></param>
///// <param name="offsetX"></param>
///// <param name="offsetY"></param>
///// <param name="larghezza"></param>
///// <param name="altezza"></param>
///// <param name="larghPassata"></param>
///// <param name="diaFresaEffettivo">Diametro Effettivo Fresa</param>
///// <param name="diaFresaIngombro">Diametro Ingombro Fressa</param>
///// <param name="sovraMetallo"></param>
///// <param name="profPassata"></param>
///// <param name="zFinale"></param>
///// <param name="zSicurezza">Z Sicurezza</param>
///// <param name="rapidSecureFeed">Avanzamento Rapido Sicurezza</param>
///// <param name="workFeed">Avanzamento di lavoro</param>
///// <param name="extraCorsaSicurezza"></param>
///// <param name="optimizePassate">Con true calcolo nuova larghezza di passata minima</param>
///// <param name="passataMinimaPercentuale">Passata Minima Percentuale</param>
//private static void SpiralFaceMillingV2(ProgramPhase programPhase,
//                                    double offsetX, double offsetY,
//                                    double larghezza, double altezza,
//                                    double larghPassata, double diaFresaEffettivo,
//                                    double diaFresaIngombro,
//                                    double sovraMetallo, double profPassata,
//                                    double zFinale, double zSicurezza,
//                                    double rapidSecureFeed, double workFeed,
//                                    double extraCorsaSicurezza, bool optimizePassate, double passataMinimaPercentuale)
//{
//    /*
//     * todo : considerare anche raggio inserto--
//     */
//    /*
//     * é meglio se avanzamenti li registro qui..
//     */


//    if (CheckValueHelper.GreatherOrEqualZero(new[] { sovraMetallo, extraCorsaSicurezza }) ||
//        CheckValueHelper.GreatherThanZero(new[] { larghezza, altezza, larghPassata, diaFresaEffettivo, profPassata, workFeed, rapidSecureFeed, diaFresaIngombro }) ||
//        CheckValueHelper.GreatherThan(new[]
//                                      {
//                                          new KeyValuePair<double, double>(zSicurezza, zFinale + sovraMetallo),
//                                          new KeyValuePair<double, double>(diaFresaEffettivo, larghPassata),
//                                      })
//        ) return;


//    var avvicinamentoFeed = workFeed / 2;

//    var feedDictionary = new Dictionary<MoveType, double>
//                             {
//                                 {MoveType.PlungeFeed, avvicinamentoFeed},
//                                 {MoveType.SecureRapidFeed, rapidSecureFeed},
//                                 {MoveType.Work, workFeed}
//                             };

//    programPhase.SetFeedDictionary(feedDictionary);

//    /*
//     * Per creare spianatura divido l'area di lavoro in griglia a quadrati con lato == larghezzaPassata
//     */

//    var nColonneTeoriche = larghezza / larghPassata;
//    var nRigheTeoriche = altezza / larghPassata;

//    /*
//     * todo: gestire quo eventuale passata minima
//     * 
//     * ovvero aumento la larghezza di passata
//     * 
//     * voglio avere una larghezza di passata in modo da ottimizare il taglio
//     */

//    /* 
//     * Se le colonne sono n le righe sono n-1
//     */
//    var nColonne = (int)Math.Floor(nColonneTeoriche);

//    var nRighe = (int)Math.Floor(nRigheTeoriche);

//    /*
//     * Raggio della fresa, sara il raggio che usero durante gli angoli nella spianatura
//     */

//    var raggioEffettivoFresa = diaFresaEffettivo / 2;

//    /*
//     * todo: gestire anche diversi direzioni lavorazione e quindi punti entrata
//     */

//    var sx = offsetX - larghezza / 2;
//    var dx = offsetX + larghezza / 2;
//    var down = offsetY - altezza / 2;
//    var up = offsetY + altezza / 2;

//    /*
//     * in pratica per trovare punto arrivo moltiplico la larghezza di Passata per le celle delle griglia rimaste..
//     */

//    var zCurrent = zFinale + sovraMetallo;

//    // Itero per tutte le passate in Z
//    while (zCurrent > zFinale)
//    {
//        zCurrent -= profPassata;
//        if (zCurrent < zFinale)
//            zCurrent = zFinale;


//        var minElement = (int)Math.Min(nColonne, nRighe);


//        /*
//         *  1.1 ) Punto attacco XY 
//         * 
//         *  Il punto di inizio in alto a dx , tiene presente dell'ingombro massimo fresa e diametro effettivo di lavorazione + extraCorsaSicurezza
//         */
//        var raggioIngombro = diaFresaIngombro / 2;

//        var puntoInizio = new Point2D
//                              {
//                                  X = sx + (nColonne) * larghPassata,
//                                  Y = up + raggioIngombro + extraCorsaSicurezza
//                              };

//        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, puntoInizio.X, puntoInizio.Y, null);

//        //  1.2 ) Andare a z Corrent 

//        programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);

//        // 1.3 ) Punto avvicinamento diametro effettivo fresa 

//        var puntoAvvi = new Point2D
//        {
//            X = puntoInizio.X,
//            Y = up + raggioEffettivoFresa
//        };

//        programPhase.AddLinearMove(MoveType.PlungeFeed, AxisAbilited.Xy, puntoAvvi.X, puntoAvvi.Y, null);

//        // 1.4) Interpolazione per fare entrata 

//        var puntoFinaleArcoEntrata = new Point2D
//                                         {
//                                             X = sx + (nColonne * larghPassata) + raggioEffettivoFresa,
//                                             Y = up,
//                                         };

//        programPhase.AddArcMove(AxisAbilited.Xy, puntoFinaleArcoEntrata.X, puntoFinaleArcoEntrata.Y, null, raggioEffettivoFresa, false, new Point2D(puntoAvvi.X, up));


//        /*
//         *  Ora magari vorrei cercare di cambiare ciclo in qualcosa di più chiaro e gestibile..
//         *  
//         * qui a posso usare metodi aux..
//         * 
//         * poso
//         */

//        // 1.5) Lavorazione fino a termine materiale

//        var continueCycle = true;

//        var moveEnum = SpiralMoveEnum.ToDown;

//        var startMove = moveEnum;

//        var tempX = puntoFinaleArcoEntrata.X;

//        var tempY = puntoFinaleArcoEntrata.Y;

//        // cambiare se cambia senso direzione
//        // counterCelleY++;

//        var tempConterColonne = nColonne;
//        var tempCounterRighe = nRighe;



//        while (continueCycle)
//        {
//            continueCycle = SpiralMovementV2(programPhase, ref moveEnum, ref tempX, ref tempY, up, down, sx, dx, larghPassata, tempConterColonne, tempCounterRighe, nRighe, nColonne, minElement, raggioEffettivoFresa);

//            // anche questo va cambiato se si decide di fare scegliere punto attacco
//            if (moveEnum == SpiralMoveEnum.ToDown)
//            {
//                tempConterColonne--;
//                tempCounterRighe--;
//            }
//        }

//        // 1.6) Porto fresa a Z Sicurezza
//        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//    }
//}
