using System;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.PreviewPathEntity
{
    internal static class TimeHelper
    {

        /// <summary>
        /// Calcola tempo di spostamento lineare.
        /// Assumo che avanzamento è espresso in unitaSpazio / unitaTempo
        /// </summary>
        /// <param name="line3D"></param>
        /// <param name="feed"></param>
        /// <returns></returns>
        internal static TimeSpan CalcTime(Line3D line3D, double feed)
        {
            var distance = line3D.GetLength();

            if (feed == 0) return new TimeSpan();

            var minuti = distance / feed;

            return TimeSpan.FromMinutes(minuti);

        }


        /// <summary>
        /// todo _ Testare questo metodo con varibili varie
        /// </summary>
        /// <param name="arc3D"></param>
        /// <param name="feed"></param>
        /// <returns></returns>
        internal static TimeSpan CalcTime(Arc3D arc3D, double feed)
        {
            var lunghezza = arc3D.GetLength();

            if (feed == 0) return new TimeSpan();

            var minuti = lunghezza / feed;

            return TimeSpan.FromMinutes(minuti);

        }

        /// <summary>
        /// Calcola movimento di sfacciatura con movimento accelerato 
        /// </summary>
        public static double CalcAcceleratedVerticalMove(double diaIni, double diaFin, double speed, double feed, double maxSpeed, MeasureUnit measureUnit)
        {
            if (feed <= 0 || speed <= 0) return 0;

            var diaMax = Math.Max(diaIni, diaFin);

            var diaMin = Math.Min(diaFin, diaIni);

            const float incremento = 0.1f;

            double time = 0.0d;

            for (var i = diaMax; i > diaMin; i -= incremento)
            {
                // calcolo Avanzamento in questo punto 
                var nGiri = FeedAndSpeedHelper.GetNumeroGiri(speed, i, measureUnit,maxSpeed);
                if (maxSpeed > 0 && nGiri > maxSpeed)
                    nGiri = maxSpeed;

                var feedCalc = nGiri * feed;

                time += (incremento / 2) / feedCalc;
            }

            time += 0.01f; // ?? compenso errore rilevato con dati sperimentali di 0.6 sec. per passata

            return time;
        }

        /// <summary>
        /// Il limitatore giri funziona solo se è > di 0
        /// </summary>
        /// <param name="l"></param>
        /// <param name="speed"></param>
        /// <param name="feed"></param>
        /// <param name="measureUnit"></param>
        /// <param name="limitGiri"></param>
        /// <returns></returns>
        internal static double CalcHorizontalTime(PreviewLine3D l, double speed, double feed, MeasureUnit measureUnit, double limitGiri)
        {
            var d = l.Start.Y;

            var nGiri = FeedAndSpeedHelper.GetNumeroGiri(speed, d, measureUnit);
            if (limitGiri > 0 && nGiri > limitGiri)
                nGiri = limitGiri;

            var feedCalc = nGiri * feed;

            return (l.GetMoveLength()) / feedCalc;
        }
    }
}