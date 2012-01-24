using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.ParameterScreen
{
    public class ForaturaSempliceParametriViewModel : CommonDrillParametriViewModel
    {
        private readonly ForaturaSemplice _foraturaSemplice;

        public ForaturaSempliceParametriViewModel(EditWorkViewModel parent, ForaturaSemplice foraturaSemplice)
            : base(parent, foraturaSemplice, "Drilling Parameter")
        {
            _foraturaSemplice = foraturaSemplice;

            EditWorkParent = parent;
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
