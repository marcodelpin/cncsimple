using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CncConvProg.Model;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.ThreadTable;
using CncConvProg.ViewModel.MainViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Abstractions.Wpf.Intefaces;

namespace CncConvProg.ViewModel.Dialog
{
    public sealed class TabellaFilettaturaViewModel : ViewModelBase, IDialog
    {
        public ObservableCollection<FilettaturaViewModel> TipologieFilettatura
        {
            get
            {
                var ttype = _tabellaFilettature.Filettature.OrderBy(l => l.Descrizione);

                var rslt = new ObservableCollection<FilettaturaViewModel>();

                foreach (var viewModelBase in ttype)
                {
                    rslt.Add(FilettaturaViewModel.GetViewModel(viewModelBase, _measureUnit));
                }

                return rslt;
            }
        }



        private FilettaturaViewModel _tipoMaschiaturaSelezionato;
        public FilettaturaViewModel TipoMaschiaturaSelezionato
        {

            get { return _tipoMaschiaturaSelezionato; }
            set
            {
                _tipoMaschiaturaSelezionato = value;
                OnPropertyChanged("TipoMaschiaturaSelezionato");
            }
        }

        enum ThreadType
        {
            Metrica,
            InPollici
        }

        private readonly TabellaFilettature _tabellaFilettature;

        private readonly MeasureUnit _measureUnit;

        public TabellaFilettaturaViewModel(TabellaFilettature tabellaFilettature, MeasureUnit measureUnit)
        {
            _tabellaFilettature = tabellaFilettature;

            _measureUnit = measureUnit;

            OnPropertyChanged("TipologieFilettatura");
        }

        #region Nuova Tipologia

        RelayCommand _addThreadCmd;
        /// <summary>
        /// Salva modifiche database
        /// </summary>
        private void AddThreadType(string type)
        {
            byte tipologia;

            byte.TryParse(type, out tipologia);

            TipologiaFilettatura threadType = null;

            switch (tipologia)
            {
                case 0:
                    {
                        threadType = new TipologiaMetrica();

                    } break;

                case 1:
                    {
                        threadType = new TipologiaInPollici();
                    } break;

                default:
                    throw new NotImplementedException();

                    break;
            }

            threadType.Descrizione = "<new>";

            _tabellaFilettature.Filettature.Add(threadType);

            OnPropertyChanged("TipologieFilettatura");

        }

        public ICommand NewThreadTypeCmd
        {
            get
            {
                return _addThreadCmd ?? (_addThreadCmd = new RelayCommand(param => AddThreadType((string)param),
                                                                          param => true));
            }
        }

        #endregion

        #region New Row

        RelayCommand _addThreadRowCmd;
        /// <summary>
        /// Salva modifiche database
        /// </summary>
        private void AddThreadRow()
        {
            if (TipoMaschiaturaSelezionato == null) return;

            TipoMaschiaturaSelezionato.AddRow();

            OnPropertyChanged("TipoMaschiaturaSelezionato");
        }

        public bool CanAddThreadRow
        {
            get { return TipoMaschiaturaSelezionato != null; }
        }

        public ICommand AddThreadRowCmd
        {
            get
            {
                return _addThreadRowCmd ?? (_addThreadRowCmd = new RelayCommand(param => AddThreadRow(),
                                                                                param => CanAddThreadRow));
            }
        }

        #endregion

        /*
         * servono i viewModel per 
         * le filettature
         */

        //#region Del Tool

        //RelayCommand _delToolCmd;

        ///// <summary>
        ///// Salva modifiche database
        ///// </summary>
        //public void DeleteThreadType()
        //{
        //    //..

        //}

        //Boolean CanDelThreadType
        //{
        //    get
        //    {
        //        return TipoMaschiaturaSelezionato != null &&
        //            TipoMaschiaturaSelezionato.Filettature.Count == 0;
        //    }
        //}

        //public ICommand DelToolCmd
        //{
        //    get
        //    {
        //        return _delToolCmd ?? (_delToolCmd = new RelayCommand(param => DeleteThreadType(),
        //                                                              param => CanDelThreadType));
        //    }
        //}

        //#endregion
        public void Save(IMainViewModel viewModel)
        {
            /*
             * todo :  prende filetteatura
             */
            PathFolderHelper.SaveTabellaFilettatura(_tabellaFilettature);
        }

    }

    /// <summary>
    /// Classi view model per tipologia filettatura
    /// </summary>
    public class FilettaturaViewModel : ViewModelBase
    {
        public string Descrizione
        {
            get { return Source.Descrizione; }

            set
            {
                Source.Descrizione = value;
                OnPropertyChanged("Descrizione");
            }
        }

        public double CoefficentOd
        {
            get { return Source.FattorePerDiametroEsterno; }

            set
            {
                Source.FattorePerDiametroEsterno = value;
                OnPropertyChanged("CoefficentOd");
            }
        }

        public double CoefficentId
        {
            get { return Source.FattorePerDiametroInterno; }

            set
            {
                Source.FattorePerDiametroInterno = value;
                OnPropertyChanged("CoefficentId");
            }
        }

        protected MeasureUnit MeasureUnit;

        public TipologiaFilettatura Source { get; private set; }

