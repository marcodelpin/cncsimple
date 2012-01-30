using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaLinea;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Lato
{
    public sealed class FresaturaLatoViewModel : EditWorkViewModel
    {
        private readonly FresaturaLatoParametriViewModel _spianaturaParametriViewModel;

        public FresaturaLatoViewModel(Model.ConversationalStructure.Lavorazioni.Fresatura.FresaturaLato fresaturaLato)
            : base(fresaturaLato)
        {
          //  StageOperazioni = new CommonMillOperationViewModel(fresaturaLato, this);

            _spianaturaParametriViewModel = new FresaturaLatoParametriViewModel(this, fresaturaLato);

            TreeView.Add(_spianaturaParametriViewModel);

         //   TreeView.Add(StageOperazioni);
         
            Initialize();
        }
    }
}
