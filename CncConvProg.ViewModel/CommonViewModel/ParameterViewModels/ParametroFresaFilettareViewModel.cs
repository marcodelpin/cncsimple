using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.CommonViewModel.ParameterViewModels
{
    public class ParametroFresaFilettareViewModel : ToolParameterViewModel
    {
        private readonly ParametroFresaFilettare _parametroFresaCandela;

        public ParametroFresaFilettareViewModel(ParametroFresaFilettare parametroFresaCandela, MeasureUnit measureUnit)
            : base(parametroFresaCandela)
        {
            _parametroFresaCandela = parametroFresaCandela;

            NumeroGiri = new UserInputViewModel(_parametroFresaCandela.NumeroGiri, GetValidationError, PropNumeroGiri);
            VelocitaTaglio = new UserInputViewModel(_parametroFresaCandela.VelocitaTaglio, GetValidationError, PropVelocitaTaglio);

            LarghezzaPassata = new UserInputViewModel(_parametroFresaCandela.LarghezzaPassata, GetValidationError, PropLarghezzaPassata);
            LarghezzaPassataPerc = new UserInputViewModel(_parametroFresaCandela.LarghezzaPassataPerc, GetValidationError, PropLarghezzaPassataPerc);
            ProfonditaPassata = new UserInputViewModel(_parametroFresaCandela.ProfonditaPassata, GetValidationError, PropProfPassata);
            ProfonditaPassataPerc = new UserInputViewModel(_parametroFresaCandela.ProfonditaPassataPerc, GetValidationError, PropProfPassataPerc);

            AvanzamentoSincrono = new UserInputViewModel(_parametroFresaCandela.AvanzamentoSincrono, GetValidationError, PropAvanzamentoSincrono);
            AvanzamentoAsincrono = new UserInputViewModel(_parametroFresaCandela.AvanzamentoAsincrono, GetValidationError, PropAvanzamentoAsincrono);
            AvanzamentoSincronoPiantata = new UserInputViewModel(_parametroFresaCandela.AvanzamentoSincronoPiantata, GetValidationError, PropAvanzamentoSincronoPiantata);
            AvanzamentoAsincronoPiantata = new UserInputViewModel(_parametroFresaCandela.AvanzamentoAsincronoPiantata, GetValidationError, PropAvanzamentoAsincronoPiantata);

            NumeroGiri.OnSourceUpdated += UserInput_SourceUpdated;
            VelocitaTaglio.OnSourceUpdated += UserInput_SourceUpdated;

            LarghezzaPassata.OnSourceUpdated += UserInput_SourceUpdated;
            LarghezzaPassataPerc.OnSourceUpdated += UserInput_SourceUpdated;
            ProfonditaPassata.OnSourceUpdated += UserInput_SourceUpdated;
            ProfonditaPassataPerc.OnSourceUpdated += UserInput_SourceUpdated;

            AvanzamentoSincrono.OnSourceUpdated += UserInput_SourceUpdated;
            AvanzamentoAsincrono.OnSourceUpdated += UserInput_SourceUpdated;
            AvanzamentoSincronoPiantata.OnSourceUpdated += UserInput_SourceUpdated;
            AvanzamentoAsincronoPiantata.OnSourceUpdated += UserInput_SourceUpdated;

        }

        private double Diametro
        {
            get
            {
                var tool = _parametroFresaCandela.Utensile as FresaCandela;

                if (tool != null)
                    return tool.Diametro;

                throw new NullReferenceException();
            }
        }

        void UserInput_SourceUpdated(object sender, EventArgs e)
        {
            //// ESSENDO BINDATO CON DOUBLE .  QUANDO NON IMMETTO NIENTE VIENE SEGNALATO ERRORE.
            //if (sender == NumeroGiri)
            //{
            //    var numeroGiri = NumeroGiri.Value;
            //    var velocitaTaglio = (numeroGiri * Diametro * Math.PI) / 1000;

            //    VelocitaTaglio.SetValue(false, velocitaTaglio);
            //}

            //else if (sender == VelocitaTaglio)
            //{
            //    var vt = VelocitaTaglio.Value;
            //    var numeroGiri = (vt * 1000) / (Math.PI * Diametro);

            //    NumeroGiri.SetValue(false, numeroGiri);
            //}
            //else if (sender == LarghezzaPassata)
            //{
            //    LarghezzaPassataPerc.SetValue(false, (LarghezzaPassata.Value / Diametro) * 100);
            //}

            //else if (sender == LarghezzaPassataPerc)
            //{
            //    LarghezzaPassata.SetValue(false, (LarghezzaPassataPerc.Value / 100) * Diametro);
            //}
            //else if (sender == ProfonditaPassata)
            //{
            //    ProfonditaPassataPerc.SetValue(false, (ProfonditaPassata.Value / Diametro) * 100);
            //}

            //else if (sender == ProfonditaPassataPerc)
            //{
            //    ProfonditaPassata.SetValue(false, (ProfonditaPassataPerc.Value / 100) * Diametro);
            //}

            //else if (sender == AvanzamentoSincrono)
            //{
            //    AvanzamentoAsincrono.SetValue(false, AvanzamentoSincrono.Value * NumeroGiri.Value);
            //}
            //else if (sender == AvanzamentoAsincrono)
            //{
            //    AvanzamentoSincrono.SetValue(false, AvanzamentoAsincrono.Value / NumeroGiri.Value);
            //}

            //else if (sender == AvanzamentoSincronoPiantata)
            //{
            //    AvanzamentoAsincronoPiantata.SetValue(false, AvanzamentoSincronoPiantata.Value * NumeroGiri.Value);
            //}
            //else if (sender == AvanzamentoAsincronoPiantata)
            //{
            //    AvanzamentoSincronoPiantata.SetValue(false, AvanzamentoAsincronoPiantata.Value / NumeroGiri.Value);
            //}

            //OnPropertyChanged("IsValid");

        }

        #region PropertyName

        private const string PropNumeroGiri = "NumeroGiri";
        private const string PropVelocitaTaglio = "VelocitaTaglio";
        private const string PropLarghezzaPassata = "LarghezzaPassata";
        private const string PropLarghezzaPassataPerc = "LarghezzaPassataPerc";
        private const string PropProfPassata = "ProfonditaPassata";
        private const string PropProfPassataPerc = "ProfonditaPassataPerc";
        private const string PropAvanzamentoSincrono = "AvanzamentoSincrono";
        private const string PropAvanzamentoAsincrono = "AvanzamentoAsincrono";
        private const string PropAvanzamentoSincronoPiantata = "AvanzamentoSincronoPiantata";
        private const string PropAvanzamentoAsincronoPiantata = "AvanzamentoAsincronoPiantata";

        #endregion

        #region Property

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
        private UserInputViewModel _avanzamentoAsincrono;
        public UserInputViewModel AvanzamentoAsincrono
        {
            get { return _avanzamentoAsincrono; }
            set
            {
                _avanzamentoAsincrono = value;
                OnPropertyChanged("AvanzamentoAsincrono");
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
        private UserInputViewModel _avanzamentoAsincronoPiantata;
        public UserInputViewModel AvanzamentoAsincronoPiantata
        {
            get { return _avanzamentoAsincronoPiantata; }
            set
            {
                _avanzamentoAsincronoPiantata = value;
                OnPropertyChanged("AvanzamentoAsincronoPiantata");
            }
        }

        #endregion


        #region IDataErrorInfo Members

        protected override string[] ValidatedProperties
        {
            get
            {
                return new string[]
                            {                     "Diametro",
                                                     PropNumeroGiri,
                                                     PropVelocitaTaglio,
                                                     PropLarghezzaPassata,
                                                     PropLarghezzaPassataPerc,
                                                     PropProfPassata,
                                                     PropProfPassataPerc,
                                                     PropAvanzamentoSincrono,
                                                     PropAvanzamentoAsincrono,
                                                     PropAvanzamentoSincronoPiantata,
                                                     PropAvanzamentoAsincronoPiantata
                                
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
                //case "Diametro":
                //    {
                //        error = InputCheck.MaggioreDiZero(Diametro.ToString());
                //    }
                //    break;

                //case PropNumeroGiri:
                //    {
                //        error = InputCheck.MaggioreDiZero(NumeroGiri.Value);
                //    }
                //    break;
                //case PropVelocitaTaglio:
                //    {
                //        error = InputCheck.MaggioreDiZero(VelocitaTaglio.Value);
                //    }
                //    break;

                //case PropLarghezzaPassata:
                //    {
                //        error = InputCheck.MaggioreDiZero(LarghezzaPassata.Value);

                //    } break;

                //case PropLarghezzaPassataPerc:
                //    {
                //        error = InputCheck.MaggioreDiZero(LarghezzaPassataPerc.Value);
                //    } break;

                //case PropProfPassata:
                //    {
                //        error = InputCheck.MaggioreDiZero(ProfonditaPassata.Value);
                //    } break;

                //case PropProfPassataPerc:
                //    {
                //        error = InputCheck.MaggioreDiZero(ProfonditaPassataPerc.Value);
                //    } break;

                //case PropAvanzamentoSincrono:
                //    {
                //        error = InputCheck.MaggioreDiZero(AvanzamentoSincrono.Value);
                //    } break;

                //case PropAvanzamentoAsincrono:
                //    {
                //        error = InputCheck.MaggioreDiZero(AvanzamentoAsincrono.Value);
                //    } break;

                //case PropAvanzamentoSincronoPiantata:
                //    {
                //        error = InputCheck.MaggioreDiZero(AvanzamentoSincronoPiantata.Value);

                //    } break;

                //case PropAvanzamentoAsincronoPiantata:
                //    {
                //        error = InputCheck.MaggioreDiZero(AvanzamentoAsincronoPiantata.Value);
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

