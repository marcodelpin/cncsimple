using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.ThreadTable;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.ParameterScreen
{
    public class MaschiaturaParametriViewModel : CommonDrillParametriViewModel
    {
        private readonly Maschiatura _maschiatura;

        public MaschiaturaParametriViewModel(EditWorkViewModel parent, Maschiatura maschiatura)
            : base(parent, maschiatura, "Tapping")
        {
            _maschiatura = maschiatura;

            EditWorkParent = parent;
        }

        public double ProfonditaMaschiatura
        {
            get
            {
                return _maschiatura.ProfonditaMaschiatura;
            }
            set
            {
                _maschiatura.ProfonditaMaschiatura = value;
                OnPropertyChanged("ProfonditaMaschiatura");
            }
        }

        public bool IsLeftHand
        {
            get
            {
                return _maschiatura.FilettaturaSinistra;
            }
            set
            {
                _maschiatura.FilettaturaSinistra = value;
                OnPropertyChanged("IsLeftHand");
            }
        }

        public bool OverrideParameter
        {
            get
            {
                return _maschiatura.ParametriFilettaturaPersonalizzati;
            }
            set
            {
                _maschiatura.ParametriFilettaturaPersonalizzati = value;
                OnPropertyChanged("OverrideParameter");
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
                if (_maschiatura.MaschiaturaSelezionata != null)
                {
                    PassoMetrico = _maschiatura.MaschiaturaSelezionata.Passo;
                }

                return _maschiatura.MaschiaturaSelezionata;
            }

            set
            {
                _maschiatura.MaschiaturaSelezionata = value;

                //if (_fresaturaFilettatura.MaschiaturaSelezionata != null)
                //{
                //    DiametroMetrico = _fresaturaFilettatura.MaschiaturaSelezionata.GetDiametroFinale(IsEsterna);

                //    PassoMetrico = _fresaturaFilettatura.MaschiaturaSelezionata.Passo;
                //}

                OnPropertyChanged("MaschiaturaSelezionata");
            }
        }

        public double PassoMetrico
        {
            get
            {
                return _maschiatura.PassoMetrico;
            }
            set
            {
                _maschiatura.PassoMetrico = value;
                OnPropertyChanged("PassoMetrico");
            }
        }
        protected override string[] ValidatedProperties
        {
            get
            {
                var o = new List<string>
                {
                    //"SicurezzaZ",
                    //"InizioZ"
                };

                foreach (var validatedProperty in base.ValidatedProperties)
                {
                    o.Add(validatedProperty);
                }

                return o.ToArray();
            }
        }



        protected override string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                // other properties..
                default:
                    error = base.GetValidationError(propertyName);
                    break;
            }

            return error;
        }
    }
}
