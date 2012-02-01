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
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Pattern
{
    public class CirclePatternViewModel : ViewModelValidable, IDataErrorInfo
    {
        private readonly PatternDrillingCircle _patternCerchio;

        public CirclePatternViewModel(PatternDrillingCircle patternCerchio)
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
        public double CentroX
        {
            get
            {
                return _patternCerchio.CircleCenterX;
            }

            set
            {
                _patternCerchio.CircleCenterX = value;
                OnPropertyChanged("CentroX");
            }
        }
        public double CentroY
        {
            get
            {
                return _patternCerchio.CircleCenterY;
            }

            set
            {
                _patternCerchio.CircleCenterY = value;
                OnPropertyChanged("CentroY");
            }
        }
        public double Radius
        {
            get
            {
                return _patternCerchio.Radius;
            }

            set
            {
                _patternCerchio.Radius = value;
                OnPropertyChanged("Radius");

            }
        }
        public double FirstAngle
        {
            get
            {
                return _patternCerchio.FirstAngle;
            }

            set
            {
                _patternCerchio.FirstAngle = value;
                OnPropertyChanged("FirstAngle");
            }
        }
        public int DrillCount
        {
            get
            {
                return _patternCerchio.DrillCount;
            }

            set
            {
                _patternCerchio.DrillCount = value;
                OnPropertyChanged("DrillCount");
            }
        }
        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

      public override bool? ValidateStage()
        {
           return ValidatedProperties.All(property => GetValidationError(property) == null); }
        

        protected string[] ValidatedProperties = {
                                                    "Radius"
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Radius":
                    {
                        error = InputCheck.MaggioreDiZero(Radius.ToString());
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

