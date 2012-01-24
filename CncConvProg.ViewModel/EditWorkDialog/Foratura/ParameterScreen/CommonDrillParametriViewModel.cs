using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.ParameterScreen
{
    /// <summary>
    /// Classe base per screen parametri nelle lavorazioni di foratura
    /// </summary>
    public class CommonDrillParametriViewModel : EditStageTreeViewItem, IDataErrorInfo, IValid
    {
        private readonly DrillBaseClass _drillBaseClass;

        public CommonDrillParametriViewModel(EditWorkViewModel parent, DrillBaseClass drillBaseClass, string label)
            : base(label, parent)
        {
            _drillBaseClass = drillBaseClass;

            EditWorkParent = parent;
        }

        public double ProfonditaForatura
        {
            get { return _drillBaseClass.ProfonditaForatura; }
            set
            {
                _drillBaseClass.ProfonditaForatura = value;
                OnPropertyChanged("ProfonditaForatura");
            }
        }

        public double SicurezzaZ
        {
            get { return _drillBaseClass.SicurezzaZ; }
            set
            {
                _drillBaseClass.SicurezzaZ = value;
                OnPropertyChanged("SicurezzaZ");
            }
        }

        public double DiametroForatura
        {
            get { return _drillBaseClass.DiametroForatura; }
            set
            {
                _drillBaseClass.DiametroForatura = value;
                OnPropertyChanged("DiametroForatura");
            }
        }

        public double InizioZ
        {
            get { return _drillBaseClass.InizioZ; }
            set
            {
                _drillBaseClass.InizioZ = value;
                OnPropertyChanged("InizioZ");
                //SourceUpdated();

            }
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

        protected virtual string[] ValidatedProperties
        {
            get
            {
                return new[]
                       {
                           "SicurezzaZ",
                           "InizioZ"
                       };
            }
        }

        protected virtual string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "SicurezzaZ":
                    {
                        if (SicurezzaZ <= InizioZ)
                            error = "Non Valido";
                    }
                    break;
                case "InizioZ":
                    {

                    }
                    break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion
    }
}
