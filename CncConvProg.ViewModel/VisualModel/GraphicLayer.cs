//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Media;
//using System.Windows.Media.Media3D;
//using CncConvProg.Geometry;
//using CncConvProg.Geometry.Entity;
//using CPlusPlusLibrary;
//using Point3D = System.Windows.Media.Media3D.Point3D;

//namespace CncConvProg.ViewModel.VisualModel
//{
//    public class GraphicLayer : IDisposable
//    {
//        private const int Meridian = 20;
//        private readonly Wrap _wrap;

//        /*
//         * current tool type
//         * current toll profile
//         * current position..
//         */

//        public GraphicLayer()
//        {
//            _wrap = new Wrap();

//            _wrap.Init();
//        }

//        public ModelVisual3D GetModel()
//        {
//            var triangles = _wrap.GetTriangleScene();

//            /*
//             * prendo linee e archi sempre da e aggiungo sempre alla scena.
//             * 
//             * colore diverso fra stock e grezzo..
//             */

//            var cube = new Model3DGroup();

//            foreach (var triangle in triangles)
//            {
//                cube.Children.Add(
//                    CreateTriangleModel(new Point3D(triangle.p1.pos.x, triangle.p1.pos.y, triangle.p1.pos.z),
//                                        new Point3D(triangle.p2.pos.x, triangle.p2.pos.y, triangle.p2.pos.z),
//                                        new Point3D(triangle.p3.pos.x, triangle.p3.pos.y, triangle.p3.pos.z)));

//            }

//            var model = new ModelVisual3D { Content = cube };

//            return model;
//        }

//        /// <summary>
//        /// Crea Grezzo
//        /// </summary>
//        public void CreateLatheStock(float radius, float length, float zOffset)
//        {
//            _wrap.DrawCylinder(radius, length, Meridian, 0, 0, 0);

//            //////////////////
//            _wrap.ExtrudeProfile(2, 2, 3);

//         //   _wrap.MoveTurningTool(9, 9);

//            var k = new Profile2D();
//            var line1 = new Line2D { Start = new Point2D { X = 1, Y = 1 }, End = new Point2D { X = 10, Y = 1 } };

//            k.AddEntity(line1);

//            var line2 = new Line2D { Start = new Point2D { X = 10, Y = 1 }, End = new Point2D { X = 10, Y = 10 } };

//            k.AddEntity(line2);

//            var line3 = new Line2D { Start = new Point2D { X = 10, Y = 10 }, End = new Point2D { X = 1, Y = 10 } };

//            k.AddEntity(line3);

//            var line4 = new Line2D { Start = new Point2D { X = 1, Y = 10 }, End = new Point2D { X = 1, Y = 1 } };

//            k.AddEntity(line4);



//            var pollo = new List<Point3d>();
//            for (int i = 0; i < k.Source.Count; i++)
//            {
//                //pollo.Add(new Point3d(k.Source[i].Start.X, k.Source[i].Start.Y, 0));
//                //pollo.Add(new Point3d(k.Source[i].End.X, k.Source[i].End.Y, 0));
//            }


//            _wrap.RevolveAndRemoveProfile(pollo, pollo.Count/2);

//        }

//        public void MoveTurningTool(float endX, float endY)
//        {
//            var insertProfile = new Profile2D(); // prendere geometria da inserto
//            //_wrap.revolveAndRemoveProfile(profile profile)
//            /*
//             * sposta profilo utensile 
//             * 
//             * graham scan e 
//             * dare profilo  ottenuto ..
//             */

//        }

//        public void DrawLatheTool(/* profilo , dimensione, */)
//        {
//        }


//        private static Model3DGroup CreateTriangleModel(Point3D p0, Point3D p1, Point3D p2)
//        {
//            var mesh = new MeshGeometry3D();
//            mesh.Positions.Add(p0);
//            mesh.Positions.Add(p1);
//            mesh.Positions.Add(p2);
//            mesh.TriangleIndices.Add(0);
//            mesh.TriangleIndices.Add(1);
//            mesh.TriangleIndices.Add(2);
//            var normal = CalculateNormal(p0, p1, p2);
//            mesh.Normals.Add(normal);
//            mesh.Normals.Add(normal);
//            mesh.Normals.Add(normal);
//            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkKhaki));
//            var model = new GeometryModel3D(
//                mesh, material);
//            var group = new Model3DGroup();
//            group.Children.Add(model);
//            return group;
//        }

//        private static Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
//        {
//            var v0 = new Vector3D(
//                p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
//            var v1 = new Vector3D(
//                p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
//            return Vector3D.CrossProduct(v0, v1);
//        }

//        public void Dispose()
//        {
//            //_wrap.Dispose();
//        }

//        //internal void ChangeTool(TurningInsert turningInsert)
//        //{
//        //    /*
//        //     * nella scena rimuove , tool vecchio e mette quello nuovo azzerato nella stessa posizione.
//        //     */
//        //}
//    }
//}
