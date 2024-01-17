using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace 比亚迪AGS_WPF.WpfExtensions;

public static class DataGridExtensions
{
    public static readonly DependencyProperty AutoScrollProperty =
        DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(DataGridExtensions), new PropertyMetadata(false, AutoScrollPropertyChanged));

    public static bool GetAutoScroll(DataGrid dataGrid)
    {
        return (bool)dataGrid.GetValue(AutoScrollProperty);
    }

    public static void SetAutoScroll(DataGrid dataGrid, bool value)
    {
        dataGrid.SetValue(AutoScrollProperty, value);
    }

    private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DataGrid dataGrid)
        {
            dataGrid.Loaded += (s, e) =>
            {
                var incc = dataGrid.Items as INotifyCollectionChanged;
                if (incc != null)
                {
                    incc.CollectionChanged += (s, e) =>
                    {
                        if (e.Action == NotifyCollectionChangedAction.Add)
                        {
                            var newItem = e.NewItems[e.NewItems.Count - 1];
                            dataGrid.ScrollIntoView(newItem);
                            dataGrid.SelectedItem = newItem;
                            dataGrid.Focus();
                        }
                    };
                }
            };
        }
    }
}