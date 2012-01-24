using System;
using System.ComponentModel;
using System.Diagnostics;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.CommonViewModel.ToolViewModels
{
    public class PuntaViewModel : ToolTreeViewItemViewModel
    {
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
                //case "DiametroEffettivo":
                //    {
                //        error = InputCheck.MaggioreDiZero(DiametroEffettivo);
                //    }
                //    break;

                //case "DiametroIngombro":
                //    {
                //        if (DiametroIngombro < DiametroEffettivo)
                //            error = "Incorrect diameter value ";
                //    }
                //    break;

                //case "RaggioInserto":
                //    {
                //        error = InputCheck.MaggioreOUgualeDiZero(RaggioInserto);
                //    } break;

                //default:
                //    Debug.Fail("Unexpected property : " + propertyName);
                //    break;
            }

            return error;
        }

        #endregion

        
        public double Diametro
        {
            get { return _drill.Diametro; }

            set
            {
                _drill.Diametro = value;
                OnPropertyChanged("Diametro");
            }
        }

        private readonly DrillTool _drill;

        public PuntaViewModel(DrillTool drillTool, TreeViewItemViewModel parent) :
            base(drillTool, parent)
        {
            _drill = drillTool;


        }
    }
}