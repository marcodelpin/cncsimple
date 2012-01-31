using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Pattern
{
    public sealed class ForaturaPatternSelectionViewModel : EditStageTreeViewItem, IValid
    {
        private readonly IForaturaPatternable _foraturaSemplice;

        // sostituire foratura semplice con classe base per forature o in caso con interfaccia
        public ForaturaPatternSelectionViewModel(IForaturaPatternable foratura, EditWorkViewModel viewModelEditWorkParent)
            : base("Pattern Selection", viewModelEditWorkParent)
        {
            _foraturaSemplice = foratura;

            PatternParameter = GetViewModel(_foraturaSemplice.PatternDrilling);

        }

        public PatternForatura PatternForatura
        {
            get
            {
                return _foraturaSemplice.Pattern;
            }

            set
            {
                _foraturaSemplice.Pattern = value;

                PatternParameter = GetViewModel(_foraturaSemplice.PatternDrilling);

                OnPropertyChanged("PatternForatura");
            }
        }

        private ViewModelBase _patternParameter;
        public ViewModelBase PatternParameter
        {
            get { return _patternParameter; }
            set
            {
                _patternParameter = value;
                OnPropertyChanged("PatternParameter");
            }
        }


        public static EditStageTreeViewItem GetViewModel(IPatternDrilling patternDrilling)
        {
            if (patternDrilling is PatternDrillingCircle)
                return new CirclePatternViewModel(patternDrilling as PatternDrillingCircle, null);

            if (patternDrilling is PatternDrillingRectangle)
                return new RectanglePatternViewModel(patternDrilling as PatternDrillingRectangle, null);

            if (patternDrilling is PatternDrillingXy)
                return new XyPatternViewModel(patternDrilling as PatternDrillingXy, null);

            if (patternDrilling is PatternDrillingLine)
                return new LinePatternViewModel(patternDrilling as PatternDrillingLine, null);

            if (patternDrilling is PatternDrillingRc)
                return new RcPatternViewModel(patternDrilling as PatternDrillingRc, null);

            if (patternDrilling is PatternDrillingArc)
                return new ArcPatternViewModel(patternDrilling as PatternDrillingArc, null);

            throw new NotImplementedException("ForaturaSempliceViewModel.GetViewModel");
        }
        public bool IsValid
        {
            get { return true; }
        }
    }
}
