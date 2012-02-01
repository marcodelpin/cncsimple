using System;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using System.Windows.Input;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Implementors.Wpf.MVVM;

namespace CncConvProg.ViewModel.AuxViewModel.TreeViewModel
{
    public class LavorazioneTreeView : TreeViewItemViewModel, IValidable
    {
        public string Label
        {
            get { return Lavorazione.Descrizione; }
        }

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

        public bool? IsValid
        {
            get
            {
                return Lavorazione.IsValid;
            }
        }

        public string IconSource
        {
            get
            {
                return GetImageIcon();
            }
        }

        public string GetImageIcon()
        {
            var p = @"pack://application:,,,/CncConvProg.View;component/Images/work/";

            var imageName = string.Empty;
            //if (Lavorazione is Tornitura || Lavorazione is TornituraSfacciatura)
            //    imageName = "turning_small.png";

            //if (Lavorazione is TornituraFilettatura)
            //    imageName = "thread_small.png";

            //if (Lavorazione is TornituraScanalatura)
            //    imageName = "groove_small.png";

            if (Lavorazione is Spianatura)
                imageName = "millface_16.png";

            if (Lavorazione is FresaturaLato)
                imageName = "millface_16.png";

            if (Lavorazione is FresaturaScanalaturaChiusa)
                imageName = "millScanaCerchio_16.png";

            if (Lavorazione is FresaturaContornatura)
                imageName = "millCont_16.png";

            if (Lavorazione is DrillBaseClass)
                imageName = "drill_small.png";

            if (Lavorazione is FresaturaCava)
                imageName = "caveMillSmall.png";

            if (Lavorazione is ScanalaturaLinea)
                imageName = "millscan.png";

            if (imageName != string.Empty)
                return p + imageName;

            return string.Empty;

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

        public bool? ValidateStage()
        {
            throw new NotImplementedException();
        }
    }
}
