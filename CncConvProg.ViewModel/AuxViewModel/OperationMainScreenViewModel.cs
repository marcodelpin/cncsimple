using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Implementors.Wpf.MVVM;

namespace CncConvProg.ViewModel.AuxViewModel
{
    public class OperationMainScreenViewModel : ViewModelBase
    {
        public readonly Operazione Operazione;

        public OperationMainScreenViewModel(Operazione operazione)
        {
            Operazione = operazione;

        }

        public int OrderIndex
        {
            get { return Operazione.PhaseOperationListPosition; }

            set { Operazione.PhaseOperationListPosition = value; }
        }

        public string OperationTime
        {
            get { return Operazione.CycleTimeString; }
        }

        public string Descrizione
        {
            get { return Operazione.NumberedDescription; }
        }

        public string ToolLabel
        {
            get { return Operazione.Utensile.ToolDescription; }
        }

        public int NumeroUtensile
        {
            get { return Operazione.GetToolPosition(); }
        }

        public string CorrettoreDiametro
        {
            get { return Operazione.GetToolDiameterCorrecto(); }
        }

        public string CorrettoreLunghezza
        {
            get { return Operazione.GetToolHeightCorrector(); }
        }

        public void SetOptionalToolChange(bool value)
        {
            Operazione.ToolChangeOptional = value;

            OnPropertyChanged("IsToolChangeOptional");
        }
        public Visibility IsToolChangeOptional
        {
            get
            {
                if (Operazione.ToolChangeOptional)
                {
                    return Visibility.Visible;
                }

                return Visibility.Hidden;
            }
        }

        public bool ForceToolChange
        {
            get { return Operazione.ForceToolChange; }

            set
            {
                Operazione.ForceToolChange = value;
                OnPropertyChanged("ForceToolChange");
            }
        }

        public LavorazioniEnumOperazioni OperationType
        {
            get { return Operazione.OperationType; }
        }

        internal void UpdateTime()
        {
            OnPropertyChanged("OperationTime");
        }

    }
}
