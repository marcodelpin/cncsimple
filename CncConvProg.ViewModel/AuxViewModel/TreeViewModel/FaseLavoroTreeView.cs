using System;
using System.Collections.Generic;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using System.Collections.ObjectModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Implementors.Wpf.MVVM;

/*
 * 
 */
namespace CncConvProg.ViewModel.AuxViewModel.TreeViewModel
{
    public sealed class FaseLavoroTreeView : TreeViewItemViewModel//, IValid
    {

        public Guid FaseDiLavoroGuid { get; private set; }

        public FaseDiLavoro FaseDiLavoro
        {
            get
            {
                return Singleton.Instance.GetFaseDiLavoro(FaseDiLavoroGuid);
            }
        }

        public FaseLavoroTreeView(Guid faseDiLavoro, ViewModelBase parentScreen)
            : base(null, false)
        {
            IsExpanded = true;
            FaseDiLavoroGuid = faseDiLavoro;
            //FaseDiLavoro = faseDiLavoro;
        }

        //public bool IsValid
        //{
        //    get { return FaseDiLavoro.IsValid; }
        //}
    }
}
