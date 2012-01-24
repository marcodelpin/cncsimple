using System;
using System.Collections.Generic;
using System.IO;
using CncConvProg.Model.ThreadTable;
using CncConvProg.Model.Tool;
using CncConvProg.Model.ToolMachine;

namespace CncConvProg.Model.FileManageUtility
{
    public static class PathFolderHelper
    {
        private const string ProgramDirectory = "\\EasyCnc\\";
        private const string UserDataDirectory = "\\Data\\";

        /// <summary>
        /// Per ora metto i percorsi dei file fissi,
        /// poi pensare modo per personalizzarli
        /// </summary>
        private const string TabellaFilettaturaFile = "ThreadTable.ttbS";
        private const string ToolStoreFile = "ToolStore.tstS";
        private const string ToolMachinesFile = "ToolMachines.tmcS";
        private const string PreferenceFile = "Preference.prcS";


        /// <summary>
        /// Ritorna directory base, se non esiste la crea. 
        /// </summary>
        /// <returns></returns>
        private static string GetProgramDirectoryPath()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + ProgramDirectory;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        private static string GetUserDataDirectory()
        {
            var folderPath = GetProgramDirectoryPath() + UserDataDirectory;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        private static string GetThreadTableFilePath()
        {
            return GetUserDataDirectory() + TabellaFilettaturaFile;
        }



        private static string GetToolMachinesFilePath()
        {
            return GetUserDataDirectory() + ToolMachinesFile;
        }

        public static TabellaFilettature GetTabellaFilettatura()
        {
            try
            {
                var path = GetThreadTableFilePath();

                if (File.Exists(path))
                    return GetSerializedFile<TabellaFilettature>(path);
                var ntf = new TabellaFilettature();

                SaveTabellaFilettatura(ntf);

                return ntf;

            }
            catch (Exception)
            {
                var ntf = new TabellaFilettature();

                SaveTabellaFilettatura(ntf);

                return ntf;
            }
        }

        public static void SaveTabellaFilettatura(TabellaFilettature tabellaFilettature)
        {
            var path = GetThreadTableFilePath();

            FileUtility.SerializeToFile(path, tabellaFilettature);
        }

        #region Magazzino Utensile

        private static string GetToolStoreFilePath()
        {
            return GetUserDataDirectory() + ToolStoreFile;
        }

        internal static MagazzinoUtensile GetMagazzinoUtensile()
        {
            try
            {
                var path = GetToolStoreFilePath();

                if (File.Exists(path))
                    return GetSerializedFile<MagazzinoUtensile>(path);

                var magazzino = new MagazzinoUtensile();

                SaveMagazzinoUtensile(magazzino);

                return magazzino;
            }
            catch (Exception)
            {
                var magazzino = new MagazzinoUtensile();

                SaveMagazzinoUtensile(magazzino);

                return magazzino;
            }

        }

        public static void SaveMagazzinoUtensile(MagazzinoUtensile tabellaFilettature)
        {
            var path = GetToolStoreFilePath();

            FileUtility.SerializeToFile(path, tabellaFilettature);
        }
        #endregion


        #region Preference File


        private static string GetPreferenceFilePath()
        {
            return GetUserDataDirectory() + PreferenceFile;
        }
        /// <summary>
        /// Se ritorna null, la classe chiamante aprira finestra dialogo per impostare preferenze.
        /// </summary>
        /// <returns></returns>
        public static ProgramPreference GetPreferenceData()
        {
            try
            {
                var path = GetPreferenceFilePath();

                if (File.Exists(path))
                    return GetSerializedFile<ProgramPreference>(path);

                return null;
                //  return new ProgramPreference();
            }
            catch (Exception)
            {
                return new ProgramPreference();
            }


        }

        public static void SavePreferenceFile(ProgramPreference preference)
        {
            var path = GetPreferenceFilePath();

            FileUtility.SerializeToFile(path, preference);
        }

        #endregion

        private static TRet GetSerializedFile<TRet>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            return FileUtility.Deserialize<TRet>(filePath);
        }


        internal static void SaveToolMachines(List<ToolMachine.ToolMachine> macchine)
        {
            var path = GetToolMachinesFilePath();

            FileUtility.SerializeToFile(path, macchine);
        }
        public static List<ToolMachine.ToolMachine> GetToolMachines()
        {
            var path = GetToolMachinesFilePath();

            if (File.Exists(path))
                return GetSerializedFile<List<ToolMachine.ToolMachine>>(path);

            var latheAxisC = new LatheAxisC { MachineName = "Lathe w/ Axis C" };

            var lathe = new HorizontalLathe2Axis { MachineName = "Horizontal Lathe" };

            var mill = new VerticalMill { MachineName = "Vertical Mill" };

            var machines = new List<ToolMachine.ToolMachine> { mill, latheAxisC, lathe };

            SaveToolMachines(machines);

            return machines;
        }


        public static List<RigaTabellaFilettatura> GetFilettatureList()
        {
            var lm = GetTabellaFilettatura();

            var fl = new List<RigaTabellaFilettatura>();

            foreach (var filettatura in lm.Filettature)
            {

                foreach (var rigaTabellaFilettatura in filettatura.RigheTabella)
                {
                    rigaTabellaFilettatura.SetTipologiaParent(filettatura);

                    fl.Add(rigaTabellaFilettatura);
                }

                /*
                 * Aggiorno referenza tipologia filettatura a rigaTabella
                 */
            }

            return fl;
        }



        internal static Data LoadData()
        {
            var d = new Data();

            var macchine = GetToolMachines();
            var toolStore = GetMagazzinoUtensile();
            var tabellaFilettatura = GetTabellaFilettatura();

            d.SetMachines(macchine);
            d.SetMagazzinoUtensile(toolStore);
            d.SetTabellaFilettattura(tabellaFilettatura);

            return d;
        }

    }

}
