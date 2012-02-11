using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.ParameterScreen
{
    public class LamaturaParametriViewModel : CommonDrillParametriViewModel
    {
        private readonly Lamatura _lamatura;

        public LamaturaParametriViewModel(EditWorkViewModel parent, Lamatura lamatura)
            : base(parent, lamatura, "Counterbore")
        {
            _lamatura = lamatura;

            EditWorkParent = parent;
        }

        public double ProfonditaLamatura
        {
            get
            {
                return _lamatura.ProfonditaLamatura;
            }
            set
            {
                _lamatura.ProfonditaLamatura = value;
                OnPropertyChanged("ProfonditaLamatura");
            }
        }

        public double DiametroLamatura
        {
            get
            {
                return _lamatura.DiametroLamatura;
            }
            set
            {
                _lamatura.DiametroLamatura = value;
                OnPropertyChanged("DiametroLamatura");
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
