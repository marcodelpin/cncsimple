using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.PreviewPathEntity;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.Model.ToolMachine;

namespace CncConvProg.Model.ConversationalStructure.Abstraction
{
    /// <summary>
    /// Magari quando faro lavorazioni per tornio 
    /// faro 2 classi derivate 
    /// - lavorazioni tornio
    /// - lavorazioni centro
    /// </summary>
    [Serializable]
    public abstract class LavorazioneTornitura : Lavorazione
    {
        public override FaseDiLavoro.TipoFaseLavoro[] FasiCompatibili
        {
            get
            {
                return new[]
                           {
                               FaseDiLavoro.TipoFaseLavoro.Tornio3Assi,
                               FaseDiLavoro.TipoFaseLavoro.Tornio2Assi
                           };
            }
        }

        internal override ProgramOperation GetOperationProgram(Operazione operazione)
        {
            /*
             * - init program
             * - toolChange ( poi in fase di calcolo programma vedo se saltarlo o meno )
             * - settaggio feed. ( vedere meglio)
             * 
             * -- calcolo programma ( questo è l'unica parte diversa )
             * 
             * - rototraslazione operazioni
             */

            var preference = Singleton.Preference.GetPreference(MeasureUnit);

            var program = new ProgramOperation(0);

            ExtraCorsa = preference.TurningSecureDistance;

            //RapidSecureFeed = preference.TurningRapidSecureFeedSync;

            var changeToolAction = new CambiaUtensileAction(program, operazione);

            var rapidFeed = FaseDiLavoro.GetRapidFeed();

            operazione.Utensile.ParametroUtensile.SetFeed(program, rapidFeed, 0, FeedType.Sync);

            CreateSpecificProgram(program, operazione);

            return program;
        }
    }
}