

using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.Model.PreviewPathEntity;

namespace CncConvProg.Model.PathGenerator
{
    /// <summary>
    /// Classe base per azione programma.
    /// da questa derivera ogni cosa 
    /// cambioUtensile : ProgramActrion
    /// </summary>
    [Serializable]
    public class ProgramAction
    {
        public ParametroVelocita ParametroVelocita { get; set; }

        public ProgramAction(ProgramPhase programPhase)
        {
            if (programPhase != null)
                programPhase.AddAction(this);
        }

        /*
         * 
         * 
         
         
         */
    }

    [Serializable]
    public class ChangeToolAction : ProgramAction
    {

        #region Ctor
        //public ChangeToolAction(OperazioneFresaCandela operazioneFresaCandela)
        //{
        //    SpindleRotation = operazioneFresaCandela.SpindleRotation;
        //    IsRotaryTool = operazioneFresaCandela.IsRotaryTool;
        //    ToolLabel = operazioneFresaCandela.GetToolName();
        //    LatheToolCorrector = operazioneFresaCandela.GetLatheToolCorrector();
        //    NumberTool = operazioneFresaCandela.GetToolNumber();
        //    ModalitaVelocita = operazioneFresaCandela.GetSpeedType();
        //    Speed = operazioneFresaCandela.GetNumeroGiri();

        //}

        public bool CambioUtensile { get; set; }
        public ChangeToolAction(ProgramPhase parent, Operazione operazione)
            : base(parent)
        {
            // todo:  gestire meglio secure z
            SecureZ = parent.SecureZ;

            ToolLabel = operazione.GetToolDescriptionName();
            IsRotaryTool = operazione.IsRotaryTool;
            NumberTool = operazione.GetToolPosition();
            Speed = operazione.GetSpeed();
            Coolant = operazione.GetCoolant();
            /*
             * prendo sia numero postazioni che correttori centro di lavoro.
             */
            LatheToolCorrector = operazione.GetLatheToolCorrector();
            MillHeightCorrector = operazione.GetToolHeightCorrector();
            ModalitaVelocita = operazione.GetSpeedType();
            SpindleRotation = operazione.SpindleRotation;
        }

        //public ChangeToolAction(OperazioneMaschiatura operazione)
        //{
        //    ToolLabel = operazione.GetToolName();
        //    IsRotaryTool = operazione.IsRotaryTool;
        //    NumberTool = operazione.GetToolNumber();
        //    Speed = operazione.GetSpeed();
        //    LatheToolCorrector = operazione.GetLatheToolCorrector();
        //    ModalitaVelocita = operazione.GetSpeedType();
        //    SpindleRotation = operazione.SpindleRotation;
        //}

        //public ChangeToolAction(OperazioneUtensileTornitura operazione)
        //{
        //    // TODO: Complete member initialization
        //    this.operazione = operazione;
        //}

        #endregion

        public bool Coolant { get; set; }

        public int NumberTool { get; set; }

        public double Speed { get; set; }

        public int LatheToolCorrector { get; set; }

        public string MillHeightCorrector { get; set; }


        public string ToolLabel { get; set; }

        public ModalitaVelocita ModalitaVelocita { get; set; }

        public SpindleRotation SpindleRotation { get; set; }

        public bool IsRotaryTool { get; set; }

        public double SecureZ { get; set; }


        public string CutViewerToolInfo { get; set; }
    }

    [Serializable]
    public class LinearMoveAction : ProgramAction
    {
        public LinearMoveAction(AxisAbilited abilited, MoveType moveType)
            : base(null)
        {
            AxisAbilited = abilited;
            MoveType = moveType;
        }


        public CncCompensationState CncCompensationState { get; set; }
        public MoveType MoveType { get; private set; }
        public AxisAbilited AxisAbilited { get; private set; }
        public double? Feed;
        public double? X;
        public double? Y;
        public double? Z;
        public double? Ix;
        public double? Iy;
        public double? Iz;

