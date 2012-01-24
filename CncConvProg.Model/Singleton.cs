using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.Model.ThreadTable;
using CncConvProg.Model.Tool;

namespace CncConvProg.Model
{
    /// <summary>
    ///  Carica machine , magazzino utensile e preference.
    /// Deve essere accessibile solo dal singleton, per comodità la faccio public
    /// </summary>
    public class Data
    {
        //
        //
        private List<ToolMachine.ToolMachine> _macchine;
        private ThreadTable.TabellaFilettature _tabellaFilettature;
        private Tool.MagazzinoUtensile _magazzinoUtensile;

        public void SetMachines(List<ToolMachine.ToolMachine> macchine)
        {
            _macchine = macchine;
        }
        public List<RigaTabellaFilettatura> GetFilettatureList()
        {
            var lm = _tabellaFilettature;

            var fl = new List<RigaTabellaFilettatura>();

            foreach (var filettatura in lm.Filettature)
            {

                foreach (var rigaTabellaFilettatura in filettatura.RigheTabella)
                {
                    rigaTabellaFilettatura.SetTipologiaParent(filettatura);

                    fl.Add(rigaTabellaFilettatura);
                }

            }

            return fl;
        }


        public void SetTabellaFilettattura(ThreadTable.TabellaFilettature tabellaFilettatura)
        {
            _tabellaFilettature = tabellaFilettatura;
        }

        public void SetMagazzinoUtensile(MagazzinoUtensile toolStore)
        {
            _magazzinoUtensile = toolStore;
        }

        public List<ToolMachine.ToolMachine> GetMachines()
        {
            return _macchine;
        }

        public ThreadTable.TabellaFilettature GetTabellaFilettattura()
        {
            return _tabellaFilettature;
        }

        public MagazzinoUtensile GetMagazzinoUtensile()
        {
            return _magazzinoUtensile;
        }
        //public IEnumerable<Utensile> GetMagazzinoUtensile(MeasureUnit measureUnit)
        //{
        //    return _magazzinoUtensile.GetTools(measureUnit);
        //}
        public IEnumerable<Materiale> GetMateriali(MeasureUnit measureUnit)
        {
            return _magazzinoUtensile.GetMaterials(measureUnit);
        }

        internal Materiale GetMaterialeFromGuid(Guid materialeGuid)
        {
            var mats = _magazzinoUtensile.GetMaterials();

            var l = mats.Where(m => m.MaterialeGuid == materialeGuid).FirstOrDefault();

            return l;
        }

        public void DeletePrezzoMateriale(PrezzoMateriale prezzoMaterialeSelezionato)
        {
            _magazzinoUtensile.DeletePrezzo(prezzoMaterialeSelezionato);
        }

        public void AddPrezzoMateriale(PrezzoMateriale prezzo)
        {
            _magazzinoUtensile.AddPrezzoMateriale(prezzo);
        }

        public void DeleteMaterial(Materiale materialeSelezionato)
        {
            _magazzinoUtensile.DeleteMaterial(materialeSelezionato);
        }

        public void AddMateriale(Materiale materiale)
        {
            _magazzinoUtensile.AddMateriale(materiale);
        }

        public IEnumerable<PrezzoMateriale> GetPrezziMateriale(Guid materialeGuid, MeasureUnit measureUnit)
        {
            return _magazzinoUtensile.GetPrezziMateriale(materialeGuid, measureUnit);
        }

        public IEnumerable<PrezzoMateriale> GetPrezziMateriale(MeasureUnit measureUnit)
        {
            return _magazzinoUtensile.GetPrezziMateriale(measureUnit);
        }


        public void SaveMagazzino()
        {
            PathFolderHelper.SaveMagazzinoUtensile(_magazzinoUtensile);
        }

        public void SaveMagazzino(MagazzinoUtensile magazzinoUtensile)
        {
            _magazzinoUtensile = magazzinoUtensile;

            SaveMagazzino();
        }

