using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Operation;
using CncConvProg.Model.Tool;
using CncConvProg.ViewModel.CommonViewModel.ParameterViewModels;
using CncConvProg.ViewModel.CommonViewModel.ToolViewModels;
using CncConvProg.ViewModel.EditWorkDialog.OperationViewModel.ToolHolder;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.OperationViewModel
{
    public class OperazioneTrocoidaleViewModel : OperazioneViewModel
    {
        private readonly OperazioneFresaturaTrocoidale _operazione;

        public OperazioneTrocoidaleViewModel(OperazioneFresaturaTrocoidale operazione, EditStageTreeViewItem parent)
            : base(operazione, parent)
        {
            _operazione = operazione;
        }

        public double ProfonditaPassataPerc
        {
            get { return _operazione.ProfonditaPassataPerc; }

            set
            {
                _operazione.ProfonditaPassataPerc = value;
                OnPropertyChanged("ProfonditaPassataPerc");
            }
        }

        public double Avanzamento
        {
            get { return _operazione.Feed; }

            set
            {
                _operazione.Feed = value;
                OnPropertyChanged("Avanzamento");
                OnPropertyChanged("AvanzamentoCentroUtensile");

    
            }
        }

        public double NumeroGiri
        {
            get { return _operazione.NumeroGiri; }

            set
            {
                _operazione.NumeroGiri = value;
                OnPropertyChanged("NumeroGiri");
            }
        }
        public double AvanzamentoCentroUtensile
        {
            get { return _operazione.AvanzamentoCentroUtensile; }
        }

        public double GrooveWidth
        {
            get
            {
                return _operazione.GrooveWidth;
            }
        }

        public double StepPerc
        {
            get { return _operazione.StepPerc; }

            set
            {
                _operazione.StepPerc = value;
                OnPropertyChanged("StepPerc");
            }
        }

        /// <summary>
        /// Ovverride della proprità is valid
        /// Su questa operazione non importa ParameterViewModel.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                return ToolHolderVm.IsValid && UtensileViewModel.IsValid;
            }
        }

    }


}

