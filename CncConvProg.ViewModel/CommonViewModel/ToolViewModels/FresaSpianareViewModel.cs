using System;
using System.ComponentModel;
using System.Diagnostics;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.CommonViewModel.ToolViewModels
{
    public class FresaSpianareViewModel : ToolTreeViewItemViewModel
    {
        private readonly FresaSpianare _fresa;

        public FresaSpianareViewModel(FresaSpianare fresaTool, TreeViewItemViewModel parent) :
            base(fresaTool, parent)
        {
            _fresa = fresaTool;


        }

        public double DiametroEffettivo
        {
            get { return _fresa.Diametro; }

            set
            {
                _fresa.Diametro = value;
                OnPropertyChanged("DiametroEffettivo");
            }
        }
        public double DiametroIngombro
        {
            get { return _fresa.DiametroIngombroFresa; }

            set
            {
                _fresa.DiametroIngombroFresa = value;
                OnPropertyChanged("DiametroIngombro");
            }
        }

        public double AltezzaFresa
        {
            get { return _fresa.Altezza; }

            set
            {
                _fresa.Altezza = value;
                OnPropertyChanged("AltezzaFresa");
            }
        }


        public double RaggioInserto
        {
            get { return _fresa.RaggioInserto; }

            set
            {
                _fresa.RaggioInserto = value;
                OnPropertyChanged("RaggioInserto");
            }
        }


        #region IDataErrorInfo Members

        protected override string[] ValidatedProperties
        {
            get
            {
                return new[]
                           {
                               "DiametroEffettivo",
                               "DiametroIngombro",
                               "RaggioInserto",
                           };
            }
        }

        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "DiametroEffettivo":
                    {
                        error = InputCheck.MaggioreDiZero(DiametroEffettivo);
                    }
                    break;

                case "DiametroIngombro":
                    {
                        if (DiametroIngombro < DiametroEffettivo)
                            error = "Incorrect diameter value ";
                    }
                    break;

                case "RaggioInserto":
                    {
                        error = InputCheck.MaggioreOUgualeDiZero(RaggioInserto);
                    }break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion

    }
}