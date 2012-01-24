using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface
{
    public interface ILamaturaAble
    {
        double ProfonditaLamatura { get; set; }

        double DiametroLamatura { get; set; }

        bool LamaturaAbilitata { get; set; }

        /*
         * 0 - Semplice Dritta
         * 1 - Interpolazione 
         */

        int ModalitaLamatura { get; set; }

        
    }
}
