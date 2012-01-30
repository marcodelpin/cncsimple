using System;
using System.Collections.ObjectModel;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.Model.ThreadTable;
using CncConvProg.ViewModel.EditWorkDialog.Foratura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Cava;
using CncConvProg.ViewModel.EditWorkDialog.Fresatura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Filettatura
{
    public class FresaturaFilettaturaViewModel : EditWorkViewModel
    {
        //private readonly IProfileEditorViewModel _profileEditorViewModel;

        private readonly FresaturaFilettaturaParametriViewModel _contornaturaParametriViewModel;

        private EditStageTreeViewItem _patternScreen;

        private readonly FresaturaFilettatura _fresaturaFilettatura;

        private readonly ForaturaPatternSelectionViewModel _foraturaPatternSelectionViewModel;


        public FresaturaFilettaturaViewModel(FresaturaFilettatura fresaturaFilettatura)
            : base(fresaturaFilettatura)
        {
            _fresaturaFilettatura = fresaturaFilettatura;

        //    StageOperazioni = new FresaturaFilettaturaOperazioniViewModel(fresaturaFilettatura, this);

            _foraturaPatternSelectionViewModel = new ForaturaPatternSelectionViewModel(fresaturaFilettatura, this);

            _foraturaPatternSelectionViewModel.OnPatternChanged += ForaturaPatternSelectionViewModelOnPatternChanged;

            _patternScreen = ForaturaPatternSelectionViewModel.GetViewModel(_fresaturaFilettatura.PatternDrilling, _foraturaPatternSelectionViewModel);

            _foraturaPatternSelectionViewModel.Children.Add(_patternScreen);

            _contornaturaParametriViewModel = new FresaturaFilettaturaParametriViewModel(fresaturaFilettatura, this);

            TreeView.Add(_contornaturaParametriViewModel);

            TreeView.Add(_foraturaPatternSelectionViewModel);


            //TreeView.Add(_stageInputProfile);
      //      TreeView.Add(StageOperazioni);

            Initialize();

        }

        void ForaturaPatternSelectionViewModelOnPatternChanged(object sender, EventArgs e)
        {
            if (_foraturaPatternSelectionViewModel.Children.Contains(_patternScreen))
                _foraturaPatternSelectionViewModel.Children.Remove(_patternScreen);

            _patternScreen = ForaturaPatternSelectionViewModel.GetViewModel(_fresaturaFilettatura.PatternDrilling, _foraturaPatternSelectionViewModel);
            _patternScreen.OnSourceUpdated += EditStageTreeViewItemOnSourceUpdated;

            _foraturaPatternSelectionViewModel.Children.Add(_patternScreen);
        }

      

    }
}
