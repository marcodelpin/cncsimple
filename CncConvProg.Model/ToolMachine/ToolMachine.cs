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
using CncConvProg.Model.PreviewEntity;
using CncConvProg.Model.PreviewPathEntity;

namespace CncConvProg.Model.ToolMachine
{

    [Serializable]
    public abstract partial class ToolMachine
    {
        /// <summary>
        /// Costruttore classe
        /// </summary>
        protected ToolMachine()
        {
            MachineGuid = Guid.NewGuid();
        }
        /// <summary>
        /// Nome Macchina 
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Identificatore univoco della macchina
        /// </summary>
        public Guid MachineGuid { get; private set; }

        /// <summary>
        /// Crea fase di lavoro
        /// </summary>
        /// <returns></returns>
        public abstract FaseDiLavoro CreateFaseLavoro();

        /// <summary>
        /// Controller Macchina.
        /// es. Fanuc, Haas,
        /// </summary>
        protected NumericControl NumericControl = new NumericControl(); // per adesso ok cosi . poi fare proprieta cosi da poter modificare..

        /// <summary>
        /// Metodo per cambiare codice numerico alla macchina
        /// </summary>
        /// <param name="numericControl"></param>
        internal void SetController(NumericControl numericControl)
        {
            NumericControl = numericControl;
        }

        


        /// <summary>
        /// forse questo metodo non è necessario che risieda qui..
        /// </summary>
        /// <param name="programPhase"></param>
        /// <returns></returns>
        public List<IPreviewEntity> GetPreview(ProgramOperation programPhase)
        {
            if (programPhase == null)
                return null;

            var path3D = new PreviewPathBuilder();

            // path3D.SetStartPoint(CurrentX, CurrentY, CurrentZ);

            foreach (var programAction in programPhase.Azioni)
            {

                // qui fare metodo ricorsivo..
                if (programAction is MacroForaturaAzione)
                {
                    var macro = programAction as MacroForaturaAzione;
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



