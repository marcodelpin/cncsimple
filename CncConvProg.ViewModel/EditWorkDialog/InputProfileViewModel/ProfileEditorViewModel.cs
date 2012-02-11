using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Abstractions.Wpf.Intefaces;
using Framework.Implementors.Wpf.MVVM;

// todo - trovare modo per aggiornare anteprima su modifica quota..
namespace CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel
{
    /*
     * Editor per profili 2D.
     * 
     * Prendo RawProfile in ingresso .
     * Faccio le mie modifiche .
     * Riaggiorno profilo in ingresso
     */

    /// <summary>
    /// 
    /// </summary>
    public sealed class ProfileEditorViewModel : EditStageTreeViewItem, IProfileEditorViewModel, ISync, IPreviewable, IHandleKeyable
    {

        public enum AxisSystem
        {
            Xy,
            Xz
        }

        public IEnumerable<RawEntity2D> Source
        {
            get
            {
                return MoveListViewModels.Select(rawEntityViewModel => rawEntityViewModel.RawEntity).ToList();
            }
        }

        public readonly AxisSystem CurrentAxisSystem;

        private readonly RawProfile _rawProfile;

        public ProfileEditorViewModel(RawProfile profile, EditStageTreeViewItem workViewModel, AxisSystem axisSystem)
            : base("Input Profile", workViewModel)
        {

            _rawProfile = profile;

            CurrentAxisSystem = axisSystem;

            DictionaryKey = CreateKeyboardShortcuts(CurrentAxisSystem);

            MoveListViewModels = new ObservableCollection<RawItemViewModel>();

            var moveList = profile.GetMoveList();

            foreach (var rawEntity2D in moveList)
            {
                //MoveListViewModels.Add(RawItemViewModel.GetViewModel(rawEntity2D, axisSystem));
                MoveListViewModels.Add(GetViewModel(rawEntity2D));

            }

            SelectedMoveViewModel = MoveListViewModels.LastOrDefault();

            UpdatePreview();

            UpdateMoveListOrientation();

        }

        public ProfileEditorViewModel(RawProfile profile, EditWorkViewModel workViewModel, AxisSystem axisSystem)
            : base("Input Profile", workViewModel)
        {

            _rawProfile = profile;

            CurrentAxisSystem = axisSystem;

            MoveListViewModels = new ObservableCollection<RawItemViewModel>();

            DictionaryKey = CreateKeyboardShortcuts(CurrentAxisSystem);
            var moveList = profile.GetMoveList();

            foreach (var rawEntity2D in moveList)
            {
                //MoveListViewModels.Add(RawItemViewModel.GetViewModel(rawEntity2D, axisSystem));
                MoveListViewModels.Add(GetViewModel(rawEntity2D));

            }

            SelectedMoveViewModel = MoveListViewModels.LastOrDefault();

            UpdatePreview();

            UpdateMoveListOrientation();

        }

        public Dictionary<Key, Key> DictionaryKey;

        private static Dictionary<Key, Key> CreateKeyboardShortcuts(AxisSystem axisSystem)
        {

            switch (axisSystem)
            {

                case AxisSystem.Xz:
                    {

                        var d = new Dictionary<Key, Key>();



                        /*

                         * sistema diametro - z 

                         */



                        d.Add(Key.Z, Key.X);

                        d.Add(Key.X, Key.Y);

                        d.Add(Key.U, Key.V);

                        d.Add(Key.W, Key.U);



                        return d;



                    } break;

                default:

                case AxisSystem.Xy:
                    {

                        return new Dictionary<Key, Key>();

                    }



            }

        }




        ///// <summary>
        ///// Obsolete
        ///// </summary>
        ///// <param name="moveList"></param>
        ///// <param name="workViewModel"></param>
        ///// <param name="axisSystem"></param>
        //public ProfileEditorViewModel(IEnumerable<RawEntity2D> moveList, EditWorkViewModel workViewModel, AxisSystem axisSystem)
        //    : base("Input Profilo", workViewModel)
        //{
        //    //_workViewModel = workViewModel;

        //    _axisSystem = axisSystem;

        //    MoveListViewModels = new ObservableCollection<RawEntityViewModel>();

