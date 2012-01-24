using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.Model.ConversationalStructure.Abstraction;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura.Pattern;
using CncConvProg.Model.PathGenerator;
using CncConvProg.Model.Tool;
using CncConvProg.Model.Tool.Mill;
using CncConvProg.Model.Tool.Parametro;
using System.Windows;
using Point3D = System.Windows.Media.Media3D.Point3D;

/*
 * 
 * bug dentro xaml font parser . indice oltre limite matrice.
 */

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura
{
    [Serializable]
    public sealed class TextEngravingModel : LavorazioneFresatura, IMillLeveable
    {
        public TextEngravingModel(Guid parent)
            : base(parent)
        {
            //if (parent.Model.MeasureUnit == MeasureUnit.Millimeter)
            //{
            //    ProfonditaLavorazione = .1;
            //    SicurezzaZ = 30;
            //    RadiusCircle = 30;
            //    FontHeight = 10;

            //}
            //else
            //{
            //    ProfonditaLavorazione = .005;
            //    SicurezzaZ = 1;
            //    RadiusCircle = 1.2;

            //    FontHeight = 0.4;
            //}

            AngleStart = -180;

            AngleWidth = 180;


            TextToEngrave = "Sample Text";

            Smussatura = new Operazione(this, LavorazioniEnumOperazioni.Smussatura);
        }

        public double ProfonditaLavorazione { get; set; }


        public double InizioLavorazioneZ { get; set; }

        public string TextToEngrave { get; set; }

        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public double FontHeight { get; set; }

        public bool WriteInCircle { get; set; }

        public double RadiusCircle { get; set; }

        public double AngleStart { get; set; }

        public double AngleWidth { get; set; }


        public override List<Operazione> Operazioni
        {
            get
            {
                return new List<Operazione> { Smussatura };
            }
        }

        public Operazione Smussatura { get; set; }

        //internal override Utensile CreateTool(LavorazioniEnumOperazioni enumOperationType)
        //{
        //    var unit = Singleton.Instance.MeasureUnit;

        //    return FresaturaHelper.CreateTool((LavorazioniEnumOperazioni)enumOperationType, unit);
        //}

        //internal override List<Utensile> GetCompatibleTools(LavorazioniEnumOperazioni operationType, MagazzinoUtensile magazzino)
        //{
        //    var unit = Singleton.Instance.MeasureUnit;

        //    return magazzino.GetTools<FresaCandela>(unit).Cast<Utensile>().ToList();
        //}

        protected override void CreateSpecificProgram(ProgramPhase programPhase, Operazione operazione)
        {
            // var fresa = operazione.Utensile as IDiametrable;

            //var parametro = operazione.Utensile.ParametroUtensile as ParametroFresaCandela;

            //if (fresa == null || parametro == null)
            //    throw new NullReferenceException();

            //var diaFresa = fresa.Diametro;

            //var larghezzaPassata = parametro.GetLarghezzaPassata();

            //var profPassata = parametro.GetProfonditaPassata();

            var moveCollection = new MoveActionCollection();


            switch ((LavorazioniEnumOperazioni)operazione.OperationType)
            {
                default:
                    {
                        EngravingText(moveCollection, GetTextProfiles(), ProfonditaLavorazione, SicurezzaZ, InizioLavorazioneZ);
                    } break;

            }
            var mm = base.GetFinalProgram(moveCollection);

            foreach (var variable in mm)
            {
                programPhase.AddMoveAction(variable);
            }
        }

        //internal override ProgramPhase GetOperationProgram(Operazione operazione)
        //{
        //    var program = new ProgramPhase(SicurezzaZ);

        //    var toolChange = new ChangeToolAction(program, operazione);

        //    program.ActiveAsseC(true);


        //    var fresa = operazione.Utensile as FresaCandela;

        //    var parametro = operazione.Utensile.ParametroUtensile as ParametroFresaCandela;

        //    if (fresa == null || parametro == null)
        //        throw new NullReferenceException();

        //    var diaFresa = fresa.DiametroFresa;

        //    var larghezzaPassata = parametro.GetLarghezzaPassata();

        //    var profPassata = parametro.GetProfonditaPassata();

        //    var feed = parametro.GetFeed(FeedType.ASync);

        //    var plungeFeed = parametro.AvanzamentoAsincronoPiantata.Value.Value;

        //    var secureFeed = 1;

        //    var extraCorsa = 1;

        //    var moveCollection = new MoveActionCollection();

        //    if (feed <= 0)
        //        return null;

        //    var feedDictionary = new Dictionary<MoveType, double>
        //                             {
        //                                 {MoveType.Rapid, 10000},
        //                                 {MoveType.SecureRapidFeed, secureFeed},
        //                                 {MoveType.Work, feed},
        //                                 {MoveType.Cw, feed},
        //                                 {MoveType.Ccw, feed},
        //                                 {MoveType.PlungeFeed, plungeFeed}
        //                             };

        //    program.SetFeedDictionary(feedDictionary);


        //    switch ((LavorazioniEnumOperazioni)operazione.OperationType)
        //    {
        //        default:
        //            {
        //                EngravingText(moveCollection, GetTextProfiles(), ProfonditaLavorazione, SicurezzaZ, InizioLavorazioneZ);
        //            } break;

        //    }
        //    var mm = base.GetFinalProgram(moveCollection);

        //    foreach (var variable in mm)
        //    {
        //        program.AddMoveAction(variable);
        //    }

        //    program.ActiveAsseC(false);
        //    return program;
        //}

        private static void EngravingText(MoveActionCollection moveCollection, IEnumerable<Profile2D> profiles, double ProfonditaLavorazione, double SicurezzaZ, double InizioLavorazioneZ)
        {
            if (CheckValueHelper.GreatherThan(new[]
                                              {
                                                  new KeyValuePair<double, double>(SicurezzaZ , InizioLavorazioneZ - ProfonditaLavorazione),
                                              })
                    ) return;

            if (profiles == null || profiles.Count() == 0) return;

            var zLavoro = InizioLavorazioneZ - ProfonditaLavorazione;

            var firstElement = profiles.First().Source.FirstOrDefault();

            if (firstElement == null) return;

            var attacPoint = firstElement.GetFirstPnt();

            if (attacPoint == null)
                return;

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, attacPoint.X, attacPoint.Y, null);


            foreach (var profile2D in profiles)
            {
                var element = profile2D.Source;
                var p = Geometry.Entity.Profile2D.ParseArcIn2DProfile(profile2D);

                var source = p.Source;

                // arc parsingg

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, SicurezzaZ);

                if (source[0] is Line2D)
                {
                    var firstMove = ((Line2D)source[0]).Start;

                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xy, firstMove.X, firstMove.Y, null);

                    moveCollection.AddLinearMove(MoveType.PlungeFeed, AxisAbilited.Z, null, null, zLavoro);

                }

                foreach (var entity2D in source)
                {
                    if (entity2D is Line2D)
                    {
                        var line = entity2D as Line2D;

                        moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, line.End.X, line.End.Y, null);
                    }

                    else if (entity2D is Arc2D)
                    {
                        // implementare archi 

                        var arc = entity2D as Arc2D;

                        moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xy, arc.Start.X, arc.Start.Y, null);

                        moveCollection.AddArcMove(AxisAbilited.Xy, arc.End.X, arc.End.Y, null, arc.Radius, arc.ClockWise, arc.Center);
                    }
                }

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Z, null, null, SicurezzaZ);

            }

        }

        public override string Descrizione
        {
            get { return "Text Engraving"; }
        }

        private IEnumerable<Profile2D> GetTextProfiles()
        {
            /*
             * se scritta in cerchio faccio lettera per volta.
             */


            if (WriteInCircle)
            {
                var charCount = TextToEngrave.Count();

                var radianAngleWidth = GeometryHelper.DegreeToRadian(AngleWidth);

                var angleIncre = radianAngleWidth / charCount;

                /*
                 * scrivo in senso orario , quindi devo decrementare angolo
                 * 
                 * devo fare una lettera alla volta con angolo diverso
                 * 
                 */
                var profiles = new List<Profile2D>();

                var radianAngleStart = GeometryHelper.DegreeToRadian(AngleStart);

                //if (radianAngleStart < 0)
                //    radianAngleStart += 2 * Math.PI;

                for (int i = 0; i < charCount; i++)
                {
                    var letterProfiles = GetProfiles(TextToEngrave[i].ToString(), FontHeight);

                    if (letterProfiles == null)
                        continue;

                    var angleCurrent = -((angleIncre * i) + radianAngleStart);

                    var rotationMatrix = new Matrix3D();

                    rotationMatrix.TranslatePrepend(new Vector3D(0, CenterY + RadiusCircle, 0));

                    rotationMatrix.RotateAt(new Quaternion(new Vector3D(0, 0, 1), GeometryHelper.RadianToDegree(angleCurrent)), new Point3D(CenterX, CenterY, 0));

                    foreach (var profile2D in letterProfiles)
                    {
                        profile2D.Multiply(rotationMatrix);
                    }

                    profiles.AddRange(letterProfiles);
                }

                return profiles;

            }
            else
            {

                var profiles = GetProfiles(TextToEngrave, FontHeight);

                var matrix = new Matrix3D();

                matrix.Translate(new Vector3D(CenterX, CenterY, 0));

                foreach (var profile2D in profiles)
                {
                    profile2D.Multiply(matrix);
                }

                return profiles;
            }

        }
        /// <summary>
        /// Ritorna anteprima della lavorazione
        /// </summary>
        /// <returns></returns>
        protected override List<IEntity3D> GetFinalPreview()
        {
            var p = GetTextProfiles();

            var preview = new List<IEntity3D>();

            foreach (var profile2D in p)
            {
                profile2D.SetPlotStyle();

                var pp = Entity3DHelper.Get3DProfile(profile2D.Source).ToList();

                preview.AddRange(pp);
            }

            return preview;
        }

        private static IEnumerable<Profile2D> GetProfiles(string str, double fontHeight)
        {
            //pack://application:,,,/Images/Work/textEngraving.png
            // Create the initial formatted text string.
            var formattedText = new FormattedText(
                str,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                  new Typeface("Verdena"),
                fontHeight,
                Brushes.Black);

            //formattedText.SetFontFamily("Times New Roman");
            //formattedText.SetFontFamily("Machine Tool SanSerif");

            var f = formattedText.BuildGeometry(new Point());
            //var ll = formattedText.BuildHighlightGeometry(new Point());
            //var typeFace = new Typeface("1CamBam_Stick_1");
            //formattedText.SetFontTypeface(typeFace); 

            var p1 = f.GetFlattenedPathGeometry(0.001, ToleranceType.Relative);

            //var p2 = f.GetOutlinedPathGeometry();

            var profiles = XamlPathDataManager.ElaboratePathData(p1, new Point2D(0, fontHeight / 2));

            /*
             * ora devo spostarlo..
             */



            /*
             * devo ruotarlo di 180 
             */
            /*
             * per fare testo su cerchio , divido il testo per carattere e faccio come con i fori..
             * 
             */

            return profiles;

        }

        static class XamlPathDataManager
        {

            internal static IEnumerable<Profile2D> ElaboratePathData(PathGeometry p1, Point2D startPoint)
            {
                /*
                 * La matrice di rotazione serve solamente a correggere il comportamento strano ( testo rovesciato) ottenuto dalla trasformazione in geometria del testo.
                 */
                var matrix = new Matrix3D();

                matrix.RotateAt(new Quaternion(new Vector3D(1, 0, 0), 180),
                                        new System.Windows.Media.Media3D.Point3D(0, startPoint.Y, 0));



                var profiles = new List<Profile2D>();

                foreach (var figure in p1.Figures)
                {
                    var profile = new Profile2D();

                    profiles.Add(profile);

                    var d = figure.Segments;

                    foreach (var l in d)
                    {
                        if (l is PolyLineSegment)
                        {
                            var pl = l as PolyLineSegment;
                            var pts = pl.Points;

                            foreach (var pnt in pts)
                            {
                                profile.AddPnt(new Point2D(pnt.X, pnt.Y), matrix);
                            }

                            if (pts.Count > 0)
                                profile.AddPnt(new Point2D(pts[0].X, pts[0].Y), matrix);

                            continue;
                        }
                        else if (l is PolyBezierSegment)
                        {
                            var pl = l as PolyBezierSegment;
                            var pts = pl.Points;

                            foreach (var pnt in pts)
                            {
                                profile.AddPnt(new Point2D(pnt.X, pnt.Y), matrix);
                            }

                            //if (pts.Count > 0)
                            //    profile.AddPnt(new Point2D(pts[0].X, pts[0].Y));

                            continue;
                        }

                        else if (l is LineSegment)
                        {
                            var pl = l as LineSegment;

                            profile.AddPnt(new Point2D(pl.Point.X, pl.Point.Y), matrix);

                            continue;
                        }

                        else if (l is BezierSegment)
                        {
                            var pl = l as BezierSegment;

                            /*
                             * todo , calcolare bezier da 3 point
                             */
                            profile.AddPnt(new Point2D(pl.Point1.X, pl.Point1.Y), matrix);
                            profile.AddPnt(new Point2D(pl.Point2.X, pl.Point3.Y), matrix);
                            profile.AddPnt(new Point2D(pl.Point2.X, pl.Point3.Y), matrix);

                            continue;
                        }

                        //if (!(l is PolyLineSegment))
                        //{
                        Debug.Fail("XamlDataManager.ElaboratePathData");
                        throw new Exception("XamlDataManager.ElaboratePathData");
                        //}
                    }
                }
                return profiles;
            }





        }
    }
}

//private static List<Profile2D> GetProfiles(string str)
//       {
//           // Create the initial formatted text string.
//           var formattedText = new FormattedText(
//               str,
//               CultureInfo.GetCultureInfo("en-us"),
//               FlowDirection.LeftToRight,
//               new Typeface("Verdana"),
//               32,
//               Brushes.Black);

//           // Use an Italic font style beginning at the 28th character and continuing for 28 characters.

//          // formattedText.SetFontStyle(FontStyles.Italic);

//           // Draw the formatted text string to the DrawingContext of the control.

//           var f = formattedText.BuildGeometry(new Point());

//           var p1 = f.GetFlattenedPathGeometry();

//           var p2 = f.GetOutlinedPathGeometry();

//           var profiles = XamlPathDataManager.ElaboratePathData(p1);

//           return profiles;

//       }