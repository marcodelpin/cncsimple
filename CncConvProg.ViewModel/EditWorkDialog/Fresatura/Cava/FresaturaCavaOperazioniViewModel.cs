using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Cava
{
    public class FresaturaCavaOperazioniViewModel : EditStageTreeViewItem
    {
        private readonly FresaturaCava _fresaturaContornatura;

        public FresaturaCavaOperazioniViewModel(FresaturaCava fresaturaContornatura, EditWorkViewModel parent)
            : base("Operation", parent)
        {
            _fresaturaContornatura = fresaturaContornatura;
        }

        public bool FinishWithCompensation
        {
            get { return _fresaturaContornatura.FinishWithCompensation; }

            set
            {
                _fresaturaContornatura.FinishWithCompensation = value;
                OnPropertyChanged("FinishWithCompensation");

            }
        }

        public double SovrametalloFinitura
        {
            get { return _fresaturaContornatura.SovrametalloFinituraProfilo; }
            set
            {
                _fresaturaContornatura.SovrametalloFinituraProfilo = value;
                OnPropertyChanged("SovrametalloFinitura");
            }
        }

        public double ProfonditaFresaSmussatura
        {
            get { return _fresaturaContornatura.ProfonditaFresaSmussatura; }
            set
            {
                _fresaturaContornatura.ProfonditaFresaSmussatura = value;
                OnPropertyChanged("ProfonditaFresaSmussatura");
            }
        }

        public bool SgrossaturaAbilitata
        {
            get { return _fresaturaContornatura.Sgrossatura.Abilitata; }

            set
            {
                _fresaturaContornatura.Sgrossatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SgrossaturaAbilitata");

            }
        }

        public bool FinituraAbilitata
        {
            get { return _fresaturaContornatura.Finitura.Abilitata; }

            set
            {
                _fresaturaContornatura.Finitura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("FinituraAbilitata");
            }
        }

        public bool SmussaturaAbilitata
        {
            get { return _fresaturaContornatura.Smussatura.Abilitata; }

            set
            {
                _fresaturaContornatura.Smussatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SmussaturaAbilitata");
            }
        }

      }
}
