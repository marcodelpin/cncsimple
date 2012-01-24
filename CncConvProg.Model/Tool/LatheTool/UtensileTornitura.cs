using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.Tool.Parametro;
using MecPrev.Resources;

namespace CncConvProg.Model.Tool.LatheTool
{
    /// <summary>
    /// dividere fra esterno e interno.
    /// </summary>
    [Serializable]
    public class UtensileTornitura : Utensile
    {
        public UtensileTornitura(MeasureUnit measureUnit)
            : base(measureUnit)
        {
            ToolName = GuiRes.TurningTool;

            ParametroUtensile = CreateParametro();

        }


        internal override sealed ParametroUtensile CreateParametro()
        {
            return new ParametroUtensileTornitura(this);
        }
    }
}