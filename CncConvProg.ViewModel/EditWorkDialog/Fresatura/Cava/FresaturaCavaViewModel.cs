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

        private readonly FresaturaCavaParametriViewModel _parametriViewModel;

        private readonly MillingPatternSelectionViewModel _millingPatternSelectionViewModel;

        private readonly FresaturaCava _fresaturaContornatura;

        public FresaturaCavaViewModel(FresaturaCava fresaturaContornatura)
            : base(fresaturaContornatura)
        {
            _fresaturaContornatura = fresaturaContornatura;

            _millingPatternSelectionViewModel = new MillingPatternSelectionViewModel(_fresaturaContornatura, this);

            _parametriViewModel = new FresaturaCavaParametriViewModel(fresaturaContornatura, this);

            TreeView.Add(_millingPatternSelectionViewModel);

            TreeView.Add(_parametriViewModel);

            Initialize();

        }
    }
}
