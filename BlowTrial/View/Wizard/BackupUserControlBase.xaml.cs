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


            var col = cols[0];
            col.Width = cloudDirectoriesGrid.ActualWidth/2;
            col.MinWidth = col.ActualWidth;

            double width = 0;
            for (int i = 0; i < 2; i++)
            {
                width += cols[i].ActualWidth;
            }

            col = cols[2];
            col.MinWidth = col.ActualWidth;
            col.Width = cloudDirectoriesGrid.ActualWidth - width -5;
        }
    }
}
