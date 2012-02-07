using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

/*
 * fare classe comune con screen ,
 * 
 * non dovrei ripetere il il collegamento , per riaggiornare 
 
 */
namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaLinea
{
    public class ScanalaturaLineaParametriViewModel : EditStageTreeViewItem, IDataErrorInfo
    {
        private readonly Model.ConversationalStructure.Lavorazioni.Fresatura.ScanalaturaLinea _scanalaturaLinea;

        public ScanalaturaLineaParametriViewModel(EditWorkViewModel parent, Model.ConversationalStructure.Lavorazioni.Fresatura.ScanalaturaLinea scanalaturaLinea)
            : base("Parameter", parent)
        {
            /*
             * fare classe base per gestire la rotazione e array..
             */
            _scanalaturaLinea = scanalaturaLinea;

            EditWorkParent = parent;


            RotoTraslateWorkViewModel = new RotoTraslateWorkViewModel(_scanalaturaLinea, this);

            Children.Add(RotoTraslateWorkViewModel);
            //RotoTraslateWorkViewModel = new RotoTraslateWorkViewModel(this._scanalaturaLinea, this);
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
            get { return _scanalaturaLinea.PuntoInizialeX; }
            set
            {
                _scanalaturaLinea.PuntoInizialeX = value;
                OnPropertyChanged("PuntoInizialeX");
                SourceUpdated();

            }
        }

        public double PuntoInizialeY
        {
            get { return _scanalaturaLinea.PuntoInizialeY; }
            set
            {
                _scanalaturaLinea.PuntoInizialeY = value;
                OnPropertyChanged("PuntoInizialeY");
                SourceUpdated();

            }
        }

        public double OrientationAngle
        {
            get
            {
                return _scanalaturaLinea.OrientationAngle;
            }
            set
            {
                _scanalaturaLinea.OrientationAngle = value; ;

                OnPropertyChanged("OrientationAngle");
            }
        }

        public double LunghezzaCava
        {
            get { return _scanalaturaLinea.LunghezzaCava; }
            set
            {
                _scanalaturaLinea.LunghezzaCava = value;
                OnPropertyChanged("LunghezzaCava");
                SourceUpdated();

            }
        }

        public double LarghezzaCava
        {
            get { return _scanalaturaLinea.LarghezzaCava; }
            set
            {
                _scanalaturaLinea.LarghezzaCava = value;
                OnPropertyChanged("LarghezzaCava");
                SourceUpdated();

            }
        }

        public double SovrametalloPerFinitura
        {
            get { return _scanalaturaLinea.SovrametalloFinituraProfilo; }
            set
            {
                _scanalaturaLinea.SovrametalloFinituraProfilo = value;
                OnPropertyChanged("SovrametalloFinitura");
                SourceUpdated();

            }
        }

        public double SicurezzaZ
        {
            get { return _scanalaturaLinea.SicurezzaZ; }
            set
            {
                _scanalaturaLinea.SicurezzaZ = value;
                OnPropertyChanged("SicurezzaZ");
                SourceUpdated();

            }
        }

        public double InizioLavorazioneZ
        {
            get { return _scanalaturaLinea.InizioLavorazioneZ; }
            set
            {
                _scanalaturaLinea.InizioLavorazioneZ = value;
                OnPropertyChanged("InizioLavorazioneZ");
                SourceUpdated();

            }
        }

        public double ProfonditaLavorazioneZ
        {
            get { return _scanalaturaLinea.ProfonditaLavorazione; }
            set
            {
                _scanalaturaLinea.ProfonditaLavorazione = value;
                OnPropertyChanged("ProfonditaLavorazioneZ");
                SourceUpdated();

            }
        }

        public bool FinishWithCompensation
        {
            get { return _scanalaturaLinea.FinishWithCompensation; }

            set
            {
                _scanalaturaLinea.FinishWithCompensation = value;
                OnPropertyChanged("FinishWithCompensation");

            }
        }

        public double SovrametalloFinitura
        {
            get { return _scanalaturaLinea.SovrametalloFinituraProfilo; }
            set
            {
                _scanalaturaLinea.SovrametalloFinituraProfilo = value;
                OnPropertyChanged("SovrametalloFinitura");
            }
        }

        public double ProfonditaFresaSmussatura
        {
            get { return _scanalaturaLinea.ProfonditaFresaSmussatura; }
            set
            {
                _scanalaturaLinea.ProfonditaFresaSmussatura = value;
                OnPropertyChanged("ProfonditaFresaSmussatura");
            }
        }

        public double TrochoidalStep
        {
            get { return _scanalaturaLinea.TrochoidalStep; }
            set
            {
                _scanalaturaLinea.TrochoidalStep = value;
                OnPropertyChanged("TrochoidalStep");
            }
        }

        public int ModoSgrossatura
        {
            get
            {
                return (int)_scanalaturaLinea.ModoSgrossatura;
            }

            set
            {
                _scanalaturaLinea.ModoSgrossatura = (ScanalaturaCavaMetodoLavorazione)value;
                OnPropertyChanged("ModoSgrossatura");
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
                                                    "LunghezzaCava",
                                                    "SicurezzaZ",
                                                    "ProfonditaLavorazioneZ",
                                                    "LarghezzaCava",
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {

                case "LarghezzaCava":
                    {
                        error = InputCheck.MaggioreDiZero(LarghezzaCava);
                    }
                    break;

                case "SicurezzaZ":
                    {
                        if (InizioLavorazioneZ >= SicurezzaZ)
                            error = "Z Secure Level Too Low";
                    }
                    break;

                case "ProfonditaLavorazioneZ":
                    {
                        error = InputCheck.MaggioreDiZero(ProfonditaLavorazioneZ);

                    } break;

                case "LunghezzaCava":
                    {
                        error = InputCheck.MaggioreDiZero(LunghezzaCava);

                    } break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion
    }
}
