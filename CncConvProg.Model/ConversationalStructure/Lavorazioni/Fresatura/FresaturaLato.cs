using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
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
 * 21/09/2011
 * 
 * todo : 
 * Devo prendere come punto di riferimento il punto centrale della linea , oppure meglio fare come ho fatto per 
 * spianatura , in modo da fare una scelta migliore.
 * 
 * Cosi come è ora è difficile scegliere punto
 */
    [Serializable]
    public sealed class FresaturaLato : LavorazioneFresatura, IMillLeveable, IMillWorkable
    {
        public FresaturaLato(Guid parent)
            : base(parent)
        {
            var fract = 1.0d;

            //if (parent.Model.MeasureUnit == MeasureUnit.Inch)
            //    fract = 25.4;

            //Sovrametallo = Math.Round(20 / fract, 3);

            //ProfonditaLavorazione = Math.Round(20 / fract, 3);

            //Lunghezza = Math.Round(80 / fract, 3);

            //SicurezzaZ = Math.Round(50 / fract, 3);

            //InizioLavorazioneZ = 0;

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
                    opList.Add(Sgrossatura);

                //if (Finitura.Abilitata)
                    opList.Add(Finitura);

                //if (Smussatura.Abilitata)
                    opList.Add(Smussatura);

                return opList;

            }
        }

        public Operazione Sgrossatura { get; set; }

        public Operazione Smussatura { get; set; }

        public Operazione Finitura { get; set; }

        public double ProfonditaFresaSmussatura { get; set; }

        public bool FinishWithCompensation { get; set; }

        public double SovrametalloFinituraProfilo { get; set; }

        public double InizioLavorazioneZ { get; set; }

        public double ProfonditaLavorazione { get; set; }

        public double PuntoInizialeX { get; set; }

        public double PuntoInizialeY { get; set; }

        public double Lunghezza { get; set; }

        public double Sovrametallo { get; set; }

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
            get { return "Side" + Lunghezza + " x " + Sovrametallo; }
        }

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
                    Y = PuntoInizialeY,
                    Z = InizioLavorazioneZ,
                },

                End = new Point3D
                {
                    X = PuntoInizialeX + Lunghezza,
                    Y = PuntoInizialeY,
                    Z = InizioLavorazioneZ,
                }
            };

            var l2 = new Line3D
                         {
                             Start = new Point3D(l1.Start),
                             End = new Point3D(l1.End),
                         };

            l2.Start.Z = l2.End.Z = InizioLavorazioneZ - ProfonditaLavorazione;

            var l3 = new Line3D
            {
                Start = new Point3D
                {
                    X = PuntoInizialeX,
                    Y = PuntoInizialeY + Sovrametallo,
                    Z = InizioLavorazioneZ - ProfonditaLavorazione,
                },

                End = new Point3D
                {
                    X = PuntoInizialeX + Lunghezza,
                    Y = PuntoInizialeY + Sovrametallo,
                    Z = InizioLavorazioneZ - ProfonditaLavorazione,
                }
            };

            var l11 = new Line3D { Start = l1.Start, End = l2.Start };

            var l12 = new Line3D { Start = l11.End, End = l3.Start };

            var l21 = new Line3D { Start = l1.End, End = l2.End };

            var l22 = new Line3D { Start = l21.End, End = l3.End };


            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l1, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l2, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l3, rotationMatrix));

            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l11, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l12, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l21, rotationMatrix));
            rslt.Add(Geometry.GeometryHelper.MultiplyLine(l22, rotationMatrix));

            foreach (var entity2D in rslt)
            {
                entity2D.PlotStyle = EnumPlotStyle.Element;
            }

            return rslt;
        }

        protected override void CreateSpecificProgram(ProgramPhase programPhase, Operazione operazione)
        {
            var dia = operazione.Utensile as IDiametrable;

            if (dia == null) return;

            var diaTool = dia.Diametro;

            var moveCollection = new MoveActionCollection();

            switch (operazione.OperationType)
            {
                case LavorazioniEnumOperazioni.Sgrossatura:
                    {

                        var par = operazione.GetParametro<ParametroFresaCandela>();

                        if (par == null)
                            throw new NullReferenceException();

                        var profPassat = par.GetProfonditaPassata();

                        var larghPassat = par.GetLarghezzaPassata();

                        ProcessRoughLineMilling(moveCollection, PuntoInizialeX, PuntoInizialeY, OrientationAngle, Lunghezza, Sovrametallo, ProfonditaLavorazione,
                            profPassat, larghPassat, par.DiametroFresa, SicurezzaZ, ExtraCorsa, InizioLavorazioneZ);

                    } break;

                case LavorazioniEnumOperazioni.Finitura:
                    {

                        var par = operazione.GetParametro<ParametroFresaCandela>();

                        if (par == null)
                            throw new NullReferenceException();

                        var profPassat = par.GetProfonditaPassata();

                        var larghPassat = par.GetLarghezzaPassata();

                        /*
                         * Per finitura assumo sovrametallo minore della larghezza passata
                         */

                        var compensationType = 0;

                        if (FinishWithCompensation)
                            compensationType = 2;

                        ProcessRoughLineMilling(moveCollection, PuntoInizialeX, PuntoInizialeY, OrientationAngle, Lunghezza, larghPassat, ProfonditaLavorazione,
                            profPassat, larghPassat, par.DiametroFresa, SicurezzaZ, ExtraCorsa, InizioLavorazioneZ, compensationType);

                    } break;

                case LavorazioniEnumOperazioni.Smussatura:
                    {
                        var profPassat = ProfonditaFresaSmussatura;

                        var larghPassat = diaTool / 2;

                        /*
                         * Per finitura assumo sovrametallo minore della larghezza passata
                         */

                        ProcessRoughLineMilling(moveCollection, PuntoInizialeX, PuntoInizialeY, OrientationAngle, Lunghezza, larghPassat, ProfonditaFresaSmussatura,
                            profPassat, larghPassat, diaTool, SicurezzaZ, ExtraCorsa, InizioLavorazioneZ);

                        //ProcessChamferSideLineMilling(moveCollection, PuntoInizialeX, PuntoInizialeY, OrientationAngle, Lunghezza, Sovrametallo, ProfonditaFresaSmussatura, diaTool, SicurezzaZ, ExtraCorsa, InizioLavorazioneZ);

                    } break;

                default:
                    throw new NotImplementedException();
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




        //    if (parametro == null)
        //        throw new NullReferenceException();


        //    var moveCollection = new MoveActionCollection();

        //    switch ((LavorazioniEnumOperazioni)operazione.OperationType)
        //    {
        //        case LavorazioniEnumOperazioni.Sgrossatura:
        //            {

        //                var par = operazione.GetParametro<ParametroFresaCandela>();

        //                if (par == null)
        //                    throw new NullReferenceException();

        //                var profPassat = par.GetProfonditaPassata();

        //                var larghPassat = par.GetLarghezzaPassata();


        //                ProcessRoughLineMilling(moveCollection, PuntoInizialeX, PuntoInizialeY, OrientationAngle, Lunghezza, Sovrametallo, ProfonditaLavorazione,
        //                    profPassat, larghPassat, par.DiametroFresa, SicurezzaZ, extraCorsa, InizioLavorazioneZ);

        //            } break;

        //        case LavorazioniEnumOperazioni.Finitura:
        //            {

        //                var par = operazione.GetParametro<ParametroFresaCandela>();

        //                if (par == null)
        //                    throw new NullReferenceException();

        //                var profPassat = par.GetProfonditaPassata();

        //                var larghPassat = par.GetLarghezzaPassata();

        //                /*
        //                 * Per finitura assumo sovrametallo minore della larghezza passata
        //                 */

        //                ProcessRoughLineMilling(moveCollection, PuntoInizialeX, PuntoInizialeY, OrientationAngle, Lunghezza, larghPassat, ProfonditaLavorazione,
        //                    profPassat, larghPassat, par.DiametroFresa, SicurezzaZ, extraCorsa, InizioLavorazioneZ);

        //            } break;

        //        case LavorazioniEnumOperazioni.Smussatura:
        //            {

        //                var par = operazione.GetParametro<ParametroFresaCandela>();

        //                if (par == null)
        //                    throw new NullReferenceException();

        //                /*
        //                 * qui al posto della profondita lavorazione uso valore profondita fresa smussatura.
        //                 */

        //                ProcessChamferSideLineMilling(moveCollection, PuntoInizialeX, PuntoInizialeY, OrientationAngle, Lunghezza, Sovrametallo, ProfonditaFresaSmussatura, par.DiametroFresa, SicurezzaZ, extraCorsa, InizioLavorazioneZ);

        //            } break;

        //        default:
        //            throw new NotImplementedException();
        //    }




        //    var mm = base.GetFinalProgram(moveCollection);

        //    foreach (var variable in mm)
        //    {
        //        program.AddMoveAction(variable);
        //    }

        //    program.ActiveAsseC(false);

        //    return program;
        //}

        private static void ProcessRoughLineMilling(MoveActionCollection moveCollection, double puntoInizialeX, double puntoInizialeY, double orientationAngle,
                                                    double lunghezza, double sovrametallo, double profonditaLavorazione, double profPassat, double larghPassat,
                                                    double diaFresa, double sicurezzaZ, double extraCorsa, double inizioLavorazioneZ, int isCncCompensated = 0)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { profPassat, larghPassat, lunghezza, sovrametallo, profonditaLavorazione, diaFresa }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(sicurezzaZ,inizioLavorazioneZ),
                                              })
              ) return;

            var currentZ = inizioLavorazioneZ;

            var zFinale = currentZ - profonditaLavorazione;

            var raggioFresa = diaFresa / 2;

            var offset = raggioFresa;

            if (isCncCompensated != 0)
                offset = 0;

            var xIniPoint = puntoInizialeX - extraCorsa - raggioFresa;
            var xEndPoint = puntoInizialeX + extraCorsa + raggioFresa + lunghezza;

            var yIniPoint = puntoInizialeY + sovrametallo + offset;
            var yEndPoint = puntoInizialeY + offset;

            CncCompensationState cncCompensationState = CncCompensationState.NoChange;


            if (isCncCompensated == 1)
                cncCompensationState = CncCompensationState.G41;

            else if (isCncCompensated == 2)
                cncCompensationState = CncCompensationState.G42;

            var rotationMatrix = new Matrix3D();

            rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), orientationAngle), new System.Windows.Media.Media3D.Point3D(puntoInizialeX, puntoInizialeY, 0));

            while (currentZ > zFinale)
            {
                currentZ -= profPassat;

                if (currentZ < zFinale)
                    currentZ = zFinale;

                var currentY = yIniPoint;

                while (currentY > yEndPoint)
                {
                    currentY -= larghPassat;
                    if (currentY < yEndPoint)
                        currentY = yEndPoint;

                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, xIniPoint, currentY, null, rotationMatrix, null, cncCompensationState);

                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, sicurezzaZ);

                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, currentZ);

                    moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xEndPoint, currentY, null, rotationMatrix);

                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, sicurezzaZ);

                }

                if (cncCompensationState == CncCompensationState.G41 || cncCompensationState == CncCompensationState.G42)
                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, sicurezzaZ, null, CncCompensationState.G40);

            }
        }

    }

}