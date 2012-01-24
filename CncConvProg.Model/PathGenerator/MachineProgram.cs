using System;
using System.Collections.Generic;

namespace CncConvProg.Model.PathGenerator
{
    [Serializable]
    public class MachineProgram
    {
        public MachineProgram(MeasureUnit measurUnit)
        {
            CreationTime = DateTime.Now;
            Operations = new List<ProgramPhase>();
            MeasureUnit = measurUnit;
        }

        public MeasureUnit MeasureUnit { get; private set; }
        public List<ProgramPhase> Operations { get; set; }

        public int ProgramNumber { get; set; }

        public string ProgramComment { get; set; }

        public DateTime CreationTime { get; private set; }

        public double NoChangeToolSecureZ { get; set; }

        public string CutViewerStockSettingStr { get; set; }
    }
}