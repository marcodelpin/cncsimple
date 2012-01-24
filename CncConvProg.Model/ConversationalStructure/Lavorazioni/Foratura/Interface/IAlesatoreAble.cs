using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface
{
    public interface IAlesatoreAble
    {
        double ProfonditaAlesatore { get; set; }

        bool AlesaturaAbilitata { get; set; }
    }
}
