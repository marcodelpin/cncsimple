using System;
using System.Collections.Generic;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.PreviewPathEntity;
using CncConvProg.Model.ThreadTable;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura
{
    [Serializable]
    public sealed class TornituraFilettatura : LavorazioneTornitura
    {
        public RigaTabellaFilettatura MaschiaturaSelezionata { get; set; }

        public double Passo { get; set; }

        public int NumeroPassate { get; set; }

        public double Diametro { get; set; }

        public double LunghezzaFiletto { get; set; }

        public double ZIniziale { get; set; }

        public override List<Operazione> Operazioni
        {
            get
            {
                return new List<Operazione> { Filettatura };
            }
        }

        public Operazione Filettatura { get; set; }

        /// <summary>
        /// Come anteprima creo rettangolo.. 
        /// </summary>
        /// <returns></returns>
        protected override List<IEntity3D> GetFinalPreview()
        {
            var diaExt = MaschiaturaSelezionata.DiametroMetrico;
            var passo = MaschiaturaSelezionata.Passo;
            var diaMin = diaExt - passo;
            var zIni = ZIniziale;
            var zEnd = ZIniziale - LunghezzaFiletto;

            var p = new Profile2D();

            var l1 = new Line2D
            {
                Start = new Point2D
                {
                    X = zIni,
                    Y = diaExt,
                },

                End = new Point2D
                {
                    X = zEnd,
                    Y = diaExt,
                }
            };

            var l2 = new Line2D
            {
                Start = l1.End,

                End = new Point2D
                {
                    X = zEnd,
                    Y = diaMin,
                }
            };

            var l3 = new Line2D
            {
                Start = l2.End,

                End = new Point2D
                {
                    X = zIni,
                    Y = diaMin,
                }
            };

            var l4 = new Line2D
            {
                Start = l3.End,
                End = l1.Start,
            };

            p.AddEntity(l1);
            p.AddEntity(l2);
            p.AddEntity(l3);
            p.AddEntity(l4);

            p.SetPlotStyle();

            var l = Entity3DHelper.Get3DProfile(p.Source);

            return new List<IEntity3D>(l);
        }

        public TornituraFilettatura()
            : base()
        {
            Filettatura = new Operazione(this, LavorazioniEnumOperazioni.TornituraFilettatura);
        }

        public override string Descrizione
        {
            get { return MecPrev.Resources.GuiRes.TurnThread; }
        }

        protected override void CreateSpecificProgram(ProgramOperation programPhase, Operazione operazione)
        {
            var diaExt = MaschiaturaSelezionata.DiametroMetrico;
            var passo = MaschiaturaSelezionata.Passo;
            var diaMin = diaExt - passo;
            var zIni = ZIniziale + passo*3;
            var zEnd = ZIniziale - LunghezzaFiletto;

            var delta = diaExt - diaMin;

            if (NumeroPassate <= 0)
                NumeroPassate = 1;

            var step = delta / NumeroPassate;

            var currentX = diaExt;

            var moveCollection = new MoveActionCollection();

            // Qui dovrebbe già essere impostato i parametri di velocità, ora pero faccio override avanzamento per lavoro
            ParametroVelocita p;

            if (programPhase.FeedDictionary.TryGetValue(MoveType.Work, out p))
            {
                p.ModoAvanzamento = VelocitaType.Sync;
                p.ValoreFeed = passo;
            }



            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, zIni, diaExt + ExtraCorsa, 0);

            var c = 0;
            while (currentX > diaMin)
            {
                c++;

                currentX -= step;
                if (currentX <= diaMin)
                    currentX = diaMin;

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, null, currentX, 0);

                moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, zEnd, null, 0);

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, null, diaExt, 0);

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, zIni, null , 0);
            }

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, zIni, diaExt + ExtraCorsa, 0);



            foreach (var variable in moveCollection)
            {
                programPhase.AggiungiAzioneMovimento(variable);
            }
        }

    }

}
