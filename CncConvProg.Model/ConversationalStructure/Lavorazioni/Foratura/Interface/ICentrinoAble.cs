using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface
{
    public interface ICentrinoAble
    {
        //double InizioZ { get; set; }

        //double SicurezzaZ { get; set; }

        double ProfonditaCentrino { get; set; }

        bool CentrinoAbilitato { get; set; }

        //List<Point2D> GetDrillPointList();

    }
}
