using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CncConvProg.Geometry;
using CncConvProg.Geometry.Entity;
using CncConvProg.ViewModel.Dialog;
using HelixToolkit;
using Petzold.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;

namespace GraphicViewer
{
    /// <summary>
    /// Interaction logic for GraphicViewer.xaml
    /// </summary>
    public partial class Viewer3D : UserControl
    {
        public Viewer3D()
        {
            InitializeComponent();

            DataContextChanged += Viewer3D_DataContextChanged;

            //if(Tag.ToString() == "Z")
            //{
            //    return;
            //}

            // Setta to XY
            Vector3D faceNormal = new Vector3D(0, 0, 1);
            Vector3D faceUp = new Vector3D(0, 1, 0);

            var camera = view.Viewport.Camera as ProjectionCamera;
            Point3D target = camera.Position + camera.LookDirection;
            double dist = camera.LookDirection.Length;

            Vector3D lookdir = -faceNormal;
            lookdir.Normalize();
            lookdir = lookdir * dist;

            Point3D pos = target - lookdir;
            Vector3D updir = faceUp;
            updir.Normalize();

            CameraHelper.AnimateTo(camera, pos, lookdir, updir, 500);
            //view.Orthographic = true;

            //view.Camera.NearPlaneDistance = 0;
            //view.Camera.FarPlaneDistance = 10000;

        }

        private ObservableCollection<IEntity3D> _myShapeListRef;


        void Viewer3D_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsLoaded) return;

            //var cube = DataContext as CubeMesh;
            if (DataContext is StockCube)
            {
                while (view.Children.Count > 0)
                {
                    view.Remove(view.Children.LastOrDefault());
                }

                var sc = DataContext as StockCube;

                var c = new CubeVisual3D { Height = sc.Height, Length = sc.Length, Width = sc.Width };

                //c.Center.Offset(sc.X, sc.Y,sc.Z);

                c.Center = new Point3D(sc.X,sc.Y,sc.Z);

                view.Add(c);

                DrawAxis(2);

                ZoomToFit();
                return;
            }
            // Prendo il riferimento alla BindingList contenuta nei ViewModel 
            _myShapeListRef = DataContext as ObservableCollection<IEntity3D>;

