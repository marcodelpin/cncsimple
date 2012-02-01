using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

/*
 * fare classe comune con screen ,
 * 
 * non dovrei ripetere il il collegamento , per riaggiornare 
 
 */
namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Spianatura
{
    public class SpianaturaParametriViewModel : EditStageTreeViewItem, IDataErrorInfo
    {
        private readonly Model.ConversationalStructure.Lavorazioni.Fresatura.Spianatura _spianatura;

        public SpianaturaParametriViewModel(EditWorkViewModel parent, Model.ConversationalStructure.Lavorazioni.Fresatura.Spianatura spianatura)
            : base("Face Milling Parameter", parent)
        {
            _spianatura = spianatura;

            EditWorkParent = parent;
        }

        public Dictionary<byte, string> StartPointLookup
        {
            get
            {
                var lookup = new Dictionary<byte, string>
                                 {
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.UpLeft, "1"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.UpRight, "2"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.DownLeft, "3"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.DownRight, "4"},
                                     {(byte)SquareShapeHelper.SquareShapeStartPoint.Center, "5"},

                                 };

                return lookup;

            }
        }

        public Dictionary<byte, string> MillingStrategy
        {
            get
            {
                var lookup = new Dictionary<byte, string>
                                 {
                                     {0, "Traditional"},
                                     {1, "Spiral"},

                                 };

                return lookup;

            }
        }

        public byte FinishSelectedStrategy
        {
            get
            {
                return (byte)_spianatura.ModoFinitura;
            }
            set
            {
                _spianatura.ModoFinitura = (SpiantaturaMetodologia)value;
                OnPropertyChanged("FinishSelectedStrategy");
            }
        }

        public byte RoughingSelectedStrategy
        {
            get
            {
                return (byte)_spianatura.ModoSgrossatura;
            }
            set
            {
                _spianatura.ModoSgrossatura = (SpiantaturaMetodologia)value;
                OnPropertyChanged("RoughingSelectedStrategy");
            }
        }

        public byte SelectedStartPoint
        {
            get
            {
                return (byte)_spianatura.StartPoint;
            }
            set
            {
                _spianatura.StartPoint = (SquareShapeHelper.SquareShapeStartPoint)value;
                OnPropertyChanged("SelectedStartPoint");
            }
        }

        //public SpianaturaParametriViewModel(EditWorkViewModel parent, Model.ConversationalStructure.Lavorazioni.Fresatura.Spianatura spianatura) :
        //    base("Face Milling Parameter", parent)
        //{
        //    parent.Children.Add(this);

        //    _spianatura = spianatura;
        //}

        public double Sovrametallo
        {
            get { return _spianatura.Sovrametallo; }
            set
            {
                _spianatura.Sovrametallo = value;
                OnPropertyChanged("Sovrametallo");
                SourceUpdated();

            }
        }
        public double SovrametalloFinitura
        {
            get { return _spianatura.SovrametalloPerFinitura; }
            set
            {
                _spianatura.SovrametalloPerFinitura = value;
                OnPropertyChanged("SovrametalloFinitura");
                SourceUpdated();

            }
        }

        public double SicurezzaZ
        {
            get { return _spianatura.SicurezzaZ; }
            set
            {
                _spianatura.SicurezzaZ = value;
                OnPropertyChanged("SicurezzaZ");
                SourceUpdated();

            }
        }

        public double InizioZ
        {
            get { return _spianatura.LivelloZ; }
            set
            {
                _spianatura.LivelloZ = value;
                OnPropertyChanged("InizioZ");
                SourceUpdated();

            }
        }

        public double Altezza
        {
            get { return _spianatura.Altezza; }
            set
            {
                _spianatura.Altezza = value;
                OnPropertyChanged("Altezza");
                SourceUpdated();

            }
        }

        public double Larghezza
        {
            get { return _spianatura.Larghezza; }
            set
            {
                _spianatura.Larghezza = value;
                OnPropertyChanged("Larghezza");
                SourceUpdated();

            }
        }

        public double OffsetCentroY
        {
            get { return _spianatura.PuntoStartY; }
            set
            {
                _spianatura.PuntoStartY = value;
                OnPropertyChanged("OffsetCentroY");
                SourceUpdated();

            }
        }

        public double OffsetCentroX
        {
            get { return _spianatura.PuntoStartX; }
            set
            {
                _spianatura.PuntoStartX = value;
                OnPropertyChanged("OffsetCentroX");
                SourceUpdated();

            }
        }

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        public override bool? ValidateStage()
        {
            return ValidatedProperties.All(property => GetValidationError(property) == null);
        }

        protected string[] ValidatedProperties = {       
                                                    "Larghezza",
                                                    "Sovrametallo",
                                                    "SovrametalloFinitura",
                                                    "SicurezzaZ",
                                                    "InizioZ",
                                                    "Altezza",
                                                    "Larghezza",
                                                    "OffsetCentroY",
                                                    "OffsetCentroX",
                                                 };

        protected string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {

                case "SovrametalloFinitura":
                    {
                        error = InputCheck.MaggioreOUgualeDiZero(SovrametalloFinitura);
                    }
                    break;

                case "Sovrametallo":
                    {
                        error = InputCheck.MaggioreDiZero(Sovrametallo);
                    }
                    break;

                case "SicurezzaZ":
                    {
                        if (InizioZ + Sovrametallo > SicurezzaZ)
                            error = "Z Secure Level Too Low";
                    }
                    break;
                case "InizioZ":
                    {

                    }
                    break;

                case "Altezza":
                    {
                        error = InputCheck.MaggioreDiZero(Altezza);

                    } break;

                case "Larghezza":
                    {
                        error = InputCheck.MaggioreDiZero(Larghezza);

                    } break;


                case "OffsetCentroY":
                    {
                    } break;

                case "OffsetCentroX":
                    {
                    } break;

                default:
                    Debug.Fail("Unexpected property : " + propertyName);
                    break;
            }

            return error;
        }

        #endregion
    }
}
