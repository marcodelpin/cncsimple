using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Geometry.RawProfile2D
{
    [Serializable]
    public abstract class RawEntity2D
    {
        public enum RawEntityOrientation
        {
            NotDefined,
            InitPoint,
            ArcCw,
            ArcCcw,
            Up,
            Ne,
            Dx,
            Se,
            Down,
            So,
            Sx,
            No,
        }

        protected RawEntity2D(RawProfile parent)
        {
            Profile = parent;
        }

        // 1° proprietà isdefined che mi dice se la posizione è definita o meno..
        // 2° devo riuscire a mandare un avviso di quando ci sono proprietà in conflitto
        //
        public abstract IEntity2D ResultGeometry();

        public RawProfile Profile { get; set; }

        public abstract RawEntityOrientation Orientation { get; }
        public RawInput Chamfer { get; set; }
        public RawInput EndRadius { get; set; }

        public bool IsSelected { get; set; }

        /// <summary>
        /// Flag che segnala se l'elemento è definito o meno
        /// </summary>
        public bool IsDefined { get; set; }

        protected abstract void Reset();

        public abstract double? GetFinalX();
        public abstract double? GetFinalY();

        public EnumPlotStyle PlotStyle { get; set; }
        //public abstract bool SetFinalX(double value);
        //public abstract bool SetFinalY(double value);


        internal abstract bool SolveElement();


        //internal void TrySolve(RawEntity2D rawEntity)
        //{
        //    throw new NotImplementedException();
        //}

        //internal abstract void Update();
    }
}

