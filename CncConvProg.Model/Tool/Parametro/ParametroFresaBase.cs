using System;
using System.Collections.Generic;
using CncConvProg.Geometry.PreviewPathEntity;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.PreviewPathEntity;
using CncConvProg.Model.Tool.Mill;

namespace CncConvProg.Model.Tool.Parametro
{
    [Serializable]
    public abstract class ParametroFresaBase : Parametro.ParametroUtensile, IFeedAble, IPlungeFeedAble
    {
        private FresaBase Mill
        {
            get { return Utensile as FresaBase; }
        }

        public double DiametroFresa { get { return Mill.Diametro; } }

        internal ParametroFresaBase(FresaBase tool)
            : base(tool)
        {
            VelocitaTaglio = new UserInput();
            NumeroGiri = new UserInput();
            LarghezzaPassata = new UserInput();
            LarghezzaPassataPerc = new UserInput();
            ProfonditaPassata = new UserInput();
            ProfonditaPassataPerc = new UserInput();
            AvanzamentoAsincrono = new UserInput();
            AvanzamentoSincrono = new UserInput();
            AvanzamentoAsincronoPiantata = new UserInput();
            AvanzamentoSincronoPiantata = new UserInput();
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

        public void SetProfPassataPerc(double value)
        {

            ProfonditaPassataPerc.SetValue(true, value);
            ProfonditaPassata.SetValue(false, (ProfonditaPassataPerc.Value / 100) * DiametroFresa);

        }

        public void SetLarghPassataPerc(double value)
        {
            LarghezzaPassataPerc.SetValue(true, value);
            LarghezzaPassata.SetValue(false, (LarghezzaPassataPerc.Value / 100) * DiametroFresa);
        }

        public void SetProfPassata(double value)
        {
            ProfonditaPassata.SetValue(true, value);
            ProfonditaPassataPerc.SetValue(false, (ProfonditaPassata.Value / DiametroFresa) * 100);
        }

        public void SetLarghPassata(double value)
        {
            LarghezzaPassata.SetValue(true, value);
            LarghezzaPassataPerc.SetValue(false, (LarghezzaPassata.Value / DiametroFresa) * 100);
        }

        public UserInput VelocitaTaglio { get; set; }
        public UserInput NumeroGiri { get; set; }
        public UserInput LarghezzaPassata { get; set; }
        public UserInput LarghezzaPassataPerc { get; set; }
        public UserInput ProfonditaPassata { get; set; }
        public UserInput ProfonditaPassataPerc { get; set; }
        public UserInput AvanzamentoAsincrono { get; set; }
        public UserInput AvanzamentoSincrono { get; set; }
        public UserInput AvanzamentoAsincronoPiantata { get; set; }
        public UserInput AvanzamentoSincronoPiantata { get; set; }

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
            var nGiri = GetNumeroGiri();
            var feed = GetFeed(FeedType.ASync);
            var plungefeed = GetPlungeFeed(FeedType.ASync);

            program.AddFeedType(MoveType.Rapid, VelocitaType.ASync, nGiri, VelocitaType.ASync, rapidFeed);

            program.AddFeedType(MoveType.SecureRapidFeed, VelocitaType.ASync, nGiri, VelocitaType.ASync, secureRapidFeed);

            program.AddFeedType(new[] { MoveType.Work, MoveType.Cw, MoveType.Ccw }, VelocitaType.ASync, nGiri, VelocitaType.ASync, feed);

            program.AddFeedType(MoveType.PlungeFeed, VelocitaType.ASync, nGiri, VelocitaType.ASync, plungefeed);
        }

        internal double GetLarghezzaPassata()
        {
            return LarghezzaPassata.Value.HasValue ? LarghezzaPassata.Value.Value : 0;
        }


        internal double GetProfonditaPassata()
        {
            return ProfonditaPassata.Value.HasValue ? ProfonditaPassata.Value.Value : 0;

        }

        internal double GetNumeroGiri()
        {
            return NumeroGiri.Value.HasValue ? NumeroGiri.Value.Value : 0;

        }

        public void SetNumeroGiri(double nGiri)
        {
            NumeroGiri.SetValue(true, nGiri);

            VelocitaTaglio.SetValue(false, FeedAndSpeedHelper.GetVelocitaTaglio(nGiri, DiametroFresa, Mill.Unit));
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

            NumeroGiri.SetValue(false, FeedAndSpeedHelper.GetNumeroGiri(velocitaTaglio, DiametroFresa, Mill.Unit));
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


        public void SetPlungeFeedSync(double feedSync)
        {
            AvanzamentoSincronoPiantata.SetValue(true, feedSync);

            if (NumeroGiri.Value.HasValue)
            {
                var nGiri = NumeroGiri.Value.Value;
                AvanzamentoAsincronoPiantata.SetValue(false, FeedAndSpeedHelper.GetFeedAsync(feedSync, nGiri));
            }
        }

        public void SetPlungeFeedAsync(double feedAsync)
        {
            AvanzamentoAsincronoPiantata.SetValue(true, feedAsync);

            if (NumeroGiri.Value.HasValue)
            {
                var nGiri = NumeroGiri.Value.Value;
                AvanzamentoSincronoPiantata.SetValue(false, FeedAndSpeedHelper.GetFeedSync(feedAsync, nGiri));
            }
        }

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

        public double PlungeFeedSync
        {
            get
            {
                if (AvanzamentoSincronoPiantata.Value.HasValue)
                    return AvanzamentoSincronoPiantata.Value.Value;
                return 0;
            }
        }

        public double PlungeFeedAsync
        {
            get
            {
                if (AvanzamentoAsincronoPiantata.Value.HasValue)
                    return AvanzamentoAsincronoPiantata.Value.Value;
                return 0;
            }
        }
    }
}