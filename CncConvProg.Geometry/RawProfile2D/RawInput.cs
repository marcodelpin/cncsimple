using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.Geometry.RawProfile2D
{
    [Serializable]
    public class RawInput
    {
        public bool IsInConflict { get; set; }

        public bool IsValueCorrect
        {
            get { return IsUserInputed && Value.HasValue; }
        }

        public bool IsUserInputed { get; set; }
        public double? Value { get; set; }

        public void SetValue(bool setUserInputed, double? value)
        {
            IsUserInputed = setUserInputed;
            Value = value;
        }

        public void SetValue(double? value)
        {
            IsUserInputed = true;
            Value = value;
        }
    }
}
