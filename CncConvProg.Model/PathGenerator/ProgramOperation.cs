using System;
using System.Linq;

using System.Collections.Generic;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.PreviewPathEntity;

namespace CncConvProg.Model.PathGenerator
{
    [Serializable]
    public class MoveActionCollection : List<LinearMoveAction>
    {
        public void AddArcMove(AxisAbilited axisAbilited, double x, double y, double? z, double radius, bool clockWise, Point2D center, Matrix3D rotationMatrix, double? feed = null, CncCompensationState cncCompensationState = CncCompensationState.NoChange)
        {
            var moveType = clockWise ? MoveType.Cw : MoveType.Ccw;

            var rotatedEndPnt = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(x, y, 0), rotationMatrix);

            var rotatedCenterArc = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(center.X, center.Y, 0), rotationMatrix);

            var moveAction = new ArcMoveAction(axisAbilited, moveType) { Radius = radius, ClockWise = clockWise, Center = new Point2D(rotatedCenterArc.X, rotatedCenterArc.Y), CncCompensationState = cncCompensationState };

            moveAction.Z = z;

            _x = moveAction.X = rotatedEndPnt.X;
            _y = moveAction.Y = rotatedEndPnt.Y;

            this.Add(moveAction);
        }

        public void AddArcMove(AxisAbilited axisAbilited, double? x, double? y, double? z, double radius, bool clockWise, Point2D center, double? feed = null, CncCompensationState cncCompensationState = CncCompensationState.NoChange)
        {
            var moveType = clockWise ? MoveType.Cw : MoveType.Ccw;

            var moveAction = new ArcMoveAction(axisAbilited, moveType) { Radius = radius, ClockWise = clockWise, Center = center, CncCompensationState = cncCompensationState };

            moveAction.Z = z;

            _x = moveAction.X = x;
            _y = moveAction.Y = y;

            this.Add(moveAction);

        }

        internal void AddLinearMove(MoveType moveType, AxisAbilited axisAbilited, double x, double y, double? z, Matrix3D rotationMatrix, double? feed = null, CncCompensationState cncCompensationState = CncCompensationState.NoChange)
        {
            var rotatedPnt = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(x, y, 0), rotationMatrix);

            var moveAction = new LinearMoveAction(axisAbilited, moveType) { Feed = feed, CncCompensationState = cncCompensationState };

            switch (axisAbilited)
            {
                case AxisAbilited.Z:
                    {
                        moveAction.Z = z;

                    } break;

                case AxisAbilited.Xy:
                    {
                        _x = moveAction.X = rotatedPnt.X;
                        _y = moveAction.Y = rotatedPnt.Y;

                    } break;

                default:
                    {
                        throw new NotImplementedException();
                    } break;
            }

            this.Add(moveAction);

        }



        internal void AddLinearMove(MoveType moveType, AxisAbilited axisAbilited, double? x, double? y, double? z, double? feed = null, CncCompensationState cncCompensationState = CncCompensationState.NoChange)
        {
            var moveAction = new LinearMoveAction(axisAbilited, moveType) { Feed = feed, CncCompensationState = cncCompensationState };

            switch (axisAbilited)
            {
                case AxisAbilited.Z:
                    {
                        moveAction.Z = z;

                    } break;

                case AxisAbilited.Xy:
                    {
                        _x = moveAction.X = x;
                        _y = moveAction.Y = y;

                    } break;

                default:
                    {
                        _x = moveAction.X = x;
                        _y = moveAction.Y = y;
                        moveAction.Z = z;
                    } break;

            }

            this.Add(moveAction);
        }

        private double? _x;
        private double? _y;

        internal Point2D GetLastPoint()
        {
            if (_x.HasValue && _y.HasValue)
                return new Point2D(_x.Value, _y.Value);

            return null;
        }
    }
    /// <summary>
    /// Questa classe a seconda con che cosa sarà letta restiruira codice nc
    ///  oppure anteprima lavorazione
    /// </summary>
    [Serializable]
    public class ProgramOperation
    {
        /// <summary>
        /// Z di sicurezza
        /// </summary>
        public double SecureZ { get; private set; }

        /// <summary>
        /// Costruttore classe
        /// </summary>
        /// <param name="secureZ"></param>
        public ProgramOperation(double secureZ)
        {
            SecureZ = secureZ;
        }

        /// <summary>
        /// Lista di azioni dell'operazione 
        /// </summary>
        public List<ProgramAction> Azioni = new List<ProgramAction>();

        /// <summary>
        /// Metodo per aggiungere azione
        /// </summary>
        /// <param name="action"></param>
        public void AggiungiAzioneMovimento(LinearMoveAction action)
        {
            SettaValoreAvanzamento(action);

            Azioni.Add(action);
        }

        /// <summary>
        /// Se non è già definito,
        /// sssegna il valore e tipologia sia di avanzamento che di velocita al movimento.
        /// </summary>
        /// <param name="action"></param>
        public void SettaValoreAvanzamento(LinearMoveAction action)
        {
            if (action.ParametroVelocita != null) return;
            ParametroVelocita parametroVelocita;
            var finded = FeedDictionary.TryGetValue(action.MoveType, out parametroVelocita);
            if (!finded)
            {
                //todo - gestire mancato avanzamento
            }

            action.ParametroVelocita = parametroVelocita;
        }
        internal void AddFeedType(MoveType[] moveType, VelocitaType velocitaType, double p, VelocitaType velocitaType2, double p2)
        {
            foreach (var type in moveType)
            {
                AddFeedType(type, velocitaType, p, velocitaType2, p2);
            }
        }

        internal void AddAction(ProgramAction programAction)
        {
            //if (!Actions.Contains(programAction))
            Azioni.Add(programAction);
        }
        /// <summary>
        /// Dizionario avanzamenti.
        /// Tengo associato tipologia di movimento ( movimento in rapido, mov. in lavoro, lavoro rapido, .. ) con un valore di avanzamento.
        /// </summary>
        public Dictionary<MoveType, ParametroVelocita> FeedDictionary = new Dictionary<MoveType, ParametroVelocita>();

        /// <summary>
        /// Aggiungo valore di avanzamento al dizionario
        /// </summary>
        /// <param name="moveType"></param>
        /// <param name="modoVelocita"></param>
        /// <param name="valoreVelocita"></param>
        /// <param name="modoAvanzamento"></param>
        /// <param name="valoreAvanzamento"></param>
        internal void AddFeedType(MoveType moveType, VelocitaType modoVelocita, double valoreVelocita, VelocitaType modoAvanzamento, double valoreAvanzamento)
        {
            var par = new ParametroVelocita
            {
                ValoreVelocita = valoreVelocita,
                ModoVelocita = modoVelocita,
                ModoAvanzamento = modoAvanzamento,
                ValoreFeed = valoreAvanzamento
            };

            FeedDictionary.Add(moveType, par);
        }
        internal void ActiveAsseC(bool active)
        {
            var g112 = new ActiveG112(this, active);

        }

        public void SetCambioUtensile(bool cambioUtensile)
        {
            var ops = Azioni.OfType<CambiaUtensileAction>().FirstOrDefault();

            if (ops == null) return;

            ops.CambioUtensile = cambioUtensile;
        }
        public bool DisimpegnoCorto { get; set; }

    }

    public enum CncCompensationState
    {
        NoChange,
        G41, // Profilo Sx
        G42, // Profilo Dx
        G40,
    }

    [Serializable]
    public class ActiveG112 : ProgramAction
    {
        public bool Activated { get; set; }
        //public ToolMachine.ToolMachine.WorkPlane WorkPlane { get; private set; }
        public ActiveG112(ProgramOperation programPhase, bool active)
            : base(programPhase)
        {
            Activated = active;
            //WorkPlane = workPlane;
        }
    }
}