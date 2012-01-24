using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CncConvProg.Model.ConversationalStructure.Abstraction;

namespace CncConvProg.Model.FileManageUtility
{
    /*
    * per facilitare ingresso operazioni mi serve tasto save default parameter..
    * cosi riesco a memorizzare i parametri di defaulkt e operazioi comuni..
    * in modo che utenta non reimetta sempre stessi valori
    * per fare questo faro non immettero nessun valore iniziale , ma bensi 
    * faro una copia dell'operazione in una lista.
    * 
    * una volta che apriro stessa operazione cerchero di caricarla
    * e la settero come iniziale.
    * 
    */
    /// <summary>
    /// Questa classe si occupa di memorizzare per intero operazioni , in modo da ricarcarle quando necessario.
    /// Questa classe la serializzo dentro la cartella dove risiede il bin, in modo da riuscire a fare il deploy..
    /// Dici che avro problemi di permessi, proviamo, mettiamo tutto fra try e catch..
    /// </summary>
    [Serializable]
    public class DefaultDataManager
    {
        private readonly List<Lavorazione> _lavorazioniMillimetri = new List<Lavorazione>();
        private readonly List<Lavorazione> _lavorazioniInch = new List<Lavorazione>();

        public void AddLavorazione(MeasureUnit measureUnit, Lavorazione lavorazioneToSave)
        {
            // Cerca lavorazione nella lista in mm
            if (measureUnit == MeasureUnit.Millimeter)
            {
                for (int i = 0; i < _lavorazioniMillimetri.Count; i++)
                {
                    var lav = _lavorazioniMillimetri[i];

                    if (lav.GetType() == lavorazioneToSave.GetType())
                    {
                        _lavorazioniMillimetri[i] = lavorazioneToSave;
                        return;
                    }
                }

                // se arriva qui non è stato trovato e sostituito niente
                _lavorazioniMillimetri.Add(lavorazioneToSave);
            }

            // Cerca lavorazione nella lista in inch
            else
            {
                for (int i = 0; i < _lavorazioniInch.Count; i++)
                {
                    var lav = _lavorazioniInch[i];

                    if (lav.GetType() == lavorazioneToSave.GetType())
                    {
                        _lavorazioniInch[i] = lavorazioneToSave;
                        return;
                    }
                }

                // se arriva qui non è stato trovato e sostituito niente
                _lavorazioniInch.Add(lavorazioneToSave);
            }
        }


        public Lavorazione GetLavorazione(MeasureUnit measureUnit, Guid faseDiLavoroGuid, Type lavorazioneType)
        {
            Lavorazione lavorazioneRslt = null;

            // Cerca lavorazione nella lista in mm
            if (measureUnit == MeasureUnit.Millimeter)
            {
                foreach (var lavorazione in _lavorazioniMillimetri)
                {
                    if (lavorazione.GetType() == lavorazioneType)
                    {
                        lavorazioneRslt = lavorazione;
                    }
                }

            }

            // Cerca lavorazione nella lista in inch
            else
            {
                foreach (var lavorazione in _lavorazioniInch)
                {
                    if (lavorazione.GetType() == lavorazioneType)
                    {
                        lavorazioneRslt = lavorazione;
                    }
                }

            }

            if (lavorazioneRslt != null)
            {
                lavorazioneRslt.RegeneretaGuid();
                lavorazioneRslt.ResetFaseDiLavoro(faseDiLavoroGuid);
            }

            return lavorazioneRslt;
        }

        /*
         * Metto i metodi per interagire con file direttamente qui
         */

        private const string FileName = "WorksPreference.wpr";

        public void Save()
        {
            try
            {
                FileUtility.SerializeToFile(FileName, this);
            }
            catch (Exception)
            {
                Debug.Fail("DefaultaDataManager.Save");
            }
        }

        /// <summary>
        /// Si usa c
        /// </summary>
        public static DefaultDataManager Load()
        {
            try
            {
                if (File.Exists(FileName))
                    return FileUtility.Deserialize<DefaultDataManager>(FileName);

                var dataManager = new DefaultDataManager();

                dataManager.Save();

                return dataManager;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}


