using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.ThreadTable;
using CncConvProg.Model.Tool;
using CncConvProg.Model.ToolMachine;
using CncConvProg.ViewModel.MainViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.Dialog
{
    public class MacchineDialogViewModel : ViewModelBase, IDialog
    {
        public ObservableCollection<ToolMachineViewModel> Machines
        {
            get
            {
                var m = new ObservableCollection<ToolMachineViewModel>();
                foreach (var toolMachine in _machines)
                {
                    var t = new ToolMachineViewModel(toolMachine);
                    m.Add(t);
                }
                return m;
            }
        }

        private ToolMachineViewModel _selectedMachine;
        public ToolMachineViewModel SelectedMachine
        {

            get { return _selectedMachine; }
            set
            {
                _selectedMachine = value;
                OnPropertyChanged("SelectedMachine");
            }
        }


        private readonly List<ToolMachine> _machines;

        public MacchineDialogViewModel(List<ToolMachine> machines)
        {
            _machines = machines;
        }

        #region New Machinde

        RelayCommand _addMachine;
        /// <summary>
        /// Salva modifiche database
        /// </summary>
        private void AddMachine(string param)
        {
            var i = int.Parse(param);

            ToolMachine machine;
            switch (i)
            {
                default:
                case 0: // mill
                    {
                        machine = new VerticalMill();
                    }
                    break;
            }

            var g = machine.MachineGuid;

            _machines.Add(machine);

            OnPropertyChanged("Machines");

            SelectedMachine = Machines.Where(m => m.MachineGuid == g).FirstOrDefault();


        }

        public ICommand AddMachineCmd
        {
            get
            {
                return _addMachine ?? (_addMachine = new RelayCommand(param => AddMachine((string)param),
                                                                              param => true));
            }
        }

        #endregion

        #region Del Machine

        RelayCommand _delMachineCmd;

        /// <summary>
        /// Salva modifiche database
        /// </summary>
        public void DelMachine(ToolMachineViewModel macchineDialogViewModel)
        {
            var name = macchineDialogViewModel.MachineName;

            var msgBoxRslt = MessageBox.Show("Delete Item ?", "Delete name", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (msgBoxRslt != MessageBoxResult.Yes)
                return;

            var machine = macchineDialogViewModel.Machine;

            if (_machines.Contains(machine))
                _machines.Remove(machine);

            SelectedMachine = Machines.FirstOrDefault();

            OnPropertyChanged("Machines");

        }

        public ICommand DelMachineCmd
        {
            get
            {
                return _delMachineCmd ?? (_delMachineCmd = new RelayCommand(param => DelMachine((ToolMachineViewModel)param),
                                                                            param => true));
            }
        }
        #endregion
        public void Save(IMainViewModel mainViewModel)
        {
           // PathFolderHelper.SaveToolMachines(_machines);
        }
    }




    /// <summary>
    /// Per ora considero solamente centro
    /// </summary>
    public class ToolMachineViewModel : ViewModelBase
    {
        private readonly ToolMachine _machine;

        public Guid MachineGuid
        {
            get { return _machine.MachineGuid; }
        }
        public ToolMachineViewModel(ToolMachine machine)
        {
            _machine = machine;
        }

        public double CostoOrarioMacchina
        {
            get { return _machine.CostoOrario; }
            set
            {
                _machine.CostoOrario = value;
                OnPropertyChanged("CostoOrarioMacchina");
            }
        }

        public int TempoCaricamentoMacchina
        {
            get { return _machine.MachineLoadingTime; }
            set
            {
                _machine.MachineLoadingTime = value;
                OnPropertyChanged("TempoCaricamentoMacchina");
            }
        }

        public int TempoSetupFixture
        {
            get { return _machine.AverageSetupFixtureTime; }
            set
            {
                _machine.AverageSetupFixtureTime = value;
                OnPropertyChanged("TempoSetupFixture");
            }
        }

        public int TempoSetupTool
        {
            get { return _machine.AverageMountingToolTime; }
            set
            {
                _machine.AverageMountingToolTime = value;
                OnPropertyChanged("TempoSetupTool");
            }
        }

        public int TempoProgrammazioneOperazione
        {
            get { return _machine.AverageProgrammingOperationTime; }
            set
            {
                _machine.AverageProgrammingOperationTime = value;
                OnPropertyChanged("TempoProgrammazioneOperazione");
            }
        }
        public int ChangeToolTime
        {
            get { return _machine.ChangeToolTime; }
            set
            {
                _machine.ChangeToolTime = value;
                OnPropertyChanged("ChangeToolTime");
            }
        }

        public string MachineName
        {
            get { return _machine.MachineName; }
            set
            {
                _machine.MachineName = value;
                OnPropertyChanged("MachineName");
            }
        }

        public ToolMachine Machine
        {
            get { return _machine; }
        }
    }
}
