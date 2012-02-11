using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura.GolePattern
{
    public abstract class GrooveVShape : IGroovePattern
    {
        public double DiametroExt { get; set; }

        public double DiametroMin { get; set; }

        public double LunghezzaEst { get; set; }

        public double LunghezzaInt { get; set; }

        public double ExternChamferValue { get; set; }

        public double InternChamferValue { get; set; }

        public double CenterZ { get; set; }

        public bool ChamferExternAbilited { get; set; }

        public bool ChamferInternAbilited { get; set; }

        public Profile2D Profile
        {
            get
            {
                return CalculateProfile();
            }
        }

        protected abstract Profile2D CalculateProfile();
    }

    /// <summary>
    /// Gola Esterna V Shape
    /// </summary>
    public class GrooveVShapeExtern : GrooveVShape
    {
        /// <summary>
        /// Crea profilo per gola a V Esterna
        /// Comprensivo di smussi.
        /// </summary>
        /// <returns></returns>
        protected override Profile2D CalculateProfile()
        {
            var rawProfile = new RawProfile(false);
            var ini = new RawInitPoint2D(rawProfile);
            ini.X.SetValue(true, CenterZ + LunghezzaEst / 2 + ExternChamferValue);
            ini.Y.SetValue(true, DiametroExt);
            rawProfile.Add(ini);

            var p1 = new RawLine2D(rawProfile);
            p1.X.SetValue(true, CenterZ + LunghezzaEst / 2);
            rawProfile.Add(p1);

            if (ExternChamferValue > 0 && ChamferExternAbilited)
            {
                p1.Chamfer.SetValue(true, ExternChamferValue);
            }
            else if (ExternChamferValue > 0 && !ChamferExternAbilited)
            {
                p1.EndRadius.SetValue(true, ExternChamferValue);
            }

            var p2 = new RawLine2D(rawProfile);
            p2.X.SetValue(true, CenterZ + LunghezzaInt / 2);
            p2.Y.SetValue(true, DiametroMin);
            rawProfile.Add(p2);

            if (InternChamferValue > 0 && ChamferInternAbilited)
            {
                p2.Chamfer.SetValue(true, ExternChamferValue);
            }
            else if (InternChamferValue > 0 && !ChamferInternAbilited)
            {
                p2.EndRadius.SetValue(true, ExternChamferValue);
            }

            // P4
            var p4 = new RawLine2D(rawProfile);
            p4.X.SetValue(true, CenterZ - LunghezzaInt / 2);
            rawProfile.Add(p2);

            if (InternChamferValue > 0 && ChamferInternAbilited)
            {
                p4.Chamfer.SetValue(true, ExternChamferValue);
            }
            else if (InternChamferValue > 0 && !ChamferInternAbilited)
            {
                p4.EndRadius.SetValue(true, ExternChamferValue);
            }

            // P5 
            var p5 = new RawLine2D(rawProfile);
            p5.X.SetValue(true, CenterZ - LunghezzaEst / 2);
            rawProfile.Add(p5);

            if (ExternChamferValue > 0 && ChamferExternAbilited)
            {
                p5.Chamfer.SetValue(true, ExternChamferValue);
            }
            else if (ExternChamferValue > 0 && !ChamferExternAbilited)
            {
                p5.EndRadius.SetValue(true, ExternChamferValue);
            }

            // P6 - Last Point
            var p6 = new RawLine2D(rawProfile);
            p6.X.SetValue(true, CenterZ - LunghezzaEst / 2 - ExternChamferValue);
            rawProfile.Add(p6);

            var rslt = rawProfile.GetProfileResult(true);

            rslt.SetPlotStyle();

            return rslt;
        }
    }

    /// <summary>
    /// Gola Esterna V Shape
    /// </summary>
    public class GrooveVShapeIntern : GrooveVShape
    {
        /// <summary>
        /// Crea profilo per gola a V Esterna
        /// Comprensivo di smussi.
        /// </summary>
        /// <returns></returns>
        protected override Profile2D CalculateProfile()
        {
            var rawProfile = new RawProfile(false);
            var ini = new RawInitPoint2D(rawProfile);
            ini.X.SetValue(true, CenterZ + LunghezzaEst / 2 + ExternChamferValue);
            ini.Y.SetValue(true, DiametroExt);
            rawProfile.Add(ini);

            var p1 = new RawLine2D(rawProfile);
            p1.X.SetValue(true, CenterZ + LunghezzaEst / 2);
            rawProfile.Add(p1);

            if (ExternChamferValue > 0 && ChamferExternAbilited)
            {
                p1.Chamfer.SetValue(true, ExternChamferValue);
            }
            else if (ExternChamferValue > 0 && !ChamferExternAbilited)
            {
                p1.EndRadius.SetValue(true, ExternChamferValue);
            }

            var p2 = new RawLine2D(rawProfile);
            p2.X.SetValue(true, CenterZ + LunghezzaInt / 2);
            p2.Y.SetValue(true, DiametroMin);
            rawProfile.Add(p2);

            if (InternChamferValue > 0 && ChamferInternAbilited)
            {
                p2.Chamfer.SetValue(true, ExternChamferValue);
            }
            else if (InternChamferValue > 0 && !ChamferInternAbilited)
            {
                p2.EndRadius.SetValue(true, ExternChamferValue);
            }

            // P4
            var p4 = new RawLine2D(rawProfile);
            p4.X.SetValue(true, CenterZ - LunghezzaInt / 2);
            rawProfile.Add(p2);

            if (InternChamferValue > 0 && ChamferInternAbilited)
            {
                p4.Chamfer.SetValue(true, ExternChamferValue);
            }
            else if (InternChamferValue > 0 && !ChamferInternAbilited)
            {
                p4.EndRadius.SetValue(true, ExternChamferValue);
            }

            // P5 
            var p5 = new RawLine2D(rawProfile);
            p5.X.SetValue(true, CenterZ - LunghezzaEst / 2);
            rawProfile.Add(p5);

            if (ExternChamferValue > 0 && ChamferExternAbilited)
            {
                p5.Chamfer.SetValue(true, ExternChamferValue);
            }
            else if (ExternChamferValue > 0 && !ChamferExternAbilited)
            {
                p5.EndRadius.SetValue(true, ExternChamferValue);
            }

            // P6 - Last Point
            var p6 = new RawLine2D(rawProfile);
            p6.X.SetValue(true, CenterZ - LunghezzaEst / 2 - ExternChamferValue);
            rawProfile.Add(p6);

            var rslt = rawProfile.GetProfileResult(true);

            rslt.SetPlotStyle();

            return rslt;
        }
    }
}
