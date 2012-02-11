#region Using Namespace
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool;
using CncConvProg.Model.ToolMachine;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.AuxViewModel.TreeViewModel;
using CncConvProg.ViewModel.Dialog;
using CncConvProg.ViewModel.EditWorkDialog;
using CncConvProg.ViewModel.EditWorkDialog.Foratura.Common;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Cava;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Contornatura;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Filettatura;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Lato;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaChiusa;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaLinea;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Spianatura;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.TextEngraving;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Abstractions.Wpf.Intefaces;
using Framework.Abstractions.Wpf.ServiceLocation;
using GongSolutions.Wpf.DragDrop;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

#endregion

namespace CncConvProg.ViewModel.MainViewModel
{
    public partial class MainViewModel : ViewModelBase, IMainViewModel
    {
        private readonly IModalDialogService _modalDialogService;
        private readonly IMessageBoxService _messageBoxService;
        private List<ToolMachine> _machines;

        /// <summary>
        /// la tabella filettatura mi interessa caricarla solamente quando ne ho necessita.
        /// le macchine invece mi servono sempre..
        /// </summary>
        //private TabellaFilettature _tabellaFilettatura;

        public MainViewModel(IModalDialogService modalDialogService, IMessageBoxService messageBoxService)
        {

            _modalDialogService = modalDialogService;
            _messageBoxService = messageBoxService;

            ProgramPreference preference;

            try
            {
                LoadData();
                preference = PathFolderHelper.GetPreferenceData();

                if (preference == null)
                {
                    // Creo nuovo e apro finestra dialogo preferenze
                    preference = new ProgramPreference();
                    OpenDialog(DialogEnum.UnitSelection);
                }

                //_model = new Singleton(_preference.MeasureUnit);

            }
            catch (Exception exception)
            {
                throw new Exception("MainViewModel.MainViewModel.( load data stage)");
            }

            Singleton.CreateNewModelClass(preference.DefaultMeasureUnit);



            Mvm = new ClassModelViewModel(_modalDialogService, _messageBoxService, Singleton.Instance);
            //ResetGui(Singleton.Instance);
        }

        private ClassModelViewModel _mvm;
        public ClassModelViewModel Mvm
        {
            get { return _mvm; }

            set
            {
                _mvm = value;
                OnPropertyChanged("Mvm");
            }
        }

        //public string CodiceArticolo
        //{
        //    get
        //    {
        //        return Singleton.Instance.CodiceArticolo;
        //    }

        //    set
        //    {
        //        Singleton.Instance.CodiceArticolo = value;
        //        OnPropertyChanged("CodiceArticolo");
        //    }
        //}

        //public Materiale MaterialeSelezionato
        //{
        //    get
        //    {
        //        return _model.Materiale;
        //    }

        //    set
        //    {

        //        _model.Materiale = value;
        //        OnPropertyChanged("MaterialeSelezionato");
        //    }
        //}


        //public ObservableCollection<Materiale> Materiali
        //{
        //    get
        //    {
        //        return new ObservableCollection<Materiale>(_magazzinoUtensile.GetMaterials());
        //    }
        //}


        public ObservableCollection<HorizontalLathe2Axis> HorizontalLatheMachines
        {
            get
            {
                var l = _machines.OfType<HorizontalLathe2Axis>();
                return new ObservableCollection<HorizontalLathe2Axis>(l);
            }
        }


        public ObservableCollection<LatheAxisC> Lathe3AxisMachines
        {
            get
            {
                var v = _machines.OfType<LatheAxisC>();
                return new ObservableCollection<LatheAxisC>(v);
            }
        }


