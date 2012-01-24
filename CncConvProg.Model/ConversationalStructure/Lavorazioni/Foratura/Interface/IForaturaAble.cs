using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface
{
    /// <summary>
    /// Interfaccia per le le lavorazioni dove è presente foratura
    /// </summary>
    public interface IForaturaAble
    {
        //double InizioZ { get; set; }

        //double SicurezzaZ { get; set; }

        bool ForaturaAbilitata { get; set; }

        double DiametroForatura { get; set; }

        double ProfonditaForatura { get; set; }

        ModalitaForatura ModalitaForatura { get; set; }

        //List<Point2D> GetDrillPointList();
    }
}

