using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura;
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
using CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Filettatura;
using CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Scanalatura;
using CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Tornitura.ProfiloLibero;
using CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Tornitura.Sfacciatura;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Abstractions.Wpf.Intefaces;
using Framework.Abstractions.Wpf.ServiceLocation;
using GongSolutions.Wpf.DragDrop;
using IDropTarget = System.Windows.Forms.IDropTarget;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace CncConvProg.ViewModel.MainViewModel
{
    public class ClassModelViewModel : ViewModelBase, GongSolutions.Wpf.DragDrop.IDropTarget
    {
        private readonly IModalDialogService _modalDialogService;
        private readonly IMessageBoxService _messageBoxService;

        private FileModel _model;

        public ClassModelViewModel(IModalDialogService modalDialogService, IMessageBoxService messageBoxService, FileModel model)
        {
            _modalDialogService = modalDialogService;
            _messageBoxService = messageBoxService;

            _model = model;
            UpdateTreeView();
        }

        public string MeasureUnitLabel
        {
            get
            {
                switch (_model.MeasureUnit)
                {
                    case MeasureUnit.Inch:
                        return "Inch";

                    default:
                        return "Millimeter";
                }
            }
        }


        public Materiale Materiale
        {
            get
            {
                if (Singleton.Instance.Materiale == null)
                    Singleton.Instance.Materiale = Singleton.Data.GetMateriali(Singleton.Instance.MeasureUnit).FirstOrDefault();

                return Singleton.Instance.Materiale;
            }

            set
            {
                Singleton.Instance.Materiale = value;
                OnPropertyChanged("Materiale");
            }
        }


        public ObservableCollection<Materiale> Materiali
        {
            get
            {
                var mat = Singleton.Data.GetMateriali(Singleton.Instance.MeasureUnit);
                return new ObservableCollection<Materiale>(mat);
            }
        }

        private ObservableCollection<FaseLavoroTreeView> _treeView = new ObservableCollection<FaseLavoroTreeView>();
        public ObservableCollection<FaseLavoroTreeView> TreeView
        {
            get { return _treeView; }

            set
            {
                _treeView = value;
                OnPropertyChanged("TreeView");
            }
        }

        /// <summary>
        /// Aggiorna controllo ad albero lasciando invariato la proprietà expanded degli elementi
        /// </summary>
        public void UpdateTreeView()
        {
            var selectedFaseVm = Guid.Empty;

            if (FaseSelectedViewModel != null)
                selectedFaseVm = FaseSelectedViewModel.FaseDiLavoroGuid;

            var treeViews = GetTreeViewUpdated(TreeView, Singleton.Instance.GetFasiDiLavoro());

            TreeView = new ObservableCollection<FaseLavoroTreeView>(treeViews);

            foreach (var faseLavoroTreeView in _treeView)
                faseLavoroTreeView.OnItemSelected += OnTreeViewItemSelected;


            if (selectedFaseVm != Guid.Empty)
            {
                var l = treeViews.Where(o => o.FaseDiLavoroGuid == selectedFaseVm);

                if (l != null)
                    FaseSelectedViewModel = l.FirstOrDefault();
            }

            else
            {
                FaseSelectedViewModel = treeViews.FirstOrDefault();

            }

            UpdateOperationsList();

            UpdatePreview();

        }

        private ObservableCollection<OperationMainScreenViewModel> _operationList;
        public ObservableCollection<OperationMainScreenViewModel> OperationList
        {
            get { return _operationList; }

            set
            {
                _operationList = value;
                OnPropertyChanged("OperationList");
            }
        }


        /// <summary>
        /// Aggiorna lista operazioni fase selezionata
        /// </summary>
        private void UpdateOperationsList()
        {
            var operationList = new ObservableCollection<OperationMainScreenViewModel>();

            if (FaseSelectedViewModel == null)
            {
                OperationList = null;
                return;
            }
            var selectedPhase = FaseSelectedViewModel.FaseDiLavoro;

            var operations = Singleton.Instance.GetOperationList(selectedPhase.FaseDiLavoroGuid);

            foreach (var operazione in operations)
            {
                operationList.Add(new OperationMainScreenViewModel(operazione));

            }


            OperationList = operationList;

            UpdateChangeToolOptional(OperationList);

        }

        #region Operation List DragDrop

        public void Drop(DropInfo dropInfo)
        {
            var opTarget = dropInfo.TargetItem as OperationMainScreenViewModel;
            var opData = dropInfo.Data as OperationMainScreenViewModel;

            if (opTarget != null && opData != null)
            {
                //var destIndex = OperationList.IndexOf(opData);
                var partIndex = OperationList.IndexOf(opTarget);

                OperationList.Remove(opData);
                OperationList.Insert(partIndex, opData);

                // riaggiorno opIndex.
                var opInd = 0;

                foreach (var operationMainScreenViewModel in _operationList)
                {
                    operationMainScreenViewModel.OrderIndex = opInd;
                    opInd++;
                }

                UpdateChangeToolOptional(OperationList);
            }

            var lavTarget = dropInfo.TargetItem as LavorazioneTreeView;
            var lav = dropInfo.Data as LavorazioneTreeView;
            var faseTarget = dropInfo.TargetItem as FaseLavoroTreeView;

            if (faseTarget != null && lav != null)
            {
                if (faseTarget.FaseDiLavoro.IsCompatible(lav.Lavorazione))
                {
                    faseTarget.FaseDiLavoro.AddLavorazione(lav.Lavorazione);
                    UpdateTreeView();
                }
            }
            else if (lav != null && lavTarget != null)
            {
                if (!lavTarget.Lavorazione.IsCompatible(lav.Lavorazione)) return;

                /* Ora o modificato la lista nel treeview, 
                * Devo persisterla nel model.
                */
                var faseT = TreeView.Where(t => t.Children.Contains(lavTarget)).FirstOrDefault();

                if (faseT == null) return;

                var lavorazioniVm = faseT.Children.OfType<LavorazioneTreeView>().ToList();

                var faseModel = faseT.FaseDiLavoro;

                var partIndex = lavorazioniVm.IndexOf(lavTarget);

                var cIndex = lavorazioniVm.IndexOf(lav);

                var removed = lavorazioniVm.Remove(lav);

                lavorazioniVm.Insert(partIndex, lav);

                /* itero fra le operazioni vm e riaggiorno index e fase di lavoro guid.*/
                var opInd = 1;
                foreach (var treeViewItemViewModel in lavorazioniVm)
                {
                    var lavModel = Singleton.Instance.GetLavorazione(treeViewItemViewModel.Lavorazione.LavorazioneGuid);

                    lavModel.LavorazionePosition = opInd;

                    opInd++;

                    faseModel.AddLavorazione(lavModel);
                }

                faseModel.ResetLavorazioniNumeration();

                UpdateTreeView();
            }
        }

        public void DragOver(DropInfo dropInfo)
        {
            if (dropInfo.Data is OperationMainScreenViewModel && dropInfo.TargetItem is OperationMainScreenViewModel)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }

            var fase = dropInfo.TargetItem as FaseLavoroTreeView;
            var lavorazioneTarget = dropInfo.TargetItem as LavorazioneTreeView;
            var lavorazione = dropInfo.Data as LavorazioneTreeView;

            if (lavorazione != null && fase != null)
            {
                if (fase.FaseDiLavoro.IsCompatible(lavorazione.Lavorazione))
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = System.Windows.DragDropEffects.Move;
                }
                else
                {
                    dropInfo.Effects = System.Windows.DragDropEffects.None;
                }
            }

            if (lavorazione != null && lavorazioneTarget != null)
            {
                if (lavorazione.Lavorazione.IsCompatible(lavorazioneTarget.Lavorazione))
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    dropInfo.Effects = System.Windows.DragDropEffects.Move;
                }
            }
        }

        #endregion
        //#region Operation List DragDrop

        //public void Drop(DropInfo dropInfo)
        //{
        //    var op = (OperationMainScreenViewModel)dropInfo.TargetItem;
        //    var opData = (OperationMainScreenViewModel)dropInfo.Data;
        //    //var destIndex = OperationList.IndexOf(opData);
        //    var partIndex = OperationList.IndexOf(op);

        //    OperationList.Remove(opData);
        //    OperationList.Insert(partIndex, opData);

        //    // riaggiorno opIndex.
        //    var opInd = 0;

        //    foreach (var operationMainScreenViewModel in _operationList)
        //    {
        //        operationMainScreenViewModel.OrderIndex = opInd;
        //        opInd++;
        //    }

        //    UpdateChangeToolOptional(OperationList);
        //}

        //public void DragOver(DropInfo dropInfo)
        //{
        //    if (dropInfo.Data is OperationMainScreenViewModel && dropInfo.TargetItem is OperationMainScreenViewModel)
        //    {
        //        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        //        dropInfo.Effects = System.Windows.DragDropEffects.Move;
        //    }
        //}

        //#endregion

        RelayCommand _cloneWorkCmd;

        private void CloneWork(LavorazioneTreeView item)
        {
            if (item == null) return;

            var lav = item.Lavorazione;

            var deepCopy = FileUtility.DeepCopy(lav);

            //Rigenero Guid

            deepCopy.RegeneretaGuid();

            Singleton.Instance.AddLavorazione(deepCopy);

            UpdateTreeView();
        }

        public ICommand CloneWorkCmd
        {
            get
            {
                return _cloneWorkCmd ?? (_cloneWorkCmd = new RelayCommand(param => CloneWork((LavorazioneTreeView)param),
                                                                          param => true));
            }
        }

        RelayCommand _sortOperationCmd;

        private void SortOperation()
        {
            if (OperationList == null || OperationList.Count == 0) return;
            /*
             * per riordinare operazioni 
             * 
             * - devo innanzitutto una lista di rifermenti per delle priorita.
             * 
             * - poi ricombino indici delle operazioni
             */
            var opComparer = new OperationComparer();

            var opList = OperationList.ToList();

            opList.Sort(opComparer);

            // riaggiorno opIndex.
            var opInd = 0;



            OperationList = new ObservableCollection<OperationMainScreenViewModel>(opList);

            foreach (var operationMainScreenViewModel in _operationList)
            {
                operationMainScreenViewModel.OrderIndex = opInd;
                opInd++;
            }

            UpdateChangeToolOptional(OperationList);

        }

        private static void UpdateChangeToolOptional(IEnumerable<OperationMainScreenViewModel> operationMainScreenViewModels)
        {
            int? toolNumber = null;

            foreach (var operationMainScreenViewModel in operationMainScreenViewModels)
            {
                var currentToolNumber = operationMainScreenViewModel.NumeroUtensile;

                if (toolNumber.HasValue && toolNumber.Value == currentToolNumber)
                    operationMainScreenViewModel.SetOptionalToolChange(true);
                else
                    operationMainScreenViewModel.SetOptionalToolChange(false);

                toolNumber = currentToolNumber;
            }
        }

        private static void UpdateChangeToolOptional(IEnumerable<Operazione> operaziones)
        {
            int? toolNumber = null;

            foreach (var operationMainScreenViewModel in operaziones)
            {
                var currentToolNumber = operationMainScreenViewModel.GetToolPosition();

                if (toolNumber.HasValue && toolNumber.Value == currentToolNumber)
                    operationMainScreenViewModel.ToolChangeOptional = true;
                else
                    operationMainScreenViewModel.ToolChangeOptional = false;

                toolNumber = currentToolNumber;
            }
        }

        public class OperationComparer : IComparer<OperationMainScreenViewModel>, IComparer<Operazione>
        {
            private readonly Dictionary<LavorazioniEnumOperazioni, int> ordList;

            public OperationComparer()
            {
                var index = 0;
                ordList = new Dictionary<LavorazioniEnumOperazioni, int>();
                ordList.Add(LavorazioniEnumOperazioni.FresaturaSpianaturaSgrossatura, index++);
                ordList.Add(LavorazioniEnumOperazioni.FresaturaSpianaturaFinitura, index++);
                ordList.Add(LavorazioniEnumOperazioni.Sgrossatura, index++);
                ordList.Add(LavorazioniEnumOperazioni.Finitura, index++);
                ordList.Add(LavorazioniEnumOperazioni.Smussatura, index++);

                ordList.Add(LavorazioniEnumOperazioni.ForaturaCentrino, index++);
                ordList.Add(LavorazioniEnumOperazioni.ForaturaPunta, index++);
                ordList.Add(LavorazioniEnumOperazioni.ForaturaMaschiaturaDx, index++);
                ordList.Add(LavorazioniEnumOperazioni.ForaturaLamatore, index++);
                ordList.Add(LavorazioniEnumOperazioni.ForaturaBareno, index++);
                ordList.Add(LavorazioniEnumOperazioni.ForaturaAlesatore, index++);
                ordList.Add(LavorazioniEnumOperazioni.ForaturaSmusso, index++);
                ordList.Add(LavorazioniEnumOperazioni.FresaturaFilettare, index++);

            }

            public int Compare(OperationMainScreenViewModel x, OperationMainScreenViewModel y)
            {
                int indice1;
                var rslt1 = ordList.TryGetValue(x.OperationType, out indice1);

                int indice2;
                var rslt2 = ordList.TryGetValue(y.OperationType, out indice2);

                if (!rslt1 || !rslt2) // se arriva qui significa che non ho inserito enum
                    return 0;

                if (indice1 == indice2)
                    return 0;

                if (indice1 > indice2)
                    return 1;
                return -1;
            }

            public int Compare(Operazione x, Operazione y)
            {
                int indice1;
                var rslt1 = ordList.TryGetValue(x.OperationType, out indice1);

                int indice2;
                var rslt2 = ordList.TryGetValue(y.OperationType, out indice2);

                if (!rslt1 || !rslt2) // se arriva qui significa che non ho inserito enum
                    return 0;

                if (indice1 == indice2)
                    return 0;

                if (indice1 > indice2)
                    return 1;
                return -1;
            }
        }


        public ICommand SortOperationCmd
        {
            get
            {
                return _sortOperationCmd ?? (_sortOperationCmd = new RelayCommand(param => SortOperation(),
                                                                                  param => true));
            }
        }

        /// <summary>
        /// Metodo richiamato quando vado su treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTreeViewItemSelected(object sender, EventArgs e)
        {

            // se sender è lavorazione
            var lav = sender as LavorazioneTreeView;

            if (lav != null)
            {
                // Setto la fase selezionata come parent della lavorazione scelta
                FaseSelectedViewModel = (FaseLavoroTreeView)lav.Parent;
            }

            // se sender e treeview
            var fase = sender as FaseLavoroTreeView;
            if (fase != null)
                FaseSelectedViewModel = fase;

            UpdatePreview();
        }
        /// <summary>
        /// Aggiorno albero di visualizzazione
        /// </summary>
        /// <param name="oldTreeViews"></param>
        /// <param name="collectionSource"></param>
        /// <returns></returns>
        private static IEnumerable<FaseLavoroTreeView> GetTreeViewUpdated(IEnumerable<FaseLavoroTreeView> oldTreeViews, IEnumerable<FaseDiLavoro> collectionSource)
        {
            /*
             * Per aggiornare il treeView ,
             *  - ricreo il treeView
             *  - guardo se i nuovi oggetti erano già presenti nel vecchio viewModel
             *  - se si prendo i loro valori di expandede e selected 
             */

            foreach (var faseDiLavoro in collectionSource)
            {
                var fase = faseDiLavoro.FaseDiLavoroGuid;

                var faseTreeViewModel = new FaseLavoroTreeView(faseDiLavoro.FaseDiLavoroGuid, null);

                // prendo vecchio view model della stessa fase
                var oldPhaseViewModel = oldTreeViews.FirstOrDefault(l => l.FaseDiLavoroGuid == fase);

                // se esiste , prendo i suoi valori di expanded e selected..
                if (oldPhaseViewModel != null)
                {
                    faseTreeViewModel.IsExpanded = oldPhaseViewModel.IsExpanded;
                    faseTreeViewModel.IsSelected = oldPhaseViewModel.IsSelected;
                }
                var lav = Singleton.Instance.GetLavorazioni(fase);

                foreach (var lavorazione in lav)
                {
                    var lavorazioneViewModel = new LavorazioneTreeView(lavorazione, faseTreeViewModel);

                    if (oldPhaseViewModel != null)
                    {
                        // vecchi viewModel delle lavorazioni
                        var oldWorkViewModels = oldPhaseViewModel.Children.Cast<LavorazioneTreeView>();

                        if (oldWorkViewModels != null)
                        {
                            var lavorazione1 = lavorazione;
                            var oldWVm = oldWorkViewModels.FirstOrDefault(l => l.Lavorazione.Equals(lavorazione1));

                            if (oldWVm != null)
                            {
                                lavorazioneViewModel.IsExpanded = oldWVm.IsExpanded;
                                lavorazioneViewModel.IsSelected = oldWVm.IsSelected;
                            }
                        }
                    }

                    faseTreeViewModel.Children.Add(lavorazioneViewModel);

                }

                yield return faseTreeViewModel;

            }
        }

        public string ProgText
        {
            get
            {
                if (FaseSelectedViewModel == null) return string.Empty;

                var selectedPhase = FaseSelectedViewModel.FaseDiLavoro.FaseDiLavoroGuid;

                var s = Singleton.Instance.GetFaseDiLavoro(selectedPhase);

                return s.ProgrammaNc;
            }

            set
            {
                if (FaseSelectedViewModel == null) return;

                var selectedPhase = FaseSelectedViewModel.FaseDiLavoro.FaseDiLavoroGuid;

                var s = Singleton.Instance.GetFaseDiLavoro(selectedPhase);

                s.ProgrammaNc = value;

                OnPropertyChanged("ProgText");
            }

        }

        public void UpdatePreview()
        {
            if (FaseSelectedViewModel != null)
            {
                var preview = new List<IEntity3D>();

                var fase = FaseSelectedViewModel.FaseDiLavoro;

                var lavs = Singleton.Instance.GetLavorazioni(fase.FaseDiLavoroGuid);

                Lavorazione lavSelected = null;

                var selectedWorkVm = FaseSelectedViewModel.Children.Where(l => l.IsSelected).Cast<LavorazioneTreeView>().FirstOrDefault();

                if (selectedWorkVm != null)
                {
                    lavSelected = selectedWorkVm.Lavorazione;
                }

                foreach (var lavorazione in lavs)
                {
                    var temp = lavorazione.GetPreview();

                    if (temp == null) return;

                    var isSelected = (lavSelected != null && lavSelected.Equals(lavorazione));

                    foreach (var entity2D in temp)
                    {
                        if (entity2D.PlotStyle == EnumPlotStyle.SelectedElement || entity2D.PlotStyle == EnumPlotStyle.Element)
                            entity2D.PlotStyle = isSelected ? EnumPlotStyle.SelectedElement : EnumPlotStyle.Element;

                        preview.Add(entity2D);
                    }
                }

                Preview = new ObservableCollection<IEntity3D>(preview);
                return;

            }
            Preview = new ObservableCollection<IEntity3D>();

        }
        //private readonly List<FaseDiLavoro> _source = new List<FaseDiLavoro>();

        private ObservableCollection<IEntity3D> _preview;
        public ObservableCollection<IEntity3D> Preview
        {
            get
            {
                return _preview;
            }

            set
            {
                _preview = value;
                OnPropertyChanged("Preview");
            }
        }



        private FaseLavoroTreeView _faseSelectedViewModel;
        public FaseLavoroTreeView FaseSelectedViewModel
        {
            get { return _faseSelectedViewModel; }

            set
            {
                if (_faseSelectedViewModel == value) return;
                _faseSelectedViewModel = value;
                UpdateOperationsList();
                OnPropertyChanged("FaseSelectedViewModel");
                OnPropertyChanged("ProgText");
            }
        }

        private double _waitScreenCurrent;
        public double WaitScreenCurrent
        {
            get
            {
                return _waitScreenCurrent;
            }
            set
            {
                _waitScreenCurrent = value;
                OnPropertyChanged("WaitScreenCurrent");
            }
        }

        #region Generate Code

        RelayCommand _generateCode;

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        private void H()
        {
            WaitScreenCurrent = 0;

            IsBusy = true;

            var worker = new BackgroundWorker();


            worker.DoWork += GenerateCode;

            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(WorkerProgressChanged);


            worker.RunWorkerCompleted += (sender, e) =>
            {
                /* [ IsBusy = false ] lo richiamo su UserControl.Loaded */
                IsBusy = false;
            };
            worker.RunWorkerAsync();
        }

        void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WaitScreenCurrent = e.ProgressPercentage;
        }

        private void GenerateCode(object sender, DoWorkEventArgs e)
        {
            var bw = (BackgroundWorker)sender;

            if (FaseSelectedViewModel == null) return;

            //_model.UpdateSecureDistance(_preference.SecureDistanceMm, _preference.SecureDistanceInch);

            //_model.UpdateSecureFeed(_preference.RapidSecureFeedMm, _preference.RapidSecureFeedInch);

            var selectedPhase = FaseSelectedViewModel.FaseDiLavoro;

            if (!selectedPhase.IsValid)
            {
                MessageBox.Show("Correct Error !", "Correct Error", MessageBoxButton.OK, MessageBoxImage.Warning,
                                MessageBoxResult.OK);
                return;
            }

            var machine = selectedPhase.GetMacchina();

            if (machine == null)
                throw new NullReferenceException();

            var uniProgram = new MachineProgram(Singleton.Instance.MeasureUnit);

            uniProgram.ProgramNumber = selectedPhase.ProgramNumber;
            uniProgram.ProgramComment = selectedPhase.Descrizione;

            uniProgram.NoChangeToolSecureZ = selectedPhase.NoChangeToolSecureZ;

            var operation = Singleton.Instance.GetOperationList(selectedPhase.FaseDiLavoroGuid);

            UpdateChangeToolOptional(operation);

            SetChangeTool(operation);

            double count = 0;
            var opCount = operation.Count();

            var stock = selectedPhase.Stock;

            uniProgram.CutViewerStockSettingStr = CutViewerHelper.PrintStockBlock(stock.Larghezza, stock.Altezza,
                                                                                  stock.Spessore, stock.OriginX + stock.Larghezza / 2,
                                                                                  stock.OriginY + stock.Altezza / 2, stock.Spessore + stock.OriginZ);
            foreach (var operazione in operation)
            {
                var programPhase = operazione.GetProgramPhase(machine, true);

                programPhase.DisimpegnoCorto = operazione.OutputDisimpegnoCorto;

                programPhase.SetCambioUtensile(operazione.OutputCambioUtensile);

                uniProgram.Operations.Add(programPhase);

                count++;
                var perc = (int)((count / opCount) * 100);
                bw.ReportProgress(perc);
            }

            var code = machine.ProcessProgram(uniProgram);

            // ProgText = string.Empty;

            ProgText = code.Replace(",", ".").Replace("#", ",");

        }

        /// <summary>
        /// Setta scrittura cambio utensile e relativo disimpegno
        /// </summary>
        /// <param name="operaziones"></param>
        private static void SetChangeTool(IEnumerable<Operazione> operaziones)
        {

            var opCount = operaziones.Count();

            for (int i = 0; i < opCount; i++)
            {
                var op = operaziones.ElementAt(i);

                if (i == opCount - 1)
                {
                    op.OutputDisimpegnoCorto = false;
                    break;
                }

                var opNext = operaziones.ElementAt(i + 1);

                if (opNext.OutputCambioUtensile)
                {
                    op.OutputDisimpegnoCorto = false;
                }
                else
                    op.OutputDisimpegnoCorto = true;

            }
        }
        public ICommand GenerateCodeCmd
        {
            get
            {
                return _generateCode ?? (_generateCode = new RelayCommand(param => H(),
                                                                                    param => true));
            }
        }
        #endregion

        #region Add Phase

        RelayCommand _addPhase;

        private Guid AddPhase(ToolMachine machine)
        {
            var fase = Singleton.Instance.CreateFaseDiLavoro(machine);

            var prefernce = Singleton.Preference.GetPreference(Singleton.Instance.MeasureUnit);

            fase.NoChangeToolSecureZ = prefernce.MillingSecureZNoChangeTool;

            Singleton.Instance.AddFaseDiLavoro(fase);

            UpdateTreeView();

            UpdateOperationsList();

            UpdatePreview();

            return fase.FaseDiLavoroGuid;
        }

        public ICommand AddPhaseCmd
        {
            get
            {
                return _addPhase ?? (_addPhase = new RelayCommand(param => AddPhase(param as ToolMachine),
                                                                           param => true));
            }
        }


        #endregion

        #region Save

        RelayCommand _saveCmd;

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                SaveFileDlg();
                return;
            }
            FileUtility.SerializeToFile(FilePath, Singleton.Instance);
            MessageBox.Show("File Saved!", "File Saved", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None, MessageBoxOptions.None);
        }

        public ICommand SaveCmd
        {
            get
            {
                return _saveCmd ?? (_saveCmd = new RelayCommand(param => Save(),
                                                                param => true));
            }
        }

        #endregion

        #region Save As File Dialog

        RelayCommand _saveFileDlg;

        private void SaveFileDlg()
        {
            var previousFilePath = FilePath;
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    DefaultExt = ".csp",
                    AddExtension = true,
                    Filter = "Cnc Simple | *.csp"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

                FilePath = saveFileDialog.FileName;

                FileUtility.SerializeToFile(FilePath, Singleton.Instance);
            }
            catch (Exception)
            {
                MessageBox.Show("Errore Salvataggio");

                FilePath = previousFilePath;

            }

        }

        public ICommand SaveFileDialogCmd
        {
            get
            {
                return _saveFileDlg ?? (_saveFileDlg = new RelayCommand(param => SaveFileDlg(),
                                                                                param => true));
            }
        }

        #endregion

        public string FilePath
        {
            get { return _model.CurrentFilePath; }
            set
            {
                _model.CurrentFilePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        #region Update Cycle Time Code


        public string Note
        {
            get { return Singleton.Instance.Note; }

            set
            {
                Singleton.Instance.Note = value;
                OnPropertyChanged("Note");
            }
        }


        #endregion

        #region Edit Phase Detail

        RelayCommand _editPhaseDetail;

        private void EditPhaseDetail(FaseLavoroTreeView item)
        {
            var fase = item.FaseDiLavoro;

            var deepCopy = FileUtility.DeepCopy(fase);

            var viewModelBase = GetViewModel(deepCopy);

            var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>(Constants.PhaseDetailEditModalDialog);

            _modalDialogService.ShowDialog(dialog, viewModelBase,
                                               returnedViewModelInstance =>
                                               {
                                                   if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                                                   {
                                                       var source = viewModelBase.FaseDiLavoro;

                                                       Singleton.Instance.AddFaseDiLavoro(source);
                                                   }
                                               });

        }

        private FaseDiLavoroViewModel GetViewModel(FaseDiLavoro fase)
        {
            //if (fase is FaseTornio)
            //    return new FresaturaProfiloLiberoEditViewModel(lavorazione as FresaturaContornatura);

            //if (fase is FaseCentroDiLavoro)
            //    return new SpianaturaViewModel(lavorazione as Spianatura);

            //if (fase is FaseCentroDiLavoro)
            //    return new 

            return new FaseDiLavoroViewModel(fase);
        }

        public ICommand EditPhaseDetailCmd
        {
            get
            {
                return _editPhaseDetail ?? (_editPhaseDetail = new RelayCommand(param => EditPhaseDetail(param as FaseLavoroTreeView),
                                                              param => true));
            }
        }

        #endregion

        #region Edit Existing Work

        RelayCommand _editWork;

        private void EditWork(LavorazioneTreeView item)
        {
            var lav = item.Lavorazione;

            var deepCopy = FileUtility.DeepCopy(lav);

            // todo : smistare varie lavorazione

            var viewModel = GetViewModel(deepCopy);

            ShowEditWorkDialog(viewModel);

        }

        private static EditWorkViewModel GetViewModel(Lavorazione lavorazione)
        {
            if (lavorazione is FresaturaContornatura)
                return new FresaturaContornaturaViewModel(lavorazione as FresaturaContornatura);

            if (lavorazione is Spianatura)
                return new SpianaturaViewModel(lavorazione as Spianatura);

            if (lavorazione is FresaturaCava)
                return new FresaturaCavaViewModel(lavorazione as FresaturaCava);

            if (lavorazione is FresaturaScanalaturaChiusa)
                return new ScanalaturaChiusaViewModel(lavorazione as FresaturaScanalaturaChiusa);

            if (lavorazione is FresaturaFilettatura)
                return new FresaturaFilettaturaViewModel(lavorazione as FresaturaFilettatura);

            if (lavorazione is TextEngravingModel)
                return new TextEngravingViewModel(lavorazione as TextEngravingModel);

            if (lavorazione is DrillBaseClass)
                return new ForaturaCommonViewModel(lavorazione as DrillBaseClass);

            if (lavorazione is ScanalaturaLinea)
                return new ScanalaturaLineaViewModel(lavorazione as ScanalaturaLinea);

            if (lavorazione is FresaturaLato)
                return new FresaturaLatoViewModel(lavorazione as FresaturaLato);

            if (lavorazione is TornituraScanalatura)
                return new TornituraScanalaturaViewModel(lavorazione as TornituraScanalatura);

            if (lavorazione is Tornitura)
                return new TornituraProfiloLiberoViewModel(lavorazione as Tornitura);

            if (lavorazione is TornituraSfacciatura)
                return new TornituraSfacciaturaViewModel(lavorazione as TornituraSfacciatura);

            if (lavorazione is TornituraFilettatura)
                return new TornituraFilettaturaViewModel(lavorazione as TornituraFilettatura);

            throw new NotImplementedException("MainViewModel.GetViewModel");
        }

        public ICommand EditWorkCmd
        {
            get
            {
                return _editWork ?? (_editWork = new RelayCommand(param => EditWork(param as LavorazioneTreeView),
                                                                  param => true));
            }
        }

        #endregion

        #region Del TreeViewItem

        RelayCommand _delTreeViewItem;

        private void DelTreeViewItem(TreeViewItemViewModel item)
        {
            var msgBoxRslt = MessageBox.Show("Delete Item ?", "Delete Item", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (msgBoxRslt != MessageBoxResult.Yes)
                return;

            if (item is LavorazioneTreeView)
            {
                var lavorazione = ((LavorazioneTreeView)item).Lavorazione;

                var fasi = Singleton.Instance.GetFasiDiLavoro();
                foreach (var faseDiLavoro in fasi)
                {
                    if (Singleton.Instance.RemoveLavorazione(lavorazione))
                    {
                        faseDiLavoro.UpdateValidFlag();
                        break;
                    }

                }

            }

            else if (item is FaseLavoroTreeView)
            {
                var faseDiLavoro = ((FaseLavoroTreeView)item).FaseDiLavoro;

                Singleton.Instance.RemoveFaseLavoro(faseDiLavoro);

                //if (_model.FasiDiLavoro.Contains(faseDiLavoro))
                //    _model.FasiDiLavoro.Remove(faseDiLavoro);
            }

            UpdateTreeView();

        }

        public ICommand DelWorkCmd
        {
            get
            {
                return _delTreeViewItem ?? (_delTreeViewItem = new RelayCommand(param => DelTreeViewItem(param as TreeViewItemViewModel),
                                                                    param => true));
            }
        }

        #endregion

        #region New Work Dialog

        RelayCommand _newWorkCmd;

        private void ShowEditWorkDialog(EditWorkViewModel viewModelBase)
        {
            var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>(Constants.EditUserModalDialog);

            _modalDialogService.ShowDialog(dialog, viewModelBase,
                                               returnedViewModelInstance =>
                                               {
                                                   if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                                                   {
                                                       var lavorazioneEdited = viewModelBase.Lavorazione;

                                                       Singleton.Instance.AddLavorazione(lavorazioneEdited);

                                                       lavorazioneEdited.FaseDiLavoro.UpdateValidFlag();

                                                       // questo non serve più ..

                                                       //              /*
                                                       //               * 
                                                       //               * innanzitutto devo prendere la fase di lavoro originale ,
                                                       //               * non la referenza del model
                                                       //               */


                                                       //              /*
                                                       //               * passo una deep copy da modificare
                                                       //               * se lavorazione è già presente 
                                                       //               * ( guardare se basta confrontare o serve guid)
                                                       //               * la sovrascrive con quella modificata 
                                                       //               */

                                                       //              foreach (var faseDiLavoro in _model.FasiDiLavoro)
                                                       //              {
                                                       //                  for (var i = 0; i < faseDiLavoro.Lavorazioni.Count; i++)
                                                       //                  {
                                                       //                      var lav = faseDiLavoro.Lavorazioni[i];

                                                       //                      if (lav.LavorazioneGuid == lavorazioneEdited.LavorazioneGuid)
                                                       //                      {
                                                       //                          // faseDiLavoro.Lavorazioni[i] = lavorazioneEdited;
                                                       //                          faseDiLavoro.AddLavorazione(lavorazioneEdited);
                                                       //                          faseDiLavoro.SetModel(_model);
                                                       //                          faseDiLavoro.UpdateValidFlag();

                                                       //                          UpdateTreeView();

                                                       //                          return;
                                                       //                      }
                                                       //                  }
                                                       //              }

                                                       //              // se arrivo qui vuol dire che lavorazione non è stata ancora inserita
                                                       //              // la inserisco ora.

                                                       //              var faseOriginale =
                                                       //TreeView.Where(
                                                       //    l =>
                                                       //    l.FaseDiLavoro.FaseDiLavoroGuid ==
                                                       //    lavorazioneEdited.FaseDiLavoro.FaseDiLavoroGuid).FirstOrDefault().FaseDiLavoro;

                                                       //              faseOriginale.Lavorazioni.Add(lavorazioneEdited);

                                                       //              faseOriginale.UpdateValidFlag();

                                                       UpdateTreeView();

                                                       UpdatePreview();

                                                   }
                                               });
        }

        private void NewWorkDialog(EnumWork enumWork)
        {
            Lavorazione lavorazione = null;

            switch (enumWork)
            {
                // Fresatura
                case EnumWork.FresaturaContornatura:
                    {
                        lavorazione = new FresaturaContornatura();
                    } break;

                case EnumWork.FresaturaCava:
                    {
                        lavorazione = new FresaturaCava();
                    } break;


                case EnumWork.FresaturaLato:
                    {
                        lavorazione = new FresaturaLato();
                    } break;

                case EnumWork.FresaturaSpianatura:
                    {
                        lavorazione = new Spianatura();
                    } break;

                case EnumWork.FresaturaScanalaturaLinea:
                    {
                        lavorazione = new ScanalaturaLinea();
                    } break;

                case EnumWork.FresaturaScanalaturaChiusa:
                    {
                        lavorazione = new FresaturaScanalaturaChiusa();
                    } break;

                case EnumWork.FresaturaFilettare:
                    {
                        lavorazione = new FresaturaFilettatura();
                    } break;

                case EnumWork.TextEngraving:
                    {
                        lavorazione = new TextEngravingModel();
                    } break;

                // Cicli Foratura 

                case EnumWork.ForaturaSemplice:
                    {
                        lavorazione = new ForaturaSemplice(false);
                    } break;

                case EnumWork.Alesatura:
                    {
                        lavorazione = new Alesatura(false);
                    } break;

                case EnumWork.Barenatura:
                    {
                        lavorazione = new Barenatura(false);
                    } break;

                case EnumWork.Lamatura:
                    {
                        lavorazione = new Lamatura(false);
                    } break;

                case EnumWork.Maschiatura:
                    {
                        lavorazione = new Maschiatura(false);
                    } break;


                // Tornitura 
                case EnumWork.TornituraScanalaturaEsterna:
                    {
                        lavorazione = new TornituraScanalatura(TornituraScanalatura.TipoScanalatura.Esterna);
                    } break;

                case EnumWork.TornituraScanalaturaInterna:
                    {
                        lavorazione = new TornituraScanalatura(TornituraScanalatura.TipoScanalatura.Interna);

                    } break;

                case EnumWork.TornituraScanalaturaFrontale:
                    {
                        lavorazione = new TornituraScanalatura(TornituraScanalatura.TipoScanalatura.Frontale);

                    } break;
                case EnumWork.TornituraInterna:
                    {
                        lavorazione = new Tornitura(Tornitura.TipoTornitura.Interna);
                    } break;

                case EnumWork.TornituraEsterna:
                    {
                        lavorazione = new Tornitura(Tornitura.TipoTornitura.Esterna);
                    } break;

                case EnumWork.TornituraSfacciatura:
                    {
                        lavorazione = new TornituraSfacciatura();
                    } break;

                case EnumWork.TornituraFilettatura:
                    {
                        lavorazione = new TornituraFilettatura();
                    } break;

                case EnumWork.TornioForaturaCentraleAlesatura:
                case EnumWork.TornioForaturaCentraleMaschiatura:
                case EnumWork.TornioForaturaCentraleLamatura:
                case EnumWork.TornioForaturaCentraleSemplice:
                    {
                        lavorazione = new ForaturaSemplice(true);
                    } break;

                default:
                    {
                        throw new NotImplementedException("MainViewModel.NewWorkDialog");
                    }
                    return;

            }

            var faseParentGuid = Guid.Empty;

            /*
             * Per determinare fase di lavoro , prima provo quella selezionata.
             * Poi prova ultima in ordine di inserimento 
             * Se non ho trovato niente ne creo una nuova
             */
            if (FaseSelectedViewModel != null && FaseSelectedViewModel.FaseDiLavoro.IsCompatible(lavorazione))
            {
                faseParentGuid = FaseSelectedViewModel.FaseDiLavoro.FaseDiLavoroGuid;
            }
            else if (TreeView.LastOrDefault() != null && TreeView.LastOrDefault().FaseDiLavoro.IsCompatible(lavorazione))
            {
                faseParentGuid = TreeView.LastOrDefault().FaseDiLavoroGuid;
            }

            if (faseParentGuid == Guid.Empty)
            {
                /* Se la fase non era compatibile ne devo creare una compatibile.*/

                var machineCompatible = GetCompatibleMachine(lavorazione, true);

                if (machineCompatible != null)
                {
                    faseParentGuid = AddPhase(machineCompatible);

                }

            }

            if (faseParentGuid == Guid.Empty)
            {
                /*
                 * se arriva qui significa che c'è stato problema. 
                 */
                MessageBox.Show("Tool Machine not defined");

                return;
            }

            var fase = Singleton.Instance.GetFaseDiLavoro(faseParentGuid);

            fase.AddLavorazione(lavorazione);

            var viewModel = GetViewModel(lavorazione);

            ShowEditWorkDialog(viewModel);

        }

        public ICommand NewWorkCmd
        {
            get
            {
                return _newWorkCmd ?? (_newWorkCmd = new RelayCommand(param => NewWorkDialog((EnumWork)param),
                                                                          param => true));
            }
        }
        private static ToolMachine GetCompatibleMachine(Lavorazione lavorazione, bool createNewMachine = false)
        {
            var machines = PathFolderHelper.GetToolMachines();

            var tipoFaseLavoro = lavorazione.FasiCompatibili.FirstOrDefault();

            IEnumerable<ToolMachine> ms = null;
            switch (tipoFaseLavoro)
            {
                case FaseDiLavoro.TipoFaseLavoro.Tornio3Assi:
                    ms = machines.OfType<LatheAxisC>();
                    break;

                case FaseDiLavoro.TipoFaseLavoro.Tornio2Assi:
                    ms = machines.OfType<HorizontalLathe2Axis>();
                    break;

                case FaseDiLavoro.TipoFaseLavoro.Centro:
                    ms = machines.OfType<VerticalMill>();
                    break;

            }

            if (ms != null && ms.Count() > 0)
                return ms.First();

            if (!createNewMachine) return null;

            ToolMachine machine = null;

            /*Non è stata trovata nessuna macchina compatibile,
             Ne creo una standard */
            switch (tipoFaseLavoro)
            {
                case FaseDiLavoro.TipoFaseLavoro.Tornio3Assi:
                    machine = new LatheAxisC();
                    break;

                case FaseDiLavoro.TipoFaseLavoro.Tornio2Assi:
                    machine = new HorizontalLathe2Axis();
                    break;

                case FaseDiLavoro.TipoFaseLavoro.Centro:
                    machine = new VerticalMill();
                    break;
            }

            machines.Add(machine);

            Singleton.Data.AddMacchina(machine);

            return machine;
        }

        #endregion
    }
}
