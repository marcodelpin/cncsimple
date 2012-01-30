using System;
using System.Windows;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern
{
    public sealed class MillingPatternSelectionViewModel : EditStageTreeViewItem
    {
        private readonly IMillingPatternable _millingPatternable;

      //  public event EventHandler OnPatternChanged;

        //private void RequestPatternChanged()
        //{
        //    var handler = OnPatternChanged;
        //    if (handler != null)
        //        handler(this, EventArgs.Empty);
        //}


        // sostituire foratura semplice con classe base per forature o in caso con interfaccia
        public MillingPatternSelectionViewModel(IMillingPatternable millingPatternable, EditWorkViewModel viewModelEditWorkParent)
            : base("Pattern Selection", viewModelEditWorkParent)
        {
            _millingPatternable = millingPatternable;

            PatternParameter = GetViewModel(_millingPatternable.Pattern);
            
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

        // Piccolo hack per mostrare anche pattern che in altre lavorazioni non sono abilitate.
        public Visibility ShowExternMillingPattern
        {
            get
            {
                if (_millingPatternable is FresaturaContornatura)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
        }

        public EnumPatternMilling PatternFresatura
        {
            get
            {
                return _millingPatternable.MillingPattern;
            }

            set
            {
                _millingPatternable.MillingPattern = value;

                //RequestPatternChanged();

                PatternParameter = GetViewModel(_millingPatternable.Pattern);

                OnPropertyChanged("PatternFresatura");
            }
        }

        public EditStageTreeViewItem GetViewModel(IMillingPattern patternMilling)
        {
            if (patternMilling is RegularPolygonPattern)
                return new RegularPolygonPatternViewModel(patternMilling as RegularPolygonPattern, this);

            if (patternMilling is CavaArcoPattern)
                return new CavaArcoPatternViewModel(patternMilling as CavaArcoPattern, this);

            if (patternMilling is CirclePattern)
                return new CirclePatternViewModel(patternMilling as CirclePattern, this);

            if (patternMilling is CavaDrittaPattern)
                return new CavaPatternViewModel(patternMilling as CavaDrittaPattern, this);

            if (patternMilling is CavaDrittaApertaPattern)
                return new CavaApertaPatternViewModel(patternMilling as CavaDrittaApertaPattern, this);

            //if (patternMilling is RettangoloPattern)
            //    return new RettangoloPatternViewModel(patternMilling as RettangoloPattern, this);

            if (patternMilling is FreeProfilePattern)
            {
                var patt = patternMilling as FreeProfilePattern;
                var rawProfile = patt.GetRawProfile();
                return new ProfileEditorViewModel(rawProfile, this, ProfileEditorViewModel.AxisSystem.Xy);
            }
            throw new NotImplementedException("MillingPatternSelection.GetViewModel");
        }
    }
}
