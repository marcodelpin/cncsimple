using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.Tool.Parametro;
using MecPrev.Resources;

namespace CncConvProg.Model.Tool.Drill
{
    [Serializable]
    public class Maschio : DrillTool
    {
        public Parametro.ParametroPunta ParametroPunta
        {
            get { return ParametroUtensile as ParametroPunta; }
            //set { ParametroUtensile = value; }
        }
        public Maschio(MeasureUnit measureUnit)
            : base(measureUnit)
        {
            ToolName = GuiRes.Tap;

        }
    }
}