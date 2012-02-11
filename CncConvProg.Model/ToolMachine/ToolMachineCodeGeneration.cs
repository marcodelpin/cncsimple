using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.PreviewPathEntity;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.MachineControl;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.PreviewPathEntity;

namespace CncConvProg.Model.ToolMachine
{

    /// <summary>
    /// Classe parziale ToolMachine.
    /// In questo file sono contenuti i metodi e proprietà necessari per creare GCODE
    /// </summary>
    public abstract partial class ToolMachine
    {
        #region Stato Macchina

        /*
         * Qui si trovano proprietà che memorizzano lo stato della macchina.
         * - La posizione corrente.
         * - L'avanzamento impostato.
         * - Il tipo del movimento corrente ( rapido, lavoro , .. )
         * - Compensazione Attivata 
         */
        
        protected MoveType CurrentMoveType = MoveType.Rapid;

        protected double? CurrentX = null;
        protected double? CurrentY = null;
        protected double? CurrentZ = null;

        protected double CurrentFeed { get; set; }

        protected CncCompensationState CurrentCncCompensation = CncCompensationState.G40;

        protected MeasureUnit MeausureUnit { get; private set; }

        protected WorkPlane CurrentWorkPlane { get; set; }

        #endregion

        /// <summary>
        /// Imposta avanzamento corrente e lo stampa il codice .
        /// Se l'avanzamento corrente equivale  con valore avanzamento nuovo non stampa niente
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="code"></param>
        protected void SetFeed(double feed, ref string code)
        {
            if (feed == 0) return;
            if (CurrentMoveType != MoveType.Rapid)
                if (CurrentFeed != feed)
                {
                    CurrentFeed = feed;

                    code += "F" + FormatFeed(CurrentFeed);
                }
        }

        /// <summary>
        /// Metodo principale che crea codice G
        /// </summary>
        /// <param name="program">Classe che contiene tutte le informazioni necessarie per creare il programma</param>
        /// <returns>Programma GCODE</returns>
        public string ProcessProgram(MachineProgram program)
        {
            /*
             * Inizializzo code, inseriro qui il codice g risultante.
             */

            var code = new StringBuilder();

            code.AppendLine("%");

            /*todo - numerare fasi*/

            var programNumber = program.ProgramNumber;
            var commentoProgramma = program.ProgramComment;
            var lastEdit = program.CreationTime;

            SetProgramNumber(programNumber, commentoProgramma, ref code);

            // Aggiungo linee per il settaggio di del grezzo in cutViewer
            // Todo - magari aggiungere una variabile booleana che abilita o meno l'inserimento di stringhe riguardanti CutViewer
            code.AppendLine(CutViewerHelper.PrintInitialToolPosition(0, 0, 200));
            code.AppendLine(program.CutViewerStockSettingStr);

            // Resetto stato macchina
            ResetMachineStatus();

            // Setto [mm]  o [inch] 
            MeausureUnit = program.MeasureUnit;

            if (MeausureUnit == MeasureUnit.Millimeter)
                code.AppendLine("G21 (MM)\n");
            else
            {
                code.AppendLine("G20 (INCH)\n");
            }

            // Contatore incrementale del numero di operazioni , lo uso per stampare
            // es. N10 ..
            var contatoreOperazione = 0;

            /*
             * Itero tutte le operazioni contenute nella variabile program.
             */
            foreach (var programPhase in program.Operazioni)
            {

                if (programPhase == null)
                    continue;
                contatoreOperazione++;

                //Aggiungo il numero di operazione
                code.Append("\nN" + contatoreOperazione + "\n");

                /*
                 * In questo ciclo annidato itero tutte le varie Azioni contenute dentro operazione corrente.
                 */
                foreach (var programAction in programPhase.Azioni)
                {
                    /*
                     * In base al tipo di azione , andrò a richiamare il metodo corretto
                     */
                    CreateCodeFromAction(programAction, ref code);
                }


                DisimpegnoUtensile(ref code, programPhase.DisimpegnoCorto, program.ZSicurezzaNoCambioUtensile);

                code.AppendLine("M1");
                code.AppendLine(string.Empty);

            }

            code.AppendLine(string.Empty);
            DisimpegnoUtensile(ref code);
            code.AppendLine("M30");
            code.AppendLine("%");

            return code.ToString();
        }

        protected abstract void Avvicinamento(ref StringBuilder code, double secureZ);

        protected abstract void DisimpegnoUtensile(ref StringBuilder code, bool stessoUtNextOp = false, double secureZ = 0);

        bool _flag = true;
        protected void CreateCodeFromAction(ProgramAction programAction, ref StringBuilder code)
        {
            dynamic action = programAction;

            if (!_flag)
            {
                // qui non ha eseguito e cominciava con stackoverflow
                return;
            }

            _flag = false;

            CreateCodeFromAction(action, ref code);
            // se qui ha eseguito
            _flag = true;
        }


