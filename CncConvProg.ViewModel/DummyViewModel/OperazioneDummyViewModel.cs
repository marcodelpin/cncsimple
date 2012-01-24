using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.ViewModel.AuxViewModel;

namespace CncConvProg.ViewModel.DummyViewModel
{
    public class OperazioneDummyViewModel
    {
        public OperazioneDummyViewModel()
        {

            NumeroGiri = new DummyUserInput();
            LarghezzaPassata = new DummyUserInput();
            ProfonditaPassata = new DummyUserInput();
            AvanazamentoGiro = new DummyUserInput();
            VelocitaTaglio = new DummyUserInput();

            VelocitaTaglio.Value = "120";

            NumeroGiri.Value = "9700";

            NumeroGiri.IsUserInputed = true;

            LarghezzaPassata.Value = "90";

            ProfonditaPassata.Value = "90";

            AvanazamentoGiro.Value = ".8";

            AvanazamentoGiro.IsUserInputed = true;

        }
        public DummyUserInput VelocitaTaglio { get; set; }
        public DummyUserInput NumeroGiri { get; set; }
        public DummyUserInput LarghezzaPassata { get; set; }
        public DummyUserInput ProfonditaPassata { get; set; }
        public DummyUserInput AvanazamentoGiro { get; set; }
    }
}
