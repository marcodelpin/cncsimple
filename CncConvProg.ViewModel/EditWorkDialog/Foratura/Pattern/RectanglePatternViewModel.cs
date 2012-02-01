using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Pattern
{
    public class RectanglePatternViewModel : ViewModelValidable, IDataErrorInfo
    {
        private readonly PatternDrillingRectangle _patternCerchio;

        public RectanglePatternViewModel(PatternDrillingRectangle patternCerchio)
        {
            _patternCerchio = patternCerchio;
        }

        public Dictionary<byte, string> StartPointLookup
        {
            get
            {
                var lookup = new Dictionary<byte, string>
                                 {
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.UpLeft, "Up Left - 1"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.UpRight, "Up Right - 2"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.DownLeft, "Down Left - 3"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.DownRight, "Down Right - 4"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.Center, "Center - 5"},

                                 };

                return lookup;

            }
        }

        public byte SelectedStartPoint
        {
            get
            {
                return (byte)_patternCerchio.StartPoint;
            }
            set
            {
                _patternCerchio.StartPoint = (SquareShapeHelper.SquareShapeStartPoint)value;
                OnPropertyChanged("SelectedStartPoint");
            }
        }

        public double RefPointX
        {
            get
            {
                return _patternCerchio.RefPointX;
            }

            set
            {
                _patternCerchio.RefPointX = value;
                OnPropertyChanged("RefPointX");
            }
        }
        public double RefPointY
        {
            get
            {
                return _patternCerchio.RefPointY;
            }

            set
            {
                _patternCerchio.RefPointY = value;
                OnPropertyChanged("RefPointY");
            }
        }
        public double Height
        {
            get
            {
                return _patternCerchio.Height;
            }

            set
            {
                _patternCerchio.Height = value;
                OnPropertyChanged("Height");
            }
        }
        public double Width
        {
            get
            {
                return _patternCerchio.Width;
            }

            set
            {
                _patternCerchio.Width = value;
                OnPropertyChanged("Width");
            }
        }
        public int DrillCountY
        {
            get
            {
                return _patternCerchio.DrillCountY;
            }

            set
            {
                _patternCerchio.DrillCountY = value;
                OnPropertyChanged("DrillCountY");
            }
        }
        public int DrillCountX
        {
            get
            {
                return _patternCerchio.DrillCountX;
            }

            set
            {
                _patternCerchio.DrillCountX = value;
                OnPropertyChanged("DrillCountX");
            }
        }
        //public int DrillCountX
        //{
        //    get
        //    {
        //        return _patternCerchio.;
        //    }

        //    set
        //    {
        //        _patternCerchio.DrillCountX = value;
        //        OnPropertyChanged("DrillCountX");
        //    }
        //}
        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        public override bool? ValidateStage()
        {
            return ValidatedProperties.All(property => GetValidationError(property) == null);
        }


        protected string[] ValidatedProperties = {
                                                   // "Radius"
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                //case "Radius":
                //    {
                //        error = InputCheck.MaggioreDiZero(Radius.ToString());
                //    }
                //    break;


                //default:
                //    Debug.Fail("Unexpected property : " + propertyName);
                //    break;
            }

            return error;
        }

        #endregion

    }


}

