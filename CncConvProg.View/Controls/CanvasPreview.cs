using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;

namespace CncConvProg.View.Controls
{
    public class PreviewCanvas : Canvas
    {
        Double _latoCanvasMin;
        Double _latoMaxScena;

        private ObservableCollection<IEntity2D> _myShapeListRef;

        /// <summary>
        /// Ctor
        /// </summary>
        public PreviewCanvas()
        {
            DataContextChanged += UiPreviewCanvasDataContextChanged;
            SizeChanged += UiPreviewCanvasSizeChanged;
        }

        void UiPreviewCanvasSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (_myShapeListRef != null)
            {
                // Trovo ingombro della scena , serve per la normalizzazione delle coordinate 
                _latoMaxScena = 0;

                FindMaxExtensionsFromList(_myShapeListRef, ref _latoMaxScena);

                DrawScene();
            }
        }

        /// <summary>
        /// Richiamato quando viene ri/assegnato il DataContext al controllo
        /// </summary>
        void UiPreviewCanvasDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Prendo il riferimento alla BindingList contenuta nei ViewModel 
            _myShapeListRef = DataContext as ObservableCollection<IEntity2D>;

            if (_myShapeListRef != null)
            {

                _latoCanvasMin = Math.Min(ActualHeight, ActualWidth);

                _latoMaxScena = 0;

                FindMaxExtensionsFromList(_myShapeListRef, ref _latoMaxScena);

                DrawScene();

            }
        }

        ///// <summary>
        ///// Richiamato per fare il refresh della scena 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void _myShapeListRef_RequestRefresh(object sender, EventArgs e)
        //{

        //    // Trovo ingombro della scena , serve per la normalizzazione delle coordinate 
        //    _latoMaxScena = 0;

        //    _myShapeListRef = sender as PreviewShapes;

        //    FindMaxExtensionsFromList(_myShapeListRef, ref _latoMaxScena);

        //    DrawScene();
        //}

        #region DisegnaScena

        /// <summary>
        /// Disegna il contenuto del dictionary. 
        /// </summary>
        private void DrawScene()
        {
            // Prima cancello tutto il contenuto del Canvas
            Children.Clear();

            // ridisegno Assi
            DrawAxis();

            foreach (IEntity2D ps in _myShapeListRef)
            {
                if (ps is Line2D)
                {
                    PlotElement((Line2D)ps);
                }
                else if (ps is Arc2D)
                {
                    PlotElement(ps as Arc2D);
                }
                //if (ps is ProfileModel)
                //{
                //    PlotProfile((ProfileModel)ps);
                //}
                //else if (ms is MyCircle)
                //{
                //    PlotHole((MyCircle)ms);
                //}
            }

        }

        #endregion


        #region Disegna linea
        /// <summary>
        /// Funzione usata per disegnare linee
        /// </summary>
        void PlotElement(Line2D line)
        {
            var newLine = new Line
                              {
                                  X1 = NormX(line.Start.X, 0),
                                  Y1 = NormY(line.Start.Y, 0),
                                  X2 = NormX(line.End.X, 0),
                                  Y2 = NormY(line.End.Y, 0)
                              };

            //if (line.IsSelected)
            //{
            //    newLine.Stroke = Brushes.Red;
            //    newLine.StrokeThickness = 3.0;
            //}
            //else
            //{
            //    newLine.Stroke = Brushes.Blue;
            //    newLine.StrokeThickness = 1.0;
            //}

            switch (line.PlotStyle)
            {
                default:
                case EnumPlotStyle.Element:
                    {
                        newLine.Stroke = Brushes.Green;
                        newLine.StrokeThickness = 1.5;

                    } break;

                case EnumPlotStyle.SelectedElement:
                    {
                        newLine.Stroke = Brushes.Red;
                        newLine.StrokeThickness = 2.5;
                    } break;

                case EnumPlotStyle.Path:
                    {
                        newLine.Stroke = Brushes.OrangeRed;
                        newLine.StrokeThickness = .3;
                    } break;

                case EnumPlotStyle.SelectedPath:
                    {
                        newLine.Stroke = Brushes.Gainsboro;
                        newLine.StrokeThickness = .5;

                    } break;


            }

            Children.Add(newLine);
        }
        #endregion

        
        private static double GetPositiveAngle(double x, double y)
        {
            var rslt = Math.Atan2(y, x);

            if (rslt < 0)
                rslt = (Math.PI * 2) - Math.Abs(rslt);

            return rslt;
        }

        #region Disegna linea
        /// <summary>
        /// Funzione usata per disegnare linee
        /// </summary>
        void PlotElement(Arc2D arc)
        {
            // se l'arco è definito solamente dal raggio prendo cerchio
            // è fare metodo per definire meglio il raggio dell'elemento da stampar..

            if (arc.Start == null && arc.End == null && arc.Radius > 0 && arc.Center != null)
            {
                var el = new Ellipse { Height = NormX(arc.Radius, arc.Radius), Width = NormX(Width, arc.Radius/2)};

                el.SetValue(LeftProperty, NormX(arc.Center.X, arc.Radius / 2));
                el.SetValue(TopProperty, NormY(arc.Center.Y, arc.Radius / 2));

                //todo: il settaggio rtaggrupparlo con metodo setHiglight(ientity2d ent, graphicelemtne graphic element)

                switch (arc.PlotStyle)
                {
                    default:
                    case EnumPlotStyle.Element:
                        {
                            el.Stroke = Brushes.Green;
                            el.StrokeThickness = 1.5;

                        } break;

                    case EnumPlotStyle.SelectedElement:
                        {
                            el.Stroke = Brushes.Red;
                            el.StrokeThickness = 2.5;
                        } break;

                    case EnumPlotStyle.Path:
                        {
                            el.Stroke = Brushes.OrangeRed;
                            el.StrokeThickness = .3;
                        } break;

                    case EnumPlotStyle.SelectedPath:
                        {
                            el.Stroke = Brushes.Gainsboro;
                            el.StrokeThickness = .5;

                        } break;

                }

                Children.Add(el);

                return;
            }

            // todo : controllare presenza 3 punti 

            var center = arc.Center;

            var startAngle = GetPositiveAngle(arc.Start.X - center.X, arc.Start.Y - center.Y);

            var endAngle = GetPositiveAngle(arc.End.X - center.X, arc.End.Y - center.Y);

            var radius = arc.Radius;

            var myPathFigure = new PathFigure();

            myPathFigure.StartPoint = new Point(NormX(arc.Start.X, 0), NormY(arc.Start.Y, 0));

            var myPathSegmentCollection = new PathSegmentCollection();


            if (arc.ClockWise)
            {
                if (startAngle < endAngle)
                    startAngle += 2 * Math.PI;

                for (var i = startAngle; i >= endAngle; i -= .05)
                {
                    var myLineSegment = new LineSegment();
                    var x = (Math.Cos(i) * radius) + center.X;
                    var y = (Math.Sin(i) * radius) + center.Y;

                    myLineSegment.Point = new Point(NormX(x, 0), NormY(y, 0));
                    myPathSegmentCollection.Add(myLineSegment);
                }
            }
            else
            {
                if (endAngle < startAngle)
                    endAngle += 2 * Math.PI;

                if (endAngle > startAngle)
                {
                    for (var i = startAngle; i <= endAngle; i += .05)
                    {
                        var myLineSegment = new LineSegment();
                        var x = (Math.Cos(i) * radius) + center.X;
                        var y = (Math.Sin(i) * radius) + center.Y;

                        myLineSegment.Point = new Point(NormX(x, 0), NormY(y, 0));
                        myPathSegmentCollection.Add(myLineSegment);
                    }
                }
            }





            myPathFigure.Segments = myPathSegmentCollection;

            var myPathFigureCollection = new PathFigureCollection { myPathFigure };

            var myPathGeometry = new PathGeometry { Figures = myPathFigureCollection };

            var myPath = new Path { Data = myPathGeometry };

            switch (arc.PlotStyle)
            {
                default:
                case EnumPlotStyle.Element:
                    {
                        myPath.Stroke = Brushes.Green;
                        myPath.StrokeThickness = 1.5;

                    } break;

                case EnumPlotStyle.SelectedElement:
                    {
                        myPath.Stroke = Brushes.Red;
                        myPath.StrokeThickness = 2.5;
                    } break;

                case EnumPlotStyle.Path:
                    {
                        myPath.Stroke = Brushes.OrangeRed;
                        myPath.StrokeThickness = .3;
                    } break;

                case EnumPlotStyle.SelectedPath:
                    {
                        myPath.Stroke = Brushes.Gainsboro;
                        myPath.StrokeThickness = .5;

                    } break;


            }

            Children.Add(myPath);
        }
        #endregion

        #region Disegna foro
        /// <summary>
        /// Funzione usata per elaborare visualizzazione dei fori
        /// </summary>
        /// <param name="circle"></param>
        //void PlotHole(MyCircle circle)
        //{
        //    Ellipse el = new Ellipse();
        //    el.Height = circle.Radius;
        //    el.Width = circle.Radius;
        //    el.SetValue(Canvas.LeftProperty, NormX(circle.X, circle.Radius / 2));
        //    el.SetValue(Canvas.TopProperty, NormY(circle.Y, circle.Radius / 2));
        //    el.Stroke = Brushes.Blue;
        //    el.StrokeThickness = 1.0;

        //    this.Children.Add(el);
        //}
        #endregion

        //#region Disegna profilo
        ///// <summary>
        ///// Funzione usata per elaborare la visualizzazione di un profilo
        ///// </summary>
        ///// <param name="profile"></param>
        //void PlotProfile(ProfileModel profile)
        //{
        //    if (profile.Count > 1)
        //    {
        //        Debug.Assert(profile != null, "Profilo deve essere inizializzato");
        //        // Debug.Assert(profile.Count > 1, "Profilo non deve essere vuoto");
        //        Debug.Assert(profile[0] is ProfileInit, "Il primo elemento del profilo deve essere InitPoint");

        //        Point startPoint = new Point();

        //        float x = ((ProfileInit)profile[0]).Xvalue;
        //        float y = ((ProfileInit)profile[0]).Yvalue;

        //        startPoint.X = this.NormX(x, 0);
        //        startPoint.Y = this.NormY(y, 0);


        //        for (int i = 1; i < profile.Count; i++)
        //        {
        //            if (profile[i] is ProfileLine)
        //            {
        //                ProfileMove thisMove = (ProfileMove)profile[i];
        //                ProfileMove prevMove = (ProfileMove)profile[i - 1];

        //                Line newLine = new Line();
        //                newLine.X1 = this.NormX(prevMove.Xvalue, 0);
        //                newLine.Y1 = this.NormY(prevMove.Yvalue, 0);
        //                newLine.X2 = this.NormX(thisMove.Xvalue, 0);
        //                newLine.Y2 = this.NormY(thisMove.Yvalue, 0);

        //                if (thisMove.IsHighlighted)
        //                {
        //                    newLine.Stroke = Brushes.Red;
        //                    newLine.StrokeThickness = 3.0;
        //                }
        //                else
        //                {
        //                    newLine.Stroke = Brushes.Blue;
        //                    newLine.StrokeThickness = 1.0;
        //                }

        //                this.Children.Add(newLine);

        //            }

        //        }
        //    }
        //}

        //#endregion

        #region Ingombro_scena
        /// <summary>
        /// Trova ingombri massimi della scena ,
        /// Utilizzata per dimensionare la scena in funzione della dimensione del canvas
        /// </summary>
        private void FindMaxExtensionsFromList(IEnumerable<IEntity2D> previewList, ref double max)
        {
            _latoCanvasMin = Math.Min(ActualHeight, ActualWidth);

            var generalPointList = new List<Point2D>();

            // per ogni MyShape prendo il suo bounding Square e li aggiungo alla lista generale 
            foreach (var el in previewList)
            {
                foreach (var point in el.GetBoundingSquare())
                {
                    generalPointList.Add(point);
                }
            }

            GetMaxFromPointList(generalPointList, ref max);

        }

        #endregion

        ///// <summary>
        ///// Restituisce una Shape , passandogli una MyShape
        ///// </summary>
        ///// <param name="sh"> Custom Shape </param>
        ///// <returns></returns>
        //private Shape GetEquivalentShape(MyShape sh)
        //{
        //    Shape result = null;

        //    if (sh is MyCircle)
        //    {
        //        MyCircle circle = sh as MyCircle;
        //        Ellipse el = new Ellipse();
        //        el.Height = circle.Radius;
        //        el.Width = circle.Radius;
        //        el.SetValue(Canvas.LeftProperty, NormX(circle.X, circle.Radius / 2));
        //        el.SetValue(Canvas.TopProperty, NormY(circle.Y, circle.Radius / 2));
        //        el.Stroke = Brushes.Blue;
        //        el.StrokeThickness = 1.0;
        //        result = (Shape)el;
        //    }

        //    //if (sh is MyPoint)
        //    //{
        //    //    MyPoint point = sh as MyPoint;
        //    //    Ellipse el = new Ellipse();
        //    //    el.Height = el.Width = 5;
        //    //    el.SetValue(Canvas.LeftProperty, NormX(point.X, 5 / 2));
        //    //    el.SetValue(Canvas.TopProperty, NormY(point.Y, 5 / 2));
        //    //    el.Stroke = Brushes.Blue;
        //    //    el.StrokeThickness = 1.0;
        //    //    result = (Shape)el;
        //    //}

        //    //if (sh is MyLine)
        //    //{
        //    //    MyLine line = sh as MyLine;
        //    //    Line ln = new Line();
        //    //    ln.X2 = NormX(line.EndX, 0);
        //    //    ln.Y2 = NormY(line.EndY, 0);
        //    //    ln.Stroke = Brushes.Blue;
        //    //    ln.StrokeThickness = 1.0;
        //    //    result = (Shape)ln;
        //    //}

        //    return result;
        //}


        ///// <summary>
        ///// Richiamato quando la BindingList ha modifiche ( anche di una proprietà di un elemento contenuto in essa).
        ///// L'aggiunta di nuovi elementi nel dictionary la faccio qua, 
        ///// Qui richiamo anche il DrawScene().
        ///// 
        ///// [O B S O L E T E ]
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void MyShapeListRef_ListChanged(object sender, ListChangedEventArgs e)
        //{
        //    ShapeList sendList = sender as ShapeList;

        //    //  _myShapeListRef.ListChanged -= MyShapeListRef_ListChanged;

        //    // Trovo ingombro della scena , serve per la normalizzazione delle coordinate 
        //    LatoMaxScena = 0;

        //    FindMaxExtensionsFromList(_myShapeListRef, ref LatoMaxScena);

        //    DrawScene();

        //    //_myShapeListRef.ListChanged += MyShapeListRef_ListChanged;
        //}

        /// <summary>
        /// Restituisce il valore massimo da una lista di Point2d
        /// </summary>
        private static void GetMaxFromPointList(IEnumerable<Point2D> pntList, ref Double maxT)
        {
            foreach (var p in pntList)
            {
                if (Math.Abs(p.X) > maxT)
                    maxT = Math.Abs(p.X);

                if (Math.Abs(p.Y) > maxT)
                    maxT = Math.Abs(p.Y);
            }
        }



        #region _Normalize_Coordinate
        /// <summary>
        /// Normalizzazione Coordinate 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private Double NormX(Double x, Double radius)
        {
            // widthMax -
            Double widthMax = (this._latoCanvasMin / 2 + radius) * .9;
            Double rslt = 0;
            // con proporzione 
            if (_latoMaxScena != 0)
            {
                rslt = (this.ActualWidth / 2) + ((widthMax * x) / this._latoMaxScena) - radius;
            }
            else
            {
                rslt = (this.ActualWidth / 2) + (widthMax * x) - radius;
            }

            return rslt;
        }

        private Double NormY(Double y, Double radius)
        {
            // heigthMax -
            var heightMax = (_latoCanvasMin / 2 + radius) * .9;
            // trovo la y normalizzata , tramite una proporzione 
            Double rslt = 0;
            if (_latoMaxScena != 0)
            {
                rslt = (ActualHeight / 2) - ((heightMax * y) / _latoMaxScena) - radius;
            }
            else
            {
                rslt = (ActualHeight / 2) - (heightMax * y) - radius;
            }
            return rslt;

        }
        #endregion


        #region _Draw_Axis
        /// <summary>
        /// Disegna assi
        /// </summary>
        public void DrawAxis()
        {
            // asse ordinate 
            var yAxis = new Line();
            yAxis.Stroke = Brushes.Blue;
            yAxis.StrokeThickness = .5;
            yAxis.X1 = yAxis.X2 = this.ActualWidth / 2;
            yAxis.Y1 = 0;
            yAxis.Y2 = this.ActualHeight;
            yAxis.StrokeDashArray.Add(20);
            yAxis.StrokeDashArray.Add(12);
            yAxis.StrokeDashArray.Add(4);
            yAxis.StrokeDashArray.Add(12);
            this.Children.Add(yAxis);

            // asse ascisse
            var xAxis = new Line();
            xAxis.Stroke = Brushes.Blue;
            xAxis.StrokeThickness = .5;
            xAxis.Y1 = xAxis.Y2 = this.ActualHeight / 2;
            xAxis.X1 = 0;
            xAxis.X2 = this.ActualWidth;
            xAxis.StrokeDashArray.Add(20);
            xAxis.StrokeDashArray.Add(12);
            xAxis.StrokeDashArray.Add(4);
            xAxis.StrokeDashArray.Add(12);
            this.Children.Add(xAxis);
        }
        #endregion



    }

}