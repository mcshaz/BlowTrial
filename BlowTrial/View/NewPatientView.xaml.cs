using System.Windows.Controls;
namespace BlowTrial.View
{
    public partial class NewPatientView : System.Windows.Controls.UserControl
    {
        public NewPatientView()
        {
            InitializeComponent();
        }

        //stackoverflow.com/questions/4997596/how-can-i-set-the-width-of-a-datagridcolumn-to-fit-contents-auto-but-comple
        private void abnormalitiesDataGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var col = abnormalitiesDataGrid.Columns[0];
            col.Width = abnormalitiesDataGrid.ActualWidth;
            col.MinWidth = col.ActualWidth;
        }
    }
}