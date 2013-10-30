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

            //Application initialisation
            AutoMapperConfiguration.Configure();
            Database.SetInitializer<DataContext>(new DataContextInitialiser());
            Database.SetInitializer<MembershipContext>(new MembershipContextInitialiser());

            //Security
            CustomPrincipal customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);
            base.OnStartup(e);

            //Setup Main Window
            MainWindow window = new MainWindow();

            // Create the ViewModel to which 
            // the main window binds.
            var mainWindowVm = new MainWindowViewModel();

            // When the ViewModel asks to be closed, 
            // close the window.
            EventHandler handler = null;
            handler = delegate
            {
                mainWindowVm.RequestClose -= handler;
                window.Close();
            };
            mainWindowVm.RequestClose += handler;

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = mainWindowVm;

            window.Show();
        }
    }
}
