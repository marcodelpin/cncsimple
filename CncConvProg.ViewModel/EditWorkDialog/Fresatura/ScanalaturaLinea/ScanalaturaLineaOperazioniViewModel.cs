using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaLinea
{
    public class ScanalaturaLineaOperazioniViewModel : Common.CommonMillOperationViewModel
    {
        private readonly Model.ConversationalStructure.Lavorazioni.Fresatura.ScanalaturaLinea _scanalaturaLinea;

        public ScanalaturaLineaOperazioniViewModel(Model.ConversationalStructure.Lavorazioni.Fresatura.ScanalaturaLinea scanalaturaLinea, EditWorkViewModel parent)
            : base(scanalaturaLinea, parent)
        {
            _scanalaturaLinea = scanalaturaLinea;
        }

        public double TrochoidalStep
        {
            get { return _scanalaturaLinea.TrochoidalStep; }
            set
            {
                _scanalaturaLinea.TrochoidalStep = value;
                OnPropertyChanged("TrochoidalStep");
            }
        }

        public int ModoSgrossatura
        {
            get
            {
                return (int)_scanalaturaLinea.ModoSgrossatura;
            }

            set
            {
                _scanalaturaLinea.ModoSgrossatura = (ScanalaturaCavaMetodoLavorazione)value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("ModoSgrossatura");
            }
        }

        //public int ModoFinitura
        //{
        //    get
        //    {
        //        return (int)_scanalaturaLinea.ModoFinitura;
        //    }

        //    set
        //    {
        //        _scanalaturaLinea.ModoFinitura = (ScanalaturaCavaMetodoLavorazione)value;
        //        EditWorkParent.SyncronizeOperation();
        //        OnPropertyChanged("ModoFinitura");
        //    }
        //}
    }
}
