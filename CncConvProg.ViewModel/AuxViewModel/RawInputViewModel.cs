using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Implementors.Wpf.MVVM;

namespace CncConvProg.ViewModel.AuxViewModel
{
    public class RawInputViewModel : ViewModelBase
    {
        private readonly RawInput _rawInput;

        public RawInputViewModel(RawInput rawInput, RawItemViewModel parent)
        {
            _rawInput = rawInput;

            parent.AddToUpdateList(this);
        }

        public RawInputViewModel(RawInput rawInput)
        {
            _rawInput = rawInput;
        }

        public void UpdateVm()
        {
            OnPropertyChanged("Value");
            OnPropertyChanged("IsUserInputed");
        }

        public event EventHandler OnSourceUpdated;

        private void RequestUpdateSource()
        {
            EventHandler handler = this.OnSourceUpdated;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public double? Value
        {
            get
            {
                return _rawInput.Value;
            }

            set
            {
                /*
                 * Allora qui succede seguente comportamento.
                 * 
                 * Ho valore non UserInputed ( ovvero calcolato dal profilo ) su X , con deltaX immesso
                 * 
                 * La casella prendee focus in automatico, poi lo perde ,
                 * in quel momento setta involontariamente il valore segnato.
                 * come fare ???
                 * 
                 * In questa maniera ogni volta che seleziono un valore poi lascio perdere è come se 
                 * lo avessi inserito.
                 * 
                 * Posso fare salto se vedo che value == con valore esistente,
                 * ma cosi non riesco a settare il valore. 
                 * 
                 * Provo cosi.. Potrei mettere quote in conflitto..
                 */

                /*
                 * todo : 
                 */

                if(value.HasValue && _rawInput.Value.HasValue)
                {
                    var v1 = Math.Round(value.Value, 7);
                    var v2 = Math.Round(_rawInput.Value.Value, 7);
                    if(v1 == v2)return;
                }
                if(value == _rawInput.Value)return;

                var isUserInputed = true;

                if (value == null)
                    isUserInputed = false;

                _rawInput.SetValue(isUserInputed, value);

                OnPropertyChanged("Value");
                OnPropertyChanged("IsUserInputed");

                RequestUpdateSource();
            }
        }

        public bool IsUserInputed
        {
            get
            {
                return _rawInput.IsUserInputed;
            }
        }
    }
}
