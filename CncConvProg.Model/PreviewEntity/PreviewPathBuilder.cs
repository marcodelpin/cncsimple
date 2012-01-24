using System;
using System.Collections.Generic;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.PreviewPathEntity;

namespace CncConvProg.Geometry.PreviewPathEntity
{
    /// <summary>
    /// Classe che rappresenta path nello spazio 3D.
    /// 
    /// Utilizzata per :
    ///     - Memorizzare il percorso utensile
    /// 
    /// </summary>
    [Serializable]
    public class PreviewPathBuilder  // interfaccia 3d solamente per poter essere stampata da controllo view , 
    {
        /*
         * apro parentesi su ientity 2d .
         * 
         * devo creare serie di classe apposite per stampa ,
         * 
         * ientity2d mi dovrà servire solamente per 2d 
         * 
         * mentre la nuova serie di classi is dovra occupare della pèreview..
         * 
         */

        private Point3D _lastPoint = null;

        private readonly List<IPreviewEntity> _profile = new List<IPreviewEntity>();

        private double? _x;

        private double? _y;

        private double? _z;

        private Point3D GetLastPoint(double? x, double? y, double? z)
        {
            /*
             * fino a che non ottengo 3 variabili definito _lastPoint non è definito.
             * 
             * quindi per centro mi serve zxy ok
             * 
             */
            if (_lastPoint != null) return _lastPoint;

            if (x.HasValue)
                _x = x.Value;

            if (y.HasValue)
                _y = y.Value;

            if (z.HasValue)
                _z = z.Value;


            if (_x.HasValue && _y.HasValue && _z.HasValue)
            {
                _lastPoint = new Point3D(_x.Value, _y.Value, _z.Value);

                return _lastPoint;
            }

            return null;
        }

        /// <summary>
        ///  Aggiunge linea al profilo
        /// </summary>
        public void AddLine(EnumPlotStyle enumPlotStyle, double? x, double? y, double? z, ParametroVelocita parametroVelocita)
        {
            /*
             * private 
             */

            var lastPoint = GetLastPoint(x, y, z);

            if (lastPoint == null)
                return;

            var endpnt = new Point3D(_lastPoint);

            if (x.HasValue)
                endpnt.X = x.Value;

            if (y.HasValue)
                endpnt.Y = y.Value;

            if (z.HasValue)
                endpnt.Z = z.Value;


            var line = new PreviewLine3D { Start = new Point3D(_lastPoint), End = new Point3D(endpnt), PlotStyle = enumPlotStyle };

            line.ParametroVelocita = parametroVelocita;

            _lastPoint = new Point3D(endpnt);

            AddEntity(line);
        }

        /// <summary>
        ///  Aggiunge linea al profilo, enum con tipo movimento
        /// </summary>
        public void AddArc(EnumPlotStyle plotStyle, Point3D center, double radius, bool clockWise, double? endX, double? endY, double? endZ, ParametroVelocita parametroVelocita)
        {

            var lastPoint = GetLastPoint(endX, endY, endZ);

            if (lastPoint == null)
                return;

            var endPoint = new Point3D(_lastPoint);

            if (endX.HasValue)
                endPoint.X = endX.Value;

            if (endY.HasValue)
                endPoint.Y = endY.Value;

            if (endZ.HasValue)
                endPoint.Z = endZ.Value;


            var arc3D = new PreviewArc3D() { Start = new Point3D(_lastPoint), End = new Point3D(endPoint), Center = center, Radius = radius, ClockWise = clockWise, PlotStyle = plotStyle };

            arc3D.ParametroVelocita = parametroVelocita;

            _lastPoint = new Point3D(endPoint);

            AddEntity(arc3D);
        }

        private void AddEntity(IPreviewEntity entity)
        {
            _profile.Add(entity);
        }

        public List<IPreviewEntity> GetProfile()
        {
            return _profile;
        }

    }
}
