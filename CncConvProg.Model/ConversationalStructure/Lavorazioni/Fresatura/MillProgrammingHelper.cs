using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.PathGenerator;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura
{

    /// <summary>
    /// Classe Ausiliaria per creare lavorazioni.
    /// è richiamata esclusivamente dalle classi lavorazioni, all'interno dell'assembly model.
    /// 
    /// Con questa classe cerco di raggruppare tutti i metodi per le varie lavorazioni
    /// 
    /// Si interfaccia con esterno con metodi che richiedono solo i parametri necessari.
    /// Poi internamente usano lo stesso metodo.
    /// Questo e per avere un 'unico metodo da controllare invece di averne un 10ina e aver difficolta a gestirli.
    /// 
    /// Di metodi fondametali al momento ne ho 2 una per esterna e 1 per interni.
    /// 
    /// poi vedere come si puo raggrupparle.
    /// </summary>
    internal static class MillProgrammingHelper
    {
        /*
         * Considerazioni :
         * Per ora la fresa si muove in maniera concorde.
         * Quindi rispetto al profilo da lavorare si muove in  :
         *  - Senso Orario CW per profilo esterno, sta alla destra del profilo G42
         *  - Senso Antiorario CCW per profilo interno , sta alla SINISTRA del Profilo G41
         */

        #region Pocket Milling


        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveActionCollection"></param>
        /// <param name="workProfile"></param>
        /// <param name="profonditaLav"></param>
        /// <param name="profPassata"></param>
        /// <param name="larghPassata"></param>
        /// <param name="millDia"></param>
        /// <param name="extraCorsa"></param>
        /// <param name="startZ">Inizio Z</param>
        /// <param name="secureZ">Sicurezza Z</param>
        /// <param name="restMaterialXy">Materiale per Finitura XY</param>
        /// <param name="restMaterialZ">Mat Finitura Z</param>
        internal static void GetInternRoughing(MoveActionCollection moveActionCollection, Profile2D workProfile, double profonditaLav,
                 double profPassata, double larghPassata, double millDia, double extraCorsa, double startZ, double secureZ, double restMaterialXy, double restMaterialZ)
        {
            /*
             */
            PocketMilling(moveActionCollection, workProfile, profonditaLav, secureZ, startZ, millDia, larghPassata,
                          profPassata, false, restMaterialXy);
        }

        /// <summary>
        /// Crea programma per smusso esterno.
        /// Inserisco anche la possibilita di usare compensazione
        /// </summary>
        internal static void GetInternChamfer(MoveActionCollection moveActionCollection, Profile2D workProfile, double profonditaChamferTool, double diameterChamferTool, double extraCorsa, bool isCncCompensated, double startZ, double secureZ)
        {
            /*
             * per fare si che faccia una passata solamente nella direzione del sovrametallo assegno una 
             */
            var larghPassata = diameterChamferTool / 2;

            double profPassata;
            var profonditaLav = profPassata = profonditaChamferTool;

            PocketMilling(moveActionCollection, workProfile, profonditaLav, secureZ, startZ,
                          diameterChamferTool, larghPassata, profPassata, isCncCompensated, 0, true);
        }

        /// <summary>
        /// Crea programma per smusso esterno.
        /// Inserisco anche la possibilita di usare compensazione
        /// </summary>
        internal static void GetInternFinish(MoveActionCollection moveActionCollection, Profile2D workProfile, double profonditaLavorazione, double profPassata, double finishMillDiameter, double extraCorsa, bool isCncCompensated, double startZ, double secureZ)
        {
            /*
             * per fare si che faccia una passata solamente nella direzione del sovrametallo assegno una 
             */


            double larghPassata = finishMillDiameter / 2;

            PocketMilling(moveActionCollection, workProfile, profonditaLavorazione, secureZ, startZ,
                          finishMillDiameter, larghPassata, profPassata, isCncCompensated, 0, true);
        }

        /*
         * si da il caso che il profilo elaborato restituisce più profili
         * 
         * 2 
         * 
         * nel successivo restituisce più profili.
         * devo associare questi profili all'interno del parent.
         * 
         */

        private class OffsetTree
        {
            private readonly List<ProfilesLevel> _tempProfiles = new List<ProfilesLevel>();
            private readonly List<ProfilesLevel> _profilesLevels = new List<ProfilesLevel>();

            public void AssignProfiless(IEnumerable<Profile2D> profiles)
            {


                if (_profilesLevels.Count == 0) // se albero è vuoto
                {
                    var rootLevel = new ProfilesLevel(null, null);
                    _profilesLevels.Add(rootLevel);

                    foreach (var profile2D in profiles)
                    {
                        var p = new ProfilesLevel(rootLevel, profile2D);
                        _tempProfiles.Add(p);
                    }

                }
                // se invece non è vuoto deve assegnare ad ogni profilo il suo parent.
                else
                {
                    var temp = new List<ProfilesLevel>(_tempProfiles);

                    while (_tempProfiles.Count > 0)
                    {
                        _tempProfiles.Remove(_tempProfiles.LastOrDefault());
                    }

                    foreach (var profile2D in profiles)
                    {
                        foreach (var profilesLevel in temp)
                        {
                            var profParent = profilesLevel.ProfileSource;

                            var rslt = GeometryHelper.PolygonInPolygon(profParent.GetPointListP2(),
                                                                       profile2D.GetPointListP2());

                            if (rslt)
                            {
                                var p = new ProfilesLevel(profilesLevel, profile2D);
                                _tempProfiles.Add(p);
                            }

                        }

                    }
                }
            }


            internal List<Profile2D> GetResultList()
            {
                var rslt = new List<Profile2D>();

                foreach (var profilesLevel in _profilesLevels)
                {
                    profilesLevel.PutOffset(rslt);
                }
                rslt.Reverse();
                return rslt;
            }
        }

        /// <summary>
        /// Insieme di profili che si contengono
        /// </summary>
        private class ProfilesLevel
        {

            internal void PutOffset(List<Profile2D> rslt)
            {
                if (ProfileSource != null)
                    rslt.Add(ProfileSource);

                if (Children == null || Children.Count == 0)
                {
                    return;
                }

                foreach (var profilesLevel in Children)
                {
                    profilesLevel.PutOffset(rslt);
                }
            }
            public ProfilesLevel Parent { get; set; }
            public List<ProfilesLevel> Children { get; set; }

            public Profile2D ProfileSource { get; private set; }

            public ProfilesLevel(ProfilesLevel parent, Profile2D profile2D)
            {
                Children = new List<ProfilesLevel>();

                Parent = parent;
                if (Parent != null)
                    Parent.Children.Add(this);

                ProfileSource = profile2D;
            }
        }

        //public bool TryAssign(Profile2D profile2D)
        //{

        //    //var rslt = GeometryHelper.PolygonInPolygon(lastInsertedProfile.GetPointListP2(), profile2D.GetPointListP2());

        //    //if (rslt)
        //    //{
        //    //    this.Add(profile2D);
        //    //    return true;
        //    //}

        //    return false;

        //}

        /// <summary>
        /// Calcolo offset in modo crescente.
        /// Poi una volta finito faccio reverse della lista.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="distance"></param>
        /// <param name="radiusValue"></param>
        /// <param name="onlyFinishPasses"></param>
        /// <param name="isCncCompesated"></param>
        /// <param name="restMaterialXy"></param>
        /// <returns></returns>
        private static List<Profile2D> CalculateRoughingInternOffset(Profile2D origin, double distance, double radiusValue, bool onlyFinishPasses, bool isCncCompesated, double restMaterialXy = 0)
        {

            var offsetTree = new OffsetTree();

            // Il primo offset è per raggio fresa + eventuale sovrametallo.
            var firstOffset = -Math.Abs(radiusValue) - restMaterialXy;

            var firstPaths = origin.Offset(firstOffset, false);

            if (firstPaths == null)
                return null;

            offsetTree.AssignProfiless(firstPaths);

            if (onlyFinishPasses)
            {
                if (isCncCompesated)
                {
                    firstPaths = origin.Offset(0, false);
                }
                return firstPaths;


            }



            var offsetValue = firstOffset;

            // non va bene passare profilo elaborato, ma devo passare originale
            RicorsivaGenerateInternOffset(origin, ref offsetValue, distance, false, offsetTree);


            return offsetTree.GetResultList();
        }
        ///// <summary>
        ///// Metodo per calcolare offset interni.
        ///// Presupposti,
        ///// Se faccio offset di un profilo offsettato c'è errore.
        ///// Se faccio offset poi lo trasformo con parsed arc. cmq c'è errore di arrotondamento.
        ///// Devo sempre fare offset del profilo originale.
        ///// 
        ///// Nei profili interni possono capitare profili annidati , e devo tenere giusto annidamento.
        ///// Per fare ordine creo una struttura in modo da ordinarli correttamente
        ///// </summary>
        ///// <param name="origin"></param>
        ///// <param name="distance"></param>
        ///// <param name="radiusValue"></param>
        ///// <param name="onlyFinishPasses"></param>
        ///// <param name="restMaterialXy"></param>
        ///// <returns></returns>
        //private static List<Profile2D> CalculateInternOffset(Profile2D origin, double distance, double radiusValue, bool onlyFinishPasses, double restMaterialXy = 0)
        //{

        //    var firstOffset = -Math.Abs(radiusValue) - restMaterialXy;

        //    var firstPaths = origin.Offset(firstOffset, false, true);

        //    if (firstPaths == null)
        //        return null;

        //    if (onlyFinishPasses)
        //        return firstPaths;

        //    var rslt = new List<Profile2D>();

        //    var offsetValue = firstOffset;

        //    foreach (var firstPath in firstPaths)
        //    {
        //        rslt.Add(firstPath);

        //        // non va bene passare profilo elaborato, ma devo passare originale
        //        RicorsivaGenerateInternOffset(firstPath, ref offsetValue, distance, false, ref rslt);
        //    }


        //    return rslt;
        //}

        private static void RicorsivaGenerateInternOffset(Profile2D profile2D, ref  double offset, double distance, bool clockwise, OffsetTree offsetTree)
        {
            offset -= distance;

            // Calcola offset , ritorna 1 o più contorni 
            var offsetRslt = profile2D.Offset(offset, clockwise);

            // se non ritorna più niente termina metodo 
            if (offsetRslt == null)
                return;

            offsetTree.AssignProfiless(offsetRslt);

            RicorsivaGenerateInternOffset(profile2D, ref offset, distance, clockwise, offsetTree);
        }

        //private static void RicorsivaGenerateInternOffset(Profile2D profile2D, ref  double offset, double distance, bool clockwise, ref List<Profile2D> profile2DsList)
        //{
        //    offset -= distance;

        //    // Calcola offset , ritorna 1 o più contorni 
        //    var offsetRslt = profile2D.Offset(offset, clockwise);

        //    // se non ritorna più niente termina metodo 
        //    if (offsetRslt == null)
        //        return;

        //    foreach (var singleContour in offsetRslt)
        //    {
        //        profile2DsList.Add(singleContour);

        //        RicorsivaGenerateInternOffset(singleContour, ref offset, distance, clockwise, ref profile2DsList);
        //    }
        //}

        //public static List<Profile2D> GetProfiles(Profile2D profile2D, bool compensationWithCnc, double offset, bool clockWise)
        //{

        //    if (compensationWithCnc || offset == 0) return new List<Profile2D>() { profile2D };

        //    return profile2D.Offset(offset, clockWise);
        //}
        /// <summary>
        /// Non posso usare questo.
        /// Se per caso faccio il profilo di un profilo mi sbambana offset.
        /// 
        /// Metodo che restituisce profilo da lavorare.
        /// 
        /// Se non viene utilizzato compensazione centro creo il profilo con il metodo.
        /// Altrimenti restituisco profilo originale 
        /// </summary>
        /// <param name="profile2D"></param>
        /// <param name="compensationWithCnc"></param>
        /// <param name="offset"></param>
        /// <param name="clockWise"></param>
        /// <returns></returns>
        /// <summary>
        /// Metodo Principale per Creare Sgrossatura Interna 
        /// </summary>
        /// <param name="programPhase">Programma</param>
        /// <param name="profile2D">Profilo da lavorare</param>
        /// <param name="profondita">Profonsita di Lavorazione</param>
        /// <param name="zSicurezza">Z Sicurezza</param>
        /// <param name="zIniziale">Z Inizio Lavorazione</param>
        /// <param name="diaFresa">Diametro Fresa</param>
        /// <param name="larghPassata">Larghezza Passata</param>
        /// <param name="profPassata">Profondita Passata</param>
        /// <param name="finishWithCompensation">Finitura Con Compenzazione Cnc</param>
        /// <param name="restMaterialeXy"></param>
        /// <param name="onlyFinishPasses"></param>
        private static void PocketMilling(MoveActionCollection programPhase, Profile2D profile2D, double profondita, double zSicurezza, double zIniziale, double diaFresa, double larghPassata, double profPassata, bool finishWithCompensation, double restMaterialeXy, bool onlyFinishPasses = false)
        {
            /*
             * teoria.
             * - Prendo profilo 
             *  - Se valido faccio offset negativo del raggio della fresa
             * - Poi faccio offset della larghezza di passata fino a che il metodo non mi restituisce più niente. ( ho raggiunto il massimo ) 
             */

            /*
             * Controllo Valori
             */
            if (CheckValueHelper.GreatherThanZero(new[] { profondita, larghPassata, diaFresa, profPassata, }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(zSicurezza, zIniziale),
                                                  new KeyValuePair<double, double>(diaFresa, larghPassata),
                                              })
                ) return;

            if (profile2D == null) return;

            var raggioFresa = diaFresa / 2;

            var profile = profile2D;

            //var offsetCountorns = CalculateInternOffset(profile, larghPassata, diaFresa / 2, onlyFinishPasses, restMaterialeXy);
            var offsetCountorns = CalculateRoughingInternOffset(profile, larghPassata, diaFresa / 2, onlyFinishPasses, finishWithCompensation, restMaterialeXy);

            if (offsetCountorns == null)
                return;

            /*
             * TODO:
             * 
             * Come punto attacco posso usare una normale al primo punto profilo.
             * 
             * Come primo punto del profilo devo scegliere automaticamente o punto più vicino a immissione utente.
             * 
             */

            /*
 * Devo spostarmi in xy del primo punto 
 */
            //var attacPath = offsetCountorns.FirstOrDefault();

            //if (attacPath == null)
            //    return;

            //var attacPoint = attacPath.Source.FirstOrDefault();

            //if (attacPoint == null)
            //    throw new Exception();

            //programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, attacPoint.GetFirstPnt().X, attacPoint.GetFirstPnt().Y, null);

            //var zCurrent = zIniziale;

            var zFinale = zIniziale - profondita;

            //programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);

            int compensationMode = 0;
            //Assumo che lavoro in concordanza ..
            if (finishWithCompensation)
                compensationMode = 1;

            WriteContourn(programPhase, offsetCountorns, diaFresa, zSicurezza, zIniziale, zFinale, profPassata, 0, null, compensationMode);

        }
        #endregion

        #region Contour Milling

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveActionCollection"></param>
        /// <param name="workProfile">Profilo</param>
        /// <param name="stockTrimProfile">Trim Profile</param>
        /// <param name="stockTrimmingAbilited">Trim On</param>
        /// <param name="profonditaLav">Profondita</param>
        /// <param name="profPassata">Prof.Passata</param>
        /// <param name="sovraMetallo">Sovrametallo</param>
        /// <param name="larghPassata">Larg.Passata</param>
        /// <param name="millDia">Diametro Ute</param>
        /// <param name="extraCorsa">Extracorsa</param>
        /// <param name="startZ">Start Z</param>
        /// <param name="secureZ">Secure Z</param>
        /// <param name="restMaterialProfile">Rest Profile Material</param>
        /// <param name="restMaterialZ">Rest Profile Z</param>
        internal static void GetExterRoughing(MoveActionCollection moveActionCollection, Profile2D workProfile,
                                            Profile2D stockTrimProfile, bool stockTrimmingAbilited, double profonditaLav,
                                            double profPassata, double sovraMetallo, double larghPassata,
                                            double millDia, double extraCorsa,
                                            double startZ, double secureZ,
                                            double restMaterialProfile,
                                            double restMaterialZ)
        {
            /*
             * Creo profilo maggiorato 
             */
            MillExternGeneral(moveActionCollection, millDia, larghPassata, profPassata, extraCorsa, sovraMetallo, startZ, profonditaLav, secureZ, workProfile, stockTrimProfile, stockTrimmingAbilited, false, restMaterialProfile);

        }

        /// <summary>
        /// Crea programma per smusso esterno.
        /// Inserisco anche la possibilita di usare compensazione
        /// </summary>
        internal static void GetExterChamfer(MoveActionCollection moveActionCollection, Profile2D workProfile, Profile2D stockTrimProfile, bool stockTrimmingAbilited, double profonditaChamferTool, double diameterChamferTool, double extraCorsa, bool isCncCompensated, double startZ, double secureZ)
        {
            /*
             * per fare si che faccia una passata solamente nella direzione del sovrametallo assegno una 
             */
            double larghPassata;
            double sovrametallo = larghPassata = diameterChamferTool / 2;

            double profPassata;
            double profonditaLav = profPassata = profonditaChamferTool;


            MillExternGeneral(moveActionCollection, diameterChamferTool, larghPassata, profPassata, extraCorsa,
                              sovrametallo, startZ, profonditaLav, secureZ, workProfile, stockTrimProfile,
                              stockTrimmingAbilited, isCncCompensated);
        }

        /// <summary>
        /// Programma per smusso esterno
        /// </summary>
        /// <param name="moveActionCollection"></param>
        /// <param name="workProfile"></param>
        /// <param name="stockTrimProfile"></param>
        /// <param name="stockTrimmingAbilited"></param>
        /// <param name="profonditaLavorazione"></param>
        /// <param name="profPassata"></param>
        /// <param name="finishMillDiameter"></param>
        /// <param name="extraCorsa"></param>
        /// <param name="isCncCompensated">Comp CNC</param>
        /// <param name="startZ">Inizio Z</param>
        /// <param name="secureZ">Secure Z</param>
        internal static void GetExterFinish(MoveActionCollection moveActionCollection, Profile2D workProfile, Profile2D stockTrimProfile, bool stockTrimmingAbilited, double profonditaLavorazione, double profPassata, double finishMillDiameter, double extraCorsa, bool isCncCompensated, double startZ, double secureZ)
        {
            /*
             * per fare si che faccia una passata solamente nella direzione del sovrametallo assegno una 
             */
            double larghPassata;
            double sovrametallo = larghPassata = finishMillDiameter / 2;

            MillExternGeneral(moveActionCollection, finishMillDiameter, larghPassata, profPassata, extraCorsa, sovrametallo, startZ, profonditaLavorazione, secureZ, workProfile, stockTrimProfile, stockTrimmingAbilited, isCncCompensated);

        }

        /// <summary>
        /// Metodo Generale Per Lavorazione Contornatura Esterna di Profili
        /// </summary>
        /// <param name="programPhase"></param>
        /// <param name="diaFresa"></param>
        /// <param name="larghPassata"></param>
        /// <param name="profPassata"></param>
        /// <param name="extraCorsa"></param>
        /// <param name="sovraMetallo"></param>
        /// <param name="zIniziale"></param>
        /// <param name="profondita"></param>
        /// <param name="zSicurezza"></param>
        /// <param name="profile2D"></param>
        /// <param name="stockProfile">Profilo del TrimStock</param>
        /// <param name="tpAbilited">Trimming Path Abilited</param>
        /// <param name="finishWithCncCompensation">Ultima Passata Di Finitura Con Compensazione CNC</param>
        private static void MillExternGeneral(MoveActionCollection programPhase, double diaFresa, double larghPassata, double profPassata, double extraCorsa,
                                                            double sovraMetallo, double zIniziale, double profondita, double zSicurezza, Profile2D profile2D, Profile2D stockProfile, bool tpAbilited, bool finishWithCncCompensation, double restMaterialProfile = 0)
        {
            /*
             * Controllo Valori
             */
            if (CheckValueHelper.GreatherOrEqualZero(new[] { extraCorsa }) ||
                CheckValueHelper.GreatherThanZero(new[] { sovraMetallo, profondita, larghPassata, diaFresa, profPassata, }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(zSicurezza, zIniziale),
                                                  new KeyValuePair<double, double>(diaFresa, larghPassata),
                                              })
                ) return;

            if (profile2D == null) return;

            /*
             * OSSERVAZIONI PER STOCK TRIMMING
             * - Lo stock deve essere minimo il bounding Box del profilo. per evitare complicazioni.
             * - I Profili che genero devo avere già integrato lo spostamento di avvicinamento.
             * - Magari i profili che genero non serve che siano della classe Profile2D ma basta che siano un set di punti.
             */

            /*
             * Le passate le costruisco dalla prima ( offset raggio fresa ), incrementando della larghezza passata fino a coprire tutto il sovrametallo
             */

            var raggioFresa = diaFresa / 2;

            var distanzaOffsetCorrente = sovraMetallo + raggioFresa;

            var continueCycle = distanzaOffsetCorrente > 0;

            var offsetCountorns = new List<Profile2D>();

            var distanzaAttacco = distanzaOffsetCorrente + extraCorsa;

            var attacPath = profile2D.Offset(distanzaAttacco, false);

            if (attacPath == null) return;

            var attacPoint = attacPath.First().Source.FirstOrDefault();

            if (attacPoint == null)
                throw new Exception();

            var attachPoint = attacPoint.GetFirstPnt();
            /*
             * qui genero i profili di contornatura,
             * 
             * Ad ogni profilo generato aggiungo anche il punto di attacco
             * ( il punto finale del profilo precedente..)
             */
            while (continueCycle)
            {
                distanzaOffsetCorrente -= larghPassata;

                var insertSourceProfile = false;
                if (distanzaOffsetCorrente <= raggioFresa + restMaterialProfile)
                {
                    distanzaOffsetCorrente = raggioFresa + restMaterialProfile;

                    if (finishWithCncCompensation)
                        insertSourceProfile = true;

                    continueCycle = false;
                }

                List<Profile2D> path;

                //Con compensazione utensile dovrei passare profilo pari pari, contenente archi
                // ma in questo modo mi salta trim, per qui trasformo in linee linee
                if (insertSourceProfile)
                {
                    var profile = profile2D.Offset(0, false);

                    path = new List<Profile2D>() { profile.FirstOrDefault() };
                }

                else
                {
                    path = profile2D.Offset(distanzaOffsetCorrente, false);
                }


                /*
                 * Nel caso della contornatura esterna prendo solamente il primo profilo
                 */
                if (path.Count > 0)
                {
                    var p = path[0];

                    p.ToolPathStartPoint = p.ToolPathEndPoint = attachPoint;

                    offsetCountorns.Add(path[0]);

                    /*
                     * Inserisco attac point,
                     */

                    attachPoint = p.Source.Last().GetLastPnt();
                }
            }
            // stock trimming experiment ..
            var offsetCounterStockTrimmed = new List<Profile2D>();

            var polygon = stockProfile;

            if (tpAbilited && stockProfile != null) // se la funzione di trimm del profilo è abilitata
            {
                /*
                 * verificare bb del profilo
                 */
                foreach (var offsetCountorn in offsetCountorns)
                {
                    try
                    {
                        var rs = GeometryHelper.TrimProfileByStockPolygon(offsetCountorn, polygon, raggioFresa + extraCorsa);

                        // aggiungere distanza di sicurezza , estendere linee
                        if (rs != null && rs.Count > 0)
                            offsetCounterStockTrimmed.AddRange(rs);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("MillProgrammingHelper.TrimmingPart.");
                    }

                }
            }
            else
                offsetCounterStockTrimmed = offsetCountorns;

            var zFinale = zIniziale - profondita;

            int compensationMode = 0;

            //Assumo che lavoro in concordanza ..
            if (finishWithCncCompensation)
                compensationMode = 2;

            WriteContourn(programPhase, offsetCounterStockTrimmed, diaFresa, zSicurezza, zIniziale, zFinale, profPassata, extraCorsa, polygon, compensationMode);

        }

        /// <summary>
        /// Metodo per scrivere il programma universale con in ingresso lista di profili
        /// </summary>
        /// <param name="moveActionCollection"></param>
        /// <param name="offset"></param>
        /// <param name="diaFresa"></param>
        /// <param name="zSicurezza"></param>
        /// <param name="profPassata"></param>
        /// <param name="extraCorsa"></param>
        /// <param name="polygon"></param>
        /// <param name="isCncCompensated">
        /// Se è true. all'ultimo profilo attiva la compensazione utensile.
        /// Di norma si tratta del profilo non modificato da funzione di offset..
        /// 
        /// 0 = disattivato
        /// 1 = g41
        /// 2 = g42
        /// 
        /// </param>
        /// <param name="zIniziale"></param>
        /// <param name="zFinale"></param>
        //private static void WriteContourn(MoveActionCollection moveActionCollection, List<Profile2D> offset, double diaFresa, double zSicurezza, double zIniziale, double zFinale, double profPassata, double extraCorsa, Profile2D polygon, int isCncCompensated = 0)
        //{
        //    try
        //    {
        //        if (offset.Count <= 0)
        //            return;

        //        var zCurrent = zIniziale;

        //        while (zCurrent > zFinale)
        //        {
        //            zCurrent -= profPassata;
        //            if (zCurrent < zFinale)
        //                zCurrent = zFinale;


        //            Point2D lastMovePoint = null;

        //            for (var i = 0; i < offset.Count; i++)
        //            {
        //                var element = offset[i];

        //                // Faccio parse per risolvere archi
        //                var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

        //                //var 
        //                var source = parsedCountor;
        //                //  source = element;

        //                /*
        //                 * se punto iniziale si trova dentro profilo fare :
        //                 * - trovare nearest point da punto attacco e profilo grezzo  
        //                 * - fare estensione segmento punto iniziale - punto vicino 
        //                 * - punto attacco è punto esteso.
        //                 */

        //                /*
        //                 * Per correttezza il profileStartPoint deve essere definito quando uso la compensazione utensile..
        //                 */

        //                Point2D profileStartPoint;
        //                // se è l'ultimo profilo della serie e devo usare la compensazione utensile il profile start point deve essere definito..

        //                if (i == offset.Count - 1 && isCncCompensated != 0)
        //                {
        //                    profileStartPoint = source.ToolPathStartPoint;

        //                    if (profileStartPoint == null)
        //                        throw new Exception("Profile start point non definito con compensazione.");

        //                }
        //                else
        //                {
        //                    profileStartPoint = source.ToolPathStartPoint ?? source.Source.First().GetFirstPnt();
        //                }

        //                if (lastMovePoint == null)
        //                {

        //                    //if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
        //                    //{

        //                    //}

        //                    moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X,
        //                                               profileStartPoint.Y, null);
        //                    moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
        //                    moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
        //                }
        //                else
        //                {
        //                    var distance = GeometryHelper.Distance(profileStartPoint, lastMovePoint);

        //                    if (distance > diaFresa)
        //                    {
        //                        if (polygon != null)
        //                            if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
        //                            {
        //                                var closestPoint = GeometryHelper.GetClosestPoint(polygon.GetPointListP2(),
        //                                                                                  profileStartPoint);

        //                                var temp = new Point2D(profileStartPoint);

        //                                var newStartPoint = GeometryHelper.GetPointAtDistance(profileStartPoint,
        //                                                                                      closestPoint,
        //                                                                                      extraCorsa + diaFresa, true,
        //                                                                                      false);

        //                                profileStartPoint = newStartPoint;

        //                                if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
        //                                {
        //                                    throw new NotImplementedException("Punto Dentro Profilo In Ogni Caso..");
        //                                    // todo : gestire se punto trovato si trova cmq sempre dentro il polygono.
        //                                }

        //                            }

        //                        moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
        //                        moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X,
        //                                                   profileStartPoint.Y, null);
        //                        moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
        //                    }
        //                    else
        //                    {
        //                        moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, profileStartPoint.X,
        //                                                   profileStartPoint.Y,
        //                                                   null);
        //                    }
        //                }

        //                CncCompensationState cncCompensationState = CncCompensationState.NoChange;

        //                if (isCncCompensated == 1)
        //                    cncCompensationState = CncCompensationState.G41;

        //                else if (isCncCompensated == 2)
        //                    cncCompensationState = CncCompensationState.G42;


        //                foreach (var entity2D in source.Source)
        //                {
        //                    if (entity2D is Line2D)
        //                    {
        //                        var line = entity2D as Line2D;

        //                        moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null, null, cncCompensationState);

        //                        lastMovePoint = new Point2D(line.End);
        //                    }

        //                    else if (entity2D is Arc2D)
        //                    {
        //                        // implementare archi 

        //                        var arc = entity2D as Arc2D;

        //                        var lastPoint = moveActionCollection.GetLastPoint();

        //                        var arcStart = arc.Start;

        //                        if (lastPoint == null || !lastPoint.Equals(arcStart))
        //                        {
        //                            moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, arc.Start.X, arc.Start.Y,
        //                                                    zCurrent, null, cncCompensationState);
        //                        }

        //                        moveActionCollection.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, zCurrent, arc.Radius,
        //                                                arc.ClockWise, arc.Center, null, cncCompensationState);

        //                        lastMovePoint = new Point2D(arc.End);
        //                    }
        //                }

        //                Point2D profileDistacPoint;

        //                //profileDistacPoint = source.ToolPathStartPoint ?? source.Source.Last().GetLastPnt();

        //                //if (isCncCompensated == 1 || isCncCompensated == 2)
        //                //    cncCompensationState = CncCompensationState.G40;

        //                //moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, profileDistacPoint.X, profileDistacPoint.Y,
        //                //                                    zCurrent, null, cncCompensationState);


        //            }

        //            // Mi sposto in z sicurezza
        //            moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //}




        ///con metodo sopra non prende bene punto attacco.. ricontrollare
        ///Backup 13/09/2011
        private static void WriteContourn(MoveActionCollection moveActionCollection, List<Profile2D> offset, double diaFresa, double zSicurezza, double zIniziale, double zFinale, double profPassata, double extraCorsa, Profile2D polygon, int isCncCompensated = 0)
        {
                if (offset.Count <= 0)
                    return;

                var zCurrent = zIniziale;

                CncCompensationState cncCompensationState = CncCompensationState.NoChange;


                while (zCurrent > zFinale)
                {
                    zCurrent -= profPassata;
                    if (zCurrent < zFinale)
                        zCurrent = zFinale;


                    Point2D lastMovePoint = null;

                    for (var i = 0; i < offset.Count; i++)
                    {
                        var element = offset[i];

                        // Faccio parse per risolvere archi
                        var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

                        //var 
                        var source = parsedCountor;
                        //  source = element;

                        /*
                         * se punto iniziale si trova dentro profilo fare :
                         * - trovare nearest point da punto attacco e profilo grezzo  
                         * - fare estensione segmento punto iniziale - punto vicino 
                         * - punto attacco è punto esteso.
                         */

                        /*
                         * Per correttezza il profileStartPoint deve essere definito quando uso la compensazione utensile..
                         */

                        Point2D profileStartPoint;
                        // se è l'ultimo profilo della serie e devo usare la compensazione utensile il profile start point deve essere definito..



                        if (isCncCompensated == 1)
                            cncCompensationState = CncCompensationState.G41;

                        else if (isCncCompensated == 2)
                            cncCompensationState = CncCompensationState.G42;

                        if (i == offset.Count - 1 && isCncCompensated != 0)
                        {
                            profileStartPoint = source.ToolPathStartPoint;

                            if (profileStartPoint == null)
                                profileStartPoint = source.Source.First().GetFirstPnt();

                        }
                        else
                        {
                            profileStartPoint = source.ToolPathStartPoint ?? source.Source.First().GetFirstPnt();
                        }

                        if (lastMovePoint == null)
                        {

                            //if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
                            //{

                            //}

                            moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X,
                                                       profileStartPoint.Y, null, null, cncCompensationState);
                            moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
                            moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
                        }
                        else
                        {
                            var distance = GeometryHelper.Distance(profileStartPoint, lastMovePoint);

                            if (distance > diaFresa)
                            {
                                if (polygon != null)
                                    if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
                                    {
                                        var closestPoint = GeometryHelper.GetClosestPoint(polygon.GetPointListP2(),
                                                                                          profileStartPoint);

                                        var temp = new Point2D(profileStartPoint);

                                        var newStartPoint = GeometryHelper.GetPointAtDistance(profileStartPoint,
                                                                                              closestPoint,
                                                                                              extraCorsa + diaFresa, true,
                                                                                              false);

                                        profileStartPoint = newStartPoint;

                                        if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
                                        {
                                            throw new NotImplementedException("Punto Dentro Profilo In Ogni Caso..");
                                            // todo : gestire se punto trovato si trova cmq sempre dentro il polygono.
                                        }

                                    }

                                moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
                                moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X,
                                                           profileStartPoint.Y, null, null, cncCompensationState);
                                moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
                            }
                            else
                            {
                                moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, profileStartPoint.X,
                                                           profileStartPoint.Y,
                                                           null, null, cncCompensationState);
                            }
                        }



                        if (source.Source[0] is Line2D)
                        {
                            var firstMove = ((Line2D)source.Source[0]).Start;

                            moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null, null, cncCompensationState);
                        }
                        else if (source.Source[0] is Arc2D)
                        {

                            var firstMove = ((Arc2D)source.Source[0]).Start;

                            moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null, null, cncCompensationState);

                            //if (lastMovePoint != null)
                            //{
                            //    /* Qui guardo che se lo spostamento che devo effettuare è maggiore del raggio fresa , 
                            //     * 1)  mi alzo 
                            //     * 2) mi sposto in rapido
                            //     * 3) mi pianto
                            //     */

                            //    var distance = GeometryHelper.Distance(firstMove, lastMovePoint);

                            //    if (distance > diaFresa)
                            //    {
                            //        moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
                            //        moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, firstMove.X, firstMove.Y,
                            //                                   null);
                            //    }
                            //    else

                            //        moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y,
                            //                                   null);
                            //}
                        }

                        foreach (var entity2D in source.Source)
                        {
                            if (entity2D is Line2D)
                            {
                                var line = entity2D as Line2D;

                                moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);

                                lastMovePoint = new Point2D(line.End);
                            }

                            else if (entity2D is Arc2D)
                            {
                                // implementare archi 

                                var arc = entity2D as Arc2D;

                                var lastPoint = moveActionCollection.GetLastPoint();

                                var arcStart = arc.Start;

                                if (lastPoint == null || !lastPoint.Equals(arcStart))
                                {
                                    moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, arc.Start.X, arc.Start.Y,
                                                            zCurrent);
                                }

                                moveActionCollection.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, zCurrent, arc.Radius,
                                                        arc.ClockWise, arc.Center);

                                lastMovePoint = new Point2D(arc.End);
                            }
                        }
                        var endPnt = source.ToolPathEndPoint;

                        if (endPnt != null)
                        {
                            if (cncCompensationState == CncCompensationState.G41 || cncCompensationState == CncCompensationState.G42)

                                moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, endPnt.X,
                                                               endPnt.Y, null, null, CncCompensationState.G40);
                        }
                    }


                    // Mi sposto in z sicurezza
                    moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);

                    if (cncCompensationState == CncCompensationState.G41 || cncCompensationState == CncCompensationState.G42)
                        moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, null,
                                                                 null, null, null, CncCompensationState.G40);
                }

        }



        #endregion

        /// <summary>
        /// Metodo comune per la creazione della smussatura su linea
        /// Usata nell'operazioni:
        /// - Scanalatura Linea
        /// - Fresatura Lato
        /// </summary>
        /// <param name="programPhase"></param>
        /// <param name="line"></param>
        /// <param name="profondita"></param>
        /// <param name="profPassata"></param>
        /// <param name="diaFresa"></param>
        /// <param name="secureZ"></param>
        /// <param name="extraCorsa"></param>
        /// <param name="zIniziale"></param>
        /// <param name="isExtern"></param>
        /// <param name="rotationMatrix"></param>
        internal static void ProcessSingleLineFinish(MoveActionCollection programPhase, Line2D line, double profondita, double profPassata,
                                          double diaFresa, double secureZ, double extraCorsa, double zIniziale, bool isExtern, int compensationMode, Matrix3D rotationMatrix)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { profondita, diaFresa }) ||
                CheckValueHelper.GreatherThan(new[]
                                                  {
                                                      new KeyValuePair<double, double>(secureZ, zIniziale),
                                                  })

                ) return;

            // se c'è compensazione ... a parte che potrei fare tutto con stesso metodo..

            var raggio = diaFresa / 2;

            var offset = raggio;

            if (compensationMode != 0)
                offset = 0;

            var lineParallel = Geometry.GeometryHelper.GetParallel(line, offset, isExtern);

            var extensionDistance = extraCorsa + raggio;

            var extIniPoint = Geometry.GeometryHelper.GetPointAtDistance(lineParallel.End, lineParallel.Start, -extensionDistance, false);

            var extEndPoint = Geometry.GeometryHelper.GetPointAtDistance(lineParallel.Start, lineParallel.End, -extensionDistance, false);

            /*
             * 1) Estendo linea per attacco .
             * 2) Muovo xy
             * 3) Z sicurezza - rapido
             * 4) Z Lavoro - secureFeed
             * 5) Smusso Linea 
             * 6) Alzo z sicurezza 
             * 7) Fine
             */

            var endZ = zIniziale - profondita;

            var currentZ = zIniziale;

            var comp = CncCompensationState.NoChange;

            if (compensationMode == 1)
                comp = CncCompensationState.G41;

            if (compensationMode == 2)
                comp = CncCompensationState.G42;


            while (currentZ > endZ)
            {
                currentZ -= profPassata;
                if (currentZ < endZ)
                    currentZ = endZ;

                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, extIniPoint.X, extIniPoint.Y, null,
                                           rotationMatrix, null, comp);

                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);

                programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, currentZ);

                programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, extEndPoint.X, extEndPoint.Y, null,
                                           rotationMatrix);

                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);

                if (comp == CncCompensationState.G41 || comp == CncCompensationState.G42)
                    programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, null, null, null, null, CncCompensationState.G40);

            }



        }

        public static void FresaturaScanalaturaChiusaProgram(MoveActionCollection programPhase, Profile2D profile2D, double profondita, double larghezza, double zSicurezza, double zIniziale, double diaFresa, double larghPassata, double profPassata, double extraCorsa, double sovrametalloXy, bool isCncCompensated)
        {
            /*
             * teoria.
             * - Prendo profilo 
             *  - Se valido faccio offset negativo del raggio della fresa
             * - Poi faccio offset della larghezza di passata fino a che il metodo non mi restituisce più niente. ( ho raggiunto il massimo ) 
             */

            /*
             * Controllo Valori
             */
            if (CheckValueHelper.GreatherOrEqualZero(new[] { extraCorsa }) ||
                CheckValueHelper.GreatherThanZero(new[] { profondita, larghPassata, diaFresa, profPassata, larghezza }) ||
                CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(zSicurezza, zIniziale),
                                                  new KeyValuePair<double, double>(diaFresa, larghPassata),
                                              })
                ) return;

            if (profile2D == null) return;

            var raggioFresa = diaFresa / 2;

            var profile = profile2D;
            /*
             * todo : implementare anche sovrametallo di finitura 
             * Come contorni da lavorare gli passo profilo originale
             * 
             * poi faccio offset partendo da 0 incremento larghPassata fino ad arrivare a metaLarghezza - raggioFresa
             * 
             * faccio prima offset positivo poi negativo 
             * quello positivo in senso ccw e quello positivo in cw.
             */

            var offsetCountorns = new List<Profile2D>();

            offsetCountorns.Add(profile);

            var offsetCurrent = 0.0d;

            var offsetEnd = (larghezza / 2) - raggioFresa - sovrametalloXy;

            while (offsetCurrent < offsetEnd)
            {
                offsetCurrent += larghPassata;

                if (offsetCurrent > offsetEnd)
                    offsetCurrent = offsetEnd;

                offsetCountorns.AddRange(profile.Offset(offsetCurrent, true));

                offsetCountorns.AddRange(profile.Offset(-offsetCurrent, false));
            }

            /*
             * Devo spostarmi in xy del primo punto 
             */
            var attacPath = offsetCountorns.FirstOrDefault();

            if (attacPath == null)
                return;

            var attacPoint = attacPath.Source.FirstOrDefault();

            if (attacPoint == null)
                throw new Exception();

            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, attacPoint.GetFirstPnt().X, attacPoint.GetFirstPnt().Y, null);

            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);


            var zFinale = zIniziale - profondita;

            WriteContourn(programPhase, offsetCountorns, diaFresa, zSicurezza, zIniziale, zFinale, profPassata, extraCorsa, null);

        }

        #region Helical Interpolation

        /*
         * Ora faccio una serie di metodi per interpolazione circolare.
         * Verranno usati per la filettatura 
         * fresata o per fresatura generiche , esterne / interne.
         */


        /// <summary>
        /// Programma per sgrossatura a interpolazione
        /// In ogni caso mi serve sapere il centro in quanto non riesco a muovermi in incrementale .
        /// </summary>
        /// <param name="moveActionCollection"></param>
        /// <param name="startZ"></param>
        /// <param name="endZ"></param>
        /// <param name="secureZ"></param>
        /// <param name="center"></param>
        /// <param name="diameterEnd"></param>
        /// <param name="millDiameter"></param>
        /// <param name="profPassata"></param>
        /// <param name="larghezzaPassata"></param>
        internal static void GetRoughHelicalInterpolation(MoveActionCollection moveActionCollection, double startZ, double endZ, double secureZ, Point2D center, double diameterEnd, double millDiameter, double profPassata, double larghezzaPassata)
        {
            var deltaD = diameterEnd - millDiameter;

            var radiusHelixFinal = deltaD / 2;

            if (radiusHelixFinal <= 0) return;
            if (startZ <= endZ) return;

            var radiusHelixCurrent = 0.0d;

            while (radiusHelixCurrent < radiusHelixFinal)
            {
                radiusHelixCurrent += larghezzaPassata;

                if (radiusHelixCurrent > radiusHelixFinal)
                    radiusHelixCurrent = radiusHelixFinal;

                HelicalInternInterpolation(moveActionCollection, startZ, endZ, secureZ, profPassata, center,
                                     radiusHelixCurrent, true, false, false);

            }
        }

        internal static void GetInternThreadDx(MoveActionCollection moveActionCollection, double workUp, double workDown, double secureZ, Point2D center, double pitch, double helixRadius, bool cncComp)
        {
            HelicalInternInterpolation(moveActionCollection, workDown, workUp, secureZ, pitch, center,
                                              helixRadius, false, false, cncComp);
        }

        internal static void GetInternThreadSx(MoveActionCollection moveActionCollection, double workUp, double workDown, double secureZ, Point2D center, double pitch, double helixRadius, bool cncComp)
        {
            HelicalInternInterpolation(moveActionCollection, workUp, workDown, secureZ, pitch, center,
                                              helixRadius, false, false, cncComp);
        }

        internal static void GetExternThreadDx(MoveActionCollection moveActionCollection, double workUp, double workDown, double secureZ, Point2D center, double pitch, double helixRadius, bool cncComp, double extraCorsa)
        {
            var attacPoint = GeometryHelper.GetCoordinate(0, extraCorsa + helixRadius, center);

            HelicalInternInterpolation(moveActionCollection, workUp, workDown, secureZ, pitch, center,
                                              helixRadius, false, false, cncComp, attacPoint);

        }

        internal static void GetExternThreadSx(MoveActionCollection moveActionCollection, double workUp, double workDown, double secureZ, Point2D center, double pitch, double helixRadius, bool cncComp, double extraCorsa)
        {
            var attacPoint = GeometryHelper.GetCoordinate(0, extraCorsa + helixRadius, center);

            HelicalInternInterpolation(moveActionCollection, workDown, workUp, secureZ, pitch, center,
                                              helixRadius, false, false, cncComp, attacPoint);

        }

        private static void HelicalInternInterpolation(MoveActionCollection moveActionCollection, double startZ, double endZ, double secureZ, double helixStep, Point2D center, double helixRadius, bool lastCircleOnEnd, bool clockWise, bool cncCompensationActivated, Point2D attacPnt = null, bool isExtern = false)
        {
            /*
             * se start è minore 
             */
            var startMaggiore = startZ > endZ;

            var zCurrent = startZ;

            var continueCycle = startMaggiore ? zCurrent > endZ : zCurrent < endZ;

            // Se punto attacco è diverso da null, vado a punto attacco
            if (attacPnt != null)
            {
                moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, attacPnt.X, attacPnt.Y, null);
            }
            else
            {
                // Vado a Centro Foro in rapido
                moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, center.X, center.Y, null);
            }


            // Vado a Z secure
            moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);

            // Vado a Z Current 
            moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);

            var xArrivo = center.X + helixRadius;

            var cncComp = CncCompensationState.NoChange;

            if (cncCompensationActivated && isExtern)
                cncComp = CncCompensationState.G42;

            else if (cncCompensationActivated)
                cncComp = CncCompensationState.G41;

            // vado verso lato 
            moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xArrivo, null, null, null, cncComp);

            while (continueCycle)
            {
                zCurrent = startMaggiore ? zCurrent - helixStep : zCurrent + helixStep;

                if (startMaggiore)
                {
                    if (zCurrent < endZ)
                        zCurrent = endZ;
                }
                else
                {
                    if (zCurrent > endZ)
                        zCurrent = endZ;
                }

                moveActionCollection.AddArcMove(AxisAbilited.Xyz, null, null, zCurrent, helixRadius, clockWise, center);

                continueCycle = startMaggiore ? zCurrent > endZ : zCurrent < endZ;

            }

            // Faccio un movimento circolare , in modo da tenere il fondo piatto
            if (lastCircleOnEnd)
                moveActionCollection.AddArcMove(AxisAbilited.Xy, null, null, zCurrent, helixRadius, clockWise, center);

            cncComp = CncCompensationState.G40;

            // Se punto attacco è diverso da null, vado a punto attacco
            if (attacPnt != null)
            {
                moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, attacPnt.X, attacPnt.Y, null, null, cncComp);
            }
            else
            {
                // Vado a Centro Foro in rapido
                moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Xy, center.X, center.Y, null, null, cncComp);
            }


            // Vado a Z Sicurezza 
            moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, secureZ);
        }

        //private static void GetHelicalInterpolation(MoveActionCollection moveActionCollection, double startZ, double endZ, Point2D attacPoint, double secureZ, bool clockWiseDirection, double helixRadius, double helixStep, bool setCncCompensation)
        //{
        //    if (CheckValueHelper.GreatherThanZero(new[] { helixStep, helixRadius }) ||
        //        CheckValueHelper.GreatherThan(new[]
        //                                      {
        //                                          new KeyValuePair<double, double>(secureZ, Math.Max(startZ, endZ)),
        //                                      })) return;

        //    /*
        //               * Considerando filettatura interna dx . eseguo seguente iter :
        //               * 
        //               *  1) Mi sposto centro cerchio
        //               *  2) Mi Abbasso a z Iniziale 
        //               *  3) Vado a diametro ( con attacco ad arco , per ora dritto. ) 
        //               *          Questo diametro sara DiametroDiArrivo = DiametroForatura - DiametroFresa
        //               *  4) Salgo in interpolazione fino a raggiungere zFinale
        //               *  5) Torno punto centrale foro
        //               *  6) Torno z Sicurezza
        //               *  
        //               */

        //    // 1) posizionamento XY sopra foro
        //    moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, pnt.X, pnt.Y, null);

        //    moveActionCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, secureZ);

        //    // 2) Vado a Z Iniziale
        //    var zInizio = zMin;
        //    var zFinale = zMax;

        //    //// tengon presente anche compensazione raggio, in questo il diametro sarà quello della filettatura
        //    //var diametroFilettatura = 30;
        //    //var diametroFresa = 10;
        //    var radius = diameter / 2; // (diametro foro - diametrofresa) /2



        //    moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zInizio);

        //    var currentZ = zInizio;

        //    // distanza asse X fra centroForo e diametroArrivo 
        //    var increX = -radius;


        //    // 3) Vado a diametro // todo : farlo in interpolazione

        //    var xArrivo = pnt.X + radius;
        //    moveActionCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, xArrivo, null, null);

        //    while (currentZ <= zFinale)
        //    {
        //        currentZ += passo;

        //        // l'interpolazione , inserisco incrementale del per trovare centro del cerchio

        //        // vedere come è possibile aggiungere incrementale per il centro del arco , dove ho messo ora è incrementale 
        //        // per il punto finale.
        //        moveActionCollection.AddArcMove(AxisAbilited.Xyz, null, null, currentZ, radius, true, pnt);
        //    }

        //    // 5) Torno punto centrale
        //    moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Xy, pnt.X, pnt.Y, null);

        //    // 6) Torno Z Sicurezza
        //    moveActionCollection.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, secureZ);

        //}

        #endregion


    }
}
//06/09/2011 -- codice per scrivere il profilo nella fresatura esterna.
//Point2D lastMovePoint = null;

