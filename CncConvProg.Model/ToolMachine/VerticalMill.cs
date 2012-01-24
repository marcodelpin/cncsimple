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
    public class VerticalMill : ToolMachine
    {
        public override FaseDiLavoro CreateFaseLavoro()
        {
            var fase = new FaseCentroDiLavoro() { MachineGuid = this.MachineGuid };

            SetDefaultTiming(fase);

            return fase;
        }

        protected override void Avvicinamento(ref StringBuilder code, double secureZ)
        {
            //... è ok cosi come imposto da programma.
        }

        protected override void DisimpegnoUtensile(ref StringBuilder code, bool stessoUtNextOp = false, double secureZ = 0)
        {
            if (stessoUtNextOp)
                code.AppendLine("Z" + secureZ);
            else
            {
                code.AppendLine("M5");
                code.AppendLine("G91 G28 Z0");
                code.AppendLine("G91 G28 X0 Y0");
            }
        }

        protected override void CreateCodeFromAction(ChangeToolAction programAction, ref StringBuilder code)
        {
            code.AppendLine("G0 G17 G40 G49 G80 G90");

            // controllo refrigerante
            code.AppendLine(programAction.Coolant ? "M8" : "M9");

            // Etichetta Utensile
            var toolLabel = programAction.ToolLabel;

            code.AppendLine(FormatComment(toolLabel));

            // Numero e correttore
            var toolNumber = programAction.NumberTool;

            code.AppendLine(programAction.CutViewerToolInfo);

            // rinominare in non cmabio utensile..
            if (programAction.CambioUtensile)
            {
                code.AppendLine(NumericControl.CharToolCode + toolNumber.ToString() + "M6");
            }

            // Parametri
            var speed = programAction.Speed;

            var spindleRotation = programAction.SpindleRotation;

            // Attivo Parametri 
            code.Append("G0 G54 " + "S" + FormatSpeed(speed) + "M3");

            // Attivo correttore altezza
            var nHeightCor = programAction.MillHeightCorrector;
            code.Append("\nG43 H" + nHeightCor + "\n");

            /*Resetto posizione attuale , in modo da forzare la riscrittura delle coordinate.
             * 
             */

            CurrentX = null;
            CurrentY = null;
            CurrentZ = null;

            /* 
             * QUI CI VORREBBE ANCHE PRIMO SPOSTAMENTO XY 
             * SET CURRENT CORRECTOR OR 
             */

        }



    }
}