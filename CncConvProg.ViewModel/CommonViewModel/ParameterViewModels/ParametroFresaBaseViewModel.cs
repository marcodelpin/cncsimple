using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.CommonViewModel.ParameterViewModels
{
    public class ParametroFresaBaseViewModel : ToolParameterViewModel
    {
        private ParametroFresaBase ParametroFresaBase
        {
            get { return Parametro as ParametroFresaBase; }
        }


        public ParametroFresaBaseViewModel(Utensile parametroFresaBase)
            : base(parametroFresaBase)
        {

            NumeroGiri = new UserInputViewModel(ParametroFresaBase.NumeroGiri, GetValidationError, PropNumeroGiri);
            VelocitaTaglio = new UserInputViewModel(ParametroFresaBase.VelocitaTaglio, GetValidationError, PropVelocitaTaglio);

            LarghezzaPassata = new UserInputViewModel(ParametroFresaBase.LarghezzaPassata, GetValidationError, PropLarghezzaPassata);
            LarghezzaPassataPerc = new UserInputViewModel(ParametroFresaBase.LarghezzaPassataPerc, GetValidationError, PropLarghezzaPassataPerc);
            ProfonditaPassata = new UserInputViewModel(ParametroFresaBase.ProfonditaPassata, GetValidationError, PropProfPassata);
            ProfonditaPassataPerc = new UserInputViewModel(ParametroFresaBase.ProfonditaPassataPerc, GetValidationError, PropProfPassataPerc);

            AvanzamentoSincrono = new UserInputViewModel(ParametroFresaBase.AvanzamentoSincrono, GetValidationError, PropAvanzamentoSincrono);
            AvanzamentoAsincrono = new UserInputViewModel(ParametroFresaBase.AvanzamentoAsincrono, GetValidationError, PropAvanzamentoAsincrono);

            AvanzamentoSincronoPiantata = new UserInputViewModel(ParametroFresaBase.AvanzamentoSincronoPiantata, GetValidationError, PropAvanzamentoSincronoPiantata);
            AvanzamentoAsincronoPiantata = new UserInputViewModel(ParametroFresaBase.AvanzamentoAsincronoPiantata, GetValidationError, PropAvanzamentoAsincronoPiantata);

            AvanzamentoSincronoPiantata.OnSourceUpdated += UserInput_SourceUpdated;
            AvanzamentoAsincronoPiantata.OnSourceUpdated += UserInput_SourceUpdated;

            NumeroGiri.OnSourceUpdated += UserInput_SourceUpdated;
            VelocitaTaglio.OnSourceUpdated += UserInput_SourceUpdated;

            LarghezzaPassata.OnSourceUpdated += UserInput_SourceUpdated;
            LarghezzaPassataPerc.OnSourceUpdated += UserInput_SourceUpdated;
            ProfonditaPassata.OnSourceUpdated += UserInput_SourceUpdated;
            ProfonditaPassataPerc.OnSourceUpdated += UserInput_SourceUpdated;

            AvanzamentoSincrono.OnSourceUpdated += UserInput_SourceUpdated;
            AvanzamentoAsincrono.OnSourceUpdated += UserInput_SourceUpdated;

        }

        private double Diametro
        {
            get
            {
                var tool = ParametroFresaBase.Utensile as FresaBase;

                if (tool != null)
                    return tool.Diametro;

                throw new NullReferenceException();
            }
        }

        void UserInput_SourceUpdated(object sender, EventArgs e)
        {
            if (sender == NumeroGiri)
            {
                if (NumeroGiri.Value.HasValue)
                    ParametroFresaBase.SetNumeroGiri(NumeroGiri.Value.Value);

                VelocitaTaglio.Update();
            }

            else if (sender == VelocitaTaglio)
            {
                if (VelocitaTaglio.Value.HasValue)
                    ParametroFresaBase.SetVelocitaTaglio(VelocitaTaglio.Value.Value);

                NumeroGiri.Update();

            }
            else if (sender == LarghezzaPassata)
            {
                LarghezzaPassataPerc.SetValue(false, (LarghezzaPassata.Value / Diametro) * 100);
            }

            else if (sender == LarghezzaPassataPerc)
            {
                LarghezzaPassata.SetValue(false, (LarghezzaPassataPerc.Value / 100) * Diametro);
            }
            else if (sender == ProfonditaPassata)
            {
                ProfonditaPassataPerc.SetValue(false, (ProfonditaPassata.Value / Diametro) * 100);
            }

            else if (sender == ProfonditaPassataPerc)
            {
                ProfonditaPassata.SetValue(false, (ProfonditaPassataPerc.Value / 100) * Diametro);
            }

            else if (sender == AvanzamentoSincrono)
            {
                if (AvanzamentoSincrono.Value.HasValue)
                    ParametroFresaBase.SetFeedSync(AvanzamentoSincrono.Value.Value);

                AvanzamentoAsincrono.Update();
            }
            else if (sender == AvanzamentoAsincrono)
            {
                if (AvanzamentoAsincrono.Value.HasValue)
                    ParametroFresaBase.SetFeedAsync(AvanzamentoAsincrono.Value.Value);

                AvanzamentoSincrono.Update();
            }

            else if (sender == AvanzamentoSincronoPiantata)
            {
                if (AvanzamentoSincronoPiantata.Value.HasValue)
                    ParametroFresaBase.SetPlungeFeedSync(AvanzamentoSincronoPiantata.Value.Value);

                AvanzamentoAsincronoPiantata.Update();
            }
            else if (sender == AvanzamentoAsincronoPiantata)
            {
                if (AvanzamentoAsincronoPiantata.Value.HasValue)
                    ParametroFresaBase.SetPlungeFeedAsync(AvanzamentoAsincronoPiantata.Value.Value);

                AvanzamentoSincronoPiantata.Update();
            }

            RequestUpdate(this);


            // OnPropertyChanged("IsValid");
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
                return new[]{
                                                     PropNumeroGiri,
                                                     PropVelocitaTaglio,
                                                     PropLarghezzaPassata,
                                                     PropLarghezzaPassataPerc,
                                                     PropProfPassata,
                                                     PropProfPassataPerc,
                                                     PropAvanzamentoSincrono,
                                                     PropAvanzamentoAsincrono,
                                                     PropAvanzamentoAsincronoPiantata,
                                                     PropAvanzamentoSincronoPiantata,
                               

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

                case PropAvanzamentoAsincrono:
                    {
                        error = InputCheck.MaggioreDiZero(AvanzamentoAsincrono.Value);
                    } break;

                case PropAvanzamentoSincronoPiantata:
                    {
                        error = ErrorAvanzamentoSincronoPiantata();
                        //error = InputCheck.MaggioreDiZero(AvanzamentoSincronoPiantata.Value);

                    } break;

                case PropAvanzamentoAsincronoPiantata:
                    {
                        //error = InputCheck.MaggioreDiZero(AvanzamentoAsincronoPiantata.Value);
                        error = ErrorAvanzamentoAsincronoPiantata();
                    } break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion

        protected virtual string ErrorAvanzamentoSincronoPiantata()
        {
            return null;
        }

        protected virtual string ErrorAvanzamentoAsincronoPiantata()
        {
            return null;
        }
    }


}

