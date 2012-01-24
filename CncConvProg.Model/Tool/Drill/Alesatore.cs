using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.Tool.Parametro;
using MecPrev.Resources;

namespace CncConvProg.Model.Tool.Drill
{
    [Serializable]
    public class Alesatore : DrillTool
    {
        public Parametro.ParametroPunta ParametroPunta
        {
            get { return ParametroUtensile as ParametroPunta; }
            //set { ParametroUtensile = value; }
        }
        public Alesatore(MeasureUnit measureUnit)
            : base(measureUnit)
        {
            ToolName = GuiRes.Reamer;

        }
    }
}
