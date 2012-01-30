using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.ViewModel.AuxViewModel;

namespace CncConvProg.ViewModel.CommonViewModel.ParameterViewModels
{
    public class ParametroUtensileTornituraViewModel : ToolParameterViewModel
    {

        private readonly ParametroUtensileTornitura _parametroUtensileTornitura;

        public ParametroUtensileTornituraViewModel(Utensile parametroUtensileTornitura )
            : base(parametroUtensileTornitura)
        {

            //Velocita = new UserInputViewModel(_parametroUtensileTornitura.Velocita, GetValidationError, PropNumeroGiri);
            //VelocitaTaglio = new UserInputViewModel(_parametroUtensileTornitura.VelocitaTaglio, GetValidationError, PropVelocitaTaglio);

            //AvanzamentoSincrono = new UserInputViewModel(_parametroUtensileTornitura.AvanzamentoSincrono, GetValidationError, PropAvanzamentoSincrono);
            //AvanzamentoAsincrono = new UserInputViewModel(_parametroUtensileTornitura.AvanzamentoAsincrono, GetValidationError, PropAvanzamentoAsincrono);

            //Velocita.OnSourceUpdated += UserInput_SourceUpdated;
            //VelocitaTaglio.OnSourceUpdated += UserInput_SourceUpdated;

            //AvanzamentoSincrono.OnSourceUpdated += UserInput_SourceUpdated;
            //AvanzamentoAsincrono.OnSourceUpdated += UserInput_SourceUpdated;
        }

        //void UserInput_SourceUpdated(object sender, EventArgs e)
        //{
        //    // ESSENDO BINDATO CON DOUBLE .  QUANDO NON IMMETTO NIENTE VIENE SEGNALATO ERRORE.
        //    if (sender == Velocita)
        //    {
        //        var numeroGiri = Velocita.Value;
        //        var velocitaTaglio = (numeroGiri * Diametro * Math.PI) / 1000;

        //        VelocitaTaglio.SetValue(false, velocitaTaglio);
        //    }

        //    else if (sender == VelocitaTaglio)
        //    {
        //        var vt = VelocitaTaglio.Value;
        //        var numeroGiri = (vt * 1000) / (Math.PI * Diametro);

        //        Velocita.SetValue(false, numeroGiri);
        //    }

        //    else if (sender == AvanzamentoSincrono)
        //    {
        //        AvanzamentoAsincrono.SetValue(false, AvanzamentoSincrono.Value * Velocita.Value);
        //    }
        //    else if (sender == AvanzamentoAsincrono)
        //    {
        //        AvanzamentoSincrono.SetValue(false, AvanzamentoAsincrono.Value / Velocita.Value);
        //    }

        //    OnPropertyChanged("IsValid");
        //}

        #region PropertyName

        private const string PropNumeroGiri = "NumeroGiri";
        private const string PropVelocitaTaglio = "VelocitaTaglio";
        private const string PropAvanzamentoSincrono = "AvanzamentoSincrono";
        private const string PropAvanzamentoAsincrono = "AvanzamentoAsincrono";

        #endregion

        #region Property

        public double Velocita
        {
            get { return _parametroUtensileTornitura.Velocita; }
            set
            {
                _parametroUtensileTornitura.Velocita = value;

                OnPropertyChanged("Velocita");
            }
        }

        public double ProfPassata
        {
            get { return _parametroUtensileTornitura.ProfonditaPassata; }
            set
            {
                _parametroUtensileTornitura.ProfonditaPassata = value;

                OnPropertyChanged("ProfPassata");
            }
        }


        public double Avanzamento
        {
            get { return _parametroUtensileTornitura.AvanzamentoSincrono; }
            set
            {
                _parametroUtensileTornitura.AvanzamentoSincrono = value;

                OnPropertyChanged("Avanzamento");
            }
        }



        #endregion


        #region IDataErrorInfo Members


        protected override string[] ValidatedProperties
        {
            get
            {
                return new string[]
                           {
                               PropNumeroGiri,
                                                     PropVelocitaTaglio,
                                                     PropAvanzamentoSincrono,
                                                     PropAvanzamentoAsincrono,
                               
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

                //case PropNumeroGiri:
                //    {
                //        error = InputCheck.MaggioreDiZero(Velocita);
                //    }
                //    break;

                //case PropAvanzamentoSincrono:
                //    {
                //        error = InputCheck.MaggioreDiZero(Avanzamento);
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