using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.RawProfile2D
{
    [Serializable]
    public class RawInitPoint2D : RawEntity2D
    {
        public RawInitPoint2D(RawProfile parent)
            : base(parent)
        {
            X = new RawInput();
            Y = new RawInput();
        }

        public RawInput X { get; set; }

        public RawInput Y { get; set; }

        public override IEntity2D ResultGeometry()
        {
            return null;
        }

        protected override void Reset()
        {
            if (!X.IsUserInputed)
                X.SetValue(false, null);

            if (!Y.IsUserInputed)
                Y.SetValue(false, null);
        }

        public override double? GetFinalX()
        {
            return X.Value;
        }

        public override double? GetFinalY()
        {
            return Y.Value;
        }

        //public override bool SetFinalX(double value)
        //{
        //    X.SetValue(false, value);

        //    return true;
        //}

        //public override bool SetFinalY(double value)
        //{
        //    Y.SetValue(false, value);
        //    return true;
        //}

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <returns></returns>
        //public override Point2D GetTeoricalFinalPnt()
        //{
        //    var teoricalPnt = new Point2D();

        //    if (X.IsUserInputed && Y.IsUserInputed && X.Value.HasValue && Y.Value.HasValue)
        //    {
        //        teoricalPnt.X = X.Value.Value;
        //        teoricalPnt.Y = Y.Value.Value;

        //        return teoricalPnt;
        //    }

        //    IsDefined = false;

        //    return null;
        //}

        #region Temporary Field

        // private Point2D _tempTeoricaFinalPnt = null;

        #endregion

        internal override bool SolveElement()
        {
            if (X.Value.HasValue && Y.Value.HasValue)
            {
                IsDefined = true;
            }
            else
            {
                IsDefined = false;
            }

            return IsDefined;
        }

        public override RawEntityOrientation Orientation
        {
            get { return RawEntityOrientation.InitPoint; }
        }
    }
}
