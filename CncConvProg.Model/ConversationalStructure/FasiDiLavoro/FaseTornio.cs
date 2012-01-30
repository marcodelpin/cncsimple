using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.Tool;

namespace CncConvProg.Model.ConversationalStructure.FasiDiLavoro
{
    [Serializable]
    public class FaseTornio : FaseDiLavoro
    {
        public double LimitatoreGiri { get; set; }

        internal FaseTornio()
            : base()
        {
            Descrizione = "Lathe 2 Axis Phase";
        }

        internal override ToolHolder GetToolHolder()
        {
            return new LatheToolHolder();
        }

        public override TipoFaseLavoro TipoFase
        {
            get { return TipoFaseLavoro.Tornio2Assi; }
        }
    }
}