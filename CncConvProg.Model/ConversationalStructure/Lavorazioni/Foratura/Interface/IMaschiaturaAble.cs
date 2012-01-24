using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface
{
    public interface IMaschiaturaAble
    {
        double InizioZ { get; set; }
        double SicurezzaZ { get; set; }

        double ProfonditaMaschiatura { get; set; }

        SensoMaschiatura SensoMaschiatura { get; set; }

        bool MaschiaturaAbilitata { get; set; }

        //bool FresaturaInterpolazioneEsterna { get; set; }

        //double DiametroFinaleDiFresatura { get; set; }

        double PassoMetrico { get; set; }

        //ModoMaschiatura ModoMaschiatura { get; set; }

        List<Point2D> GetDrillPointList();

    }

    public enum SensoMaschiatura
    {
        Dx,
        Sx,
    }

    //public enum ModoMaschiatura
    //{
    //    Maschio,
    //    FresaFilettare
    //}
}
