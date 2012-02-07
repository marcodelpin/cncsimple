using System;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.LatheTool;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.CommonViewModel.ToolViewModels
{
    public class UtensileTornituraScanalaturaViewModel : ToolTreeViewItemViewModel
    {

        private readonly UtensileScanalatura _utensileTornitura;

        public UtensileTornituraScanalaturaViewModel(UtensileScanalatura fresaTool, TreeViewItemViewModel parent) :
            base(fresaTool, parent)
        {
            _utensileTornitura = fresaTool;
        }


        public double LarghezzaUtensile
        {
            get { return _utensileTornitura.LarghezzaUtensile; }
            set
            {
                if (_utensileTornitura.LarghezzaUtensile == value) return;

                _utensileTornitura.LarghezzaUtensile = value;
                OnPropertyChanged("LarghezzaUtensile");
            }
        }

        #region IDataErrorInfo Members

        protected override string[] ValidatedProperties
        {
            get
            {
                return new[]
                           {
                               "LarghezzaUtensile",
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
                case "LarghezzaUtensile":
                    {
                        error = InputCheck.MaggioreDiZero(LarghezzaUtensile);
                    }
                    break;

                //default:
                //    Debug.Fail("Unexpected property : " + propertyName);
                //    break;
            }

            return error;
        }

        #endregion

    }
}