using BlowTrial.ViewModel;
namespace BlowTrial.View
{
    public partial class ParticipantUpdateView : System.Windows.Window
    {
        public ParticipantUpdateView(ParticipantUpdateViewModel model)
        {        
            DataContextChanged += ParticipantUpdateView_DataContextChanged;
            Closed += ParticipantUpdateView_Closed;
            DataContext = model;
            InitializeComponent();
        }

        void ParticipantUpdateView_Closed(object sender, System.EventArgs e)
        {
            Closing -= ((ParticipantUpdateViewModel)DataContext).OnClosingWindow;
            Closed -= ParticipantUpdateView_Closed;
        }

        void ParticipantUpdateView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                Closing -= ((ParticipantUpdateViewModel)e.OldValue).OnClosingWindow;
            }
            if (e.NewValue != null) 
            { 
                Closing += ((ParticipantUpdateViewModel)e.NewValue).OnClosingWindow;
            }
        }
    }
}