using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel
{
    public abstract class RawItemViewModel : ViewModelBase
    {
        public static RawItemViewModel GetViewModel(RawEntity2D rawEntity2D, ProfileEditorViewModel.AxisSystem axisSystem)
        {
            if (rawEntity2D is RawInitPoint2D)
                return new RawInitPointViewModel(rawEntity2D as RawInitPoint2D, axisSystem);

            if (rawEntity2D is RawLine2D)
                return new RawLineViewModel(rawEntity2D as RawLine2D, axisSystem);

            if (rawEntity2D is RawArc2D)
                return new RawArcViewModel(rawEntity2D as RawArc2D, axisSystem);

            throw new NotImplementedException("RawItemViewModel.GetViewModel");
        }
        public RawEntity2D.RawEntityOrientation Orientation
        {
            get
            {
                return RawEntity.Orientation;
            }
        }

        public event EventHandler OnSourceUpdated;

        protected void RequestUpdateSource()
        {
            EventHandler handler = this.OnSourceUpdated;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public void UpdateOrientation()
        {
            OnPropertyChanged("Orientation");
        }
        public bool IsSelected
        {
            get { return RawEntity.IsSelected; }
            set
            {
                RawEntity.IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public RawEntity2D RawEntity { private set; get; }

        protected RawItemViewModel(RawEntity2D rawEntity2D, ProfileEditorViewModel.AxisSystem axisSystem)
        {
            RawEntity = rawEntity2D;

            switch (axisSystem)
            {
                case ProfileEditorViewModel.AxisSystem.Xy:
                    {
                        XLabel = "X";
                        YLabel = "Y";
                        DeltaXLabel = "Incre. X";
                        DeltaYLabel = "Incre. Y";
                        CenterXLabel = "Center X";
                        CenterYLabel = "Center Y";
                    } break;

                case ProfileEditorViewModel.AxisSystem.Xz:
                    {
                        XLabel = "Z";
                        YLabel = "X";
                        DeltaXLabel = "Incre. Z";
                        DeltaYLabel = "Incre. X";
                        CenterXLabel = "Center X";
                        CenterYLabel = "Center C";
                    } break;
            }
        }

        protected List<RawInputViewModel> InputVmList = new List<RawInputViewModel>();

        #region Label Change 
        private string _xLabel;
        public string XLabel
        {
            get { return _xLabel; }

            set
            {
                _xLabel = value;
                OnPropertyChanged("XLabel");
            }
        }

        private string _yLabel;
        public string YLabel
        {
            get { return _yLabel; }

            set
            {
                _yLabel = value;
                OnPropertyChanged("YLabel");
            }
        }

        private string _centerXLabel;
        public string CenterXLabel
        {
            get { return _centerXLabel; }

            set
            {
                _centerXLabel = value;
                OnPropertyChanged("CenterXLabel");
            }
        }

        private string _centerYLabel;
        public string CenterYLabel
        {
            get { return _centerYLabel; }

            set
            {
                _centerYLabel = value;
                OnPropertyChanged("CenterYLabel");
            }
        }

        private string _deltaXLabel;
        public string DeltaXLabel
        {
            get { return _deltaXLabel; }

            set
            {
                _deltaXLabel = value;
                OnPropertyChanged("DeltaXLabel");
            }
        }

        private string _deltaYLabel;
        public string DeltaYLabel
        {
            get { return _deltaYLabel; }

            set
            {
                _deltaYLabel = value;
                OnPropertyChanged("DeltaYLabel");
            }
        }
#endregion

        internal void UpdateIsUserInputedAndValue()
        {
            foreach (var rawInputViewModel in InputVmList)
            {
                rawInputViewModel.UpdateVm();
            }
        }

        internal void AddToUpdateList(RawInputViewModel rawInputViewModel)
        {
            InputVmList.Add(rawInputViewModel);
        }
    }
}
