using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel
{
    public abstract class RawItemViewModel : ViewModelBase
    {

        public Dictionary<Key, Key> KeyboardDictionary
        {
            get { return Parent.DictionaryKey; }
        }

        public readonly ProfileEditorViewModel Parent;

        protected RawItemViewModel(RawEntity2D rawEntity2D, ProfileEditorViewModel.AxisSystem axisSystem, ProfileEditorViewModel parent)
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
            this.Parent = parent;
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

        public string GetImageUri(object p)
        {

            var t = p.ToString();



            t = t.ToLower();

            switch (Parent.CurrentAxisSystem)
            {

                case ProfileEditorViewModel.AxisSystem.Xz:
                    {

                        switch (t)
                        {
                            case "x":
                                return @"pack://application:,,,/CncConvProg.View;component/Images/gui/Z.png";
                            case "y":
                                return @"pack://application:,,,/CncConvProg.View;component/Images/gui/keyX.png";
                            case "v":
                                return @"pack://application:,,,/CncConvProg.View;component/Images/gui/keyU.png";
                            case "u":
                                return @"pack://application:,,,/CncConvProg.View;component/Images/gui/W.png";
                            default:
                                return string.Empty;
                        }
                    } break;

                //default:
                //case ProfileEditorViewModel.AxisSystem.Xy:
                //    switch (t)
                //    {
                //        case "x":
                //            return @"pack://application:,,,/CncConvProg.View;component/Images/gui/keyX.png";
                //        case "y":
                //            return @"pack://application:,,,/CncConvProg.View;component/Images/gui/keyY.png";
                //        default:
                //            return string.Empty;
                //    }
            }

            return string.Empty;

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
