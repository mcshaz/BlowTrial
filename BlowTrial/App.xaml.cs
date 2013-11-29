using BlowTrial.Domain.Providers;
using BlowTrial.Security;
using BlowTrial.View;
using BlowTrial.ViewModel;
using DabTrial.Models;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Linq;
using System.IO;
using BlowTrial.Helpers;
using log4net;
using log4net.Appender;
using log4net.Config;

namespace BlowTrial
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            // This code is used to test the app when using other cultures.
            //
            //System.Threading.Thread.CurrentThread.CurrentCulture =
            //    System.Threading.Thread.CurrentThread.CurrentUICulture =
            //        new System.Globalization.CultureInfo("it-IT");


            // Ensure the current culture passed into bindings is the OS culture.
            // By default, WPF uses en-US as the culture, regardless of the system settings.
            //
            FrameworkElement.LanguageProperty.OverrideMetadata(
              typeof(FrameworkElement),
              new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Set data directory
            string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\BlowTrial";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            AppDomain.CurrentDomain.SetData("DataDirectory", baseDir);

            //Application initialisation
            AutoMapperConfiguration.Configure();

            //Security
            CustomPrincipal customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

            ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            //test if wizard needs to run
            bool displayWizard = (BlowTrialDataService.GetBackupDetails().BackupData == null);

            if (!displayWizard)
            {
                using (var t = new TrialDataContext())
                {
                    displayWizard = !t.StudyCentres.Any();
                }
            }
            if (displayWizard && !DisplayAppSettingsWizard())
            {
                Shutdown(259);
                return;
            }
            
            // Create the ViewModel to which 
            // the main window binds.
            var mainWindowVm = new MainWindowViewModel();
            MainWindow window = new MainWindow(mainWindowVm);

            // When the ViewModel asks to be closed, 
            // close the window.
            EventHandler handler = null;
            handler = delegate
            {
                window.Close();
                if (!window.IsLoaded) //in case user cancelled
                {
                    mainWindowVm.RequestClose -= handler;
                }
            };
            mainWindowVm.RequestClose += handler;

            window.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if app settings were succesfully added to the appropriate repositories</returns>
        static bool DisplayAppSettingsWizard()
        {
            //testfor and display starup wizard
            var wizard = new GetAppSettingsWizard();
            GetAppSettingsViewModel appSettings = new GetAppSettingsViewModel();
            wizard.DataContext = appSettings;
            EventHandler wizardHandler = null;
            wizardHandler = delegate
            {
                wizard.Close();
                wizard = null;
                appSettings.RequestClose -= wizardHandler;
            };
            appSettings.RequestClose += wizardHandler;
            wizard.ShowDialog();
            return !appSettings.WasCancelled; // user cancel
        }
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#if DEBUG
            throw e.Exception;
#else
            Log.Debug("Application_DispatcherUnhandledException", e.Exception);
            e.Handled = true;
#endif
        }
    }
}
