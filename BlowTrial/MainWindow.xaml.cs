using BlowTrial.Domain.Providers;
using BlowTrial.ViewModel;
using DabTrial.Models;
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
            InitializeComponent();
        }

        void MainWindow_Closed(object sender, System.EventArgs e)
        {
            Closing -= ((MainWindowViewModel)DataContext).OnClosing;
            Closed -= MainWindow_Closed;
        }
    }
}