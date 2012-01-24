using System;
using CncConvProg.Model.Tool.Mill;

namespace CncConvProg.Model.Tool.Parametro
{
    [Serializable]
    public class ParametroFresaSpianare : ParametroFresaBase
    {
        private FresaSpianare Mill
        {
            get { return Utensile as FresaSpianare; }

        }

        public double DiametroMinimoFresa { get { return Mill.Diametro; } }

        public double DiametroIngombroFresa { get { return Mill.DiametroIngombroFresa; } }

        internal ParametroFresaSpianare(FresaSpianare tool)
            : base(tool)
        {
        }


        internal double GetRaggioInserto()
        {
            return Mill.RaggioInserto;
        }
    }
}