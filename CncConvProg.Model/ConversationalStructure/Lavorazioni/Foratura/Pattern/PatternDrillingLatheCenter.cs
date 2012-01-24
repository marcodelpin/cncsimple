using System;
using System.Collections.Generic;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Pattern
{
    [Serializable]
    public class PatternDrillingLatheCenter : IPatternDrilling
    {
        public List<Point2D> GetPointList()
        {
            return new List<Point2D> { new Point2D(0, 0) };
        }
    }
}