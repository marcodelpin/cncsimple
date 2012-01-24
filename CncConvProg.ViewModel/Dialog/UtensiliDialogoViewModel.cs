using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CncConvProg.Model;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.ThreadTable;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.ViewModel.CommonViewModel.ToolViewModels;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MainViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.Dialog
{
    /*
     * si è deciso che devo sistemare schermata utensili..
     * 
     * per ora non mi preoccupo del materiale.
     *
     * intanto devo separo tool inch da tool mm
     * 
     * poi cerco un modo automattizzato di separare i vari tool
     * 
     * magari 
     * 
     * con metodo..
     * 
     * o itero vari tool e a seconda li metto in lista differente
     * 
     * o raggruppo vari tool !! tramite linq cast !! ok per questo.
     */
    public class UtensiliDialogoViewModel : ViewModelBase, IDialog
    {
        private MagazzinoUtensile _magazzinoUtensile;
        private readonly MeasureUnit _measureUnit;

        public UtensiliDialogoViewModel(MagazzinoUtensile magazzinoUtensile, MeasureUnit measureUnit)
        {
            _measureUnit = measureUnit;
            _magazzinoUtensile = magazzinoUtensile;

            /*
             * ho intenzione di dividere utensili in base a genere .. poi se vedo che non basta fare ulteriore split..
             */
            UpdateTreeView();

            MaterialeSelezionato = Materiali.FirstOrDefault();
        }

        private ToolTreeViewItemViewModel _utensileSelezionato;
        public ToolTreeViewItemViewModel UtensileSelezionato
        {

            get { return _utensileSelezionato; }
            set
            {
                _utensileSelezionato = value;
                if (_utensileSelezionato != null && MaterialeSelezionato != null)
                    _utensileSelezionato.RefreshParameterList(MaterialeSelezionato, _measureUnit);

                OnPropertyChanged("UtensileSelezionato");
            }
        }


        private ObservableCollection<TreeViewItemViewModel> _treeView = new ObservableCollection<TreeViewItemViewModel>();
        public ObservableCollection<TreeViewItemViewModel> TreeView
        {
            get { return _treeView; }

            set
            {
                _treeView = value;
                OnPropertyChanged("TreeView");
            }
        }


        private Materiale _materialeSelezionato;
        public Materiale MaterialeSelezionato
        {
            get
            {
                return _materialeSelezionato;
            }

            set
            {
                _materialeSelezionato = value;

                if (UtensileSelezionato != null && _materialeSelezionato != null)
                    UtensileSelezionato.RefreshParameterList(_materialeSelezionato, _measureUnit);

                OnPropertyChanged("MaterialeSelezionato");
            }
        }

        public ObservableCollection<Materiale> Materiali
        {
            get
            {
            //    var mat = _magazzinoUtensile.GetMaterials();
            //    if (mat != null)
            //        return new ObservableCollection<Materiale>(mat);

                return new ObservableCollection<Materiale>();
            }
        }

        readonly Dictionary<Type, string> _toolDictName = new Dictionary<Type, string> 
                                   {
                                       {typeof (Punta), "Drill"},
                                       {typeof (Svasatore), "Chamfer"},
                                       {typeof (Alesatore), "Reamer"},
                                       {typeof (Centrino), "Center Drill"},
                                       {typeof (Lamatore), "Counterbore"},
                                       {typeof (Bareno), "Bore"},
                                       {typeof (Maschio), "Tap"},

                                       {typeof (FresaCandela), "Mill"},
                                       {typeof (FresaSpianare), "Face Mill"},
                                       {typeof (FresaFilettare), "Thread Mill"},
                                       
                                   };

        private void UpdateTreeView()
        {
            if (TreeView.Count > 0)
                TreeView.Clear();

            foreach (var keyValuePair in _toolDictName)
            {
                var tools = _magazzinoUtensile.GetTools(keyValuePair.Key, _measureUnit);

                var drillTreeItem = new ToolTypeItemViewModel(keyValuePair.Value);

                drillTreeItem.OnItemSelected += TOnItemSelected;

                foreach (var drillTool in tools)
                {
                    var t = ToolTreeViewItemViewModel.GetViewModel(drillTool, drillTreeItem);

                    t.OnItemSelected += TOnItemSelected;

                    drillTreeItem.Children.Add(t);
                }

                TreeView.Add(drillTreeItem);

            }


        }

        //private object _selectedScreen;
        //public object SelectedScreen
        //{
        //    get
        //    {
        //        return _selectedScreen;
        //    }
        //    set
        //    {
        //        _selectedScreen = value;
        //        OnPropertyChanged("SelectedScreen");
        //    }
        //}

        void TOnItemSelected(object sender, EventArgs e)
        {
            UtensileSelezionato = sender as ToolTreeViewItemViewModel;
        }

        #region Add Tool

        RelayCommand _addToolCmd;
        /// <summary>
        /// Salva modifiche database
        /// </summary>
        private void AddTool(ToolTypeEnum toolType)
        {
            Utensile tool = null;
            switch (toolType)
            {
                case ToolTypeEnum.Punta:
                    {
                        tool = new Punta(_measureUnit);
                    } break;
                case ToolTypeEnum.Centrino:
                    {
                        tool = new Centrino(_measureUnit);
                    } break;
                case ToolTypeEnum.Svasatore:
                    {
                        tool = new Svasatore(_measureUnit);
                    } break;
                case ToolTypeEnum.Lamatore:
                    {
                        tool = new Lamatore(_measureUnit);
                    } break;
                case ToolTypeEnum.Bareno:
                    {
                        tool = new Bareno(_measureUnit);
                    } break;
                case ToolTypeEnum.Maschio:
                    {
                        tool = new Maschio(_measureUnit);
                    } break;

                case ToolTypeEnum.FresaCandela:
                    {
                        tool = new FresaCandela(_measureUnit);
                    } break;

                case ToolTypeEnum.Alesatore:
                    {
                        tool = new Alesatore(_measureUnit);
                    } break;

                case ToolTypeEnum.FresaSpianare:
                    {
                        tool = new FresaSpianare(_measureUnit);
                    } break;

                default:
                    throw new NotImplementedException("UtViewModel.AddTool");

            }

            var guid = tool.ToolGuid;

            _magazzinoUtensile.SaveTool(tool);

            UpdateTreeView();

            foreach (var treeViewItemViewModel in _treeView)
            {
                foreach (var viewItemViewModel in treeViewItemViewModel.Children)
                {
                    var tvm = viewItemViewModel as ToolTreeViewItemViewModel;

                    if (tvm != null)
                    {
                        if (tvm.ToolGuid == guid)
                        {
                            tvm.IsSelected = true;
                            break;
                        }
                    }
                }
            }
        }

        public ICommand NewToolCmd
        {
            get
            {
                return _addToolCmd ?? (_addToolCmd = new RelayCommand(param => AddTool((ToolTypeEnum)param),
                                                                                param => true));
            }
        }

        #endregion


        public void Save(IMainViewModel mainViewModel)
        {
            //throw new NotImplementedException();
            PathFolderHelper.SaveMagazzinoUtensile(_magazzinoUtensile);

        }

        //

        #region Add Tool

        RelayCommand _resetStoreCmd;
        private void ResetStoreCmd()
        {
            var msg = MessageBox.Show("Reset Tool Store ?", "Reset Tool Store", MessageBoxButton.YesNo,
                                                     MessageBoxImage.Warning, MessageBoxResult.None);

            if(msg != MessageBoxResult.Yes)return;

            _magazzinoUtensile = new MagazzinoUtensile();

            UpdateTreeView();
        }

        public ICommand ResetToolStoreCmd
        {
            get
            {
                return _resetStoreCmd ?? (_resetStoreCmd = new RelayCommand(param => ResetStoreCmd(),
                                                                                param => true));
            }
        }

        #endregion

    }

    public enum ToolTypeEnum
    {
        FresaCandela,
        FresaSpianare,

        // Drill Type
        Punta,
        Alesatore,
        Svasatore,
        Bareno,
        Maschio,
        Lamatore,
        Centrino,

        /* Utensili Tornitura */
        UtensileTornire,
        UtensileFilettare,
        UtensileScanalatura,

    }
    public class ToolTypeItemViewModel : TreeViewItemViewModel
    {
        public ToolTypeItemViewModel(string label)
            : base(label, null)
        {

        }


    }
}


