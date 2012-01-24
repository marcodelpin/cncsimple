using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.Model.ConversationalStructure.Abstraction
{
    public interface IWorkRotable
    {
        bool RotationAbilited { get; set; }
        double CenterRotationX { get; set; }
        double CenterRotationY { get; set; }
        double FirstAngle { get; set; }
        int NumberInstance { get; set; }
    }
}
