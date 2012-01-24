using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CncConvProg.Model.ThreadTable
{
    /*
     * Questa sarà la classe che andrà serializzata su file 
     */
    [Serializable]
    public class TabellaFilettature
    {
        public List<TipologiaFilettatura> Filettature { get; set; }


        /// <summary>
        /// Ctor
        /// </summary>
        public TabellaFilettature()
        {
            Filettature = new List<TipologiaFilettatura>();

            /*
             * Inserisco un pò di valori di default
             */

            AddDefaultValue();
        }

        private void AddDefaultValue()
        {
            var metrica = new TipologiaMetrica()
                              {
                                  Descrizione = "Metric",
                                  FattorePerDiametroInterno = 1.2,
                                  FattorePerDiametroEsterno = 1.2
                              };

            Filettature.Add(metrica);

            AddMetricRow(metrica, 3, .5, 2.5);
            AddMetricRow(metrica, 3.5, .6, 2.9);
            AddMetricRow(metrica, 4, .7, 3.3);
            AddMetricRow(metrica, 5, .8, 4.2);
            AddMetricRow(metrica, 6, 1, 5);
            AddMetricRow(metrica, 8, 1.25, 6.8);
            AddMetricRow(metrica, 10, 1.5, 8.5);

            AddMetricRow(metrica, 12, 1.75, 10.2);
            AddMetricRow(metrica, 14, 2, 12);
            AddMetricRow(metrica, 16, 2, 14);
            AddMetricRow(metrica, 18, 2.5, 15.5);
            AddMetricRow(metrica, 20, 2.5, 17.5);
            AddMetricRow(metrica, 22, 2.5, 19.5);
            AddMetricRow(metrica, 24, 3, 21);
            AddMetricRow(metrica, 27, 3, 24);
            AddMetricRow(metrica, 30, 3.5, 26.5);
            AddMetricRow(metrica, 33, 3.5, 29.5);
            AddMetricRow(metrica, 39, 4, 35);

            AddMetricRow(metrica, 42, 4.5, 37.5);
            AddMetricRow(metrica, 45, 4.5, 40.5);
            AddMetricRow(metrica, 48, 5);
            AddMetricRow(metrica, 52, 5);
            AddMetricRow(metrica, 56, 5.5);
            AddMetricRow(metrica, 60, 5.5);
            AddMetricRow(metrica, 64, 6);
            AddMetricRow(metrica, 68, 6);

            var withworth = new TipologiaInPollici()
            {
                Descrizione = "Whitworth ",
                FattorePerDiametroInterno = 1.2,
                FattorePerDiametroEsterno = 1.2
            };

        }

        private static void AddMetricRow(TipologiaMetrica metrica, double diameter, double pitch, double tapDrill = 0)
        {
            if (tapDrill == 0)
                tapDrill = diameter - pitch;

            var metricRow = metrica.CreateRigaTabella();
            metricRow.DiametroMetrico = diameter;
            metricRow.Passo = pitch;
            metricRow.Preforo = tapDrill;

            metrica.RigheTabella.Add(metricRow);
        }

        private static void AddInchTypeRow(TipologiaInPollici inPollici, string descrizione, double diameter, double pitch, double tapDrill, double tpi)
        {
            var metricRow = inPollici.CreateRigaTabella();
            metricRow.DiametroMetrico = diameter;
            metricRow.Passo = pitch;
            metricRow.Preforo = tapDrill;

            inPollici.RigheTabella.Add(metricRow);
        }
    }
}