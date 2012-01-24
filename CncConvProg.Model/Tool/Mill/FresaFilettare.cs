using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.Tool.Parametro;
using MecPrev.Resources;

namespace CncConvProg.Model.Tool.Mill
{
    [Serializable]
    public class FresaFilettare : FresaBase
    {
        public FresaFilettare(MeasureUnit measureUnit)
            : base(measureUnit)
        {
            ToolName = GuiRes.ThreadMill;

            ParametroUtensile = CreateParametro();
        }

        public double Diameter { get; set; }

        internal override sealed ParametroUtensile CreateParametro()
        {
            return new ParametroFresaFilettare(this);
        }
    }
}