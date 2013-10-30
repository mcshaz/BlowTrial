using BlowTrial.Domain.Providers;
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
        public MainWindow()
        {
            InitializeComponent();
            //Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}