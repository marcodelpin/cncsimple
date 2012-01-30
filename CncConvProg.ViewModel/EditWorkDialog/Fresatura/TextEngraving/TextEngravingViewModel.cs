using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Cava;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.TextEngraving
{
    public class TextEngravingViewModel : EditWorkViewModel
    {
        //private readonly IProfileEditorViewModel _profileEditorViewModel;

        private readonly TextEngravingParametriViewModel _contornaturaParametriViewModel;

        private readonly TextEngravingModel _fresaturaContornatura;

        public TextEngravingViewModel(TextEngravingModel fresaturaContornatura)
            : base(fresaturaContornatura)
        {
            _fresaturaContornatura = fresaturaContornatura;

            /* Stage Vuoto*/
           // StageOperazioni = new EditStageTreeViewItem("Operation", this);

            _contornaturaParametriViewModel = new TextEngravingParametriViewModel(fresaturaContornatura, this);

            TreeView.Add(_contornaturaParametriViewModel);

          //  TreeView.Add(StageOperazioni);

            Initialize();

        }
    }
}
