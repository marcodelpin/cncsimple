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

        private readonly ProfileEditorViewModel _stageInputProfile;

        private readonly MillingPatternSelectionViewModel _millingPatternSelectionViewModel;

        private readonly FresaturaScanalaturaChiusa _fresaturaScanalaturaChiusa;

        public ScanalaturaChiusaViewModel(FresaturaScanalaturaChiusa fresaturaScanalaturaChiusa)
            : base(fresaturaScanalaturaChiusa)
        {
            _fresaturaScanalaturaChiusa = fresaturaScanalaturaChiusa;

            _millingPatternSelectionViewModel = new MillingPatternSelectionViewModel(_fresaturaScanalaturaChiusa, this);

            _contornaturaParametriViewModel = new ScanalaturaChiusaParametriViewModel(fresaturaScanalaturaChiusa, this);

            TreeView.Add(_millingPatternSelectionViewModel);

            TreeView.Add(_contornaturaParametriViewModel);

            Initialize();

        }
    }
}
