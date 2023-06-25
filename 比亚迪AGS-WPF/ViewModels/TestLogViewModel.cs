using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace 比亚迪AGS_WPF.ViewModels
{
    public  class TestLogViewModel
    {
        public ObservableCollection<TestLog> TestLogs { get; set; } = new();

        public TestLogViewModel()
        {
            WeakReferenceMessenger.Default.Register<TestLog>(this, ((recipient, message) =>
            {
                var log = new TestLog()
                {
                    Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Type = message.Type,
                    Log = message.Log,
                    Level = message.Level
                };
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TestLogs.Add(log);
                    if (TestLogs.Count > 200)
                        TestLogs.RemoveAt(0);
                });
            }));
        }
    }
}
