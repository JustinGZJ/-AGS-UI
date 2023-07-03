using System.Collections;
using System.Collections.Generic;
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
            AutoGenerateColumns = true,
            CanUserAddRows = false,
            CanUserDeleteRows = false,
        };
        
        // 返回DataGrid作为编辑器
        return dataGrid;
    }

    public override DependencyProperty GetDependencyProperty() => DataGrid.ItemsSourceProperty;
}