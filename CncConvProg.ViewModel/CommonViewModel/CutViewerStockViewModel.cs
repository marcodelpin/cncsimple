using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.Simulation;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.CommonViewModel
{
    public class CutViewerStockViewModel : ViewModelBase
    {
        private readonly CutViewerStock _stock;
        public CutViewerStockViewModel(CutViewerStock stock)
        {
            _stock = stock;
        }

        public double Larghezza
        {
            get { return _stock.Larghezza; }
            set
            {
                _stock.Larghezza = value;
                OnPropertyChanged("Larghezza");
            }
        }

        public double Altezza
        {
            get { return _stock.Altezza; }
            set
            {
                _stock.Altezza = value;
                OnPropertyChanged("Altezza");
            }
        }

        public double Spessore
        {
            get { return _stock.Spessore; }
            set
            {
                _stock.Spessore = value;
                OnPropertyChanged("Spessore");
            }
        }

        public double OriginX
        {
            get { return _stock.OriginX; }
            set
            {
                _stock.OriginX = value;
                OnPropertyChanged("OriginX");
            }
        }

        public double OriginY
        {
            get { return _stock.OriginY; }
            set
            {
                _stock.OriginY = value;
                OnPropertyChanged("OriginY");
            }
        }

        public double OriginZ
        {
            get { return _stock.OriginZ; }
            set
            {
                _stock.OriginZ = value;
                OnPropertyChanged("OriginZ");
            }
        }
    }
}
