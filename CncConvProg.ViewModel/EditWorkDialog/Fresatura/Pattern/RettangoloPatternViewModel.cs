using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern
{
    public class RettangoloPatternViewModel : ViewModelValidable, IDataErrorInfo
    {
        private readonly RettangoloPattern _rectanglePattern;

        public RettangoloPatternViewModel(RettangoloPattern patternCerchio)
        {
            _rectanglePattern = patternCerchio;
        }

        public Dictionary<byte, string> StartPointLookup
        {
            get
            {
                var lookup = new Dictionary<byte, string>
                                 {
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.UpLeft, "1"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.UpRight, "2"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.DownLeft, "3"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.DownRight, "4"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.Center, "5"},

                                 };

                return lookup;

            }
        }

        public byte SelectedStartPoint
        {
            get
            {
                return (byte)_rectanglePattern.StartPoint;
            }
            set
            {
                _rectanglePattern.StartPoint = (SquareShapeHelper.SquareShapeStartPoint)value;
                OnPropertyChanged("SelectedStartPoint");
            }
        }

        public double Altezza
        {
            get { return _rectanglePattern.Altezza; }
            set
            {
                if (_rectanglePattern.Altezza == value) return;

                _rectanglePattern.Altezza = value;
                OnPropertyChanged("Altezza");
            }
        }

        public double Larghezza
        {
            get { return _rectanglePattern.Larghezza; }
            set
            {
                if (_rectanglePattern.Larghezza == value) return;

                _rectanglePattern.Larghezza = value;
                OnPropertyChanged("Larghezza");
            }
        }

        public double OffsetCentroY
        {
            get { return _rectanglePattern.PuntoStartY; }
            set
            {
                if (_rectanglePattern.PuntoStartY == value) return;

                _rectanglePattern.PuntoStartY = value;
                OnPropertyChanged("OffsetCentroY");
            }
        }

        public double OffsetCentroX
        {
            get { return _rectanglePattern.PuntoStartX; }
            set
            {
                if (_rectanglePattern.PuntoStartX == value) return;

                _rectanglePattern.PuntoStartX = value;
                OnPropertyChanged("OffsetCentroX");
            }
        }

        public double ChamferValue
        {
            get { return _rectanglePattern.Chamfer; }
            set
            {
                _rectanglePattern.Chamfer = value;
                OnPropertyChanged("ChamferValue");
            }
        }

        public bool ChamferAbilited
        {
            get { return _rectanglePattern.ChamferAbilited; }
            set
            {
                _rectanglePattern.ChamferAbilited = value;
                OnPropertyChanged("ChamferAbilited");
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
        public override bool? ValidateStage()
        {
            return ValidatedProperties.All(property => GetValidationError(property) == null);
        }


        protected string[] ValidatedProperties = {
                                                    "Raggio",
                                                    "LunghezzaCentroLato",
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                //    case "LunghezzaCentroLato":
                //        {
                //            error = InputCheck.MaggioreDiZero(LunghezzaCentroLato.ToString());
                //        }
                //        break;

                //    case "Raggio":
                //        {
                //            error = InputCheck.MaggioreDiZero(Raggio.ToString());
                //        }
                //        break;


                //default:
                //    Debug.Fail("Unexpected property : " + propertyName);
                //    break;
            }

            return error;
        }

        #endregion


    }


}

