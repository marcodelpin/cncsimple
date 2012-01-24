using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CncConvProg.Model.ThreadTable
{
    [Serializable]
    public abstract class TipologiaFilettatura
    {
        public string Descrizione { get; set; }

        public List<RigaTabellaFilettatura> RigheTabella = new List<RigaTabellaFilettatura>();

        public double FattorePerDiametroEsterno { get; set; }

        public double FattorePerDiametroInterno { get; set; }

        public abstract RigaTabellaFilettatura CreateRigaTabella();

  

        /*
         * per trovare diametro di arrivo per esterno so che devo fare :
         * 
         * se proprio bisogna fare override con valori immessi da utente..
         * 
         * diametroEst -   ((passo / fattore ) *2 )
         * 
         * 1.65 est
         * 
         * 1.15 int 
         * 
         */
    }

    [Serializable]
    public class TipologiaMetrica : TipologiaFilettatura
    {
        public override RigaTabellaFilettatura CreateRigaTabella()
        {
            return new RigaTabellaFilettaturaMetrica();
        }
    }

    [Serializable]
    public class TipologiaInPollici : TipologiaFilettatura
    {
        public override RigaTabellaFilettatura CreateRigaTabella()
        {
            return new RigaTabellaFilettaturaInPollici();
        }
    }
}