        protected virtual void CreateCodeFromAction(ActiveG112 activeG112, ref StringBuilder code) { }

        /// <summary>
        /// Crea codice da macro drilling action
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="code"></param>
        protected virtual void CreateCodeFromAction(MacroForaturaAzione macro, ref StringBuilder code)
        {
            /*
             * si muove in xy del primo elemento
             * zSicurezza
             * calcola R
             * macro
             * listaPunti
             * chiudeMacro.
             */

            string macroCode;

            var firstPoint = macro.PuntiForatura.FirstOrDefault();

            if (firstPoint == null) return;

            var firstMoveCode = string.Empty;

            WriteMoveToX(firstPoint.X, ref firstMoveCode, true);
            WriteMoveToY(firstPoint.Y, ref firstMoveCode, true);

            code.AppendLine(firstMoveCode);

            var toZ = string.Empty;
            WriteMoveToZ(macro.SicurezzaZ, ref toZ, true);

            code.AppendLine(toZ);

            switch (macro.TipologiaLavorazione)
            {
                case LavorazioniEnumOperazioni.ForaturaAlesatore:
                    {
                        macroCode = "G85";
                    } break;

                case LavorazioniEnumOperazioni.ForaturaMaschiaturaDx:
                    {
                        macroCode = "G84";
                    } break;

                case LavorazioniEnumOperazioni.ForaturaMaschiaturaSx:
                    {
                        macroCode = "G84.1";
                    } break;

                case LavorazioniEnumOperazioni.ForaturaBareno:
                    {
                        macroCode = "G84";
                    } break;

                case LavorazioniEnumOperazioni.ForaturaPunta:
                    {
                        switch (macro.ModalitaForatura)
                        {
                            case ModalitaForatura.StepScaricoTruciolo:
                                {
                                    macroCode = "G83";
                                } break;

                            case ModalitaForatura.StepSenzaScaricoTruciolo:
                                {
                                    macroCode = "G83";
                                } break;

                            default:
                            case ModalitaForatura.Semplice:
                                {
                                    macroCode = "G81";
                                } break;
                        }
                    } break;


                default:
                case LavorazioniEnumOperazioni.ForaturaCentrino:
                case LavorazioniEnumOperazioni.ForaturaSmusso:
                case LavorazioniEnumOperazioni.ForaturaLamatore:
                    {
                        macroCode = "G81";
                    } break;
            }



            macroCode += ("Z" + FormatCoordinate(macro.EndZ));

            if (macro.Step > 0)
                macroCode += ("Q" + FormatCoordinate(macro.Step));

            if (macro.PuntoRitorno > 0)
                macroCode += ("R" + FormatCoordinate(macro.PuntoRitorno));

            if (macro.Sosta > 0)
                macroCode += ("P" + FormatCoordinate(macro.Sosta));

            if (macro.MacroFeed > 0)
                macroCode += (FormatFeed(macro.MacroFeed));

            code.AppendLine(macroCode);

            for (var i = 1; i < macro.PuntiForatura.Count(); i++)
            {
                var pnt = macro.PuntiForatura.ElementAt(i);

                code.Append("X" + FormatCoordinate(pnt.X));
                code.Append("Y" + FormatCoordinate(pnt.Y));
                code.Append("\n");

            }

            code.AppendLine("G80");
            code.AppendLine(toZ);

        }

        protected virtual void CreateCodeFromAction(CambiaUtensileAction programAction, ref StringBuilder code) { }
        protected virtual void CreateCodeFromAction(MacroLongitudinalTurningAction programAction, ref StringBuilder code) { }

        /// <summary>
        /// Metodo ausiliario che inserisce la linea contenente il numero di programma e il commento.
        /// </summary>
        /// <param name="numeroProgramma"></param>
        /// <param name="commentoProgramma"></param>
        /// <param name="code"></param>
        private void SetProgramNumber(int numeroProgramma, string commentoProgramma, ref StringBuilder code)
        {
            var pgrNumberCode = NumericControl.CharNumberProgram + numeroProgramma.ToString();

            var programComment = FormatComment(commentoProgramma);

            code.AppendLine(pgrNumberCode + programComment);
        }

