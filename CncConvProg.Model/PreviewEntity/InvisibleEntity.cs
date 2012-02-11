using System;
using CncConvProg.Model.PathGenerator;

namespace CncConvProg.Model.PreviewEntity
{
    /// <summary>
    /// Quando nel programma c'è un elemento che non si può vedere nell'anteprima ma prende tempo.
    /// es. cambio tool, sosta , dwell , ecc..
    /// </summary>
    [Serializable]
    public class InvisibleEntity : IPreviewEntity
    {
        /// <summary>
        /// Secondi
        /// </summary>
        public double TimeStop { get; set; }

        public ParametroVelocita ParametroVelocita
        {
            get;
            set;
        }

        public double GetMoveLength()
        {
             return 0; 
        }

        public bool IsRapidMovement
        {
            get { return false; }
        }

        public TimeSpan GetTimeSpan()
        {
            return TimeSpan.FromSeconds(TimeStop);
        }
    }
}
