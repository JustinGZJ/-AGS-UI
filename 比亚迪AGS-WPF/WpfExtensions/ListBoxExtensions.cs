﻿using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace 比亚迪AGS_WPF.WpfExtensions;

public static class ListBoxExtensions
{
    public static readonly DependencyProperty AutoScrollProperty =
        DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(ListBoxExtensions), new PropertyMetadata(false, AutoScrollPropertyChanged));

    public static bool GetAutoScroll(ListBox listView)
    {
        return (bool)listView.GetValue(AutoScrollProperty);
    }

    public static void SetAutoScroll(ListBox listView, bool value)
    {
        listView.SetValue(AutoScrollProperty, value);
    }

    private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListBox listView)
        {
            listView.Loaded += (s, e) =>
            {
                var incc = CollectionViewSource.GetDefaultView(listView.ItemsSource) as INotifyCollectionChanged;
                if (incc != null)
                {
                    incc.CollectionChanged += (s, e) =>
                    {
                        if (e.Action == NotifyCollectionChangedAction.Add)
                        {
                            var newItem = e.NewItems[e.NewItems.Count - 1];
                            listView.ScrollIntoView(newItem);
                            listView.SelectedItem = newItem;
                        }
                    };
                }
            };
        }
    }
}