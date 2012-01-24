using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.ThreadTable;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Filettatura
{
    public class FresaturaFilettaturaParametriViewModel : EditStageTreeViewItem, IDataErrorInfo, IValid
    {
        private readonly FresaturaFilettatura _fresaturaFilettatura;

        public FresaturaFilettaturaParametriViewModel(FresaturaFilettatura fresaturaFilettatura, EditWorkViewModel treeItemParent)
            : base("Parameter", treeItemParent)
        {
            _fresaturaFilettatura = fresaturaFilettatura;

            //RotoTraslateWorkViewModel = new RotoTraslateWorkViewModel(this._fresaturaCava, this);
        }

        //private RotoTraslateWorkViewModel _rotoTraslateWorkViewModel;
        //public RotoTraslateWorkViewModel RotoTraslateWorkViewModel
        //{
        //    get { return _rotoTraslateWorkViewModel; }
        //    set
        //    {
        //        _rotoTraslateWorkViewModel = value;
        //        OnPropertyChanged("RotoTraslateWorkViewModel");
        //    }
        //}
        public bool IsEsterna
        {
            get
            {
                return _fresaturaFilettatura.FilettaturaEsterna;
            }
            set
            {
                _fresaturaFilettatura.FilettaturaEsterna = value;

                OnPropertyChanged("IsEsterna");
                OnPropertyChanged("MaschiaturaSelezionata");
               
            }
        }

        public bool IsLeftHand
        {
            get
            {
                return _fresaturaFilettatura.FilettaturaSinistra;
            }
            set
            {
                _fresaturaFilettatura.FilettaturaSinistra = value;
                OnPropertyChanged("IsLeftHand");
            }
        }

        public bool OverrideParameter
        {
            get
            {
                return _fresaturaFilettatura.ParametriFilettaturaPersonalizzati;
            }
            set
            {
                _fresaturaFilettatura.ParametriFilettaturaPersonalizzati = value;
                OnPropertyChanged("OverrideParameter");
            }
        }


        public double Profondita
        {
            get
            {
                return _fresaturaFilettatura.ProfonditaLavorazione;
            }
            set
            {
                _fresaturaFilettatura.ProfonditaLavorazione = value;
                OnPropertyChanged("Profondita");
            }
        }


        public double PassoMetrico
        {
            get
            {
                return _fresaturaFilettatura.PassoMetrico;
            }
            set
            {
                _fresaturaFilettatura.PassoMetrico = value;
                OnPropertyChanged("PassoMetrico");
            }
        }

        public double DiametroMetrico
        {
            get
            {
                return _fresaturaFilettatura.DiametroMetricoFinale;
            }
            set
            {
                _fresaturaFilettatura.DiametroMetricoFinale = value;
                OnPropertyChanged("DiametroMetrico");
            }
        }

        public double SicurezzaZ
        {
            get { return _fresaturaFilettatura.SicurezzaZ; }
            set
            {
                _fresaturaFilettatura.SicurezzaZ = value;
                OnPropertyChanged("SicurezzaZ");
            }
        }

        public double InizioZ
        {
            get { return _fresaturaFilettatura.InizioLavorazioneZ; }
            set
            {
                _fresaturaFilettatura.InizioLavorazioneZ = value;
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
                    var tf = PathFolderHelper.GetFilettatureList();

                    _listaMaschiature = new ObservableCollection<RigaTabellaFilettatura>(tf);
                }
                return _listaMaschiature;
            }
        }
        public RigaTabellaFilettatura MaschiaturaSelezionata
        {
            get
            {
                if (_fresaturaFilettatura.MaschiaturaSelezionata != null)
                {
                    DiametroMetrico = _fresaturaFilettatura.MaschiaturaSelezionata.GetDiametroFinale(IsEsterna);

                    PassoMetrico = _fresaturaFilettatura.MaschiaturaSelezionata.Passo;
                }

                return _fresaturaFilettatura.MaschiaturaSelezionata;
            }

            set
            {
                _fresaturaFilettatura.MaschiaturaSelezionata = value;

                //if (_fresaturaFilettatura.MaschiaturaSelezionata != null)
                //{
                //    DiametroMetrico = _fresaturaFilettatura.MaschiaturaSelezionata.GetDiametroFinale(IsEsterna);

                //    PassoMetrico = _fresaturaFilettatura.MaschiaturaSelezionata.Passo;
                //}

                OnPropertyChanged("MaschiaturaSelezionata");
            }
        }

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid
        {
            get { return ValidatedProperties.All(property => GetValidationError(property) == null); }
        }


        protected string[] ValidatedProperties = {
                                                     "Profondita",
                                                     "SicurezzaZ"
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "Profondita":
                    {
                        error = InputCheck.MaggioreDiZero(Profondita.ToString());
                    }
                    break;

                case "SicurezzaZ":
                    {
                        if (SicurezzaZ <= InizioZ)
                            error = "Must be higher than StartZ";
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