        public ObservableCollection<VerticalMill> MillMachines
        {
            get
            {
                var v = _machines.OfType<VerticalMill>();
                return new ObservableCollection<VerticalMill>(v);
            }
        }
        private void LoadData()
        {
            try
            {
                /* todo :
                 * 
                 * qui c'è problema logico . se lancio eccezione durante caricamento di un file mi salta tutto il loadData
                 * 
                 */
                /*
                 * controllo che 
                 */

                /*
                 * Controllo che il file delle preferenze esista..
                 */
                var preference = PathFolderHelper.GetPreferenceData();

                if (preference == null)
                {
                    // Creo nuovo e apro finestra dialogo preferenze
                    preference = new ProgramPreference();
                    OpenDialog(DialogEnum.UnitSelection);
                }

                // qui la classe preference deve essere settata per forza..

                _machines = PathFolderHelper.GetToolMachines();

                OnPropertyChanged("HorizontalLatheMachines");
                OnPropertyChanged("Lathe3AxisMachines");
                OnPropertyChanged("MillMachines");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante caricamento data");
            }
        }

        #region Open Dialog

        RelayCommand _openDialog;



        private void OpenDialog(DialogEnum dialogEnum)
        {
            ViewModelBase viewModelBase = null;
            var dialogKey = string.Empty;

            switch (dialogEnum)
            {
                case DialogEnum.TabellaFilettatura:
                    {
                        var tabellaFilettatura = PathFolderHelper.GetTabellaFilettatura();

                        /*
                         * todo prendere measure unit da preferenze
                         */
                        viewModelBase = new TabellaFilettaturaViewModel(tabellaFilettatura, Singleton.Instance.MeasureUnit);

                        dialogKey = Constants.TabellaFilettaturaModalDialog;


                    } break;

                case DialogEnum.MacchineDialogo:
                    {
                        viewModelBase = new MacchineDialogViewModel(_machines);
                        dialogKey = Constants.MacchineModalDialog;

                    } break;

                case DialogEnum.MaterialiDialogo:
                    {
                        //var magazzinoUtensile = PathFolderHelper.GetMagazzinoUtensile();

                        viewModelBase = new MaterialiDialogoViewModel();

                        dialogKey = Constants.MaterialiModalDialog;


                    } break;

                case DialogEnum.UtensiliDialogo:
                    {

                        //var magazzinoUtensile = PathFolderHelper.GetMagazzinoUtensile();

                        viewModelBase = new UtensiliDialogoViewModel(Singleton.Instance.MeasureUnit);

                        dialogKey = Constants.UtensiliModalDialog;

                    } break;

                case DialogEnum.UnitSelection:
                    {
                        var preference = PathFolderHelper.GetPreferenceData();

                        if (preference == null)
                        {
                            // Creo nuovo e apro finestra dialogo preferenze
                            preference = new ProgramPreference();
                        }

                        viewModelBase = new ProgramPreferenceViewModel(preference);

                        dialogKey = Constants.UnitSelectionDialog;

                    } break;

                case DialogEnum.DettagliArticolo:
                    {
                        viewModelBase = new ArticleDetailViewModel();

                        dialogKey = Constants.ArticleDetailDialog;
                    } break;


                case DialogEnum.ProgramPreference:
                    {
                        //viewModelBase = new ArticleDetailViewModel();

                        //dialogKey = Constants.ArticleDetailDialog;
                    } break;

                default:
                    throw new NotImplementedException("MainViewModel.OpenDialog");
            }

            var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>(dialogKey);

            _modalDialogService.ShowDialog(dialog, viewModelBase,
                                               returnedViewModelInstance =>
                                               {
                                                   if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                                                   {
                                                       if (viewModelBase is IDialog)
                                                       {
                                                           var d = viewModelBase as IDialog;

                                                           d.Save(this);
                                                       }
                                                   }
                                               });
        }

        public ICommand OpenDialogCmd
        {
            get
            {
                return _openDialog ?? (_openDialog = new RelayCommand(param => OpenDialog((DialogEnum)param),
                                                                                param => true));
            }
        }

        #endregion




        #region New File

        RelayCommand _newFileCmd;

