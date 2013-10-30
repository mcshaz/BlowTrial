using BlowTrial.ViewModel;
namespace BlowTrial.View
{
    public partial class ParticipantUpdateView : System.Windows.Window
    {
        public ParticipantUpdateView(ParticipantUpdateViewModel model)
        {
            this.DataContext = model;
            Closing += model.OnClosingWindow;
            InitializeComponent();
        }

    }
}