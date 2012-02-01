using System;
using System.Collections.Generic;
using System.Linq;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.Simulation;

namespace CncConvProg.Model.ConversationalStructure.Abstraction
{
    [Serializable]
    public abstract class FaseDiLavoro : IEquatable<FaseDiLavoro>
    {

        /*
         * le origini le metto qui nella fase di lavoro,
         * 
         * Devo mettere nella fase per centro di lavoro tutte le bool come qui, in fase per centro posso avere più origini
         * 
         * per il tornio devo mettere radioButton , perchè ne posso scegliere solamente 1
         */

        public bool OriginG54 { get; set; }
        public bool OriginG55 { get; set; }
        public bool OriginG56 { get; set; }
        public bool OriginG57 { get; set; }
        public bool OriginG58 { get; set; }
        public bool OriginG59 { get; set; }

        public Guid FaseDiLavoroGuid { get; private set; }

        public int ProgramNumber { get; set; }

        public Guid MachineGuid { get; set; }

        public string ProgrammaNc { get; set; }

        public string Descrizione { get; set; }

        public CutViewerStock Stock { get; set; }

        public DateTime CreationTime { get; set; }

        protected FaseDiLavoro()
        {
            FaseDiLavoroGuid = Guid.NewGuid();

            Stock = new CutViewerStock();

            Descrizione = "PROGRAM NAME";

            ProgramNumber = 100;
        }

        public bool Equals(FaseDiLavoro other)
        {
            return other.FaseDiLavoroGuid == FaseDiLavoroGuid;
        }

        ///// <summary>
        ///// Restituisce porta utensile.
        ///// Non lo implemento nella classe macchina perchè potrebbe non essere selezionata
        ///// </summary>
        ///// <returns></returns>
        //internal abstract Tool.ToolHolder GetToolHolder();

        public bool IsValid { get; set; }

        public IEnumerable<Lavorazione> Lavorazioni
        {
            get
            {
                return Singleton.Instance.GetLavorazioni(FaseDiLavoroGuid);
            }
        }
        public void UpdateValidFlag()
        {
            IsValid = Lavorazioni.All(p => p.IsValid);
        }

        public ToolMachine.ToolMachine GetMacchina()
        {
            var machines = PathFolderHelper.GetToolMachines();

            return machines.Where(m => m.MachineGuid == MachineGuid).FirstOrDefault();
        }

        public double NoChangeToolSecureZ { get; set; }

        public abstract TipoFaseLavoro TipoFase { get; }
        public enum TipoFaseLavoro
        {
            Centro,
            Tornio2Assi,
            Tornio3Assi
        }

        //public Guid FaseDiLavoroGuid { get; private set; }

        //public Guid MachineGuid { get; set; }

        public double LimitatoreTornio { get; set; }

        //public DateTime CreationTime { get; set; }

        //public string Descrizione { get; set; }

        //protected FaseDiLavoro()
        //{
        //    FaseDiLavoroGuid = Guid.NewGuid();
        //    IsValid = true;
        //}

        //public bool Equals(FaseDiLavoro other)
        //{
        //    return other.FaseDiLavoroGuid == FaseDiLavoroGuid;
        //}

        ///// <summary>
        ///// Restituisce porta utensile.
        ///// Non lo implemento nella classe macchina perchè potrebbe non essere selezionata
        ///// </summary>
        ///// <returns></returns>

        //public bool IsValid { get; set; }

        //public IEnumerable<Lavorazione> Lavorazioni
        //{
        //    get
        //    {
        //        return Singleton.Instance.GetLavorazioni(FaseDiLavoroGuid);
        //    }
        //}
        //public void UpdateValidFlag()
        //{
        //    IsValid = Lavorazioni.All(p => p.IsValid);
        //}

        //public ToolMachine.ToolMachine GetMacchina()
        //{
        //    var machines = PathFolderHelper.GetToolMachines();

        //    return machines.Where(m => m.MachineGuid == MachineGuid).FirstOrDefault();
        //}

        //public double NoChangeToolSecureZ { get; set; }

        public void ResetLavorazioniNumeration()
        {
            var index = 1;
            var lavs = this.Lavorazioni;

            foreach (var l in lavs)
            {
                l.LavorazionePosition = index;
                index++;
            }
        }


        public void AddLavorazione(Lavorazione lavorazione)
        {
            var prevFase = lavorazione.FaseDiLavoro;

            lavorazione.ResetFaseDiLavoro(this.FaseDiLavoroGuid);

            if (prevFase != null && prevFase.FaseDiLavoroGuid != FaseDiLavoroGuid)
                prevFase.ResetLavorazioniNumeration();

            //Riaggiorno valid flag
            UpdateValidFlag();

        }

        public bool IsCompatible(Lavorazione lavorazione)
        {
            return lavorazione.FasiCompatibili.Contains(TipoFase);
        }

        /// <summary>
        /// Ritorna il valore più basso fra limitatore giri macchina e quello settato
        /// </summary>
        /// <returns></returns>
        internal double GetLimitatoreGiri(ToolMachine.ToolMachine macchina)
        {
            var lM = 0.0d;
            if (macchina != null)
                lM = macchina.MaxGiri;

            if (LimitatoreTornio <= 0 && lM > 0)
                return lM;

            if (LimitatoreTornio > 0 && lM <= 0)
                return LimitatoreTornio;

            if (LimitatoreTornio > 0 && lM > 0)
                return Math.Min(LimitatoreTornio, lM);

            return 0;
        }

        internal double GetRapidFeed()
        {
            var macchina = Singleton.Data.GetMacchina(MachineGuid);

            if (macchina != null)
                return macchina.VelocitaRapido;

            return 10000;
        }
    }
}