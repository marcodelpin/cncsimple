using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.ParameterScreen
{
    public class BarenaturaParametriViewModel : CommonDrillParametriViewModel
    {
        private readonly Barenatura _barenatura;

        public BarenaturaParametriViewModel(EditWorkViewModel parent, Barenatura barenatura)
            : base(parent, barenatura, "Boring")
        {
            _barenatura = barenatura;

            EditWorkParent = parent;
        }

        public double DiametroBareno
        {
            get
            {
                return _barenatura.DiametroBarenatura;
            }

            set
            {
                _barenatura.DiametroBarenatura = value;
                OnPropertyChanged("DiametroBareno");
            }
        }

        public double ProfonditaBareno
        {
            get
            {
                return _barenatura.ProfonditaBareno;
            }

            set
            {
                _barenatura.ProfonditaBareno = value;
                OnPropertyChanged("ProfonditaBareno");
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

        public Dictionary<byte, string> MillingStrategy
        {
            get
            {
                var lookup = new Dictionary<byte, string>
                                 {
                                     {0, "Traditional"},
                                     {1, "Spiral"},

                                 };

                return lookup;

            }
        }

        public double MaterialePerFinitura
        {
            get { return _barenatura.MaterialePerFinitura; }
            set
            {
                _barenatura.MaterialePerFinitura = value;
                OnPropertyChanged("MaterialePerFinitura");
            }
        }

        public bool AllargaturaConFresa
        {
            get { return _barenatura.AllargaturaAbilitata; }
            set
            {
                _barenatura.AllargaturaAbilitata = value;
                OnPropertyChanged("AllargaturaConFresa");
            }
        }

        public byte MillingSelectedStrategy
        {
            get
            {
                return _barenatura.ModalitaAllargatura;
            }
            set
            {
                _barenatura.ModalitaAllargatura = value;
                OnPropertyChanged("MillingSelectedStrategy");
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
