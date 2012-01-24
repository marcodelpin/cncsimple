using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Common
{
    public class CommonMillOperationViewModel : EditStageTreeViewItem
    {
        private readonly IMillWorkable _millWorkable;

        public CommonMillOperationViewModel(IMillWorkable millWorkable, EditWorkViewModel parent)
            : base("Operation", parent)
        {
            _millWorkable = millWorkable;
        }

        public bool FinishWithCompensation
        {
            get { return _millWorkable.FinishWithCompensation; }

            set
            {
                _millWorkable.FinishWithCompensation = value;
                OnPropertyChanged("FinishWithCompensation");

            }
        }

        public double SovrametalloFinitura
        {
            get { return _millWorkable.SovrametalloFinituraProfilo; }
            set
            {
                _millWorkable.SovrametalloFinituraProfilo = value;
                OnPropertyChanged("SovrametalloFinitura");
            }
        }

        public double ProfonditaFresaSmussatura
        {
            get { return _millWorkable.ProfonditaFresaSmussatura; }
            set
            {
                _millWorkable.ProfonditaFresaSmussatura = value;
                OnPropertyChanged("ProfonditaFresaSmussatura");
            }
        }

        public bool SgrossaturaAbilitata
        {
            get { return _millWorkable.Sgrossatura.Abilitata; }

            set
            {
                _millWorkable.Sgrossatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SgrossaturaAbilitata");

            }
        }

        public bool FinituraAbilitata
        {
            get { return _millWorkable.Finitura.Abilitata; }

            set
            {
                _millWorkable.Finitura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("FinituraAbilitata");
            }
        }

        public bool SmussaturaAbilitata
        {
            get { return _millWorkable.Smussatura.Abilitata; }

            set
            {
                _millWorkable.Smussatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SmussaturaAbilitata");
            }
        }

      }
}
