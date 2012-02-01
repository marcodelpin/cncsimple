using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.Tool;
using CncConvProg.ViewModel.CommonViewModel.ParameterViewModels;
using CncConvProg.ViewModel.CommonViewModel.ToolViewModels;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.OperationViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class OperazioneViewModel : EditStageTreeViewItem, IPreviewable
    {
        protected readonly Operazione Operazione;

        public OperazioneViewModel(Operazione operazione, EditWorkViewModel parent)
            : base(operazione.Descrizione, parent)
        {
            Operazione = operazione;

            if (operazione.FirstStart)
            {
                AutoToolFind();
                operazione.FirstStart = false;
            }
            else
                LoadCompatibleTools();
        }

        private ToolTreeViewItemViewModel _utensileViewModel;
        public ToolTreeViewItemViewModel UtensileViewModel
        {
            get
            {

                //if (_utensileViewModel == null)
                {
                    _utensileViewModel = ToolTreeViewItemViewModel.GetViewModel(Operazione.Utensile, null);

                    _utensileViewModel.OnUpdated += ChildViewModelUpdated;

                }
                return _utensileViewModel;
            }
        }

        void ChildViewModelUpdated(object sender, EventArgs e)
        {
            Operazione.ProgramNeedUpdate = true;
            OnPropertyChanged("IsValid");
        }

        #region Tools List

        private ObservableCollection<Utensile> _utensiliCompatibili;
        public ObservableCollection<Utensile> UtensiliCompatibili
        {
            get
            {
                return _utensiliCompatibili;
            }
            set
            {
                _utensiliCompatibili = value;
                OnPropertyChanged("UtensiliCompatibili");
            }
        }

        public override string Label
        {
            get { return Operazione.DescriptionWithTime; }
        }

        protected void SaveParameter(string param)
        {
            try
            {
                // Guid del materiale per parametri
                var matGuid = Singleton.Instance.MaterialeGuid;

                var tool = UtensileViewModel.Tool;

                tool.OperazioneTipo = Operazione.OperationType;

                tool.SaveTime = DateTime.Now;

                switch (param)
                {
                    case "0":
                        {
                            // sovrascrivo parametro per stesso matguid. e aggiungo a lista parametri

                            /*
                             * magari aggiungere operation type in modo da smistare
                             * se un'utensile faccio più operazioni tipo sgrossatura e sfacciatura  finitura sgrossatura..
                             * vedere poi
                             */
                            tool.AddOrUpdateParametro(tool.ParametroUtensile, matGuid);

                            Singleton.Data.AddOrUpdateTool(tool);
                        } break;

                    case "1":
                        {
                            // Qui invece non sovrascrivo niente, clone utensile e salvo

                            var clonedTool = tool.Clone();

                            clonedTool.AddOrUpdateParametro(tool.ParametroUtensile, matGuid);

                            Singleton.Data.AddOrUpdateTool(clonedTool);

                            LoadCompatibleTools();
                        } break;
                }

                Singleton.Data.SaveMagazzino();

            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in SaveParameter");
            }
        }

        RelayCommand _saveParameterCmd;

        public ICommand SaveParameterCmd
        {
            get
            {
                return _saveParameterCmd ?? (_saveParameterCmd = new RelayCommand(param => SaveParameter((string)param),
                                                                                param => true));
            }
        }

        #endregion

        /// <summary>
        /// Carica utensili compatibili
        /// </summary>
        private void LoadCompatibleTools()
        {
            var t = UtensileViewModel.Tool;

            var compatibleTools = Singleton.Data.GetCompatibleTools(t);

            UtensiliCompatibili = new ObservableCollection<Utensile>(compatibleTools.OrderByDescending(o => o.SaveTime));
        }
        protected void AutoToolFind()
        {
            try
            {
                var matGuid = Singleton.Instance.MaterialeGuid;
                /*
                 * Qui l'utensile è sempre 
                 */

                LoadCompatibleTools();

                /*
                 * Ora devo scegliere quello più adatto in base al materiale ( e in 2° tempo al tipo di operazione )
                 * 
                 * Se non trova niente inserisce il primo della lista degli utensili compatibili.
                 */

                /*
                 * Metodo con il quale sposto la decisione per utensile a lavorazione.
                 * In modo che in una foratura , venga selezionata la punta con diametro più vicino,
                 * Oppure in altre lavorazioni
                 */
                var tool = Operazione.Lavorazione.PickBestTool(Operazione, _utensiliCompatibili, matGuid);




                if (tool == null)
                    tool = _utensiliCompatibili.FirstOrDefault();


                if (tool == null) return;

                SetThisTool(tool);
            }
            catch (Exception ex)
            {
                throw new Exception("OperazioneViewModel.AutoToolFind");
            }
        }

        protected void SetThisTool(Utensile tool)
        {

            try
            {
                if (tool == null) return;

                var matGuid = Singleton.Instance.MaterialeGuid;

                tool.SelectParameter(matGuid);

                Operazione.SetTool(tool);

                _utensileViewModel = ToolTreeViewItemViewModel.GetViewModel(Operazione.Utensile, null);

                _utensileViewModel.OnUpdated += ChildViewModelUpdated;

                OnPropertyChanged("UtensileViewModel");

            }
            catch (Exception ex)
            {
                throw new Exception("OperazioneViewModel.AutoToolFind");
            }
        }

        RelayCommand _setThisToolCmd;

        public ICommand SetThisToolCmd
        {
            get
            {
                return _setThisToolCmd ?? (_setThisToolCmd = new RelayCommand(param => SetThisTool(param as Utensile),
                                                                                param => true));
            }
        }

        RelayCommand _autoToolFindCmd;

        public ICommand AutoToolFindCmd
        {
            get
            {
                return _autoToolFindCmd ?? (_autoToolFindCmd = new RelayCommand(param => AutoToolFind(),
                                                                                param => true));
            }
        }

        public bool OperationAbilited
        {
            get { return Operazione.Abilitata; }
            set
            {
                Operazione.Abilitata = value;
                OnPropertyChanged("OperationAbilited");
            }
        }

        public override bool? ValidateStage()
        {
            if (OperationAbilited)
                return UtensileViewModel.IsValid;

            return true;
        }

        public IEnumerable<IEntity3D> GetPreview()
        {
            // se flag di ricalcolo percorso è true , ricalcola
            // altrimenti restituisce percorso già calcolato

            try
            {
                var rslt = new List<IEntity3D>();

                var workPreview = Operazione.Lavorazione.GetPreview();

                if (workPreview != null)
                    rslt.AddRange(workPreview);

                /*
                 * se profilo è valido
                 *      stampo profilo
                 *      
                 *    se lavorazione è valid 
                 *         stampo anche lavorazione
                 */

                if (IsValid.HasValue && IsValid.Value)
                {
                    /*
                     * piccolo hack . aggiorno le preferenze in questo putno.. todo gestire meglio..
                     */
                    var macchina = Operazione.Lavorazione.FaseDiLavoro.GetMacchina();

                    var path = Operazione.GetPathPreview(macchina);

                    rslt.AddRange(path);
                }

                OnPropertyChanged("Label"); // riaggiorna etichetta con tempo..

                return rslt;
            }

            catch (Exception ex)
            {
                throw new Exception("OperazioneViewModel.GetPreview");
            }
        }

        //internal static OperazioneViewModel GetViewModel(Operazione operazione, EditStageTreeViewItem stageOperazioni)
        //{
        //    if (operazione.Utensile is DrillTool)
        //        return new OperazionePuntaViewModel(operazione.Utensile as DrillTool, operazione, stageOperazioni);

        //    if (operazione.Utensile is UtensileTornitura)
        //        return new OperazioneUtensileTornituraViewModel(operazione.Utensile as UtensileTornitura, operazione, stageOperazioni);


        //    throw new NotImplementedException();
        //}

        //internal static EditStageTreeViewItem GetViewModel(Operazione operazione, EditStageTreeViewItem stageOperazioni)
        //{
        //    //if (operazione is OperazioneFresaturaTrocoidale)
        //    //    return new OperazioneTrocoidaleViewModel(operazione as OperazioneFresaturaTrocoidale, stageOperazioni);

        //    return new OperazioneViewModel(operazione, stageOperazioni);
        //}

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateProgram()
        {
            Operazione.UpdateProgramPath(Operazione.Lavorazione.FaseDiLavoro.GetMacchina());
        }
    }


}


