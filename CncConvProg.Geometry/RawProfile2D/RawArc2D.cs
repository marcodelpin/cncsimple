using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.RawProfile2D
{
    [Serializable]
    public class RawArc2D : RawEntity2D
    {
        /*
         * todo : check ! anche qui mi sembra il comportamento di cw ccw sia al rovescio..
         */
        public RawArc2D(RawProfile parent)
            : base(parent)
        {
            X = new RawInput();
            Y = new RawInput();
            CenterX = new RawInput();
            CenterY = new RawInput();
            DeltaX = new RawInput();
            DeltaY = new RawInput();
            Chamfer = new RawInput();
            EndRadius = new RawInput();
            Radius = new RawInput();

        }

        public bool IsClockwise { get; set; }

        public bool AlternateArc { get; set; }

        public RawInput Radius { get; set; }

        public RawInput DeltaY { get; set; }

        public RawInput DeltaX { get; set; }

        public RawInput X { get; set; }

        public RawInput Y { get; set; }

        public RawInput CenterX { get; set; }

        public RawInput CenterY { get; set; }


        private void UpdateGeometry()
        {
            /*
             * se centro xy non è definito..
             */
            _geometry = null;

            /*
             * qui mi devo basare sui punti che ho 
             * non devo fare calcoli ulteriori , se defined procedo , altrimenti mi attcco
             */


            if (IsDefined && _prevTeoricalPnt != null && _endPnt != null)
            {
                if (!X.Value.HasValue || !Y.Value.HasValue || !Radius.Value.HasValue) return;

                var arc = new Arc2D
                {
                    End = new Point2D(X.Value.Value, Y.Value.Value),

                    Radius = Radius.Value.Value,

                    Start = new Point2D(_prevTeoricalPnt),

                    ClockWise = IsClockwise,


                };

                if (CenterX.Value.HasValue)
                    arc.Center.X = CenterX.Value.Value;

                if (CenterY.Value.HasValue)
                    arc.Center.Y = CenterY.Value.Value;

                arc.PlotStyle = IsSelected ? EnumPlotStyle.SelectedElement : EnumPlotStyle.Element;

                _geometry = arc;

                // Set Plot Style
                if (PlotStyle == EnumPlotStyle.Invisible)
                    _geometry.PlotStyle = EnumPlotStyle.Invisible;

            }


        }
        private IEntity2D _geometry;

        public override IEntity2D ResultGeometry()
        {
            return _geometry;
        }

        public override RawEntityOrientation Orientation
        {
            get
            {
                if (IsDefined && _endPnt != null)
                {
                    return IsClockwise ? RawEntityOrientation.ArcCw : RawEntityOrientation.ArcCcw;
                }

                return RawEntityOrientation.NotDefined;
            }
        }

        //#region Temporary Field

        //private Point2D _tempTeoricaFinalPnt = null;
        //private Point2D _prevTeoricalPnt = null;

        //#endregion
        protected override void Reset()
        {
            // resetto i valori non UserInputed
            if (!X.IsUserInputed)
                X.SetValue(false, null);

            if (!DeltaX.IsUserInputed)
                DeltaX.SetValue(false, null);

            if (!Y.IsUserInputed)
                Y.SetValue(false, null);

            if (!DeltaY.IsUserInputed)
                DeltaY.SetValue(false, null);

            if (!CenterX.IsUserInputed)
                CenterX.SetValue(false, null);

            if (!CenterY.IsUserInputed)
                CenterY.SetValue(false, null);

            if (!Radius.IsUserInputed)
                Radius.SetValue(false, null);
        }

        #region Temporary Field

        private Point2D _endPnt = null;
        private Point2D _prevTeoricalPnt = null;

        #endregion

        public override double? GetFinalX()
        {
            if (_endPnt != null)
                return _endPnt.X;

            return null;
        }

        public override double? GetFinalY()
        {
            if (_endPnt != null)
                return _endPnt.Y;

            return null;
        }

        //public override bool SetFinalX(double value)
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool SetFinalY(double value)
        //{
        //    throw new NotImplementedException();
        //}

        internal override bool SolveElement()
        {
            Reset();
            // update direction enum
            // update isdefined.

            if (Profile == null) return false;

            double? endX = null;
            double? endY = null;

            var prevMove = Profile.GetPrev(this);

            double? prevX = null, prevY = null;


            if (prevMove != null)
            {
                endY = prevY = prevMove.GetFinalY();
                endX = prevX = prevMove.GetFinalX();
            }

            /*
             * Definisco punto finale.
             */
            if (X.IsUserInputed && X.Value.HasValue)
            {
                endX = X.Value.Value;
            }
            else
            {
                if (DeltaX.IsUserInputed && DeltaX.Value.HasValue)
                {

                    if (prevX != null)
                    {
                        endX = DeltaX.Value.Value + prevX;

                        X.SetValue(false, endX);
                    }
                }

                else if (endX != null)
                {
                    X.SetValue(false, endX);
                }
            }

            if (Y.IsUserInputed && Y.Value.HasValue)
            {
                endY = Y.Value.Value;
            }
            else
            {
                if (DeltaY.IsUserInputed && DeltaY.Value.HasValue)
                {

                    if (prevY != null)
                    {
                        endY = DeltaY.Value.Value + prevY;
                        Y.SetValue(false, endY);

                    }
                }

                else if (endY != null)
                {
                    Y.SetValue(false, endY);
                }
            }

            if (endX.HasValue && endY.HasValue)
            {
                _endPnt = new Point2D(endX.Value, endY.Value);
            }
            else
            {
                _endPnt = null;
            }


            if (prevX.HasValue && prevY.HasValue)
            {
                _prevTeoricalPnt = new Point2D(prevX.Value, prevY.Value);
            }
            else
            {
                _prevTeoricalPnt = null;
            }
            /* Fino qui ho punto iniziale e punto finale ,
                continuo con la definizione dell'elemento ..*/
            /*
                 * Definito punto finale posso avere :
             *  1 ) raggio e non centro definito
             *  2 )centro e non raggio definito
             *  3) centro x non cy e raggio 
             *  4) centro y non cx e raggio ""
             *  5)raggio e centro (probabile overdefined )
             */
            if (_endPnt != null && _prevTeoricalPnt != null && !_endPnt.Equals(_prevTeoricalPnt))
            {
                // 1 ) raggio e non centro definito
                if (!CenterX.Value.HasValue && !CenterY.Value.HasValue && Radius.Value.HasValue)
                {
                    var center = GeometryHelper.GetCenterFromTwoPointsAndRadius(_prevTeoricalPnt, _endPnt, Radius.Value.Value,
                                                                                AlternateArc);

                    CenterX.SetValue(false, center.X);
                    CenterY.SetValue(false, center.Y);
                }

                // 2) centro definito e non raggio definito
                else if (CenterX.Value.HasValue && CenterY.Value.HasValue && !Radius.Value.HasValue)
                {
                    var raggio = GeometryHelper.Distance(new Point2D(CenterX.Value.Value, CenterY.Value.Value), _endPnt);
                    if (raggio != null)
                        Radius.SetValue(false, raggio.Value);
                }
                // *  3) centro x non cy e raggio 
                else if (CenterX.Value.HasValue && !CenterY.Value.HasValue && Radius.Value.HasValue)
                {
                    /* Qui provo a cambiare alternate arc , ovvero se ho una quota ( x o y ) del centro del arco prendo il valore di alternate arc che coincide */

                    var center1 = GeometryHelper.GetCenterFromTwoPointsAndRadius(_prevTeoricalPnt, _endPnt, Radius.Value.Value,
                                                                         false);

                    var center2 = GeometryHelper.GetCenterFromTwoPointsAndRadius(_prevTeoricalPnt, _endPnt, Radius.Value.Value,
                                                                         true);

                    if (center1.X == CenterX.Value.Value)
                    {
                        AlternateArc = false;

                        CenterY.SetValue(false, center1.Y);
                    }
                    else if (center2.X == CenterX.Value.Value)
                    {
                        AlternateArc = true;

                        CenterY.SetValue(false, center2.Y);
                    }
                }

                // *  4) centro y non cx e raggio ""
                else if (!CenterX.Value.HasValue && CenterY.Value.HasValue && Radius.Value.HasValue)
                {
                    /* Qui provo a cambiare alternate arc , ovvero se ho una quota ( x o y ) del centro del arco prendo il valore di alternate arc che coincide */

                    var center1 = GeometryHelper.GetCenterFromTwoPointsAndRadius(_prevTeoricalPnt, _endPnt, Radius.Value.Value,
                                                                         false);

                    var center2 = GeometryHelper.GetCenterFromTwoPointsAndRadius(_prevTeoricalPnt, _endPnt, Radius.Value.Value,
                                                                         true);

                    if (center1.Y == CenterY.Value.Value)
                    {
                        AlternateArc = false;

                        CenterX.SetValue(false, center1.X);
                    }
                    else if (center2.Y == CenterY.Value.Value)
                    {
                        AlternateArc = true;

                        CenterX.SetValue(false, center2.X);
                    }
                }

            }

            /*
             * Ora al termine della definizione dell'elemento se ho tutti i dati necessari lo setto come definito , altrimento non è definito
             */
            if (Radius.Value.HasValue && CenterX.Value.HasValue && CenterY.Value.HasValue && X.Value.HasValue && Y.Value.HasValue)
            {

                var valoreRaggioCorretto = GeometryHelper.ValutateRadiusCorrectness(Radius.Value.Value,
                                                                                    CenterX.Value.Value,
                                                                                    CenterY.Value.Value,
                                                                                    X.Value.Value,
                                                                                    Y.Value.Value);
                if (valoreRaggioCorretto)
                    IsDefined = true;
                else
                {
                    IsDefined = false;
                }
            }
            else
            {
                IsDefined = false;
            }

            /*
             * Per essere definito devo avere raggio (corretto ), centro, Piniziale e Pfinale
             */
            /*
             * Controllo se arco è definito o meno 
             * Devo avere 
             * - raggio 
             * - centro 
             * - punto Iniziale 
             * - punto finale
             */



            UpdateGeometry();
            // todo : implementare raggio, se raggio non è diametro corretto non definire.


            return IsDefined;
        }
    }
}

