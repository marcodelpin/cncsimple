using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Pattern
{
    public class LinePatternViewModel : EditStageTreeViewItem, IDataErrorInfo, IValid
    {
        private readonly PatternDrillingLine _patternCerchio;

        public LinePatternViewModel(PatternDrillingLine patternCerchio, EditStageTreeViewItem parent)
            : base("Line", parent)
        {
            _patternCerchio = patternCerchio;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName != "IsValid")
            {
                OnPropertyChanged("IsValid");
                //SourceUpdated(); // messo in classe base
            }

            base.OnPropertyChanged(propertyName);
        }
        public double CenterX
        {
            get
            {
                return _patternCerchio.CenterX;
            }

            set
            {
                _patternCerchio.CenterX = value;
                OnPropertyChanged("CenterX");
            }
        }
        public double CenterY
        {
            get
            {
                return _patternCerchio.CenterY;
            }

            set
            {
                _patternCerchio.CenterY = value;
                OnPropertyChanged("CenterY");
            }
        }
        //public double FirstLenght
        //{
        //    get
        //    {
        //        return _patternCerchio.FirstLenght;
        //    }

        //    set
        //    {
        //        _patternCerchio.FirstLenght = value;
        //        OnPropertyChanged("FirstLenght");

        //    }
        //}
        public int DrillCount
        {
            get
            {
                return _patternCerchio.NumeroFori;
            }

            set
            {
                _patternCerchio.NumeroFori = value;
                OnPropertyChanged("DrillCount");
            }
        }
        public double Angle
        {
            get
            {
                return _patternCerchio.Angle;
            }

            set
            {
                _patternCerchio.Angle = value;
                OnPropertyChanged("Angle");
            }
        }

        public double Passo
        {
            get
            {
                return _patternCerchio.Passo;
            }

            set
            {
                _patternCerchio.Passo = value;
                OnPropertyChanged("Passo");
            }
        }
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
                                                    "Passo"
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Passo":
                    {
                        error = InputCheck.MaggioreDiZero(Passo.ToString());
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

