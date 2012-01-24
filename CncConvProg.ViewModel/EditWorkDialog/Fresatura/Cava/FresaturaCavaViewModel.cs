using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Contornatura;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Cava
{
    public class FresaturaCavaViewModel : EditWorkViewModel
    {
        //private readonly IProfileEditorViewModel _profileEditorViewModel;

        private readonly FresaturaCavaParametriViewModel _contornaturaParametriViewModel;

        private EditStageTreeViewItem _patternScreen;

        private readonly ProfileEditorViewModel _stageInputProfile;

        private readonly MillingPatternSelectionViewModel _millingPatternSelectionViewModel;

        private readonly FresaturaCava _fresaturaContornatura;
        public FresaturaCavaViewModel(FresaturaCava fresaturaContornatura)
            : base(fresaturaContornatura)
        {
            _fresaturaContornatura = fresaturaContornatura;

            StageOperazioni = new FresaturaCavaOperazioniViewModel(fresaturaContornatura, this);

            _millingPatternSelectionViewModel = new MillingPatternSelectionViewModel(_fresaturaContornatura, this);

            _millingPatternSelectionViewModel.OnPatternChanged += MillingPatternSelectionViewModelOnPatternChanged;

            _patternScreen = _millingPatternSelectionViewModel.GetViewModel(_fresaturaContornatura.Pattern);

            _millingPatternSelectionViewModel.Children.Add(_patternScreen);

           // _stageInputProfile = new ProfileEditorViewModel(fresaturaContornatura.Profile, this, ProfileEditorViewModel.AxisSystem.Xy);

            _contornaturaParametriViewModel = new FresaturaCavaParametriViewModel(fresaturaContornatura, this);

            TreeView.Add(_millingPatternSelectionViewModel);

            TreeView.Add(_contornaturaParametriViewModel);

            //TreeView.Add(_stageInputProfile);
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
