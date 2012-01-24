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
    public class ArcPatternViewModel : EditStageTreeViewItem, IDataErrorInfo, IValid
    {
        private readonly PatternDrillingArc _patternCerchio;

        public ArcPatternViewModel(PatternDrillingArc patternCerchio, EditStageTreeViewItem parent)
            : base("Arc", parent)
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
        public double EndAngle
        {
            get
            {
                return _patternCerchio.EndAngle;
            }

            set
            {
                _patternCerchio.EndAngle = value;
                OnPropertyChanged("EndAngle");
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

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid
        {
            get { return ValidatedProperties.All(property => GetValidationError(property) == null); }
        }

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

