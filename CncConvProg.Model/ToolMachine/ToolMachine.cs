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

    /*
     * fare statistiche lavorazione
     * 
     * totali e in base al tool..
     * . tempo taglio
     *  tempo rapido
     *  tempo sosta
     *  tempo cambio tool
     *  lunghezza percorso
     *  luunghezza lavoro
     *  lunghezza rapido.
     *  feed max
     *  feed min
     *  
     *  bounfing box per la lavorazione
     *  numero di tool
     *  
     * lista di utensile e bounding box lavorazione..
     */

    [Serializable]
    public abstract class ToolMachine
    {
        protected ToolMachine()
        {
            MachineGuid = Guid.NewGuid();
        }
        public string MachineName { get; set; }
        public Guid MachineGuid { get; private set; }

        public abstract FaseDiLavoro CreateFaseLavoro();

        protected NumericControl NumericControl = new NumericControl(); // per adesso ok cosi . poi fare proprieta cosi da poter modificare..


        internal void SetPostProcessor(NumericControl numericControl)
        {
            NumericControl = numericControl;
        }

        public void GetToolHolder()
        {
            /*
             * metodo che mi restituira porta utensile,.
             * 
             * su centro dovra chiedere anche # comp.raggio
             *  e # comp.diametro
             *  
             * su tornio solo correttore.
             */
        }

        protected MoveType CurrentMoveType = MoveType.Rapid;

        // todo _ inizializzarli con valori adeguati a macchina

        // obs 
        protected double? CurrentX = null;
        protected double? CurrentY = null;
        protected double? CurrentZ = null;

        protected CncCompensationState CurrentCncCompensation = CncCompensationState.G40;

        protected MeasureUnit MeausureUnit { get; private set; }

        //// 
        //private Point3D _currentPos;

        //protected Point3D GetCurrentPos()
        //{
        //    if (_currentPos != null)
        //        return _currentPos;

        //    return null;
        //}



        protected WorkPlane CurrentWorkPlane { get; set; }

        /// <summary>
        /// forse questo metodo non è necessario che risieda qui..
        /// </summary>
        /// <param name="programPhase"></param>
        /// <returns></returns>
        public List<IPreviewEntity> GetPreview(ProgramPhase programPhase)
        {
            if (programPhase == null)
                return null;

            var path3D = new PreviewPathBuilder();

            // path3D.SetStartPoint(CurrentX, CurrentY, CurrentZ);

            foreach (var programAction in programPhase.Actions)
            {

                // qui fare metodo ricorsivo..
                if (programAction is MacroDrillingAction)
                {
                    var macro = programAction as MacroDrillingAction;
                    //CreatePreview(programAction as MacroDrillingAction, path3D);
                    foreach (var v in macro.MoveActionCollection)
                    {
                        if (v is ArcMoveAction)
                        {
                            var moveAction = programAction as ArcMoveAction;

                            AddElement(path3D, moveAction);
                        }

                        else
                        {
                            AddElement(path3D, v);
                        }

                    }
                }

                else if (programAction is ArcMoveAction)
                {
                    var moveAction = programAction as ArcMoveAction;

                    AddElement(path3D, moveAction);


                }

                else if (programAction is LinearMoveAction)
                {
                    var moveAction = programAction as LinearMoveAction;

                    AddElement(path3D, moveAction);

                }

            }

            return path3D.GetProfile();
        }

        private static void AddElement(PreviewPathBuilder path3D, LinearMoveAction moveAction)
        {
            var plotStyle = EnumPlotStyle.Path;

            if (moveAction.MoveType == MoveType.Rapid)
                plotStyle = EnumPlotStyle.RapidMove;

            path3D.AddLine(plotStyle, moveAction.X, moveAction.Y, moveAction.Z, moveAction.ParametroVelocita);

        }

        private static void AddElement(PreviewPathBuilder path3D, ArcMoveAction moveAction)
        {
            var center = new Point3D(moveAction.Center.X, moveAction.Center.Y, 0);

            path3D.AddArc(EnumPlotStyle.Arc, center, moveAction.Radius, moveAction.ClockWise,
                moveAction.X, moveAction.Y, moveAction.Z, moveAction.ParametroVelocita);
        }


        protected double CurrentFeed { get; set; }

        //protected void SetFeed(double feed, ref StringBuilder code)
        //{
        //    if (CurrentMoveType != MoveType.Rapid)
        //        if (CurrentFeed != feed)
        //        {
        //            CurrentFeed = feed;

        //            code.Append("F" + FormatFeed(CurrentFeed));
        //        }
        //}


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
        /// Metodo principale che trasforma Programma Universale in codice.
        /// </summary>
        /// <param name="uniProgram"></param>
        /// <returns></returns>
        public string ProcessProgram(MachineProgram uniProgram)
        {
            var code = new StringBuilder();
            code.AppendLine("%");

            /*todo numerare fasi*/


            var programNumber = uniProgram.ProgramNumber;
            var commentoProgramma = uniProgram.ProgramComment;
            var lastEdit = uniProgram.CreationTime;

            SetProgramNumber(programNumber, commentoProgramma, ref code);

            code.AppendLine(CutViewerHelper.PrintInitialToolPosition(0, 0, 200));
            code.AppendLine(uniProgram.CutViewerStockSettingStr);

            /*
             * resetto currentX y z
             */

            CurrentX = null;
            CurrentY = null;
            CurrentZ = null;


            // Setto mm  o inch
            MeausureUnit = uniProgram.MeasureUnit;

            if (MeausureUnit == MeasureUnit.Millimeter)
                code.AppendLine("G21 (MM)\n");
            else
            {
                code.AppendLine("G20 (INCH)\n");
            }

            int counter = 0;
            foreach (var programPhase in uniProgram.Operations)
            {

                if (programPhase == null)
                    continue;
                counter++;

                code.Append("\nN" + counter + "\n");

                /*
                 * su tornio serve avvicinamento prima in z o in alternativa in xz ,
                 * 
                 * su centro fa prima xy e poi z 
                 */


                /*
                 * qui itero le varie operazioni.
                 *  - vado a fare il controllo se necessito cambio utensile o altro,
                 *  - gestisco numero fase ( anche se in teoria dovrebbero essere gia settati 
                 *   ( però metti che lascio la possibilita di non generare certe operazioni dalla lista operazioni ..)
                 */

                foreach (var programAction in programPhase.Actions)
                {

                    CreateCodeFromAction(programAction, ref code);

                    /*
                     * 
                     */
                }


                DisimpegnoUtensile(ref code, programPhase.DisimpegnoCorto, uniProgram.NoChangeToolSecureZ);

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
        protected virtual void CreateCodeFromAction(MacroDrillingAction macro, ref StringBuilder code)
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

            var firstPoint = macro.DrillPoints.FirstOrDefault();

            if (firstPoint == null) return;

            var firstMoveCode = string.Empty;

            WriteMoveToX(firstPoint.X, ref firstMoveCode, true);
            WriteMoveToY(firstPoint.Y, ref firstMoveCode, true);

            code.AppendLine(firstMoveCode);

            var toZ = string.Empty;
            WriteMoveToZ(macro.SecureZ, ref toZ, true);

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

            if (macro.PuntoR > 0)
                macroCode += ("R" + FormatCoordinate(macro.PuntoR));

            if (macro.Dweel > 0)
                macroCode += ("P" + FormatCoordinate(macro.Dweel));

            if (macro.MacroFeed > 0)
                macroCode += (FormatFeed(macro.MacroFeed));

            code.AppendLine(macroCode);

            for (var i = 1; i < macro.DrillPoints.Count(); i++)
            {
                var pnt = macro.DrillPoints.ElementAt(i);

                code.Append("X" + FormatCoordinate(pnt.X));
                code.Append("Y" + FormatCoordinate(pnt.Y));
                code.Append("\n");

            }

            code.AppendLine("G80");
            code.AppendLine(toZ);

        }

        protected virtual void CreateCodeFromAction(ChangeToolAction programAction, ref StringBuilder code) { }
        protected virtual void CreateCodeFromAction(MacroLongitudinalTurningAction programAction, ref StringBuilder code) { }

        private void SetProgramNumber(int programNumber, string commentoProgramma, ref StringBuilder code)
        {
            var pgrNumberCode = NumericControl.CharNumberProgram + programNumber.ToString();

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
        //public abstract void SetZeroPoint(Point3D point3D);

        //public abstract List<Geometry.Entity.IEntity2D> GetPreview(ProgramPhase programPhase);

        //protected TimeSpan GetActionTime(ArcMoveAction arcMoveAction, ProgramPhase programPhase)
        //{
        //    var feedValue = programPhase.GetFeedValue(arcMoveAction);

        //    /* 
        //     * con feed 0 magari lanciare eccezzione..
        //     */

        //}

        //protected TimeSpan GetActionTime(LinearMoveAction linearMoveAction, ProgramPhase programPhase)
        //{
        //    var feedValue = programPhase.GetFeedValue(linearMoveAction);

        //    Debug.Assert(feedValue > 0);

        //    if (CurrentX != null && CurrentY != null && CurrentZ != null && feedValue > 0)
        //    {
        //        var line = new Line3D
        //                       {
        //                           Start = new Point3D(CurrentX.Value, CurrentY.Value, CurrentZ.Value),
        //                           End = new Point3D(CurrentX.Value, CurrentY.Value, CurrentZ.Value)
        //                       };


        //        if (linearMoveAction.X.HasValue)
        //            CurrentX = line.End.X = linearMoveAction.X.Value;

        //        if (linearMoveAction.Y.HasValue)
        //            CurrentY = line.End.Y = linearMoveAction.Y.Value;

        //        if (linearMoveAction.Z.HasValue)
        //            CurrentZ = line.End.Z = linearMoveAction.Z.Value;

        //        return TimeHelper.CalcTime(line, feedValue, MeausureUnit);
        //    }

        //    if (linearMoveAction.X.HasValue)
        //        CurrentX = linearMoveAction.X;

        //    if (linearMoveAction.Y.HasValue)
        //        CurrentY = linearMoveAction.Y;

        //    if (linearMoveAction.Z.HasValue)
        //        CurrentZ = linearMoveAction.Z;



        //    return new TimeSpan();
        //}

        //protected TimeSpan GetActionTime(ArcMoveAction linearMoveAction, ProgramPhase programPhase)
        //{
        //    var feedValue = programPhase.GetFeedValue(linearMoveAction);

        //    Debug.Assert(feedValue > 0);

        //    if (CurrentX != null && CurrentY != null && CurrentZ != null && feedValue > 0)
        //    {
        //        var arc3D = new Arc3D()
        //                       {
        //                           Start = new Point3D(CurrentX.Value, CurrentY.Value, CurrentZ.Value),
        //                           End = new Point3D(CurrentX.Value, CurrentY.Value, CurrentZ.Value),
        //                           Center = new Point3D(linearMoveAction.Center.X, linearMoveAction.Center.Y),
        //                           ClockWise = linearMoveAction.ClockWise,
        //                           Radius = linearMoveAction.Radius,

        //                       };


        //        if (linearMoveAction.X.HasValue)
        //            CurrentX = arc3D.End.X = linearMoveAction.X.Value;

        //        if (linearMoveAction.Y.HasValue)
        //            CurrentY = arc3D.End.Y = linearMoveAction.Y.Value;

        //        if (linearMoveAction.Z.HasValue)
        //            CurrentZ = arc3D.End.Z = linearMoveAction.Z.Value;

        //        return TimeHelper.CalcTime(arc3D, feedValue, MeausureUnit);
        //    }

        //    if (linearMoveAction.X.HasValue)
        //        CurrentX = linearMoveAction.X;

        //    if (linearMoveAction.Y.HasValue)
        //        CurrentY = linearMoveAction.Y;

        //    if (linearMoveAction.Z.HasValue)
        //        CurrentZ = linearMoveAction.Z;

        //    return new TimeSpan();
        //}

        //internal TimeSpan GetTime(ProgramPhase programPhase, MeasureUnit measureUnit)
        //{
        //    MeausureUnit = measureUnit; // 

        //    var operationTime = new TimeSpan();

        //    ResetMachineStatus();

        //    /*
        //     * per ora ho solamente tempi per spostamenti
        //     * poi bastera fare altri metodi per cambio ute, sosta ec..
        //     */

        //    if (programPhase == null) return new TimeSpan();

        //    foreach (var programAction in programPhase.Actions)
        //    {
        //        var timeP = GetActionTime(programAction, programPhase);

        //        operationTime = operationTime.Add(timeP);
        //    }

        //    return operationTime;
        //}


        //bool _flagCalcTime = true;
        //protected TimeSpan GetActionTime(ProgramAction programAction, ProgramPhase programPhase)
        //{
        //    dynamic action = programAction;

        //    if (!_flagCalcTime)
        //    {
        //        // qui non ha eseguito e cominciava con stackoverflow
        //        return new TimeSpan();
        //    }

        //    _flagCalcTime = false;

        //    var time = GetActionTime(action, programPhase);

        //    // se qui ha eseguito
        //    _flagCalcTime = true;

        //    return time;
        //}

        /// <summary>
        /// Da insieme di linee e archi restituisco 
        /// </summary>
        /// <param name="pathPreview"></param>
        /// <returns></returns>
        internal OperazioneTime GetTime(List<IPreviewEntity> pathPreview)
        {
            return new OperazioneTime();
            ///*
            // * todo . dividere fra i tempi di rapido, lavoro , varie.
            // */
            //var rapid = new TimeSpan();
            //var work = new TimeSpan();
            //var workDistancePercurred = 0d;
            //var rapidDistancePercurred = 0d;

            //foreach (var previewable in pathPreview)
            //{
            //    if (previewable.IsRapidMovement)
            //    {
            //        rapid += previewable.GetTimeSpan();
            //        rapidDistancePercurred += previewable.GetMoveLength;

            //    }
            //    else
            //    {
            //        work += previewable.GetTimeSpan();
            //        workDistancePercurred += previewable.GetMoveLength;

            //    }
            //}

            //var op = new OperazioneTime
            //             {
            //                 TempoRapido = rapid,
            //                 TempoLavoro = work,
            //                 DistanzaPercorsaLavoro = workDistancePercurred,
            //                 DistanzaPercorsaRapido = rapidDistancePercurred,
            //             };

            //return op;
        }

        public double MaxGiri { get; set; }

        public double VelocitaRapido { get; set; }
    }
}



