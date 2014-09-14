using BlowTrial.ViewModel;
using System.Windows;
using System.Windows.Controls;
namespace BlowTrial.View
{
    public partial class AllViolationsView : System.Windows.Controls.UserControl
    {
        public AllViolationsView()
        {
            InitializeComponent();
        }
        //http://stackoverflow.com/questions/8331940/how-can-i-get-a-listview-gridviewcolumn-to-fill-the-remaining-space-in-my-grid#14674830
        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_loaded) { return; }
            ListView _ListView = sender as ListView;
            GridView _GridView = _ListView.View as GridView;
            var _ActualWidth = _ListView.ActualWidth - SystemParameters.VerticalScrollBarWidth-10;
            int lastcol = _GridView.Columns.Count-1;
            for (int i = 0; i < lastcol; i++)
            {
                _ActualWidth -= _GridView.Columns[i].ActualWidth;
            }
            _GridView.Columns[lastcol].Width = _ActualWidth;
            int textWidth = (int)(_ActualWidth/7);
            foreach (var v in ((AllViolationsViewModel)DataContext).AllViolations)
            {
                v.AbbrevLength = textWidth;
            }

        }
        bool _loaded;
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
            ListView_SizeChanged(sender, null);
        }

    }
}