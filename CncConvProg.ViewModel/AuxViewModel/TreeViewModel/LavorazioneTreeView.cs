using System;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using System.Windows.Input;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Implementors.Wpf.MVVM;

namespace CncConvProg.ViewModel.AuxViewModel.TreeViewModel
{
    public class LavorazioneTreeView : TreeViewItemViewModel, IValid
    {
        public Lavorazione Lavorazione { get; private set; }

        public LavorazioneTreeView(Lavorazione lavorazione, FaseLavoroTreeView parentFaseLavoro)
            : base(parentFaseLavoro, false)
        {
            IsExpanded = true;
            Lavorazione = lavorazione;
        }

        public int LavorazionePosition
        {
            get { return Lavorazione.LavorazionePosition; }
        }
        public bool IsValid
        {
            get
            {
                return Lavorazione.IsValid;
            }
        }

        //#region _ Edit Command _

        //RelayCommand _editCommand;

        //public void Edit()
        //{
        //    //((FaseLavoroTreeView)(this.Parent)).ScreenParent.EditConvPhase(this.Source);
        //}

        //public ICommand EditCommand
        //{
        //    get
        //    {
        //        return _editCommand ?? (_editCommand = new RelayCommand(param => Edit(),
        //                                                                param => true));
        //    }
        //}

        //#endregion

        //#region _ del Command _

        //RelayCommand _delCommand;

        //public void Delete()
        //{
        //}

        //public ICommand DeleteCommand
        //{
        //    get
        //    {
        //        return _delCommand ?? (_delCommand = new RelayCommand(param => Delete(),
        //                                                              param => true));
        //    }
        //}

        //#endregion

    }
}