//for (var i = 0; i < offsetCounterStockTrimmed.Count; i++)
//{
//    var element = offsetCounterStockTrimmed[i];

//    // Faccio parse per risolvere archi
//    var parsedCountor = Profile2D.ParseArcIn2DProfile(element);

//    //var 
//    var source = parsedCountor;
// //  source = element;

//    /*
//     * se punto iniziale si trova dentro profilo fare :
//     * - trovare nearest point da punto attacco e profilo grezzo  
//     * - fare estensione segmento punto iniziale - punto vicino 
//     * - punto attacco è punto esteso.
//     */
//    var profileStartPoint = source.ToolPathStartPoint ?? source.Source.First().GetFirstPnt();


//    if (lastMovePoint == null)
//    {

//        //if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//        //{

//        //}

//        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y, null);
//        programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//        programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
//    }
//    else
//    {
//        var distance = GeometryHelper.Distance(profileStartPoint, lastMovePoint);

//        if (distance > diaFresa)
//        {
//            if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//            {
//                var closestPoint = GeometryHelper.GetClosestPoint(polygon.GetPointListP2(), profileStartPoint);

//                var temp = new Point2D(profileStartPoint);

//                var newStartPoint = GeometryHelper.GetPointAtDistance(profileStartPoint, closestPoint,
//                                                                      extraCorsa + diaFresa, true, false);

