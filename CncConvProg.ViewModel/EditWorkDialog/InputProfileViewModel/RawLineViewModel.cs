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
    public class RawLineViewModel : RawItemViewModel
    {
        private readonly RawLine2D _rawLine;

        public RawLine2D Source { get { return _rawLine; } }

        public RawLineViewModel(RawLine2D rawLine2D, ProfileEditorViewModel.AxisSystem axisSystem)
            : base(rawLine2D, axisSystem)
        {
            _rawLine = rawLine2D;

            X = new RawInputViewModel(_rawLine.X,this);
            Angle = new RawInputViewModel(_rawLine.Angle, this);
            Y = new RawInputViewModel(_rawLine.Y, this);
            DeltaX = new RawInputViewModel(_rawLine.DeltaX, this);
            DeltaY = new RawInputViewModel(_rawLine.DeltaY, this);
            Chamfer = new RawInputViewModel(_rawLine.Chamfer, this);
            EndRadius = new RawInputViewModel(_rawLine.EndRadius, this);

            foreach (var rawInputViewModel in InputVmList)
            {
                rawInputViewModel.OnSourceUpdated += RawLine_SourceUpdated;
            }
            //Angle.OnSourceUpdated += RawLine_SourceUpdated;
            //X.OnSourceUpdated += RawLine_SourceUpdated;
            //Y.OnSourceUpdated += RawLine_SourceUpdated;
            //DeltaX.OnSourceUpdated += RawLine_SourceUpdated;
            //DeltaY.OnSourceUpdated += RawLine_SourceUpdated;
            //Chamfer.OnSourceUpdated += RawLine_SourceUpdated;
            //EndRadius.OnSourceUpdated += RawLine_SourceUpdated;
        }

        void RawLine_SourceUpdated(object sender, EventArgs e)
        {
            RequestUpdateSource();
        }



        private RawInputViewModel _angle;
        public RawInputViewModel Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                OnPropertyChanged("Angle");
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
        private RawInputViewModel _endRadius;
        private RawLine2D rawLine2D;
        private ProfileEditorViewModel.AxisSystem _axisSystem;
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

