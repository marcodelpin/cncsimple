using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using CncConvProg.View.AuxClass;


namespace CncConvProg.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private DateTime _appStart;

        public App()
        {
            Startup += Application_Startup;
            Exit += Application_Exit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _appStart = DateTime.Now;
            var cultureName = Thread.CurrentThread.CurrentCulture.Name;
            var xmlLang = XmlLanguage.GetLanguage(cultureName);
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(xmlLang));

            Bootstrapper.InitializeIoc();

            var vw = new ShellWiew();

            vw.Show();

        }

        private void Application_Exit(object sender, EventArgs e)
        {
#if RELEASE
#endif
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }
    }
}
