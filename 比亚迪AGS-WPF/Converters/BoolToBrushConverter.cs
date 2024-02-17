using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace 比亚迪AGS_WPF.Converters;

// bool to brush converter
public class RunningStatusColorConverter : IValueConverter
{
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // 根据运行转台返回不同的颜色  运行 绿色  停止 红色 报警 黄色 未知 灰色
        var state= (string)value;
        switch (state)
        {
            case "运行":
                return Brushes.Lime;
            case "停止":
                return Brushes.Red;
            case "报警":
                return Brushes.Red;
            case "待机":
                return Brushes.Gold;
            case "未知":
                return Brushes.Gray;
            default:
                return Brushes.Gray;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class TestResultColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
            // 根据测试结果显示不同的颜色  OK:绿色 NG:红色 未知 灰色
            var state= (string)value;
            return state switch
            {
                "OK" => Brushes.Lime,
                "NG" => Brushes.Red,
                "PASS" => Brushes.Lime,
                "FAIL" => Brushes.Red,
                _ => Brushes.Gray
            };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

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