using System;
using System.Collections.Generic;
using System.Diagnostics;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura
{
    [Serializable]
    public sealed class TornituraSfacciatura : LavorazioneTornitura
    {
        /*
         * creare sia verticale che orizzontale..
         * con stessa classe.
         * 
         */

        public double DiametroMax { get; set; }
        public double DiametroMin { get; set; }

        public double Sovrametallo { get; set; }

        public double InizioZ { get; set; }

        public override List<Operazione> Operazioni
        {
            get
            {
                return new List<Operazione> { Sgrossatura, Finitura };
            }
        }

        public Operazione Sgrossatura { get; set; }

        public Operazione Finitura { get; set; }


        protected override void CreateSpecificProgram(ProgramOperation programPhase, Operazione operazione)
        {
            var moveCollection = new MoveActionCollection();

            var zStart = InizioZ + Sovrametallo;

            var parametro = operazione.Utensile.ParametroUtensile as ParametroUtensileTornitura;

            if (parametro == null) return;

            var profPassata = parametro.ProfonditaPassata;

            switch (operazione.OperationType)
            {
                case LavorazioniEnumOperazioni.TornituraSfacciaturaSgrossatura:
                    {
                        TurnProgrammingHelper.GetFaceRoughing(moveCollection, DiametroMax, DiametroMin, InizioZ, zStart, profPassata, 1);
                    }
                    break;

                case LavorazioniEnumOperazioni.TornituraSfacciaturaFinitura:
                    {
                        TurnProgrammingHelper.GetFaceFinishing(moveCollection, DiametroMax, DiametroMin, InizioZ, 1);
                    }
                    break;

                default:
                    Trace.WriteLine("Tornitura.CreateSpecificProgram");
                    break;
            }


            foreach (var action in moveCollection)
            {
                programPhase.AggiungiAzioneMovimento(action);
            }
        }

        protected override List<IEntity3D> GetFinalPreview()
        {
            var rslt = new List<IEntity2D>();


            var zMax = InizioZ + Sovrametallo;
            var zMin = InizioZ;

            var l1 = new Line2D { Start = new Point2D(zMax, DiametroMax), End = new Point2D(zMin, DiametroMax) };

            var l2 = new Line2D { Start = l1.End, End = new Point2D(zMin, DiametroMin) };

            var l3 = new Line2D { Start = l2.End, End = new Point2D(zMax, DiametroMin) };

            var l4 = new Line2D { Start = l3.End, End = l1.Start };

            rslt.Add(l1);
            rslt.Add(l2);
            rslt.Add(l3);
            rslt.Add(l4);

            var d = Entity3DHelper.Get3DProfile(rslt);

            Entity3DHelper.SetPlotStyle(d);

            return new List<IEntity3D>(d);
        }


        public TornituraSfacciatura()
            : base()
        {
            Sgrossatura = new Operazione(this, LavorazioniEnumOperazioni.TornituraSfacciaturaSgrossatura);

            Finitura = new Operazione(this, LavorazioniEnumOperazioni.TornituraSfacciaturaFinitura);
        }

        public override string Descrizione
        {
            get { return MecPrev.Resources.GuiRes.FaceTurning; }
        }

    }

}

