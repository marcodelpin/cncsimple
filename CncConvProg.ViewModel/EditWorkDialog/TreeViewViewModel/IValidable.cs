using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.ViewModel.EditWorkDialog.TreeViewViewModel
{
    internal interface IValid
    {
        bool? IsValid { get; }
    }

    interface IValidable : IValid
    {
        bool? ValidateStage();
    }
}
