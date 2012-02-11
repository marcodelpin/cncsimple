using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool.Parametro;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura
{
    [Serializable]
    public sealed class Tornitura : LavorazioneTornitura
    {
        /*
         * creare sia verticale che orizzontale..
         * con stessa classe.
         * 
         */
        public RawProfile Profile { get; set; }

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

            var parametro = operazione.Utensile.ParametroUtensile as ParametroUtensileTornitura;

            if (parametro == null) return;

            // La profondità di passata = x2 
            var profPassata = parametro.ProfonditaPassata * 2;

            var profile2D = Profile.GetProfileResult(false);

            switch (operazione.OperationType)
            {
                case LavorazioniEnumOperazioni.TornituraSgrossatura:
                    {
                        TurnProgrammingHelper.GetRoughingTurnProgram(moveCollection, profile2D, profPassata, ExtraCorsa, ExtraCorsa, _tipoTornitura);
                    }
                    break;

                case LavorazioniEnumOperazioni.TornituraFinitura:
                    {
                        TurnProgrammingHelper.GetFinishingProgram(moveCollection, profile2D, _tipoTornitura, ExtraCorsa);
                    }
                    break;

                default:
                    Trace.WriteLine("Tornitura.CreateSpecificProgram");
                    break;
            }

            foreach (var variable in moveCollection)
            {
                programPhase.AggiungiAzioneMovimento(variable);
            }
        }

        public override void SetProfile(List<RawEntity2D> list)
        {
            Profile.Syncronize(list);

            base.SetProfile(list);
        }

        protected override List<IEntity3D> GetFinalPreview()
        {
            try
            {
                return Entity3DHelper.Get3DProfile(Profile.GetProfileResult(false).Source).ToList();
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public enum TipoTornitura
        {
            Esterna,
            Interna
        }

        private void AddDummyProfile(TipoTornitura tipoTornitura)
        {
            switch (tipoTornitura)
            {
                case TipoTornitura.Esterna:
                    {
                        var profile = new RawProfile(true);

                        var ini = new RawInitPoint2D(profile);
                        ini.X.SetValue(true, 0);
                        ini.Y.SetValue(true, 25);

                        var line1 = new RawLine2D(profile);
                        line1.DeltaX.SetValue(true, -10);

                        var line2 = new RawLine2D(profile);
                        line2.DeltaY.SetValue(true, 10);
                        line2.DeltaX.SetValue(true, -10);

                        var line3 = new RawLine2D(profile);
                        line3.DeltaY.SetValue(true, 10);

                        profile.Add(ini);
                        profile.Add(line1);
                        profile.Add(line2);
                        profile.Add(line3);

                        Profile = profile;
                    } break;

                case TipoTornitura.Interna:
                    {
                        var profile = new RawProfile(true);

                        var ini = new RawInitPoint2D(profile);
                        ini.X.SetValue(true, 0);
                        ini.Y.SetValue(true, 70);

                        var line1 = new RawLine2D(profile);
                        line1.DeltaX.SetValue(true, -10);

                        var line2 = new RawLine2D(profile);
                        line2.DeltaY.SetValue(true, -10);
                        line2.DeltaX.SetValue(true, -10);

                        var line3 = new RawLine2D(profile);
                        line3.DeltaY.SetValue(true, -10);

                        profile.Add(ini);
                        profile.Add(line1);
                        profile.Add(line2);
                        profile.Add(line3);

                        Profile = profile;
                    } break;
            }

        }

        private readonly TipoTornitura _tipoTornitura;
        public Tornitura(TipoTornitura tipoTornitura)
            : base()
        {
            _tipoTornitura = tipoTornitura;

            Profile = new RawProfile(true);
            /*
             * per evitare danni inserisco punto iniziale qui,
             * anche se e meglio che venga creato dentro ctorù             */

            var ini = new RawInitPoint2D(Profile);
            Profile.Add(ini);
            ini.X.SetValue(true, 0);


           // AddDummyProfile(_tipoTornitura);

            Sgrossatura = new Operazione(this, LavorazioniEnumOperazioni.TornituraSgrossatura);

            Finitura = new Operazione(this, LavorazioniEnumOperazioni.TornituraFinitura);
        }

        public override string Descrizione
        {
            get
            {
                switch (_tipoTornitura)
                {
                    case TipoTornitura.Esterna:
                        return MecPrev.Resources.GuiRes.Tornitura + " " + MecPrev.Resources.GuiRes.Esterna;
                    case TipoTornitura.Interna:
                        return MecPrev.Resources.GuiRes.Tornitura + " " + MecPrev.Resources.GuiRes.Intern;
                }

                return string.Empty;
            }


        }

    }

}
