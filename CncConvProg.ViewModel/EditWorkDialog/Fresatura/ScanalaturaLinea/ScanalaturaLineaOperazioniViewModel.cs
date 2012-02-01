using System;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.ScanalaturaLinea
{
    public class ScanalaturaLineaOperazioniViewModel : EditStageTreeViewItem
    {
        private readonly Model.ConversationalStructure.Lavorazioni.Fresatura.ScanalaturaLinea _scanalaturaLinea;

        public ScanalaturaLineaOperazioniViewModel(Model.ConversationalStructure.Lavorazioni.Fresatura.ScanalaturaLinea scanalaturaLinea, EditWorkViewModel parent)
            : base(scanalaturaLinea, parent)
        {
            _scanalaturaLinea = scanalaturaLinea;
        }

        public double TrochoidalStep
        {
            get { return _scanalaturaLinea.TrochoidalStep; }
            set
            {
                _scanalaturaLinea.TrochoidalStep = value;
                OnPropertyChanged("TrochoidalStep");
            }
        }

        public int ModoSgrossatura
        {
            get
            {
                return (int)_scanalaturaLinea.ModoSgrossatura;
            }

            set
            {
                _scanalaturaLinea.ModoSgrossatura = (ScanalaturaCavaMetodoLavorazione)value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("ModoSgrossatura");
            }
        }

        public bool FinishWithCompensation
        {
            get { return _scanalaturaLinea.FinishWithCompensation; }

            set
            {
                _scanalaturaLinea.FinishWithCompensation = value;
                OnPropertyChanged("FinishWithCompensation");

            }
        }

        public double SovrametalloFinitura
        {
            get { return _scanalaturaLinea.SovrametalloFinituraProfilo; }
            set
            {
                _scanalaturaLinea.SovrametalloFinituraProfilo = value;
                OnPropertyChanged("SovrametalloFinitura");
            }
        }

        public double ProfonditaFresaSmussatura
        {
            get { return _scanalaturaLinea.ProfonditaFresaSmussatura; }
            set
            {
                _scanalaturaLinea.ProfonditaFresaSmussatura = value;
                OnPropertyChanged("ProfonditaFresaSmussatura");
            }
        }

        public bool SgrossaturaAbilitata
        {
            get { return _scanalaturaLinea.Sgrossatura.Abilitata; }

            set
            {
                _scanalaturaLinea.Sgrossatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SgrossaturaAbilitata");

            }
        }

        public bool FinituraAbilitata
        {
            get { return _scanalaturaLinea.Finitura.Abilitata; }

            set
            {
                _scanalaturaLinea.Finitura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("FinituraAbilitata");
            }
        }

        public bool SmussaturaAbilitata
        {
            get { return _scanalaturaLinea.Smussatura.Abilitata; }

            set
            {
                _scanalaturaLinea.Smussatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SmussaturaAbilitata");
            }
        }
        //public int ModoFinitura
        //{
        //    get
        //    {
        //        return (int)_scanalaturaLinea.ModoFinitura;
        //    }

        //    set
        //    {
        //        _scanalaturaLinea.ModoFinitura = (ScanalaturaCavaMetodoLavorazione)value;
        //        EditWorkParent.SyncronizeOperation();
        //        OnPropertyChanged("ModoFinitura");
        //    }
        //}
        public override bool? ValidateStage()
        {
            return null;
        }
    }
}