//                profileStartPoint = newStartPoint;

//                if (GeometryHelper.PointInPolygon(polygon.GetPointListP2(), profileStartPoint))
//                {
//                    throw new NotImplementedException();  // todo : gestire se punto trovato si trova cmq sempre dentro il polygono.
//                }

//            }

//            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//            programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y, null);
//            programPhase.AddLinearMove(MoveType.SecureRapidFeed, AxisAbilited.Z, null, null, zCurrent);
//        }
//        else
//        {
//            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, profileStartPoint.X, profileStartPoint.Y,
//                                       null);
//        }
//    }
//    if (source.Source[0] is Line2D)
//    {
//        var firstMove = ((Line2D)source.Source[0]).Start;

//        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//    }
//    else if (source.Source[0] is Arc2D)
//    {

//        var firstMove = ((Arc2D)source.Source[0]).Start;

//        if (lastMovePoint != null)
//        {
//            /* Qui guardo che se lo spostamento che devo effettuare è maggiore del raggio fresa , 
//             * 1)  mi alzo 
//             * 2) mi sposto in rapido
//             * 3) mi pianto
//             */

//            var distance = GeometryHelper.Distance(firstMove, lastMovePoint);

//            if (distance > diaFresa)
//            {
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, firstMove.X, firstMove.Y,
//                                           null);
//            }
//            else

