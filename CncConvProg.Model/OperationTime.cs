using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.Model
{
    [Serializable]
    public class OperazioneTime
    {
        public TimeSpan TempoTotale
        {
            get { return TempoLavoro + TempoRapido; }
        }

        public double CostoConsumoUtensile
        {
            get
            {
                return TempoLavoro.TotalMinutes * ConsumoUtensilePerMinuto;
            }
        }


        public TimeSpan TempoRapido { get; set; }

        public TimeSpan TempoLavoro { get; set; }

        public double DistanzaPercorsaLavoro { get; set; }

        public double DistanzaPercorsaRapido { get; set; }

        public int NumeroUtensile { get; set; }

        public double ConsumoUtensilePerMinuto { get; set; }

        public bool ForzaCambioUtensile { get; set; }

        public string DescrizioneUtensile { get; set; }
    }
}
