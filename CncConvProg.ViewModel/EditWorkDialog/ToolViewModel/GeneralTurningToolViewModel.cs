using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.Tool.LatheTool;

namespace CncConvProg.ViewModel.EditWorkDialog.ToolViewModel
{
    public class GeneralTurningToolViewModel : MVVM_Library.ViewModelBase
    {
        private readonly UtensileTornitura _genericTurningTool;

        public GeneralTurningToolViewModel(UtensileTornitura genericTurningTool)
        {
            _genericTurningTool = genericTurningTool;
        }

        public string ToolName
        {
            get { return _genericTurningTool.ToolName; }
            set
            {
                _genericTurningTool.ToolName = value;
                OnPropertyChanged("ToolName");
            }
        }

        public int NumeroPostazione
        {
            get { return _genericTurningTool.; }
            set
            {
                _genericTurningTool.NumeroPostazione = value;
                OnPropertyChanged("NumeroPostazione");
            }
        }

        public int NumeroCorrettore
        {
            get { return _genericTurningTool.NumeroCorrettore; }
            set
            {
                _genericTurningTool.NumeroCorrettore = value;
                OnPropertyChanged("NumeroCorrettore");
            }
        }


    }
}
