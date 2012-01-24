using System;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.PathGenerator;

namespace CncConvProg.Model.Tool.Parametro
{
    [Serializable]
    public abstract class ParametroUtensile
    {
        public Utensile Utensile { get; private set; }

        public abstract double CalcolateUnitToolCost();

        protected ParametroUtensile(Utensile tool)
        {
            Utensile = tool;

        }

        public Guid MaterialGuid { get; set; }

        /// <summary>
        /// Costo usura utensile in moneta/minuto
        /// </summary>
        //public double CostoUtensilePerMinuto { get; set; }

        internal void SetMateriale(Guid materialeGuid)
        {
            MaterialGuid = materialeGuid;
        }

        public abstract double GetSpeed();

        internal double GetFeed(FeedType feedType)
        {
            if (Utensile.ParametroUtensile is IFeedAble)
            {
                var e = Utensile.ParametroUtensile as IFeedAble;

                if (feedType == FeedType.ASync)
                    return e.FeedASync;

                if (feedType == FeedType.Sync)
                    return e.FeedSync;
            }

            // se parametro non implementa questa interfaccia restituisco 0
            return 0;
        }

        internal double GetPlungeFeed(FeedType feedType)
        {
            if (Utensile.ParametroUtensile is IPlungeFeedAble)
            {
                var e = Utensile.ParametroUtensile as IPlungeFeedAble;

                if (feedType == FeedType.ASync)
                    return e.PlungeFeedAsync;

                if (feedType == FeedType.Sync)
                    return e.PlungeFeedSync;
            }

            // se parametro non implementa questa interfaccia restituisco 0
            return 0;
        }

        internal abstract void SetFeed(ProgramPhase program, double rapidFeed, double secureRapidFeed, FeedType feedType);

        public double TempoVitaUtensile { get; set; }

        public double MetriVitaUtensile { get; set; }

        public double CostoUtensile { get; set; }

        public double UsuraEuroMin { get; set; }

        internal void SetUtensile(Tool.Utensile utensile)
        {
            Utensile = utensile;
        }
    }
}