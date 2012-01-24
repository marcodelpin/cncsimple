using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CncConvProg.Geometry
{
    [Serializable]
    public class Point3D
    {
        public Point3D()
        {
            
        }
        public Point3D(Point3D point3D)
        {
            X = point3D.X;
            Y = point3D.Y;
            Z = point3D.Z;
        }
        //public Point3d() { }
        public Point3D(Double mX, Double mY, Double mZ = 0) { X = mX; Y = mY; Z = mZ; }
        //public Point3D(Double mX, Double mY) { X = mX; Y = mY; Z = 0; }
        public Double X;
        public Double Y;
        public Double Z;
    }
}

