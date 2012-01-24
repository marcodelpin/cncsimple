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
            try
            {
                WriteReport("Success Ending");
            }
            catch (Exception)
            {

            }
#endif
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            //if (!System.Diagnostics.Debugger.IsAttached)
            {
#if RELEASE
                try
                {
                    WriteReport(e.ExceptionObject.ToString() + " " + e.ToString());

                    Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

                    Current.Shutdown(1);

                }
                catch (Exception)
                {

                }
#endif

            }
        }
        private void WriteReport(string error)
        {
            if (!RptSen.ReportGenerator.PcId.CheckInternetConnection.IsConnectedToInternet()) return;

            using (var s = new ServiceReference1.Service1Client())
            {
                var timeSpan = DateTime.Now.Subtract(_appStart).TotalMinutes;

                var mi = RptSen.ReportGenerator.PcId.GetPcId();

                s.Rep(timeSpan, mi.MacAddress, error, "CncConvProg");
            }

        }
    }
}
