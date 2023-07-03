using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;

namespace HandyControl.Controls;
public class ListPropertyEditor : PropertyEditorBase
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem)
    {
        // 创建一个DataGrid作为编辑器
        var dataGrid = new DataGrid
        {
            AutoGenerateColumns = false,
            CanUserAddRows = true,
            CanUserDeleteRows = true,
            ItemsSource = (IEnumerable) propertyItem.Value
        };
        
        // 遍历List中的对象并为其创建相应的列
        foreach (var item in dataGrid.ItemsSource)
        {
            var column = new DataGridTextColumn
            {
                Header = item.GetType().Name,
                Binding = new Binding($"[{dataGrid.Items.IndexOf(item)}]")
            };
            
            dataGrid.Columns.Add(column);
        }
        
        // 返回DataGrid作为编辑器
        return dataGrid;
    }

    public override DependencyProperty GetDependencyProperty() => DataGrid.ItemsSourceProperty;
}