using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Abstraction.IPattern
{
    public interface IPatternDrilling
    {
        List<Point2D> GetPointList();
     }
}