///// Salva modifiche database
///// </summary>
//private void AddThreadType(string type)
//{
//    byte tipologia;

//    byte.TryParse(type, out tipologia);

//    TipologiaFilettatura threadType = new TipologiaFilettatura();

//    //switch ((ThreadType)tipologia)
//    //{
//    //    case ThreadType.Metrica:
//    //        {
//    //            threadType = new FilettaturaMetrica();

//    //        } break;

//    //    case ThreadType.InPollici:
//    //        {
//    //            threadType = new FilettaturaInPollici();
//    //        } break;

//    //    default:
//    //        break;
//    //}

//    threadType.Descrizione = "<new>";

//    TipiMaschiatura.Add(threadType);
//}

//public ICommand NewThreadTypeCmd
//{
//    get
//    {
//        return _addThreadCmd ?? (_addThreadCmd = new RelayCommand(param => AddThreadType((string)param),
//                                                                  param => true));
//    }
//}

//#endregion

//#region New Row

//RelayCommand _addThreadRowCmd;
///// <summary>
///// Salva modifiche database
///// </summary>
//private void AddThreadRow()
//{
//    var row = new RigaTabellaFilettatura()
//    {
//        //TipologiaMaschiatura = TipoMaschiaturaSelezionato,
//    };

//  //  var threaRep = new MaschiatureRepository(_dataSource);

