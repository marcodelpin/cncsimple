using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;

namespace CncConvProg.Model.Tool.Drill
{
    [Serializable]
    public abstract class DrillTool : Utensile , IDiametrable
    {
        protected DrillTool(MeasureUnit measureUnit)
            : base(measureUnit)
        {
            ParametroUtensile = CreateParametro();
        }
        public double Diametro { get; set; }

        internal override sealed ParametroUtensile CreateParametro()
        {
            return new ParametroPunta(this);
        }

        internal double GetDrillStep()
        {
            if (ParametroUtensile == null)
                throw new NullReferenceException();

            if (ParametroUtensile is ParametroPunta)
            {
                var par = ParametroUtensile as ParametroPunta;
                return par.Step;
            }

            return 0;
        }

    }
}