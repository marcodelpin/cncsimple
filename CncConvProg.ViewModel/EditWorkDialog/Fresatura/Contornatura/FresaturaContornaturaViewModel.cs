using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Contornatura
{
    public class FresaturaContornaturaViewModel : EditWorkViewModel
    {
        //private readonly IProfileEditorViewModel _profileEditorViewModel;

        private readonly ContornaturaParametriViewModel _contornaturaParametriViewModel;

        private EditStageTreeViewItem _patternScreen;

        private readonly MillingPatternSelectionViewModel _millingPatternSelectionViewModel;

        private readonly FresaturaContornatura _fresaturaContornatura;

        public FresaturaContornaturaViewModel(FresaturaContornatura fresaturaContornatura)
            : base(fresaturaContornatura)
        {
            _fresaturaContornatura = fresaturaContornatura;

            StageOperazioni = new CommonMillOperationViewModel(fresaturaContornatura, this);

            _millingPatternSelectionViewModel = new MillingPatternSelectionViewModel(_fresaturaContornatura, this);

            _millingPatternSelectionViewModel.OnPatternChanged += MillingPatternSelectionViewModelOnPatternChanged;

            _patternScreen = _millingPatternSelectionViewModel.GetViewModel(_fresaturaContornatura.Pattern);

            _millingPatternSelectionViewModel.Children.Add(_patternScreen);

            _contornaturaParametriViewModel = new ContornaturaParametriViewModel(fresaturaContornatura, this);

            TreeView.Add(_millingPatternSelectionViewModel);

            TreeView.Add(_contornaturaParametriViewModel);

            TreeView.Add(StageOperazioni);

            Initialize();

        }

        void MillingPatternSelectionViewModelOnPatternChanged(object sender, System.EventArgs e)
        {
            if (_millingPatternSelectionViewModel.Children.Contains(_patternScreen))
                _millingPatternSelectionViewModel.Children.Remove(_patternScreen);

            _patternScreen = _millingPatternSelectionViewModel.GetViewModel(_fresaturaContornatura.Pattern);
            _patternScreen.OnSourceUpdated += EditStageTreeViewItemOnSourceUpdated;

            _millingPatternSelectionViewModel.Children.Add(_patternScreen);
        }
    }
}
