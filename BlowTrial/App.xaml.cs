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
            base.OnStartup(e);

            //testfor and display starup wizard
            var wizard = new GetAppSettingsWizard(new GetAppSettingsWizardViewModel());
            wizard.Show();
            return;
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
    }
}
