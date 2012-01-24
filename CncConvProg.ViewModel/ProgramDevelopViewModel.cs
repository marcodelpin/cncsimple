using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CncConvProg.ViewModel.Interface;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Abstractions.Wpf.Intefaces;
using Framework.Abstractions.Wpf.ServiceLocation;
using MVVM_Library;

namespace CncConvProg.ViewModel
{
    public class ProgramDevelopViewModel : ViewModelBase , IScreen, IProgramDevelopViewModel
    {
        private readonly IModalDialogService _modalDialogService;
        private readonly IMessageBoxService _messageBoxService;

        public ProgramDevelopViewModel(IModalDialogService modalDialogService, IMessageBoxService messageBoxService)
        {
            _modalDialogService = modalDialogService;
            _messageBoxService = messageBoxService;
        }

        public IMainScreen Parent
        {
            get { throw new NotImplementedException(); }
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        #region Start Simulation

        RelayCommand _startSimulation;

        public ProgramDevelopViewModel(IModalDialogService modalDialogService)
        {
            this._modalDialogService = modalDialogService;
        }

        private void StartSimulation()
        {
            var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>(Constants.EditUserModalDialog);
            _modalDialogService.ShowDialog(dialog, new EditWorkViewModel(),
                                               returnedViewModelInstance =>
                                               {
                                                   if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                                                   {
                                                       // persisti modifiche
                                                   }
                                               });
        }

        public ICommand StartSimulationCmd
        {
            get
            {
                return _startSimulation ?? (_startSimulation = new RelayCommand(param => StartSimulation(),
                                                                                param => true));
            }
        }

        #endregion
    }
}
