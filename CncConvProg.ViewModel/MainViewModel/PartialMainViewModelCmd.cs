using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CncConvProg.Model;
using CncConvProg.Model.ConversationalStructure;
using CncConvProg.Model.ConversationalStructure.Lavorazioni.Common;
using CncConvProg.ViewModel.AuxViewModel;
using CncConvProg.ViewModel.AuxViewModel.TreeViewModel;
using CncConvProg.ViewModel.MVVM_Library;
using GongSolutions.Wpf.DragDrop;

/*
 * todo : devo fare viewModel incapsulato
 * 
 * non posso fare molte proprietà direttamente nel vm principale
 * 
 * devo fare in modo che quando cambio model mi cambiano direttamente anche tutte le regioni ad esso collegato.
 * 
 */

namespace CncConvProg.ViewModel.MainViewModel
{
    public partial class MainViewModel
    {
      

        //public string ImagePath
        //{
        //    get
        //    {
        //        return @"I:\Canc\Scan\2.pdf";
        //    }
        //}

        //public string ImageSource
        //{
        //    get
        //    {
        //        // se è pdf creo immagine temp e passo indirizzo..
        //        return GetImagePath(@"I:\Canc\Scan\2.pdf");
        //    }
        //}

        /// <summary>
        /// Ghostscript engine
        /// </summary>
        //   private readonly GS _mGs = new GS();

        //private void TableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        //{
        //    var l = sender as TableView;

        //    if (l.AutoFilterRowData.IsFocused) return;

        //    //  var selectedRow = l.FocusedRowHandle;

        //    // detailGrid.RefreshData();

        //    //detailGrid.RefreshRow();

        //    //   l.SelectRow(selectedRow);

        //    pictureBox.ImageLocation = string.Empty;

        //    imageControl.Source = null;
        //    //var worker = new BackgroundWorker();
        //    //worker.DoWork += (sender1, e1) =>
        //    //                     {
        //    // la domanda è , posso gestire tutto dalla parte view ? si
        //    var vm = ((ProductionQueueViewModel)DataContext);
        //    if (vm == null || vm.DettaglioSelezionato == null || vm.DettaglioSelezionato.Articolo == null) return;

        // ok
        /// <summary>
        /// Cercare Immagine Compatibile.( il codice articolo è contenuto per intero nel nome immagine)
        /// Se pdf creare anteprima
        /// Altrimenti ok
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        //private string GetImagePath(string imagePath)
        //{
        //    // Controllo che il percorso sia stringa valida e il file esista
        //    if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath)) return string.Empty;

        //    if (Path.GetExtension(imagePath).ToLower() == ".PDF".ToLower())
        //    {
        //        try
        //        {
        //            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp\\";

        //            if (!Directory.Exists(folderPath))
        //                Directory.CreateDirectory(folderPath);

        //            // todo _ ripulire cartella temp..

        //            var filename = folderPath + Guid.NewGuid() + "tempImage.png";

        //            // _mGs = new GS();

        //            var image = _mGs.PdfToPng(imagePath, 1, 50);
        //            image.Save(filename);
        //            image.Dispose();
        //            //  _mGs.Dispose();

        //            return filename;

        //        }
        //        catch (Exception ex)
        //        {

        //            Console.WriteLine("Errore nella conversione file pdf: " + ex.Message);
        //        }

        //    }

        //    else
        //    {
        //        return imagePath;
        //    }

        //    return string.Empty;
        //}

        //    // se non c'è percorso predefinito
        //    if (string.IsNullOrWhiteSpace(filePath))
        //    {
        //        // provo a prendere codice articolo e cercarlo nella cartella base

        //        // controllo che folder image base non sia vuoto è che la directory esista
        //        if (string.IsNullOrWhiteSpace(imageFolderBase) || !Directory.Exists(imageFolderBase)) return;

        //        var fileName = vm.DettaglioSelezionato.Articolo.CodiceArticolo;

        //        // cerco nella directory base e sotto directory il file 
        //        var filePaths = Directory.GetFiles(imageFolderBase, fileName + ".*", SearchOption.AllDirectories);

        //        if (filePaths.Count() == 0)
        //        {
        //            pictureBox.ImageLocation = string.Empty;

        //            imageControl.Source = null;

        //            return;
        //        }

        //        var rsltPath = filePaths.FirstOrDefault();

        //        if (rsltPath == null) return; // non ho trovato niente

        //        if (Path.GetExtension(rsltPath).ToLower() == ".PDF".ToLower())
        //        {
        //            try
        //            {
        //                // todo , incapsulare in metodo e usare try cartch
        //                // todo . controllare cartella see esiste c temp!
        //                var tempFolder = PreferenceUtility.GetTempFolder();

