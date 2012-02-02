using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CncConvProg.Model.Simulation;
using CncConvProg.ViewModel.Dialog;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.CommonViewModel
{
    public sealed class CutViewerStockViewModel : ViewModelBase
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
                if (value == _stock.Larghezza) return;

                _stock.Larghezza = value;
                OnPropertyChanged("Larghezza");
                OnPropertyChanged("StockCube");


            }
        }

        public double Altezza
        {
            get { return _stock.Altezza; }
            set
            {
                if (value == _stock.Altezza) return;

                _stock.Altezza = value;
                OnPropertyChanged("Altezza");
                OnPropertyChanged("StockCube");

            }
        }

        public double Spessore
        {
            get { return _stock.Spessore; }
            set
            {
                if (value == _stock.Spessore) return;

                _stock.Spessore = value;
                OnPropertyChanged("Spessore");
                OnPropertyChanged("StockCube");

            }
        }

        public double OriginX
        {
            get { return _stock.OriginX; }
            set
            {
                if (value == _stock.OriginX) return;

                _stock.OriginX = value;
                OnPropertyChanged("OriginX");
                OnPropertyChanged("StockCube");

            }
        }

        public double OriginY
        {
            get { return _stock.OriginY; }
            set
            {
                if (value == _stock.OriginY) return;

                _stock.OriginY = value;
                OnPropertyChanged("OriginY");
                OnPropertyChanged("StockCube");

            }
        }

        public double OriginZ
        {
            get { return _stock.OriginZ; }
            set
            {
                if (value == _stock.OriginZ) return;

                _stock.OriginZ = value;
                OnPropertyChanged("OriginZ");
                OnPropertyChanged("StockCube");
            }
        }

        public StockCube StockCube
        {
            get
            {
                var sc = new StockCube
                             {
                                 Width = Altezza,
                                 Length = Larghezza,
                                 Height = Spessore,
                                 X = -OriginX,
                                 Y = -OriginY,
                                 Z =  -(Spessore/2) -OriginZ
                             };
                return sc;
            }
        }

        RelayCommand _updateStock;
        /// <summary>
        /// Salva modifiche database
        /// </summary>
        private void UpdateStockCommand()
        {
            OnPropertyChanged("StockCube");

        }

        public ICommand UpdateStockCmd
        {
            get
            {
                return _updateStock ?? (_updateStock = new RelayCommand(param => UpdateStockCommand(),
                                                                              param => true));
            }
        }

    }
}
