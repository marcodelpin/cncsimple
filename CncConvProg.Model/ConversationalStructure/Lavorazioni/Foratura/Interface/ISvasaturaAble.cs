using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface
{
    public interface ISvasaturaAble
    {
        bool SvasaturaConFresa { get; set; }

        bool SvasaturaConCompensazione { get; set; }

        double ProfonditaSvasatura { get; set; }

        bool SvasaturaAbilitata { get; set; }
    }
}
