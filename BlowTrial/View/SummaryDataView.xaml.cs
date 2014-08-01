using BlowTrial.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace BlowTrial.View
{
    public partial class SummaryDataView : System.Windows.Controls.UserControl
    {
        public SummaryDataView()
        {
            InitializeComponent();
        }

        private void Participants_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            AddPartCols();
            ((ParticipantSummaryViewModel)e.NewValue).ColHeaders.CollectionChanged += ColHeaders_CollectionChanged;
        }

        void ColHeaders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
 	        AddPartCols();
        }
        void AddPartCols()
        {
            ParticipantSummaryViewModel part = ((DataSummaryViewModel)DataContext).ParticipantData;
            DataTemplate cellTemplate = (DataTemplate)Resources["CellTemplate"];
            int colIndex = Participants.Columns.Count;
            /*
            if (colIndex==0)
            {
                var col = new DataGridTextColumn
                {
                    Header = "Stage",
                    Binding = new Binding
                    {
                        Path = new System.Windows.PropertyPath("RowHeader"),
                        Mode = BindingMode.OneWay
                    }
                };
                //
            }
             * */
            for(;colIndex <= part.ColHeaders.Count;colIndex++)
            {
                /*
                DataGridTemplateColumn templateColumn = new DataGridTemplateColumn
                    {
                        Header = part.ColHeaders[colIndex - 1],
                        CellTemplate = cellTemplate
                    };
                FrameworkElementFactory cellTemplateFactory = new FrameworkElementFactory(typeof(Grid));
                Binding binding = new Binding
                    {
                        Path = new System.Windows.PropertyPath("SummaryCells[" + (colIndex - 1) + "]"),
                        Mode = BindingMode.OneWay
                    };
                cellTemplateFactory.SetBinding(Grid.DataContextProperty, binding);
                cellTemplate.VisualTree = cellTemplateFactory;
                cellTemplate.Seal();
                */

                /*    
                 <DataGridTextColumn.CellStyle>
                    <Style TargetType="DataGridCell">
                        <Setter Property="ToolTip" Value="{Binding Name}" />
                    </Style>
                </DataGridTextColumn.CellStyle>
                 */

                var col = new DataGridTextColumn
                {
                    Header = part.ColHeaders[colIndex-1], //should potentially make the number of pre-existing cols a property
                    Binding = new Binding
                    {
                        Path = new System.Windows.PropertyPath("SummaryCells[" + (colIndex - 1) + "].Count"),
                        Mode = BindingMode.OneWay
                    },
                    CellStyle = new Style { }
                };
                Style style = new Style(typeof(DataGridCell));
                Binding toolTipBinding = new Binding
                {
                    Path = new System.Windows.PropertyPath("SummaryCells[" + (colIndex - 1) + "].IdList"),
                    Mode = BindingMode.OneWay
                };
                style.Setters.Add(new Setter(DataGridCell.ToolTipProperty, toolTipBinding));
                col.CellStyle = style;
                Participants.Columns.Add(col);
                
            }
        }
    }
}