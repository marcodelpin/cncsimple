using System;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.ViewModel.EditWorkDialog.Foratura.ParameterScreen;
using CncConvProg.ViewModel.EditWorkDialog.Foratura.Pattern;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

/*
 * cicli foratura
 * - input punti =
 * - parametri lavorazione != 
 * - operazioni != 
 */
namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Common
{
    public sealed class ForaturaCommonViewModel : EditWorkViewModel
    {
        private readonly EditStageTreeViewItem _foraturaParametriViewModel;
        private readonly ForaturaPatternSelectionViewModel _foraturaPatternSelectionViewModel;
        private readonly DrillBaseClass _foratura;
        /*
         * qui ho lo schermo per 
         */
        //private readonly LavorazioneViewModel _lavorazioneViewModel;

        //private readonly EditStageTreeViewItem _stageInputProfile;
        //private readonly EditStageTreeViewItem _stageParametriLavorazione;

        /*
         * faccio 5 costruttori ??
         * 
         * faccio metodo che da operazione fa viewmodel
         * listviewmodel
         * metodo per screen parametri lavorazione
         */

        public ForaturaCommonViewModel(DrillBaseClass foraturaBaseClass)
            : base(foraturaBaseClass)
        {
            _foratura = foraturaBaseClass;

        //    StageOperazioni = new ForaturaCommonOperazioniViewModel(foraturaBaseClass, this);

            if (!_foratura.ForaturaCentraleTornio)
            {
                _foraturaPatternSelectionViewModel = new ForaturaPatternSelectionViewModel(foraturaBaseClass, this);
            }

            _foraturaParametriViewModel = GetParameterViewModel(foraturaBaseClass);

            TreeView.Add(_foraturaParametriViewModel);

            if (_foraturaPatternSelectionViewModel != null)
                TreeView.Add(_foraturaPatternSelectionViewModel);

         //   TreeView.Add(StageOperazioni);

            Initialize();
        }

        private EditStageTreeViewItem GetParameterViewModel(DrillBaseClass drillBaseClass)
        {
            if (drillBaseClass is ForaturaSemplice)
                return new ForaturaSempliceParametriViewModel(this, drillBaseClass as ForaturaSemplice);

            if (drillBaseClass is Maschiatura)
                return new MaschiaturaParametriViewModel(this, drillBaseClass as Maschiatura);

            if (drillBaseClass is Alesatura)
                return new ReamerParametriViewModel(this, drillBaseClass as Alesatura);

            if (drillBaseClass is Barenatura)
                return new BarenaturaParametriViewModel(this, drillBaseClass as Barenatura);

            if (drillBaseClass is Lamatura)
                return new LamaturaParametriViewModel(this, drillBaseClass as Lamatura);

            throw new NotImplementedException();
        }
    }
}
