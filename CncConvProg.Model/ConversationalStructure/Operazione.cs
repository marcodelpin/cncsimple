using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.PreviewPathEntity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.PreviewEntity;
using CncConvProg.Model.PreviewPathEntity;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.ConversationalStructure
{
    /*
     * 30/07/2011
     * 
     * tempo 
     * 
     * ogni azione deve restituirmi il tempo 
     * 
     * poi faccio somma.
     * 
     * 
     * lo faccio elaborare dalla macchina , cosi riesco a recuperare info quali velocita e feed e caxxi vari
     * 
     * 
     */

    /*
     * 11/06/11
     * 
     * cerco di tenere operazione unica e non fare derivazioni per ogni tipo di utensile.
     * 
     * 
     * un 'operazione ha :
     *  - tool e tool ha parametro.
     *  - enum della lavorazione
     *  - logica per generare programma uni -> preview -> code
     *  
     * - in caso serva si puo pensare a dividere fra :
     * 
     *  - operazioneCentro
     *  - operazioneTornio
     */

    /// <summary>
    /// Divideri piuttosto classe operazioneTornio e operazioneCentro
    /// </summary>
    [Serializable]
    public class Operazione
    {
        public bool FirstStart = true;

        public bool ProgramNeedUpdate { get; set; }

        protected ProgramOperation ProgramPhase { get; set; }

        protected List<IPreviewEntity> PathPreview { get; set; }

        public TimeSpan CycleTime { get; set; }

        public OperazioneTime OperationTime { get; set; }

        /* 
         * tool -> parameter
         * 
         * toolHolder -> dato da macchina
         */


        /// <summary>
        /// Questo è il metodo che restituisce il Program in formato universale.
        /// </summary>
        /// <returns></returns>
        public ProgramOperation GetProgramPhase(ToolMachine.ToolMachine toolMachine, bool forceUpdate = false)
        {
            if (ProgramNeedUpdate || forceUpdate)
            {
                UpdateProgramPath(toolMachine);
            }

            return ProgramPhase;
        }

        /// <summary>
        /// Aggiorna il programma per l'operazione.
        /// Dal programma crea oggetti per rappresentare anteprima.
        /// Dagli oggetti che rappresentano anteprima calcola il tempo macchina.
        /// </summary>
        public void UpdateProgramPath(ToolMachine.ToolMachine machine)
        {
            if(machine == null)
                throw new Exception("Operazione.UpdateProgramPath- Machine == null");

            ProgramPhase = Lavorazione.GetOperationProgram(this);

            PathPreview = Lavorazione.GetPathPreview(ProgramPhase, machine);

            OperationTime = machine.GetTime(PathPreview);

            // qui recupero info rigurdo tool e parametro che mi serviranno in 2nd momento
            OperationTime.NumeroUtensile = GetToolPosition();
            //OperationTime.ConsumoUtensilePerMinuto = Utensile.ParametroUtensile.CostoUtensilePerMinuto;

            CycleTime = OperationTime.TempoTotale;

            ProgramNeedUpdate = false;

        }

        /// <summary>
        /// Qui restituisce path anteprima , decido di
        /// </summary>
        /// <param name="toolMachine"></param>
        /// <returns></returns>
        public IEnumerable<IEntity3D> GetPathPreview(ToolMachine.ToolMachine toolMachine)
        {
            var rslt = new List<IEntity3D>();

            // var workPreview = Lavorazione.GetPreview();

            if (ProgramNeedUpdate)
            {
                return rslt;
                //  UpdateProgramPath(toolMachine);
            }

            //   rslt.AddRange(workPreview);
            var p = PreviewEntityHelper.GetIEntity3DFromIPreviewEntity(PathPreview);

            rslt.AddRange(p);

            return rslt;
        }

        private int ParentOperationListPosition
        {
            get
            {
                if (Lavorazione == null)
                    throw new NullReferenceException();

                var l = Lavorazione.Operazioni.ToList();

                if (!l.Contains(this))
                    throw new Exception();

                var pos = l.IndexOf(this);

                return pos + 1;
            }
        }
        public int PhaseOperationListPosition { get; set; }

        public SpindleRotation SpindleRotation { get; set; }
        public string Descrizione { get; set; }
        public LavorazioniEnumOperazioni OperationType { get; private set; }


        /// <summary>
        /// Quando nella operazioni successiva ho lo stesso utensile ,
        /// Il cambio utensile è opzionale.
        /// Lo setto al momento del riordino delle operazione .
        /// o tramite view model..
        /// </summary>
        public bool ToolChangeOptional { get; set; }

        /// <summary>
        /// Quando il cambio utensile è opzionale , posso comunque forzare il cambio utensile.
        /// Di default e settato su false.
        /// </summary>
        public bool ForceToolChange { get; set; }

        public bool Abilitata { get; set; }

        /// <summary>
        /// Dice se l'operazione utilizza utensile rotante.
        /// false in quelle operazioni di tornitura
        /// </summary>
        public virtual bool IsRotaryTool { get { return true; } }



        public Utensile Utensile { get; private set; }
        //public ParametroUtensile ParametroUtensile { get; private set; }
        public Lavorazione Lavorazione { get; private set; }
       // public ToolHolder ToolHolder { get; private set; }

        public string NumberedDescription
        {
            get
            {
                if (Lavorazione == null)
                    throw new NullReferenceException();

                var lavPos = Lavorazione.LavorazionePosition;

                const string sep = " - ";

                var opPos = ParentOperationListPosition;

                return lavPos + sep + opPos + " " + Descrizione;
            }
        }

        /// <summary>
        /// Ritorna il tempo formattato hh:mm:ss
        /// o xxH xxMin xxSec
        /// </summary>
        public static string FormatTime(double minutes)
        {
            if (double.IsInfinity(minutes) || double.IsNaN(minutes)) return string.Empty;

            var span = TimeSpan.FromMinutes(minutes);

            var days = span.Days;
            var hours = span.Hours;
            var min = span.Minutes;
            var second = span.Seconds;

            // return hours.ToString("00") + "h " + min.ToString("00") + "' " + second.ToString("00") + "''";

            var rslt = new StringBuilder();

            if (days > 0)
            {
                rslt.Append(days);
                rslt.Append("d:");
            }

            if (hours > 0)
            {
                rslt.Append(hours.ToString("00"));
                rslt.Append("h:");
            }

            rslt.Append(min.ToString("00"));
            rslt.Append("m:");
            rslt.Append(second.ToString("00"));
            rslt.Append("s");

            return rslt.ToString();
        }

        public string CycleTimeString
        {
            get { return FormatTime(CycleTime.TotalMinutes); }
        }
        public string DescriptionWithTime
        {
            get
            {
                return Descrizione + " " + CycleTimeString;
            }
        }

        public Operazione(Lavorazione parent, LavorazioniEnumOperazioni enumOperationType)
        {
            Abilitata = true;

            Lavorazione = parent;
            OperationType = enumOperationType;

            Descrizione = Lavorazione.GetOperationDescription(enumOperationType);

            Utensile = Lavorazione.CreateTool(enumOperationType, Singleton.Instance.MeasureUnit);

            if (Utensile == null)
                throw new NotImplementedException();

            try
            {
                var matGuid = Singleton.Instance.MaterialeGuid;

                Utensile.SetMaterial(matGuid);
            }
            catch (Exception)
            {

                Trace.WriteLine("Errore nel creare parametro utensile ");
            }
        }

        //public Operazione(Lavorazioni.Fresatura.ScanalaturaLinea scanalaturaLinea, int p)
        //{
        //    // TODO: Complete member initialization
        //    this.scanalaturaLinea = scanalaturaLinea;
        //    this.p = p;
        //}

        //public IEnumerable<IEntity2D> GetPreview()
        //{
        //    var rslt = Lavorazione.GetPreview().ToList();

        //    rslt.AddRange(Lavorazione.GetOperationPreview(OperationType));

        //    return rslt;
        //}

        //  public abstract void SetCutParameter(ref UniversalCode universalCode);
        //public abstract UniversalCode GetUniversalCode();

        internal string GetToolDescriptionName()
        {
            return Utensile != null ? Utensile.ToolDescription : "NotDefined";
        }

        public int GetToolPosition()
        {
            return Utensile.NumeroPostazione;
        }

        internal int GetLatheToolCorrector()
        {
            return Utensile.NumeroPostazione;
        }

        internal ModalitaVelocita GetSpeedType()
        {
            //if (this is IOperationTurnable)
            //{
            //    var op = this as IOperationTurnable;

            //    if (op != null) return op.ModalitaVelocita;
            //}

            return ModalitaVelocita.GiriFissi;
        }

        public virtual double GetSpeed()
        {
            return Utensile.ParametroUtensile.GetSpeed();
        }


        public Utensile GetTool()
        {
            return Utensile;
        }


        public List<Utensile> GetCompatibleTools(MagazzinoUtensile magazzino)
        {
            var l = Lavorazione.GetCompatibleTools(OperationType, magazzino);
            return l;
        }

        public void SetTool(Utensile tool)
        {
            if (tool == null) return;

            var clonedTool = FileManageUtility.FileUtility.DeepCopy(tool);

            Utensile = clonedTool;
        }

        internal T1 GetParametro<T1>() where T1 : ParametroUtensile
        {
            if (Utensile != null && Utensile.ParametroUtensile != null)
            {
                if (Utensile.ParametroUtensile.GetType() == typeof(T1))
                {
                    return (T1)Utensile.ParametroUtensile;

                }
            }

            throw new NullReferenceException();
        }

        public string GetToolDiameterCorrecto()
        {
            return Utensile.NumeroCorrettoreRaggio;
        }

        public string GetToolHeightCorrector()
        {
            return Utensile.NumeroCorrettoreLunghezza;
        }

        internal bool GetCoolant()
        {
            return Utensile.CoolantOn;
        }

        /// <summary>
        /// Restituisce se devo scrivere cambio utensile
        /// </summary>
        public bool OutputCambioUtensile
        {
            get
            {
                if (ToolChangeOptional == false)
                    return true;

                return ToolChangeOptional && ForceToolChange;
            }
        }

        /// <summary>
        /// Mi dice se il disimpegno e corto o torna a Zero Macchina.
        /// Se operazione successiva non cambio utensile : true
        /// Se utensile cambia : false
        /// </summary>
        public bool OutputDisimpegnoCorto { get; set; }
    }
}



