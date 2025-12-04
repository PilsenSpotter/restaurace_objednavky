using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KebabBrnoPOS.App;

public class AvailabilityBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush PrimaryBrush = new(Color.FromRgb(34, 197, 94));
    private static readonly SolidColorBrush DisabledBrush = new(Color.FromRgb(75, 85, 99));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool available && available ? PrimaryBrush : DisabledBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
