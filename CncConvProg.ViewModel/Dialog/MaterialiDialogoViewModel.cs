using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.ThreadTable;
using CncConvProg.Model.Tool;
using CncConvProg.ViewModel.MainViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.Dialog
{
    public class MaterialiDialogoViewModel : ViewModelBase, IDialog
    {
        private readonly MagazzinoUtensile _magazzinoUtensile;

        public ObservableCollection<MaterialViewModel> Materiali
        {
            get
            {
                //var mat = _magazzinoUtensile.GetMaterials();

                //var rslt = new ObservableCollection<MaterialViewModel>();

                //foreach (var viewModelBase in mat)
                //{
                //    rslt.Add(new MaterialViewModel(viewModelBase));
                //}

                //return rslt;
                return new ObservableCollection<MaterialViewModel>();
            }
        }

        public MaterialiDialogoViewModel(MagazzinoUtensile magazzinoUtensile)
        {
            _magazzinoUtensile = magazzinoUtensile;
        }

        private MaterialViewModel _materialeSelezionato;

        public MaterialViewModel MaterialeSelezionato
        {
            get { return _materialeSelezionato; }

            set
            {
                _materialeSelezionato = value;
                OnPropertyChanged("MaterialeSelezionato");
            }
        }

        #region New Material

        RelayCommand _addMaterialCmd;
        /// <summary>
        /// Salva modifiche database
        /// </summary>
        private void AddMaterial()
        {
         //   var mat = new Materiale();

          //  _magazzinoUtensile.AddMaterial(mat);

            OnPropertyChanged("Materiali");
        }

        public ICommand AddMaterialCmd
        {
            get
            {
                return _addMaterialCmd ?? (_addMaterialCmd = new RelayCommand(param => AddMaterial(),
                                                                              param => true));
            }
        }

        #endregion

        /*
         * su materiale cancellato eliminare i parametri 
         */
        public void Save(IMainViewModel mainViewModel)
        {
            PathFolderHelper.SaveMagazzinoUtensile(_magazzinoUtensile);

            /*
             * aggiornare viewModel
             */
        }
    }

    public class MaterialViewModel : ViewModelBase
    {
        private readonly Materiale _materiale;

        public MaterialViewModel(Materiale materiale)
        {
            _materiale = materiale;
        }


        public string Descrizione
        {
            get { return _materiale.Descrizione; }

            set
            {
                _materiale.Descrizione = value;
                OnPropertyChanged("Descrizione");
            }
        }

    }
}
