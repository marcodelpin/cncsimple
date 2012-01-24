using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel;

namespace CncConvProg.ViewModel.CommonViewModel.ParameterViewModels
{
    public class ParametroFresaSpianareViewModel : ParametroFresaBaseViewModel
    {
        private readonly ParametroFresaSpianare _parametroFresaSpianare;

        public ParametroFresaSpianareViewModel(ParametroFresaSpianare parametroFresaSpianare)
            : base(parametroFresaSpianare)
        {
            _parametroFresaSpianare = parametroFresaSpianare;

        }

    }


}

