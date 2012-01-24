using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.RawProfile2D
{
    /*
     * todo arrotondare a tot decimali il risultato delle operazioni trigonometriche.
     */
    [Serializable]
    public class RawLine2D : RawEntity2D
    {
        public RawLine2D(RawProfile parent)
            : base(parent)
        {
            Angle = new RawInput();
            X = new RawInput();
            Y = new RawInput();
            DeltaX = new RawInput();
            DeltaY = new RawInput();
            Chamfer = new RawInput();
            EndRadius = new RawInput();
        }


        public RawInput Angle { get; set; }

        public RawInput DeltaY { get; set; }

        public RawInput DeltaX { get; set; }

        public RawInput X { get; set; }

        public RawInput Y { get; set; }

        private IEntity2D _geometry;

        public override IEntity2D ResultGeometry()
        {
            return _geometry;
        }

        private void UpdateGeometry()
        {
            _geometry = null;

            if (IsDefined && _prevTeoricalPnt != null && _endPnt != null)
            {
                _geometry = new Line2D
                {
                    Start = new Point2D(_prevTeoricalPnt),
                    End = new Point2D(_endPnt),
                    PlotStyle = IsSelected ? EnumPlotStyle.SelectedElement : EnumPlotStyle.Element

                };

                // Set Plot Style
                if (PlotStyle == EnumPlotStyle.Invisible)
                    _geometry.PlotStyle = EnumPlotStyle.Invisible;
            }
        }

        public override RawEntityOrientation Orientation
        {
            get
            {
                var prev = _prevTeoricalPnt;

                if (IsDefined && _endPnt != null && prev != null)
                {
                    var angle = GetAngle(prev, _endPnt);

                    angle = GetPositiveAngle(angle);

                    angle = Math.Round(angle, 5);

                    var pi12 = Math.Round(Math.PI / 2, 5);
                    var pi = Math.Round(Math.PI, 5);
                    var pi34 = Math.Round(Math.PI + Math.PI / 2, 5);
                    var pi2 = Math.Round(Math.PI * 2, 5);


                    if (angle == 0 || angle == pi2)
                        return RawEntityOrientation.Dx;

                    if (angle > 0 && angle < pi12)
                        return RawEntityOrientation.No;

                    if (angle == pi12)
                        return RawEntityOrientation.Up;

                    if (angle > pi12 && angle < pi)
                        return RawEntityOrientation.Ne;

                    if (angle == pi)
                        return RawEntityOrientation.Sx;

                    if (angle > pi && angle < pi34)
                        return RawEntityOrientation.Se;

                    if (angle == pi34)
                        return RawEntityOrientation.Down;

                    if (angle > pi34 && angle < pi2)
                        return RawEntityOrientation.So;
                }

                return RawEntityOrientation.NotDefined;
            }
        }

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

            if (!Angle.IsUserInputed)
                Angle.SetValue(false, null);
        }

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

        /*
         * per ora posso lasciare stare troppe complicazioni
         */
        //public override bool SetFinalX(double value)
        //{
        //    /*
        //     * allora questo metodo , lo richiamo quando per esempio ho caso con linea definita xy e a ed ho la necessita 
        //     * di settare x precedente, se x è definita chiamo errore, se è incrementale so che dovro settarla al sio elemento 
        //     * precedente. se incorro in un problema restituisxco false, in segno di errore
        //     */

        //    if (DeltaX.IsUserInputed && DeltaX.Value.HasValue)
        //    {
        //        if (Profile != null)
        //        {
        //            var prevElement = Profile.GetPrev(this);

        //            if (prevElement == null) return false;

        //            var deltaX = DeltaX.Value.Value;

        //            return prevElement.SetFinalX(value - deltaX);

        //        }
        //    }

        //    if (!X.IsUserInputed && !X.Value.HasValue)
        //    {
        //        X.SetValue(false, value);
        //        return true;
        //    }

        //    return false;
        //}

        //public override bool SetFinalY(double value)
        //{
        //    if (DeltaY.IsUserInputed && DeltaY.Value.HasValue)
        //    {
        //        if (Profile != null)
        //        {
        //            var prevElement = Profile.GetPrev(this);

        //            if (prevElement == null) return false;

        //            var deltaY = DeltaY.Value.Value;

        //            return prevElement.SetFinalY(value - deltaY);

        //        }
        //    }

        //    if (!Y.IsUserInputed && !Y.Value.HasValue)
        //    {
        //        Y.SetValue(false, value);
        //        return true;
        //    }

        //    return false;

        //}


        #region Temporary Field

        private Point2D _endPnt = null;
        private Point2D _prevTeoricalPnt = null;

        #endregion

        /// <summary>
        /// Metodo che compila i valori mancanti e setta is defined
        /// Per ora mi accontento di una cosa brutale..
        /// </summary>
        /// <returns></returns>
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

            // Se X è inserita ok
            if (X.IsUserInputed && X.Value.HasValue)
            {
                endX = X.Value.Value;
            }
            else // Altrimenti provo a vedere se è settata quota incrementale
            {
                if (DeltaX.IsUserInputed && DeltaX.Value.HasValue)
                {

                    if (prevX != null)
                    {
                        endX = DeltaX.Value.Value + prevX;

                        X.SetValue(false, endX);
                    }
                }
                else
                {
                    //  X.SetValue(false, endX);

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
                else
                {
                    //     Y.SetValue(false, endY);
                }
            }

            /*
             * controllo punto precedente se è definito o meno
             */
            if (prevX.HasValue && prevY.HasValue)
            {
                _prevTeoricalPnt = new Point2D(prevX.Value, prevY.Value);
            }
            else
            {
                _prevTeoricalPnt = null;
            }

            /*
             * Ho valori temporanei del punto finale in XY 
             */

            if (endX.HasValue && endY.HasValue)
            {
                /*
                 * Ora controllo che non sia stato immesso angolo , in questo caso il punto finale sara
                 * risulatato del calcolo trigonometrico.
                 * 
                 * -Il punto precedente deve essere definito
                 * 
                 */
                if (Angle.IsUserInputed && Angle.Value.HasValue && _prevTeoricalPnt != null)
                {
                    var angle = GeometryHelper.DegreeToRadian(Angle.Value.Value);

                    // y deve essere definito ! ( x non lo è come precedentemente trovato

                    if (Y.IsUserInputed && Y.Value.HasValue ||
                        DeltaY.IsUserInputed && DeltaY.Value.HasValue)
                    {
                        // calcolo nuova X
                        endX = GeometryHelper.TrigonometricFindX(_prevTeoricalPnt, angle, endY.Value);

                        X.SetValue(false, endX);

                    }

                    else if (X.IsUserInputed && X.Value.HasValue ||
                             DeltaX.IsUserInputed && DeltaX.Value.HasValue)
                    {
                        // calcolo nuova Y 
                        endY = GeometryHelper.TrigonometricFindY(_prevTeoricalPnt, angle, endX.Value);

                        Y.SetValue(false, endY);

                    }

                    _endPnt = new Point2D(endX.Value, endY.Value);
                    IsDefined = true;
                }
                else
                {
                    if (_prevTeoricalPnt != null)
                    {
                        _endPnt = new Point2D(endX.Value, endY.Value);

                        var radian = GeometryHelper.GetAngle(_prevTeoricalPnt, _endPnt);
                        radian = GeometryHelper.GetPositiveAngle(radian);
                        Angle.SetValue(false, GeometryHelper.RadianToDegree(radian));

                    }
                }
            }
            else
            {
                _endPnt = null;
                IsDefined = false;
            }

            if (endX.HasValue && endY.HasValue)
            {
                if (!X.Value.HasValue)
                    X.SetValue(false, endX.Value);

                if (!Y.Value.HasValue)
                    Y.SetValue(false, endY.Value);

                _endPnt = new Point2D(endX.Value, endY.Value);



                IsDefined = true;

                if (_prevTeoricalPnt != null)
                {
                    if (_endPnt.Equals(_prevTeoricalPnt))
                        IsDefined = false;
                }
            }
            else
            {
                _endPnt = null;
                IsDefined = false;
            }

            UpdateGeometry();
            // todo : implementare anche angolo, se c'è angolo setto coordinata precedente. 

            return IsDefined;
        }


        private static double GetAverageAngle(double angle1, double angle2)
        {
            //if (angle2 == 0)
            //    angle2 = Math.PI;

            var x = Math.Cos(angle1);
            var y = Math.Sin(angle1);

            x += Math.Cos(angle2);
            y += Math.Sin(angle2);

            var angle = Math.Atan2(y, x);

            //if (angle < 0)
            //    angle = (Math.PI * 2) - Math.Abs(angle);

            return angle;
        }

        private static double GetPositiveAngle(double radian)
        {
            if (radian < 0)
                radian = (Math.PI * 2) - Math.Abs(radian);

            return radian;
        }

        private static double GetAngleDifference(double angle1, double angle2)
        {
            var rslt1 = GetPositiveAngle(angle1);
            var rslt2 = GetPositiveAngle(angle2);

            if (rslt2 < rslt1)
                rslt2 += 2 * Math.PI;

            return rslt2 - rslt1;
        }

        private static Point2D CalcCenterRadius(Point2D p1, Point2D p2, Point2D p3, double radius)
        {
            // Angoli li misuro dal punto teorico all'altra estremità
            var angle1 = GetAngle(p2, p1);
            var angle2 = GetAngle(p2, p3);

            var difference = GetAngleDifference(angle1, angle2);

            var angleResult = GetAverageAngle(angle1, angle2);

            var angoloDelRaccordo = Math.PI - Math.Abs(difference);
            // ottengo la distanza fra il punto d'incrocio teorico e il centro del raccordo
            // come angolo mi serve meta angolo arco 
            var distance = 1 / (Math.Cos(angoloDelRaccordo / 2));

            distance *= radius;

            var deltaPnt = GetPointAtAngleAndDistance(Math.Abs(distance), angleResult);

            deltaPnt.X += p2.X;
            deltaPnt.Y += p2.Y;

            return deltaPnt;
        }

        private static Point2D GetPointAtAngleAndDistance(double distance, double angle)
        {
            var pnt = new Point2D
                          {
                              Y = Math.Sin(angle) * distance,
                              X = Math.Cos(angle) * distance
                          };

            return pnt;

        }

        //private static double GetAngle(Line2D line1)
        //{
        //    var start = line1.Start;
        //    var end = line1.End;

        //    return GetAngle(start, end);
        //}

        /// <summary>
        /// Mi deve restituire valore da 0 a 2Pi, non negativi
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static double GetAngle(Point2D p1, Point2D p2)
        {
            var rslt = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);

            //if (rslt < 0)
            //    rslt = (Math.PI * 2) - Math.Abs(rslt);

            return rslt;

        }
    }
}
