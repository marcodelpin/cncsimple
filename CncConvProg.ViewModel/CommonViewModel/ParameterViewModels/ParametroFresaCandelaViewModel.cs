using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.CommonViewModel.ParameterViewModels
{
    public class ParametroFresaCandelaViewModel : ParametroFresaBaseViewModel
    {
        private readonly ParametroFresaCandela _parametroFresaCandela;

        public ParametroFresaCandelaViewModel(ParametroFresaCandela parametroFresaCandela)
            : base(parametroFresaCandela)
        {
            _parametroFresaCandela = parametroFresaCandela;

            //AvanzamentoSincronoPiantata = new UserInputViewModel(_parametroFresaCandela.AvanzamentoSincronoPiantata, GetValidationError, PropAvanzamentoSincronoPiantata);
            //AvanzamentoAsincronoPiantata = new UserInputViewModel(_parametroFresaCandela.AvanzamentoAsincronoPiantata, GetValidationError, PropAvanzamentoAsincronoPiantata);

            //AvanzamentoSincronoPiantata.OnSourceUpdated += UserInput_SourceUpdated;
            //AvanzamentoAsincronoPiantata.OnSourceUpdated += UserInput_SourceUpdated;

        }

        // Guarda fresa spianare
        //void UserInput_SourceUpdated(object sender, EventArgs e)
        //{
        //    if (sender == AvanzamentoSincronoPiantata)
        //    {
        //        if (AvanzamentoSincronoPiantata.Value.HasValue)
        //            _parametroFresaCandela.SetPlungeFeedSync(AvanzamentoSincronoPiantata.Value.Value);

        //        AvanzamentoAsincronoPiantata.Update();
        //    }
        //    else if (sender == AvanzamentoAsincronoPiantata)
        //    {
        //        if (AvanzamentoAsincronoPiantata.Value.HasValue)
        //            _parametroFresaCandela.SetPlungeFeedAsync(AvanzamentoAsincronoPiantata.Value.Value);

        //        AvanzamentoSincronoPiantata.Update();
        //    }


        //    RequestUpdate(this);

        //    //  OnPropertyChanged("IsValid");

        //}

        #region PropertyName

        //private const string PropAvanzamentoSincronoPiantata = "AvanzamentoSincronoPiantata";
        //private const string PropAvanzamentoAsincronoPiantata = "AvanzamentoAsincronoPiantata";

        #endregion

        #region Property

        //private UserInputViewModel _avanzamentoSincronoPiantata;
        //public UserInputViewModel AvanzamentoSincronoPiantata
        //{
        //    get { return _avanzamentoSincronoPiantata; }
        //    set
        //    {
        //        _avanzamentoSincronoPiantata = value;
        //        OnPropertyChanged("AvanzamentoSincronoPiantata");
        //    }
        //}
        //private UserInputViewModel _avanzamentoAsincronoPiantata;
        //public UserInputViewModel AvanzamentoAsincronoPiantata
        //{
        //    get { return _avanzamentoAsincronoPiantata; }
        //    set
        //    {
        //        _avanzamentoAsincronoPiantata = value;
        //        OnPropertyChanged("AvanzamentoAsincronoPiantata");
        //    }
        //}

        #endregion


        #region IDataErrorInfo Members


        //protected override string[] ValidatedProperties
        //{
        //    get
        //    {
        //        var p = new List<string>
        //                   {

        //                                             PropAvanzamentoSincronoPiantata,
        //                                             PropAvanzamentoAsincronoPiantata


        //    };

        //        p.AddRange(base.ValidatedProperties);

        //        return p.ToArray();
        //    }

        //}

        //protected override string GetValidationError(string propertyName)
        //{
        //    if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
        //        return null;

        //    string error = null;

        //    switch (propertyName)
        //    {
        //        case PropAvanzamentoSincronoPiantata:
        //            {
        //                error = InputCheck.MaggioreDiZero(AvanzamentoSincronoPiantata.Value);

        //            } break;

        //        case PropAvanzamentoAsincronoPiantata:
        //            {
        //                error = InputCheck.MaggioreDiZero(AvanzamentoAsincronoPiantata.Value);
        //            } break;

        //        default:
        //            // Richiamo controllo della classe base..
        //            error = base.GetValidationError(propertyName);
        //            break;
        //    }


        //    return error;
        //}

        protected override string ErrorAvanzamentoAsincronoPiantata()
        {
            return InputCheck.MaggioreDiZero(AvanzamentoAsincronoPiantata.Value);
        }

        protected override string ErrorAvanzamentoSincronoPiantata()
        {
            return InputCheck.MaggioreDiZero(AvanzamentoSincronoPiantata.Value);
        }
        #endregion

    }


}

