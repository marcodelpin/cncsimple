using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CncConvProg.ViewModel.EditWorkDialog.OperationViewModel.ToolHolder;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.OperationViewModel
{
    /*
     * allora , per personalizzare in modo migliore immissione dei parametri ,
     * 
     * devo creare..
     * 
     * esistono alcuni casi speciali dove non posso risolvere con comandi particolari ..
     * 
     * o dove l'immissione deve essere più guidata..
     * 
     * per trocoidale > 2dc sarebbe anche ok--
     * 
     * potrei mettere w e ap dentro operazioni 
     * 
     * e creare maniera per disabilitare parametro utensile 
     * 
     * determinati parametri utensile..
     * 
     */
    public class OperazioneViewModel : EditStageTreeViewItem, IPreviewable, IValid
    {
        protected readonly Operazione Operazione;

        private readonly MeasureUnit _measureUnit;

        public OperazioneViewModel(Operazione operazione, EditStageTreeViewItem parent)
            : base(operazione.Descrizione, parent)
        {
            Operazione = operazione;

            _measureUnit = Singleton.Instance.MeasureUnit;
        }

        /*
         * per aggiornare le 3 proprieta sotto o creo metodo update nel viewModel
         *  
         * oppure ricreo il viewModel. scelgo questa quest'ultima
         */
        private ToolHolderViewModel _toolHolderViewModel;
        public ToolHolderViewModel ToolHolderVm
        {
            get
            {
                //if (_toolHolderViewModel == null)
                {
                    _toolHolderViewModel = ToolHolderViewModel.GetViewModel(Operazione.ToolHolder, Operazione.Utensile, this);
                    _toolHolderViewModel.OnUpdated += ChildViewModelUpdated;
                }
                return _toolHolderViewModel;
            }
        }

        private ToolParameterViewModel _toolParameterViewModel;
        public ToolParameterViewModel ToolParameterViewModel
        {
            get
            {
                //if (_toolParameterViewModel == null)
                {
                    _toolParameterViewModel = ToolParameterViewModel.GetViewModel(
                        Operazione.Utensile.ParametroUtensile, _measureUnit);
                    _toolParameterViewModel.OnUpdated += ChildViewModelUpdated;
                }
                return _toolParameterViewModel;


            }
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

        protected void SaveParameter()
        {
            //try
            //{
            //    // magari tenerlo caricato in memoria..
            //    //var magazzino = FileManageUtility.PathFolderHelper.GetMagazzinoUtensile();

            //    ///*
            //    // * qui succede arcano
            //    // */
            //    //var tool = Operazione.Utensile;

            //    //if(tool == null)return;

            //    //var millHolder = Operazione.ToolHolder as MillToolHolder;
            //    //var latheToolHolder = Operazione.ToolHolder as LatheToolHolder;

            //    //if (millHolder != null)
            //    //{
            //    //    tool.MillToolHolder.GetToolDefaultData(tool);

            //    //}
            //    //if (latheToolHolder != null)
            //    //{
            //    //    tool.LatheToolHolder.GetToolDefaultData(tool);
            //    //}

            //    //magazzino.SaveTool(tool);

            //    //FileManageUtility.PathFolderHelper.SaveMagazzinoUtensile(magazzino);


            //    ///*
            //    // * dovrei salvare correttori dentro utensile. usare solamente toolHolderViewModel
            //    // * 
            //    // * tagliare via classi toolholder
            //    // */

            //}
            //catch (Exception ex)
            //{

            //}
            // chiede a utensile di cercare fra utensili che gli passo
        }

        RelayCommand _saveParameterCmd;

        public ICommand SaveParameterCmd
        {
            get
            {
                return _saveParameterCmd ?? (_saveParameterCmd = new RelayCommand(param => SaveParameter(),
                                                                                param => true));
            }
        }

        #endregion

        protected void AutoToolFind()
        {
            try
            {
                // magari tenerlo caricato in memoria..
                //var magazzino = PathFolderHelper.GetMagazzinoUtensile();

                //var tools = Operazione.GetCompatibleTools(magazzino);

                //if (tools == null || tools.Count == 0) return;

                ///*devo assicurarmi che utensile sia settato*/

                //UtensiliCompatibili = new ObservableCollection<Utensile>(tools);


                //var tool = _utensiliCompatibili.FirstOrDefault();


                //Operazione.SetTool(tool);

                //OnPropertyChanged("UtensileViewModel");
                //OnPropertyChanged("ToolParameterViewModel");
                //OnPropertyChanged("ToolHolderVm");

                /*
                 * una volta che ho utensili compatibili
                 */

                /*
                 * todo : su apertura screen , e se utensile corrente ha guid 00.00.00 
                 * allora cercare tool, altrimenti niente.
                 * 
                 * ?? aggiorna lista utensili compatibili e seleziona quello più adatto..
                 * 
                 * var compatibleTool = abstrat getCompatibleTools(toolStore)
                 * 
                 * var selectdtool = selectBestTool(compatibleTool);
                 */



            }
            catch (Exception ex)
            {
                throw new Exception("OperazioneViewModel.AutoToolFind");
            }
            // chiede a utensile di cercare fra utensili che gli passo
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

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                /*
                 * todo : qui viene rigenerato il viewModel ogni volta che 1 campo viene modificato
                 * e di conseguenza rivengono controllati tutti i campi.. e rigenerati 10 volte
                 */
                return ToolHolderVm.IsValid && ToolParameterViewModel.IsValid && UtensileViewModel.IsValid;
            }
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

                if (IsValid)
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

        internal static EditStageTreeViewItem GetViewModel(Operazione operazione, EditStageTreeViewItem stageOperazioni)
        {
            //if (operazione is OperazioneFresaturaTrocoidale)
            //    return new OperazioneTrocoidaleViewModel(operazione as OperazioneFresaturaTrocoidale, stageOperazioni);

            return new OperazioneViewModel(operazione, stageOperazioni);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateProgram()
        {
            Operazione.UpdateProgramPath(Operazione.Lavorazione.FaseDiLavoro.GetMacchina());
        }
    }


}


