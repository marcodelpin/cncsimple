using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.Tool.Parametro;
using MecPrev.Resources;

namespace CncConvProg.Model.Tool.Mill
{
    [Serializable]
    public class FresaCandela : FresaBase 
    {

        public ParametroFresaCandela ParametroFresaCandela
        {
            get
            {
                return ParametroUtensile as ParametroFresaCandela;
            }
        }

        public FresaCandela(MeasureUnit measureUnit)
            : base(measureUnit)
        {
            ToolName = GuiRes.FlatMill;

            ParametroUtensile = CreateParametro();
        }

        /*
         * facendo cose semplici..
         * 
         * parametro lo puo memorizzare dentro qua ..
         */

        internal override sealed ParametroUtensile CreateParametro()
        {
            return new ParametroFresaCandela(this);
        }

        public double Altezza { get; set; }
    }
}