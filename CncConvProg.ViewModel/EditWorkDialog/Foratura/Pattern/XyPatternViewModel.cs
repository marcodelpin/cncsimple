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
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Pattern
{
    public class XyPatternViewModel : ViewModelValidable, IDataErrorInfo
    {
        private readonly PatternDrillingXy _patternCerchio;

        public XyPatternViewModel(PatternDrillingXy patternCerchio)
        {
            _patternCerchio = patternCerchio;
        }

        public double CenterX
        {
            get
            {
                return _patternCerchio.ReferencePntX;
            }

            set
            {
                _patternCerchio.ReferencePntX = value;
                OnPropertyChanged("CenterX");
            }
        }
        public double CenterY
        {
            get
            {
                return _patternCerchio.ReferencePntY;
            }

            set
            {
                _patternCerchio.ReferencePntY = value;
                OnPropertyChanged("CenterY");
            }
        }

        public ObservableCollection<PointViewModel> PointList
        {
            get
            {
                var p = new ObservableCollection<PointViewModel>();

                foreach (var pointRc in _patternCerchio.PointList)
                {
                    p.Add(new PointViewModel(pointRc));
                }

                return p;
            }
        }

        protected void AddPnt()
        {
            var pnt = new Point2D();
            _patternCerchio.AddPnt(pnt);

            OnPropertyChanged("PointList");

        }

        RelayCommand _addPnt;

        public ICommand AddPntCmd
        {
            get
            {
                return _addPnt ?? (_addPnt = new RelayCommand(param => AddPnt(),
                                                                param => true));
            }
        }


        protected void RemovePnt(PointViewModel pointToRemove)
        {
            var pn = pointToRemove.PntRc;

            if (_patternCerchio.PointList.Contains(pn))
                _patternCerchio.PointList.Remove(pn);

            OnPropertyChanged("PointList");
        }

        RelayCommand _delPnt;

        public ICommand DelPntCmd
        {
            get
            {
                return _delPnt ?? (_delPnt = new RelayCommand(param => RemovePnt(param as PointViewModel),
                                                                                param => true));
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

        public class PointViewModel : ViewModelBase
        {
            public readonly Point2D PntRc;

            public PointViewModel(Point2D pnt)
            {
                PntRc = pnt;
            }

            public double CooX
            {
                get { return PntRc.X; }
                set
                {
                    PntRc.X = value;
                    OnPropertyChanged("CooX");
                }
            }

            public double CooY
            {
                get { return PntRc.Y; }
                set
                {
                    PntRc.Y = value;
                    OnPropertyChanged("CooY");
                }
            }
        }

    }


}

