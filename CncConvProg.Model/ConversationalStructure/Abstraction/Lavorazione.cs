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
using CncConvProg.Model.PreviewEntity;
using CncConvProg.Model.PreviewPathEntity;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.LatheTool;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using CncConvProg.Model.ToolMachine;
using MecPrev.Resources;

namespace CncConvProg.Model.ConversationalStructure.Abstraction
{
    /// <summary>
    /// Magari quando faro lavorazioni per tornio 
    /// faro 2 classi derivate 
    /// - lavorazioni tornio
    /// - lavorazioni centro
    /// </summary>
    [Serializable]
    public abstract class Lavorazione : IPreviewable, IEquatable<Lavorazione>, IWorkRotable
    {
        /*
         * 18/07/2011
         * Rivoluzione & Array :
         * 
         * la rivoluzione e array della lavorazioni , può essere effettuta per tutte lo lavorazioni
         * di centro  ,con grezzo non rotante, quindi per ora posso implementarlo nella classe base Lavorazione.
         * 
         * in pratica ho una matrice dove memorizzo sia la rotazione che array.
         * 
         * Come faccio ad applicarlo in modo non invasivo??
         * 
         * per ora mi preoccupo solo della parte preview..
         * 
         */

        /// <summary>
        /// Questa proprietà è comune in tutte le lavorazioni.
        /// Poi la mettero in LavorazioneCentro quando specializzero la classe lavorazioni.
        /// </summary>
        public double SicurezzaZ { get; set; }

        protected MeasureUnit MeasureUnit
        {
            get
            {
                return Singleton.Instance.MeasureUnit;
            }
        }

        public Guid LavorazioneGuid { get; private set; }

        /// <summary>
        /// Renderlo abstract
        /// </summary>
        public abstract List<Operazione> Operazioni { get; }
        //public FaseDiLavoro FaseDiLavoro { get; private set; }
        public int LavorazionePosition { get; set; }

        public abstract string Descrizione { get; }

        public Guid FaseDiLavoroGuid { get; private set; }

        public FaseDiLavoro FaseDiLavoro
        {
            get
            {
                return Singleton.Instance.GetFaseDiLavoro(FaseDiLavoroGuid);
            }
        }

        protected Lavorazione()
        {
            /*
             * Questa lavorazione la inserisco nelle lavorazione del parent solo su ok del dialogo
             */

            LavorazioneGuid = Guid.NewGuid();

            //LavorazionePosition = FaseDiLavoro.GetLavorazioniCount() + 1;
        }

        ///// <summary>
        ///// </summary>
        ///// <param name="operazione"></param>
        ///// <returns></returns>
        //internal abstract ProgramPhase GetOperationProgram(Operazione operazione);

        protected double SecureFeed;
        protected double ExtraCorsa;

        public abstract FaseDiLavoro.TipoFaseLavoro[] FasiCompatibili { get; }

        /// 18/08/2011
        /// <summary>
        /// Metodo V2 per creazione programma.
        /// Cerco di raggruppare ulteriormente metodi comuni per avere meno problemi.
        /// </summary>
        /// <returns></returns>
        internal abstract ProgramOperation GetOperationProgram(Operazione operazione);
        
        /// <summary>
        /// Metodo per ottenere miglior utensile fra quelli selezionati.
        /// </summary>
        /// <param name="operazione"></param>
        /// <param name="utensiles"></param>
        /// <param name="matGuid"></param>
        /// <returns></returns>
        public virtual Utensile PickBestTool(Operazione operazione, IEnumerable<Utensile> utensiles, Guid matGuid)
        {
            var tool = (from utensile in utensiles
                        from parametro in utensile.ParametriUtensile
                        where parametro.MaterialGuid == matGuid
                        orderby utensile.SaveTime descending
                        where utensile.OperazioneTipo == operazione.OperationType
                        select utensile).FirstOrDefault();

            return tool;
        }

        /// <summary>
        /// Metodo astratto dove si genera la parte del programma specifica di ogni lavorazione.
        /// La parte in comune è stata raggruppata nel metodo GetOperationProgram.
        /// </summary>
        /// <param name="programPhase"></param>
        /// <param name="operazione"></param>
        protected abstract void CreateSpecificProgram(ProgramOperation programPhase, Operazione operazione);



        protected MoveActionCollection GetFinalProgram(MoveActionCollection moveActionCollection)
        {
            var rslt = new MoveActionCollection();

            var preview = moveActionCollection;

            if (RotationAbilited)
            {
                //Angoli in degreesss...!! 
                var angleStep = 360 / NumberInstance;

                for (int i = 1; i <= NumberInstance; i++)
                {
                    var angleCurrent = (angleStep * i) + FirstAngle;

                    var rotationMatrix = new Matrix3D();

                    rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), angleCurrent),
                                            new System.Windows.Media.Media3D.Point3D(CenterRotationX, CenterRotationY, 0));


