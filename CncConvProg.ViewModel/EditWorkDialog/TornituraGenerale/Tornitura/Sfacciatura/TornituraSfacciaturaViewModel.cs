using CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Tornitura.Sfacciatura
{
    public sealed class TornituraSfacciaturaViewModel : EditWorkViewModel
    {
        private readonly EditStageTreeViewItem _stageParameter;

        public TornituraSfacciaturaViewModel(TornituraSfacciatura sfacciatura)
            : base(sfacciatura)
        {
            _stageParameter = new SfacciaturaParametriViewModel(sfacciatura, this);

            TreeView.Add(_stageParameter);

            Initialize();
        }

    }
}
