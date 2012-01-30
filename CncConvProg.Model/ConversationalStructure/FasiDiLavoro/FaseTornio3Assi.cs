using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.ConversationalStructure.Abstraction;

namespace CncConvProg.Model.ConversationalStructure.FasiDiLavoro
{
    [Serializable]
    public class FaseTornio3Assi : FaseTornio
    {
        internal FaseTornio3Assi()
            : base()
        {
            Descrizione = "Lathe 3 Axis Phase";
        }
    }
}