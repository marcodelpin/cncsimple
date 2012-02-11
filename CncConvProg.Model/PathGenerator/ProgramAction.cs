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
    /// Da questa classe derivano tutte le altre azioni ( spostamento utensile, macro foratura, richiamo utensile )
    /// </summary>
    [Serializable]
    public abstract class ProgramAction
    {
        public ParametroVelocita ParametriTaglio { get; set; }

        protected ProgramAction(ProgramOperation programPhase)
        {
            if (programPhase != null)
                programPhase.AddAction(this);
        }

        /*
         * 
         * 
         
         
         */
    }

    public enum VelocitaType
    {
        Sync,
        ASync
    }

    /// <summary>
    /// Contiene i dati riguardanti avanzamento e velocità
    /// </summary>
    [Serializable]
    public class ParametroVelocita
    {
        // 0 giri fissi 1 giri variabili
        public VelocitaType ModoVelocita { get; set; }

        // 0 mm/min 1 mm/giro
        public VelocitaType ModoAvanzamento { get; set; }

        public double ValoreVelocita { get; set; }

        public double ValoreFeed { get; set; }
    }

    /// <summary>
    /// Classe Specializzata per il cambio utensile
    /// 
    /// </summary>
    [Serializable]
    public class CambiaUtensileAction : ProgramAction
    {

        #region Ctor

        public bool CambioUtensile { get; set; }

        public CambiaUtensileAction(ProgramOperation parent, Operazione operazione)
            : base(parent)
        {
            // todo:  gestire meglio secure z
            SicurezzaZ = parent.SecureZ;

            EtichettaUtensile = operazione.GetToolDescriptionName();
            IsUtensileRotante = operazione.IsRotaryTool;
            NumeroUtensile = operazione.GetToolPosition();
            Velocità = operazione.GetSpeed();
            Refrigerante = operazione.GetCoolant();
            /*
             * prendo sia numero postazioni che correttori centro di lavoro.
             */
            CorrettoreUtensileTornio = operazione.GetLatheToolCorrector();
            CorrettoreUtensileAltezzaCentro = operazione.GetToolHeightCorrector();
            ModalitaVelocita = operazione.GetSpeedType();
            RotazioneMandrino = operazione.SpindleRotation;
        }

        #endregion

        public bool Refrigerante { get; set; }

        public int NumeroUtensile { get; set; }

        public double Velocità { get; set; }

        public int CorrettoreUtensileTornio { get; set; }

        public string CorrettoreUtensileAltezzaCentro { get; set; }

        public string EtichettaUtensile { get; set; }

        public ModalitaVelocita ModalitaVelocita { get; set; }

        public SpindleRotation RotazioneMandrino { get; set; }

        public bool IsUtensileRotante { get; set; }

        public double SicurezzaZ { get; set; }

        public string CutViewerToolInfo { get; set; }
    }

    /// <summary>
    /// Questa classe contiene dati per movimenti lineari
    /// </summary>
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

    /// <summary>
    /// Questa classe contiene dati per interpolazioni circolari
    /// </summary>
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

    /// <summary>
    /// Macro per tornitura 
    /// </summary>
    [Serializable]
    public class MacroLongitudinalTurningAction : ProgramAction
    {
        public MacroLongitudinalTurningAction(ProgramOperation programPhase)
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
    /// Azione che si occupa di contenere tutti i dati riguardo la macro di foratura da eseguire
    /// </summary>
    [Serializable]
    public class MacroForaturaAzione : ProgramAction
    {
        public MacroForaturaAzione(ProgramOperation phase)
            : base(phase)
        {
            MoveActionCollection = new MoveActionCollection();
        }

        public MoveActionCollection MoveActionCollection { get; set; }

        /// <summary>
        /// Memorizza il tipo di lavorazione.
        /// es . Foratura Semplice , Maschiatura, Barenatura..
        /// </summary>
        public LavorazioniEnumOperazioni TipologiaLavorazione { get; set; }

        /// <summary>
        /// Contiene tutti i punti di dove verrà eseguita la lavorazione
        /// </summary>
        public IEnumerable<Point2D> PuntiForatura { get; set; }

        /// <summary>
        /// Contiene modalità di foratura.
        /// Semplice, Scarico Truciolo.
        /// </summary>
        public ModalitaForatura ModalitaForatura { get; set; }

        /// <summary>
        /// Punto Ritorno
        /// </summary>
        public double PuntoRitorno { get; set; }

        /// <summary>
        /// Step
        /// </summary>
        public double Step { get; set; }

        /// <summary>
        /// Z di Sicurezza
        /// </summary>
        public double SicurezzaZ { get; set; }

        /// <summary>
        /// Punto iniziale lavorazione
        /// </summary>
        public double StartZ { get; set; }

        /// <summary>
        /// Profondità in Z della lavorazione da eseguire
        /// </summary>
        public double EndZ { get; set; }

        /// <summary>
        /// Sosta
        /// </summary>
        public double Sosta { get; set; }

      //  public int MacroFeed { get; set; }
    }
}
