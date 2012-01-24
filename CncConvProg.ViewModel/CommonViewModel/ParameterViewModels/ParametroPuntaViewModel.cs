using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.ViewModel.AuxViewModel;

namespace CncConvProg.ViewModel.CommonViewModel.ParameterViewModels
{
    public class ParametroPuntaViewModel : ToolParameterViewModel
    {

        private readonly ParametroPunta _parametroPunta;
        private readonly DrillTool _punta;

        private double Diametro { get { return _punta.Diametro; } }

        public ParametroPuntaViewModel(ParametroPunta parametroPunta, DrillTool punta, MeasureUnit measureUnit )
            : base(parametroPunta)
        {
            _parametroPunta = parametroPunta;
            _punta = punta;

            NumeroGiri = new UserInputViewModel(_parametroPunta.NumeroGiri, GetValidationError, PropNumeroGiri);
            VelocitaTaglio = new UserInputViewModel(_parametroPunta.VelocitaTaglio, GetValidationError, PropVelocitaTaglio);

            AvanzamentoSincrono = new UserInputViewModel(_parametroPunta.AvanzamentoSincrono, GetValidationError, PropAvanzamentoSincrono);
            AvanzamentoAsincrono = new UserInputViewModel(_parametroPunta.AvanzamentoAsincrono, GetValidationError, PropAvanzamentoAsincrono);

            NumeroGiri.OnSourceUpdated += UserInput_SourceUpdated;
            VelocitaTaglio.OnSourceUpdated += UserInput_SourceUpdated;

            AvanzamentoSincrono.OnSourceUpdated += UserInput_SourceUpdated;
            AvanzamentoAsincrono.OnSourceUpdated += UserInput_SourceUpdated;
        }



        void UserInput_SourceUpdated(object sender, EventArgs e)
        {
            if (sender == NumeroGiri)
            {
                if (NumeroGiri.Value.HasValue)
                    _parametroPunta.SetNumeroGiri(NumeroGiri.Value.Value);

                VelocitaTaglio.Update();
            }

            else if (sender == VelocitaTaglio)
            {
                if (VelocitaTaglio.Value.HasValue)
                    _parametroPunta.SetVelocitaTaglio(VelocitaTaglio.Value.Value);

                NumeroGiri.Update();

            }

            else if (sender == AvanzamentoSincrono)
            {
                if (AvanzamentoSincrono.Value.HasValue)
                    _parametroPunta.SetFeedSync(AvanzamentoSincrono.Value.Value);

                AvanzamentoAsincrono.Update();
            }
            else if (sender == AvanzamentoAsincrono)
            {
                if (AvanzamentoAsincrono.Value.HasValue)
                    _parametroPunta.SetFeedAsync(AvanzamentoAsincrono.Value.Value);

                AvanzamentoSincrono.Update();
            }

            RequestUpdate(this);
            
        }

        #region PropertyName

        private const string PropNumeroGiri = "NumeroGiri";
        private const string PropVelocitaTaglio = "VelocitaTaglio";
        private const string PropAvanzamentoSincrono = "AvanzamentoSincrono";
        private const string PropAvanzamentoAsincrono = "AvanzamentoAsincrono";

        #endregion

        #region Property

        public double Step
        {
            get { return _parametroPunta.Step; }
            set
            {
                _parametroPunta.Step = value;

                UserInput_SourceUpdated(null, EventArgs.Empty);

                OnPropertyChanged("Step");
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

        #endregion


        #region IDataErrorInfo Members

        protected override string[] ValidatedProperties
        {
            get
            {
                return new[]
                {                  
                PropNumeroGiri,
                PropVelocitaTaglio,
                PropAvanzamentoSincrono,
                PropAvanzamentoAsincrono
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

                case PropAvanzamentoSincrono:
                    {
                        error = InputCheck.MaggioreDiZero(AvanzamentoSincrono.Value);
                    } break;

                case PropAvanzamentoAsincrono:
                    {
                        error = InputCheck.MaggioreDiZero(AvanzamentoAsincrono.Value);
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