//    //threaRep.Add(row);

//    //threaRep.Save();

//    TipoMaschiaturaSelezionato.ThreadItem.Add(row);

//    OnPropertyChanged("TipoMaschiaturaSelezionato");
//}

//public bool CanAddThreadRow
//{
//    get { return TipoMaschiaturaSelezionato != null; }
//}

//public ICommand AddThreadRowCmd
//{
//    get
//    {
//        return _addThreadRowCmd ?? (_addThreadRowCmd = new RelayCommand(param => AddThreadRow(),
//                                                                        param => CanAddThreadRow));
//    }
//}

//#endregion


//#region Del Tool

//RelayCommand _delToolCmd;

///// <summary>
///// Salva modifiche database
///// </summary>
//public void DeleteThreadType()
//{
//    //..

//}

//Boolean CanDelThreadType
//{
//    get
//    {
//        return TipoMaschiaturaSelezionato != null &&
//            TipoMaschiaturaSelezionato.Filettature.Count == 0;
//    }
//}

//public ICommand DelToolCmd
//{
//    get
//    {
//        return _delToolCmd ?? (_delToolCmd = new RelayCommand(param => DeleteThreadType(),
//                                                              param => CanDelThreadType));
//    }
//}

//#endregion

// private static IEnumerable<UtensileViewModel> GetTreeViewUpdated(IEnumerable<UtensileViewModel> oldTreeViews, IEnumerable<FaseDiLavoro> collectionSource)
//{
//    /*
//     * Per aggiornare il treeView ,
//     *  - ricreo il treeView
//     *  - guardo se i nuovi oggetti erano già presenti nel vecchio viewModel
//     *  - se si prendo i loro valori di expandede e selected 
//     */

//    foreach (var faseDiLavoro in collectionSource)
//    {
//        var fase = faseDiLavoro;

//        var faseTreeViewModel = new FaseLavoroTreeView(faseDiLavoro, null);

//        var oldPhaseViewModel = oldTreeViews.FirstOrDefault(l => l.FaseDiLavoro == fase);

//        if (oldPhaseViewModel != null)
//        {
//            faseTreeViewModel.IsExpanded = oldPhaseViewModel.IsExpanded;
//            faseTreeViewModel.IsSelected = oldPhaseViewModel.IsSelected;
//        }

//        foreach (var lavorazione in fase.Lavorazioni)
//        {
//            var lavorazioneViewModel = new LavorazioneTreeView(lavorazione, faseTreeViewModel);

//            var oldViewModel = oldTreeViews.FirstOrDefault(l => l.FaseDiLavoro == fase);

//            if (oldViewModel != null)
//            {
//                lavorazioneViewModel.IsExpanded = oldViewModel.IsExpanded;
//                lavorazioneViewModel.IsSelected = oldViewModel.IsSelected;
//            }

//            //check
//            faseTreeViewModel.Children.Add(lavorazioneViewModel);

//        }

//        yield return faseTreeViewModel;

//    }