        private static string GetMoveCode(MoveType movetype, NumericControl numericControl)
        {
            switch (movetype)
            {
                case MoveType.Rapid:
                    return numericControl.CmdMoveRapid;

                case MoveType.SecureRapidFeed:
                case MoveType.PlungeFeed:
                case MoveType.Work:
                    return numericControl.CmdMoveWork;

                case MoveType.Ccw:
                    return numericControl.CMD_MoveCCW;

                case MoveType.Cw:
                    return numericControl.CMD_MoveCW;

                default:
                    throw new NotImplementedException();
                    break;
            }
        }
        protected void SetMoveType(MoveType moveType, ref StringBuilder code)
        {
            var moveCode = GetMoveCode(moveType, NumericControl);

            if (moveCode != GetMoveCode(CurrentMoveType, NumericControl))
            {
                code.Append(moveCode);
            }

            CurrentMoveType = moveType;

        }

        protected void SetMoveType(MoveType moveType, ref string strCode)
        {
            var moveCode = GetMoveCode(moveType, NumericControl);

            if (moveCode != GetMoveCode(CurrentMoveType, NumericControl))
            {
                strCode += moveCode;
            }

            CurrentMoveType = moveType;

        }


        protected static string FormatComment(string comment)
        {
            if (comment == null)
                comment = string.Empty;

            return "(" + comment.ToUpper() + ")";
        }

        protected static string oo(double s)
        {
            return String.Format("{0:0.##}", s);
        }
        protected static string FormatSpeed(double speed)
        {
            var v = Math.Round(speed, 0);
            var v1 = Math.Truncate(v);
            return v1.ToString();
        }

        protected static string FormatValue(double value, int maxDecimal)
        {
            var v = Math.Round(value, maxDecimal);
            var v1 = Math.Truncate(v);
            return v1.ToString();
        }

        protected double FormatCoordinate(double coordinate)
        {
            var numberDecimal = 3;

            if (MeausureUnit == MeasureUnit.Inch)
                numberDecimal = 4;

            var v = Math.Round(coordinate, numberDecimal);

            return v;
        }


        protected string FormatFeed(double feed)
        {
            var numberDecimal = 3;

            if (MeausureUnit == MeasureUnit.Inch)
                numberDecimal = 4;

            var v = Math.Round(feed, numberDecimal);

            return v.ToString();
        }

        public enum WorkPlane
        {
            XY,
            XZ,
            YZ,
            LatheXz,
        }



        protected void CreateCodeFromAction(LinearMoveAction programAction, ref StringBuilder code)
        {
            var str = string.Empty;

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

            SetCompensation(programAction.CncCompensationState, ref str);

            SetFeed(programAction.GetFeedValue(), ref str);

            if (string.IsNullOrWhiteSpace(str)) return;

            code.AppendLine(str);



        }

        private void SetCompensation(CncCompensationState cncCompensationState, ref string str)
        {
            if (CurrentCncCompensation == cncCompensationState || cncCompensationState == CncCompensationState.NoChange) return;

            CurrentCncCompensation = cncCompensationState;

            switch (cncCompensationState)
            {
                case CncCompensationState.G41:
                    {
                        str += "G41";
                    } break;

                case CncCompensationState.G42:
                    {
                        str += "G42";
                    } break;

                case CncCompensationState.G40:
                    {
                        str += "G40";
                    } break;

                default:
                    break;
            }
        }

        protected virtual void WriteMoveToX(double p, ref string code, bool forceWrite = false)
        {
            if (!forceWrite)
                if (CurrentX.HasValue && p == CurrentX.Value) return;

            code += ("X" + FormatCoordinate(p));

            CurrentX = p;
        }

        protected void WriteMoveToY(double p, ref string code, bool forceWrite = false)
        {
            if (!forceWrite)
                if (CurrentY.HasValue && p == CurrentY.Value) return;

            var codeY = GetYCode();
            code += (codeY + FormatCoordinate(p));

            CurrentY = p;
        }

        protected virtual string GetYCode()
        {
            return "Y";
        }

        protected void WriteMoveToZ(double p, ref string code, bool forceWrite = false)
        {
            if (!forceWrite)
                if (CurrentZ.HasValue && p == CurrentZ.Value) return;

            code += ("Z" + FormatCoordinate(p));

            CurrentZ = p;
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
        protected virtual void CreateCodeFromAction(ArcMoveAction programAction, ref StringBuilder code)
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
            // str += "R" + FormatCoordinate(programAction.Radius);

            SetFeed(programAction.GetFeedValue(), ref str);

            SetCompensation(programAction.CncCompensationState, ref str);

            if (string.IsNullOrWhiteSpace(str)) return;

            code.AppendLine(str);
        }


        /// <summary>
        /// In fase di creazione programma , o quando è necessario calcolare tempo macchina , bisogna tenere lo stato della macchina 
        /// ( posizione, modalita avanzamento e mod. velocita , valore avanzamanto ecc. .) .
        /// 
        /// Bisogna però resettare questo stato quando si ricalcola..
        /// </summary>
        private void ResetMachineStatus()
        {
            CurrentX = null;
            CurrentY = null;
            CurrentZ = null;
        }
   
    }
}



