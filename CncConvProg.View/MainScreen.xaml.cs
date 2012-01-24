using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using CncConvProg.ViewModel.MainViewModel;
using ICSharpCode.AvalonEdit.Highlighting;

namespace CncConvProg.View
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : UserControl
    {
        public MainScreen()
        {
            #region Load CustomHiglighting
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(ShellWiew).Assembly.GetManifestResourceStream("CncConvProg.View.CustomHighlighting.xshd"))
            //using (Stream s = typeof(DefaultView).Assembly.GetManifestResourceStream("CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);

            #endregion


            InitializeComponent();
            textEditor1.SyntaxHighlighting = customHighlighting;

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*
             * Non sempre il source dell'image è quello che voglio io
             */
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                var imageCtrl = sender as Image;

                if (imageCtrl == null) return;

                var path = imageCtrl.Tag.ToString();

                if (!File.Exists(path)) return;

                var p = new Process
                {
                    StartInfo = { FileName = path }
                };

                p.Start();
            }

        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void Image_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
