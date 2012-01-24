using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CncConvProg.Model.Tool
{
    [Serializable]
    public abstract class ToolHolder
    {
        public int NumeroPostazione { get; set; }

        public bool CoolantOn { get; set; }

        public abstract void GetToolDefaultData(Utensile tool);
    }

    [Serializable]
    public class LatheToolHolder : ToolHolder
    {
        public int NumeroCorrettore { get; set; }

        public override void GetToolDefaultData(Utensile tool)
        {
            NumeroPostazione = tool.LatheToolHolder.NumeroPostazione;
            NumeroCorrettore = tool.LatheToolHolder.NumeroCorrettore;
        }
    }

    [Serializable]
    public class MillToolHolder : ToolHolder
    {
        public string NumeroCorrettoreLunghezza { get; set; }
        public string NumeroCorrettoreRaggio { get; set; }

        public override void GetToolDefaultData(Utensile tool)
        {
            NumeroPostazione = tool.MillToolHolder.NumeroPostazione;
            NumeroCorrettoreLunghezza = tool.MillToolHolder.NumeroCorrettoreLunghezza;
            NumeroCorrettoreRaggio = tool.MillToolHolder.NumeroCorrettoreRaggio;
        }
    }
}