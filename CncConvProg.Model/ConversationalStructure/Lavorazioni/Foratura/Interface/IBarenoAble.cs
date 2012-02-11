using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface
{
    public interface IBarenoAble
    {
        double ProfonditaBareno { get; set; }

        double DiametroBarenatura { get; set; }

     //   bool BarenoAbilitato { get; set; }

        double MaterialePerFinitura { get; set; }

        bool AllargaturaAbilitata { get; set; }

        /*
         * 0 - fresatura normale
         * 1 - Interpolazione
         */

        byte ModalitaAllargatura { get; set; }
    }
}
