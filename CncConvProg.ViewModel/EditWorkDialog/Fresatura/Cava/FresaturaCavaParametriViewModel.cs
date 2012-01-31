using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Cava
{
    public class FresaturaCavaParametriViewModel : EditStageTreeViewItem, IDataErrorInfo, IValid
    {
        private readonly FresaturaCava _fresaturaCava;

        public FresaturaCavaParametriViewModel(FresaturaCava fresaturaCava, EditWorkViewModel treeItemParent)
            : base("Parameter", treeItemParent)
        {
            _fresaturaCava = fresaturaCava;

        }
        
        public double Profondita
        {
            get
            {
                return _fresaturaCava.ProfonditaLavorazione;
            }
            set
            {
                _fresaturaCava.ProfonditaLavorazione = value;
                OnPropertyChanged("Profondita");
            }
        }

        public double SicurezzaZ
        {
            get { return _fresaturaCava.SicurezzaZ; }
            set
            {
                _fresaturaCava.SicurezzaZ = value;
                OnPropertyChanged("SicurezzaZ");
            }
        }

        public double InizioZ
        {
            get { return _fresaturaCava.InizioLavorazioneZ; }
            set
            {
                _fresaturaCava.InizioLavorazioneZ = value;
                OnPropertyChanged("InizioZ");
            }
        }

        public bool FinishWithCompensation
        {
            get { return _fresaturaCava.FinishWithCompensation; }

            set
            {
                _fresaturaCava.FinishWithCompensation = value;
                OnPropertyChanged("FinishWithCompensation");

            }
        }

        public double SovrametalloFinitura
        {
            get { return _fresaturaCava.SovrametalloFinituraProfilo; }
            set
            {
                _fresaturaCava.SovrametalloFinituraProfilo = value;
                OnPropertyChanged("SovrametalloFinitura");
            }
        }

        public double ProfonditaFresaSmussatura
        {
            get { return _fresaturaCava.ProfonditaFresaSmussatura; }
            set
            {
                _fresaturaCava.ProfonditaFresaSmussatura = value;
                OnPropertyChanged("ProfonditaFresaSmussatura");
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


        protected string[] ValidatedProperties = {
                                                     "Profondita",
                                                     "SicurezzaZ"
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Profondita":
                    {
                        error = InputCheck.MaggioreDiZero(Profondita.ToString());
                    }
                    break;

                case "SicurezzaZ":
                    {
                        if (SicurezzaZ <= InizioZ)
                            error = "Must be higher than StartZ";
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
