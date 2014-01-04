using BlowTrial.Domain.Providers;
using BlowTrial.Security;
using BlowTrial.View;
using BlowTrial.ViewModel;
using BlowTrial.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Linq;
using System.IO;
using BlowTrial.Helpers;
using log4net;
using log4net.Appender;
using System.Deployment.Application;
using BlowTrial.Infrastructure.Extensions;

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
#if !DEBUG
            if (_log == null)
            {
                _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
            string deployVersion = GetClickOnceVersion();
            if (deployVersion != null)
            {
                ThreadContext.Properties["deploymentVersion"] = deployVersion;
            }
            this.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            log4net.Config.XmlConfigurator.Configure();
#endif
            base.OnStartup(e);

            //Set data directory
            string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\BlowTrial";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            AppDomain.CurrentDomain.SetData("DataDirectory", baseDir);

            //Application initialisation
            try
            { 
                AutoMapperConfiguration.Configure();
            }
            catch (Exception ex)
            {
                _log.Error("App_AutomapperConfigurationException", ex);
                MessageBox.Show("An error has occured with automapper. An error has been logged, but this file will have to be attached and emailed to the application developer");
                throw;
            }

            //Security
            CustomPrincipal customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

            ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            //test if wizard needs to run

            try
            {
                using (var db = new TrialDataContext())
                {
                    db.Participants.Any();
                }
                using (var db = new MembershipContext())
                {
                    db.CloudDirectories.Any();
                }
            }
            catch (Exception ex)
            {
                _log.Error("App_FirstDatabaseAccessException", ex);
                MessageBox.Show("An error has occured trying to access the database - this may be because of access permissions or the database pasword may have changed. An error has been logged, but this file will have to be attached and emailed to the application developer");
                throw;
            }

            var backDetails = BlowTrialDataService.GetBackupDetails();
            bool displayWizard = (backDetails.BackupData == null);
            if (!displayWizard  && backDetails.BackupData.IsBackingUpToCloud)
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
        static ILog _log;
        void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _log.Error("Application_DispatcherUnhandledException", e.Exception);
            if (_log.IsErrorEnabled)
            {
                try
                {
                    string fn = GetLogOutputPath();
                    if (fn != null)
                    {
                        MoveLogFileToCloud(fn);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Application_DispatcherUnhandled_ImplementationException", ex);
                    return;
                }
            }
        }
        public static string GetClickOnceVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var myVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                return string.Format("v{0}.{1}.{2}.{3}",
                    myVersion.Major,
                    myVersion.Minor,
                    myVersion.Build,
                    myVersion.Revision);
            }
            return null;
        }
        static string GetLogOutputPath()
        {
            BackupDataSet bak = BlowTrialDataService.GetBackupDetails();
            if (bak != null)
            {
                string dir = bak.CloudDirectories.FirstOrDefault();
                if (dir != null)
                {
                    // if environment.machinename not working due to duplicate names, could try
                    //var searcher = new System.Management.ManagementObjectSearcher("select * from " + Key);
                    // key = Win32_DiskDrive or Win32_Processor
                    return Path.Combine(dir, StringExtensions.GetSafeFilename(string.Format("log_{0}.txt", Environment.MachineName)));
                }
            }
            return null;
        }
        static bool MoveLogFileToCloud(string cloudFileName)
        {
            foreach (var appender in LogManager.GetRepository().GetAppenders())
            {
                var fileAppender = appender as FileAppender;
                if (fileAppender != null)
                {
                    string fn = fileAppender.File;
                    try 
                    {
                        File.Copy(fn, cloudFileName,true);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("MoveLogFileToCloud", ex);
                        return false;
                    }
                    return true;
                }
            }
            return false;

        }
    }
}
