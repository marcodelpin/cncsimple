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
    public class RawInitPointViewModel : RawItemViewModel
    {
        private readonly RawInitPoint2D _rawPoint2D;
        public RawInitPoint2D Source { get { return _rawPoint2D; } }

        /*
         * la gestione del axisLabel può essere raggruppata in classe abstract
         */
        public RawInitPointViewModel(RawInitPoint2D rawPoint2D, ProfileEditorViewModel.AxisSystem axisSystem)
            : base(rawPoint2D, axisSystem)
        {
            _rawPoint2D = rawPoint2D;

            X = new RawInputViewModel(_rawPoint2D.X);
            Y = new RawInputViewModel(_rawPoint2D.Y);

            X.OnSourceUpdated += RawLine_SourceUpdated;
            Y.OnSourceUpdated += RawLine_SourceUpdated;

        }

        void RawLine_SourceUpdated(object sender, EventArgs e)
        {
            RequestUpdateSource();
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


    }
}
