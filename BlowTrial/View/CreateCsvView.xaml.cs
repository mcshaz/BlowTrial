using System.Diagnostics;
using System.Windows.Navigation;

namespace BlowTrial.View
{

    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class CreateCsvView : System.Windows.Controls.UserControl
    {
        public CreateCsvView()
        {
            InitializeComponent();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
