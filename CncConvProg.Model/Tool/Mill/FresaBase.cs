using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.Tool.Mill
{
    [Serializable]
    public abstract class FresaBase : Utensile, IDiametrable
    {
        protected FresaBase(MeasureUnit measureUnit)
            : base(measureUnit)
        {
        }

        //private string _toolDescription;
        //public override string ToolDescription
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(_toolDescription))
        //            _toolDescription = ToolName + " " + Diametro + " " + GetMeasureDescp();

        //        return _toolDescription;
        //    }

        //    set { _toolDescription = value; }
       // }
        

        public double Diametro { get; set; }
    }
}