using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.LatheTool;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.ViewModel.AuxViewModel;

namespace CncConvProg.ViewModel.CommonViewModel.ParameterViewModels
{
    public class ParametroUtensileTornituraFilettaturaViewModel : ToolParameterViewModel
    {
        private ParametroUtensileTornitura ParametroUtensileTornitura
        {
            get { return Parametro as ParametroUtensileTornitura; }
        }

        public ParametroUtensileTornituraFilettaturaViewModel(Utensile utensileTornitura)
            : base(utensileTornitura)
        {
        }

        #region Property

        public double Velocita
        {
            get { return ParametroUtensileTornitura.Velocita; }
            set
            {
                if (ParametroUtensileTornitura.Velocita == value) return;
                ParametroUtensileTornitura.Velocita = value;

                OnPropertyChanged("Velocita");
            }
        }

        public ModalitaVelocita ModalitaVelocita
        {
            get { return ParametroUtensileTornitura.ModalitaVelocita; }

            set
            {
                if (ParametroUtensileTornitura.ModalitaVelocita == value) return;

                ParametroUtensileTornitura.ModalitaVelocita = value;
                OnPropertyChanged("ModalitaVelocita");
            }
        }


        #endregion

        #region IDataErrorInfo Members


        protected override string[] ValidatedProperties
        {
            get
            {
                return new string[]
                           {
                               "Velocita"
                               
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
                case "Velocita":
                    {
                        error = InputCheck.MaggioreDiZero(Velocita);
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