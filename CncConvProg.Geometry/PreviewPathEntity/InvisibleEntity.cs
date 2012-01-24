﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.Geometry.PreviewPathEntity
{
    /// <summary>
    /// Quando nel programma c'è un elemento che non si può vedere nell'anteprima ma prende tempo.
    /// es. cambio tool, sosta , dwell , ecc..
    /// </summary>
    [Serializable]
    public class InvisibleEntity : IPreviewEntity
    {
        public double SecondsStop { get; set; }

        public double GetMoveLength
        {
            get { return 0; }
        }

        public bool IsRapidMovement
        {
            get { return false; }
        }

        public TimeSpan GetTimeSpan()
        {
            return TimeSpan.FromSeconds(SecondsStop);
        }
    }
}