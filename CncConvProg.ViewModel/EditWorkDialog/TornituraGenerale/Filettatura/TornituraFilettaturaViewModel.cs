using CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Filettatura
{
    public sealed class TornituraFilettaturaViewModel : EditWorkViewModel
    {
        private readonly EditStageTreeViewItem _parameterStage;

        public TornituraFilettaturaViewModel(TornituraFilettatura scanalatura)
            : base(scanalatura)
        {
            _parameterStage = new TornituraFilettaturaParametriViewModel(scanalatura, this);

            TreeView.Add(_parameterStage);

            Initialize();
        }

    }
}