                    foreach (var entity2D in preview)
                    {
                        rslt.Add(entity2D.MultiplyMatrix(rotationMatrix));
                    }

                }

            }

            else if (TranslateAbilited)
            {
                var stepX = TransStepX;

                var stepY = TransStepY;

                for (int i = 0; i < TransCountY; i++)
                {
                    var yMultiplier = stepY * i;

                    for (int j = 0; j < TransCountX; j++)
                    {
                        var xMultiplier = stepX * j;

                        var rotationMatrix = new Matrix3D();

                        rotationMatrix.Translate(new Vector3D(xMultiplier, yMultiplier, 0));

                        foreach (var entity2D in preview)
                        {
                            rslt.Add(entity2D.MultiplyMatrix(rotationMatrix));
                        }
                    }

                }


            }
            else
            {
                return preview;
            }

            return rslt;

        }

        /// <summary>
        /// Restituisce anteprima lavorazione,
        /// Prende anteprima sorgente dalla lavorazione con metodo abstract
        /// poi fa le eventuali moltiplicazioni con matrici di rototraslazione
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEntity3D> GetPreview()
        {

            var rslt = new List<IEntity3D>();

            var preview = GetFinalPreview();

            if (preview == null) return rslt;

            foreach (var e in preview)
                if (e.PlotStyle == EnumPlotStyle.SelectedElement) e.PlotStyle = EnumPlotStyle.Element;

            if (RotationAbilited)
            {
                //Angoli in degreesss...!! 
                if (NumberInstance == 0) return rslt;

                var angleStep = 360 / NumberInstance;

                for (int i = 1; i <= NumberInstance; i++)
                {
                    var angleCurrent = (angleStep * i) + FirstAngle;

                    var rotationMatrix = new Matrix3D();

                    rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), angleCurrent),
                                            new System.Windows.Media.Media3D.Point3D(CenterRotationX, CenterRotationY, 0));


                    foreach (var entity2D in preview)
                    {
                        rslt.Add(entity2D.MultiplyMatrix(rotationMatrix));
                    }

                }

            }

            else if (TranslateAbilited)
            {
                var stepX = TransStepX;

                var stepY = TransStepY;

                for (int i = 0; i < TransCountY; i++)
                {
                    var yMultiplier = stepY * i;

                    for (int j = 0; j < TransCountX; j++)
                    {
                        var xMultiplier = stepX * j;

                        var rotationMatrix = new Matrix3D();

                        rotationMatrix.Translate(new Vector3D(xMultiplier, yMultiplier, 0));

                        foreach (var entity2D in preview)
                        {
                            rslt.Add(entity2D.MultiplyMatrix(rotationMatrix));
                        }
                    }

                }


            }
            else
            {
                return preview;
            }

            return rslt;
        }

        internal static string GetOperationDescription(LavorazioniEnumOperazioni enumOp)
        {
            switch (enumOp)
            {
                case LavorazioniEnumOperazioni.FresaturaSpianaturaSgrossatura:
                case LavorazioniEnumOperazioni.Sgrossatura:
                    {
                        return EditWorkRes.Roughing;

                    }
                    break;

                case LavorazioniEnumOperazioni.FresaturaSpianaturaFinitura:
                case LavorazioniEnumOperazioni.Finitura:
                    {
                        return EditWorkRes.Finishing;
                    }
                    break;

                case LavorazioniEnumOperazioni.Smussatura:
                    {
                        return EditWorkRes.Chamfering;
                    }
                    break;

                case LavorazioniEnumOperazioni.FresaturaFilettare:
                    {
                        return GuiRes.MillThreadOp;
                    }
                    break;

                case LavorazioniEnumOperazioni.AllargaturaBareno:
                    {
                        return EditWorkRes.Roughing;
                    }
                    break;
                case LavorazioniEnumOperazioni.ForaturaPunta:
                    {
                        return GuiRes.Drill;
                    }
                    break;

                case LavorazioniEnumOperazioni.ForaturaCentrino:
                    {
                        return GuiRes.CenterDrill;
                    }
                    break;

                case LavorazioniEnumOperazioni.ForaturaSmusso:
                    {
                        return GuiRes.Chamfer;
                    }
                    break;

                case LavorazioniEnumOperazioni.ForaturaMaschiaturaDx:
                    {
                        return GuiRes.Tapping;
                    }
                    break;
                case LavorazioniEnumOperazioni.ForaturaLamatore:
                    {
                        return GuiRes.Counterboring;
                    }
                    break;

                case LavorazioniEnumOperazioni.ForaturaAlesatore:
                    {
                        return GuiRes.Reamering;
                    }
                    break;

                case LavorazioniEnumOperazioni.ForaturaBareno:
                    {
                        return GuiRes.Boring;
                    }
                    break;

                case LavorazioniEnumOperazioni.TornituraSfacciaturaSgrossatura:
                    {
                        return GuiRes.FaceTurning + " " + EditWorkRes.Roughing;

                    } break;
                case LavorazioniEnumOperazioni.TornituraSfacciaturaFinitura:
                    {
                        return GuiRes.FaceTurning + " " + EditWorkRes.Finishing; ;

                    } break;
                case LavorazioniEnumOperazioni.TornituraSgrossatura:
                    {
                        return EditWorkRes.Roughing;
                    }
                    break;

                case LavorazioniEnumOperazioni.TornituraFinitura:
                    {
                        return EditWorkRes.Finishing;
                    }
                    break;

                case LavorazioniEnumOperazioni.TornituraScanalaturaFinitura:
                    {
                        return GuiRes.TurnGroove + " " + EditWorkRes.Finishing;
                    }
                    break;
                case LavorazioniEnumOperazioni.TornituraScanalaturaSgrossatura:
                    {
                        return GuiRes.TurnGroove + " " + EditWorkRes.Roughing;
                    }
                    break;

                case LavorazioniEnumOperazioni.TornituraFilettatura:
                    {
                        return GuiRes.TurnThreadingOp;
                    }
                    break;


                default:
                    return "Not Defined";
            }

        }

        public bool IsValid { get; set; }

        public virtual void SetProfile(List<Geometry.RawProfile2D.RawEntity2D> list)
        {

        }
        /*
         * come operation program anche qui posso fare come preview per gestire array e rotazioni.
         * 
         * ovverro
         * 
         * in quanto il codice per creare la parte e richiamo utensile puo esserre messo in modo astratto
         * 
         * la parte che mi serve è solamente qella relativa al codice , comunque per ora non me ne proccoupa
         */

        internal List<IPreviewEntity> GetPathPreview(ProgramOperation programPhase, ToolMachine.ToolMachine toolMachine)
        {
            return toolMachine.GetPreview(programPhase);
        }

        internal static Utensile CreateTool(LavorazioniEnumOperazioni enumOperationType, MeasureUnit unit)
        {
            switch (enumOperationType)
            {
                case LavorazioniEnumOperazioni.TornituraScanalaturaSgrossatura:
                case LavorazioniEnumOperazioni.TornituraScanalaturaFinitura:
                    return new UtensileScanalatura(unit);

                case LavorazioniEnumOperazioni.FresaturaSpianaturaFinitura:
                case LavorazioniEnumOperazioni.FresaturaSpianaturaSgrossatura:
                    return new FresaSpianare(unit);

                case LavorazioniEnumOperazioni.AllargaturaBareno:
                case LavorazioniEnumOperazioni.Sgrossatura:
                case LavorazioniEnumOperazioni.Finitura:
                case LavorazioniEnumOperazioni.FresaturaFilettare:
                    return new FresaCandela(unit);

                case LavorazioniEnumOperazioni.ForaturaCentrino:
                    return new Centrino(unit);

                case LavorazioniEnumOperazioni.ForaturaPunta:
                    return new Punta(unit);

                case LavorazioniEnumOperazioni.Smussatura:
                case LavorazioniEnumOperazioni.ForaturaSmusso:
                    return new Svasatore(unit);

                case LavorazioniEnumOperazioni.ForaturaMaschiaturaDx:
                    return new Maschio(unit);

                case LavorazioniEnumOperazioni.ForaturaBareno:
                    return new Bareno(unit);

                case LavorazioniEnumOperazioni.ForaturaAlesatore:
                    return new Alesatore(unit);

                case LavorazioniEnumOperazioni.ForaturaLamatore:
                    return new Lamatore(unit);

                case LavorazioniEnumOperazioni.TornituraSgrossatura:
                case LavorazioniEnumOperazioni.TornituraFinitura:
                case LavorazioniEnumOperazioni.TornituraSfacciaturaSgrossatura:
                case LavorazioniEnumOperazioni.TornituraSfacciaturaFinitura:
                    return new UtensileTornitura(unit);

                case LavorazioniEnumOperazioni.TornituraFilettatura:
                    return new UtensileFilettare(unit);
            }

            throw new NotImplementedException();

        }

        /*
         * todo : 
         * questi 2 metodi sono simili.
         * 
         * magari fare - 1 metodo che ti da il tipo dell'utensile 
         * 
         * per create tool basta usare activator per avere oggetto 
         * 
         * per compatibletools usi il tipo per avere la lista di oggetti da magazzino utensile.
         * 
         * in casi particolari controlli che alla lavorazione non sia legata un'interfaccia per avere 
         * parametri per filtrare ricerca.
         * es IDiametroPuntaForatura 
         * cosi restituisco solo punte con diametro certi in tutti altri casi è ok qualsiasi parametro..
         */

        internal List<Utensile> GetCompatibleTools(LavorazioniEnumOperazioni operationType, MagazzinoUtensile magazzino)
        {
            IEnumerable<Utensile> tools = null;

            switch (operationType)
            {
                case LavorazioniEnumOperazioni.FresaturaSpianaturaFinitura:
                case LavorazioniEnumOperazioni.FresaturaSpianaturaSgrossatura:
                    tools = magazzino.GetTools<FresaSpianare>(MeasureUnit);
                    break;


                case LavorazioniEnumOperazioni.AllargaturaBareno:
                case LavorazioniEnumOperazioni.Sgrossatura:
                case LavorazioniEnumOperazioni.Finitura:
                //case LavorazioniEnumOperazioni.SgrossaturaTrocoidale:
                case LavorazioniEnumOperazioni.FresaturaFilettare:
                    tools = magazzino.GetTools<FresaCandela>(MeasureUnit);
                    break;

                case LavorazioniEnumOperazioni.Smussatura:
                case LavorazioniEnumOperazioni.ForaturaSmusso:
                    {
                        tools = magazzino.GetTools<Svasatore>(MeasureUnit);
                    } break;

                case LavorazioniEnumOperazioni.ForaturaPunta:
                    {
                        tools = magazzino.GetTools<Punta>(MeasureUnit);
                        /* filtro punta diametro */
                    } break;

                case LavorazioniEnumOperazioni.ForaturaCentrino:
                    {
                        tools = magazzino.GetTools<Centrino>(MeasureUnit);

                    } break;


                case LavorazioniEnumOperazioni.ForaturaLamatore:
                    {
                        tools = magazzino.GetTools<Lamatore>(MeasureUnit);

                    } break;


                case LavorazioniEnumOperazioni.ForaturaBareno:
                    {
                        tools = magazzino.GetTools<Bareno>(MeasureUnit);

                    } break;

                case LavorazioniEnumOperazioni.ForaturaAlesatore:
                    {
                        tools = magazzino.GetTools<Alesatore>(MeasureUnit);

                    } break;

                default:
                    {
                        throw new NotImplementedException("DrBaseClass.GetCompTools");
                        //return magazzino.GetDrill(DiametroForatura, unit);
                    } break;
            }

            if (tools != null)
                return tools.ToList();

            return null;
        }

        public bool Equals(Lavorazione other)
        {
            return other.LavorazioneGuid == LavorazioneGuid;
        }

        //internal void SetFaseDiLavoro(Abstraction.FaseDiLavoro faseDiLavoro)
        //{
        //    FaseDiLavoro = faseDiLavoro;
        //}

        public bool RotationAbilited { get; set; }

        public double CenterRotationX { get; set; }

        public double CenterRotationY { get; set; }

        public double FirstAngle { get; set; }

        public int NumberInstance { get; set; }

        public bool TranslateAbilited { get; set; }

        public double TransStepX { get; set; }

        public double TransCountX { get; set; }

        public double TransStepY { get; set; }

        public int TransCountY { get; set; }

        /// <summary>
        /// Restituisce preview dopo elaborazione rotazione e array.
        /// </summary>
        /// <returns></returns>
        protected abstract List<IEntity3D> GetFinalPreview();

        public void RegeneretaGuid()
        {
            LavorazioneGuid = Guid.NewGuid();
        }

        internal void ResetFaseDiLavoro(Guid faseDiLavoroGuid)
        {
            FaseDiLavoroGuid = faseDiLavoroGuid;
        }



        public void SetProgramDirty()
        {
            foreach (var operazione in Operazioni)
            {
                operazione.ProgramNeedUpdate = true;
            }
        }

        public bool IsCompatible(Lavorazione lavorazioneTarget)
        {
            foreach (var tipoFaseLavoro in FasiCompatibili)
            {
                if (lavorazioneTarget.FasiCompatibili.Contains(tipoFaseLavoro))
                    return true;
            }
            return false;
        }
    }
}