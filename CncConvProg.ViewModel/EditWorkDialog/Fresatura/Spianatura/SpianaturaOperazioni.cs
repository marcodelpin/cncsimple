using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Lavorazioni;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.EditWorkDialog.Fresatura.Spianatura
{
    public class SpianaturaOperazioni : EditStageTreeViewItem
    {
        private readonly Model.ConversationalStructure.Lavorazioni.Fresatura.Spianatura _spianatura;

        public SpianaturaOperazioni(Model.ConversationalStructure.Lavorazioni.Fresatura.Spianatura spianatura, EditWorkViewModel parent)
            : base("Operation", parent)
        {
            _spianatura = spianatura;
        }

        public bool SgrossaturaAbilitata
        {
            get { return _spianatura.Sgrossatura.Abilitata; }

            set
            {
                _spianatura.Sgrossatura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("SgrossaturaAbilitata");
            }
        }

        public bool FinituraAbilitata
        {
            get { return _spianatura.Finitura.Abilitata; }

            set
            {
                _spianatura.Finitura.Abilitata = value;
                EditWorkParent.SyncronizeOperation();
                OnPropertyChanged("FinituraAbilitata");
            }
        }

        public int ModoSgrossatura
        {
            get
            {
                return (int)_spianatura.ModoSgrossatura;
            }

            set
            {
                _spianatura.ModoSgrossatura = (SpiantaturaMetodologia) value;
                OnPropertyChanged("ModoSgrossatura");
            }
        }

        public int ModoFinitura
        {
            get
            {
                return (int)_spianatura.ModoFinitura;
            }

            set
            {
                _spianatura.ModoFinitura = (SpiantaturaMetodologia)value;
                OnPropertyChanged("ModoFinitura");
            }
        }
    }
}
