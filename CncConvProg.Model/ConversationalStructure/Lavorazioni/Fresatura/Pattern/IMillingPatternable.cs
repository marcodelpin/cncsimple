using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    public interface IMillingPatternable
    {
        EnumPatternMilling MillingPattern { get; set; }

        IMillingPattern Pattern { get; }

    }
}