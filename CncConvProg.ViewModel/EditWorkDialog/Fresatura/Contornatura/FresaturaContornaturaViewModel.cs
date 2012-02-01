using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Contornatura
{
    public class FresaturaContornaturaViewModel : EditWorkViewModel
    {
        private readonly ContornaturaParametriViewModel _contornaturaParametriViewModel;

        private readonly MillingPatternSelectionViewModel _millingPatternSelectionViewModel;

        private readonly FresaturaContornatura _fresaturaContornatura;

        public FresaturaContornaturaViewModel(FresaturaContornatura fresaturaContornatura)
            : base(fresaturaContornatura)
        {
            _fresaturaContornatura = fresaturaContornatura;

            _millingPatternSelectionViewModel = new MillingPatternSelectionViewModel(_fresaturaContornatura, this);

            _contornaturaParametriViewModel = new ContornaturaParametriViewModel(fresaturaContornatura, this);

            TreeView.Add(_millingPatternSelectionViewModel);

            TreeView.Add(_contornaturaParametriViewModel);

            Initialize();

        }
    }
}