        //    foreach (var rawEntity2D in moveList)
        //    {
        //        MoveListViewModels.Add(new RawEntityViewModel(rawEntity2D));
        //    }

        //    SelectedMoveViewModel = MoveListViewModels.LastOrDefault();

        //    UpdatePreview();

        //    UpdateMoveListOrientation();

        //}

        public List<RawEntity2D> GetProfile()
        {
            /*Richiamarlo ad ogni cambio è un po eccessivo ..*/
            // todo : richiamarlo anche su ok dialogo

            var rslt = MoveListViewModels.Select(rawEntityViewModel => rawEntityViewModel.RawEntity).ToList();

            return rslt;
        }
        /// <summary>
        /// Devo avere già la classe qui , in modo da avere gia le coordinate calcolate quando modifico
        /// le quote,
        /// </summary>
        private ObservableCollection<RawItemViewModel> _moveListViewModels;// = new ObservableCollection<IRawEntity2D>();
        public ObservableCollection<RawItemViewModel> MoveListViewModels
        {
            get
            { return _moveListViewModels; }
            set
            {
                _moveListViewModels = value;
                OnPropertyChanged("MoveListViewModels");
            }
        }

        private RawItemViewModel _selectedMoveViewModel;
        public RawItemViewModel SelectedMoveViewModel
        {
            get { return _selectedMoveViewModel; }
            set
            {
                if (value == null) return;
                _selectedMoveViewModel = value;

                foreach (var moveListViewModel in MoveListViewModels)
                {
                    if (moveListViewModel != _selectedMoveViewModel)
                        moveListViewModel.IsSelected = false;
                    else
                    {
                        moveListViewModel.IsSelected = true;
                    }

                }

                UpdatePreview();

                RequestUpdateSource();

                UpdateMoveListOrientation();


                //CurrentMovement = GetViewModel(_selectedMoveViewModel.RawEntity);

                OnPropertyChanged("SelectedMoveViewModel");
            }
        }

        public RawItemViewModel GetViewModel(RawEntity2D entity2D)
        {
            // todo , creare rispettivi viewModel
            if (entity2D is RawInitPoint2D)
            {
                var vm = new RawInitPointViewModel(entity2D as RawInitPoint2D, CurrentAxisSystem, this);
                vm.OnSourceUpdated += VmOnSourceUpdated;
                return vm;
            }

            if (entity2D is RawLine2D)
            {
                var vm = new RawLineViewModel(entity2D as RawLine2D, CurrentAxisSystem, this);
                vm.OnSourceUpdated += VmOnSourceUpdated;
                return vm;
            }

            if (entity2D is RawArc2D)
            {
                var vm = new RawArcViewModel(entity2D as RawArc2D, CurrentAxisSystem, this);
                vm.OnSourceUpdated += VmOnSourceUpdated;
                return vm;
            }

            throw new NotImplementedException("ProfileEditorViewModel.GetViewModel");
        }

        void VmOnSourceUpdated(object sender, EventArgs e)
        {
            RequestUpdateSource();

            UpdatePreview();

            UpdateMoveListOrientation();
        }


        public IEnumerable<IEntity3D> GetPreview()
        {

            try
            {
                _rawProfile.Syncronize(GetProfile());

                var prof2d = _rawProfile.GetProfileResult(true).Source;

                return Entity3DHelper.Get3DProfile(prof2d).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception("ProfileEditorViewModel.GetPreview");
            }

            return null;

        }


        private void UpdateMoveListOrientation()
        {
            foreach (var rawEntityViewModel in MoveListViewModels)
            {
                rawEntityViewModel.UpdateOrientation();
                rawEntityViewModel.UpdateIsUserInputedAndValue();
            }
        }

        //private ViewModelBase _currentMovement;
        //public ViewModelBase CurrentMovement
        //{
        //    get { return _currentMovement; }
        //    set
        //    {
        //        _currentMovement = value;

        //        UpdatePreview();

        //        RequestUpdateSource();

        //        OnPropertyChanged("CurrentMovement");
        //    }
        //}

        #region Add Entity

        RelayCommand _addEntity;

