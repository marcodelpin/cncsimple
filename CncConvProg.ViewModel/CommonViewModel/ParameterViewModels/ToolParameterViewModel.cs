using System;
using System.ComponentModel;
using System.Linq;
using CncConvProg.Model;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.LatheTool;
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

        private readonly ParametroUtensile _parametro;
        private readonly MeasureUnit _measureUnit;

        protected ToolParameterViewModel(ParametroUtensile parametroUtensile)
        {
            _parametro = parametroUtensile;
            _measureUnit = _parametro.Utensile.Unit;
        }

        public static ToolParameterViewModel GetViewModel(ParametroUtensile parametroUtensile, MeasureUnit measureUnit)
        {
            /*todo : lo screen per utensile viene rigenerato 10 volte, ogni volta che viene controllato un elemento..*/

            if (parametroUtensile is ParametroPunta && parametroUtensile.Utensile is DrillTool)
                return new ParametroPuntaViewModel(parametroUtensile as ParametroPunta, parametroUtensile.Utensile as DrillTool, measureUnit);

            if (parametroUtensile is ParametroUtensileTornitura && parametroUtensile.Utensile is UtensileTornitura)
                return new ParametroUtensileTornituraViewModel(parametroUtensile as ParametroUtensileTornitura, measureUnit);

            if (parametroUtensile is ParametroFresaCandela)
                return new ParametroFresaCandelaViewModel(parametroUtensile as ParametroFresaCandela);

            if (parametroUtensile is ParametroFresaFilettare)
                return new ParametroFresaFilettareViewModel(parametroUtensile as ParametroFresaFilettare, measureUnit);

            if (parametroUtensile is ParametroFresaSpianare)
                return new ParametroFresaSpianareViewModel(parametroUtensile as ParametroFresaSpianare);

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
