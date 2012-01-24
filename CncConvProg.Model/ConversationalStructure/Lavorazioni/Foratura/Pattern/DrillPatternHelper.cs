using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern
{
    internal static class DrillPatternHelper
    {
        internal static IPatternDrilling UpdatePattern(PatternForatura patternForatura)
        {
            switch (patternForatura)
            {
                case PatternForatura.Circolare:
                    {
                        return new PatternDrillingCircle();

                    } break;

                case PatternForatura.Rettangolare:
                    {
                        return new PatternDrillingRectangle();

                    } break;

                case PatternForatura.Arco:
                    {
                        return new PatternDrillingArc();

                    } break;

                case PatternForatura.Linea:
                    {
                        return new PatternDrillingLine();

                    } break;

                case PatternForatura.CoordinateRc:
                    {
                        return new PatternDrillingRc();

                    } break;

                case PatternForatura.CoordinateXy:
                    {
                        return new PatternDrillingXy();

                    } break;

                case PatternForatura.TornioForaturaCentrale:
                    {
                        return new PatternDrillingLatheCenter();

                    } break;

                default:
                    throw new NotImplementedException("ForaturaSemplice.UpdatePattern");
            }
        }
    }
}
