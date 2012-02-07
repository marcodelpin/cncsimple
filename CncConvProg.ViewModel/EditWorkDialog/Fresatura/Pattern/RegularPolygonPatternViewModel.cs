using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern
{
    public class RegularPolygonPatternViewModel : ViewModelValidable, IDataErrorInfo
    {
        private readonly RegularPolygonPattern _patternCerchio;

        public RegularPolygonPatternViewModel(RegularPolygonPattern patternCerchio)
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
        public bool Circoscritto
        {
            get
            {
                return _patternCerchio.Circoscritto;
            }

            set
            {
                _patternCerchio.Circoscritto = value;
                OnPropertyChanged("Circoscritto");
            }
        }
        public int SideCount
        {
            get
            {
                return _patternCerchio.SideNumber;
            }

            set
            {
                _patternCerchio.SideNumber = value;
                OnPropertyChanged("SideCount");
            }
        }

        public double ChamferValue
        {
            get { return _patternCerchio.Chamfer; }
            set
            {
                _patternCerchio.Chamfer = value;
                OnPropertyChanged("ChamferValue");
            }
        }

        public bool ChamferAbilited
        {
            get { return _patternCerchio.ChamferAbilited; }
            set
            {
                _patternCerchio.ChamferAbilited = value;
                OnPropertyChanged("ChamferAbilited");
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
            return ValidatedProperties.All(property => GetValidationError(property) == null);
        }

        protected string[] ValidatedProperties = {
                                                    "Radius",
                                                    "SideCount",
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

                case "SideCount":
                    {
                        if (SideCount < 3)
                            error = "Minimum 3 side";
                    }
                    break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion


        /////
        ///// Questo metodo è uguale in tutti il viewModel per i dati di taglio,
        ///// se si fare classe base implementarlo li..
        ///// <returns></returns>
        //public IEnumerable<IEntity2D> GetPreview()
        //{
        //    // todo_ se non è valida cercare di restiruire null.

        //    if (!IsValid) return new List<IEntity2D>();

        //    //return _patternCerchio.GetPreview();
        //    // todo : in questo punto non è ancora noto diametro
        //    // fare che ritorna croci o qualcosa ..
        //    return new List<IEntity2D>();
        //}
    }


}

