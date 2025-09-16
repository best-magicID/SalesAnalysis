using System.Windows.Data;
using System.Windows.Media;

namespace SalesAnalysis.Converters
{
    class ValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double doubleValue && doubleValue > 40000)
            {
                return Brushes.Yellow;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
