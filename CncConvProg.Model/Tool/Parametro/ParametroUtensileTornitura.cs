using System;
using CncConvProg.Geometry.PreviewPathEntity;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.PreviewPathEntity;

namespace CncConvProg.Model.Tool.Parametro
{
    [Serializable]
    public class ParametroUtensileTornitura : ParametroUtensile
    {
        internal ParametroUtensileTornitura(Utensile tool)
            : base(tool)
        {


        }
        internal override void SetFeed(ProgramOperation program, double rapidFeed, double secureRapidFeed, FeedType feedType)
        {
            var velocitaType = ModalitaVelocita == ModalitaVelocita.GiriFissi ? VelocitaType.ASync : VelocitaType.Sync;

            var feedT = feedType == FeedType.Sync ? VelocitaType.Sync : VelocitaType.ASync;

            var feed = AvanzamentoSincrono;

            var speed = Velocita;

            program.AddFeedType(MoveType.Rapid, VelocitaType.ASync, speed, VelocitaType.ASync, rapidFeed);

            program.AddFeedType(MoveType.SecureRapidFeed, VelocitaType.ASync, speed, feedT, secureRapidFeed);

            // queste sono le uniche tipologie di movimento possibili in operazione di tornio
            // poi c'è sosta programmata ..
            program.AddFeedType(new[] { MoveType.Work, MoveType.Cw, MoveType.Ccw }, velocitaType, speed, feedT, feed);
        }

        public override double GetSpeed()
        {
            return Velocita;
        }



        /// <summary>
        /// Todo, supportare inch. e velocità giri fissi. 
        /// </summary>
        /// <returns></returns>
        public override double CalcolateUnitToolCost()
        {
            var rslt = 0d;

            if (TempoVitaUtensile > 0 && CostoUtensile > 0)
                rslt = CostoUtensile / TempoVitaUtensile;

            else if (MetriVitaUtensile > 0 && CostoUtensile > 0 && Velocita > 0)
            {
                var tempoVita = (MetriVitaUtensile * 1000) / Velocita;

                rslt = CostoUtensile / tempoVita;
            }

            return rslt;
        }

        public SpindleRotation SpindleRotation { get; set; }

        public double Velocita { get; set; }
        public double RaggioInserto { get; set; }

        public ModalitaVelocita ModalitaVelocita { get; set; }
        public double ProfonditaPassata { get; set; }
        public double AvanzamentoSincrono { get; set; }

    }
}