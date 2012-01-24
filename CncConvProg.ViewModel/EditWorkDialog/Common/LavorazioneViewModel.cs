//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Windows.Documents;
//using System.Windows.Input;
//using CncConvProg.Geometry.Entity2D;
//using CncConvProg.Model.ConversationalStructure;
//using CncConvProg.Model.ConversationalStructure.Abstraction;
//using CncConvProg.Model.ConversationalStructure.Lavorazioni;
//using CncConvProg.Model.ConversationalStructure.Operazioni;
//using CncConvProg.ViewModel.EditWorkDialog.OperationViewModel;
//using CncConvProg.ViewModel.MVVM_Library;
//using MVVM_Library;

//namespace CncConvProg.ViewModel.EditWorkDialog.Common
//{
//    public class LavorazioneViewModel : TreeViewItemViewModel, IPreviewable
//    {
//        private readonly Lavorazione _lavorazione;

//        public LavorazioneViewModel(TreeViewItemViewModel parent, Lavorazione lavorazione)
//            : base(parent,false)
//        {
//            Label = "Operazioni";
//            _lavorazione = lavorazione;

//            Operazioni = new ObservableCollection<TreeViewViewModel.OperationViewModel>();

//            foreach (var operazione in _lavorazione.Operazioni)
//                Operazioni.Add(new ParametriFresaCandelaViewModel(operazione as OperazioneFresaCandela, ));

//            // todo _: associare view model al proprio model
//        }

//        public event EventHandler OnSourceUpdated;

//        private void RequestUpdateSource()
//        {
//            EventHandler handler = this.OnSourceUpdated;
//            if (handler != null)
//                handler(this, EventArgs.Empty);
//        }

//        private TreeViewViewModel.OperationViewModel _operazioneSelezionata;
//        public TreeViewViewModel.OperationViewModel OperazioneSelezionata
//        {
//            get
//            {
//                return _operazioneSelezionata;
//            }
//            set
//            {
//                _operazioneSelezionata = value;
//                RequestUpdateSource();
//                OnPropertyChanged("OperazioneSelezionata");
//            }
//        }

//        private ObservableCollection<TreeViewViewModel.OperationViewModel> _operations;
//        public ObservableCollection<TreeViewViewModel.OperationViewModel> Operazioni
//        {
//            get
//            {
//                return _operations;
//            }
//            set
//            {
//                _operations = value;
//                RequestUpdateSource();
//                OnPropertyChanged("Operazioni");
//            }
//        }
//        #region Genera Operazioni

//        RelayCommand _generaOperazioni;

//        private void GeneraOperazioni()
//        {
//            _lavorazione.GeneraOperazioni();

//            if (Operazioni.Count > 0)
//                Operazioni.Clear();

//            // todo : fix this
//            foreach (var operazione in _lavorazione.Operazioni)
//                Operazioni.Add(new ParametriFresaCandelaViewModel(operazione as OperazioneFresaCandela));
//        }

//        public ICommand GeneraOperazioniCmd
//        {
//            get
//            {
//                return _generaOperazioni ?? (_generaOperazioni = new RelayCommand(param => GeneraOperazioni(),
//                                                                                  param => true));
//            }
//        }

//        #endregion

//        public IEnumerable<IEntity2D> GetPreview()
//        {
//            var profiloLav = _lavorazione.GetPreview();

//            var preview = profiloLav.ToList();

//            foreach (var operazione in _lavorazione.Operazioni)
//            {
//                var isSelected = false;
//                if (OperazioneSelezionata != null && operazione == OperazioneSelezionata.Source)
//                    isSelected = true;

//                var sin = operazione.GetPreview();

//                foreach (var entity2D in sin)
//                {
//                    entity2D.IsSelected = isSelected;

//                    entity2D.PlotStyle = entity2D.IsSelected ? EnumPlotStyle.SelectedPath : EnumPlotStyle.Path;

//                    preview.Add(entity2D);
//                }
//            }

//            return preview;
//        }
//    }
//}
