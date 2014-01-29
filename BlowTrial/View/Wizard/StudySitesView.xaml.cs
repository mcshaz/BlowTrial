using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BlowTrial.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class StudySitesView : UserControl
    {
        public StudySitesView()
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
