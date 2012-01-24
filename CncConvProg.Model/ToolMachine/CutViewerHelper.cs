using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using CncConvProg.Model.Tool.Drill;
using CncConvProg.Model.Tool.Mill;

namespace CncConvProg.Model.ToolMachine
{
    /// <summary>
    /// Classe Aux per Inserire Codice per creare file 
    /// per CutViewer
    /// </summary>
    public class CutViewerHelper
    {
        private static string FormatRsltString(string str)
        {
            return str.Replace(",", "#");
        }
        public static string PrintInitialToolPosition(double iniX, double iniY, double iniZ)
        {
            var r = "(FROM/" + iniX + "," + iniY + "," + iniZ + ")";
            return FormatRsltString(r);
        }

        /// <summary>
        /// Formatta i numeri in modo che il decimale sia punto e non virgola.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string FormatNumber(double number)
        {
            var o = number.ToString().Replace(",", ".");
            return o;
        }

        public static string PrintStockBlock(double lunghezza, double altezza, double spessore, double origineX, double origineY, double origineZ)
        {
            var r = "(STOCK/BLOCK," + FormatNumber(lunghezza) + "," + FormatNumber(altezza) + "," + FormatNumber(spessore) + "," + FormatNumber(origineX) + "," + FormatNumber(origineY) +
                   "," + FormatNumber(origineZ) + ")";

            return FormatRsltString(r);
        }

        public static string PrintColor(Color color)
        {
            return "(COLOR," + color.R + "," + color.G + "," + color.B + ")";
        }
        public static string PrintTool(Tool.Utensile tool)
        {
            var rslt = string.Empty;

            if (tool is FresaCandela)
            {
                var f = tool as FresaCandela;
                rslt = PrintMill(f.Diametro, 0, f.Diametro * 2, 0);
                rslt += "\n";
                rslt += PrintColor(Colors.Yellow);
            }
            if (tool is FresaSpianare)
            {
                var f = tool as FresaSpianare;
                rslt = PrintMill(f.Diametro, f.RaggioInserto, f.Altezza, 45);
                rslt += "\n";
                rslt += PrintColor(Colors.CadetBlue);
            }
            if (tool is Centrino)
            {
                var f = tool as Centrino;
                rslt = PrintCenterDrill(f.Diametro, 118, f.Diametro * 3, f.Diametro * 3, 60, f.Diametro * 7);
                rslt += "\n";
                rslt += PrintColor(Colors.DarkCyan);

            }
            if (tool is Punta)
            {
                var f = tool as Punta;
                rslt = PrintDrill(f.Diametro, 118, f.Diametro * 7);
                rslt += "\n";
                rslt += PrintColor(Colors.Green);

            }

            if (tool is Svasatore)
            {
                var f = tool as Svasatore;
                rslt = PrintChamferTool(f.Diametro * 2, 45, f.Diametro * 2, f.Diametro);
                rslt += "\n";
                rslt += PrintColor(Colors.GreenYellow);

            }

            return FormatRsltString(rslt);
        }

        private static string PrintChamferTool(double diameter, double angle, double altezza, double lunghezzaAngolo)
        {
            return "(TOOL/CHAMFER," + FormatNumber(diameter) + "," + FormatNumber(angle) + "," + FormatNumber(altezza) + "," + FormatNumber(lunghezzaAngolo) + ")";
        }
        private static string PrintCenterDrill(double diameter1, double angle1, double height1, double diameter2, double angle2, double height2)
        {
            return "(TOOL/CDRILL," + FormatNumber(diameter1) + "," + FormatNumber(angle1) + "," + FormatNumber(height1) + "," + FormatNumber(diameter2) + "," + FormatNumber(angle2) + "," + FormatNumber(height2) + ")";
        }

        private static string PrintDrill(double diameter, double angle, double height)
        {
            return "(TOOL/DRILL," + FormatNumber(diameter) + "," + FormatNumber(angle) + "," + FormatNumber(height) + ")";
        }
        private static string PrintMill(double diameter, double cornerRadius, double height, double taperAngle)
        {
            return "(TOOL/MILL," + FormatNumber(diameter) + "," + FormatNumber(cornerRadius) + "," + FormatNumber(height) + ", " + FormatNumber(taperAngle) + ")";
        }
    }
}
