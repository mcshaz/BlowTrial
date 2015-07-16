using BlowTrial.ViewModel;
namespace BlowTrial.View
{
    public partial class ParticipantUpdateView : System.Windows.Window
    {
        public ParticipantUpdateView(ParticipantProgressViewModel model)
        {        
            DataContextChanged += ParticipantUpdateView_DataContextChanged;
            Closed += ParticipantUpdateView_Closed;
            DataContext = model;
            InitializeComponent();
        }

        void ParticipantUpdateView_Closed(object sender, System.EventArgs e)
        {
            Closing -= ((ParticipantProgressViewModel)DataContext).OnClosingWindow;
            Closed -= ParticipantUpdateView_Closed;
        }

        void ParticipantUpdateView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                Closing -= ((ParticipantProgressViewModel)e.OldValue).OnClosingWindow;
            }
            if (e.NewValue != null) 
            { 
                Closing += ((ParticipantProgressViewModel)e.NewValue).OnClosingWindow;
            }
        }

        private void outcomeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}