using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Common
{
    public class RotoTraslateWorkViewModel : ViewModelBase
    {
        private readonly Lavorazione _lavorazione;
        private readonly EditStageTreeViewItem _parentViewModel;

        public RotoTraslateWorkViewModel(Lavorazione lavorazione, EditStageTreeViewItem parent)
        {
            _lavorazione = lavorazione;
            _parentViewModel = parent;

        }
        /* Da qui comincia rot e trans*/
        public bool RotationAbilited
        {
            get { return _lavorazione.RotationAbilited; }
            set
            {
                _lavorazione.RotationAbilited = value;

                if (value)
                    TranslateAbilited = false;

                OnPropertyChanged("RotationAbilited");
                _parentViewModel.SourceUpdated();

            }
        }

        public double CenterRotationX
        {
            get { return _lavorazione.CenterRotationX; }
            set
            {
                _lavorazione.CenterRotationX = value;
                OnPropertyChanged("CenterRotationX");
                _parentViewModel.SourceUpdated();

            }
        }

        public double CenterRotationY
        {
            get { return _lavorazione.CenterRotationY; }
            set
            {
                _lavorazione.CenterRotationY = value;
                OnPropertyChanged("CenterRotationY");
                _parentViewModel.SourceUpdated();

            }
        }

        public double FirstAngle
        {
            get { return _lavorazione.FirstAngle; }
            set
            {
                _lavorazione.FirstAngle = value;
                OnPropertyChanged("FirstAngle");
                _parentViewModel.SourceUpdated();

            }
        }

        public int NumberInstance
        {
            get { return _lavorazione.NumberInstance; }
            set
            {
                _lavorazione.NumberInstance = value;
                OnPropertyChanged("NumberInstance");
                _parentViewModel.SourceUpdated();

            }
        }


        /* ----*/
        public bool TranslateAbilited
        {
            get { return _lavorazione.TranslateAbilited; }
            set
            {
                _lavorazione.TranslateAbilited = value;

                if (value)
                    RotationAbilited = false;

                OnPropertyChanged("TranslateAbilited");
                _parentViewModel.SourceUpdated();

            }
        }

        public double TransStepX
        {
            get { return _lavorazione.TransStepX; }
            set
            {
                _lavorazione.TransStepX = value;
                OnPropertyChanged("TransStepX");
                _parentViewModel.SourceUpdated();

            }
        }

        public double TransCountX
        {
            get { return _lavorazione.TransCountX; }
            set
            {
                _lavorazione.TransCountX = value;
                OnPropertyChanged("TransCountX");
                _parentViewModel.SourceUpdated();

            }
        }

        public double TransStepY
        {
            get { return _lavorazione.TransStepY; }
            set
            {
                _lavorazione.TransStepY = value;
                OnPropertyChanged("TransStepY");
                _parentViewModel.SourceUpdated();

            }
        }

        public int TransCountY
        {
            get { return _lavorazione.TransCountY; }
            set
            {
                _lavorazione.TransCountY = value;
                OnPropertyChanged("TransCountY");
                _parentViewModel.SourceUpdated();

            }
        }

    }
}
