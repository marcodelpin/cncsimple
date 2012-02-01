using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.Common;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Pattern
{
    public sealed class ForaturaPatternSelectionViewModel : EditStageTreeViewItem
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


        public static ViewModelValidable GetViewModel(IPatternDrilling patternDrilling)
        {
            if (patternDrilling is PatternDrillingCircle)
                return new CirclePatternViewModel(patternDrilling as PatternDrillingCircle);

            if (patternDrilling is PatternDrillingRectangle)
                return new RectanglePatternViewModel(patternDrilling as PatternDrillingRectangle);

            if (patternDrilling is PatternDrillingXy)
                return new XyPatternViewModel(patternDrilling as PatternDrillingXy);

            if (patternDrilling is PatternDrillingLine)
                return new LinePatternViewModel(patternDrilling as PatternDrillingLine);

            if (patternDrilling is PatternDrillingRc)
                return new RcPatternViewModel(patternDrilling as PatternDrillingRc);

            if (patternDrilling is PatternDrillingArc)
                return new ArcPatternViewModel(patternDrilling as PatternDrillingArc);

            throw new NotImplementedException("ForaturaSempliceViewModel.GetViewModel");
        }

        public override bool? ValidateStage()
        {
            return null;
        }
    }
}
