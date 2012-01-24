using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.ViewModel.CommonViewModel.ParameterViewModels;
using CncConvProg.ViewModel.EditWorkDialog.OperationViewModel.ToolHolder;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.CommonViewModel.ToolViewModels
{
    /*
     * todo : c'è confusione fra i i vari viewModel . ma cmq funzione ok . posticipo l'aggiustaggio
     * 
     * il viewModel .ToolTreeViewItemViewModel è la viewModel utensile, ma contiene anche il vm dei parametri e toolholder
     * 
     * dovrebbe invece esserci un viewModel sup. che contiene i 3 viewModel..
     * */
    /// <summary>
    /// Dentro il view Model ci sono i vari viewModel
    /// </summary>
    public abstract class ToolTreeViewItemViewModel : TreeViewItemViewModel, IValid, IDataErrorInfo
    {
        private readonly Utensile _tool;

        public ToolTreeViewItemViewModel(Utensile tool, TreeViewItemViewModel parent)
            : base(string.Empty, parent)
        {
            _tool = tool;

            //if (string.IsNullOrWhiteSpace(ToolDescription))
            //    ToolDescription = tool.ToolDescription;
        }

        public ToolHolderViewModel MillToolHolderVm
        {
            get
            {
                return ToolHolderViewModel.GetViewModel(_tool.MillToolHolder, _tool, null);
            }
        }

        public ToolHolderViewModel LatheToolHolderVm
        {
            get
            {
                return ToolHolderViewModel.GetViewModel(_tool.LatheToolHolder, _tool, null);
            }
        }

        public ToolParameterViewModel ToolParameterViewModel
        {
            get
            {
                return ToolParameterViewModel.GetViewModel(_tool.ParametroUtensile, _tool.Unit);
            }
        }

        public ToolTreeViewItemViewModel ToolViewModel
        {
            get
            {
                return ToolTreeViewItemViewModel.GetViewModel(_tool, null);
            }
        }

        //public string ToolDescription
        //{
        //    get { return _tool.ToolDescription; }

        //    set
        //    {
        //        _tool.ToolDescription = value;
        //        OnPropertyChanged("ToolDescription");
        //    }
        //}

        public static ToolTreeViewItemViewModel GetViewModel(Utensile tool, TreeViewItemViewModel parent)
        {
            if (tool is DrillTool)
                return new PuntaViewModel(tool as DrillTool, parent);

            if (tool is FresaCandela)
                return new FresaCandelaViewModel(tool as FresaCandela, parent);

            if (tool is FresaFilettare)
                return new FresaFilettareViewModel(tool as FresaFilettare, parent);

            if (tool is FresaSpianare)
                return new FresaSpianareViewModel(tool as FresaSpianare, parent);

            throw new NotImplementedException();

        }

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid
        {
            get { return ValidatedProperties.All(property => GetValidationError(property) == null); }
        }

        protected virtual string[] ValidatedProperties { get { return new[] { "" }; } }

        protected virtual string GetValidationError(string propertyName)
        {
            return string.Empty;
        }

        #endregion

        /// <summary>
        /// Faccio override del label del TreeView
        /// </summary>
        public override string Label
        {
            get { return _tool.ToolDescription; }
        }

        public Guid ToolGuid
        {
            get { return _tool.ToolGuid; }
        }



        /*
         * questa classe si occupa anche di fare visualizzare i parametri :
         * 
         * combo box materiale è esterno 
         * 
         * quando viene cambiato riaggiorno la lista parametri.
         * 
         * devo mettere refreshParameterList
         * 
         */

        private ObservableCollection<ToolParameterViewModel> _parametri;
        public ObservableCollection<ToolParameterViewModel> Parametri
        {
            get { return _parametri; }
            set
            {
                _parametri = value;
                OnPropertyChanged("Parametri");
            }
        }

        public void RefreshParameterList(Materiale materiale, MeasureUnit measureUnit)
        {
            if (materiale == null)
                return;

            var parametri = _tool.ParametriUtensile.Where(p => p.MaterialGuid == materiale.MaterialeGuid);

            var list = new ObservableCollection<ToolParameterViewModel>();

            foreach (var parametroUtensile in parametri)
            {
                list.Add(ToolParameterViewModel.GetViewModel(parametroUtensile, measureUnit));
            }

            Parametri = list;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName != "IsValid")
            {
                // richiedi aggiornamento parent
                OnPropertyChanged("IsValid");
                RequestUpdate(this);
            }

            base.OnPropertyChanged(propertyName);
        }


    }
}