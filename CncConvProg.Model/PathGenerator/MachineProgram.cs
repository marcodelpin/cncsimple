using System;
using System.Collections.Generic;

namespace CncConvProg.Model.PathGenerator
{
    /// <summary>
    /// Classe che contiene tutte le informazioni per crare il programma GCODE
    /// </summary>
    [Serializable]
    public class MachineProgram
    {
        /// <summary>
        /// Costruttore classe
        /// </summary>
        /// <param name="measurUnit">Unità di misura utilizzato</param>
        public MachineProgram(MeasureUnit measurUnit)
        {
            CreationTime = DateTime.Now;
            Operazioni = new List<ProgramOperation>();
            MeasureUnit = measurUnit;
        }

        /// <summary>
        /// Unita di misura utilizzato [mm] o [inch]
        /// </summary>
        public MeasureUnit MeasureUnit { get; private set; }

        /// <summary>
        /// Lista delle varie operazioni
        /// </summary>
        public List<ProgramOperation> Operazioni { get; set; }

        /// <summary>
        /// Numero programma
        /// </summary>
        public int ProgramNumber { get; set; }

        /// <summary>
        /// Commento programma
        /// </summary>
        public string ProgramComment { get; set; }

        /// <summary>
        /// Data Creazione
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// Z di sicurezza alla quale sposto utensile fra 2 operazioni che usano stesso utensile.
        /// </summary>
        public double ZSicurezzaNoCambioUtensile { get; set; }

        /// <summary>
        /// Stringa da inserire per settare il grezzo in CutViewer
        /// </summary>
        public string CutViewerStockSettingStr { get; set; }
    }
}