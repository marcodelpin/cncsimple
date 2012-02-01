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
    public class RcPatternViewModel : ViewModelValidable, IDataErrorInfo
    {
        private readonly PatternDrillingRc _patternCerchio;

        public RcPatternViewModel(PatternDrillingRc patternCerchio)
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

        public ObservableCollection<PointRcViewModel> PointList
        {
            get
            {
                var p = new ObservableCollection<PointRcViewModel>();

                foreach (var pointRc in _patternCerchio.PointRcList)
                {
                    p.Add(new PointRcViewModel(pointRc));
                }

                return p;
            }
        }

        protected void AddPnt()
        {
            var pnt = new PointRc();
            _patternCerchio.PointRcList.Add(pnt);

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


        protected void RemovePnt(PointRcViewModel pointToRemove)
        {
            var pn = pointToRemove.PntRc;

            if (_patternCerchio.PointRcList.Contains(pn))
                _patternCerchio.PointRcList.Remove(pn);

            OnPropertyChanged("PointList");
        }

        RelayCommand _delPnt;

        public ICommand DelPntCmd
        {
            get
            {
                return _delPnt ?? (_delPnt = new RelayCommand(param => RemovePnt(param as PointRcViewModel),
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
                //case "Radius":
                //    {
                //     //   error = InputCheck.MaggioreDiZero(Radius.ToString());
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

    public class PointRcViewModel : ViewModelBase
    {
        public readonly PointRc PntRc;

        public PointRcViewModel(PointRc pnt)
        {
            PntRc = pnt;
        }

        public double Radius
        {
            get { return PntRc.Radius; }
            set
            {
                PntRc.Radius = value;
                OnPropertyChanged("Radius");
            }
        }

        public double Angle
        {
            get { return PntRc.Angle; }
            set
            {
                PntRc.Angle = value;
                OnPropertyChanged("Angle");
            }
        }

    }


}

