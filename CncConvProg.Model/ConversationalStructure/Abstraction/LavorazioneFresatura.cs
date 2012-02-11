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
    public abstract class LavorazioneFresatura : Lavorazione
    {
        public override FaseDiLavoro.TipoFaseLavoro[] FasiCompatibili
        {
            get
            {
                return new[]
                           {
                               FaseDiLavoro.TipoFaseLavoro.Centro,
                               FaseDiLavoro.TipoFaseLavoro.Tornio3Assi
                           };
            }
        }
        internal override ProgramOperation  GetOperationProgram(Operazione operazione)
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

            // todo : tenere conto anche dell'asse c attivazione g112 per il tornio ..
            // Creo nuovo programma, tutte le lavorazioni (  hanno Z Sicurezza )
            // anche perchè pensavo di settarla a livello di fase di lavoro..
            var program = new ProgramOperation(SicurezzaZ);

            // Se cambio utensile è opzionale , oppure se è forzato non cambio utensile..
            //var cambioUtensile = (operazione.ToolChangeOptional && operazione.ForceToolChange) || !operazione.ToolChangeOptional;

            var changeToolAction = new CambiaUtensileAction(program, operazione);

            changeToolAction.CutViewerToolInfo = CutViewerHelper.PrintTool(operazione.Utensile);


            // Ora setto avanzamenti comuni - Rapido - SecureFeed - Ecc..
            // per ora setto cazzo .. dove lo posso prendere ??

            var preference = Singleton.Preference.GetPreference(MeasureUnit);

            ExtraCorsa = preference.MillEntryExitSecureDistance;

            SecureFeed = preference.MillingRapidSecureFeedAsync;

            /*
             * L'idea per i feed e di cercare di inserirli tutti nel dizionario, se poi vengono usati ok, 
             * altrimenti è lo stesso..
             */
            // Magari è piu corretto settarle nella classe base parametro..

            var feed = operazione.Utensile.ParametroUtensile.GetFeed(FeedType.ASync);
            var plungeFeed = operazione.Utensile.ParametroUtensile.GetPlungeFeed(FeedType.ASync);

            /*
             * Non controllo più che i feed siano <= 0 , al limite nel prog ci sarà scritto F0
             */
            var rapidFeed = FaseDiLavoro.GetRapidFeed();

            operazione.Utensile.ParametroUtensile.SetFeed(program, rapidFeed, SecureFeed, FeedType.ASync);

            //var feedDictionary = new Dictionary<MoveType, double>
            //                         {
            //                             {MoveType.Rapid, 10000},
            //                             {MoveType.SecureRapidFeed, SecureFeed},
            //                             {MoveType.Work, feed},
            //                             {MoveType.Cw, feed},
            //                             {MoveType.Ccw, feed},
            //                             {MoveType.PlungeFeed, plungeFeed},
            //                         };

            //program.SetFeedDictionary(feedDictionary);

            /*
             * ora c'è richiamo il metodo specifico della lavorazione.
             * In ingresso prende il ProgramPhase. e Operazione.
             */
            CreateSpecificProgram(program, operazione);

            /*
             * todo.
             * qui andrebbe il metodo per fare le rototraslazioni. 
             * per ora lo lascio delegato alla lavorazione specifica 
             * per via del problemi con ciclo di foratura..
             */

            return program;
        }
    }
}