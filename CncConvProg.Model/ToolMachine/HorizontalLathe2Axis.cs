using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.FasiDiLavoro;
using CncConvProg.Model.PathGenerator;

namespace CncConvProg.Model.ToolMachine
{
    [Serializable]
    public class HorizontalLathe2Axis : ToolMachine
    {
        public HorizontalLathe2Axis()
        {
            CurrentWorkPlane = WorkPlane.XZ;
        }

        private void WriteToolParameter(ModalitaVelocita modalitaVelocita, double speed, SpindleRotation spindleRotation, ref StringBuilder code)
        {
            string speedTypeCode;

            switch (modalitaVelocita)
            {
                case ModalitaVelocita.VelocitaTaglio:
                    {
                        speedTypeCode = NumericControl.CMDG_SpeedSync;

                    } break;


                case ModalitaVelocita.GiriFissi:
                default:
                    {
                        speedTypeCode = NumericControl.CMDG_SpeedASync;
                    } break;
            }

            var formatedSpeed = FormatSpeed(speed);
            var speedCode = NumericControl.GetSpeedCode(formatedSpeed);

            string spindleRotationCode;

            switch (spindleRotation)
            {
                case SpindleRotation.Cw:
                    {
                        spindleRotationCode = NumericControl.CMDM_SpindleCW;

                    } break;


                case SpindleRotation.Ccw:
                default:
                    {
                        spindleRotationCode = NumericControl.CMDM_SpindleCCW;
                    } break;
            }


            code.AppendLine(speedTypeCode + speedCode + spindleRotationCode);
        }

        public override FaseDiLavoro CreateFaseLavoro()
        {
            var fase = new FaseTornio() {MachineGuid = this.MachineGuid};

            return fase;
        }

        protected override void Avvicinamento(ref StringBuilder code, double secureZ)
        {
            /*
             *  qui smistare in base all'avvicinamento scelto :
             *  
             *  - prima in z poi in x
             *  - interpolazione xz
             *  
             */

            var str = string.Empty;
            WriteMoveToZ(secureZ, ref str, true);
            code.Append("\n" + str );
        }

        protected override void DisimpegnoUtensile(ref StringBuilder code, bool stessoUtNextOp = false, double secureZ = 0)
        {
            code.AppendLine("G53 X0");
            code.AppendLine("G53 Z0");
        }

        protected override void CreateCodeFromAction(CambiaUtensileAction programAction, ref StringBuilder code)
        {
            // Etichetta Utensile
            var toolLabel = programAction.EtichettaUtensile;

            code.AppendLine(FormatComment(toolLabel));


            // Numero e correttore
            var toolNumber = programAction.NumeroUtensile;

            var latheToolCorrector = programAction.CorrettoreUtensileTornio;

            code.AppendLine(NumericControl.CharToolCode +
                            toolNumber.ToString("00") +
                            latheToolCorrector.ToString("00"));

            // Parametri
            var modalitaVelocita = programAction.ModalitaVelocita;
            var speed = programAction.Velocità;
            var spindleRotation = programAction.RotazioneMandrino;

            WriteToolParameter(modalitaVelocita, speed, spindleRotation, ref code);

        }

        protected override void CreateCodeFromAction(MacroLongitudinalTurningAction programAction, ref StringBuilder code)
        {
            /*
             * crea codice per macro sgrossatura esterna
             * - punto attacco , prende punto più alto profilo o altro se fatto override
             */
            var attacPnt = programAction.PuntoAttacco;

            code.AppendLine(NumericControl.CmdMoveRapid + "X" + attacPnt.Y + "Z" + attacPnt.X);

            code.AppendLine("G71U" + programAction.ProfonditaPassata + "R1");
            code.AppendLine("G71P100Q101");
            code.AppendLine("N100");

            var profile = programAction.Profile.Source;

            if (profile == null || profile.Count <= 0)
                return;

            var firstElement = profile.First();

            var firstPnt = firstElement.GetFirstPnt();

            code.AppendLine(NumericControl.CmdMoveRapid + "X" + firstPnt.Y + "Z" + firstPnt.X);

            foreach (var entity2D in programAction.Profile.Source)
            {
                if (entity2D is Line2D)
                {
                    var line = entity2D as Line2D;

                    MoveLineWork(line.End, ref code);
                }
                else if (entity2D is Arc2D)
                {
                    var arc = entity2D as Arc2D;

                    MoveArcWork(arc, ref code);
                }
            }
            code.AppendLine("N101");

            CurrentMoveType = MoveType.Rapid;

            code.AppendLine(NumericControl.CmdMoveRapid + "X" + attacPnt.Y + "Z" + attacPnt.X);

            code.AppendLine("G53X0Z0");

        }

        //public override void SetZeroPoint(Point3D point3D)
        //{
        //    throw new NotImplementedException();
        //}

        private void MoveArcWork(Arc2D arc, ref StringBuilder code)
        {
            var moveTypeCmd = string.Empty;

            var moveType = arc.ClockWise ? MoveType.Cw : MoveType.Ccw;

            if (CurrentMoveType != moveType)
            {
                CurrentMoveType = moveType;

                moveTypeCmd = arc.ClockWise ? NumericControl.CMD_MoveCW : NumericControl.CMD_MoveCCW;
            }

            code.AppendLine(moveTypeCmd + "X" + arc.End.Y + "Z" + arc.End.X);
        }

