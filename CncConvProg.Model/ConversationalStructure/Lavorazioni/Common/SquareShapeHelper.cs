using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.Entity;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Fresatura;

namespace CncConvProg.Model.ConversationalStructure.Lavorazioni.Common
{
    public static class SquareShapeHelper
    {
        public enum SquareShapeStartPoint
        {
            UpLeft,
            UpRight,
            DownLeft,
            DownRight,
            Center
        }

        public static Profile2D GetSquareProfile(double centerX, double centerY, double width, double height)
        {
            var pp1 = new Point2D(centerX + width / 2, centerY + height / 2);
            var pp2 = new Point2D(centerX + width / 2, centerY - height / 2);
            var pp3 = new Point2D(centerX - width / 2, centerY - height / 2);
            var pp4 = new Point2D(centerX - width / 2, centerY + height / 2);

            var polygon = new Profile2D();

            polygon.AddPnt(pp1);
            polygon.AddPnt(pp2);
            polygon.AddPnt(pp3);
            polygon.AddPnt(pp4);
            polygon.AddPnt(pp1);

            return polygon;
        }
        public static Point2D GetCenterPoint(SquareShapeStartPoint startPoint, double pntX, double pntY, double width, double height)
        {
            switch (startPoint)
            {
                case SquareShapeStartPoint.UpLeft:
                    return new Point2D { X = pntX + width / 2, Y = pntY - height / 2 };

                case SquareShapeStartPoint.UpRight:
                    return new Point2D { X = pntX - width / 2, Y = pntY - height / 2 };

                case SquareShapeStartPoint.DownLeft:
                    return new Point2D { X = pntX + width / 2, Y = pntY + height / 2 };

                case SquareShapeStartPoint.DownRight:
                    return new Point2D { X = pntX - width / 2, Y = pntY + height / 2 };

                case SquareShapeStartPoint.Center:
                default:
                    return new Point2D(pntX, pntY);
            }
        }
    }
}
