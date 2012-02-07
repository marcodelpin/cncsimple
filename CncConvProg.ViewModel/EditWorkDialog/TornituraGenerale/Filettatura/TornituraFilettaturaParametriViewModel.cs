using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura;
using CncConvProg.Model.ThreadTable;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.TornituraGenerale.Filettatura
{
    public class TornituraFilettaturaParametriViewModel : EditStageTreeViewItem
    {
        private readonly TornituraFilettatura _filettatura;

        public TornituraFilettaturaParametriViewModel(TornituraFilettatura filettatura, EditWorkViewModel treeItemParent)
            : base("Parameter", treeItemParent)
        {
            _filettatura = filettatura;
        }

        public double LunghezzaFiletto
        {
            get { return _filettatura.LunghezzaFiletto; }
            set
            {
                if (_filettatura.LunghezzaFiletto == value) return;
                _filettatura.LunghezzaFiletto = value;
                OnPropertyChanged("LunghezzaFiletto");
            }
        }

        public double ZIniziale
        {
            get { return _filettatura.ZIniziale; }
            set
            {
                if (_filettatura.ZIniziale == value) return;
                _filettatura.ZIniziale = value;
                OnPropertyChanged("InizioZ");
            }
        }

        private ObservableCollection<RigaTabellaFilettatura> _listaMaschiature;
        public ObservableCollection<RigaTabellaFilettatura> ListaMaschiature
        {
            get
            {
                if (_listaMaschiature == null)
                {
                    var tf = Singleton.Data.GetFilettatureList();

                    _listaMaschiature = new ObservableCollection<RigaTabellaFilettatura>(tf);
                }
                return _listaMaschiature;
            }
        }
        public RigaTabellaFilettatura MaschiaturaSelezionata
        {
            get
            {
                if (_filettatura.MaschiaturaSelezionata != null)
                {
                    Passo = _filettatura.MaschiaturaSelezionata.Passo;
                    NumeroPassate = _filettatura.MaschiaturaSelezionata.NumeroPassate;
                }

                return _filettatura.MaschiaturaSelezionata;
            }

            set
            {
                _filettatura.MaschiaturaSelezionata = value;

                OnPropertyChanged("MaschiaturaSelezionata");
            }
        }
        public double Passo
        {
            get { return _filettatura.Passo; }
            set
            {
                if (_filettatura.Passo == value) return;
                _filettatura.Passo = value;
                OnPropertyChanged("Passo");
            }
        }
        public int NumeroPassate
        {
            get { return _filettatura.NumeroPassate; }
            set
            {
                if (_filettatura.NumeroPassate == value) return;
                _filettatura.NumeroPassate = value;
                OnPropertyChanged("NumeroPassate");
            }
        }



        #region IDataErrorInfo Members

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid
        {
            get { return ValidatedProperties.All(property => GetValidationError(property) == null); }
        }

        public override bool? ValidateStage()
        {
            return null;
        }


        protected string[] ValidatedProperties = {
                                                     "Sovrametallo",
                                                     "Profondita",
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Diametro":
                    {
                    }
                    break;

                case "Sovrametallo":
                    {
                    }
                    break;

                case "Profondita":
                    {
                    }
                    break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion
    }
}
