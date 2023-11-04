using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RegexWpf
{
    public class ValidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool and true)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            return new SolidColorBrush(Colors.Red)
            {
                Opacity = 0.5,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}