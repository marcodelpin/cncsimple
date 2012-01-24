using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    public enum EnumPatternMilling
    {
        ProfiloLibero,
        PoligonoRegolare,
        CavaArco,
        Cava,
        Rettangolo,
        Cerchio,
        CavaDritta,

        // Patten lavorazioni aperte
        CavaDrittaAperta,
        CavaDrittaApertaSuCirconferenza

    }

    public interface IMillingPattern
    {
        // Se il calcolo del profilo non è possibile ritorna null
        Geometry.Entity.Profile2D GetClosedProfile();
    }

    public interface IOpenMillingPattern : IMillingPattern
    {

        double MaterialToRemove { get; }
        Profile2D GetTrimmingProfile();
    }
}