        private void AddEntity(string param)
        {
            int type;

            int.TryParse(param, out type);

            RawEntity2D newEntity = null;

            switch (type)
            {
                case 1:
                    {
                        newEntity = new RawLine2D(_rawProfile);
                    }
                    break;

                case 2:
                    {
                        newEntity = new RawArc2D(_rawProfile);

                    } break;

                default:
                    break;

            }

            // qui quando inserisco deve prendere coordinate oggetto precedente,
            // faccio u metodo semplice in questa classe...
            // nel profileSolver non mi serve sapere se quota è stata messa
            /* .
             * da mi serve solo nella view un aiuto visivo per calcolare quote 
             */

            InsertMove(newEntity);
        }

        private void InsertMove(RawEntity2D rawEntity2D)
        {
            /*
             * in pratica ora , creo il viewModel poi inserisco.
             * 
             * sarebbe più chiaro avere il model , inserirlo li poi riaggirnare parteViewModel
             */
            RawItemViewModel precMove = null;

            var entityToInsert = GetViewModel(rawEntity2D);
            if (SelectedMoveViewModel == null)
            {
                precMove = MoveListViewModels.LastOrDefault();
                MoveListViewModels.Add(entityToInsert);
            }
            else
            {
                precMove = MoveListViewModels.ElementAtOrDefault(MoveListViewModels.IndexOf(SelectedMoveViewModel));
                MoveListViewModels.Insert(MoveListViewModels.IndexOf(SelectedMoveViewModel) + 1, entityToInsert);
            }

            InitEntity(rawEntity2D, precMove.RawEntity);

            SelectedMoveViewModel = entityToInsert;

            RequestUpdateSource();
        }

        private static void InitEntity(RawEntity2D rawEntity2D, RawEntity2D precMove)
        {
            if (rawEntity2D is RawLine2D && precMove is RawLine2D)
            {
                var line = rawEntity2D as RawLine2D;
                var precLine = precMove as RawLine2D;

                // prima devo lanciare il ricalcolo profilo qui , in modo 
                // che il calculated point sia giusto..
                // todo. devo sopratutto gestire angoli
                line.X.SetValue(false, precLine.X.Value);
                line.Y.SetValue(false, precLine.Y.Value);
            }

        }

        public ICommand AddEntityCmd
        {
            get
            {
                return _addEntity ?? (_addEntity = new RelayCommand(param => AddEntity(param as string),
                                                                                    param => true));
            }
        }

        #endregion

        #region Del Entity

        RelayCommand _delEntity;

        private void DelEntity()
        {
            if (SelectedMoveViewModel == null) return;

            var index = MoveListViewModels.IndexOf(SelectedMoveViewModel);

            /*
             * non cancello punto iniziale !
             */
            if (SelectedMoveViewModel is RawInitPointViewModel)
                return;

            MoveListViewModels.Remove(SelectedMoveViewModel);

            OnPropertyChanged("MoveListViewModels");

            index--;

            var nextFocused = MoveListViewModels.ElementAtOrDefault(index);

            SelectedMoveViewModel = nextFocused;
        }

        private bool CanDeleteEntity
        {
            get
            {
                return SelectedMoveViewModel != null;
            }
        }
        public ICommand DelEntityCmd
        {
            get
            {
                return _delEntity ?? (_delEntity = new RelayCommand(param => DelEntity(),
                                                                    param => CanDeleteEntity));
            }
        }

        #endregion

        #region Move Selected

        private void GotoNextMove()
        {
            if (SelectedMoveViewModel == MoveListViewModels.Last()) return;

            var index = MoveListViewModels.IndexOf(SelectedMoveViewModel);

            index++;

            if (index > MoveListViewModels.Count) return;

            SelectedMoveViewModel = MoveListViewModels[index];
        }

        private void GotoPrevMove()
        {
            if (SelectedMoveViewModel == MoveListViewModels.First()) return;

            var index = MoveListViewModels.IndexOf(SelectedMoveViewModel);

            index--;

            if (index < 0) return;

            SelectedMoveViewModel = MoveListViewModels[index];
        }

        #endregion

        public void HandleKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.PageDown)
            {
                GotoNextMove();
                e.Handled = true;
            }

            if (e.Key == Key.PageUp)
            {
                GotoPrevMove();
                e.Handled = true;
            }

        }

        public override bool? ValidateStage()
        {
            return null;
        }
    }
}
