using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    [Serializable]
    public class FreeProfilePattern : IMillingPattern
    {
        private readonly RawProfile _rawProfile = new RawProfile(false);

        public FreeProfilePattern()
        {
           _rawProfile =  CreateDefaultData();
        }

        private static RawProfile CreateDefaultData()
        {
            var profile = new RawProfile(false);

            var ini = new RawInitPoint2D(profile);
            ini.X.SetValue(true, 0);
            ini.Y.SetValue(true, 0);

            var line1 = new RawLine2D(profile);
            line1.DeltaY.SetValue(true, 10);

            var line2 = new RawLine2D(profile);
            line2.DeltaX.SetValue(true, -10);

            var line3 = new RawLine2D(profile);
            line3.Y.SetValue(true, 0);

            var line4 = new RawLine2D(profile);
            line4.X.SetValue(true, 0);


            profile.Add(ini);
            profile.Add(line1);
            profile.Add(line2);
            profile.Add(line3);
            profile.Add(line4);

            return profile;
        }

        public Profile2D GetClosedProfile()
        {
            var result = _rawProfile.GetProfileResult(true);
            
            return result;
        }

        public RawProfile GetRawProfile()
        {
            return _rawProfile;
        }
    }
}