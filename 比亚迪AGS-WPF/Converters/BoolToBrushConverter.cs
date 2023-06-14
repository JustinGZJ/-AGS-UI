using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace 比亚迪AGS_WPF.Converters;

// bool to brush converter


public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, 
        object parameter, CultureInfo culture)
    {
        var brush = parameter != null ? new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)parameter)) : null;
        return (bool)value ? brush : Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, 
        object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}