using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Lato
{
    public class FresaturaLatoOperazioniViewModel : EditStageTreeViewItem
    {
        private readonly Model.ConversationalStructure.Lavorazioni.Fresatura.FresaturaLato _fresaturaLato;

        public FresaturaLatoOperazioniViewModel(Model.ConversationalStructure.Lavorazioni.Fresatura.FresaturaLato fresaturaLato, EditWorkViewModel parent)
            : base("Operation", parent)
        {
            _fresaturaLato = fresaturaLato;
        }

        public bool FinishWithCompensation
        {
            get { return _fresaturaLato.FinishWithCompensation; }

            set
            {
                _fresaturaLato.FinishWithCompensation = value;
                OnPropertyChanged("FinishWithCompensation");

            }
        }

        public double SovrametalloFinitura
        {
            get { return _fresaturaLato.SovrametalloPerFinitura; }
            set
            {
                _fresaturaLato.SovrametalloPerFinitura = value;
                OnPropertyChanged("SovrametalloFinitura");
            }
        }

        public double ProfonditaFresaSmussatura
        {
            get { return _fresaturaLato.ProfonditaFresaSmussatura; }
            set
            {
                _fresaturaLato.ProfonditaFresaSmussatura = value;
                OnPropertyChanged("ProfonditaFresaSmussatura");
            }
        }

        public bool SgrossaturaAbilitata
        {
            get { return _fresaturaLato.Sgrossatura.Abilitata; }

            set
            {
                _fresaturaLato.Sgrossatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SgrossaturaAbilitata");
            }
        }

        public bool FinituraAbilitata
        {
            get { return _fresaturaLato.Finitura.Abilitata; }

            set
            {
                _fresaturaLato.Finitura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("FinituraAbilitata");
            }
        }

        public bool SmussaturaAbilitata
        {
            get { return _fresaturaLato.Smussatura.Abilitata; }

            set
            {
                _fresaturaLato.Smussatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SmussaturaAbilitata");
            }
        }

    }
}
