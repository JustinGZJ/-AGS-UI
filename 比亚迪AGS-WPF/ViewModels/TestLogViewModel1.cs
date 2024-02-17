using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using 比亚迪AGS_WPF.DataObject;
using 比亚迪AGS_WPF.Utils;

namespace 比亚迪AGS_WPF.ViewModels;

public partial class TestLogViewModel : ObservableRecipient, IRecipient<TestLog>
{
    public TestLogViewModel()
    {
        IsActive = true;
    }

    /// <summary>
    ///     日志
    /// </summary>
    public ObservableCollection<TestLog> TestLogs { get; set; } = new();
    
    
    public TestItemsViewModel TestItemsViewModel { get; set; } = new();
    
    public PlotViewModel PlotViewModel { get; set; } = new();

    public void Receive(TestLog message)
    {
        var log = new TestLog
        {
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Type = message.Type,
            Log = message.Log,
            Level = message.Level
        };
        Application.Current.Dispatcher.Invoke(() =>
        {
            TestLogs.Add(log);
            if (TestLogs.Count >= 200)
                TestLogs.RemoveAt(TestLogs.Count - 1);
        });
    }
}