        private void NewFile(bool forceNewFile = false)
        {
            var previousFilePath = Mvm.FilePath;

            try
            {
                var messageBox = MessageBox.Show("Save current file ? ", "New File",
                                                                MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                                                                MessageBoxResult.Cancel);

                if (!forceNewFile)
                    if (messageBox == MessageBoxResult.Cancel)
                        return;

                if (messageBox == MessageBoxResult.Yes)
                    Mvm.Save();

                /* 
                 * apro nuova finestra dialogo  e chiedo inch / mm
                 * 
                 * oppure faccio file impostazioni a inizio ?
                 * 
                 * se faccio cosi come faccio a cambiare ?? oppure cambio da settaggio
                 * 
                 * siccome è ovvio che se parto con mm continuo 
                 */

                var preference = PathFolderHelper.GetPreferenceData();

                if (preference == null)
                {
                    // Creo nuovo e apro finestra dialogo preferenze
                    preference = new ProgramPreference();
                }

                Singleton.CreateNewModelClass(preference.DefaultMeasureUnit);

                var model = Singleton.Instance;

                Mvm = new ClassModelViewModel(_modalDialogService, _messageBoxService, model);

            }
            catch (Exception)
            {
                MessageBox.Show("Errore Creazione nuovo file");
            }

        }

        public ICommand NewFileCmd
        {
            get
            {
                return _newFileCmd ?? (_newFileCmd = new RelayCommand(param => NewFile(),
                                                                      param => true));
            }
        }

        #endregion



        #region Open File Dialog

        RelayCommand _openFileDialog;

        private void OpenFileDlg()
        {
            try
            {
                var ofd = new OpenFileDialog
                {
                    Filter = "Cnc Simple | *.csp"
                };

                var o = ofd.ShowDialog();

                if (o != DialogResult.OK) return;

                var currentFilePath = ofd.FileName;

                var model = FileUtility.Deserialize<FileModel>(currentFilePath);

                // aggiorno filepath
                model.CurrentFilePath = currentFilePath;

                Singleton.SetModelClass(model);

                Mvm = new ClassModelViewModel(_modalDialogService, _messageBoxService, model);

            }
            catch (Exception exception)
            {
                MessageBox.Show("Caricamento file fallito", "Errore", MessageBoxButton.OK,
                                               MessageBoxImage.Error, MessageBoxResult.None);
            }

        }

        public ICommand OpenFileDialogCmd
        {
            get
            {
                return _openFileDialog ?? (_openFileDialog = new RelayCommand(param => OpenFileDlg(),
                                                                                param => true));
            }
        }

        #endregion



        public void RequestNewFile()
        {
            NewFile(true);
        }
    }

    public enum EnumWork
    {
        Tornitura,
        TornituraScanalaturaEsterna,
        TornituraScanalaturaInterna,
        TornituraScanalaturaFrontale,
        TornituraFilettatura,

        TornioForaturaCentraleSemplice,
        TornioForaturaCentraleMaschiatura,
        TornioForaturaCentraleAlesatura,
        TornioForaturaCentraleLamatura,

        FilettaturaInterna,
        FilettaturaEsterna,
        TornituraSfacciatura,
        TornituraEsterna,
        TornituraInterna,
        ScanalaturaEsterna,
        ScanalaturaInterna,

        FresaturaSpianatura,
        FresaturaContornatura,
        FresaturaCava,
        FresaturaScanalaturaLinea,
        FresaturaLato,
        FresaturaScanalaturaChiusa,
        FresaturaFilettare,
        TextEngraving,

        ForaturaSemplice,
        Maschiatura,
        Alesatura,
        Barenatura,
        Lamatura

    }

    public enum DialogEnum
    {
        TabellaFilettatura,
        MacchineDialogo,
        UtensiliDialogo,
        MaterialiDialogo,
        UnitSelection,
        DettagliArticolo,
        ProgramPreference

    }
}

