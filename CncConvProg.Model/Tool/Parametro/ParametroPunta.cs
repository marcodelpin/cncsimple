using System;
using CncConvProg.Geometry.PreviewPathEntity;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.PreviewPathEntity;

namespace CncConvProg.Model.Tool.Parametro
{
    [Serializable]
    public class ParametroPunta : Parametro.ParametroUtensile, IFeedAble
    {

        private Drill.DrillTool Drill
        {
            get
            {
                return Utensile as Drill.DrillTool;
            }
        }
        internal ParametroPunta(Drill.DrillTool tool)
            : base(tool)
        {

            VelocitaTaglio = new UserInput();
            NumeroGiri = new UserInput();
            AvanzamentoAsincrono = new UserInput();
            AvanzamentoSincrono = new UserInput();
        }

        public override double CalcolateUnitToolCost()
        {
            var rslt = 0d;

            if (AvanzamentoAsincrono.Value.HasValue)
            {
                var feedMmMin = AvanzamentoAsincrono.Value.Value;

                if (TempoVitaUtensile > 0 && CostoUtensile > 0)
                    rslt = CostoUtensile / TempoVitaUtensile;

                else if (MetriVitaUtensile > 0 && CostoUtensile > 0 && feedMmMin > 0)
                {
                    var tempoVita = (MetriVitaUtensile * 1000) / feedMmMin;

                    rslt = CostoUtensile / tempoVita;
                }
            }

            return rslt;
        }

        private double DiametroPunta { get { return Drill.Diametro; } }

        public UserInput VelocitaTaglio { get; set; }
        public UserInput NumeroGiri { get; set; }
        public UserInput AvanzamentoSincrono { get; set; }
        public UserInput AvanzamentoAsincrono { get; set; }

        public double Step { get; set; }

        public override double GetSpeed()
        {
            if (NumeroGiri != null && NumeroGiri.Value.HasValue)
            {
                return NumeroGiri.Value.Value;
            }

            return 0;
        }

        internal override void SetFeed(ProgramOperation program, double rapidFeed, double secureRapidFeed, FeedType feedType)
        {
            var nGiri = GetSpeed();
            var feed = GetFeed(FeedType.ASync);

            program.AddFeedType(MoveType.Rapid, VelocitaType.ASync, nGiri, VelocitaType.ASync, rapidFeed);

            program.AddFeedType(MoveType.SecureRapidFeed, VelocitaType.ASync, nGiri, VelocitaType.ASync, secureRapidFeed);

            program.AddFeedType(new[] { MoveType.Work, MoveType.Cw, MoveType.Ccw, MoveType.PlungeFeed }, VelocitaType.ASync, nGiri, VelocitaType.ASync, feed);
        }

        public void SetNumeroGiri(double nGiri)
        {
            NumeroGiri.SetValue(true, nGiri);

            VelocitaTaglio.SetValue(false, FeedAndSpeedHelper.GetVelocitaTaglio(nGiri, DiametroPunta, Drill.Unit));
        }

        public void SetFeedSync(double feedSync)
        {
            AvanzamentoSincrono.SetValue(true, feedSync);

            if (NumeroGiri.Value.HasValue)
            {
                var nGiri = NumeroGiri.Value.Value;
                AvanzamentoAsincrono.SetValue(false, FeedAndSpeedHelper.GetFeedAsync(feedSync, nGiri));
            }
        }

        public void SetFeedAsync(double feedAsync)
        {
            AvanzamentoAsincrono.SetValue(true, feedAsync);

            if (NumeroGiri.Value.HasValue)
            {
                var nGiri = NumeroGiri.Value.Value;
                AvanzamentoSincrono.SetValue(false, FeedAndSpeedHelper.GetFeedSync(feedAsync, nGiri));
            }
        }

        public void SetVelocitaTaglio(double velocitaTaglio)
        {
            VelocitaTaglio.SetValue(true, velocitaTaglio);

            NumeroGiri.SetValue(false, FeedAndSpeedHelper.GetNumeroGiri(velocitaTaglio, DiametroPunta, Drill.Unit));
        }

        //internal double GetFeed(ConversationalStructure.FeedType feedType)
        //{
        //    switch (feedType)
        //    {
        //        case ConversationalStructure.FeedType.Sync:
        //            {
        //                if (AvanzamentoSincrono.Value.HasValue)
        //                    return AvanzamentoSincrono.Value.Value;

        //            } break;

        //        case ConversationalStructure.FeedType.ASync:
        //        default:
        //            {
        //                if (AvanzamentoAsincrono.Value.HasValue)
        //                    return AvanzamentoAsincrono.Value.Value;

        //            } break;
        //    }

        //    return 0;
        //}


        public double FeedSync
        {
            get
            {
                if (AvanzamentoSincrono.Value.HasValue)
                    return AvanzamentoSincrono.Value.Value;
                return 0;
            }
        }

        public double FeedASync
        {
            get
            {
                if (AvanzamentoAsincrono.Value.HasValue)
                    return AvanzamentoAsincrono.Value.Value;
                return 0;
            }
        }
    }
}