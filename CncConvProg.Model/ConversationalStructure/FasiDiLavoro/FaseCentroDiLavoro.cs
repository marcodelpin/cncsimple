using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.Tool;

namespace CncConvProg.Model.ConversationalStructure.FasiDiLavoro
{
    [Serializable]
    public class FaseCentroDiLavoro : FaseDiLavoro
    {
        internal FaseCentroDiLavoro()
            : base()
        {
            Descrizione = "Vertical Mill Phase";
        }

        //internal override ToolHolder GetToolHolder()
        //{
        //    return new MillToolHolder();
        //}

        public override FaseDiLavoro.TipoFaseLavoro TipoFase
        {
            get { return TipoFaseLavoro.Centro; }
        }
    }
}