//                programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y,
//                                           null);
//        }
//    }

//    foreach (var entity2D in source.Source)
//    {
//        if (entity2D is Line2D)
//        {
//            var line = entity2D as Line2D;

//            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);

//            lastMovePoint = new Point2D(line.End);
//        }

//        else if (entity2D is Arc2D)
//        {
//            // implementare archi 

//            var arc = entity2D as Arc2D;

//            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, arc.Start.X, arc.Start.Y, zCurrent);

//            programPhase.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, zCurrent, arc.Radius, arc.ClockWise, arc.Center);

//            lastMovePoint = new Point2D(arc.End);
//        }
//    }

//}


// scrive profilo per sgrossatura interna
//Point2D lastMovePoint = null;

//for (var i = offsetCountorns.Count - 1; i >= 0; i--)
//{
//    var element = offsetCountorns[i];

//    var parsedCountor = Profile2D.ParseArcIn2DProfile(element);
//    //* problema con parsed arc..
//    var source = parsedCountor;
//    // var source = element;

//    if (source.Source[0] is Line2D)
//    {
//        var firstMove = ((Line2D)source.Source[0]).Start;

//        if (lastMovePoint != null)
//        {
//            /* Qui guardo che se lo spostamento che devo effettuare è maggiore del raggio fresa , 
//             * 1)  mi alzo 
//             * 2) mi sposto in rapido
//             * 3) mi pianto
//             */

