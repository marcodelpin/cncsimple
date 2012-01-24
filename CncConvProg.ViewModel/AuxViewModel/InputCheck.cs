using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.ViewModel.AuxViewModel
{
    public static class InputCheck
    {
        // todo implementarlo in classe utility
        public static string MaggioreDiZero(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Must be greater than zero";

            var v = Convert.ToDouble(value);

            return MaggioreDiZero(v);
        }

        public static string MaggioreDiZero(double? value)
        {
            if (!value.HasValue || value <= 0)
                return "Must be greater than zero";

            return null;
        }


        public static string MaggioreOUgualeDiZero(double? value)
        {
            if (!value.HasValue || value < 0)
                return "Must be greather or equal zero";

            return null;
        }
    }
}
