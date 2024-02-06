using System;
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
            
            // 记录上次滚动的时间
            DateTime lastScrollTime = DateTime.MinValue;

            dataGrid.Loaded += (s, e) =>
            {
                var incc = dataGrid.Items as INotifyCollectionChanged;
                incc.CollectionChanged += (s, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            if (e.Action == NotifyCollectionChangedAction.Add)
                            {
                                // 检查是否已经过了0.5秒
                                if ((DateTime.Now - lastScrollTime).TotalSeconds >= 0.5)
                                {
                                    var newItem = e.NewItems[e.NewItems.Count - 1];
                                    dataGrid.ScrollIntoView(newItem);
                                    dataGrid.SelectedItem = newItem;
                                    dataGrid.Focus();

                                    // 更新上次滚动的时间
                                    lastScrollTime = DateTime.Now;
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    });
                };
            };
        }
    }
}