//            var distance = GeometryHelper.Distance(firstMove, lastMovePoint);

//            if (distance > diaFresa)
//            {
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//            }
//            else

//                programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);


//        }
//        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);

//        // Movimento in piantata a z di lavoro..
//        programPhase.AddLinearMove(MoveType.PlungeFeed, AxisAbilited.Z, null, null, zCurrent);
//    }
//    else if (source.Source[0] is Arc2D)
//    {
//        // questo codice non l'ho controllato moltissimo
//        var firstMove = ((Arc2D)source.Source[0]).Start;

//        if (lastMovePoint != null)
//        {
//            /* Qui guardo che se lo spostamento che devo effettuare è maggiore del raggio fresa , 
//             * 1)  mi alzo 
//             * 2) mi sposto in rapido
//             * 3) mi pianto
//             */

//            var distance = GeometryHelper.Distance(firstMove, lastMovePoint);

//            if (distance > diaFresa)
//            {
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, zSicurezza);
//                programPhase.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);
//            }
//            else

//                programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);


//        }
//        programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);

//        // Movimento in piantata a z di lavoro..
//        programPhase.AddLinearMove(MoveType.PlungeFeed, AxisAbilited.Z, null, null, zCurrent);
//    }

//    foreach (var entity2D in source.Source)
//    {
//        if (entity2D is Line2D)
//        {
//            var line = entity2D as Line2D;

//            programPhase.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);

//            lastMovePoint = new Point2D(line.End.X, line.End.Y);
//        }

//        else if (entity2D is Arc2D)
//        {
//            // implementare archi 

//            var arc = entity2D as Arc2D;
//            programPhase.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, null, arc.Radius, arc.ClockWise, arc.Center);

//            /*
//             * todo anche arco puo essere primo elemento
//             */
//            lastMovePoint = new Point2D(arc.End.X, arc.End.Y);
//        }
//    }

//}


