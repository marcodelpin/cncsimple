using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern
{
    public class CavaPatternViewModel : EditStageTreeViewItem, IDataErrorInfo, IValid
    {
        private readonly CavaDrittaPattern _patternCerchio;

        public CavaPatternViewModel(CavaDrittaPattern patternCerchio, EditStageTreeViewItem parent)
            : base("Slot", parent)
        {
            _patternCerchio = patternCerchio;
        }

        public double CentroX
        {
            get
            {
                return _patternCerchio.CentroX;
            }

            set
            {
                if (_patternCerchio.CentroX == value) return;

                _patternCerchio.CentroX = value;

                OnPropertyChanged("CentroX");
            }
        }
        public double CentroY
        {
            get
            {
                return _patternCerchio.CentroY;
            }

            set
            {
                _patternCerchio.CentroY = value;
                OnPropertyChanged("CentroY");
            }
        }
        public double DistanzaCentri
        {
            get
            {
                return _patternCerchio.LunghezzaCentri;
            }

            set
            {
                _patternCerchio.LunghezzaCentri = value;
                OnPropertyChanged("DistanzaCentri");

            }
        }
        public double Raggio
        {
            get
            {
                return _patternCerchio.Radius;
            }

            set
            {
                _patternCerchio.Radius = value;
                OnPropertyChanged("Raggio");
            }
        }
        //public double AngoloAmpiezza
        //{
        //    get
        //    {
        //        return _patternCerchio.AngoloAmpiezza;
        //    }

        //    set
        //    {
        //        _patternCerchio.AngoloAmpiezza = value;
        //        OnPropertyChanged("AngoloAmpiezza");
        //    }
        //}

        //public double RaggioInterasse
        //{
        //    get
        //    {
        //        return _patternCerchio.RaggioInterasse;
        //    }

        //    set
        //    {
        //        _patternCerchio.RaggioInterasse = value;
        //        OnPropertyChanged("RaggioInterasse");
        //    }
        //}

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid
        {
            get { return ValidatedProperties.All(property => GetValidationError(property) == null); }
        }

        protected string[] ValidatedProperties = {
                                                    "Raggio",
                                                    "DistanzaCentri",
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "DistanzaCentri":
                    {
                        error = InputCheck.MaggioreDiZero(DistanzaCentri.ToString());
                    }
                    break;

                case "Raggio":
                    {
                        error = InputCheck.MaggioreDiZero(Raggio.ToString());
                    }
                    break;
              

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion


    }


}

