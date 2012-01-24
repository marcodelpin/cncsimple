using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Operazioni;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.OperationViewModel
{
    public class ParametriScanalatoreViewModel : EditStageTreeViewItem, IPreviewable, IDataErrorInfo, IValid
    {
        private readonly OperazioneScanalatore _operazioneScanalatore;

        public ParametriScanalatoreViewModel(OperazioneScanalatore operazioneScanalatore, EditStageTreeViewItem parent)
            : base(operazioneScanalatore.Descrizione, parent)
        {
            _operazioneScanalatore = operazioneScanalatore;

            NumeroGiri = new UserInputViewModel(_operazioneScanalatore.NumeroGiri, GetValidationError, PropNumeroGiri);
            VelocitaTaglio = new UserInputViewModel(_operazioneScanalatore.VelocitaTaglio, GetValidationError, PropVelocitaTaglio);

            LarghezzaPassata = new UserInputViewModel(_operazioneScanalatore.LarghezzaPassata, GetValidationError, PropLarghezzaPassata);
            LarghezzaPassataPerc = new UserInputViewModel(_operazioneScanalatore.LarghezzaPassataPerc, GetValidationError, PropLarghezzaPassataPerc);
            ProfonditaPassata = new UserInputViewModel(_operazioneScanalatore.ProfonditaPassata, GetValidationError, PropProfPassata);
            ProfonditaPassataPerc = new UserInputViewModel(_operazioneScanalatore.ProfonditaPassataPerc, GetValidationError, PropProfPassataPerc);

            AvanzamentoSincrono = new UserInputViewModel(_operazioneScanalatore.AvanzamentoSincrono, GetValidationError, PropAvanzamentoSincrono);
            AvanzamentoSincronoPiantata = new UserInputViewModel(_operazioneScanalatore.AvanzamentoSincronoPiantata, GetValidationError, PropAvanzamentoSincronoPiantata);

            NumeroGiri.OnSourceUpdated += UserInput_SourceUpdated;
            VelocitaTaglio.OnSourceUpdated += UserInput_SourceUpdated;

            LarghezzaPassata.OnSourceUpdated += UserInput_SourceUpdated;
            LarghezzaPassataPerc.OnSourceUpdated += UserInput_SourceUpdated;
            ProfonditaPassata.OnSourceUpdated += UserInput_SourceUpdated;
            ProfonditaPassataPerc.OnSourceUpdated += UserInput_SourceUpdated;

            AvanzamentoSincrono.OnSourceUpdated += UserInput_SourceUpdated;
            AvanzamentoSincronoPiantata.OnSourceUpdated += UserInput_SourceUpdated;

        }

        void UserInput_SourceUpdated(object sender, EventArgs e)
        {
            if (sender == NumeroGiri)
            {
                var numeroGiri = NumeroGiri.Value;
                var velocitaTaglio = (numeroGiri * LarghezzaUtensile * Math.PI) / 1000;

                VelocitaTaglio.SetValue(false, velocitaTaglio);
            }

            else if (sender == VelocitaTaglio)
            {
                var vt = VelocitaTaglio.Value;
                var numeroGiri = (vt * 1000) / (Math.PI * LarghezzaUtensile);

                NumeroGiri.SetValue(false, numeroGiri);
            }
            else if (sender == LarghezzaPassata)
            {
                LarghezzaPassataPerc.SetValue(false, (LarghezzaPassata.Value / LarghezzaUtensile) * 100);
            }

            else if (sender == LarghezzaPassataPerc)
            {
                LarghezzaPassata.SetValue(false, (LarghezzaPassataPerc.Value / 100) * LarghezzaUtensile);
            }
            else if (sender == ProfonditaPassata)
            {
                ProfonditaPassataPerc.SetValue(false, (ProfonditaPassata.Value / LarghezzaUtensile) * 100);
            }

            else if (sender == ProfonditaPassataPerc)
            {
                ProfonditaPassata.SetValue(false, (ProfonditaPassataPerc.Value / 100) * LarghezzaUtensile);
            }

            OnPropertyChanged("IsValid");

            SourceUpdated();
        }

        #region PropertyName

        private const string PropNumeroGiri = "NumeroGiri";
        private const string PropVelocitaTaglio = "VelocitaTaglio";
        private const string PropLarghezzaPassata = "LarghezzaPassata";
        private const string PropLarghezzaPassataPerc = "LarghezzaPassataPerc";
        private const string PropProfPassata = "ProfonditaPassata";
        private const string PropProfPassataPerc = "ProfonditaPassataPerc";
        private const string PropAvanzamentoSincrono = "AvanzamentoSincrono";
        private const string PropAvanzamentoSincronoPiantata = "AvanzamentoSincronoPiantata";

        #endregion

        #region Property
        public double LarghezzaUtensile
        {
            get { return _operazioneScanalatore.LarghezzaUtensile; }
            set
            {
                _operazioneScanalatore.LarghezzaUtensile = value;

                UserInput_SourceUpdated(null, EventArgs.Empty);

                OnPropertyChanged("LarghezzaUtensile");
            }
        }

        private UserInputViewModel _numeroGiri;
        public UserInputViewModel NumeroGiri
        {
            get { return _numeroGiri; }
            set
            {
                _numeroGiri = value;
                OnPropertyChanged("NumeroGiri");
            }
        }
        private UserInputViewModel _velocitaTaglio;
        public UserInputViewModel VelocitaTaglio
        {
            get { return _velocitaTaglio; }
            set
            {
                _velocitaTaglio = value;
                OnPropertyChanged("VelocitaTaglio");
            }
        }
        private UserInputViewModel _larghezzaPassata;
        public UserInputViewModel LarghezzaPassata
        {
            get { return _larghezzaPassata; }
            set { _larghezzaPassata = value; OnPropertyChanged("LarghezzaPassata"); }
        }

        private UserInputViewModel _profonditaPassata;
        public UserInputViewModel ProfonditaPassata
        {
            get { return _profonditaPassata; }
            set
            {
                _profonditaPassata = value;
                OnPropertyChanged("ProfonditaPassata");
            }
        }
        private UserInputViewModel _larghezzaPassataPerc;
        public UserInputViewModel LarghezzaPassataPerc
        {
            get { return _larghezzaPassataPerc; }
            set { _larghezzaPassataPerc = value; OnPropertyChanged("LarghezzaPassataPerc"); }
        }

        private UserInputViewModel _profonditaPassataPerc;
        public UserInputViewModel ProfonditaPassataPerc
        {
            get { return _profonditaPassataPerc; }
            set
            {
                _profonditaPassataPerc = value;
                OnPropertyChanged("ProfonditaPassataPerc");
            }
        }
        private UserInputViewModel _avanzamentoSincrono;
        public UserInputViewModel AvanzamentoSincrono
        {
            get { return _avanzamentoSincrono; }
            set
            {
                _avanzamentoSincrono = value;
                OnPropertyChanged("AvanzamentoSincrono");
            }
        }
     
        private UserInputViewModel _avanzamentoSincronoPiantata;
        public UserInputViewModel AvanzamentoSincronoPiantata
        {
            get { return _avanzamentoSincronoPiantata; }
            set
            {
                _avanzamentoSincronoPiantata = value;
                OnPropertyChanged("AvanzamentoSincronoPiantata");
            }
        }

        #endregion


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
                                                     "Diametro",
                                                     PropNumeroGiri,
                                                     PropVelocitaTaglio,
                                                     PropLarghezzaPassata,
                                                     PropLarghezzaPassataPerc,
                                                     PropProfPassata,
                                                     PropProfPassataPerc,
                                                     PropAvanzamentoSincrono,
                                                     PropAvanzamentoSincronoPiantata,
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Diametro":
                    {
                        error = InputCheck.MaggioreDiZero(LarghezzaUtensile.ToString());
                    }
                    break;

                case PropNumeroGiri:
                    {
                        error = InputCheck.MaggioreDiZero(NumeroGiri.Value);
                    }
                    break;
                case PropVelocitaTaglio:
                    {
                        error = InputCheck.MaggioreDiZero(VelocitaTaglio.Value);
                    }
                    break;

                case PropLarghezzaPassata:
                    {
                        error = InputCheck.MaggioreDiZero(LarghezzaPassata.Value);

                    } break;

                case PropLarghezzaPassataPerc:
                    {
                        error = InputCheck.MaggioreDiZero(LarghezzaPassataPerc.Value);
                    } break;

                case PropProfPassata:
                    {
                        error = InputCheck.MaggioreDiZero(ProfonditaPassata.Value);
                    } break;

                case PropProfPassataPerc:
                    {
                        error = InputCheck.MaggioreDiZero(ProfonditaPassataPerc.Value);
                    } break;

                case PropAvanzamentoSincrono:
                    {
                        error = InputCheck.MaggioreDiZero(AvanzamentoSincrono.Value);
                    } break;

                case PropAvanzamentoSincronoPiantata:
                    {
                        error = InputCheck.MaggioreDiZero(AvanzamentoSincronoPiantata.Value);

                    } break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion

        private ObservableCollection<IEntity2D> _preview;
        public ObservableCollection<IEntity2D> Preview
        {
            get
            {
                return _preview;
            }

            set
            {
                _preview = value;
                OnPropertyChanged("Preview");
            }
        }

        #region Update Preview

        RelayCommand _updatePreview;

        public void UpdatePreview()
        {

                Preview = new ObservableCollection<IEntity2D>(GetPreview());
        }

        public ICommand UpdatePreviewCmd
        {
            get
            {
                return _updatePreview ?? (_updatePreview = new RelayCommand(param => UpdatePreview(),
                                                                            param => true));
            }
        }

        #endregion

        /// <summary>
        /// Questo metodo è uguale in tutti il viewModel per i dati di taglio,
        /// se si fare classe base implementarlo li..
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEntity2D> GetPreview()
        {
            // todo_ se non è valida cercare di restiruire null.

            if (!IsValid) return null;

            return _operazioneScanalatore.GetPathPreview();
        }
    }


}

