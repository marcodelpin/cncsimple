using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using CncConvProg.Geometry.RawProfile2D;

namespace CncConvProg.View.ValueConverter
{
    public class EnumToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value,
            Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (parameter.ToString().ToLower().Equals(value.ToString().ToLower()))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter;

        }
        #endregion

    }

    public class EnumToImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value,
            Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            // todo : magari pensare anche a rotazione bindata con angolo,
            // anche se magari non è buona idea per via 
            /*
             *  1 - è più chiaro con rotazione max di 45 *
             *  2 - ci sono casi speciali. initpnt , arc e indefined dove in questi casi servirebbe doppio valueConverter.
              */
            switch ((RawEntity2D.RawEntityOrientation)value)
            {
                case RawEntity2D.RawEntityOrientation.ArcCcw:
                    {
                        return new Uri("pack://application:,,,/Images/gui/move_ccw.png");
                    }break;

                case RawEntity2D.RawEntityOrientation.ArcCw:
                    {
                        return new Uri("pack://application:,,,/Images/gui/move_cw.png");
                    } break;

                case RawEntity2D.RawEntityOrientation.Up:
                    {
                        return new Uri("pack://application:,,,/Images/gui/Up.png");
                    } break;

                case RawEntity2D.RawEntityOrientation.Ne:
                    {
                        return new Uri("pack://application:,,,/Images/gui/MoveUpLeft.png");
                    } break;


                case RawEntity2D.RawEntityOrientation.No:
                    {
                        return new Uri("pack://application:,,,/Images/gui/MoveUpRight.png");
                    } break;


                case RawEntity2D.RawEntityOrientation.Dx:
                    {
                        return new Uri("pack://application:,,,/Images/gui/right.png");
                    } break;


                case RawEntity2D.RawEntityOrientation.Sx:
                    {
                        return new Uri("pack://application:,,,/Images/gui/left.png");
                    } break;


                case RawEntity2D.RawEntityOrientation.Down:
                    {
                        return new Uri("pack://application:,,,/Images/gui/down.png");
                    } break;

                case RawEntity2D.RawEntityOrientation.So:
                    {
                        return new Uri("pack://application:,,,/Images/gui/moveDownright.png");
                    } break;

                case RawEntity2D.RawEntityOrientation.Se:
                    {
                        return new Uri("pack://application:,,,/Images/gui/moveDownleft.png");
                    } break;

                case RawEntity2D.RawEntityOrientation.InitPoint:
                    {
                        return new Uri("pack://application:,,,/Images/gui/profileStartPoint.png");
                    } break;

                case RawEntity2D.RawEntityOrientation.NotDefined:
                    return new Uri("pack://application:,,,/Images/gui/moveundefined.png");

                default:
                    {
                        return new Uri("pack://application:,,,/Images/archivia.png");
                    } break;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter;

        }
        #endregion

    }
}
