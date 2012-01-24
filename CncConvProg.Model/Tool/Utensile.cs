using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.Tool
{
    [Serializable]
    public abstract class Utensile
    {
        protected Utensile(MeasureUnit measureUnit)
        {
            ToolGuid = Guid.NewGuid();

            Unit = measureUnit;

            ParametroUtensile = CreateParametro();

            ParametriUtensile = new List<ParametroUtensile>();


            // per ora il discorso toolholder decade
            MillToolHolder = new MillToolHolder();
            LatheToolHolder = new LatheToolHolder();
            MillToolHolder.NumeroPostazione = 1;
            MillToolHolder.NumeroCorrettoreLunghezza = "1";
            MillToolHolder.NumeroCorrettoreRaggio = "1";
            LatheToolHolder.NumeroCorrettore = 1;

        }

        /// <summary>
        /// Seleziona il parametro adatto al materiale,
        /// 
        /// </summary>
        /// <param name="materiale"></param>
        public void SelectParametroFromMaterial(Materiale materiale)
        {
            var matGuid = materiale.MaterialeGuid;

            var selected = from p in ParametriUtensile
                           where p.MaterialGuid == matGuid
                           select p;

            var parameter = selected.FirstOrDefault();

            /*
             * todo : clonare parametro 
             */
            if (parameter != null)
                ParametroUtensile = parameter;
        }

        public LatheToolHolder LatheToolHolder { get; set; }

        public MillToolHolder MillToolHolder { get; set; }

        /// <summary>
        /// Serve per assegnare ad ogni utensile il tipo di lavorazione che andrà a fare..
        /// Per distinguere fra finitura e sgrossatura più che altro.
        /// </summary>
        public LavorazioniEnumOperazioni OperazioneTipo { get; set; }

        /// <summary>
        /// In caso ci siano più utensili , con medesimo operazione tipo
        /// Uso questa proprietà per ordinarle e prendere il più recente
        /// </summary>
        public DateTime SaveTime { get; set; }

        public Guid ToolGuid { get; private set; }

        /// <summary>
        /// Non mi ricordo perchè l'ho messo
        /// </summary>
        //public string OverrideToolName { get; set; }

        /// <summary>
        /// Descrizione Completa 
        ///  NomeUtensile e Diametro
        /// Punta HSS - 10 mm
        /// </summary>
        public string ToolDescription
        {
            get
            {
                double? d = null;
                var td = this as IDiametrable;
                if (td != null)
                {
                    d = td.Diametro;
                }
                return ToolName + " " + d;
            }

            //set
            //{
            //    ToolName = value;
            //}
        }

        protected string GetMeasureDescp()
        {
            return Unit == MeasureUnit.Millimeter ? "mm" : "inch";
        }

        /// <summary>
        /// Nome Utensile :
        /// es. Punta HSS
        /// </summary>
        public string ToolName { get; set; }

        public MeasureUnit Unit { get; private set; }

        /// <summary>
        /// Seleziona il parametro adatto al materiale,
        /// 
        /// </summary>
        /// <param name="materialeGuid"></param>
        public void SelectParametroFromMaterial(Guid materialeGuid)
        {

            var selected = from p in ParametriUtensile
                           where p.MaterialGuid == materialeGuid
                           select p;

            var parameter = selected.FirstOrDefault();

            /*
             * todo : clonare parametro 
             */
            if (parameter != null)
                ParametroUtensile = parameter;
        }

        public List<ParametroUtensile> ParametriUtensile { get; set; }

        /// <summary>
        ///  queste due vanno dentro tool holder..
        /// </summary>
        //public int NumeroCorrettore { get; set; }
        //public int NumeroPostazione { get; set; }

        internal abstract ParametroUtensile CreateParametro();

        public ParametroUtensile ParametroUtensile { get; set; }

        public int ToolPosition { get; set; }

        /// <summary>
        /// Se esiste un parametro analogo , lo cancello poi inserisco il parametro da salvare.
        /// </summary>
        /// <param name="parametroUtensile"></param>
        /// <param name="matGuid"></param>
        public void AddOrUpdateParametro(ParametroUtensile parametroUtensile, Guid matGuid)
        {
            if (parametroUtensile == null) return;

            var parameter = (from p in ParametriUtensile
                             where p.MaterialGuid == matGuid
                             select p).FirstOrDefault();

            if (parameter != null)
            {
                if (ParametriUtensile.Contains(parameter))
                    ParametriUtensile.Remove(parameter);
            }

            parametroUtensile.MaterialGuid = matGuid;

            //Devo spezzare la referenza
            var clonedParametro = FileManageUtility.FileUtility.DeepCopy(parametroUtensile);

            clonedParametro.SetUtensile(this);
            ParametriUtensile.Add(clonedParametro);
        }

        /// <summary>
        /// Clona utensile e rigenera guid.
        /// </summary>
        /// <returns></returns>
        public Utensile Clone()
        {
            var ct = FileManageUtility.FileUtility.DeepCopy(this);
            ct.RegenerateGuid();
            return ct;
        }

        private void RegenerateGuid()
        {
            ToolGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Seleziona il parametro in base al materiale
        /// Se non esiste lo crea ..
        /// </summary>
        /// <param name="matGuid"></param>
        public void SelectParameter(Guid matGuid)
        {
            var p = (from par in ParametriUtensile
                     where par.MaterialGuid == matGuid
                     select par).FirstOrDefault();

            if (p == null)
            {
                p = CreateParametro();
                p.MaterialGuid = matGuid;
                ParametriUtensile.Add(p);
            }

            ParametroUtensile = p;
        }

        internal void SetMaterial(Guid matGuid)
        {
            ParametroUtensile.SetMateriale(matGuid);
        }

    }
}