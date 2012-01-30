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
    public class LatheAxisC : HorizontalLathe2Axis
    {
        public override FaseDiLavoro CreateFaseLavoro()
        {
            var fase = new FaseTornio3Assi() { MachineGuid = this.MachineGuid };



            return fase;
        }

        public LatheAxisC()
        {
            CurrentWorkPlane = WorkPlane.XZ;

            NumericControl = new MachineControl.NumericControl();
        }


        protected override void WriteMoveToX(double p, ref string code, bool forceWrite = false)
        {
            if (G112Activated)
            {
                if (!forceWrite)
                    if (CurrentX.HasValue && p == CurrentX.Value) return;

                // Quota diametrale
                //p *= 2;
                code += ("X" + FormatCoordinate(p * 2));

                CurrentX = p;
            }
            else
            {
                base.WriteMoveToX(p, ref  code);
            }

        }


        protected override void CreateCodeFromAction(ChangeToolAction programAction, ref StringBuilder code)
        {
            // se è utensile rotante creo codice abilitando i parametri per attivazione asse c
            if (programAction.IsRotaryTool)
            {
                // fermo mandrino
                code.AppendLine("M5");

                // controllo refrigerante
                code.AppendLine("M8");

                // Stacco freno asse C
                code.AppendLine("M69");

                // Setto Feed mm/min e attivo asseC
                code.AppendLine("G98M45");

                // Azzerro asse C
                code.AppendLine("G0G28H0");

                // Etichetta Utensile
                var toolLabel = programAction.ToolLabel;

                code.AppendLine(FormatComment(toolLabel));


                // Numero e correttore
                var toolNumber = programAction.NumberTool;

                var latheToolCorrector = programAction.LatheToolCorrector;

                code.AppendLine("G0" + NumericControl.CharToolCode +
                                toolNumber.ToString("00") +
                                latheToolCorrector.ToString("00"));

                // Parametri
                var speed = programAction.Speed;
                var spindleRotation = programAction.SpindleRotation;

                // Attivo Parametri 
                code.Append("G97" + "S" + FormatSpeed(speed) + "M13");

                /*
                 * G0M5
                    M8
                    M69
                    G98M45
                    G28H0
                    G0T1111
                    G97S200M13
                 */

                Avvicinamento(ref code, programAction.SecureZ);

            }
            else
            {
                // altrimenti setto i parametri per attivazione utensile fisso normale.
                base.CreateCodeFromAction(programAction, ref code);
            }
        }

        /// <summary>
        /// Crea codice per arco.
        /// Interpolazione Circolare 
        /// G2 G3 X/U  Z/W I K F
        /// Interpolazione Circolare con raggio specificato
        /// G2 G3 X/U Z/W R F
        /// 
        /// Di default prendo prima forma. Se in caso fosse necessario con raggio impostato creare flag
        /// dentro ArcMoveAction
        /// </summary>
        /// <param name="programAction"></param>
        /// <param name="code"></param>
        protected override void CreateCodeFromAction(ArcMoveAction programAction, ref StringBuilder code)
        {
            if (G112Activated)
            {
                var str = string.Empty;

                // centro arco devo prenderlo prima di scrivere coordinate xyz in quanto vado a modificare currentxyz

                var iCode = string.Empty;
                var jCode = string.Empty;

                // Devo mettere coordinate incrementali del centro del cerchio rispetto al punto precedente
                if (CurrentX.HasValue && CurrentY.HasValue)
                {
                    var deltaX = programAction.Center.X - CurrentX.Value;
                    var deltaY = programAction.Center.Y - CurrentY.Value;

                    iCode = ("I" + FormatCoordinate(deltaX));

                    jCode = ("J" + FormatCoordinate(deltaY));

                }

                SetMoveType(programAction.MoveType, ref str);

                if (programAction.X.HasValue)
                {
                    WriteMoveToX(programAction.X.Value, ref str);
                }

                if (programAction.Y.HasValue)
                {
                    WriteMoveToY(programAction.Y.Value, ref str);
                }

                if (programAction.Z.HasValue)
                {
                    WriteMoveToZ(programAction.Z.Value, ref str);
                }


                str += iCode;
                str += jCode;

                // il raggio lo ometto
                //str += "R" + FormatCoordinate(programAction.Radius);

                SetFeed(programAction.GetFeedValue(), ref str);

                if (string.IsNullOrWhiteSpace(str)) return;

                code.AppendLine(str);

            }
            else
            {
                base.CreateCodeFromAction(programAction, ref code);
            }
        }

        private void MoveToAngularCoordinate(double x, double y, double z, ref StringBuilder code)
        {
            /*
             * per ora stampo coordinata xy, poi provvedo a inserire coordinata angolare.
             */

            var cooX = x;
            var cooY = y;

            code.AppendLine("X" + FormatCoordinate(x) + "Y" + FormatCoordinate(y));

        }

        /// <summary>
        /// Rappresenta coordinata angolare. usata nelle macro.. per dare angolo e diametro.
        /// </summary>
        private class AngularCoordinate
        {

        }



        private void WriteAttivazioneAsseC(ref StringBuilder code)
        {
            code.AppendLine(NumericControl.CMDM_SpindleStop);
            code.AppendLine(NumericControl.CMDM_Lathe3Ax_On);
            code.AppendLine("G0G28H0");
        }

        private void SetToolPlane(WorkPlane workPlane, ref StringBuilder code)
        {
            CurrentWorkPlane = workPlane;

            switch (CurrentWorkPlane)
            {
                case WorkPlane.LatheXz:
                    {

                    } break;

                default:
                    throw new NotImplementedException();
            }

            code.AppendLine(NumericControl.CMDM_SpindleStop);
            code.AppendLine(NumericControl.CMDM_Lathe3Ax_On);
            code.AppendLine("G0G28H0");
        }

        private bool G112Activated { get; set; }
        protected override void CreateCodeFromAction(ActiveG112 activeG112, ref StringBuilder code)
        {
            G112Activated = activeG112.Activated;

            code.Append(activeG112.Activated ? "\nG112\n" : "\nG113\n");
        }

        protected override string GetYCode()
        {
            if (G112Activated)
                return "C";

            return "Y";
        }
        //protected override void CreateCodeFromAction(ArcMoveAction programAction, ref StringBuilder code)
        //{

        //    code.AppendLine();

        //    SetMoveType(programAction.MoveType, ref code);

        //    var codeX = "X";
        //    var codeY = "Y";

        //    if (G112Activated)
        //    {
        //        codeY = "C";
        //    }

        //    if (programAction.X.HasValue)
        //        code.Append(codeX + FormatCoordinate(programAction.X.Value));

        //    if (programAction.Y.HasValue)
        //        code.Append(codeY + FormatCoordinate(programAction.Y.Value));

        //    if (programAction.Z.HasValue)
        //        code.Append("Z" + FormatCoordinate(programAction.Z.Value));

        //    code.Append("R" + FormatCoordinate(programAction.Radius));

        //    if (programAction.Feed != CurrentFeed)
        //        SetFeed(programAction.Feed, ref code);


        //}

        //protected override void CreateCodeFromAction(LinearMoveAction programAction, ref StringBuilder code)
        //{

        //    code.AppendLine();

        //    SetMoveType(programAction.MoveType, ref code);




        //    var codeX = "X";
        //    var codeY = "Y";

        //    if (G112Activated)
        //    {
        //        codeY = "C";
        //    }

        //    if (programAction.X.HasValue)
        //        code.Append(codeX + FormatCoordinate(programAction.X.Value));

        //    if (programAction.Y.HasValue)
        //        code.Append(codeY + FormatCoordinate(programAction.Y.Value));

        //    if (programAction.Z.HasValue)
        //        code.Append("Z" + FormatCoordinate(programAction.Z.Value));


        //    if (programAction.Feed != CurrentFeed)
        //        SetFeed(programAction.Feed, ref code);


        //}



        private void WriteToolParameter(double speed, SpindleRotation spindleRotation, ref StringBuilder code)
        {
            //var speedCode = PostProcessor.GetSpeedCode(speed);

            var speedFormated = FormatSpeed(speed);

            var speedCode = NumericControl.GetSpeedCode(speedFormated);


            string spindleRotationCode;

            switch (spindleRotation)
            {
                case SpindleRotation.Cw:
                    {
                        spindleRotationCode = NumericControl.CmdRotaryToolCw;

                    } break;


                case SpindleRotation.Ccw:
                default:
                    {
                        spindleRotationCode = NumericControl.CmdRotaryToolCcw;
                    } break;
            }

            code.AppendLine(speedCode + spindleRotationCode);
        }


        protected override void CreateCodeFromAction(MacroDrillingAction macro, ref StringBuilder code)
        {
            //if (drillingAction.DrillPoints == null)
            //    throw new NullReferenceException();

            //var pnts = drillingAction.DrillPoints.ToList();

            //if (pnts.Count() == 0)
            //    return;

            //var firstPnt = pnts.First();

            //switch (drillingAction.DrillMacroTypeEnum)
            //{
            //    default:
            //    case ModalitaForatura.Semplice:
            //        {
            //            WriteSimpleDrilling(ref code, firstPnt, drillingAction.Dweel);
            //        } break;
            //}

            //// il primo è già stampato.. 
            //for (var i = 1; i < pnts.Count(); i++)
            //{
            //    var pnt = pnts[i];

            //    MoveToAngularCoordinate(pnt.X, pnt.Y, 0, ref code);
            //}

            //code.AppendLine("G80");
            ///*
            // * i punti nelle macro di foratura vanno espressini in angolo & diametro
            // */
        }

        void WriteSimpleDrilling(ref StringBuilder code, Point2D firstPnt, double dwell)
        {
            var macroCode = NumericControl.CMD_Cycle_Simple_Drilling;

            code.AppendLine(macroCode + "P" + dwell);
        }

        internal List<IEntity2D> CreatePreview(MacroDrillingAction drillingAction)
        {
            //var rslt = new List<IEntity2D>();

            //if (drillingAction.DrillPoints == null)
            //    throw new NullReferenceException();

            //var pntList = drillingAction.DrillPoints.ToList();

            //if (pntList.Count <= 0)
            //    return null;

            //var initPnt = new Point3D();

            //initPnt.X = 100;
            //initPnt.Y = 100;
            //initPnt.Z = 100;


            //var prevPnt = initPnt;

            //foreach (var point2D in pntList)
            //{
            //    var attacLine = new Line3D();

            //    attacLine.Start = new Point3D(prevPnt);

            //    var secureZ = drillingAction.SecureZ;

            //    attacLine.End = new Point3D(point2D.X, point2D.Y, secureZ);

            //    rslt.Add(attacLine);

            //    var drillLine = new Line3D();

            //    drillLine.Start = new Point3D(attacLine.End);

            //    drillLine.End = new Point3D(attacLine.End);

            //    var endZ = drillingAction.EndZ;

            //    drillLine.End.Z = endZ;

            //    rslt.Add(drillLine);

            //    var retractLine = new Line3D();

            //    retractLine.Start = new Point3D(drillLine.End);

            //    retractLine.End = new Point3D(drillLine.Start);

            //    rslt.Add(retractLine);
            //}

            //return rslt;
            return null;
        }



        //internal void SetZeroPoint(Point3D point3D)
        //{
        //    CurrentX = point3D.X;
        //    CurrentZ = point3D.Z;
        //    CurrentY = point3D.Y;
        //}
    }
}
