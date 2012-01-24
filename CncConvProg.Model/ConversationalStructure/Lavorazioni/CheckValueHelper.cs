using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni
{
    public static class CheckValueHelper
    {
        public static bool GreatherThanZero(IEnumerable<double> values)
        {
            var rslt = values.All(value => value > 0);
            return !rslt;
        }

        public static bool GreatherOrEqualZero(IEnumerable<double> values)
        {
            var rslt = values.All(value => value >= 0);
            return !rslt;
        }

        public static bool GreatherThan(IEnumerable<KeyValuePair<double, double>> valuePairs)
        {
            var rslt = valuePairs.All(keyValuePair => keyValuePair.Key > keyValuePair.Value);
            return !rslt;
        }
    }
}