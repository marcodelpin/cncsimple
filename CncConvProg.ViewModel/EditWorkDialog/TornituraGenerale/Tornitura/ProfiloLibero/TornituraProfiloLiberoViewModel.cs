using System;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Tornitura.ProfiloLibero
{
    public sealed class TornituraProfiloLiberoViewModel : EditWorkViewModel
    {
        //  private readonly SpianaturaParametriViewModel _spianaturaParametriViewModel;

        //private readonly LavorazioneViewModel _lavorazioneViewModel;

        private readonly EditStageTreeViewItem _stageInputProfile;
        //private readonly EditStageTreeViewItem _stageParametriLavorazione;

        public TornituraProfiloLiberoViewModel(Model.ConversationalStructure.Lavorazioni.Tornitura.Tornitura tornitura)
            : base(tornitura)
        {
            _stageInputProfile = new ProfileEditorViewModel(tornitura.Profile, this, ProfileEditorViewModel.AxisSystem.Xz);

            TreeView.Add(_stageInputProfile);

            Initialize();
        }

    }
}
