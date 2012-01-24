using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.Tool.Parametro;
using MecPrev.Resources;

namespace CncConvProg.Model.Tool.LatheTool
{
    [Serializable]
    public class UtensileScanalatura : Utensile
    {
        public UtensileScanalatura(MeasureUnit measureUnit) :
            base(measureUnit)
        {
            ToolName = GuiRes.TurnGroove;

        }

        public double LarghezzaUtensile { get; set; }

        internal override ParametroUtensile CreateParametro()
        {
            return new ParametroUtensileTornituraScanalatura(this);
        }
    }

    [Serializable]
    public class UtensileFilettare : Utensile
    {
        public UtensileFilettare(MeasureUnit measureUnit) :
            base(measureUnit)
        {
            ToolName = GuiRes.TurnThread;

        }

        internal override ParametroUtensile CreateParametro()
        {
            return new ParametroUtensileTornitura(this);
        }
    }
}