        internal PrezzoMateriale GetPrezzoMateriale(Guid prezzoGuid)
        {
            return _magazzinoUtensile.GetPrezzoMateriale(prezzoGuid);
        }

        public Utensile GetTool(Guid utensileGuid)
        {
            var ts = _magazzinoUtensile.GetTool(utensileGuid);

            return ts;
        }

        public void AddOrUpdateTool(Utensile tool)
        {
            _magazzinoUtensile.AddOrUpdateTool(tool);

        }

        public IEnumerable<Utensile> GetCompatibleTools(Utensile t)
        {
            var mu = t.Unit;
            var type = t.GetType();

            var ts = _magazzinoUtensile.GetTools(mu);

            var tcs = from ct in ts
                      where ct.GetType() == type
                      select ct;

            return tcs;
        }

        public IEnumerable<Utensile> GetTools(MeasureUnit measureUnit)
        {
            return _magazzinoUtensile.GetTools(measureUnit);
        }



        public Materiale GetMateriale(Guid guid)
        {
            var ms = _magazzinoUtensile.GetMaterials();

            var m = (from mat in ms
                     where mat.MaterialeGuid == guid
                     select mat).FirstOrDefault();

            return m;
        }

        internal ToolMachine.ToolMachine GetMacchina(Guid machineGuid)
        {

            var m = (from mat in _macchine
                     where mat.MachineGuid == machineGuid
                     select mat).FirstOrDefault();

            return m;
        }

        public void SaveToolMachines(List<ToolMachine.ToolMachine> machines)
        {
            _macchine = machines;
            PathFolderHelper.SaveToolMachines(_macchine);
        }

        public void AddMacchina(ToolMachine.ToolMachine machine)
        {
            _macchine.Add(machine);

            PathFolderHelper.SaveToolMachines(_macchine);
        }
    }
    /*
     * importante è non serializzare singleton
     */
    /// <summary>
    /// Classe model.
    /// Devo fare una classe singleton che gestisce elementi tipo entity framework..
    /// Altrimenti con serializable perdo i riferimenti.
    /// </summary>
    public sealed class Singleton
    {
        static FileModel _instance = null;
        static readonly object Padlock = new object();

        public static void CreateNewModelClass(MeasureUnit measureUnit)
        {
            lock (Padlock)
            {
                _instance = new FileModel(measureUnit);
            }
        }

        public static FileModel Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        throw new Exception("Create Model First");
                    }
                    return _instance;
                }
            }
        }


        public void AddLavorazione(Lavorazione lavorazione)
        {
            Instance.AddLavorazione(lavorazione);
        }

        public void AddFaseDiLavoro(FaseDiLavoro lavorazione)
        {
            Instance.AddFaseDiLavoro(lavorazione);
        }
        public Lavorazione GetLavorazione(Guid lavorazioneguid)
        {
            return Instance.GetLavorazione(lavorazioneguid);
        }

        public FaseDiLavoro GetFaseDiLavoro(Guid faseGuid)
        {
            return Instance.GetFaseDiLavoro(faseGuid);
        }

        public MeasureUnit MeasureUnit
        {
            get { return Instance.MeasureUnit; }
        }

        static ProgramPreference _preference = null;

        private static Data _data;
        public static Data Data
        {
            get
            {
                lock (Padlock)
                {
                    return _data ?? (_data = PathFolderHelper.LoadData());
                }
            }
        }



        public static ProgramPreference Preference
        {
            get
            {
                lock (Padlock)
                {
                    if (_preference == null)
                    {
                        var p = PathFolderHelper.GetPreferenceData();
                        _preference = p;
                    }
                    return _preference;
                }
            }
        }

        public static void SetModelClass(FileModel model)
        {
            _instance = model;

        }


        public static void SetPreference(ProgramPreference preference)
        {
            _preference = preference;
        }
    }

    public enum MeasureUnit
    {
        Millimeter,
        Inch,
    }
}