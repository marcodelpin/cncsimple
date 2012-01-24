using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.TextEngraving
{
    public class TextEngravingParametriViewModel : EditStageTreeViewItem, IDataErrorInfo, IValid
    {
        private readonly TextEngravingModel _textEngravingModel;

        public TextEngravingParametriViewModel(TextEngravingModel textEngravingModel, EditWorkViewModel treeItemParent)
            : base("Parameter", treeItemParent)
        {
            _textEngravingModel = textEngravingModel;

        }

        public double CenterX
        {
            get
            {
                return _textEngravingModel.CenterX;
            }
            set
            {
                _textEngravingModel.CenterX = value;
                OnPropertyChanged("CenterX");
            }
        }
        public double CenterY
        {
            get
            {
                return _textEngravingModel.CenterY;
            }
            set
            {
                _textEngravingModel.CenterY = value;
                OnPropertyChanged("CenterY");
            }
        }
        public double RadiusCircle
        {
            get
            {
                return _textEngravingModel.RadiusCircle;
            }
            set
            {
                _textEngravingModel.RadiusCircle = value;
                OnPropertyChanged("RadiusCircle");
            }
        }

        public double AngleStart
        {
            get
            {
                return _textEngravingModel.AngleStart;
            }
            set
            {
                _textEngravingModel.AngleStart = value;
                OnPropertyChanged("AngleStart");
            }
        }

        public double AngleWidth
        {
            get
            {
                return _textEngravingModel.AngleWidth;
            }
            set
            {
                _textEngravingModel.AngleWidth = value;
                OnPropertyChanged("AngleWidth");
            }
        }

        public double FontHeight
        {
            get
            {
                return _textEngravingModel.FontHeight;
            }
            set
            {
                _textEngravingModel.FontHeight = value;
                OnPropertyChanged("FontHeight");
            }
        }

        public bool WriteInCircle
        {
            get
            {
                return _textEngravingModel.WriteInCircle;
            }
            set
            {
                _textEngravingModel.WriteInCircle = value;
                OnPropertyChanged("WriteInCircle");
            }
        }

        public string TextToEngrave
        {
            get
            {
                return _textEngravingModel.TextToEngrave;
            }
            set
            {
                _textEngravingModel.TextToEngrave = value;
                OnPropertyChanged("TextToEngrave");
            }
        }

        public double Profondita
        {
            get
            {
                return _textEngravingModel.ProfonditaLavorazione;
            }
            set
            {
                _textEngravingModel.ProfonditaLavorazione = value;
                OnPropertyChanged("Profondita");
            }
        }

        public double SicurezzaZ
        {
            get { return _textEngravingModel.SicurezzaZ; }
            set
            {
                _textEngravingModel.SicurezzaZ = value;
                OnPropertyChanged("SicurezzaZ");
            }
        }

        public double InizioZ
        {
            get { return _textEngravingModel.InizioLavorazioneZ; }
            set
            {
                _textEngravingModel.InizioLavorazioneZ = value;
                OnPropertyChanged("InizioZ");
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
                                                     "Profondita",
                                                     "SicurezzaZ"
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Profondita":
                    {
                        error = InputCheck.MaggioreDiZero(Profondita.ToString());
                    }
                    break;

                case "SicurezzaZ":
                    {
                        if (SicurezzaZ <= InizioZ)
                            error = "Must be higher than StartZ";
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
