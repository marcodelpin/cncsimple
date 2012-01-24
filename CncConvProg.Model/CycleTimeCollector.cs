using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.Model
{
    /// <summary>
    /// Classe utilizzata per memorizzare le statistiche della lavorazione.
    /// </summary>
    [Serializable]
    public class CycleTimeCollector
    {
        public CycleTimeCollector()
        {
            FasiDiLavoroTime = new List<FaseDiLavoroTime>();
        }

        public int NumeroUtensiliUtilizzati { get; set; }

        public TimeSpan TempoTotaleMacchinaSingoloPezzo { get; set; }

        public TimeSpan TempoTotaleLotto { get; set; }

        public TimeSpan TempoTotaleCaricamentoPezzi { get; set; }

        public TimeSpan TempoTotaleProgrammazione { get; set; }

        public TimeSpan TempoTotaleSetupMacchina { get; set; }

        public TimeSpan TempoTotaleRapido { get; set; }

        public TimeSpan TempoTotaleCambioUtensile { get; set; }

        public TimeSpan TempoTotaleLavorazione { get; set; }

        public List<FaseDiLavoroTime> FasiDiLavoroTime { get; set; }

        public int StockQuantity { get; set; }


        /*
         * 
         */

        public void Add(FaseDiLavoroTime faseTimeCollector)
        {
            FasiDiLavoroTime.Add(faseTimeCollector);
        }
    }

    [Serializable]
    public class FaseDiLavoroTime
    {
        public double CostoOrarioMacchina { get; set;}
        public int NumeroOperazioni { get; set; }

        public int MinutiProgrammazioneOperazione { get; set; }

        public FaseDiLavoroTime()
        {
            OperationsTime = new List<OperazioneTime>();
        }

        public List<OperazioneTime> OperationsTime { get; set; }


        public void Add(OperazioneTime opTime)
        {
            OperationsTime.Add(opTime);
        }

        public int GetUsedToolCount()
        {
            var l = GetListaPostazioniUtensili();
            return l.Count;

        }

        /// <summary>
        /// Da implementare correttamente
        /// </summary>
        /// <returns></returns>
        public int GetChangeToolNumber()
        {
            var counter = 0;
            var prevToolNumber = -1;

            foreach (var operazioneTime in OperationsTime)
            {

                var currentToolNumber = operazioneTime.NumeroUtensile;

                if (currentToolNumber != prevToolNumber || operazioneTime.ForzaCambioUtensile)
                {
                    counter++;
                }

                prevToolNumber = currentToolNumber;
            }

            return counter;
        }

        public List<int> GetListaPostazioniUtensili()
        {
            var toolNumber = new List<int>();

            foreach (var operazioneTime in OperationsTime)
            {
                var toolPos = operazioneTime.NumeroUtensile;

                if (!toolNumber.Contains(toolPos))
                    toolNumber.Add(toolPos);
            }

            return toolNumber;
        }

        public TimeSpan GetTotalTimeFromToolNumber(int i)
        {
            var rslt = new TimeSpan();

            foreach (var operazioneTime in OperationsTime)
            {
                if (operazioneTime.NumeroUtensile == i)
                {
                    rslt = rslt + operazioneTime.TempoTotale;
                }

            }
            return rslt;
        }

        public double GetTotalToolWearFromToolNumber(int i)
        {
            var rslt = 0.0d;

            foreach (var operazioneTime in OperationsTime)
            {
                if (operazioneTime.NumeroUtensile == i)
                {
                    rslt += operazioneTime.CostoConsumoUtensile;
                }
            }

            return rslt;
        }

        public int MinutiMontaggioUtensile { get; set; }

        public int MinutiPreparazioneStaffaggio { get; set; }

        public int SecondiCaricamentoMaterialeGrezzo { get; set; }

        public TimeSpan GetTotalMachinigTime()
        {
            var t = new TimeSpan();

            foreach (var operazioneTime in OperationsTime)
            {
                t = t + operazioneTime.TempoTotale;
            }
            return t;
        }

        public int TempoCambioUtensile { get; set; }
    }

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
    }


}
