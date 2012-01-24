using System;
using CncConvProg.Geometry.PreviewPathEntity;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.PathGenerator;

namespace CncConvProg.Model.Tool.Parametro
{
    [Serializable]
    public class ParametroUtensileTornituraScanalatura : ParametroUtensileTornitura
    {
        internal ParametroUtensileTornituraScanalatura(Utensile tool)
            : base(tool)
        {

        }
        public double LarghezzaPassata
        {
            get
            {
                return ProfonditaPassata;
            }
            set { ProfonditaPassata = value; }
        }
        public double Step { get; set; }
    }
}