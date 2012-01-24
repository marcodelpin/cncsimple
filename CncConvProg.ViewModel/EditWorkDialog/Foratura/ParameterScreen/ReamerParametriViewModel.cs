using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.ParameterScreen
{
    public class ReamerParametriViewModel : CommonDrillParametriViewModel
    {
        private readonly Alesatura _alesatura;

        public ReamerParametriViewModel(EditWorkViewModel parent, Alesatura alesatura)
            : base(parent, alesatura, "Reamer")
        {
            _alesatura = alesatura;

            EditWorkParent = parent;

            
        }

        public double ProfonditaAlesatura
        {

            get { return _alesatura.ProfonditaAlesatore; }
            set
            {
                _alesatura.ProfonditaAlesatore = value;
                OnPropertyChanged("ProfonditaAlesatura");
            }
        }

        protected override string[] ValidatedProperties
        {
            get
            {
                var o = new List<string>
                {
                    //"SicurezzaZ",
                    //"InizioZ"
                };

                foreach (var validatedProperty in base.ValidatedProperties)
                {
                    o.Add(validatedProperty);
                }

                return o.ToArray();
            }
        }



        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                // other properties..
                default:
                    error = base.GetValidationError(propertyName);
                    break;
            }

            return error;
        }
    }
}
