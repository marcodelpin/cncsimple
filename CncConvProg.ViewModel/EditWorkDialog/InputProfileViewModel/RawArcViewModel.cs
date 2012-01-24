using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Implementors.Wpf.MVVM;

namespace CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel
{
    public class RawArcViewModel : RawItemViewModel
    {
        private readonly RawArc2D _rawArc;

        public RawArc2D Source { get { return _rawArc; } }

        public RawArcViewModel(RawArc2D rawLine2D, ProfileEditorViewModel.AxisSystem axisSystem)
            : base(rawLine2D, axisSystem)
        {
            _rawArc = rawLine2D;

            X = new RawInputViewModel(_rawArc.X, this);
            Y = new RawInputViewModel(_rawArc.Y, this);
            CenterX = new RawInputViewModel(_rawArc.CenterX, this);
            CenterY = new RawInputViewModel(_rawArc.CenterY, this);
            DeltaX = new RawInputViewModel(_rawArc.DeltaX, this);
            DeltaY = new RawInputViewModel(_rawArc.DeltaY, this);
            Chamfer = new RawInputViewModel(_rawArc.Chamfer, this);
            Radius = new RawInputViewModel(_rawArc.Radius, this);
            EndRadius = new RawInputViewModel(_rawArc.EndRadius, this);
            foreach (var rawInputViewModel in InputVmList)
            {
                rawInputViewModel.OnSourceUpdated += RawLine_SourceUpdated;
            }

            //X.OnSourceUpdated += RawLine_SourceUpdated;
            //Y.OnSourceUpdated += RawLine_SourceUpdated;
            //DeltaX.OnSourceUpdated += RawLine_SourceUpdated;
            //DeltaY.OnSourceUpdated += RawLine_SourceUpdated;
            //Chamfer.OnSourceUpdated += RawLine_SourceUpdated;
            //Radius.OnSourceUpdated += RawLine_SourceUpdated;
            //EndRadius.OnSourceUpdated += RawLine_SourceUpdated;

        }

        //public RawArcViewModel(RawArc2D rawArc2D, ProfileEditorViewModel.AxisSystem _axisSystem)
        //{
        //    // TODO: Complete member initialization
        //    this.rawArc2D = rawArc2D;
        //    this._axisSystem = _axisSystem;
        //}

        void RawLine_SourceUpdated(object sender, EventArgs e)
        {
            RequestUpdateSource();
        }

        public bool DirectionClockwise
        {
            get { return _rawArc.IsClockwise; }
            set
            {
                _rawArc.IsClockwise = value;
                RequestUpdateSource();
                OnPropertyChanged("DirectionClockwise");
            }
        }
   
        public bool AlternateArc
        {
            get { return _rawArc.AlternateArc; }

            set
            {
                _rawArc.AlternateArc = value;
                RequestUpdateSource();
                OnPropertyChanged("AlternateArc");
            }
        }

        private RawInputViewModel _x;
        public RawInputViewModel X
        {
            get { return _x; }
            set
            {
                _x = value;
                OnPropertyChanged("X");
            }
        }
        private RawInputViewModel _y;
        public RawInputViewModel Y
        {
            get { return _y; }
            set { _y = value; OnPropertyChanged("Y"); }
        }

        private RawInputViewModel _deltaX;
        public RawInputViewModel DeltaX
        {
            get { return _deltaX; }
            set
            {
                _deltaX = value;
                OnPropertyChanged("DeltaX");
            }
        }

        private RawInputViewModel _centerX;
        public RawInputViewModel CenterX
        {
            get { return _centerX; }
            set
            {
                _centerX = value;
                OnPropertyChanged("CenterX");
            }
        }

        private RawInputViewModel _centerY;
        public RawInputViewModel CenterY
        {
            get { return _centerY; }
            set
            {
                _centerY = value;
                OnPropertyChanged("CenterY");
            }
        }

        private RawInputViewModel _deltaY;
        public RawInputViewModel DeltaY
        {
            get { return _deltaY; }
            set
            {
                _deltaY = value;
                OnPropertyChanged("DeltaY");
            }
        }

        private RawInputViewModel _chamfer;
        public RawInputViewModel Chamfer
        {
            get { return _chamfer; }
            set
            {
                _chamfer = value;
                OnPropertyChanged("Chamfer");
            }
        }
        private RawInputViewModel _radius;
        public RawInputViewModel Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                OnPropertyChanged("Radius");
            }
        }
        private RawArc2D rawArc2D;
        private ProfileEditorViewModel.AxisSystem _axisSystem;

        private RawInputViewModel _endRadius;
        public RawInputViewModel EndRadius
        {
            get { return _endRadius; }
            set
            {
                _endRadius = value;
                OnPropertyChanged("EndRadius");
            }
        }


    }
}


