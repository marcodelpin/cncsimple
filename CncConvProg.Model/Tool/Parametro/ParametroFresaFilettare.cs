using System;
using CncConvProg.Model.Tool.Mill;

namespace CncConvProg.Model.Tool.Parametro
{
    [Serializable]
    public class ParametroFresaFilettare : ParametroFresaBase
    {
        internal ParametroFresaFilettare(FresaFilettare tool)
            : base(tool)
        {
        }
    }
}