        public static FilettaturaViewModel GetViewModel(TipologiaFilettatura ttype, MeasureUnit measureUnit)
        {
            if (ttype is TipologiaMetrica)
                return new FilettaturaMetricaViewModel(ttype as TipologiaMetrica, measureUnit);

            if (ttype is TipologiaInPollici)
                return new FilettaturaInPolliciViewModel(ttype as TipologiaInPollici, measureUnit);

            throw new NotImplementedException();
        }

        public ObservableCollection<RigaFilettaturaViewModel> RigheTabella
        {
            get
            {
                var ttype = Source.RigheTabella.OrderBy(l => l.DiametroMetrico);

                var rslt = new ObservableCollection<RigaFilettaturaViewModel>();

                foreach (var viewModelBase in ttype)
                {
                    rslt.Add(GetViewModel(viewModelBase, MeasureUnit));
                }

                return rslt;

            }
        }

        private static RigaFilettaturaViewModel GetViewModel(RigaTabellaFilettatura ttype, MeasureUnit measureUnit)
        {
            if (ttype is RigaTabellaFilettaturaMetrica)
                return new RigaFilettaturaMetricaViewModel(ttype as RigaTabellaFilettaturaMetrica, measureUnit);

            if (ttype is RigaTabellaFilettaturaInPollici)
                return new RigaFilettaturaInPolliciViewModel(ttype as RigaTabellaFilettaturaInPollici, measureUnit);

            throw new NotImplementedException();
        }

        protected FilettaturaViewModel(TipologiaFilettatura tipologiaFilettatura, MeasureUnit measureUnit)
        {
            MeasureUnit = measureUnit;

            Source = tipologiaFilettatura;
        }

        internal void AddRow()
        {
            var row = Source.CreateRigaTabella();

            Source.RigheTabella.Add(row);

            OnPropertyChanged("RigheTabella");
        }
    }

    public class FilettaturaInPolliciViewModel : FilettaturaViewModel
    {

        public FilettaturaInPolliciViewModel(TipologiaInPollici inPollici, MeasureUnit measureUnit)
            : base(inPollici, measureUnit)
        {

        }
    }

    public class FilettaturaMetricaViewModel : FilettaturaViewModel
    {
        public FilettaturaMetricaViewModel(TipologiaMetrica metrica, MeasureUnit measureUnit)
            : base(metrica, measureUnit)
        {

        }
    }

    public static class MmInchConverter
    {
        /// <summary>
        /// Metodo utilizzato quando software chiede valore .
        /// Se in mm ok. perchè memorizzo valori in mm
        /// Se in Inch divido per 25.4
        /// </summary>
        /// <param name="v"></param>
        /// <param name="measureUnit"></param>
        /// <returns></returns>
        public static double GetValue(double v, MeasureUnit measureUnit)
        {
            if (measureUnit == MeasureUnit.Millimeter)
                return v;

            return v / 25.4;
        }

        /// <summary>
        /// Metodo utilizzato per salvare valori.
        /// Se valori è in mm ok . 
        /// Se valore è in inch moltiplico per 25.4
        /// </summary>
        /// <param name="v"></param>
        /// <param name="measureUnit"></param>
        /// <returns></returns>
        public static double SetValue(double v, MeasureUnit measureUnit)
        {
            if (measureUnit == MeasureUnit.Millimeter)
                return v;

            return v * 25.4;
        }

    }
    /// <summary>
    /// Layer ViewModel per le righe della filettatura
    /// </summary>
    public class RigaFilettaturaViewModel : ViewModelBase
    {
        public RigaTabellaFilettatura Source { get; private set; }

        /*
         * Memorizzo valori in mm.
         * 
         * restituisco valori a seconda dell'unita scelta ( mm o inch )
         */
        protected readonly MeasureUnit MeasureUnit;

        public RigaFilettaturaViewModel(RigaTabellaFilettatura riga, MeasureUnit measureUnit)
        {
            MeasureUnit = measureUnit;

            Source = riga;
        }

    }



    public class RigaFilettaturaMetricaViewModel : RigaFilettaturaViewModel
    {
        public double Diametro
        {
            get
            {
                return MmInchConverter.GetValue(Source.DiametroMetrico, MeasureUnit);
            }
            set
            {
                Source.DiametroMetrico = MmInchConverter.SetValue(value, MeasureUnit);
                OnPropertyChanged("Diametro");
            }
        }

        public double Passo
        {
            get
            {
                return MmInchConverter.GetValue(Source.Passo, MeasureUnit);
            }
            set
            {
                Source.Passo = MmInchConverter.SetValue(value, MeasureUnit);
                OnPropertyChanged("Passo");
            }
        }

        public double Preforo
        {
            get { return MmInchConverter.GetValue(Source.Preforo, MeasureUnit); }
            set
            {
                Source.Preforo = MmInchConverter.SetValue(value, MeasureUnit);
                OnPropertyChanged("Preforo");
            }
        }
        public RigaFilettaturaMetricaViewModel(RigaTabellaFilettatura metrica, MeasureUnit measureUnit)
            : base(metrica, measureUnit)
        {

        }
    }

    public class RigaFilettaturaInPolliciViewModel : RigaFilettaturaViewModel
    {
        public RigaFilettaturaInPolliciViewModel(RigaTabellaFilettatura metrica, MeasureUnit measureUnit)
            : base(metrica, measureUnit)
        {

        }
    }

}
