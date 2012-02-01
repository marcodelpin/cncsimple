using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Common
{
    /// <summary>
    /// Classe base per i ViewModel dove è richiesta validazione
    /// </summary>
    public abstract class ViewModelValidable : ViewModelBase, IValidable
    {
        protected override void OnPropertyChanged(string propertyName)
        {
            _stageModified = true;

            base.OnPropertyChanged(propertyName);
        }
        /// <summary>
        /// Flag che indica se il stage è stato modificato da ultimo ricalcolo
        /// </summary>
        private bool _stageModified = true;

        /// <summary>
        /// Flag che indica se lo stage è valido
        /// </summary>
        private bool? _stageValid;

        /// <summary>
        /// Se lo stage risulta modificato , ricalcolo il valore di valid e restituisco.
        /// </summary>
        public bool? IsValid
        {
            get
            {
                if (_stageModified || _stageValid == null)
                {
                    _stageValid = ValidateStage();
                }

                return _stageValid;
            }
        }

        public abstract bool? ValidateStage();

    }
}
