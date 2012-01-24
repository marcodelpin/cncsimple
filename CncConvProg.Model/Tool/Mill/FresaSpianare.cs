using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.Tool.Parametro;
using MecPrev.Resources;

namespace CncConvProg.Model.Tool.Mill
{
    [Serializable]
    public class FresaSpianare : FresaBase
    {
        public ParametroFresaSpianare ParametroFresaSpianare
        {
            get
            {
                return ParametroUtensile as ParametroFresaSpianare;
            }
        }

        public FresaSpianare(MeasureUnit measureUnit)
            : base(measureUnit)
        {
            ParametroUtensile = CreateParametro();
            ToolName = GuiRes.FaceMill;
           


        }

        public double DiametroIngombroFresa { get; set; }

        public double RaggioInserto { get; set; }

        internal override sealed ParametroUtensile CreateParametro()
        {
            return new ParametroFresaSpianare(this);
        }

        public double Altezza { get; set; }
    }
}