        private LinearMoveAction(LinearMoveAction linearMove)
            : base(null)
        {
            // clonare..
            MoveType = linearMove.MoveType;
        }

        internal virtual LinearMoveAction MultiplyMatrix(Matrix3D rotationMatrix)
        {
            if (AxisAbilited == AxisAbilited.Z || !(X.HasValue && Y.HasValue))
            {
                return this;
            }

            // coordinate dovrebbere essere note entrambi
            if (!X.HasValue || !Y.HasValue)
                throw new Exception("LinearAction.MuliplyMatrix");

            var rsltLine = new LinearMoveAction(this);

            var rotatedPnt = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(X.Value, Y.Value, 0), rotationMatrix);
            rsltLine.X = rotatedPnt.X;
            rsltLine.Y = rotatedPnt.Y;
            return rsltLine;
        }



        internal double GetFeedValue()
        {
            return Feed ?? 0;
        }
    }

    [Serializable]
    public class ArcMoveAction : LinearMoveAction
    {
        internal override LinearMoveAction MultiplyMatrix(Matrix3D rotationMatrix)
        {
            // coordinate dovrebbere essere note entrambi
            if (!X.HasValue || !Y.HasValue)
                throw new Exception("LinearAction.MuliplyMatrix");

            var rsltArc = new ArcMoveAction(AxisAbilited, MoveType)
                               {
                                   ClockWise = ClockWise,
                                   Radius = Radius,
                                   Z = Z
                               };

            var rotatedEndPnt = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(X.Value, Y.Value, 0), rotationMatrix);

            var rotatedCenterArc = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(Center.X, Center.Y, 0), rotationMatrix);

            rsltArc.X = rotatedEndPnt.X;
            rsltArc.Y = rotatedEndPnt.Y;

            rsltArc.Center = new Point2D(rotatedCenterArc.X, rotatedCenterArc.Y);

            return rsltArc;




        }
        public ArcMoveAction(AxisAbilited abilited, MoveType moveType)
            : base(abilited, moveType)
        {



        }


        public Point2D Center { get; set; }
        public double Radius { get; set; }
        public bool ClockWise { get; set; }

    }

    [Serializable]
    public class MacroLongitudinalTurningAction : ProgramAction
    {
        public MacroLongitudinalTurningAction(ProgramPhase programPhase)
            : base(programPhase)
        {

        }
        public Profile2D Profile { get; set; }

        public double ProfonditaPassata { get; set; }

        public double SovraMetalloX { get; set; }

        public double SovraMetalloZ { get; set; }

        public Point2D PuntoAttacco { get; set; }
    }

    /// <summary>
    /// Magari fare classe base per tutte le macro di foratura..
    /// comue punti
    /// inizio
    /// fine
    /// sicurezza
    /// profondita.
    /// </summary>
    [Serializable]
    public class MacroDrillingAction : ProgramAction
    {
        public MacroDrillingAction(ProgramPhase phase)
            : base(phase)
        {
            MoveActionCollection = new MoveActionCollection();
        }

        public MoveActionCollection MoveActionCollection { get; set; }

        public LavorazioniEnumOperazioni TipologiaLavorazione { get; set; }

        public IEnumerable<Point2D> DrillPoints { get; set; }

        public ModalitaForatura ModalitaForatura { get; set; }

        public double PuntoR { get; set; }

        public double Step { get; set; }

        public double SecureZ { get; set; }

        public double StartZ { get; set; }

        public double EndZ { get; set; }

        public double Dweel { get; set; }

        public int MacroFeed { get; set; }
    }

    //[Serializable]
    //public class MacroThreadingAction : ProgramAction
    //{
    //    public MacroThreadingAction(ProgramPhase phase)
    //        : base(phase)
    //    {

    //    }
    //    public List<Point2D> DrillPoints { get; set; }

    //    public double SecureZ { get; set; }

    //    public double StartZ { get; set; }

    //    public double EndZ { get; set; }

    //    public double Dweel { get; set; }
    //}
}
