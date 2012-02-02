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
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.CommonViewModel.ToolViewModels
{
    /// <summary>
    /// Tool View Model 
    ///    Contiene :
    ///     - Parametro
    ///     - Dati Utensile
    ///     - Dati PortaUtensile
    /// </summary>
    public abstract class ToolTreeViewItemViewModel : TreeViewItemViewModel, IValidable, IDataErrorInfo
    {
        public readonly Utensile Tool;

        protected ToolTreeViewItemViewModel(Utensile tool, TreeViewItemViewModel parent)
            : base(string.Empty, parent)
        {
            Tool = tool;

            _modified = true;
            //if (string.IsNullOrWhiteSpace(ToolDescription))
            //    ToolDescription = tool.ToolDescription;
        }

        private ToolParameterViewModel _toolParameterViewModel;
        public ToolParameterViewModel ToolParameterViewModel
        {
            get
            {
                if (_toolParameterViewModel == null)
                {
                    _toolParameterViewModel = ToolParameterViewModel.GetViewModel(Tool);
                    _toolParameterViewModel.OnUpdated += ChildViewModelUpdated;
                }
                return _toolParameterViewModel;
            }

            set
            {
                _toolParameterViewModel = value;
                OnPropertyChanged("ToolParameterViewModel");
            }
        }

        private bool? _isValid;

        private bool _modified = true;

        protected override void OnPropertyChanged(string propertyName)
        {
            _modified = true;

            if (propertyName != "IsValid")
            {
                // richiedi aggiornamento parent
                OnPropertyChanged("IsValid");
                RequestUpdate(this);
            }

            base.OnPropertyChanged(propertyName);
        }

        void ChildViewModelUpdated(object sender, EventArgs e)
        {
            _modified = true;

            RequestUpdate(this);
        }

        public string ToolDescription
        {
            get { return Tool.ToolDescription; }
        }

        public string ToolName
        {
            get { return Tool.ToolName; }

            set
            {
                if (Tool.ToolName == value) return;
                Tool.ToolName = value;
                OnPropertyChanged("ToolDescription");
                OnPropertyChanged("ToolName");

            }
        }

        public int NumeroPostazione
        {
            get { return Tool.NumeroPostazione; }
            set
            {
                Tool.NumeroPostazione = value;
                OnPropertyChanged("NumeroPostazione");
            }

        }


        public bool CoolantOn
        {
            get { return Tool.CoolantOn; }
            set
            {
                Tool.CoolantOn = value;

                OnPropertyChanged("CoolantOn");
            }
        }

        public string NumeroCorrettoreRaggio
        {
            get { return Tool.NumeroCorrettoreRaggio; }

            set
            {
                Tool.NumeroCorrettoreRaggio = value;
                OnPropertyChanged("NumeroCorrettoreRaggio");
            }
        }

        public string NumeroCorrettoreLunghezza
        {
            get { return Tool.NumeroCorrettoreLunghezza; }

            set
            {
                Tool.NumeroCorrettoreLunghezza = value;
                OnPropertyChanged("NumeroCorrettoreLunghezza");
            }
        }


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
        /// Se lo stage risulta modificato , ricalcolo il valore di valid e restituisco.
        /// </summary>
        public bool? IsValid
        {
            get
            {
                if (_modified || _isValid == null)
                {
                    _isValid = ValidateStage();
                }

                return _isValid;
            }
        }

        public bool? ValidateStage()
        {
            _isValid = (ValidatedProperties.All(property => GetValidationError(property) == null) &&
                        (ToolParameterViewModel.IsValid.HasValue && ToolParameterViewModel.IsValid.Value));

            return _isValid;

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
            get { return Tool.ToolDescription; }
        }

        public Guid ToolGuid
        {
            get { return Tool.ToolGuid; }
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

            var parametri = Tool.ParametriUtensile.Where(p => p.MaterialGuid == materiale.MaterialeGuid);

            var list = new ObservableCollection<ToolParameterViewModel>();

            foreach (var parametroUtensile in parametri)
            {
                list.Add(ToolParameterViewModel.GetViewModel(Tool));
            }

            Parametri = list;
        }


        internal void UpdateParameterViewModel()
        {
            ToolParameterViewModel = ToolParameterViewModel.GetViewModel(Tool);
            //OnPropertyChanged("ToolParameterViewModel");
        }

    }
}