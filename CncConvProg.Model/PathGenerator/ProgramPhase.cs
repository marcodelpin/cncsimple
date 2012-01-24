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

            var moveAction = new ArcMoveAction(axisAbilited, moveType) { Radius = radius, ClockWise = clockWise, Center = new Point2D(rotatedCenterArc.X, rotatedCenterArc.Y), Feed = feed, CncCompensationState = cncCompensationState };

            moveAction.Z = z;

            _x = moveAction.X = rotatedEndPnt.X;
            _y = moveAction.Y = rotatedEndPnt.Y;

            this.Add(moveAction);
            //switch (axisAbilited)
            //{
            //    case AxisAbilited.Z:
            //        {
            //            moveAction.Z = z;

            //        } break;

            //    case AxisAbilited.Xy:
            //        {
            //            moveAction.X = x;
            //            moveAction.Y = y;

            //        } break;

            //    default:
            //        {
            //            throw new NotImplementedException();
            //        } break;
            //}

            //  Actions.Add(moveAction);
        }

        public void AddArcMove(AxisAbilited axisAbilited, double? x, double? y, double? z, double radius, bool clockWise, Point2D center, double? feed = null, CncCompensationState cncCompensationState = CncCompensationState.NoChange)
        {
            var moveType = clockWise ? MoveType.Cw : MoveType.Ccw;

            var moveAction = new ArcMoveAction(axisAbilited, moveType) { Radius = radius, ClockWise = clockWise, Center = center, Feed = feed, CncCompensationState = cncCompensationState };

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
                        throw new NotImplementedException();
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
    public class ProgramPhase
    {
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


        internal void AddFeedType(MoveType[] moveType, VelocitaType velocitaType, double p, VelocitaType velocitaType2, double p2)
        {
            foreach (var type in moveType)
            {
                AddFeedType(type, velocitaType, p, velocitaType2, p2);
            }
        }

        public double SecureZ { get; private set; }

        public ProgramPhase(double secureZ)
        {
            SecureZ = secureZ;
        }

        /*
         * o apro tipo buffer e poi immaggazzion tutti i poi creo buffer azioni da ripetere...
         * 
         * come farei con o flag su azioni o comunque il codice lo devo ripetere..
         * 
         * o apro altro programma e 
         */
        public List<ProgramAction> Actions = new List<ProgramAction>();

        /*
         * todo : ce un piccolo malinteso di clockwise, funziona all'incontrrio . risolvere
         */

        public void AddMoveAction(LinearMoveAction action)
        {
            //// se feed non è dichiarato esplicitamente , prendo quello del dizionario
            //if (action.Feed == null)
            //    if (FeedDictionary != null)
            //    {
            //        double feed;
            //        FeedDictionary.TryGetValue(action.MoveType, out feed);

            //        // se non trova niente setta 0
            //        action.Feed = feed;
            //    }
            //    else
            //        action.Feed = action.GetFeedValue();

            SetFeedMoveAction(action);

            Actions.Add(action);
        }

        /// <summary>
        /// Assegna il feed al movimento.
        /// </summary>
        /// <param name="action"></param>
        public void SetFeedMoveAction(LinearMoveAction action)
        {
            // se feed non è dichiarato esplicitamente , prendo quello del dizionario
            ParametroVelocita parametroVelocita;
            var finded = FeedDictionary.TryGetValue(action.MoveType, out parametroVelocita);
            if (!finded)
            {
                //Trace.
            }

            action.ParametroVelocita = parametroVelocita;
        }

        ///// <summary>
        /////  Questi 4 metodi spostati in altra classe.
        ///// </summary>

        //public void AddArcMove(AxisAbilited axisAbilited, double x, double y, double? z, double radius, bool counterClockWise, Point2D center, Matrix3D rotationMatrix)
        //{
        //    var moveType = counterClockWise ? MoveType.Cw : MoveType.Ccw;

        //    var rotatedEndPnt = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(x, y, 0), rotationMatrix);

        //    var rotatedCenterArc = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(center.X, center.Y, 0), rotationMatrix);

        //    var moveAction = new ArcMoveAction(axisAbilited, moveType) { Radius = radius, ClockWise = counterClockWise, Center = new Point2D(rotatedCenterArc.X, rotatedCenterArc.Y) };

        //    if (FeedDictionary != null)
        //        FeedDictionary.TryGetValue(moveType, out moveAction.Feed);

        //    moveAction.Z = z;

        //    moveAction.X = rotatedEndPnt.X;
        //    moveAction.Y = rotatedEndPnt.Y;

        //    //switch (axisAbilited)
        //    //{
        //    //    case AxisAbilited.Z:
        //    //        {
        //    //            moveAction.Z = z;

        //    //        } break;

        //    //    case AxisAbilited.Xy:
        //    //        {
        //    //            moveAction.X = x;
        //    //            moveAction.Y = y;

        //    //        } break;

        //    //    default:
        //    //        {
        //    //            throw new NotImplementedException();
        //    //        } break;
        //    //}

        //      Actions.Add(moveAction);
        //}

        //public void AddArcMove(AxisAbilited axisAbilited, double? x, double? y, double? z, double radius, bool counterClockWise, Point2D center)
        //{
        //    var moveType = counterClockWise ? MoveType.Cw : MoveType.Ccw;

        //    var moveAction = new ArcMoveAction(axisAbilited, moveType) { Radius = radius, ClockWise = counterClockWise, Center = center };

        //    if (FeedDictionary != null)
        //        FeedDictionary.TryGetValue(moveType, out moveAction.Feed);

        //    moveAction.Z = z;

        //    moveAction.X = x;
        //    moveAction.Y = y;

        //    //switch (axisAbilited)
        //    //{
        //    //    case AxisAbilited.Z:
        //    //        {
        //    //            moveAction.Z = z;

        //    //        } break;

        //    //    case AxisAbilited.Xy:
        //    //        {
        //    //            moveAction.X = x;
        //    //            moveAction.Y = y;

        //    //        } break;

        //    //    default:
        //    //        {
        //    //            throw new NotImplementedException();
        //    //        } break;
        //    //}

        //      Actions.Add(moveAction);
        //}

        //internal void AddLinearMove(MoveType moveType, AxisAbilited axisAbilited, double x, double y, double? z, Matrix3D rotationMatrix)
        //{

        //    var rotatedPnt = Geometry.GeometryHelper.MultiplyPoint(new Geometry.Point3D(x, y, 0), rotationMatrix);

        //    var moveAction = new LinearMoveAction(axisAbilited, moveType);

        //    switch (axisAbilited)
        //    {
        //        case AxisAbilited.Z:
        //            {
        //                moveAction.Z = z;

        //            } break;

        //        case AxisAbilited.Xy:
        //            {
        //                moveAction.X = rotatedPnt.X;
        //                moveAction.Y = rotatedPnt.Y;

        //            } break;

        //        default:
        //            {
        //                throw new NotImplementedException();
        //            } break;
        //    }


        //    if (FeedDictionary != null)
        //        FeedDictionary.TryGetValue(moveType, out moveAction.Feed);

        //      Actions.Add(moveAction);
        //}

        //internal void AddLinearMove(MoveType moveType, AxisAbilited axisAbilited, double? x, double? y, double? z)
        //{
        //    var moveAction = new LinearMoveAction(axisAbilited, moveType);

        //    switch (axisAbilited)
        //    {
        //        case AxisAbilited.Z:
        //            {
        //                moveAction.Z = z;

        //            } break;

        //        case AxisAbilited.Xy:
        //            {
        //                moveAction.X = x;
        //                moveAction.Y = y;

        //            } break;

        //        default:
        //            {
        //                throw new NotImplementedException();
        //            } break;
        //    }


        //    if (FeedDictionary != null)
        //        FeedDictionary.TryGetValue(moveType, out moveAction.Feed);

        //       Actions.Add(moveAction);
        //}

        internal void AddAction(ProgramAction programAction)
        {
            //if (!Actions.Contains(programAction))
            Actions.Add(programAction);
        }
        public Dictionary<MoveType, ParametroVelocita> FeedDictionary { get; private set; }

        //private double _currentFeed;

        //private void SetFeed(MoveType moveType)
        //{
        //    if(_feedDictionary == null)
        //        throw new NullReferenceException();

        //    var success = _feedDictionary.TryGetValue(moveType, out _currentFeed);
        //}

        internal void ActiveAsseC(bool active)
        {
            var g112 = new ActiveG112(this, active);

        }


        //internal void AddMoveCollection(MoveActionCollection moveList)
        //{
        //    throw new NotImplementedException();
        //}

        //internal double GetFeedValue(LinearMoveAction arcMoveAction)
        //{

        //    var feed = arcMoveAction.Feed;

        //    FeedDictionary.TryGetValue(arcMoveAction.MoveType, out feed);

        //    return feed;


        //}

        public void SetCambioUtensile(bool cambioUtensile)
        {
            var ops = Actions.OfType<ChangeToolAction>().FirstOrDefault();

            if(ops == null)return;

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
        public ActiveG112(ProgramPhase programPhase, bool active)
            : base(programPhase)
        {
            Activated = active;
            //WorkPlane = workPlane;
        }
    }
}