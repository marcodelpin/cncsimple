using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Common
{
    public class LamaturaOperazioneViewModel : ViewModelBase
    {
        private readonly ILamaturaAble _lamaturaAble;
        private readonly EditWorkViewModel _editWorkParent;

        public LamaturaOperazioneViewModel(ILamaturaAble svasaturaAble, EditWorkViewModel editWorkViewModel)
        {
            _editWorkParent = editWorkViewModel;
            _lamaturaAble = svasaturaAble;
        }

        public bool LamaturaAbilitata
        {
            get { return _lamaturaAble.LamaturaAbilitata; }
            set
            {
                _lamaturaAble.LamaturaAbilitata = value;
                _editWorkParent.SyncronizeOperation();
                OnPropertyChanged("LamaturaAbilitata");
            }
        }

        public double ProfonditaLamatura
        {
            get { return _lamaturaAble.ProfonditaLamatura; }
            set
            {
                _lamaturaAble.ProfonditaLamatura = value;
                OnPropertyChanged("ProfonditaLamatura");
            }
        }
    }

    public class BarenoOperazioneViewModel : ViewModelBase
    {
        private readonly IBarenoAble _barenoAble;
        private readonly EditWorkViewModel _editWorkParent;

        public BarenoOperazioneViewModel(IBarenoAble barenoAble, EditWorkViewModel editWorkViewModel)
        {
            _editWorkParent = editWorkViewModel;
            _barenoAble = barenoAble;
        }

        public bool AllargaturaAbilitata
        {
            get { return _barenoAble.AllargaturaAbilitata; }
            set
            {
                _barenoAble.AllargaturaAbilitata = value;
                _editWorkParent.SyncronizeOperation();
                OnPropertyChanged("AllargaturaAbilitata");
            }
        }

        public int ModoAllargatura
        {
            get
            {
                return _barenoAble.ModalitaAllargatura;
            }

            set
            {
                _barenoAble.ModalitaAllargatura = value;
                OnPropertyChanged("ModoAllargatura");
            }
        }
        public bool BarenaturaAbilitata
        {
            get { return _barenoAble.BarenoAbilitato; }
            set
            {
                _barenoAble.BarenoAbilitato = value;
                _editWorkParent.SyncronizeOperation();
                OnPropertyChanged("BarenaturaAbilitata");
            }
        }

        public double DiametroAllargatura
        {
            get { return _barenoAble.DiametroAllargatura; }
            set
            {
                _barenoAble.DiametroAllargatura = value;
                OnPropertyChanged("DiametroAllargatura");
            }
        }
    }

    public class AlesaturaOperazioneViewModel : ViewModelBase
    {
        private readonly IAlesatoreAble _centrinoAble;
        private readonly EditWorkViewModel _editWorkParent;

        public AlesaturaOperazioneViewModel(IAlesatoreAble svasaturaAble, EditWorkViewModel editWorkViewModel)
        {
            _editWorkParent = editWorkViewModel;
            _centrinoAble = svasaturaAble;
        }

        public bool AlesaturaAbilitata
        {
            get { return _centrinoAble.AlesaturaAbilitata; }
            set
            {
                _centrinoAble.AlesaturaAbilitata = value;
                _editWorkParent.SyncronizeOperation();
                OnPropertyChanged("AlesaturaAbilitata");
            }
        }

        public double ProfonditaAlesatore
        {
            get { return _centrinoAble.ProfonditaAlesatore; }
            set
            {
                _centrinoAble.ProfonditaAlesatore = value;
                OnPropertyChanged("ProfonditaAlesatore");
            }
        }
    }
    public class CentrinoOperazioneViewModel : ViewModelBase
    {
        private readonly ICentrinoAble _centrinoAble;
        private readonly EditWorkViewModel _editWorkParent;

        public CentrinoOperazioneViewModel(ICentrinoAble centrinoAble, EditWorkViewModel editWorkViewModel)
        {
            _editWorkParent = editWorkViewModel;
            _centrinoAble = centrinoAble;
        }

        public bool CentrinoAbilitato
        {
            get { return _centrinoAble.CentrinoAbilitato; }
            set
            {
                _centrinoAble.CentrinoAbilitato = value;
                _editWorkParent.SyncronizeOperation();
                OnPropertyChanged("CentrinoAbilitato");
            }
        }

        public double ProfonditaCentrino
        {
            get { return _centrinoAble.ProfonditaCentrino; }
            set
            {
                _centrinoAble.ProfonditaCentrino = value;
                OnPropertyChanged("ProfonditaCentrino");
            }
        }
    }

    public class MaschiaturaOperazioneViewModel : ViewModelBase
    {
        private readonly IMaschiaturaAble _maschiaturaAble;
        private readonly EditWorkViewModel _editWorkParent;

        public MaschiaturaOperazioneViewModel(IMaschiaturaAble maschiaturaAble, EditWorkViewModel editWorkViewModel)
        {
            _editWorkParent = editWorkViewModel;
            _maschiaturaAble = maschiaturaAble;
        }

        public bool MaschiaturaAbilitata
        {
            get { return _maschiaturaAble.MaschiaturaAbilitata; }
            set
            {
                _maschiaturaAble.MaschiaturaAbilitata = value;
                _editWorkParent.SyncronizeOperation();
                OnPropertyChanged("MaschiaturaAbilitata");
            }
        }

        public double ProfonditaMaschiatura
        {
            get { return _maschiaturaAble.ProfonditaMaschiatura; }
            set
            {
                _maschiaturaAble.ProfonditaMaschiatura = value;
                OnPropertyChanged("ProfonditaMaschiatura");
            }
        }
    }

    public class SvasaturaOperazioneViewModel : ViewModelBase
    {
        private readonly ISvasaturaAble _centrinoAble;
        private readonly EditWorkViewModel _editWorkParent;

        public SvasaturaOperazioneViewModel(ISvasaturaAble svasaturaAble, EditWorkViewModel editWorkViewModel)
        {
            _editWorkParent = editWorkViewModel;
            _centrinoAble = svasaturaAble;
        }

        public bool SvasaturaAbilitata
        {
            get { return _centrinoAble.SvasaturaAbilitata; }
            set
            {
                _centrinoAble.SvasaturaAbilitata = value;
                _editWorkParent.SyncronizeOperation();
                OnPropertyChanged("SvasaturaAbilitata");
            }
        }

        public double ProfonditaSvasatura
        {
            get { return _centrinoAble.ProfonditaSvasatura; }
            set
            {
                _centrinoAble.ProfonditaSvasatura = value;
                OnPropertyChanged("ProfonditaSvasatura");
            }
        }
    }

    public class ForaturaPuntaOperazioneViewModel : ViewModelBase
    {
        private readonly IForaturaAble _foratura;
        private readonly EditWorkViewModel _editWorkParent;

        public ForaturaPuntaOperazioneViewModel(IForaturaAble foratura, EditWorkViewModel editWorkViewModel)
        {
            _editWorkParent = editWorkViewModel;
            _foratura = foratura;
        }

        public bool ForaturaAbilitata
        {
            get { return _foratura.ForaturaAbilitata; }
            set
            {
                _foratura.ForaturaAbilitata = value;
                _editWorkParent.SyncronizeOperation();
                OnPropertyChanged("ForaturaAbilitata");
            }
        }

        public double ProfonditaForatura
        {
            get { return _foratura.ProfonditaForatura; }
            set
            {
                _foratura.ProfonditaForatura = value;
                OnPropertyChanged("ProfonditaForatura");
            }
        }


        public int ModoForatura
        {
            get
            {
                return (int) _foratura.ModalitaForatura;
            }

            set
            {
                _foratura.ModalitaForatura = (ModalitaForatura)value;
                OnPropertyChanged("ModoForatura");
            }
        }
    }
}
