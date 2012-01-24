using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.Tool.Parametro;
using MecPrev.Resources;

namespace CncConvProg.Model.Tool.Drill
{
    [Serializable]
    public class Centrino : DrillTool
    {
        public Parametro.ParametroPunta ParametroPunta
        {
            get { return ParametroUtensile as ParametroPunta; }
            //set { ParametroUtensile = value; }
        }
        public Centrino(MeasureUnit measureUnit)
            : base(measureUnit)
        {
            ToolName = GuiRes.CenterDrill;

        }
    }
}
