namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Spianatura
{
    public sealed class SpianaturaViewModel : EditWorkViewModel
    {
        private readonly SpianaturaParametriViewModel _spianaturaParametriViewModel;

        public SpianaturaViewModel(Model.ConversationalStructure.Lavorazioni.Fresatura.Spianatura spianatura)
            : base(spianatura)
        {
            StageOperazioni = new SpianaturaOperazioni(spianatura, this);

            _spianaturaParametriViewModel = new SpianaturaParametriViewModel(this, spianatura);

            TreeView.Add(_spianaturaParametriViewModel);

            TreeView.Add(StageOperazioni);
         
            Initialize();
        }
    }
}
