using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Abstraction.IPattern;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura.Interface;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.ThreadTable;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Foratura
{
    /// <summary>
    /// Foratura Semplice
    /// </summary>
    [Serializable]
    public sealed class ForaturaSemplice : DrillBaseClass
    {
        public override List<Operazione> Operazioni
        {
            get
            {
                var opList = new List<Operazione> { Centrinatura, Foratura, Svasatura };
                return opList;
            }
        }

        public ForaturaSemplice(bool foraturaCentraleTornio)
            : base(foraturaCentraleTornio)
        {

            DiametroForatura = 10;

            ProfonditaForatura = 25;

            SicurezzaZ = 2;

            InizioZ = 0;

            ProfonditaCentrino = 4;

            ProfonditaSvasatura = 4;
        }


        public override string Descrizione
        {
            get { return "Drilling " + DiametroForatura + " "; }
        }

        public override double DiametroPreview
        {
            get { return DiametroForatura; }
        }
    }



    /// <summary>
    /// Maschiatura
    /// </summary>
    [Serializable]
    public sealed class Maschiatura : DrillBaseClass, IMaschiaturaAble
    {
        public double ProfonditaMaschiatura { get; set; }

        public SensoMaschiatura SensoMaschiatura { get; set; }

        public bool MaschiaturaAbilitata
        {
            get
            {
                return MaschiaturaMaschioOp.Abilitata;
            }
            set { MaschiaturaMaschioOp.Abilitata = value; }
        }

        public Maschiatura(bool foroCentraleTornio)
            : base(foroCentraleTornio)
        {
            MaschiaturaMaschioOp = new Operazione(this, LavorazioniEnumOperazioni.ForaturaMaschiaturaDx);

            SensoMaschiatura = SensoMaschiatura.Dx;
        }

        public double PassoMetrico { get; set; }

        public override List<Operazione> Operazioni
        {
            get
            {
                var opList = new List<Operazione> { Centrinatura, Foratura, MaschiaturaMaschioOp, Svasatura };

                return opList;
            }
        }

        public Operazione MaschiaturaMaschioOp { get; set; }

        public RigaTabellaFilettatura MaschiaturaSelezionata { get; set; }
        //{
        //    get
        //    {
        //        return _maschiaturaSelezionata;
        //    }
        //    set
        //    {
        //        _maschiaturaSelezionata = value;

        //        if (_maschiaturaSelezionata == null) return;

        //        DiametroNominale.SetValue(false, _maschiaturaSelezionata.DiametroMetrico);

        //        PassoMetrico.SetValue(false, _maschiaturaSelezionata.Passo);
        //    }
        //}

        public override string Descrizione
        {
            get
            {
                var descMasch = string.Empty;

                if (MaschiaturaSelezionata != null)
                    descMasch = MaschiaturaSelezionata.Descrizione;

                return "Tapping " + descMasch;
            }
        }

        public override double DiametroPreview
        {
            get
            {
                return MaschiaturaSelezionata != null ? MaschiaturaSelezionata.DiametroMetrico : 1;
            }
        }

        public bool FilettaturaSinistra { get; set; }

        public bool ParametriFilettaturaPersonalizzati { get; set; }
    }


    /// <summary>
    /// Lamatura
    /// </summary>
    [Serializable]
    public sealed class Lamatura : DrillBaseClass, ILamaturaAble
    {
        public override List<Operazione> Operazioni
        {
            get
            {
                var opList = new List<Operazione> { Centrinatura, Foratura, LamaturaOp, Svasatura };

                return opList;
            }
        }

        public Lamatura(bool foraturaCentraleTornio)
            : base(foraturaCentraleTornio)
        {

            DiametroForatura = 10;

            ProfonditaForatura = 25;

            SicurezzaZ = 2;

            InizioZ = 0;

            ProfonditaCentrino = 4;

            ProfonditaSvasatura = 4;

            LamaturaOp = new Operazione(this, LavorazioniEnumOperazioni.ForaturaLamatore);
        }

        public Operazione LamaturaOp { get; set; }

        public override string Descrizione
        {
            get { return "Counterbore " + DiametroLamatura; }
        }

        public override double DiametroPreview
        {
            get { return DiametroForatura; }
        }

        public double ProfonditaLamatura { get; set; }

        public double DiametroLamatura { get; set; }

        public bool LamaturaAbilitata
        {
            get { return LamaturaOp.Abilitata; }
            set { LamaturaOp.Abilitata = value; }
        }

        public int ModalitaLamatura { get; set; }
    }

    /// <summary>
    /// Alesatura
    /// </summary>
    [Serializable]
    public sealed class Alesatura : DrillBaseClass, IAlesatoreAble
    {
        public override List<Operazione> Operazioni
        {
            get
            {
                var opList = new List<Operazione> { Centrinatura, Foratura, AlesaturaOp, Svasatura };

                return opList;
            }
        }

        public Alesatura(bool foraturaCentraleTornio)
            : base(foraturaCentraleTornio)
        {

            DiametroForatura = 10;

            ProfonditaForatura = 25;

            SicurezzaZ = 2;

            InizioZ = 0;

            ProfonditaCentrino = 4;

            ProfonditaSvasatura = 4;

            AlesaturaOp = new Operazione(this, LavorazioniEnumOperazioni.ForaturaAlesatore);
        }

        public Operazione AlesaturaOp { get; set; }
        public override string Descrizione
        {
            get { return "Drilling " + DiametroForatura; }
        }

        public override double DiametroPreview
        {
            get { return DiametroForatura; }
        }

        public double ProfonditaAlesatore { get; set; }

        public bool AlesaturaAbilitata
        {
            get { return AlesaturaOp.Abilitata; }
            set { AlesaturaOp.Abilitata = value; }
        }
    }

    /// <summary>
    /// Foratura Semplice
    /// </summary>
    [Serializable]
    public sealed class Barenatura : DrillBaseClass, IBarenoAble
    {
        public override List<Operazione> Operazioni
        {
            get
            {
                var opList = new List<Operazione> { Centrinatura, Foratura, BarenaturaOp, AllargaturaOp, Svasatura };

                return opList;
            }
        }

        public Barenatura(bool foraturaCentraleTornio)
            : base(foraturaCentraleTornio)
        {

            DiametroForatura = 10;

            ProfonditaForatura = 25;

            SicurezzaZ = 2;

            InizioZ = 0;

            ProfonditaCentrino = 4;

            ProfonditaSvasatura = 4;

            BarenaturaOp = new Operazione(this, LavorazioniEnumOperazioni.ForaturaBareno);

            AllargaturaOp = new Operazione(this, LavorazioniEnumOperazioni.AllargaturaBareno);

        }

        public Operazione BarenaturaOp { get; set; }

        public Operazione AllargaturaOp { get; set; }

        public override string Descrizione
        {
            get { return "Boring " + DiametroBarenatura + " x " + ProfonditaBareno; }
        }

        public override double DiametroPreview
        {
            get { return DiametroForatura; }
        }

        public double ProfonditaBareno { get; set; }

        public double DiametroBarenatura { get; set; }

        public double DiametroAllargatura { get; set; }

        public bool BarenoAbilitato { get { return BarenaturaOp.Abilitata; } set { BarenaturaOp.Abilitata = value; } }

        public bool AllargaturaAbilitata { get; set; }

        public int ModalitaAllargatura { get; set; }

        public double MaterialePerFinitura { get; set; }
    }



}
