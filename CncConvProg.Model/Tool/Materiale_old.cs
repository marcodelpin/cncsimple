using System;

namespace CncConvProg.Model.Tool
{
    [Serializable]
    public class Materiale : IEquatable<Materiale>
    {
        public Guid MaterialeGuid { get; private set; }

        public Materiale()
        {
            MaterialeGuid = Guid.NewGuid();
        }

        public EnumMaterialType MaterialType { get; set; }

        public string Descrizione { get; set; }



        public bool Equals(Materiale other)
        {
            return other.MaterialeGuid == MaterialeGuid;
        }
    }


    public enum EnumMaterialType
    {
        P,
        M,
        K,
        H,
        N,
    }


}