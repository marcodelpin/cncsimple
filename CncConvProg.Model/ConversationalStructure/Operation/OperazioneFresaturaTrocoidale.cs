using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.ConversationalStructure.Operation
{
    [Serializable]
    public class OperazioneFresaturaTrocoidale : Operazione
    {
        private ScanalaturaLinea _scanalaturaLinea;

        public OperazioneFresaturaTrocoidale(ScanalaturaLinea parent, int enumOperationType)
            : base(parent, enumOperationType)
        {
            ProfonditaPassataPerc = 150;
            StepPerc = 7;

            _scanalaturaLinea = parent;
        }

        /// <summary>
        /// Il diametro fresa , 
        /// </summary>
        public double DiametroFresa
        {
            get
            {
                var mill = Utensile as FresaCandela;

                if (mill == null)
                    throw new NullReferenceException();

                return mill.DiametroFresa;
            }

            //set
            //{
            //    var mill = Utensile as FresaCandela;

            //    if (mill == null)
            //        throw new NullReferenceException();

            //    mill.DiametroFresa = value;
            //}
        }

        /// <summary>
        /// Profondita Passata Percentuale
        /// </summary>
        public double ProfonditaPassataPerc { get; set; }

        public double NumeroGiri
        {
            get
            {
                var parFresa = GetParametro<ParametroFresaCandela>();

                if (parFresa == null)
                    throw new NullReferenceException();

                if (!parFresa.NumeroGiri.Value.HasValue)
                    parFresa.NumeroGiri.Value = 0;

                return parFresa.NumeroGiri.Value.Value;
            }

            set
            {
                var parFresa = GetParametro<ParametroFresaCandela>();

                if (parFresa == null)
                    throw new NullReferenceException();

                parFresa.NumeroGiri.Value = value;
            }
        }

        public double Feed
        {
            get
            {
                var parFresa = GetParametro<ParametroFresaCandela>();

                if (parFresa == null)
                    throw new NullReferenceException();

                if (!parFresa.AvanzamentoAsincrono.Value.HasValue)
                    parFresa.AvanzamentoAsincrono.Value = 0;

                return parFresa.AvanzamentoAsincrono.Value.Value;
            }

            set
            {
                var parFresa = GetParametro<ParametroFresaCandela>();

                if (parFresa == null)
                    throw new NullReferenceException();

                parFresa.AvanzamentoAsincrono.Value = value;
            }
        }

        public double GrooveWidth
        {
            get
            {
                return _scanalaturaLinea.LarghezzaCava;
            }
        }

        public double AvanzamentoCentroUtensile
        {
            get
            {

                var dvf = GrooveWidth - DiametroFresa;

                return (dvf / GrooveWidth) * Feed;
            }
        }


        /// <summary>
        /// Passo, incremento della spirale.
        /// </summary>
        public double StepPerc { get; set; }
    }
}
