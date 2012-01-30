using System;
using System.ComponentModel;
using System.Linq;
using CncConvProg.Model;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.LatheTool;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.CommonViewModel.ParameterViewModels
{
    public abstract class ToolParameterViewModel : ViewModelBase, IValid, IDataErrorInfo
    {

        public string VelocitaTaglioLabel
        {
            get
            {
                if (_measureUnit == MeasureUnit.Millimeter)
                    return "mt/min";
                return "ft/min";
            }
        }

        public string FeedSyncLabel
        {
            get
            {
                if (_measureUnit == MeasureUnit.Millimeter)
                    return "mm/rev";
                return "inch/rev";
            }
        }

        public string FeedAsyncLabel
        {
            get
            {
                if (_measureUnit == MeasureUnit.Millimeter)
                    return "mm/min";
                return "inch/min";
            }
        }

        public string Unit
        {
            get
            {
                if (_measureUnit == MeasureUnit.Millimeter)
                    return "[mm]";
                return "[Inch]";
            }
        }

        public ParametroUtensile Parametro
        {
            get { return Utensile.ParametroUtensile; }
        }

        private readonly MeasureUnit _measureUnit;

        public readonly Utensile Utensile;

        protected ToolParameterViewModel(Utensile utensile)
        {
            Utensile = utensile;
            _measureUnit = Utensile.Unit;
        }

        public static ToolParameterViewModel GetViewModel(Utensile utensile)
        {
            if (utensile is DrillTool)
                return new ParametroPuntaViewModel(utensile as DrillTool);

            if (utensile is FresaCandela)
                return new ParametroFresaCandelaViewModel(utensile as FresaCandela);

            if (utensile is FresaFilettare)
                return new ParametroFresaFilettareViewModel(utensile as FresaFilettare);

            if (utensile is FresaSpianare)
                return new ParametroFresaSpianareViewModel(utensile as FresaSpianare);

            throw new NotImplementedException();
        }

        public event EventHandler OnUpdated;

        protected void RequestUpdate(ViewModelBase caller)
        {
            var handler = OnUpdated;
            if (handler != null)
                handler(caller, EventArgs.Empty);
        }

        //protected override void OnPropertyChanged(string propertyName)
        //{
        //    if (propertyName != "IsValid")
        //    {
        //        OnPropertyChanged("IsValid");
        //        RequestUpdate(this);
        //    }
        //    base.OnPropertyChanged(propertyName);
        //}

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

        protected abstract string[] ValidatedProperties { get; }

        protected abstract string GetValidationError(string propertyName);

        #endregion
    }
}
