using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;

namespace HandyControl.Controls;

// 反射创建对象编辑器, 用于编辑复杂对象
public class ObjectEditor: PropertyEditorBase
{
    public override FrameworkElement CreateElement(PropertyItem propertyItem)
    {
        return new DynamicPropertyGrid();
    }

    public override DependencyProperty GetDependencyProperty()
    {
       return FrameworkElement.DataContextProperty;
    }
}

public class DynamicPropertyGrid : DataGrid
{
    public DynamicPropertyGrid()
    {
        this.AutoGenerateColumns = false;
        this.DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        this.Columns.Clear();
        if (e.NewValue != null)
        {
            Type type = e.NewValue.GetType();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                DataGridColumn column = CreateColumnForProperty(prop);
                if (column != null)
                {
                    this.Columns.Add(column);
                }
            }
            this.ItemsSource = new[] { e.NewValue };
        }
    }

    private DataGridColumn CreateColumnForProperty(PropertyInfo prop)
    {
        if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(string) || prop.PropertyType == typeof(double) || prop.PropertyType == typeof(bool))
        {
            var column = new DataGridTextColumn()
            {
                Header = prop.Name,
                Binding = new Binding(prop.Name)
            };
            return column;
        }
        // 这里可以根据需要添加更多类型的处理
        return null;
    }
}
