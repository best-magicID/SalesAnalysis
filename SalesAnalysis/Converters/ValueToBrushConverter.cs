using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SalesAnalysis.Converters
{
    class ValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int input;
            try
            {
                DataGridCell dataGridCell = (DataGridCell)value;
                //if(dataGridCell.DataContext)
                dataGridCell.Background = new SolidColorBrush(Colors.Yellow);

                //System.Data.DataRowView rowView = (System.Data.DataRowView)dataGridCell.DataContext;
                //input = (int)rowView.Row.ItemArray[dataGridCell.Column.DisplayIndex];
            }
            catch { }
            //catch (InvalidCastException e)
            //{
            //    return DependencyProperty.UnsetValue;
            //}

            return Brushes.White;

            //switch (input)
            //{
            //    case 1: return Brushes.Red;
            //    case 2: return Brushes.White;
            //    case 3: return Brushes.Blue;
            //    default: return DependencyProperty.UnsetValue;
            //}
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
