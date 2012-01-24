using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Pattern
{
    public sealed class ForaturaPatternSelectionViewModel : EditStageTreeViewItem, IValid
    {
        private readonly IForaturaPatternable _foraturaSemplice;

        public event EventHandler OnPatternChanged;

        private void RequestPatternChanged()
        {
            var handler = OnPatternChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }


        // sostituire foratura semplice con classe base per forature o in caso con interfaccia
        public ForaturaPatternSelectionViewModel(IForaturaPatternable foratura, EditWorkViewModel viewModelEditWorkParent)
            : base("Pattern Selection", viewModelEditWorkParent)
        {
            _foraturaSemplice = foratura;
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

                RequestPatternChanged();

                OnPropertyChanged("PatternForatura");
            }
        }

        public static EditStageTreeViewItem GetViewModel(IPatternDrilling patternDrilling, ForaturaPatternSelectionViewModel treeViewParent)
        {
            if (patternDrilling is PatternDrillingCircle)
                return new CirclePatternViewModel(patternDrilling as PatternDrillingCircle, treeViewParent);

            if (patternDrilling is PatternDrillingRectangle)
                return new RectanglePatternViewModel(patternDrilling as PatternDrillingRectangle, treeViewParent);

            if (patternDrilling is PatternDrillingXy)
                return new XyPatternViewModel(patternDrilling as PatternDrillingXy, treeViewParent);

            if (patternDrilling is PatternDrillingLine)
                return new LinePatternViewModel(patternDrilling as PatternDrillingLine, treeViewParent);

            if (patternDrilling is PatternDrillingRc)
                return new RcPatternViewModel(patternDrilling as PatternDrillingRc, treeViewParent);

            if (patternDrilling is PatternDrillingArc)
                return new ArcPatternViewModel(patternDrilling as PatternDrillingArc, treeViewParent);

            throw new NotImplementedException("ForaturaSempliceViewModel.GetViewModel");
        }
        public bool IsValid
        {
            get { return true; }
        }
    }
}
