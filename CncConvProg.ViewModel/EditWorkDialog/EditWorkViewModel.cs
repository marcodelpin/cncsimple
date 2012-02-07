using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.ViewModel.AuxViewModel.TreeViewModel;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.OperationViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Implementors.Wpf.MVVM;
using System.Linq;

namespace CncConvProg.ViewModel.EditWorkDialog
{
    public abstract class EditWorkViewModel : ViewModelBase
    {
        private ObservableCollection<EditStageTreeViewItem> _treeView = new ObservableCollection<EditStageTreeViewItem>();
        public ObservableCollection<EditStageTreeViewItem> TreeView
        {
            get { return _treeView; }

            set
            {
                _treeView = value;
                OnPropertyChanged("TreeView");
            }
        }

        // public EditStageTreeViewItem StageOperazioni;

        public void SyncronizeOperation()
        {
            //if (StageOperazioni.Children.Count > 0)
            //    StageOperazioni.Children.Clear();

            foreach (var operazione in Lavorazione.Operazioni/*.Where(op => op.Abilitata)*/)
            {
                var viewModel = new OperazioneViewModel(operazione, this);

                TreeView.Add(viewModel);
            }
        }

        //private EditStageTreeViewItem GetToolViewModel(Operazione operazione)
        //{
        //    if (operazione is OperazioneFresaCandela)
        //    {
        //        return new ParametriFresaCandelaViewModel(operazione as OperazioneFresaCandela, StageOperazioni);
        //    }

        //    if (operazione is OperazioneScanalatore)
        //    {
        //        return new ParametriScanalatoreViewModel(operazione as OperazioneScanalatore, StageOperazioni);
        //    }

        //    if (operazione is OperazioneUtensileTornitura)
        //    {
        //        return new ParametroUtensileTornioViewModel(operazione as OperazioneUtensileTornitura, StageOperazioni);
        //    }

        //    if (operazione is OperazioneForatura)
        //    {
        //        return new ParametroPuntaViewModel(operazione as OperazioneForatura, StageOperazioni);
        //    }


        //    /*
        //     * in base alla tipologia di utensile ritorno diverso viewModel
        //     * 
        //     */
        //    return new OperazioneViewModel(operazione, StageOperazioni);

        //}

        public Lavorazione Lavorazione { get; private set; }

        protected EditWorkViewModel(Lavorazione lavorazione)
        {
            Lavorazione = lavorazione;
        }

        protected void Initialize()
        {
            SyncronizeOperation();

            foreach (var editStageTreeViewItem in TreeView)
            {
                editStageTreeViewItem.OnSourceUpdated += EditStageTreeViewItemOnSourceUpdated;
                editStageTreeViewItem.ExpandChild();
            }

            var first = TreeView.FirstOrDefault();
            if (first != null)
            {
                SelectedScreen = first;
                first.IsSelected = true;
            }
        }


        protected void EditStageTreeViewItemOnSourceUpdated(object sender, EventArgs e)
        {
            // UpdatePreview(); // aggiornare solo la preview interessata

            // Lavorazione.SetProgramDirty(); // é s

            OnPropertyChanged("IsValid");
        }


        private object _selectedScreen;
        public object SelectedScreen
        {
            get
            {
                return _selectedScreen;
            }
            set
            {
                // hack per aggiornare il profilo..
                //if (SelectedScreen is ISync)
                //{
                //    Lavorazione.SetProfile(((ISync)SelectedScreen).GetProfile());
                //    //((ISync)SelectedScreen).SyncronizeProfile();
                //}

                _selectedScreen = value;
                UpdatePreview();

                OnPropertyChanged("SelectedScreen");
            }
        }

        public bool IsValid
        {
            get
            {
                var rslt = true;

                foreach (var editStageTreeViewItem in TreeView)
                {
                    var rootValid = editStageTreeViewItem.IsTreeViewValid();

                    if (rootValid) continue;

                    rslt = false;
                    break;
                }

                // Setto flag su model della lavorazione 

                Lavorazione.IsValid = rslt;

                // !! sempre per la solita storia delle referenze devo chiamare questo metodo una volta 
                // settato con la fase di lavoro giusta, una volta chiusa la finestra di dialogo e 
                // In questo modo viene richiamato solo una volta 
                //  Lavorazione.FaseDiLavoro.UpdateValidFlag(); /\

                return Lavorazione.IsValid;
            }
        }

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

        #region Update Preview

        RelayCommand _updatePreview;

        /// <summary>
        /// Metodo chiamato per aggiornare la schermata per la lavorazione corrente.
        /// Per evitare che in certe situazioni diventi troppo pesante riaggiorno operazione solamente quando c'è la pressione del tasto
        /// UpdatePreview.. Quando cambia screen stampa il percorso calcolato precedentemente.
        /// </summary>
        public virtual void UpdatePreview(bool updateOperation = false)
        {
            try
            {
                if (IsCalculating) return;

                if (SelectedScreen is IPreviewable)
                {
                    if (SelectedScreen is OperazioneViewModel)
                    {
                        var ovm = SelectedScreen as OperazioneViewModel;
                        if (updateOperation)
                            ovm.UpdateProgram();
                    }
                    //if (SelectedScreen is InputProfileViewModel.ProfileEditorViewModel)
                    //{
                    // con pprofile editor e mt si crea problemi

                    var screenPreviable = SelectedScreen as IPreviewable;

                    var preview = screenPreviable.GetPreview();

                    if (preview != null)
                        Preview = new ObservableCollection<IEntity3D>(preview);
                    else
                        Preview = null;

                    //}
                    //else
                    //{
                    //    IsCalculating = true;

                    //    var bw = new BackgroundWorker();


                    //    bw.DoWork += new DoWorkEventHandler(GetPreview);

                    //    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

                    //    bw.RunWorkerAsync();
                    //}

                }
                else
                {
                    Preview = new ObservableCollection<IEntity3D>(Lavorazione.GetPreview());
                }
            }
            catch (Exception exception)
            {
                //  throw;
            }

        }
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            var rslt = e.Result as IEnumerable<IEntity3D>;

