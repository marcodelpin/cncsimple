using System;
using System.Collections.ObjectModel;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.EditWorkDialog.Foratura.Common
{
    public class ForaturaCommonOperazioniViewModel : EditStageTreeViewItem
    {
        private readonly DrillBaseClass _drillBaseClass;

        public ForaturaCommonOperazioniViewModel(DrillBaseClass drillBaseClass, EditWorkViewModel parent)
            : base("Operation", parent)
        {
            _drillBaseClass = drillBaseClass;

            foreach (var operazione in _drillBaseClass.Operazioni)
            {
                OperationList.Add(GetViewModel(operazione, drillBaseClass, parent));
            }

        }

        private static ViewModelBase GetViewModel(Model.ConversationalStructure.Operazione operazione, DrillBaseClass baseClass, EditWorkViewModel parent)
        {
            switch (operazione.OperationType)
            {
                case LavorazioniEnumOperazioni.ForaturaCentrino:
                    {
                        return new CentrinoOperazioneViewModel(baseClass, parent);
                    } break;

                case LavorazioniEnumOperazioni.ForaturaPunta:
                    {
                        return new ForaturaPuntaOperazioneViewModel(baseClass, parent);
                    } break;

                case LavorazioniEnumOperazioni.ForaturaSmusso:
                    {
                        return new SvasaturaOperazioneViewModel(baseClass, parent);
                    } break;

                case LavorazioniEnumOperazioni.ForaturaLamatore:
                    {
                        var p = baseClass as ILamaturaAble;
                        if (p != null)
                            return new LamaturaOperazioneViewModel(p, parent);
                    } break;

                case LavorazioniEnumOperazioni.ForaturaMaschiaturaDx:
                    {
                        var p = baseClass as IMaschiaturaAble;
                        if (p != null)
                            return new MaschiaturaOperazioneViewModel(p, parent);

                    } break;
                case LavorazioniEnumOperazioni.ForaturaAlesatore:
                    {
                        var p = baseClass as IAlesatoreAble;
                        if (p != null)
                            return new AlesaturaOperazioneViewModel(p, parent);
                    } break;

                case LavorazioniEnumOperazioni.AllargaturaBareno:
                    {
                        // le cose che dovevano essere qui sono gia dentro il vw per barenantura
                        //var p = baseClass as IBarenoAble;
                        //if (p != null)
                        //    return new BarenoOperazioneViewModel(p, parent);
                    }break;

                case LavorazioniEnumOperazioni.ForaturaBareno:
                    {
                        var p = baseClass as IBarenoAble;
                        if (p != null)
                            return new BarenoOperazioneViewModel(p, parent);
                    } break;



                default:
                    throw new NotImplementedException();
            }

            return null;
        }

        private ObservableCollection<ViewModelBase> _operationList = new ObservableCollection<ViewModelBase>();
        public ObservableCollection<ViewModelBase> OperationList
        {
            get
            {
                return _operationList;
            }

            set
            {
                _operationList = value;
                OnPropertyChanged("OperationList");
            }
        }

        //public int ModoForatura
        //{
        //    get { return (int)_foraturaSemplice.ModalitaForatura; }

        //    set
        //    {
        //        _foraturaSemplice.ModalitaForatura = (ModalitaForatura)value;
        //        OnPropertyChanged("ModoForatura");
        //    }
        //}


        //public bool CentrinoAbilitato
        //{
        //    get { return _foraturaSemplice.Centrinatura.Abilitata; }

        //    set
        //    {
        //        _foraturaSemplice.Centrinatura.Abilitata = value;
        //        EditWorkParent.SyncronizeOperation();
        //        OnPropertyChanged("CentrinoAbilitato");
        //    }
        //}
        //public bool ForaturaAbilitata
        //{
        //    get { return _foraturaSemplice.Foratura.Abilitata; }

        //    set
        //    {
        //        _foraturaSemplice.Foratura.Abilitata = value;
        //        EditWorkParent.SyncronizeOperation();
        //        OnPropertyChanged("ForaturaAbilitata");
        //    }
        //}
        //public bool SvasaturaAbilitata
        //{
        //    get { return _foraturaSemplice.Svasatura.Abilitata; }

        //    set
        //    {
        //        _foraturaSemplice.Svasatura.Abilitata = value;
        //        EditWorkParent.SyncronizeOperation();
        //        OnPropertyChanged("SvasaturaAbilitata");
        //    }
        //}

    }
}
