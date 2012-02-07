using CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Scanalatura
{
    /// <summary>
    /// Cerco di fare viewModel unico per classi di tornitura di scanalatura.
    /// </summary>
    public sealed class TornituraScanalaturaViewModel : EditWorkViewModel
    {
        /*
         * vorrei cercare di fare schermo unico per pattern e immissione dati patter,
         * 
         * che non sarebbe fastidioso tranne che per immissione profilo ..
         * 
         * dovro cmq separarlo da immissione profilo di centro per via delle isole.
         * 
         * ad ogni modo posso provare che su profile editor ad aggiungere pagina..
         */
        //  _fresaturaContornatura = fresaturaContornatura;

        //StageOperazioni = new CommonMillOperationViewModel(fresaturaContornatura, this);

        //_millingPatternSelectionViewModel = new MillingPatternSelectionViewModel(_fresaturaContornatura, this);

        //_millingPatternSelectionViewModel.OnPatternChanged += MillingPatternSelectionViewModelOnPatternChanged;

        //_patternScreen = _millingPatternSelectionViewModel.GetViewModel(_fresaturaContornatura.Pattern);

        //_millingPatternSelectionViewModel.Children.Add(_patternScreen);

        //_contornaturaParametriViewModel = new ContornaturaParametriViewModel(fresaturaContornatura, this);

        //TreeView.Add(_millingPatternSelectionViewModel);

        //TreeView.Add(_contornaturaParametriViewModel);

        //TreeView.Add(StageOperazioni);

        //Initialize();
        //  private readonly SpianaturaParametriViewModel _spianaturaParametriViewModel;

        //private readonly LavorazioneViewModel _lavorazioneViewModel;

        private readonly EditStageTreeViewItem _parameterStage;
        //private readonly EditStageTreeViewItem _stageParametriLavorazione;

        //   private readonly EditStageTreeViewItem _patternStage;

        public TornituraScanalaturaViewModel(TornituraScanalatura scanalatura)
            : base(scanalatura)
        {
            _parameterStage = new TornituraScanalaturaCommonViewModel(scanalatura, this);

            TreeView.Add(_parameterStage);

            Initialize();
        }

    }
}
