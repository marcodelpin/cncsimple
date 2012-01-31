using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

/*
 * fare classe comune con screen ,
 * 
 * non dovrei ripetere il il collegamento , per riaggiornare 
 
 */
namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Lato
{
    public class FresaturaLatoParametriViewModel : EditStageTreeViewItem, IDataErrorInfo, IValid
    {
        private readonly Model.ConversationalStructure.Lavorazioni.Fresatura.FresaturaLato _fresaturaLato;

        public FresaturaLatoParametriViewModel(EditWorkViewModel parent, Model.ConversationalStructure.Lavorazioni.Fresatura.FresaturaLato fresaturaLato)
            : base("Parameter", parent)
        {
            _fresaturaLato = fresaturaLato;

            EditWorkParent = parent;

            RotoTraslateWorkViewModel = new RotoTraslateWorkViewModel(this._fresaturaLato, this);
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

        public double PuntoInizialeX
        {
            get { return _fresaturaLato.PuntoInizialeX; }
            set
            {
                _fresaturaLato.PuntoInizialeX = value;
                OnPropertyChanged("PuntoInizialeX");
                SourceUpdated();

            }
        }

        public double PuntoInizialeY
        {
            get { return _fresaturaLato.PuntoInizialeY; }
            set
            {
                _fresaturaLato.PuntoInizialeY = value;
                OnPropertyChanged("PuntoInizialeY");
                SourceUpdated();

            }
        }

        public double OrientationAngle
        {
            get
            {
                return _fresaturaLato.OrientationAngle;
            }
            set
            {
                _fresaturaLato.OrientationAngle = value; ;

                OnPropertyChanged("OrientationAngle");
            }
        }

        public double Lunghezza
        {
            get { return _fresaturaLato.Lunghezza; }
            set
            {
                _fresaturaLato.Lunghezza = value;
                OnPropertyChanged("Lunghezza");
                SourceUpdated();

            }
        }

        public double Sovrametallo
        {
            get { return _fresaturaLato.Sovrametallo; }
            set
            {
                _fresaturaLato.Sovrametallo = value;
                OnPropertyChanged("Sovrametallo");
                SourceUpdated();

            }
        }

        public double SovrametalloPerFinitura
        {
            get { return _fresaturaLato.SovrametalloFinituraProfilo; }
            set
            {
                _fresaturaLato.SovrametalloFinituraProfilo = value;
                OnPropertyChanged("SovrametalloPerFinitura");
                SourceUpdated();

            }
        }

        public double SicurezzaZ
        {
            get { return _fresaturaLato.SicurezzaZ; }
            set
            {
                _fresaturaLato.SicurezzaZ = value;
                OnPropertyChanged("SicurezzaZ");
                SourceUpdated();

            }
        }

        public double InizioLavorazioneZ
        {
            get { return _fresaturaLato.InizioLavorazioneZ; }
            set
            {
                _fresaturaLato.InizioLavorazioneZ = value;
                OnPropertyChanged("InizioLavorazioneZ");
                SourceUpdated();

            }
        }

        public double ProfonditaLavorazioneZ
        {
            get { return _fresaturaLato.ProfonditaLavorazione; }
            set
            {
                _fresaturaLato.ProfonditaLavorazione = value;
                OnPropertyChanged("ProfonditaLavorazioneZ");
                SourceUpdated();

            }
        }

        public bool FinishWithCompensation
        {
            get { return _fresaturaLato.FinishWithCompensation; }

            set
            {
                _fresaturaLato.FinishWithCompensation = value;
                OnPropertyChanged("FinishWithCompensation");

            }
        }

        public double SovrametalloFinitura
        {
            get { return _fresaturaLato.SovrametalloFinituraProfilo; }
            set
            {
                _fresaturaLato.SovrametalloFinituraProfilo = value;
                OnPropertyChanged("SovrametalloFinitura");
            }
        }

        public double ProfonditaFresaSmussatura
        {
            get { return _fresaturaLato.ProfonditaFresaSmussatura; }
            set
            {
                _fresaturaLato.ProfonditaFresaSmussatura = value;
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
                                                    "Sovrametallo",
                                                    //"SovrametalloPerFinitura",
                                                    "SicurezzaZ",
                                                    "Lunghezza",
                                                    "ProfonditaLavorazioneZ",
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {

                //case "SovrametalloPerFinitura":
                //    {
                //        error = InputCheck.MaggioreOUgualeDiZero(SovrametalloPerFinitura);
                //    }
                //    break;

                case "Sovrametallo":
                    {
                           error = InputCheck.MaggioreDiZero(Sovrametallo);
                    }
                    break;

                case "SicurezzaZ":
                    {
                        if (SicurezzaZ <= InizioLavorazioneZ)
                            error = "Z Secure Level Too Low";
                    }
                    break;

                case "Lunghezza":
                    {
                        error = InputCheck.MaggioreDiZero(Lunghezza);

                    } break;

                case "ProfonditaLavorazioneZ":
                    {
                        error = InputCheck.MaggioreDiZero(ProfonditaLavorazioneZ);
                        
                    }break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion
    }
}
