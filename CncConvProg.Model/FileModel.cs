using System;
using System.Collections.Generic;
using System.Linq;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.FileManageUtility;

namespace CncConvProg.Model
{
    /// <summary>
    /// Classe model interna alla classe singleton,
    /// </summary>
    [Serializable]
    public class FileModel
    {
        public string CurrentFilePath { get; set; }

        //public string TitleFilePath { get { return CurrentFilePath; } }

        public MeasureUnit MeasureUnit { get; private set; }

        internal FileModel(MeasureUnit measureUnit)
        {
            FasiDiLavoro = new List<FaseDiLavoro>();

            Lavorazioni = new List<Lavorazione>();

            MeasureUnit = measureUnit;
        }


        public Tool.Materiale Materiale
        {
            get
            {
                var o = Singleton.Data.GetMaterialeFromGuid(MaterialeGuid);
                return o;
            }
            set
            {
                if (value == null)
                    MaterialeGuid = Guid.Empty;
                MaterialeGuid = value.MaterialeGuid;

            }
        }

        public Guid MaterialeGuid { get; set; }


        private List<FaseDiLavoro> FasiDiLavoro { get; set; }

        private List<Lavorazione> Lavorazioni { get; set; }

        public FaseDiLavoro GetFaseDiLavoro(Guid guid)
        {
            return FasiDiLavoro.Where(l => l.FaseDiLavoroGuid == guid).FirstOrDefault();
        }

        public Lavorazione GetLavorazione(Guid guid)
        {
            return Lavorazioni.Where(l => l.LavorazioneGuid == guid).FirstOrDefault();
        }

        /// <summary>
        /// Aggiunge o sovrascrive lavorazione , se già presente lavorazione con stesso guid
        /// </summary>
        /// <param name="lavorazione"></param>
        /// <returns></returns>
        public void AddLavorazione(Lavorazione lavorazione)
        {
            for (var i = 0; i < Lavorazioni.Count; i++)
            {
                if (Lavorazioni[i].Equals(lavorazione))
                {
                    Lavorazioni[i] = lavorazione;
                    return;
                }
            }

            Lavorazioni.Add(lavorazione);

            // Riaggiorno Numero Lavorazione
            for (int i = 0; i < Lavorazioni.Count; i++)
            {
                Lavorazioni[i].LavorazionePosition = i + 1;
            }
        }

        /// <summary>
        /// Aggiunge o sovrascrive fase , se già presente lavorazione con stesso guid
        /// </summary>
        /// <param name="faseDiLavoro"></param>
        /// <returns></returns>
        public void AddFaseDiLavoro(FaseDiLavoro faseDiLavoro)
        {
            for (var i = 0; i < FasiDiLavoro.Count; i++)
            {
                if (FasiDiLavoro[i].Equals(faseDiLavoro))
                {
                    FasiDiLavoro[i] = faseDiLavoro;
                    return;
                }
            }

            FasiDiLavoro.Add(faseDiLavoro);
        }

        public FaseDiLavoro CreateFaseDiLavoro(ToolMachine.ToolMachine toolMachine)
        {
            var faseLavoro = toolMachine.CreateFaseLavoro();

            AddFaseDiLavoro(faseLavoro);

            return faseLavoro;
        }

        public IEnumerable<Lavorazione> GetLavorazioni(Guid faseGuid)
        {
            return Lavorazioni.Where(l => l.FaseDiLavoroGuid == faseGuid);
        }
        public IEnumerable<FaseDiLavoro> GetFasiDiLavoro()
        {
            return FasiDiLavoro;
        }

        public bool RemoveLavorazione(Lavorazione lavorazione)
        {
            if (Lavorazioni.Contains(lavorazione))
            {
                Lavorazioni.Remove(lavorazione);

                for (int i = 0; i < Lavorazioni.Count; i++)
                {
                    Lavorazioni[i].LavorazionePosition = i + 1;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Metodo per rimuovere fasi di lavoro
        /// </summary>
        /// <param name="faseDiLavoro"></param>
        public void RemoveFaseLavoro(FaseDiLavoro faseDiLavoro)
        {
            // Prima rimuovo tutte le lavorazioni figlie

            var lavToRemove = Lavorazioni.Where(l => l.FaseDiLavoroGuid == faseDiLavoro.FaseDiLavoroGuid).ToList();

            while (lavToRemove.Count() > 0)
            {
                var lav = lavToRemove.FirstOrDefault();

                if (Lavorazioni.Contains(lav))
                    Lavorazioni.Remove(lav);


                lavToRemove.Remove(lav);
            }

            // poi rimuvo la fase

            if (FasiDiLavoro.Contains(faseDiLavoro))
                FasiDiLavoro.Remove(faseDiLavoro);

        }

        public IEnumerable<Operazione> GetOperationList(Guid faseDiLavoroId)
        {
            return from lavorazione in Lavorazioni
                   where lavorazione.FaseDiLavoro.FaseDiLavoroGuid == faseDiLavoroId
                   from operazione in lavorazione.Operazioni
                   where operazione.Abilitata
                   orderby operazione.PhaseOperationListPosition
                   select operazione;
        }


        public double PrezzoCalcolto { get; set; }

        public double PrezzoDefinitivo { get; set; }

        public string Note { get; set; }
    }
}