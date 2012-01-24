using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CncConvProg.Model
{
    public static class FeedAndSpeedHelper
    {
        /// <summary>
        /// Return RPM
        /// </summary>
        /// <param name="velocitaTaglio">Mt/Min -- Foot/Min</param>
        /// <param name="diametro">mm -- inch</param>
        /// <param name="measureUnit">Unit</param>
        /// <param name="limitGiri"></param>
        /// <param name="decimalRounding"></param>
        /// <returns></returns>
        public static double GetNumeroGiri(double velocitaTaglio, double diametro, MeasureUnit measureUnit, double limitGiri = 0, int decimalRounding = 4)
        {
            var nGiri = 0.0d;
            switch (measureUnit)
            {
                case MeasureUnit.Millimeter:
                    {
                        // diametro in mm
                        // vt in mt/min
                        nGiri = (velocitaTaglio * 1000) / (Math.PI * diametro);

                    } break;

                case MeasureUnit.Inch:
                    {
                        // diametro in inch
                        // vt in foot/min 

                        nGiri = velocitaTaglio / (Math.PI * diametro * 1 / 12);

                    } break;
            }

            if (limitGiri > 0 && nGiri > limitGiri)
                return limitGiri;

            return Math.Round(nGiri, decimalRounding);
        }

        public static double GetVelocitaTaglio(double numeroGiri, double diametro, MeasureUnit measureUnit, int decimalRound = 4)
        {
            switch (measureUnit)
            {
                case MeasureUnit.Millimeter:
                    {
                        var velocitaTaglio = (numeroGiri * diametro * Math.PI) / 1000;

                        return Math.Round(velocitaTaglio, decimalRound);

                    }
                    break;

                case MeasureUnit.Inch:
                    {
                        var velocitaTaglio = (numeroGiri * diametro * Math.PI * 1 / 12);

                        return Math.Round(velocitaTaglio, decimalRound);
                    }
                    break;
            }

            throw new NotImplementedException();
        }

        public static double GetFeedAsync(double feedSync, double nGiri, int decimalRounding = 4)
        {
            return Math.Round(feedSync * nGiri, decimalRounding);
        }

        public static double GetFeedSync(double feedAsync, double nGiri, int decimalRounding = 4)
        {
            if (nGiri != 0)
                return Math.Round(feedAsync / nGiri, decimalRounding);

            return 0;
        }

        private const double KgToLbK = 2.20462262185;

        private const double PesoSpecificoDmToInch = 0.036;

        public static double KgToLb(double kg)
        {
            return kg / KgToLbK;
        }
        public static double LbToKg(double lb)
        {
            return lb * KgToLbK;
        }

        public static double DensityDmToInch(double dm)
        {
            return Math.Round(dm * PesoSpecificoDmToInch, 5);
        }

        public static double DensityInchToDm(double lb)
        {
            return Math.Round(lb / PesoSpecificoDmToInch, 5);
        }

        private const double FeetToMeter = 3.48;

        internal static double GetInchSpeedCut(double velMeter)
        {
            return velMeter * FeetToMeter;
        }

        private const double InchToMillimeter = 25.4;

        internal static double GetInchFromMm(double mm)
        {
            return Math.Round(mm / InchToMillimeter, 5);
        }
    }
}