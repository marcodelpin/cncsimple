using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CncConvProg.Model
{
    [Serializable]
    public class UserInput
    {
       
        public UserInput()
        {
            //IsUserInputed = true;
        }
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
    }
}