            if (_myShapeListRef != null)
            {
                //BackgroundWorker backgroundWorker = new BackgroundWorker();

                //backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorkerDoWork);

                //backgroundWorker.RunWorkerAsync();

                DrawScene();

            }
            else
            {
                while (view.Children.Count > 0)
                {
                    view.Remove(view.Children.LastOrDefault());
                }
            }

        }

        //void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        DrawScene();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        private double _sceneMaxWidth = 100; // dovrebbe dare indicativo ampiezza scena

        /// <summary>
        /// Disegna il contenuto del dictionary. 
        /// </summary>
        private void DrawScene()
        {
            try
            {
                while (view.Children.Count > 0)
                {
                    view.Remove(view.Children.LastOrDefault());
                }

                var bounds = Rect3D.Empty;

                foreach (var visual in _myShapeListRef)
                {
                    try
                    {
                        var b = visual.GetBoundary();
                        bounds.Union(b);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (bounds.IsEmpty ||
                     double.IsNaN(bounds.SizeX) ||
                     double.IsNaN(bounds.SizeZ) ||
                     double.IsNaN(bounds.SizeY) ||
                     double.IsNaN(bounds.X) ||
                     double.IsNaN(bounds.Y) ||
                     double.IsNaN(bounds.Z)
                     )
                    _sceneMaxWidth = 100;

                _sceneMaxWidth = Math.Max(Math.Max(bounds.SizeX, bounds.SizeY), bounds.Z);

                /*
                 * mi servirebbe boundingbox qui per sapere dimensione bb
                 */
                /*
                 * avere solamente linea 3d e arc3d , il resto non è utile qui..
                 */
                // settare dimensione scenza qui..
                foreach (IEntity3D ps in _myShapeListRef)
                {
                    if (ps.PlotStyle == EnumPlotStyle.Invisible)
                        continue;

                    // entity 2d sono obsoleti per questa operazione.
                    //if (ps is Line2D)
                    //{
                    //    PlotElement((Line2D)ps);
                    //}
                    //else if (ps is Arc2D)
                    //{
                    //    PlotElement(ps as Arc2D);
                    //}

                    if (ps is Line3D)
                    {
                        PlotElement((Line3D)ps);
                    }

                    else if (ps is Arc3D)
                    {
                        PlotElement((Arc3D)ps);
                    }

                }

                DrawAxis();

                ZoomToFit();
            }
            catch (Exception)
            {
                Debug.WriteLine("Errore su DrawScene");

            }

        }

        /// <summary>
        /// Disegna assi su scena
        /// </summary>
        private void DrawAxis(double factor = 10)
        {
            var cooDim = view.GetCooAxisDim();
            if (double.IsNaN(cooDim))
                return;

            var coo = new CoordinateSystemVisual3D { ArrowLengths = cooDim / factor };

            var g = new HelixToolkit.DefaultLightsVisual3D();
            view.Add(g);
            view.Add(coo);
        }

        /// <summary>
        /// Stampa arco 3D
        /// L'elemento deve essere già definito , ( devono essere noti centro . start. end . raggio .clockwise
        /// </summary>
        /// <param name="arc3D"></param>
        private void PlotElement(Arc3D arc3D)
        {
            // ottengo dimensione massima scena
            // se pensi che prenda troppe risorse , modificare variabile solo quando si aggiunge scena 
            //var sceneMaxWidth = view.GetCooAxisDim();

            var radius = arc3D.Radius;

            /*
             * Più questo rapporto è grande , piu il raggio può essere approsimato ( disegnato con meno segmenti) 
             */
            var rapportoScenaArco = _sceneMaxWidth / radius * 2;

            // Se diametro cerchio coincide con ampiezza scenza , per disegnare la cfr usero il numero di segmenti max
            const int arcSegmentMaxNumber = 200;
            const int arcSegmentMinNumber = 20;

            // se il risultato non è soddisfacente provare con proporzione
            var segmentUsed = arcSegmentMaxNumber / rapportoScenaArco;

            if (segmentUsed < arcSegmentMinNumber)
                segmentUsed = arcSegmentMinNumber;

            //var l = new CubeMesh();
            var arcWire = new PolyLine();

            var center = arc3D.Center;

            var startAngle = GeometryHelper.GetPositiveAngle(arc3D.Start.X - center.X, arc3D.Start.Y - center.Y);

            var endAngle = GeometryHelper.GetPositiveAngle(arc3D.End.X - center.X, arc3D.End.Y - center.Y);

            /*
             * Piccolo hack per disegnare cerchio 
             */
            {

            }
            var deltaZ = arc3D.End.Z - arc3D.Start.Z;

            var currentZ = arc3D.Start.Z;

            var incrementoAngolo = (Math.PI * 2) / segmentUsed;

            arcWire.AddPnt(new Point3D(arc3D.Start.X, arc3D.Start.Y, arc3D.Start.Z));

            //var deltaA = GeometryHelper.GetDeltaAngle(startAngle, endAngle, arc3D.ClockWise);




            /*
             * Se arco è clockwise , angolo finale è minore ,
             * 
             * si parte da angolo iniziale e si diminuisce fino a angolo finale
             */
            if (arc3D.ClockWise)
            {
                /* 
                 * Se il senso è antiorario l'angolo finale dovra essere maggiore
                 */

                if (startAngle < 0)
                {
                    startAngle += Math.PI * 2;
                    endAngle += Math.PI * 2;
                }

                if (endAngle >= startAngle)
                    endAngle -= 2 * Math.PI;

                var deltaAngle = endAngle - startAngle;

                if (deltaAngle == 0)
                {
                    /* è un cerchio completo*/
                    endAngle -= Math.PI * 2;
                    deltaAngle = endAngle - startAngle;

                }

                var numeroIncrementiAngolo = (int)Math.Ceiling(Math.Abs(deltaAngle) / incrementoAngolo);

                var incrementoZ = deltaZ / numeroIncrementiAngolo;

                // Se è clocwise angolo diminuisce 
                for (var i = startAngle; i >= endAngle; i -= incrementoAngolo)
                {
                    currentZ += incrementoZ; // incremento contralle

                    var x = (Math.Cos(i) * radius) + center.X;
                    var y = (Math.Sin(i) * radius) + center.Y;

                    arcWire.AddPnt(new Point3D(x, y, currentZ));
                }
            }
            else // Arco antiorario
            {
                /*
                 * Se arco è ccw , angolo è maggiore ,
                 * 
                 * si parte da angolo iniziale e si aumenta fino a angolo finale
                 * 
                 */

                if (startAngle < 0)
                {
                    startAngle += Math.PI * 2;
                    endAngle += Math.PI * 2;
                }

                /* 
                 * Se arco antiorario angolo finale dovra essere maggiore 
                 */

                if (endAngle < startAngle)
                    endAngle += 2 * Math.PI;

                var deltaAngle = endAngle - startAngle;


                if (deltaAngle == 0)
                {
                    /* è un cerchio completo*/
                    /* */

                    endAngle += Math.PI * 2;
                    deltaAngle = endAngle - startAngle;

                }

                var numeroIncrementiAngolo = (int)Math.Ceiling(Math.Abs(deltaAngle) / incrementoAngolo);

                var incrementoZ = deltaZ / numeroIncrementiAngolo;

                for (var i = startAngle; i <= endAngle; i += incrementoAngolo)
                {
                    currentZ += incrementoZ;

                    var x = (Math.Cos(i) * radius) + center.X;
                    var y = (Math.Sin(i) * radius) + center.Y;

                    arcWire.AddPnt(new Point3D(x, y, currentZ));

                }
            }

            arcWire.AddPnt(new Point3D(arc3D.End.X, arc3D.End.Y, arc3D.End.Z));

            SetColor(arcWire, arc3D);

            AddWireElement(arcWire);
        }

        ///// <summary>
        ///// Stampa arco 3D
        ///// L'elemento deve essere già definito , ( devono essere noti centro . start. end . raggio .clockwise
        ///// </summary>
        ///// <param name="arc3D"></param>
        //private void PlotElement(Arc3D arc3D)
        //{
        //    // ottengo dimensione massima scena
        //    var sceneMaxWidth = view.GetCooAxisDim();

        //    var radius = arc3D.Radius;

        //    /*
        //     * Più questo rapporto è grande , piu il raggio può essere approsimato ( disegnato con meno segmenti) 
        //     */
        //    var rapportoScenaArco = sceneMaxWidth / radius * 2;

        //    // Se diametro cerchio coincide con ampiezza scenza , per disegnare la cfr usero il numero di segmenti max
        //    const int arcSegmentMaxNumber = 60;
        //    const int arcSegmentMinNumber = 12;

        //    // se il risultato non è soddisfacente provare con proporzione
        //    var segmentUsed = arcSegmentMaxNumber / rapportoScenaArco;

        //    if (segmentUsed < arcSegmentMinNumber)
        //        segmentUsed = arcSegmentMinNumber;

        //    var arcWire = new WirePolyline();

        //    var pntArcWire = new Point3DCollection();

        //    var center = arc3D.Center;

        //    var startAngle = GeometryHelper.GetPositiveAngle(arc3D.Start.X - center.X, arc3D.Start.Y - center.Y);

        //    var endAngle = GeometryHelper.GetPositiveAngle(arc3D.End.X - center.X, arc3D.End.Y - center.Y);


        //    var deltaZ = arc3D.End.Z - arc3D.Start.Z;

        //    var currentZ = arc3D.Start.Z;



        //    //const int numeroSegmentiCerchio = 20;

        //    //const double incrementoTeorico = (Math.PI * 2) / numeroSegmentiCerchio;

        //    pntArcWire.Add(new Point3D(arc3D.Start.X, arc3D.Start.Y, arc3D.Start.Z));

        //    const double incrementoAngolo = 0.2d;



        //    /*
        //     * 
        //     * Se arco è clockwise , angolo finale è minore ,
        //     * 
        //     * si parte da angolo iniziale e si diminuisce fino a angolo finale
        //     */
        //    if (arc3D.ClockWise)
        //    {
        //        /* 
        //         * Se il senso è antiorario l'angolo finale dovra essere maggiore
        //         */

        //        if (startAngle < 0)
        //        {
        //            startAngle += Math.PI * 2;
        //            endAngle += Math.PI * 2;
        //        }

        //        if (endAngle >= startAngle)
        //            endAngle -= 2 * Math.PI;

        //        var deltaAngle = endAngle - startAngle;

        //        if (deltaAngle == 0)
        //            throw new Exception();

        //        var numeroIncrementiAngolo = (int)Math.Ceiling(deltaAngle / incrementoAngolo);

        //        var incrementoZ = deltaZ / numeroIncrementiAngolo;

        //        // Se è clocwise angolo diminuisce 
        //        for (var i = startAngle; i >= endAngle; i -= incrementoAngolo)
        //        {
        //            currentZ += incrementoZ; // incremento contralle

        //            var x = (Math.Cos(i) * radius) + center.X;
        //            var y = (Math.Sin(i) * radius) + center.Y;

        //            pntArcWire.Add(new Point3D(x, y, currentZ));
        //        }
        //    }
        //    else // Arco antiorario
        //    {
        //        /*
        //         * Se arco è ccw , angolo è maggiore ,
        //         * 
        //         * si parte da angolo iniziale e si aumenta fino a angolo finale
        //         * 
        //         */

        //        if (startAngle < 0)
        //        {
        //            startAngle += Math.PI * 2;
        //            endAngle += Math.PI * 2;
        //        }

        //        /* 
        //         * Se arco antiorario angolo finale dovra essere maggiore 
        //         */

        //        if (endAngle < startAngle)
        //            endAngle += 2 * Math.PI;

        //        var deltaAngle = endAngle - startAngle;


        //        if (deltaAngle == 0)
        //            throw new Exception();

        //        var numeroIncrementiAngolo = (int)Math.Ceiling(deltaAngle / incrementoAngolo);

        //        var incrementoZ = deltaZ / numeroIncrementiAngolo;

        //        for (var i = startAngle; i <= endAngle; i += incrementoAngolo)
        //        {
        //            currentZ += incrementoZ;

        //            var x = (Math.Cos(i) * radius) + center.X;
        //            var y = (Math.Sin(i) * radius) + center.Y;

        //            pntArcWire.Add(new Point3D(x, y, currentZ));

        //        }
        //    }

        //    pntArcWire.Add(new Point3D(arc3D.End.X, arc3D.End.Y, arc3D.End.Z));
        //    arcWire.Points = pntArcWire;

        //    SetColor(arcWire, arc3D);

        //    AddWireElement(arcWire);
        //}
        private void AddWireElement(IEnumerable<WireLine> wireBase)
        {
            foreach (var wireLine in wireBase)
            {
                wireLine.Recalculate();

                view.Add(wireLine);
            }
        }

        private void AddWireElement(WireBase wireBase)
        {
            wireBase.Recalculate();

            view.Add(wireBase);
        }

        private void PlotElement(Line3D line3D)
        {
            var normal0Wire = new WireLine
                                  {
                                      Point1 = new Point3D(line3D.Start.X, line3D.Start.Y, line3D.Start.Z),
                                      Point2 = new Point3D(line3D.End.X, line3D.End.Y, line3D.End.Z)
                                  };

            SetColor(normal0Wire, line3D);

            AddWireElement(normal0Wire);

        }


        private static void SetColor(IEnumerable<WireLine> polyline, IEntity3D entity3D)
        {
            foreach (var variable in polyline)
            {
                SetColor(variable, entity3D);
            }
        }

        private static void SetColor(WireBase wireBase, IEntity3D entity2D)
        {

            switch (entity2D.PlotStyle)
            {
                default:
                case EnumPlotStyle.Element:
                    {
                        wireBase.Color = Colors.Green;
                        wireBase.Thickness = 1.5;

                    } break;

                case EnumPlotStyle.RapidMove:
                    {
                        wireBase.Color = Colors.Coral;
                        wireBase.Thickness = 1.2;
                    } break;

                case EnumPlotStyle.SelectedElement:
                    {
                        wireBase.Color = Colors.Red;
                        wireBase.Thickness = 1.8;
                    } break;

                case EnumPlotStyle.Path:
                    {
                        wireBase.Color = Colors.PowderBlue;
                        wireBase.Thickness = .7;
                    } break;

                case EnumPlotStyle.Arc:
                    {
                        wireBase.Color = Colors.CornflowerBlue;
                        wireBase.Thickness = .7;
                    } break;

                case EnumPlotStyle.SelectedPath:
                    {
                        wireBase.Color = Colors.CornflowerBlue;
                        wireBase.Thickness = .7;

                    } break;

                case EnumPlotStyle.TrimPath:
                    {
                        wireBase.Color = Colors.GreenYellow;
                        wireBase.Thickness = 1.2;
                    } break;
            }

        }


        public void ZoomToFit()
        {
            if (view.CameraController != null)
            {
                view.ZoomToFit(1000);
            }
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ZoomToFit();
        }



    }
}

/*
 * Con polyline non fà zoomtofit. faccio piccolo hack
 */

internal class PolyLine : List<WireLine>
{

    private Point3D? _lastPoint = null;

    public void AddPnt(Point3D pnt)
    {
        if (_lastPoint == null)
        {
            _lastPoint = pnt;
            return;
        }

        var normal0Wire = new WireLine
        {
            Point1 = new Point3D(_lastPoint.Value.X, _lastPoint.Value.Y, _lastPoint.Value.Z),
            Point2 = new Point3D(pnt.X, pnt.Y, pnt.Z)
        };

        Add(normal0Wire);
        _lastPoint = pnt;
    }

}
