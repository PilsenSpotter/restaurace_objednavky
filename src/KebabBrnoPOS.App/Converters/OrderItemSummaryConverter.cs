using System;
using System.Globalization;
using System.Windows.Data;
using KebabBrnoPOS.App.Models;

namespace KebabBrnoPOS.App;

public class OrderItemSummaryConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not OrderItemModel item) return string.Empty;
        var option = string.IsNullOrWhiteSpace(item.SelectedOption) ? string.Empty : $" · {item.SelectedOption}";
        return $"{item.Quantity}x {item.Item.Name}{option}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
