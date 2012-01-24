using System;
using System.Collections.Generic;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using CncConvProg.ViewModel;
using ICSharpCode.AvalonEdit.Highlighting;

/*
 * code processor ,
 */

namespace CncConvProg.View
{
    /// <summary>
    /// Interaction logic for ProgramSimulationView.xaml
    /// </summary>
    public partial class ProgramSimulationView : UserControl
    {
        private ProgramSimulationViewModel _viewModel;

        public ProgramSimulationView()
        {
            #region Load CustomHiglighting
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("CncConvProg.View.CustomHighlighting.xshd"))
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

            Loaded += ProgramSimulationView_Loaded;
        }

        void ProgramSimulationView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as ProgramSimulationViewModel;

            if (_viewModel == null) return;

            _viewModel.OnRefreshCalled += ViewModelOnRefreshCalled;

            _viewModel.Refresh();//delete
        }

        void ViewModelOnRefreshCalled(object sender, EventArgs e)
        {
            //while (mainViewport.Children.Count > 0)
            //{
            //   // mainViewport.Children.Remove(mainViewport.Children.FirstOrDefault()); // in questa maniera rimuove anche luci..
            //}

            mainViewport.Children.Add(_viewModel.ModelVisual);
        }


    }
}
