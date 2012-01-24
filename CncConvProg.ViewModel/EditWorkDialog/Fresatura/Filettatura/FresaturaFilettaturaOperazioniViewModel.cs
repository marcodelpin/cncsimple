using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Filettatura
{
    public class FresaturaFilettaturaOperazioniViewModel : EditStageTreeViewItem
    {
        private readonly FresaturaFilettatura _fresaturaContornatura;

        public FresaturaFilettaturaOperazioniViewModel(FresaturaFilettatura fresaturaContornatura, EditWorkViewModel parent)
            : base("Operation", parent)
        {
            _fresaturaContornatura = fresaturaContornatura;
        }

        public bool SgrossaturaAbilitata
        {
            get { return _fresaturaContornatura.FilettaturaOp.Abilitata; }

            set
            {
                _fresaturaContornatura.FilettaturaOp.Abilitata = value;
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

        //public SpiantaturaMetodologia ModoSgrossatura
        //{
        //    get
        //    {
        //        return _fresaturaContornatura.ModoSgrossatura;
        //    }

        //    set
        //    {
        //        _fresaturaContornatura.ModoSgrossatura = value;
        //        OnPropertyChanged("ModoSgrossatura");
        //    }
        //}

        //public SpiantaturaMetodologia ModoFinitura
        //{
        //    get
        //    {
        //        return _fresaturaContornatura.ModoFinitura;
        //    }

        //    set
        //    {
        //        _fresaturaContornatura.ModoFinitura = value;
        //        OnPropertyChanged("ModoFinitura");
        //    }
        //}


    }
}