        //                // todo _ ripulire cartella temp..
        //                var imageJpg = Guid.NewGuid() + "image2.png";

        //                // _mGs = new GS();

        //                var image = _mGs.PdfToPng(rsltPath, 1, 50);
        //                image.Save(tempFolder + imageJpg);
        //                image.Dispose();
        //                //  _mGs.Dispose();

        //                var logo = new BitmapImage();
        //                logo.BeginInit();
        //                logo.UriSource = new Uri(tempFolder + imageJpg, UriKind.Absolute);
        //                logo.EndInit();

        //                imageControl.Source = logo.CloneCurrentValue();

        //                pictureBox.ImageLocation = tempFolder + imageJpg;
        //            }
        //            catch (Exception ex)
        //            {

        //                Console.WriteLine("Errore nella conversione file pdf: " + ex.Message);
        //            }

        //        }

        //        else
        //        {
        //            // se è immagine jpg
        //            pictureBox.ImageLocation = rsltPath;

        //            var logo = new BitmapImage();
        //            logo.BeginInit();
        //            logo.UriSource = new Uri(rsltPath, UriKind.Absolute);
        //            logo.EndInit();

        //            imageControl.Source = logo;

        //        }
        //    }

        //    //                     };

        //    //worker.RunWorkerAsync();
        //}
        //     /*
        //     *  public class MainViewModel : CncConvProg.ViewModel.MainViewModel.MainViewModel
        //{
        //    public MainViewModel(IModalDialogService modalDialogService, IMessageBoxService messageBoxService)
        //        : base(modalDialogService, messageBoxService)
        //    {
        //    }

      
    }

    /// <summary>
    /// </summary>
    public class ToolCycleViewModel : ViewModelBase
    {
        private int _toolNumber;
        public int ToolNumber { get { return _toolNumber; } set { _toolNumber = value; OnPropertyChanged("ToolNumber"); } }

        private TimeSpan _toolTotalTime;
        public TimeSpan ToolTotalTime
        {
            get { return _toolTotalTime.Subtract(TimeSpan.FromMilliseconds(_toolTotalTime.Milliseconds)); }
            set
            {
                _toolTotalTime = value;
                OnPropertyChanged("ToolTotalTime");
            }
        }

        private double _toolTotalWear;
        public double ToolTotalWear { get { return _toolTotalWear; } set { _toolTotalWear = value; OnPropertyChanged("ToolTotalWear"); } }
    }

    public class VoceReportTreeview : TreeViewItemViewModel
    {
        private string _voce;
        public string Voce
        {
            get { return _voce; }
            set
            {
                _voce = value;
                OnPropertyChanged("Voce");
            }
        }

        private TimeSpan _timeSpan;
        public TimeSpan Time
        {
            get { return _timeSpan.Subtract(TimeSpan.FromMilliseconds(_timeSpan.Milliseconds)); }
            set
            {
                _timeSpan = value;
                OnPropertyChanged("Time");
            }
        }
        public VoceReportTreeview(string label, TreeViewItemViewModel parent)
            : base(label, parent)
        {
            IsExpanded = true;

            Voce = label;
            if (parent != null)
                parent.Children.Add(this);
        }


    }

    public static class FormatTimeHelper
    {
        public static string GetFormattedPhaseNumber(int phaseNumber)
        {
            return "# " + phaseNumber + " - ";
        }
        public static TimeSpan FormatTime(int minutes)
        {
            var timeSpan = TimeSpan.FromMinutes(minutes);

            return FormatTime(timeSpan);
        }

        public static TimeSpan FormatTime(TimeSpan timeSpan)
        {
            return timeSpan.Subtract(TimeSpan.FromMilliseconds(timeSpan.Milliseconds));
        }



        /// <summary>
        /// Ritorna stringa 
        /// </summary>
        /// <param name="singleLoadingTime"></param>
        /// <param name="stockQuantity"></param>
        /// <returns></returns>
        public static string FormatStockTime(TimeSpan singleLoadingTime, int stockQuantity)
        {
            return " " + singleLoadingTime.Subtract(TimeSpan.FromMilliseconds(singleLoadingTime.Milliseconds)) + " x " + stockQuantity + "pcs = ";
        }

        /// <summary>
        /// Ritorna stringa 
        /// </summary>
        /// <returns></returns>
        public static string FormatNumberIterationTime(int numberOperation, int minutes, string suffisso)
        {
            var s = string.Empty;
            if (numberOperation > 1)
                s = "s";

            return " " + FormatTime(minutes) + " x " + numberOperation + " " + suffisso + s + " = ";
        }
    }

}



