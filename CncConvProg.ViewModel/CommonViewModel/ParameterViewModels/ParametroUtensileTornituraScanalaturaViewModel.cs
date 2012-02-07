using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.ViewModel.AuxViewModel;

namespace CncConvProg.ViewModel.CommonViewModel.ParameterViewModels
{
    public class ParametroUtensileTornituraScanalaturaViewModel : ParametroUtensileTornituraViewModel
    {

        private ParametroUtensileTornituraScanalatura _parametroUtensileTornitura
        {
            get { return Parametro as ParametroUtensileTornituraScanalatura; }
        }


        public ParametroUtensileTornituraScanalaturaViewModel(Utensile parametroUtensileTornitura)
            : base(parametroUtensileTornitura)
        {
        }

        #region Property

        public double LarghezzaPassata
        {
            get { return _parametroUtensileTornitura.LarghezzaPassata; }
            set
            {
                _parametroUtensileTornitura.LarghezzaPassata = value;

                OnPropertyChanged("LarghezzaPassata");
            }
        }

        public double Step
        {
            get { return _parametroUtensileTornitura.Step; }
            set
            {
                _parametroUtensileTornitura.Step = value;

                OnPropertyChanged("Step");
            }
        }

        #endregion

        #region IDataErrorInfo Members


        protected override string[] ValidatedProperties
        {
            get
            {
                var baseProperties = base.ValidatedProperties;

                var r = new List<string>();
                r.AddRange(baseProperties);

                r.Add("Step");
                r.Add("LarghezzaPassata");

                return r.ToArray();
            }

        }

        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Step":
                    {

                    }
                    break;

                case "LarghezzaPassata":
                    {
                        error = InputCheck.MaggioreDiZero(LarghezzaPassata);
                    } break;

                default:
                    error = base.GetValidationError(propertyName);
                    break;
            }

            return error;
        }


        #endregion
    }
}