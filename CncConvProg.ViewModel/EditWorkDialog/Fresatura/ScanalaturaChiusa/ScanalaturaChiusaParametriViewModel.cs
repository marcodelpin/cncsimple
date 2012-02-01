using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaChiusa
{
    public class ScanalaturaChiusaParametriViewModel : EditStageTreeViewItem, IDataErrorInfo
    {
        private readonly FresaturaScanalaturaChiusa _fresaturaCava;

        public ScanalaturaChiusaParametriViewModel(FresaturaScanalaturaChiusa fresaturaCava, EditWorkViewModel treeItemParent)
            : base("Parameter", treeItemParent)
        {
            _fresaturaCava = fresaturaCava;

            RotoTraslateWorkViewModel = new RotoTraslateWorkViewModel(this._fresaturaCava, this);
        }

        private RotoTraslateWorkViewModel _rotoTraslateWorkViewModel;
        public RotoTraslateWorkViewModel RotoTraslateWorkViewModel
        {
            get { return _rotoTraslateWorkViewModel; }
            set
            {
                _rotoTraslateWorkViewModel = value;
                OnPropertyChanged("RotoTraslateWorkViewModel");
            }
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

        public double Larghezza
        {
            get { return _fresaturaCava.Larghezza; }
            set
            {
                _fresaturaCava.Larghezza = value;
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

        public override bool? ValidateStage()
        {
            return ValidatedProperties.All(property => GetValidationError(property) == null);
        }



        protected string[] ValidatedProperties = {
                                                     "Profondita",
                                                     "SicurezzaZ",
                                                     "Larghezza"
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Larghezza":
                    {
                        error = InputCheck.MaggioreDiZero(Larghezza.ToString());
                    }
                    break;

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
