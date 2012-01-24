using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel
{
    public class RawEntityViewModel : ViewModelBase
    {
        public RawEntity2D.RawEntityOrientation Orientation
        {
            get
            {
                return Entity.Orientation;
            }
        }

        public void UpdateOrientation()
        {
            OnPropertyChanged("Orientation");
        }
        public bool IsSelected
        {
            get { return Entity.IsSelected; }
            set
            {
                Entity.IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public readonly RawEntity2D Entity;

        public RawEntityViewModel(RawEntity2D rawEntity)
        {
            Entity = rawEntity;
        }
    }
}
