using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura.GolePattern
{
    public class GrooveExternStandard : IGroovePattern
    {
        public double DiametroExt { get; set; }

        public double DiametroMin { get; set; }

        public double Lunghezza { get; set; }

        public double ExternChamferValue { get; set; }

        public double InternChamferValue { get; set; }

        public double StartZ { get; set; }

        public Profile2D Profile
        {
            get
            {
                var rawProfile = new RawProfile(true);

                var iniPnt = new RawInitPoint2D(rawProfile);
                iniPnt.Y.SetValue(true, StartZ);
                iniPnt.X.SetValue(true, DiametroExt);

                var l = new RawLine2D(rawProfile);
                l.Y.SetValue(true, DiametroMin);

                var l1 = new RawLine2D(rawProfile);
                l1.X.SetValue(true, StartZ - Lunghezza);

                var l2 = new RawLine2D(rawProfile);
                l2.Y.SetValue(true, DiametroExt);

                return rawProfile.GetProfileResult(false);
            }
        }
    }

    public enum EnumGroovePattern
    {
        EsternaStandard
    }
}
