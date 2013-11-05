using BlowTrial.ViewModel;
using System.Windows;

namespace BlowTrial.View
{

    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class BackupUserControlBase : System.Windows.Controls.UserControl
    {
        public BackupUserControlBase()
        {
            InitializeComponent();
        }
        private void CloudDirectoriesGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
           
            var cols = cloudDirectoriesGrid.Columns;

            double buttonWidth = cols[0].ActualWidth;

            var col = cols[1];
            col.MinWidth = col.ActualWidth;
            col.Width = cloudDirectoriesGrid.ActualWidth-buttonWidth;
        }
    }
}
