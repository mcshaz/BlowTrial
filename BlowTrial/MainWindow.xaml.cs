using BlowTrial.Domain.Providers;
using BlowTrial.Helpers;
using BlowTrial.View;
using BlowTrial.ViewModel;
using DabTrial.Models;
using System;
using System.Data.Entity;
using System.Windows;

namespace BlowTrial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow(MainWindowViewModel model)
        {
            DataContext = model;
            Closing += model.OnClosing;
            Closed += MainWindow_Closed;
            EnsureAppIsSetup();
            InitializeComponent();
        }

        static void EnsureAppIsSetup()
        {
            var backup = BlowTrialDataService.GetBackupDetails();
            if (backup.BackupData == null)
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
            }
        }

        void MainWindow_Closed(object sender, System.EventArgs e)
        {
            Closing -= ((MainWindowViewModel)DataContext).OnClosing;
            Closed -= MainWindow_Closed;
        }
    }
}