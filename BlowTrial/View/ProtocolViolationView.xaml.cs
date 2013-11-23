using BlowTrial.ViewModel;
namespace BlowTrial.View
{
    public partial class ProtocolViolationView : System.Windows.Window
    {
        public ProtocolViolationView(ProtocolViolationViewModel model)
        {        
            Closed += ProtocolViolationView_Closed;
            DataContext = model;
            InitializeComponent();
        }

        void ProtocolViolationView_Closed(object sender, System.EventArgs e)
        {
            Closing -= ((ProtocolViolationViewModel)DataContext).OnClosingWindow;
            Closed -= ProtocolViolationView_Closed;
        }
    }
}