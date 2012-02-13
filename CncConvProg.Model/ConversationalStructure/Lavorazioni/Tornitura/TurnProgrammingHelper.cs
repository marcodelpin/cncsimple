using System;
using System.Linq;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.PathGenerator;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Tornitura
{
    internal class TurnProgrammingHelper
    {

        private static double GetUpperDiameter(Profile2D profile2D)
        {
            var s = profile2D.Source;
            var max = double.MinValue;
            foreach (var entity2D in s)
            {
                var p = entity2D.GetFirstPnt();
                var p1 = entity2D.GetLastPnt();

                if (p.Y > max)
                    max = p.Y;

                if (p1.Y > max)
                    max = p1.Y;
            }
            return max;
        }

        private static double GetLowestDiameter(Profile2D profile2D)
        {
            var s = profile2D.Source;
            var max = double.MaxValue;
            foreach (var entity2D in s)
            {
                var p = entity2D.GetFirstPnt();
                var p1 = entity2D.GetLastPnt();

                if (p.Y < max)
                    max = p.Y;

                if (p1.Y < max)
                    max = p1.Y;
            }
            return max;
        }
        private static double GetRoughFinDiameter(Profile2D profile2D, Tornitura.TipoTornitura tipoTornitura)
        {
            if (tipoTornitura == Tornitura.TipoTornitura.Esterna)
                return GetLowestDiameter(profile2D);

            return GetUpperDiameter(profile2D);
        }

        private static double GetRoughIniDiameter(Profile2D profile2D, Tornitura.TipoTornitura tipoTornitura)
        {
            if (tipoTornitura == Tornitura.TipoTornitura.Esterna)
                return GetUpperDiameter(profile2D);

            return GetLowestDiameter(profile2D);
        }

        private static double GetMinZ(Profile2D profile2D)
        {
            var s = profile2D.Source;

            var min = double.MaxValue;

            foreach (var entity2D in s)
            {
                var p = entity2D.GetFirstPnt();
                var p1 = entity2D.GetLastPnt();

                if (p.X < min)
                    min = p.X;

                if (p1.X < min)
                    min = p1.X;
            }

            return min;
        }
        private static double GetMaxZ(Profile2D profile2D)
        {
            var s = profile2D.Source;

            var maxZ = double.MinValue;

            foreach (var entity2D in s)
            {
                var p = entity2D.GetFirstPnt();
                var p1 = entity2D.GetLastPnt();

                if (p.X > maxZ)
                    maxZ = p.X;

                if (p1.X > maxZ)
                    maxZ = p1.X;
            }

            return maxZ;
        }
        internal static void GetRoughingTurnProgram(ProgramOperation programOperation, MoveActionCollection moveCollection, Profile2D profile2D, double profPassata, double avvicinamento, double stacco, Tornitura.TipoTornitura tipoTornitura, bool useMacro, double sovraX, double sovraZ)
        {
            // assumo che sia diametro esterno.

            if (CheckValueHelper.GreatherThanZero(new[] { profPassata, })) return;

            if (profile2D == null) return;

            if(useMacro)
            {
                    var turnMacro = new MacroLongitudinalTurningAction(programOperation)
                    {
                        SovraMetalloX = sovraX,
                        SovraMetalloZ = sovraZ,
                        Profile = profile2D,
                        ProfonditaPassata = profPassata,
                        Distacco = stacco,
                        TipologiaLavorazione = tipoTornitura,
                    };
            }

            switch (tipoTornitura)
            {
                case Tornitura.TipoTornitura.Esterna:
                    {
                        GetSgrossaturaEsterna(programOperation, moveCollection, profile2D, profPassata, avvicinamento, stacco);
                    } break;

                case Tornitura.TipoTornitura.Interna:
                    {
                        GetSgrossaturaInterna(moveCollection, profile2D, profPassata, avvicinamento, stacco, useMacro);
                    } break;
            }
        }

        internal static void GetSgrossaturaEsterna(ProgramOperation programOperation, MoveActionCollection moveCollection, Profile2D profile2D, double profPassata, double avvicinamento, double stacco)
        {
            /*
             * assumo anche che nel profilo non ci siano tasche..
             */
            var diaIniziale = GetRoughIniDiameter(profile2D, Tornitura.TipoTornitura.Esterna);

            var diaFinale = GetRoughFinDiameter(profile2D, Tornitura.TipoTornitura.Esterna);

            var zMin = GetMinZ(profile2D);

            var zIniziale = GetMaxZ(profile2D) + avvicinamento;

            var r = stacco;

            
                var currentDia = diaIniziale;

                moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, zIniziale, currentDia, 0);
                while (currentDia > diaFinale)
                {
                    currentDia -= profPassata;
                    if (currentDia <= diaFinale)
                        currentDia = diaFinale;

                    var scanIniPoint = new Point2D(zIniziale, currentDia);
                    var scanEndPoint = new Point2D(zMin, currentDia);

                    var scanLine = new Line2D() { Start = scanIniPoint, End = scanEndPoint };

                    var intersectPoint = FindIntersectionPoint(profile2D, scanLine);

                    if (intersectPoint == null)
                        break;

                    moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, null, currentDia, 0);



                    moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, intersectPoint.X, null, 0);

                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, zIniziale, currentDia + avvicinamento,
                                                 0);

                    //var line = new Line2D() {Start = iniPoint, End = intersectPoint};

                    // poi devo fare stacco , ritorno a z e incremento diametro.

                
            }
        }

        internal static void GetSgrossaturaInterna(PathGenerator.MoveActionCollection moveCollection, Profile2D profile2D, double profPassata, double avvicinamento, double stacco, bool useMacro)
        {
            /*
             * assumo anche che nel profilo non ci siano tasche..
             */
            var diaIniziale = GetRoughIniDiameter(profile2D, Tornitura.TipoTornitura.Interna);

            var diaFinale = GetRoughFinDiameter(profile2D, Tornitura.TipoTornitura.Interna);

            var zMin = GetMinZ(profile2D);

            var zIniziale = GetMaxZ(profile2D) + avvicinamento;

            var r = stacco;

            var currentDia = diaIniziale;

            moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, zIniziale, currentDia, 0);
            while (currentDia < diaFinale)
            {
                currentDia += profPassata;
                if (currentDia >= diaFinale)
                    currentDia = diaFinale;

                var scanIniPoint = new Point2D(zIniziale, currentDia);
                var scanEndPoint = new Point2D(zMin, currentDia);

                var scanLine = new Line2D() { Start = scanIniPoint, End = scanEndPoint };

                var intersectPoint = FindIntersectionPoint(profile2D, scanLine);

                if (intersectPoint == null)
                    break;

                moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, zIniziale, currentDia, 0);


                moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, intersectPoint.X, null, 0);

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, zIniziale, currentDia - avvicinamento, 0);

                //var line = new Line2D() {Start = iniPoint, End = intersectPoint};

                // poi devo fare stacco , ritorno a z e incremento diametro.

            }
        }


        private static Point2D FindIntersectionPoint(Profile2D profile2D, Line2D segment)
        {
            /*
             * assumo che il profilo non ha insenature o altre pippe ho solo un punto di contatto.
             */
            var s = profile2D.Source;

            foreach (var entity2D in s)
            {
                Point2D o;
                if (Geometry.GeometryHelper.Entity2DIntersection(entity2D, segment, out o))
                {
                    return o;
                }
            }

            return null;
        }

        internal static void GetSgrossaturaGolaEsterna(PathGenerator.MoveActionCollection moveCollection, double diameterIniziale, double diameterFinale, double effectiveStart, double endZ, double step, double larghPassata, double rit)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { diameterFinale, diameterIniziale, larghPassata })) return;


            var s = effectiveStart;
            var e = endZ;

            /*
             * assumo di partire da sx e andare verso dx
             */
            var currentZ = effectiveStart;


            if (step <= 0)
                step = diameterIniziale - diameterFinale;

            var currentD = diameterIniziale;

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, currentD, 0);

            while (currentD > diameterFinale)
            {
                currentD -= step;
                if (currentD < diameterFinale)
                    currentD = diameterFinale;

                moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, currentZ, currentD, 0);
                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, currentD + rit, 0);

            }

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, null, diameterIniziale, 0);

            while (currentZ < e)
            {
                currentZ += larghPassata;

                if (currentZ > e)
                    currentZ = e;

                currentD = diameterIniziale;

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, currentD, 0);

                while (currentD > diameterFinale)
                {
                    currentD -= step;
                    if (currentD < diameterFinale)
                        currentD = diameterFinale;

                    moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, currentZ, currentD, 0);
                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, currentD + rit, 0);

                }

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, diameterIniziale, 0);

            }
        }
        internal static void GetFinishingProgram(PathGenerator.MoveActionCollection moveCollection, Profile2D profile2D, Tornitura.TipoTornitura tipoTornitura, double avvicinamento)
        {
            var s = profile2D.Source;

            var f = s.FirstOrDefault();

            if (f == null) return;

            var iniP = f.GetFirstPnt();

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, iniP.X + avvicinamento, iniP.Y, 0);

            foreach (var move in s)
            {
                if (move is Line2D)
                {
                    var m = move as Line2D;
                    var p = m.GetLastPnt();
                    moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, p.X, p.Y, 0);
                }
                else if (move is Arc2D)
                {
                    var m = move as Arc2D;

                    var p = m.GetLastPnt();

                    moveCollection.AddArcMove(AxisAbilited.Xyz, p.X, p.Y, 0, m.Radius, m.ClockWise, m.Center);
                }
            }

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, iniP.X + avvicinamento, null, null);
        }

        internal static void GetFaceRoughing(PathGenerator.MoveActionCollection moveCollection, double diametroMax, double diametroMin, double zMin, double zMax, double profPassata, double stacco)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { profPassata })) return;

            var dMax = diametroMax;
            var dMin = diametroMin;

            var zEnd = zMin;

            var currentZ = zMax;

            while (currentZ > zEnd)
            {
                currentZ -= profPassata;

                if (currentZ <= zEnd)
                    currentZ = zEnd;

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, dMax, 0);
                moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, null, dMin, 0);
                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ + stacco, dMax, 0);
            }
        }

        internal static void GetFaceFinishing(PathGenerator.MoveActionCollection moveCollection, double diametroMax, double diametroMin, double zValue, double stacco)
        {

            var dMax = diametroMax;
            var dMin = diametroMin;

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, zValue, dMax, 0);
            moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, null, dMin, 0);
            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, zValue + stacco, dMax, 0);
        }
        internal static void GetSgrossaturaGolaInterna(PathGenerator.MoveActionCollection moveCollection, double diameterIniziale, double diameterFinale, double effectiveStart, double endZ, double step, double larghPassata, double rit)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { diameterFinale, diameterIniziale, larghPassata })) return;


            var s = effectiveStart;
            var e = endZ;

            /*
             * assumo di partire da sx e andare verso dx
             */
            var currentZ = effectiveStart;

            if (step <= 0)
                step = diameterFinale - diameterIniziale;

            var currentD = diameterIniziale;

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, currentD, 0);

            while (currentD < diameterFinale)
            {
                currentD += step;
                if (currentD > diameterFinale)
                    currentD = diameterFinale;

                moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, currentZ, currentD, 0);
                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, currentD - rit, 0);

            }

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, null, diameterIniziale, 0);


            while (currentZ < e)
            {
                currentZ += larghPassata;

                if (currentZ > e)
                    currentZ = e;

                currentD = diameterIniziale;

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, currentD, 0);

                while (currentD < diameterFinale)
                {
                    currentD += step;
                    if (currentD > diameterFinale)
                        currentD = diameterFinale;

                    moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, currentZ, currentD, 0);
                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, currentD - rit, 0);

                }

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currentZ, diameterIniziale, 0);

            }
        }

        internal static void GetSgrossaturaGolaFrontale(PathGenerator.MoveActionCollection moveCollection, double diameterIniziale, double diameterFinale, double iniZ, double endZ, double step, double larghPassata, double rit)
        {
            if (CheckValueHelper.GreatherThanZero(new[] { larghPassata })) return;

            var effectiveEndX = diameterFinale;
            /*
             * assumo di partire da sx e andare verso dx
             */
            var currentX = diameterIniziale;

            if (step <= 0)
                step = iniZ - endZ;

            var currenzZ = iniZ;

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currenzZ, currentX, 0);

            while (currenzZ > endZ)
            {
                currenzZ -= Math.Abs(step);
                if (currenzZ < endZ)
                    currenzZ = endZ;

                moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, currenzZ, null, 0);
                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currenzZ + rit, null, 0);

            }

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, iniZ, null, 0);


            while (currentX < effectiveEndX)
            {
                currentX += larghPassata;

                if (currentX > effectiveEndX)
                    currentX = effectiveEndX;

                currenzZ = iniZ;


                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, null, currentX, 0);

                while (currenzZ > endZ)
                {
                    currenzZ -= Math.Abs(step);
                    if (currenzZ < endZ)
                        currenzZ = endZ;

                    moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, currenzZ, null, 0);
                    moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, currenzZ + rit, null, 0);

                }

                moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, iniZ, null, 0);


            }
        }

        internal static void GetFinituraGolaEsternaInterna(PathGenerator.MoveActionCollection moveCollection, double diameterIniziale, double diameterFinale, double effectiveStart, double endZ, double stacco, double avvicinamento)
        {

            if (diameterIniziale < diameterFinale)

                avvicinamento = -Math.Abs(avvicinamento);



            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, endZ, diameterIniziale + avvicinamento, 0);



            moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, null, diameterFinale, 0);



            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, endZ + stacco, diameterIniziale + avvicinamento, 0);



            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, effectiveStart, null, 0);



            moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, null, diameterFinale, 0);



            moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, endZ, null, 0);



            moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, endZ + stacco, diameterIniziale + avvicinamento, 0);

        }



        internal static void GetFinituraGolaFrontale(PathGenerator.MoveActionCollection moveCollection, double diameterIniziale, double diameterFinale, double zIni, double zEnd, double avvicinamento, double stacco)
        {

            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, zIni + avvicinamento, diameterFinale, 0);



            moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, zEnd, null, 0);



            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, zIni + avvicinamento, diameterFinale - stacco, 0);



            moveCollection.AddLinearMove(MoveType.Rapid, AxisAbilited.Xyz, null, diameterIniziale, 0);



            moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, zEnd, null, 0);



            moveCollection.AddLinearMove(MoveType.Work, AxisAbilited.Xyz, zIni + avvicinamento, diameterIniziale + stacco, 0);

        }


    }
}

