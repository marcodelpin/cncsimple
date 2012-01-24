using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaChiusa
{
    public class FresaturaScanalaturaChiusaOperazioniViewModel : EditStageTreeViewItem
    {
        private readonly FresaturaScanalaturaChiusa _fresaturaScanalaturaChiusa;

        public FresaturaScanalaturaChiusaOperazioniViewModel(FresaturaScanalaturaChiusa fresaturaScanalaturaChiusa, EditWorkViewModel parent)
            : base("Operation", parent)
        {
            _fresaturaScanalaturaChiusa = fresaturaScanalaturaChiusa;
        }

        public bool SgrossaturaAbilitata
        {
            get { return _fresaturaScanalaturaChiusa.Sgrossatura.Abilitata; }

            set
            {
                _fresaturaScanalaturaChiusa.Sgrossatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SgrossaturaAbilitata");

            }
        }

        public bool FinituraAbilitata
        {
            get { return _fresaturaScanalaturaChiusa.Finitura.Abilitata; }

            set
            {
                _fresaturaScanalaturaChiusa.Finitura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("FinituraAbilitata");
            }
        }

        public bool SmussaturaAbilitata
        {
            get { return _fresaturaScanalaturaChiusa.Smussatura.Abilitata; }

            set
            {
                _fresaturaScanalaturaChiusa.Smussatura.Abilitata = value;
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
