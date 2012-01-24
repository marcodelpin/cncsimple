using System;
using CncConvProg.Model.Tool.Mill;

namespace CncConvProg.Model.Tool.Parametro
{
    [Serializable]
    public class ParametroFresaCandela : ParametroFresaBase
    {
        internal ParametroFresaCandela(FresaCandela tool)
            : base(tool)
        {
        }
    }

    [Serializable]
    public class ParametroFresaCandelaTrocoidal : ParametroFresaCandela
    {
        internal ParametroFresaCandelaTrocoidal(FresaCandela tool)
            : base(tool)
        {
           
        }

        public double StepSpiral { get; set; }
    }

    [Serializable]
    public class ParametroFresaCandelaTrocoidalLarge : ParametroFresaCandela
    {
        internal ParametroFresaCandelaTrocoidalLarge(FresaCandela tool)
            : base(tool)
        {
        }
    }

}