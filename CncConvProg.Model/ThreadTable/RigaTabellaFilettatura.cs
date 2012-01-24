using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CncConvProg.Model.ThreadTable
{
    [Serializable]
    public abstract class RigaTabellaFilettatura : IEquatable<RigaTabellaFilettatura>
    {
        public abstract string Descrizione { get; }

        protected RigaTabellaFilettatura()
        {
            RigaGuid = Guid.NewGuid();
        }
        public Guid RigaGuid { get; set; }

        public double Preforo { get; set; }

        public double DiametroMetrico { get; set; }

        public double Passo { get; set; }

        public double PreforoRullare { get; set; }

        public int NumeroPassate { get; set; }

        public bool Equals(RigaTabellaFilettatura other)
        {
            return other.RigaGuid == RigaGuid;
        }

        private TipologiaFilettatura _parent;

        public void SetTipologiaParent(TipologiaFilettatura filettatura)
        {
            _parent = filettatura;
        }

        public double GetDiametroFinale(bool isEsterna)
        {
            if (_parent != null)
            {
                if (isEsterna)
                    return DiametroMetrico - (Passo * _parent.FattorePerDiametroEsterno);

                return Preforo + (Passo * _parent.FattorePerDiametroInterno);
            }

            return 0;
        }
    }

    [Serializable]
    public class RigaTabellaFilettaturaMetrica : RigaTabellaFilettatura
    {
        internal RigaTabellaFilettaturaMetrica()
        {

        }

        public override string Descrizione
        {
            get { return "M" + DiametroMetrico; }
        }
    }

    [Serializable]
    public class RigaTabellaFilettaturaInPollici : RigaTabellaFilettatura
    {
        internal RigaTabellaFilettaturaInPollici()
        {

        }

        public override string Descrizione
        {
            get { return "todoDescrizione"; }
        }
    }

    //[Serializable]
    //public class RigaTabellaFilettatura
    //{
    //    public double Preforo { get; set; }

    //    public double DiametroMetrico { get; set; }

    //    public double Passo { get; set; }

    //    public double PreforoRullare { get; set; }

    //    public int NumeroPassate { get; set; }

    //}


}