            if (rslt != null)
            {

                Preview = new ObservableCollection<IEntity3D>(rslt);

            }

            else
            {

                Preview = null;

            }



            IsCalculating = false;



        }





        #region gePreview mt



        private void GetPreview(object sender, DoWorkEventArgs e)
        {

            /*

             * se si comparta in modo strano fare una copia del rslt e lasciare al dispatcher il compito di assegnare la preview.

             */

            //var bw = sender as BackgroundWorker;
            var screenPreviable = SelectedScreen as IPreviewable;

            if (screenPreviable != null)

                lock (_lockObject)
                {

                    e.Result = screenPreviable.GetPreview();
                }

            return;
        }

        private readonly object _lockObject = new object();



        private bool _isCalculating;

        public bool IsCalculating
        {

            get { return _isCalculating; }

            set
            {

                _isCalculating = value;

                OnPropertyChanged("IsCalculating");

            }

        }



        #endregion




        public ICommand UpdatePreviewCmd
        {
            get
            {
                return _updatePreview ?? (_updatePreview = new RelayCommand(param => UpdatePreview(true),
                                                                            param => !IsCalculating));
            }
        }

        #endregion


        #region NextScreen

        RelayCommand _nextScreen;

        private void NextScreen()
        {
            // hack per aggiornare il profilo..
            if (SelectedScreen is ISync)
                ((ISync)SelectedScreen).GetProfile();

            if (SelectedScreen == null) return;

            var selectedScreen = SelectedScreen as TreeViewItemViewModel;

            var currentRoot = selectedScreen.GetRoot();

            var nextOrDefault = currentRoot.GetNextOrDefault(selectedScreen);

            if (nextOrDefault != null)
            {
                SelectScreen(nextOrDefault);
                return;
            }
            var rootIndex = TreeView.IndexOf((EditStageTreeViewItem)currentRoot);

            rootIndex++;

            var nextRoot = TreeView.ElementAtOrDefault(rootIndex);

            if (nextRoot != null)
            {
                //var firstOrDefault = nextRoot.Children.FirstOrDefault();

                //if (firstOrDefault != null)
                //    selectScreen(firstOrDefault);
                //else
                SelectScreen(nextRoot);

                return;
            }
        }

        private bool CanNextScreen
        {
            get
            {
                var lastRoot = TreeView.LastOrDefault();

                if (lastRoot != null)
                {
                    var lastElement = lastRoot.Children.LastOrDefault();

                    var last = lastElement ?? lastRoot;

                    return SelectedScreen != last;
                }

                return false;

            }

        }

        public ICommand NextScreenCmd
        {
            get
            {
                return _nextScreen ?? (_nextScreen = new RelayCommand(param => NextScreen(),
                                                                      param => CanNextScreen));
            }
        }

        #endregion

        #region Prev Screen

        private void SelectScreen(TreeViewItemViewModel treeViewItemViewModel)
        {
            foreach (var editStageTreeViewItem in TreeView)
            {
                foreach (var viewItemViewModel in editStageTreeViewItem.Children)
                {
                    //if (viewItemViewModel == treeViewItemViewModel)
                    viewItemViewModel.IsSelected = false;
                }
            }

            // espandere parent..
            treeViewItemViewModel.IsSelected = true;
            if (treeViewItemViewModel.Parent != null)
                treeViewItemViewModel.Parent.IsExpanded = true;

        }
        RelayCommand _prevScreen;

        private void PrevScreen()
        {
            // hack per aggiornare il profilo..
            if (SelectedScreen is ISync)
                ((ISync)SelectedScreen).GetProfile();

            if (SelectedScreen == null) return;

            var selectedScreen = SelectedScreen as TreeViewItemViewModel;

            var currentRoot = selectedScreen.GetRoot();

            var prev = currentRoot.GetPrevOrDefault(selectedScreen);

            if (prev != null)
            {
                SelectScreen(prev);
                return;
            }
            var rootIndex = TreeView.IndexOf((EditStageTreeViewItem)currentRoot);

            rootIndex--;

            var prevRoot = TreeView.ElementAtOrDefault(rootIndex);

            if (prevRoot != null)
            {
                var lastChild = prevRoot.Children.LastOrDefault();

                if (lastChild != null)
                    SelectScreen(lastChild);
                else
                    SelectScreen(prevRoot);

                return;
            }

        }

        private bool CanPrevScreen
        {
            get
            {
                var firstRoot = TreeView.FirstOrDefault();

                if (firstRoot != null)
                    return SelectedScreen != firstRoot;

                return false;
            }
        }

        public ICommand PrevScreenCmd
        {
            get
            {
                return _prevScreen ?? (_prevScreen = new RelayCommand(param => PrevScreen(),
                                                                      param => CanPrevScreen));
            }
        }

        #endregion

    }
}
