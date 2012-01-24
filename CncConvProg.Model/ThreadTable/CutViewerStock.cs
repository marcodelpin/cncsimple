using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.Model.Simulation
{
    [Serializable]
    public class CutViewerStock
    {
        public CutViewerStock()
        {
            Larghezza = 100;
            Altezza = 100;
            Spessore = 40;
            OriginX = 50;
            OriginY = 50;
            OriginZ = -1;
        }
        public double Larghezza { get; set; }
        public double Altezza { get; set; }
        public double Spessore { get; set; }

        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public double OriginZ { get; set; }
    }

    [Serializable]
    public class CylinderStock
    {

    }

    [Serializable]
    public class BlockStock
    {

    }
}
