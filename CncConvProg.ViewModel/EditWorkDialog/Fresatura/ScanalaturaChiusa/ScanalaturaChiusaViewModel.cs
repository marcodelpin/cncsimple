using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaChiusa
{
    public class ScanalaturaChiusaViewModel : EditWorkViewModel
    {
        //private readonly IProfileEditorViewModel _profileEditorViewModel;

        private readonly ScanalaturaChiusaParametriViewModel _contornaturaParametriViewModel;

        private EditStageTreeViewItem _patternScreen;

        private readonly ProfileEditorViewModel _stageInputProfile;

        private readonly MillingPatternSelectionViewModel _millingPatternSelectionViewModel;

        private readonly FresaturaScanalaturaChiusa _fresaturaScanalaturaChiusa;

        public ScanalaturaChiusaViewModel(FresaturaScanalaturaChiusa fresaturaScanalaturaChiusa)
            : base(fresaturaScanalaturaChiusa)
        {
            _fresaturaScanalaturaChiusa = fresaturaScanalaturaChiusa;

            StageOperazioni = new CommonMillOperationViewModel(fresaturaScanalaturaChiusa, this);

            _millingPatternSelectionViewModel = new MillingPatternSelectionViewModel(_fresaturaScanalaturaChiusa, this);

            _millingPatternSelectionViewModel.OnPatternChanged += MillingPatternSelectionViewModelOnPatternChanged;

            _patternScreen = _millingPatternSelectionViewModel.GetViewModel(_fresaturaScanalaturaChiusa.Pattern);

            _millingPatternSelectionViewModel.Children.Add(_patternScreen);

           // _stageInputProfile = new ProfileEditorViewModel(fresaturaContornatura.Profile, this, ProfileEditorViewModel.AxisSystem.Xy);

            _contornaturaParametriViewModel = new ScanalaturaChiusaParametriViewModel(fresaturaScanalaturaChiusa, this);

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

            _patternScreen = _millingPatternSelectionViewModel.GetViewModel(_fresaturaScanalaturaChiusa.Pattern);
            _patternScreen.OnSourceUpdated += EditStageTreeViewItemOnSourceUpdated;

            _millingPatternSelectionViewModel.Children.Add(_patternScreen);
        }
    }
}
