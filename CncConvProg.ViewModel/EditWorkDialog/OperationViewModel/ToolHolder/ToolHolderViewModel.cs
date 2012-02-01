using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.Tool;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using System;

namespace CncConvProg.ViewModel.EditWorkDialog.OperationViewModel.ToolHolder
{
    public abstract class ToolHolderViewModel : ViewModelValidable, IDataErrorInfo
    {
        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public override bool? ValidateStage()
        {
            return ValidatedProperties.All(property => GetValidationError(property) == null);
        }


        protected abstract string[] ValidatedProperties { get; }

        protected abstract string GetValidationError(string propertyName);

        #endregion

        private readonly OperazioneViewModel _parent;
        private readonly Model.Tool.ToolHolder _toolHolder;
        private readonly Model.Tool.Utensile _tool;


        //protected ToolHolderViewModel(Model.Tool.ToolHolder toolHolder, OperazioneViewModel parent)
        //{
        //    _toolHolder = toolHolder;
        //    _parent = parent;
        //}

        protected ToolHolderViewModel(Model.Tool.ToolHolder toolHolder, Model.Tool.Utensile utensile, OperazioneViewModel parent)
        {
            _toolHolder = toolHolder;
            _parent = parent;
            _tool = utensile;
        }

        public int NumeroPostazione
        {
            get { return _toolHolder.NumeroPostazione; }
            set
            {
                _toolHolder.NumeroPostazione = value;
                OnPropertyChanged("NumeroPostazione");
            }

        }


        public bool CoolantOn
        {
            get { return _toolHolder.CoolantOn; }
            set
            {
                _toolHolder.CoolantOn = value;

                OnPropertyChanged("CoolantOn");
            }
        }

        internal static ToolHolderViewModel GetViewModel(Model.Tool.ToolHolder toolHolder, Utensile tool, OperazioneViewModel viewModelParent)
        {
            if (toolHolder == null)
                throw new NullReferenceException();

            if (toolHolder is MillToolHolder)
                return new MillToolHolderViewModel(viewModelParent, toolHolder as MillToolHolder, tool);

            if (toolHolder is LatheToolHolder)
                return new LatheToolHolderViewModel(viewModelParent, toolHolder as LatheToolHolder, tool);


            throw new NotImplementedException();
        }

        public event EventHandler OnUpdated;

        protected void RequestUpdate(ViewModelBase caller)
        {
            var handler = OnUpdated;
            if (handler != null)
                handler(caller, EventArgs.Empty);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName != "IsValid")
            {
                OnPropertyChanged("IsValid");
                RequestUpdate(this);
            }
            base.OnPropertyChanged(propertyName);
        }
    }

    public class LatheToolHolderViewModel : ToolHolderViewModel
    {
        private readonly LatheToolHolder _latheToolHolder;

        public LatheToolHolderViewModel(OperazioneViewModel parent, LatheToolHolder holder, Utensile tool)
            : base(holder, tool, parent)
        {
            _latheToolHolder = holder;
        }

        public int NumeroCorrettore
        {
            get { return _latheToolHolder.NumeroCorrettore; }

            set
            {
                _latheToolHolder.NumeroCorrettore = value;
                OnPropertyChanged("NumeroCorrettore");
            }
        }



        #region IDataErrorInfo Members

        protected override string[] ValidatedProperties
        {
            get
            {
                return new[]
                           {
                               "NumeroPostazione",
                               "NumeroCorrettore",
                           };
            }
        }

        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                //case "DiametroEffettivo":
                //    {
                //        error = InputCheck.MaggioreDiZero(DiametroEffettivo);
                //    }
                //    break;

                //case "DiametroIngombro":
                //    {
                //        if (DiametroIngombro < DiametroEffettivo)
                //            error = "Incorrect diameter value ";
                //    }
                //    break;

                //case "RaggioInserto":
                //    {
                //        error = InputCheck.MaggioreOUgualeDiZero(RaggioInserto);
                //    } break;

                //default:
                //    Debug.Fail("Unexpected property : " + propertyName);
                //    break;
            }

            return error;
        }

        #endregion

    }

    public class MillToolHolderViewModel : ToolHolderViewModel
    {
        private readonly MillToolHolder _millToolHolder;

        public MillToolHolderViewModel(OperazioneViewModel parent, MillToolHolder holder, Utensile tool)
            : base(holder, tool, parent)
        {
            _millToolHolder = holder;
        }



        public string NumeroCorrettoreRaggio
        {
            get { return _millToolHolder.NumeroCorrettoreRaggio; }

            set
            {
                _millToolHolder.NumeroCorrettoreRaggio = value;
                OnPropertyChanged("NumeroCorrettoreRaggio");
            }
        }

        public string NumeroCorrettoreLunghezza
        {
            get { return _millToolHolder.NumeroCorrettoreLunghezza; }

            set
            {
                _millToolHolder.NumeroCorrettoreLunghezza = value;
                OnPropertyChanged("NumeroCorrettoreLunghezza");
            }
        }



        #region IDataErrorInfo Members

        protected override string[] ValidatedProperties
        {
            get
            {
                return new[]
                           {
                               "NumeroCorrettoreLunghezza",
                               "NumeroCorrettoreDiametro",
                               "NumeroPostazione",
                           };
            }
        }

        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                //case "DiametroEffettivo":
                //    {
                //        error = InputCheck.MaggioreDiZero(DiametroEffettivo);
                //    }
                //    break;

                //case "DiametroIngombro":
                //    {
                //        if (DiametroIngombro < DiametroEffettivo)
                //            error = "Incorrect diameter value ";
                //    }
                //    break;

                //case "RaggioInserto":
                //    {
                //        error = InputCheck.MaggioreOUgualeDiZero(RaggioInserto);
                //    } break;

                //default:
                //    Debug.Fail("Unexpected property : " + propertyName);
                //    break;
            }

            return error;
        }

        #endregion


    }
}