        private void MoveLineWork(Point2D point2D, ref StringBuilder code)
        {
            var moveTypeCmd = string.Empty;

            if (CurrentMoveType != MoveType.Work)
            {
                CurrentMoveType = MoveType.Work;

                moveTypeCmd = NumericControl.CmdMoveWork;
            }

            code.AppendLine(moveTypeCmd + "X" + point2D.Y + "Z" + point2D.X);
        }

        private void MoveRapid(Point2D point2D, ref StringBuilder code)
        {
            if (CurrentMoveType != MoveType.Rapid)
            {

            }
        }

        //protected override void WriteTool(Operazione operazione, ref StringBuilder code)
        //{
        //    var toolNumber = operazione.GetToolNumber();

        //    var latheToolCorrector = operazione.GetLatheToolCorrector();

        //    code.AppendLine(PostProcessor.CharToolCode +
        //                    toolNumber.ToString("00") +
        //                    latheToolCorrector.ToString("00"));

        //}

        //bool fladdd;

        //private void ppp1(Operazione operazione, Lavorazione lavorazione, ref  StringBuilder code)
        //{
        //    dynamic op = operazione;
        //    dynamic lav = lavorazione;

        //    if (fladdd)
        //    {
        //        fladdd = false;
        //        // questo dovrebbe interromper in caso non fosse stato implementato supercc

        //        ppp1(op, lav, ref code);
        //    }
        //    else
        //    {
        //        throw new Exception("Implementare horizontal2axis.");
        //    }
        //    // da qui dynamic 
        //    // posso 
        //}

        //void ppp1(ForaturaSemplice foraturaSemplice, OperazioneForatura operazioneForatura, ref StringBuilder code)
        //{
        //    /* todo : questa lavorazione non si dovrebbe neanche trovare qui..
        //     io implemento solo le lavorazioni che dovrebbero esserci , se non viene trovato niente , lancia errore.*/

        //    /*
        //     * todo : su tornio devo abilitare anche asse c 
        //     */

        //    var pntList = foraturaSemplice.GetDrillPointList();

        //    if (pntList == null || pntList.Count() == 0)
        //        return;

        //    var firstPnt = pntList.First();

        //    // todo , devo ottenere anche il tipo di ciclo che voglio effetture.

        //    switch ((OperazioniForatura)operazioneForatura.OperationType)
        //    {
        //        case OperazioniForatura.Alesatore:
        //        case OperazioniForatura.Punta:
        //        case OperazioniForatura.Lamatore:

        //            {
        //                WriteDrillMacro(foraturaSemplice.ModalitaForatura, pntList);
        //                //switch (foraturaSemplice.ModalitaForatura)
        //                //{
        //                //    case ModalitaForatura.Semplice:
        //                //    case ModalitaForatura.StepScaricoTruciolo:
        //                //    case ModalitaForatura.StepSenzaScaricoTruciolo:
        //                //        {
        //                //            /*
        //                //             * prendi parametri . step. 
        //                //             */


        //                //        } break;
        //                //}
        //            } break
        //                ;

        //        case OperazioniForatura.Centrino:
        //        case OperazioniForatura.Smusso:
        //            {
        //                WriteDrillMacro(ModalitaForatura.Semplice, pntList);

        //            }break;

        //        case OperazioniForatura.Maschio:
        //            {
        //                WriteDrillMacro(ModalitaForatura.Maschiatura, pntList);

        //            } break;


        //        default:
        //            throw new NotImplementedException("Hor2Axis.pp1");
        //    }
        //    // arrivo qui
        //    /*
        //     * simile a quello gia fatto in mec , speravo fare cosa un po più chiara.
        //     */
        //}

        //private void WriteDrillMacro(ModalitaForatura modalitaForatura, IEnumerable<Geometry.Entity2D.Point2D> pntList)
        //{
        //    throw new NotImplementedException();
        //}


        //protected override void WriteProgramBody(Operazione operazione, ref StringBuilder code)
        //{
        //    //    fladdd = false;

        //    //    var lav = operazione.Lavorazione;

        //    //    ppp1(operazione, lav, ref code);


        //    // questa parte serve per scrivere corpo programma.
        //    /*
        //     *  tipo se si tratta di filettatura devo mettere macro 
        //     *  se si tratta di sgrossatura altor
        //     *  
        //     *  se invece si tratta di spianatuta altro cosa
        //     *  qui forse è utile avere classe layer 
        //     *  perche non posso pr
        //     *  operazioen..
        //     */

        //}
        //internal override void WriteMoveToInitPoint(Operazione operazione, ref StringBuilder code)
        //{
        //    /* 
        //     * todo : nel tornio fare attacco con movimento prima in z e x , o assieme
        //     * per le lavorazioni di tornira posso usare il diametro del grezzo oppure mi tocca 
        //     */
        //}
    }
}