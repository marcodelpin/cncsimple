using System;

namespace CncConvProg.Model.FileManageUtility
{
    [Serializable]
    public class ProgramPreference
    {
        public ProgramPreference()
        {
            _mmPreference = new UnitPreference
                                {
                                    MillEntryExitSecureDistance = 5,
                                    MillingRapidSecureFeedAsync = 2000,
                                    TurningSecureDistance = 2,
                                    TurningRapidSecureFeedSync = .5,
                                    MillingSecureZNoChangeTool = 50,
                                    DistanzaSicurezzaCicliForatura = 2,
                                };

            _inchPreference = new UnitPreference
                                  {
                                      MillEntryExitSecureDistance = .2,
                                      MillingRapidSecureFeedAsync = 80,
                                      TurningSecureDistance = .08,
                                      TurningRapidSecureFeedSync = .2,
                                      MillingSecureZNoChangeTool = 2.5,
                                      DistanzaSicurezzaCicliForatura = 2 / 25.4,
                                  };
        }

        public UnitPreference GetPreference(MeasureUnit measureUnit)
        {
            return measureUnit == MeasureUnit.Millimeter ? _mmPreference : _inchPreference;
        }

        private readonly UnitPreference _mmPreference;
        private readonly UnitPreference _inchPreference;

        public MeasureUnit DefaultMeasureUnit { get; set; }

        public double NoChangeToolSecureZMm { get; set; }
    }

    [Serializable]
    public class UnitPreference
    {
        #region Mill Preference

        public double MillEntryExitSecureDistance { get; set; }
        public double MillingRapidSecureFeedAsync { get; set; }
        public double MillingSecureZNoChangeTool { get; set; }


        #endregion

        #region Lathe Preference

        public double TurningSecureDistance { get; set; }
        public double TurningRapidSecureFeedSync { get; set; }

        public double DistaccoSgrossaturaTornitura { get; set; }

        public double DistanzaSicurezzaCicliForatura { get; set; }

        #endregion

    }

}

