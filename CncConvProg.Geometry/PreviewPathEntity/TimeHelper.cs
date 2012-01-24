using System;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.PreviewPathEntity
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
    }
}