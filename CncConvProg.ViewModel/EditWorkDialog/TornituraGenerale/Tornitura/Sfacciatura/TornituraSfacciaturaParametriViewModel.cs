using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Tornitura.Sfacciatura
{
    public class SfacciaturaParametriViewModel : EditStageTreeViewItem
    {
        private readonly TornituraSfacciatura _sfacciatura;

        public SfacciaturaParametriViewModel(TornituraSfacciatura sfacciatura, EditWorkViewModel treeItemParent)
            : base("Parameter", treeItemParent)
        {
            _sfacciatura = sfacciatura;
        }

        public double DiametroMax
        {
            get
            {
                return _sfacciatura.DiametroMax;
            }
            set
            {
                _sfacciatura.DiametroMax = value;
                OnPropertyChanged("DiametroMax");
            }
        }

        public double DiametroMin
        {
            get
            {
                return _sfacciatura.DiametroMin;
            }
            set
            {
                _sfacciatura.DiametroMin = value;
                OnPropertyChanged("DiametroMin");
            }
        }


        public double Sovrametallo
        {
            get { return _sfacciatura.Sovrametallo; }
            set
            {
                _sfacciatura.Sovrametallo = value;
                OnPropertyChanged("Sovrametallo");
            }
        }

        public double InizioZ
        {
            get { return _sfacciatura.InizioZ; }
            set
            {
                _sfacciatura.InizioZ = value;
                OnPropertyChanged("InizioZ");
            }
        }

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid
        {
            get { return ValidatedProperties.All(property => GetValidationError(property) == null); }
        }

        public override bool? ValidateStage()
        {
            return null;
        }


        protected string[] ValidatedProperties = {
                                                     "Sovrametallo",
                                                     "Profondita",
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Diametro":
                    {
                    }
                    break;

                case "Sovrametallo":
                    {
                    }
                    break;

                case "Profondita":
                    {
                    }
                    break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

    }
}
