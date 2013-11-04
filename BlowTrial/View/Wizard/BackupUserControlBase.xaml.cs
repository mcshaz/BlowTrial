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
            double width = 0;
            for (int i=1; i<cols.Count;i++)
            {
                width += cols[i].ActualWidth;
            }
            var col = cloudDirectoriesGrid.Columns[0];
            col.Width = cloudDirectoriesGrid.ActualWidth - width;
            col.MinWidth = col.ActualWidth;
        }
    }
}
