using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.Model;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Implementors.Wpf.MVVM;

namespace CncConvProg.ViewModel.AuxViewModel
{
    public class UserInputViewModel : ViewModelBase, IDataErrorInfo
    {

        private readonly UserInput _userInput;

        public delegate string Validate(string propertyName);

        private readonly Validate _validate;

        private readonly string _param;

        public UserInputViewModel(UserInput userInput, Validate validationMethod, string propertyName)
        {
            _param = propertyName;
            _validate = validationMethod;
            _userInput = userInput;

        }

        public UserInputViewModel(UserInput userInput)
        {
            _userInput = userInput;

        }

        public void SetValue(bool isUserInputed, double? value)
        {
            _userInput.SetValue(isUserInputed, value);

            OnPropertyChanged("Value");
            OnPropertyChanged("IsUserInputed");
        }


        public event EventHandler OnSourceUpdated;

        private void RequestUpdateSource()
        {
            EventHandler handler = OnSourceUpdated;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        public double? Value
        {
            get
            {
                if (_userInput.Value.HasValue)
                    return _userInput.Value.Value;
                else
                    return null;
            }

            set
            {
                if (_userInput.Value == value) return;

                _userInput.SetValue(true, value);

                OnPropertyChanged("Value");
                OnPropertyChanged("IsUserInputed");

                RequestUpdateSource();
            }
        }

        public bool IsUserInputed
        {
            get
            {
                return _userInput.IsUserInputed;
            }
        }

        #region Validation

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
            get
            {
                return GetValidationError(_param) == null;
            }
        }

        static readonly string[] ValidatedProperties = { };

        string GetValidationError(string propertyName)
        {
            //if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
            //    return null;

            string error = null;

            if (_validate != null)
                error = _validate(_param);

            return error;
        }

        #endregion // Validation

        internal void Update()
        {
            OnPropertyChanged("Value");
            OnPropertyChanged("IsUserInputed");
        }
    }
}
