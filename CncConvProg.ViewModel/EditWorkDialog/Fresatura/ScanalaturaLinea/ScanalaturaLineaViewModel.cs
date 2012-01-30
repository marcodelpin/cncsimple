using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Spianatura;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaLinea
{
    public sealed class ScanalaturaLineaViewModel : EditWorkViewModel
    {
        private readonly ScanalaturaLineaParametriViewModel _spianaturaParametriViewModel;

        public ScanalaturaLineaViewModel(Model.ConversationalStructure.Lavorazioni.Fresatura.ScanalaturaLinea scanalaturaLinea)
            : base(scanalaturaLinea)
        {
           // StageOperazioni = new ScanalaturaLineaOperazioniViewModel(scanalaturaLinea, this);

            _spianaturaParametriViewModel = new ScanalaturaLineaParametriViewModel(this, scanalaturaLinea);

            TreeView.Add(_spianaturaParametriViewModel);

         //   TreeView.Add(StageOperazioni);
         
            Initialize();
        }
    }
}
