using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Scanalatura
{
    public class TornituraScanalaturaCommonViewModel : EditStageTreeViewItem
    {
        private readonly TornituraScanalatura _groove;

        public TornituraScanalaturaCommonViewModel(TornituraScanalatura groove, EditWorkViewModel parent)
            : base("Extern", parent)
        {
            _groove = groove;
        }

        public double DiametroIni
        {
            get { return _groove.DiameterIniziale; }
            set
            {
                _groove.DiameterIniziale = value;
                OnPropertyChanged("DiametroIni");
            }
        }

        public double DiametroFin
        {
            get { return _groove.DiameterFinale; }
            set
            {
                _groove.DiameterFinale = value;
                OnPropertyChanged("DiametroFin");
            }
        }

        public double StartZ
        {
            get { return _groove.StartZ; }
            set
            {
                _groove.StartZ = value;
                OnPropertyChanged("StartZ");
            }
        }

        public double Larghezza
        {
            get { return _groove.Larghezza; }
            set
            {
                _groove.Larghezza = value;
                OnPropertyChanged("Larghezza");
            }
        }

        public int NumeroGole
        {
            get { return _groove.NumeroGole; }
            set
            {
                _groove.NumeroGole = value;
                OnPropertyChanged("NumeroGole");
            }
        }

        public double DistanzaGole
        {
            get { return _groove.DistanzaGole; }
            set
            {
                _groove.DistanzaGole = value;
                OnPropertyChanged("DistanzaGole");
            }
        }

        public override bool? ValidateStage()
        {
            return null;
        }
    }
}
