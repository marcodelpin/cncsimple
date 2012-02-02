using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ToolMachine;
using CncConvProg.ViewModel.CommonViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.Dialog
{
    public class FaseDiLavoroViewModel : ViewModelBase
    {
        public FaseDiLavoro FaseDiLavoro
        {
            get
            {
                return _fase;
            }
        }
        private readonly FaseDiLavoro _fase;

        private CutViewerStockViewModel _cut;
        public CutViewerStockViewModel CutViewerStockViewModel
        {
            get { return _cut; }
            set
            {
                _cut = value;
                OnPropertyChanged("CutViewerStockViewModel");
            }
        }

        public FaseDiLavoroViewModel(FaseDiLavoro fase)
        {

            _fase = fase;

            CutViewerStockViewModel = new CutViewerStockViewModel(_fase.Stock);

        }


        public string DescrizioneFase { get; set; }

        public double NoChangeToolSecureZ
        {
            get { return _fase.NoChangeToolSecureZ; }

            set
            {
                _fase.NoChangeToolSecureZ = value;
                OnPropertyChanged("NoChangeToolSecureZ");
            }
        }

        public string NomeProgramma
        {
            get { return _fase.Descrizione; }
            set
            {
                _fase.Descrizione = value;
                OnPropertyChanged("NomeProgramma");
            }
        }

        public int NumeroProgramma
        {
            get
            {
                return _fase.ProgramNumber;
            }
            set
            {
                _fase.ProgramNumber = value;
                OnPropertyChanged("NumeroProgramma");
            }
        }


        public ToolMachine Machine { get; set; }

        public List<ToolMachine> MachineList { get; set; }

        /*
         * Per ora metto le origini nella classe base per la fase , poi dovrei metterle solamente 
         * nella fase di centro di lavoro visto che  
         */

        public bool OriginG54
        {
            get
            {
                return _fase.OriginG54;
            }
            set
            {
                _fase.OriginG54 = value;
                OnPropertyChanged("Origin54");
            }
        }
        public bool OriginG55
        {
            get
            {
                return _fase.OriginG55;
            }
            set
            {
                _fase.OriginG55 = value;
                OnPropertyChanged("OriginG55");
            }
        }
        public bool OriginG56
        {
            get
            {
                return _fase.OriginG56;
            }
            set
            {
                _fase.OriginG56 = value;
                OnPropertyChanged("OriginG56");
            }
        }
        public bool OriginG57
        {
            get
            {
                return _fase.OriginG57;
            }
            set
            {
                _fase.OriginG57 = value;
                OnPropertyChanged("OriginG57");
            }
        }
        public bool OriginG58
        {
            get
            {
                return _fase.OriginG58;
            }
            set
            {
                _fase.OriginG58 = value;
                OnPropertyChanged("OriginG58");
            }
        }
        public bool OriginG59
        {
            get
            {
                return _fase.OriginG59;
            }
            set
            {
                _fase.OriginG59 = value;
                OnPropertyChanged("OriginG59");
            }
        }


    }


    public class StockCube
    {
        public double Length;
        public double Height;
        public double Width;

        public double X;
        public double Y;
        public double Z;
    }
}


