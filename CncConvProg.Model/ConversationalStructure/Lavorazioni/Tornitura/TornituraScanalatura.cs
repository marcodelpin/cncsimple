using System;
using System.Collections.Generic;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura.GolePattern;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool.LatheTool;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura
{
    /// <summary>
    /// Classe per creare scanalature con tornio.
    /// Può avere immissione profilo o template con gole con forma standard.
    /// </summary>
    [Serializable]
    public class TornituraScanalatura : LavorazioneTornitura, IGroovePatternable
    {
        public readonly GrooveDirection ScanalaturaType;

        public TornituraScanalatura(GrooveDirection grooveDirection)
            : base()
        {
            Sgrossatura = new Operazione(this, LavorazioniEnumOperazioni.TornituraScanalaturaSgrossatura);

            Finitura = new Operazione(this, LavorazioniEnumOperazioni.TornituraScanalaturaFinitura);

            NumeroGole = 1;

            ScanalaturaType = grooveDirection;
        }

        public double DiameterIniziale { get; set; }

        public double DiameterFinale { get; set; }

        public double Larghezza { get; set; }

        public int NumeroGole { get; set; }

        public double DistanzaGole { get; set; }

        public double StartZ { get; set; }

        public override List<Operazione> Operazioni
        {
            get
            {
                return new List<Operazione> { Sgrossatura, Finitura };
            }
        }

        public Operazione Sgrossatura { get; set; }

        public Operazione Finitura { get; set; }

        public override string Descrizione
        {
            get { return MecPrev.Resources.GuiRes.TurnGroove; }
        }

        protected override void CreateSpecificProgram(ProgramOperation programPhase, Operazione operazione)
        {
            var parametro = operazione.Utensile.ParametroUtensile as ParametroUtensileTornituraScanalatura;

            var utensile = operazione.Utensile as UtensileScanalatura;
            if (parametro == null || utensile == null) return;

            var larghUtensile = utensile.LarghezzaUtensile;
            var larghPassata = parametro.LarghezzaPassata;
            var step = parametro.Step;

            var moveCollection = new MoveActionCollection();

            if (larghUtensile > Larghezza) return;


            for (int i = 0; i < NumeroGole; i++)
            {

                switch (ScanalaturaType)
                {
                    case GrooveDirection.Extern:
                        {
                            var stepGole = i * DistanzaGole;

                            var startZ = StartZ - stepGole;
                            var endZ = startZ + Larghezza - larghUtensile;
                            var dMax = Math.Max(DiameterIniziale, DiameterFinale);
                            var dMin = Math.Min(DiameterIniziale, DiameterFinale);

                            var dIni = dMax;
                            var dFin = dMin;
                            //var effectiveStart = startZ + larghUtensile;

                            switch (operazione.OperationType)
                            {
                                case LavorazioniEnumOperazioni.TornituraScanalaturaSgrossatura:
                                    {
                                        TurnProgrammingHelper.GetSgrossaturaGolaEsterna(moveCollection, dIni,
                                                                                        dFin, startZ, endZ, step,
                                                                                        larghPassata, 0);
                                    }
                                    break;

                                case LavorazioniEnumOperazioni.TornituraScanalaturaFinitura:
                                    {
                                        TurnProgrammingHelper.GetFinituraGolaEsternaInterna(moveCollection, DiameterIniziale, DiameterFinale, startZ, endZ, 0, ExtraCorsa);
                                    }
                                    break;
                            }
                        } break;

                    case GrooveDirection.Intern:
                        {
                            var stepGole = i * DistanzaGole;

                            var startZ = StartZ - stepGole;
                            var endZ = startZ + Larghezza - larghUtensile;

                            var dMax = Math.Max(DiameterIniziale, DiameterFinale);
                            var dMin = Math.Min(DiameterIniziale, DiameterFinale);

                            var dIni = dMin;
                            var dFin = dMax;
                            //var effectiveStart = startZ + larghUtensile;

                            switch (operazione.OperationType)
                            {
                                case LavorazioniEnumOperazioni.TornituraScanalaturaSgrossatura:
                                    {
                                        TurnProgrammingHelper.GetSgrossaturaGolaInterna(moveCollection, dIni,
                                                                                        dFin, startZ, endZ, step,
                                                                                        larghPassata, 0);
                                    }
                                    break;

                                case LavorazioniEnumOperazioni.TornituraScanalaturaFinitura:
                                    {
                                        TurnProgrammingHelper.GetFinituraGolaEsternaInterna(moveCollection, DiameterIniziale, DiameterFinale, startZ, endZ, 0, ExtraCorsa);


                                    }
                                    break;
                            }

                        } break;

                    case GrooveDirection.Face:
                        {
                            var stepGole = i * DistanzaGole;

                            var startZ = StartZ;
                            var endZ = startZ - Larghezza;

                            var dMax = Math.Max(DiameterIniziale, DiameterFinale);
                            var dMin = Math.Min(DiameterIniziale, DiameterFinale);
                            var diaIni = dMin + stepGole;
                            var diaFin = dMax + stepGole - larghUtensile;

                            switch (operazione.OperationType)
                            {
                                case LavorazioniEnumOperazioni.TornituraScanalaturaSgrossatura:
                                    {
                                        TurnProgrammingHelper.GetSgrossaturaGolaFrontale(moveCollection, diaIni,
                                                                                        diaFin, startZ, endZ, step,
                                                                                        larghPassata, 0);
                                    }
                                    break;

                                case LavorazioniEnumOperazioni.TornituraScanalaturaFinitura:
                                    {
                                        TurnProgrammingHelper.GetFinituraGolaFrontale(moveCollection, diaIni, diaFin, startZ, endZ, ExtraCorsa, 0);
                                    }
                                    break;
                            }

                        } break;

                }

            }

            foreach (var variable in moveCollection)
            {
                programPhase.AggiungiAzioneMovimento(variable);
            }
        }

        protected override List<IEntity3D> GetFinalPreview()
        {
            var rslt = new List<IEntity2D>();

            for (int i = 0; i < NumeroGole; i++)
            {


                switch (ScanalaturaType)
                {
                    case GrooveDirection.Intern:
                    case GrooveDirection.Extern:
                        {
                            var step = i * DistanzaGole;
                            var startZ = StartZ - step;
                            var endZ = startZ + Larghezza;

                            var p = new Line2D { Start = { X = startZ, Y = DiameterIniziale }, End = { X = startZ, Y = DiameterFinale } };
                            var p1 = new Line2D { Start = p.End, End = { X = endZ, Y = p.End.Y } };
                            var p2 = new Line2D { Start = p1.End, End = { X = p1.End.X, Y = DiameterIniziale } };

                            rslt.Add(p);
                            rslt.Add(p1);
                            rslt.Add(p2);

                        } break;

                    case GrooveDirection.Face:
                        {
                            var step = i * DistanzaGole;
                            var startD = DiameterIniziale + step;
                            var endD = DiameterFinale + step;

                            var endZ = StartZ - Larghezza;

                            var p = new Line2D { Start = { X = StartZ, Y = startD }, End = { X = endZ, Y = startD } };
                            var p1 = new Line2D { Start = p.End, End = { X = endZ, Y = endD } };
                            var p2 = new Line2D { Start = p1.End, End = { X = StartZ, Y = endD } };

                            rslt.Add(p);
                            rslt.Add(p1);
                            rslt.Add(p2);

                        } break;
                }


            }

            var l = Entity3DHelper.Get3DProfile(rslt);

            Entity3DHelper.SetPlotStyle(l);

            return new List<IEntity3D>(l);
        }

        private TurnGroovePattern _groovePattern;
        public TurnGroovePattern GroovePattern
        {
            get
            {
                return _groovePattern;
            }
            set
            {
                _groovePattern = value;
                UpdatePattern();
            }
        }

        private void UpdatePattern()
        {
            switch (GroovePattern)
            {
                case TurnGroovePattern.VShape:
                    {
                        Pattern = new GrooveVShapeExtern();
                    }
                    break;

            }
        }
        public IGroovePattern Pattern { get; private set; }
    }

    //[Serializable]
    //public sealed class TornituraScanalaturaEsterna : TornituraScanalatura
    //{
    //    public TornituraScanalaturaEsterna(Guid parent)
    //        : base(parent)
    //    {
    //    }

    //   

    //    //public override Dictionary<EnumGroovePattern, string> Patterns
    //    //{
    //    //    get
    //    //    {

    //    //        var d = new Dictionary<EnumGroovePattern, string> { { EnumGroovePattern.EsternaStandard, "Extern Standard" } };

    //    //        return d;
    //    //    }
    //    //}

    //}


    //[Serializable]
    //public sealed class TornituraScanalaturaInterna : TornituraScanalatura
    //{
    //    public TornituraScanalaturaInterna(Guid parent)
    //        : base(parent)
    //    {
    //    }

    //    protected override void CreateSpecificProgram(ProgramPhase programPhase, Operazione operazione)
    //    {
    //        var dIni = DiameterIniziale / 2;
    //        var dFin = DiameterFinale / 2;
    //        var larghUtensile = 3;
    //        var larghPassata = 2;
    //        var step = 2;

    //        var moveCollection = new MoveActionCollection();

    //        for (int i = 0; i < NumeroGole; i++)
    //        {

    //            var stepGole = i * DistanzaGole;

    //            var startZ = StartZ - stepGole;
    //            var endZ = startZ + Larghezza - larghUtensile;

    //            //var effectiveStart = startZ + larghUtensile;

    //            switch (operazione.OperationType)
    //            {
    //                case LavorazioniEnumOperazioni.TornituraScanalaturaSgrossatura:
    //                    {
    //                        TurnProgrammingHelper.GetSgrossaturaGolaEsterna(moveCollection, dIni,
    //                                                                        dFin, startZ, endZ, step,
    //                                                                        larghPassata, .1);
    //                    }
    //                    break;

    //                case LavorazioniEnumOperazioni.TornituraScanalaturaFinitura:
    //                    {

    //                    }
    //                    break;
    //            }
    //        }

    //        foreach (var variable in moveCollection)
    //        {
    //            programPhase.AddAction(variable);
    //        }
    //    }

    //    protected override List<IEntity3D> GetFinalPreview()
    //    {
    //        var rslt = new List<IEntity2D>();

    //        for (int i = 0; i < NumeroGole; i++)
    //        {
    //            var step = i * DistanzaGole;
    //            var startZ = StartZ - step;
    //            var endZ = startZ + Larghezza;

    //            var p = new Line2D { Start = { X = startZ, Y = DiameterIniziale / 2 }, End = { X = startZ, Y = DiameterFinale / 2 } };

    //            var p1 = new Line2D { Start = p.End, End = { X = endZ, Y = p.End.Y } };

    //            var p2 = new Line2D { Start = p1.End, End = { X = p1.End.X, Y = DiameterIniziale / 2 } };

    //            rslt.Add(p);
    //            rslt.Add(p1);
    //            rslt.Add(p2);
    //        }

    //        var l = Entity3DHelper.Get3DProfile(rslt);

    //        Entity3DHelper.SetPlotStyle(l);

    //        return new List<IEntity3D>(l);


    //    }

    //    //public override Dictionary<EnumGroovePattern, string> Patterns
    //    //{
    //    //    get
    //    //    {

    //    //        var d = new Dictionary<EnumGroovePattern, string> { { EnumGroovePattern.EsternaStandard, "Extern Standard" } };

    //    //        return d;
    //    //    }
    //    //}

    //}


    //[Serializable]
    //public sealed class TornituraScanalaturaFrontale : TornituraScanalatura
    //{
    //    public TornituraScanalaturaFrontale(Guid parent)
    //        : base(parent)
    //    {
    //    }

    //    protected override void CreateSpecificProgram(ProgramPhase programPhase, Operazione operazione)
    //    {
    //        var dIni = DiameterIniziale / 2;
    //        var dFin = DiameterFinale / 2;
    //        var larghUtensile = 3;
    //        var larghPassata = 2;
    //        var step = 2;

    //        var moveCollection = new MoveActionCollection();

    //        for (int i = 0; i < NumeroGole; i++)
    //        {

    //            var stepGole = i * DistanzaGole;

    //            var startZ = StartZ - stepGole;
    //            var endZ = startZ + Larghezza - larghUtensile;

    //            //var effectiveStart = startZ + larghUtensile;

    //            switch (operazione.OperationType)
    //            {
    //                case LavorazioniEnumOperazioni.TornituraScanalaturaSgrossatura:
    //                    {
    //                        TurnProgrammingHelper.GetSgrossaturaGolaEsterna(moveCollection, dIni,
    //                                                                        dFin, startZ, endZ, step,
    //                                                                        larghPassata, .1);
    //                    }
    //                    break;

    //                case LavorazioniEnumOperazioni.TornituraScanalaturaFinitura:
    //                    {

    //                    }
    //                    break;
    //            }
    //        }

    //        foreach (var variable in moveCollection)
    //        {
    //            programPhase.AddAction(variable);
    //        }
    //    }

    //    protected override List<IEntity3D> GetFinalPreview()
    //    {
    //        var rslt = new List<IEntity2D>();

    //        for (int i = 0; i < NumeroGole; i++)
    //        {
    //            var step = i * DistanzaGole;
    //            var startZ = StartZ - step;
    //            var endZ = startZ + Larghezza;

    //            var p = new Line2D { Start = { X = startZ, Y = DiameterIniziale / 2 }, End = { X = startZ, Y = DiameterFinale / 2 } };

    //            var p1 = new Line2D { Start = p.End, End = { X = endZ, Y = p.End.Y } };

    //            var p2 = new Line2D { Start = p1.End, End = { X = p1.End.X, Y = DiameterIniziale / 2 } };

    //            rslt.Add(p);
    //            rslt.Add(p1);
    //            rslt.Add(p2);
    //        }

    //        var l = Entity3DHelper.Get3DProfile(rslt);

    //        Entity3DHelper.SetPlotStyle(l);

    //        return new List<IEntity3D>(l);


    //    }

    //    //public override Dictionary<EnumGroovePattern, string> Patterns
    //    //{
    //    //    get
    //    //    {

    //    //        var d = new Dictionary<EnumGroovePattern, string> { { EnumGroovePattern.EsternaStandard, "Extern Standard" } };

    //    //        return d;
    //    //    }
    //    //}

    //}

}
