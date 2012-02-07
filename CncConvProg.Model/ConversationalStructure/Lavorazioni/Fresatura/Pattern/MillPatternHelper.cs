using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern
{
    internal static class MillPatternHelper
    {
        internal static IMillingPattern GetPattern(EnumPatternMilling patternMilling)
        {
            switch (patternMilling)
            {
                case EnumPatternMilling.PoligonoRegolare:
                    {
                        return new RegularPolygonPattern();

                    } break;

                case EnumPatternMilling.ProfiloLibero:
                    {
                        return new FreeProfilePattern();

                    } break;

                case EnumPatternMilling.CavaArco:
                    {
                        return new CavaArcoPattern();

                    } break;

                case EnumPatternMilling.Cerchio:
                    {
                        return new CirclePattern();

                    } break;

                case EnumPatternMilling.CavaDritta:
                    {
                        return new CavaDrittaPattern();

                    } break;

                case EnumPatternMilling.CavaDrittaAperta:
                    {
                        return new CavaDrittaApertaPattern();

                    } break;


                case EnumPatternMilling.Rettangolo:
                    {
                        return new RettangoloPattern();

                    } break;

                default:
                    throw new NotImplementedException("FresaturaContornatura.UpdatePattern");
            }
        }
    }
}
